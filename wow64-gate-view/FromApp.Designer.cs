namespace wow64_gate_view
{
    partial class FromApp
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listView_traceInfo = new System.Windows.Forms.ListView();
            this.columnHeader_Pid = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_Tid = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_FnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_Argc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_Argv = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_CallStackSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_CallStack = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip_traceInfo = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem_add = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_del = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label_ServiceName = new System.Windows.Forms.Label();
            this.textBox_ServiceName = new System.Windows.Forms.TextBox();
            this.listBox_ServiceNameFilter = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cLeanAllToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.button_ServiceNameFilterAdd = new System.Windows.Forms.Button();
            this.textBox_ProcessID = new System.Windows.Forms.TextBox();
            this.label_ProcessID = new System.Windows.Forms.Label();
            this.button_start = new System.Windows.Forms.Button();
            this.contextMenuStrip_traceInfo.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView_traceInfo
            // 
            this.listView_traceInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader_Pid,
            this.columnHeader_Tid,
            this.columnHeader_FnName,
            this.columnHeader_Argc,
            this.columnHeader_Argv,
            this.columnHeader_CallStackSize,
            this.columnHeader_CallStack});
            this.listView_traceInfo.ContextMenuStrip = this.contextMenuStrip_traceInfo;
            this.listView_traceInfo.FullRowSelect = true;
            this.listView_traceInfo.HideSelection = false;
            this.listView_traceInfo.Location = new System.Drawing.Point(118, 46);
            this.listView_traceInfo.Name = "listView_traceInfo";
            this.listView_traceInfo.Size = new System.Drawing.Size(1185, 619);
            this.listView_traceInfo.TabIndex = 0;
            this.listView_traceInfo.UseCompatibleStateImageBehavior = false;
            this.listView_traceInfo.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader_Pid
            // 
            this.columnHeader_Pid.Text = "Pid";
            this.columnHeader_Pid.Width = 77;
            // 
            // columnHeader_Tid
            // 
            this.columnHeader_Tid.Text = "Tid";
            this.columnHeader_Tid.Width = 68;
            // 
            // columnHeader_FnName
            // 
            this.columnHeader_FnName.Text = "Fn Name";
            this.columnHeader_FnName.Width = 161;
            // 
            // columnHeader_Argc
            // 
            this.columnHeader_Argc.Text = "Argc";
            // 
            // columnHeader_Argv
            // 
            this.columnHeader_Argv.Text = "Argv";
            this.columnHeader_Argv.Width = 144;
            // 
            // columnHeader_CallStackSize
            // 
            this.columnHeader_CallStackSize.Text = "CallStackSize";
            this.columnHeader_CallStackSize.Width = 117;
            // 
            // columnHeader_CallStack
            // 
            this.columnHeader_CallStack.Text = "CallStack";
            this.columnHeader_CallStack.Width = 492;
            // 
            // contextMenuStrip_traceInfo
            // 
            this.contextMenuStrip_traceInfo.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip_traceInfo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_add,
            this.toolStripMenuItem_del,
            this.cleanAllToolStripMenuItem});
            this.contextMenuStrip_traceInfo.Name = "contextMenuStrip_traceInfo";
            this.contextMenuStrip_traceInfo.Size = new System.Drawing.Size(142, 76);
            this.contextMenuStrip_traceInfo.Text = "HHHHH";
            // 
            // toolStripMenuItem_add
            // 
            this.toolStripMenuItem_add.Name = "toolStripMenuItem_add";
            this.toolStripMenuItem_add.Size = new System.Drawing.Size(141, 24);
            this.toolStripMenuItem_add.Text = "添加";
            this.toolStripMenuItem_add.Click += new System.EventHandler(this.toolStripMenuItem_add_Click);
            // 
            // toolStripMenuItem_del
            // 
            this.toolStripMenuItem_del.Name = "toolStripMenuItem_del";
            this.toolStripMenuItem_del.Size = new System.Drawing.Size(141, 24);
            this.toolStripMenuItem_del.Text = "删除";
            this.toolStripMenuItem_del.Click += new System.EventHandler(this.toolStripMenuItem_del_Click);
            // 
            // cleanAllToolStripMenuItem
            // 
            this.cleanAllToolStripMenuItem.Name = "cleanAllToolStripMenuItem";
            this.cleanAllToolStripMenuItem.Size = new System.Drawing.Size(141, 24);
            this.cleanAllToolStripMenuItem.Text = "Clean All";
            this.cleanAllToolStripMenuItem.Click += new System.EventHandler(this.cleanAllToolStripMenuItem_Click);
            // 
            // label_ServiceName
            // 
            this.label_ServiceName.AutoSize = true;
            this.label_ServiceName.Location = new System.Drawing.Point(9, 9);
            this.label_ServiceName.Name = "label_ServiceName";
            this.label_ServiceName.Size = new System.Drawing.Size(103, 15);
            this.label_ServiceName.TabIndex = 1;
            this.label_ServiceName.Text = "Service Name";
            // 
            // textBox_ServiceName
            // 
            this.textBox_ServiceName.Location = new System.Drawing.Point(118, 6);
            this.textBox_ServiceName.Name = "textBox_ServiceName";
            this.textBox_ServiceName.Size = new System.Drawing.Size(177, 25);
            this.textBox_ServiceName.TabIndex = 2;
            // 
            // listBox_ServiceNameFilter
            // 
            this.listBox_ServiceNameFilter.ContextMenuStrip = this.contextMenuStrip1;
            this.listBox_ServiceNameFilter.FormattingEnabled = true;
            this.listBox_ServiceNameFilter.ItemHeight = 15;
            this.listBox_ServiceNameFilter.Location = new System.Drawing.Point(12, 46);
            this.listBox_ServiceNameFilter.Name = "listBox_ServiceNameFilter";
            this.listBox_ServiceNameFilter.Size = new System.Drawing.Size(100, 619);
            this.listBox_ServiceNameFilter.TabIndex = 3;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.cLeanAllToolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(146, 52);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(145, 24);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // cLeanAllToolStripMenuItem1
            // 
            this.cLeanAllToolStripMenuItem1.Name = "cLeanAllToolStripMenuItem1";
            this.cLeanAllToolStripMenuItem1.Size = new System.Drawing.Size(145, 24);
            this.cLeanAllToolStripMenuItem1.Text = "CLean All";
            this.cLeanAllToolStripMenuItem1.Click += new System.EventHandler(this.cLeanAllToolStripMenuItem1_Click);
            // 
            // button_ServiceNameFilterAdd
            // 
            this.button_ServiceNameFilterAdd.Location = new System.Drawing.Point(301, 6);
            this.button_ServiceNameFilterAdd.Name = "button_ServiceNameFilterAdd";
            this.button_ServiceNameFilterAdd.Size = new System.Drawing.Size(75, 32);
            this.button_ServiceNameFilterAdd.TabIndex = 4;
            this.button_ServiceNameFilterAdd.Text = "添加";
            this.button_ServiceNameFilterAdd.UseVisualStyleBackColor = true;
            this.button_ServiceNameFilterAdd.Click += new System.EventHandler(this.button_ServiceNameFilterAdd_Click);
            // 
            // textBox_ProcessID
            // 
            this.textBox_ProcessID.Location = new System.Drawing.Point(467, 6);
            this.textBox_ProcessID.Name = "textBox_ProcessID";
            this.textBox_ProcessID.Size = new System.Drawing.Size(100, 25);
            this.textBox_ProcessID.TabIndex = 5;
            // 
            // label_ProcessID
            // 
            this.label_ProcessID.AutoSize = true;
            this.label_ProcessID.Location = new System.Drawing.Point(382, 9);
            this.label_ProcessID.Name = "label_ProcessID";
            this.label_ProcessID.Size = new System.Drawing.Size(79, 15);
            this.label_ProcessID.TabIndex = 6;
            this.label_ProcessID.Text = "ProcessID";
            // 
            // button_start
            // 
            this.button_start.Location = new System.Drawing.Point(746, 8);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(75, 32);
            this.button_start.TabIndex = 7;
            this.button_start.Text = "开始";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button_start_Click);
            // 
            // FromApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1315, 677);
            this.Controls.Add(this.button_start);
            this.Controls.Add(this.label_ProcessID);
            this.Controls.Add(this.textBox_ProcessID);
            this.Controls.Add(this.button_ServiceNameFilterAdd);
            this.Controls.Add(this.listBox_ServiceNameFilter);
            this.Controls.Add(this.textBox_ServiceName);
            this.Controls.Add(this.label_ServiceName);
            this.Controls.Add(this.listView_traceInfo);
            this.MaximizeBox = false;
            this.Name = "FromApp";
            this.Text = "procemon-view";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip_traceInfo.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView_traceInfo;
        private System.Windows.Forms.Label label_ServiceName;
        private System.Windows.Forms.TextBox textBox_ServiceName;
        private System.Windows.Forms.ListBox listBox_ServiceNameFilter;
        private System.Windows.Forms.Button button_ServiceNameFilterAdd;
        private System.Windows.Forms.TextBox textBox_ProcessID;
        private System.Windows.Forms.Label label_ProcessID;
        private System.Windows.Forms.ColumnHeader columnHeader_Pid;
        private System.Windows.Forms.ColumnHeader columnHeader_Tid;
        private System.Windows.Forms.ColumnHeader columnHeader_FnName;
        private System.Windows.Forms.ColumnHeader columnHeader_Argc;
        private System.Windows.Forms.ColumnHeader columnHeader_Argv;
        private System.Windows.Forms.ColumnHeader columnHeader_CallStackSize;
        private System.Windows.Forms.ColumnHeader columnHeader_CallStack;
        private System.Windows.Forms.Button button_start;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_traceInfo;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_add;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_del;
        private System.Windows.Forms.ToolStripMenuItem cleanAllToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cLeanAllToolStripMenuItem1;
    }
}

