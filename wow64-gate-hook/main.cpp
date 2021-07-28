#include <iostream>
#include <iomanip>
#include <algorithm>
#include <thread>
#include <vector>
#include <atomic>
#include <functional>

#include <WinSock2.h>
#include <WS2tcpip.h>
#include <stdio.h>

#include <phnt_windows.h>
#include <phnt.h>
#include <Shion.h>


#pragma comment(lib,"ws2_32.lib")  

#pragma pack(push)
#pragma pack(1)
struct WOW64_NT_API_TRACE_INFO {
	HANDLE thread_id;
	HANDLE process_id;
	DWORD service_id;
	char nt_api_name[40];
	USHORT argc;
	DWORD argv[30];
	USHORT trace_size;
	DWORD trace[50];
};
#pragma pack(pop)

static std::atomic_bool g_is_exit = false;
static PSHION_HOOK g_wow64_gate_hook = NULL;
static std::atomic_bool g_lock = false;
static const int THREAD_ARRAYS_MAX = 233;
static HANDLE g_allowlist_threads[THREAD_ARRAYS_MAX] = { NULL };
static sockaddr_in g_sin;
static std::thread g_thread_gate_log;

bool wow64_gate_hook_start();
bool wow64_gate_hook_end();


static void simple_lock(std::atomic_bool& b) {
	bool n = false;
	while (!b.compare_exchange_strong(n, true)) {
		n = false;
	}
}

static void simple_unlock(std::atomic_bool& b) {
	b = false;
}

static bool thread_allowlist_enter(HANDLE n) {
	simple_lock(g_lock);

	for (int i = 0; i < THREAD_ARRAYS_MAX; i++) {
		if (g_allowlist_threads[i] == NULL) {
			g_allowlist_threads[i] = n;
			simple_unlock(g_lock);
			return true;
		}
	}
	simple_unlock(g_lock);
	return false;
}

static bool thread_allowlist_leave(HANDLE n) {
	simple_lock(g_lock);

	for (int i = 0; i < THREAD_ARRAYS_MAX; i++) {
		if (g_allowlist_threads[i] == n) {
			g_allowlist_threads[i] = NULL;
			simple_unlock(g_lock);
			return true;
		}
	}
	simple_unlock(g_lock);
	return false;
}

static bool thread_allowlist_has(HANDLE n) {
	simple_lock(g_lock);
	for (int i = 0; i < THREAD_ARRAYS_MAX; i++) {
		if (g_allowlist_threads[i] == n) {
			simple_unlock(g_lock);
			return true;
		}
	}
	simple_unlock(g_lock);
	return false;
}


template<class T>
class SimpleQueue {
private:
	T* datas = nullptr;
	unsigned int put_index = 0;
	unsigned int pop_index = 0;
	unsigned int size = 0;
	std::atomic<unsigned int> capacity = 0;

	enum { DEFAULT_SIZE = 3000 };

	std::atomic_bool lock = false;

public:
	SimpleQueue(unsigned int size = DEFAULT_SIZE) {
		this->size = size;
		this->capacity = size;
		this->datas = new T[this->capacity];
	}

	~SimpleQueue() {
		if (this->datas) {
			delete[] this->datas;
		}
	}

	bool put(T & data) {
		while (!capacity) {
			return false;
		}

		simple_lock(this->lock);
		
		bool b = false;

		if (capacity > 0) {
			this->datas[this->put_index++] = data;
			if (this->put_index >= this->size) {
				this->put_index = 0;
			}
			capacity--;
			b = true;
		}
		
		simple_unlock(this->lock);

		return b;
	}

	bool pop(T& n) {
		while (capacity == this->size) {
			return false;
		}

		simple_lock(this->lock);
		
		if (capacity < this->size) {
			n = this->datas[this->pop_index++];
			if (this->pop_index >= this->size) {
				this->pop_index = 0;
			}
			capacity++;
		}
		
		simple_unlock(this->lock);

		return true;
	}


};


static SimpleQueue<WOW64_NT_API_TRACE_INFO> g_wow64_nt_api_trace_info_queue;
static SimpleQueue<DWORD> g_service_allowlist;
bool get_api_trace_info(WOW64_NT_API_TRACE_INFO& info, PVOID stack_ebp);
BOOL WINAPI wow64_gate(DWORD service_id, PVOID stack_ebp, PNTSTATUS pStatus) {
	HANDLE thread_id = NtCurrentThreadId();
	if (g_is_exit || thread_allowlist_has(thread_id)) {
		return FALSE;
	}

	thread_allowlist_enter(thread_id);

	WOW64_NT_API_TRACE_INFO trace_info = {};
	auto is_ok = get_api_trace_info(trace_info, stack_ebp);

	if (is_ok) {
		trace_info.thread_id = thread_id;
		trace_info.process_id = NtCurrentProcessId();
		trace_info.service_id = service_id;

		g_wow64_nt_api_trace_info_queue.put(trace_info);
	}

	thread_allowlist_leave(thread_id);
	return FALSE;
}

PVOID __declspec(naked) wow64_gate_hook() {
	__asm {
		push ebp;
		mov ebp, esp;
		pushfd;

		push eax;
		push ebx;
		push ecx;
		push edx;
		push esi;
		push edi;
		
		push eax;
		lea eax, dword ptr [esp];
		push eax;
		push ebp;
		push dword ptr [eax];
		call wow64_gate;
		test eax, eax;
		je _zero;
		mov eax, dword ptr[esp];
		add esp, 4;
		
		pop edi;
		pop esi;
		pop edx;
		pop ecx;
		pop ebx;
		add esp, 4;

		popfd;
		mov esp, ebp;
		pop ebp;
		ret;

	_zero:
		add esp, 4;
		pop edi;
		pop esi;
		pop edx;
		pop ecx;
		pop ebx;
		pop eax;
		popfd;
		mov esp, ebp;
		pop ebp;
		push g_wow64_gate_hook;
		add dword ptr [esp], 8;
		add esp, 4;
		jmp dword ptr [esp - 4];
	}
}

static bool EnumModule(std::function<BOOL(PLDR_DATA_TABLE_ENTRY)> callback) {

	PPEB pPeb = NtCurrentPeb();
	PPEB_LDR_DATA pLdr = pPeb->Ldr;
	if (pLdr) {
		auto pListEntryStart = pLdr->InMemoryOrderModuleList.Flink;
		auto pListEntryEnd = pListEntryStart->Blink;
		do {
			auto pLdrDataEntry = (PLDR_DATA_TABLE_ENTRY)CONTAINING_RECORD(pListEntryStart, LDR_DATA_TABLE_ENTRY, InMemoryOrderLinks);
			pListEntryStart = pListEntryStart->Flink;

			if (callback(pLdrDataEntry))return true;
		} while (pListEntryStart != pListEntryEnd);
	}
	return false;
}

#define MakePointer(t, p, offset) ((t)((PBYTE)(p) + offset))

BOOL RIsValidPEFormat(_In_ LPVOID lpPeModuleBuffer) {
	if (NULL == lpPeModuleBuffer)
		return FALSE;

	PIMAGE_DOS_HEADER pImageDosHeader = (PIMAGE_DOS_HEADER)lpPeModuleBuffer;
	if (IMAGE_DOS_SIGNATURE != pImageDosHeader->e_magic)
		return FALSE;

	PIMAGE_NT_HEADERS pImageNtHeader = MakePointer(PIMAGE_NT_HEADERS, pImageDosHeader, pImageDosHeader->e_lfanew);
	if (IMAGE_NT_SIGNATURE != pImageNtHeader->Signature)
		return FALSE;

	if (!(IMAGE_FILE_EXECUTABLE_IMAGE & pImageNtHeader->FileHeader.Characteristics))
		return FALSE;

	if (sizeof(IMAGE_OPTIONAL_HEADER) != pImageNtHeader->FileHeader.SizeOfOptionalHeader)
		return FALSE;

	if (IMAGE_NT_OPTIONAL_HDR32_MAGIC != pImageNtHeader->OptionalHeader.Magic &&
		IMAGE_NT_OPTIONAL_HDR64_MAGIC != pImageNtHeader->OptionalHeader.Magic)
		return FALSE;

	return TRUE;
}



HMODULE get_module_by_address(PVOID address) {
	HMODULE h = NULL;
	EnumModule([&](PLDR_DATA_TABLE_ENTRY pLdrDataEntry) {
		
		PIMAGE_DOS_HEADER pImageDosHeader = (PIMAGE_DOS_HEADER)pLdrDataEntry->DllBase;

		if (!RIsValidPEFormat(pImageDosHeader)) {
			return false;
		}

		PIMAGE_NT_HEADERS pImageNtHeader = MakePointer(PIMAGE_NT_HEADERS, pImageDosHeader, pImageDosHeader->e_lfanew);

		if ((DWORD)address > (DWORD)pLdrDataEntry->DllBase &&
			(DWORD)address < (DWORD)pLdrDataEntry->DllBase + pImageNtHeader->OptionalHeader.SizeOfImage) {
			h = (HMODULE)pLdrDataEntry->DllBase;
			return true;
		}

		return false;
	});
	return h;
}

PSTR get_api_name(HMODULE h_module, PVOID api_entry) {
	PIMAGE_DOS_HEADER pImageDosHeader = (PIMAGE_DOS_HEADER)h_module;

	if (!RIsValidPEFormat(pImageDosHeader)) {
		return NULL;
	}

	PIMAGE_NT_HEADERS pImageNtHeader = MakePointer(PIMAGE_NT_HEADERS, pImageDosHeader, pImageDosHeader->e_lfanew);

	if (0 == pImageNtHeader->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_EXPORT].VirtualAddress ||
		0 == pImageNtHeader->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_EXPORT].Size)
		return NULL;
	PIMAGE_EXPORT_DIRECTORY pImageExportDirectory =
		MakePointer(PIMAGE_EXPORT_DIRECTORY, h_module, pImageNtHeader->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_EXPORT].VirtualAddress);
	PDWORD pAddressTable = MakePointer(PDWORD, h_module, pImageExportDirectory->AddressOfFunctions);
	PDWORD pFuncNameTable = MakePointer(PDWORD, h_module, pImageExportDirectory->AddressOfNames);
	PWORD pOrdinalTable = MakePointer(PWORD, h_module, pImageExportDirectory->AddressOfNameOrdinals);
	for (DWORD i = 0; i < pImageExportDirectory->NumberOfNames; ++i) {
		DWORD address_offset = pAddressTable[pOrdinalTable[i]];
		if ((PBYTE)api_entry - (PBYTE)h_module  ==  address_offset) {
			return (char*)h_module + pFuncNameTable[i];
		}
	}

	return NULL;
}

bool get_api_trace_info(WOW64_NT_API_TRACE_INFO& info, PVOID stack_ebp) {

	__try {
		PBYTE nt_ret_address = *((PBYTE*)stack_ebp + 1);

		PBYTE api_address = NULL;
		USHORT argc = 0;
		switch (*nt_ret_address) {
			case 0xC2: {//win10
				argc = *(PUSHORT)(nt_ret_address + 1) / 4;
				api_address = nt_ret_address - 0xC;
				if (*api_address == 0xB8) {
					break;
				}
				return false;
			}break;
			case 0x83: {//win7
				argc = *(PUSHORT)(nt_ret_address + 4) / 4;
				api_address = nt_ret_address - 0x12;
				if (*api_address == 0xB8) {
					break;
				}
				api_address = nt_ret_address - 0x15;
				if (*api_address == 0xB8) {
					break;
				}
				return false;
			}break;
			default: {
				return false;
			}break;
		}

		
		
		info.argc = argc;
		PDWORD nt_argv_address = (PDWORD)((PBYTE)stack_ebp + 0xC);
		for (int i = 0; i < argc && i < 30; i++) {
			info.argv[i] = nt_argv_address[i];
		}

		
		PDWORD stack_ebp_start = *(PDWORD*)stack_ebp;
		for (int i = 0; i < 50; i++) {
			if (stack_ebp_start == NULL) {
				break;
			}

			MEMORY_BASIC_INFORMATION mem_info = {};
			SIZE_T size = 0;

			auto status = NtQueryVirtualMemory(NtCurrentProcess(), stack_ebp_start, MemoryBasicInformation, &mem_info, sizeof(mem_info), &size);
			if (!NT_SUCCESS(status) || sizeof(MEMORY_BASIC_INFORMATION) != size) {
				break;
			}

			if (mem_info.State != MEM_COMMIT ||
				(mem_info.AllocationProtect != PAGE_READONLY &&
				mem_info.AllocationProtect != PAGE_READWRITE)) {
				break;
			}

			info.trace[i] = stack_ebp_start[1];

			info.trace_size = i + 1;

			stack_ebp_start = *(PDWORD*)stack_ebp_start;
		}

		auto h_module = get_module_by_address(api_address);
		if (h_module != NULL) {
			auto api_name = get_api_name(h_module, api_address);
			if (api_name == NULL) {
				for (int i = 0; i < info.trace_size; i++) {
					auto n = info.trace[i];
					if (*(PSHORT)(n - 0x6) == 0x15FF) {
						api_address = **(PBYTE**)(n - 0x4);
						h_module = get_module_by_address(api_address);
						if (h_module != NULL) {
							api_name = get_api_name(h_module, api_address);
							break;
						}
						
					}
				}
			}
			if (api_name != NULL) {
				RtlMoveMemory(info.nt_api_name, api_name, strnlen_s(api_name, 40 - 1) + 1);
			}
		}
	}
	__except (EXCEPTION_EXECUTE_HANDLER) {

	}
	
	return true;
}



char g_send_buf[0x1000] = { 0 };//LPVOID lpParameter
DWORD wow64_nt_api_trace_log() {
	auto thread_id = NtCurrentThreadId();

	thread_allowlist_enter(thread_id);

	//g_sin.sin_family = AF_INET;
	//g_sin.sin_port = htons(2333);
	//inet_pton(AF_INET, "127.0.0.1", &g_sin.sin_addr);

	//SOCKET sock = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);

	while (!g_is_exit) {
		WOW64_NT_API_TRACE_INFO trace_info = {};
		
		if (!g_wow64_nt_api_trace_info_queue.pop(trace_info)) {
			continue;
		}

		auto h = FindWindowW(NULL, L"procemon-view");

		if (h != NULL && trace_info.nt_api_name[0] != 0) {
			COPYDATASTRUCT copy;
			copy.cbData = sizeof(WOW64_NT_API_TRACE_INFO);
			copy.dwData = 7;
			copy.lpData = &trace_info;
			SendMessageA(h, WM_COPYDATA, 0, (LPARAM)&copy);

			/*sprintf_s(g_send_buf, "[wow64 hook]serviceId: %d fn name: %s argc: %d", trace_info.service_id, trace_info.nt_api_name, trace_info.argc);

			COPYDATASTRUCT copy;
			copy.cbData = strnlen_s(g_send_buf, sizeof(g_send_buf)) + 1;
			copy.dwData = 7;
			copy.lpData = g_send_buf;
			SendMessageA(h, WM_COPYDATA, 0, (LPARAM)&copy);

			RtlZeroMemory(g_send_buf, sizeof(g_send_buf));
			char buf[255] = { 0 };
			strcat_s(g_send_buf, "[wow64 hook]{");
			for (int i = 0; i < trace_info.argc; i++) {
				sprintf_s(buf, "0x%X, ", trace_info.argv[i]);
				strcat_s(g_send_buf, buf);
			}
			strcat_s(g_send_buf, "}");

			copy.cbData = strnlen_s(g_send_buf, sizeof(g_send_buf)) + 1;
			copy.dwData = 7;
			copy.lpData = g_send_buf;
			SendMessageA(h, WM_COPYDATA, 0, (LPARAM)&copy);


			RtlZeroMemory(g_send_buf, sizeof(g_send_buf));
			strcat_s(g_send_buf, "[wow64 hook]{");
			for (int i = 0; i < trace_info.trace_size; i++) {
				sprintf_s(buf, "0x%X, ", trace_info.trace[i]);
				strcat_s(g_send_buf, buf);
			}
			strcat_s(g_send_buf, "}");

			copy.cbData = strnlen_s(g_send_buf, sizeof(g_send_buf)) + 1;
			copy.dwData = 7;
			copy.lpData = g_send_buf;
			SendMessageA(h, WM_COPYDATA, 0, (LPARAM)&copy);*/

			//if (sock != INVALID_SOCKET) {
				//sendto(sock, g_send_buf, strnlen_s(g_send_buf, sizeof(g_send_buf)) + 1, 0, (sockaddr*)&g_sin, sizeof(g_sin));
			//}
		}
	}
	thread_allowlist_leave(thread_id);
	return 0;
}

void wow64_gate_thread_start() {
	g_is_exit = false;
	g_thread_gate_log = std::thread(wow64_nt_api_trace_log);
	g_thread_gate_log.detach();
}

void wow64_gate_thread_end() {
	g_is_exit = true;
	g_thread_gate_log.join();
}

bool wow64_gate_hook_start() {

	g_wow64_gate_hook = new SHION_HOOK();

	thread_allowlist_enter(NtCurrentThreadId());

	auto is_ok = ShionHook(NtCurrentTeb()->WOW32Reserved, wow64_gate_hook, g_wow64_gate_hook);

	thread_allowlist_leave(NtCurrentThreadId());

	return is_ok;
}

bool wow64_gate_hook_end() {
	thread_allowlist_enter(NtCurrentThreadId());

	auto is_ok = ShionRestore(g_wow64_gate_hook);
	if (is_ok) {
		delete g_wow64_gate_hook;
	}

	thread_allowlist_leave(NtCurrentThreadId());

	return is_ok;
}

bool socket_init() {
	WORD socketVersion = MAKEWORD(2, 2);
	WSADATA wsaData;
	if (WSAStartup(socketVersion, &wsaData) != 0) {
		return false;
	}
	return true;
}



BOOL WINAPI DllMain(HINSTANCE hInstance, DWORD fdwReason, PVOID pvReserved) {

	switch (fdwReason)
	{
	case DLL_PROCESS_ATTACH: {
		//socket_init();
		wow64_gate_hook_start();
		wow64_gate_thread_start();
		break;
	}
	case DLL_PROCESS_DETACH: {
		//crash
		wow64_gate_thread_end();
		wow64_gate_hook_end();
		break;
	}
	case DLL_THREAD_ATTACH:
		break;
	case DLL_THREAD_DETACH:
		break;
	}

	return TRUE;
}