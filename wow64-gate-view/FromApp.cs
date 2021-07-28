using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wow64_gate_view
{
    public partial class FromApp : Form
    {
        public FromApp()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private List<string> api_name_filters = new List<string>();
        private bool is_start = false;

        protected override void WndProc(ref Message m)
        {
            // Listen for operating system messages.
            switch (m.Msg)
            {
                case WM_COPYDATA:
                    if(!is_start)
                    {
                        break;
                    }

                    COPYDATASTRUCT cds = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(COPYDATASTRUCT));
                    WOW64_NT_API_TRACE_INFO trace_info = (WOW64_NT_API_TRACE_INFO)Marshal.PtrToStructure(cds.lpData, typeof(WOW64_NT_API_TRACE_INFO));

                    if(textBox_ProcessID.Text.Length != 0 && trace_info.process_id != Convert.ToInt32(textBox_ProcessID.Text))
                    {
                        break;
                    }

                    var api_name = trace_info.nt_api_name;
                    if (ApiNameFiltersHas(api_name))
                    {
                        break;
                    }
                    

                    var call_stacks = GetCallStackStringList(trace_info.process_id, trace_info.trace.SubArray(0, trace_info.trace_size));

                    var item = new ListViewItem(new string[] {
                        trace_info.process_id.ToString(),
                        trace_info.thread_id.ToString(),
                        api_name,
                        trace_info.argc.ToString(),
                        string.Join(",", trace_info.argv.SubArray(0,trace_info.argc).ArrayToString("X")),
                        trace_info.trace_size.ToString(),
                        string.Join(",",call_stacks),
                    });
                    listView_traceInfo.BeginUpdate();
                    listView_traceInfo.Items.Add(item);
                    listView_traceInfo.Items[listView_traceInfo.Items.Count - 1].EnsureVisible();
                    listView_traceInfo.EndUpdate();
                    break;
            }
            base.WndProc(ref m);
        }

        public bool ApiNameFiltersHas(string name)
        {
            foreach (var s in api_name_filters)
            {
                if (s.Last() == '*' && name.StartsWith(s.Substring(0, s.Length - 1)))
                {
                    return true;
                }
                if (s == name)
                {
                    return true;
                }
            }
            return false;
        }

        public List<String> GetCallStackStringList(int process_id, uint[] trace_arrays)
        {
            var list = new List<string>();
            var process = Process.GetProcessById(process_id);
            if(process.HasExited)
            {
                return list;
            }
            foreach(var trace in trace_arrays)
            {
                var n = trace.ToString("X");
                foreach (ProcessModule module in process.Modules)
                {
                    if(trace > module.BaseAddress.ToInt32() && trace < module.BaseAddress.ToInt32() + module.ModuleMemorySize)
                    {
                        n = string.Format("{0}+{1}", module.ModuleName, (trace - module.BaseAddress.ToInt32()).ToString("X"));
                        break;
                    }
                }
                list.Add(n);
            }
            return list;
        }



        private const int WM_COPYDATA = 0x4A;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct WOW64_NT_API_TRACE_INFO
        {
            public int thread_id;
            public int process_id;
            public int service_id;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
            public string nt_api_name;
            public ushort argc;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
            public int[] argv;
            public ushort trace_size;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
            public uint[] trace;
        };

        private void button_ServiceNameFilterAdd_Click(object sender, EventArgs e)
        {
            var text = textBox_ServiceName.Text;
            if(text.Length != 0)
            {
                api_name_filters.Add(text);
                listBox_ServiceNameFilter.Items.Add(text);
            }
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            if (!is_start)
            {
                button_start.Text = "停止";
                is_start = true;
            } 
            else
            {
                button_start.Text = "开始";
                is_start = false;
            }
        }

        private void toolStripMenuItem_add_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView_traceInfo.SelectedItems)
            {
                var api_name = item.SubItems[2].Text;
                api_name_filters.Add(api_name);
                listBox_ServiceNameFilter.Items.Add(api_name);
            }
            
        }

        private void toolStripMenuItem_del_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView_traceInfo.SelectedItems)
            {
                var api_name = item.SubItems[2].Text;
                api_name_filters.Remove(api_name);
                listBox_ServiceNameFilter.Items.Remove(api_name);
            }
        }

        private void cleanAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView_traceInfo.BeginUpdate();
            listView_traceInfo.Items.Clear();
            listView_traceInfo.EndUpdate();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(listBox_ServiceNameFilter.SelectedItems.Count > 0)
            {
                var item = listBox_ServiceNameFilter.SelectedItem;
                listBox_ServiceNameFilter.Items.Remove(item);
                api_name_filters.Remove(item.ToString());
            }
        }

        private void cLeanAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            listBox_ServiceNameFilter.Items.Clear();
            api_name_filters.Clear();
        }
    }
    public static class Extensions
    {
        public static T[] SubArray<T>(this T[] array, int offset, int length)
        {
            T[] result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }

        public static string[] ArrayToString(this int[] array, string format)
        {
            string[] result = new string[array.Length];
            for(var i = 0; i < array.Length; i++)
            {
                result[i] = array[i].ToString(format);
            }
            return result;
        }
    }
}
