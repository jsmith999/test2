namespace Conta {
    partial class frmMain {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FileExitMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.jumpToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CompaniesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ClientsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.projectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EmployeesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.depotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.theDataGridView = new System.Windows.Forms.DataGridView();
            this.theBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.DataAddNewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.DataUpdateMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.DataDeleteMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.theDetail = new Conta.DetailCtrl();
            this.mainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.theDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.theBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.jumpToToolStripMenuItem,
            this.toolStripMenuItem1,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(464, 25);
            this.mainMenu.TabIndex = 1;
            this.mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileExitMenu});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // FileExitMenu
            // 
            this.FileExitMenu.Name = "FileExitMenu";
            this.FileExitMenu.Size = new System.Drawing.Size(96, 22);
            this.FileExitMenu.Text = "E&xit";
            this.FileExitMenu.Click += new System.EventHandler(this.FileExitMenu_Click);
            // 
            // jumpToToolStripMenuItem
            // 
            this.jumpToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CompaniesMenu,
            this.ClientsMenu,
            this.projectsToolStripMenuItem,
            this.EmployeesMenu,
            this.depotToolStripMenuItem});
            this.jumpToToolStripMenuItem.Name = "jumpToToolStripMenuItem";
            this.jumpToToolStripMenuItem.Size = new System.Drawing.Size(76, 21);
            this.jumpToToolStripMenuItem.Text = "&Jump to...";
            // 
            // CompaniesMenu
            // 
            this.CompaniesMenu.Enabled = false;
            this.CompaniesMenu.Name = "CompaniesMenu";
            this.CompaniesMenu.Size = new System.Drawing.Size(141, 22);
            this.CompaniesMenu.Text = "&Companies";
            // 
            // ClientsMenu
            // 
            this.ClientsMenu.Name = "ClientsMenu";
            this.ClientsMenu.Size = new System.Drawing.Size(141, 22);
            this.ClientsMenu.Text = "C&lients";
            this.ClientsMenu.Click += new System.EventHandler(this.ClientsMenu_Click);
            // 
            // projectsToolStripMenuItem
            // 
            this.projectsToolStripMenuItem.Enabled = false;
            this.projectsToolStripMenuItem.Name = "projectsToolStripMenuItem";
            this.projectsToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.projectsToolStripMenuItem.Text = "&Projects";
            // 
            // EmployeesMenu
            // 
            this.EmployeesMenu.Name = "EmployeesMenu";
            this.EmployeesMenu.Size = new System.Drawing.Size(141, 22);
            this.EmployeesMenu.Text = "&Employees";
            this.EmployeesMenu.Click += new System.EventHandler(this.EmployeesMenu_Click);
            // 
            // depotToolStripMenuItem
            // 
            this.depotToolStripMenuItem.Enabled = false;
            this.depotToolStripMenuItem.Name = "depotToolStripMenuItem";
            this.depotToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.depotToolStripMenuItem.Text = "&Depot";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(52, 21);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.settingsToolStripMenuItem.Text = "&Settings";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(47, 21);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            // 
            // theDataGridView
            // 
            this.theDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.theDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.theDataGridView.Location = new System.Drawing.Point(0, 25);
            this.theDataGridView.Name = "theDataGridView";
            this.theDataGridView.Size = new System.Drawing.Size(464, 140);
            this.theDataGridView.TabIndex = 5;
            this.theDataGridView.SelectionChanged += new System.EventHandler(this.theDataGridView_SelectionChanged);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DataAddNewMenu,
            this.DataUpdateMenu,
            this.DataDeleteMenu});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(47, 21);
            this.toolStripMenuItem1.Text = "Data";
            // 
            // DataAddNewMenu
            // 
            this.DataAddNewMenu.Name = "DataAddNewMenu";
            this.DataAddNewMenu.Size = new System.Drawing.Size(152, 22);
            this.DataAddNewMenu.Text = "Add &new";
            this.DataAddNewMenu.Click += new System.EventHandler(this.DataAddNewMenu_Click);
            // 
            // DataUpdateMenu
            // 
            this.DataUpdateMenu.Name = "DataUpdateMenu";
            this.DataUpdateMenu.Size = new System.Drawing.Size(152, 22);
            this.DataUpdateMenu.Text = "&Update";
            // 
            // DataDeleteMenu
            // 
            this.DataDeleteMenu.Name = "DataDeleteMenu";
            this.DataDeleteMenu.Size = new System.Drawing.Size(152, 22);
            this.DataDeleteMenu.Text = "&Delete";
            // 
            // theDetail
            // 
            this.theDetail.BindingSource = null;
            this.theDetail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.theDetail.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.theDetail.Location = new System.Drawing.Point(0, 165);
            this.theDetail.Name = "theDetail";
            this.theDetail.Size = new System.Drawing.Size(464, 121);
            this.theDetail.TabIndex = 4;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 286);
            this.Controls.Add(this.theDataGridView);
            this.Controls.Add(this.theDetail);
            this.Controls.Add(this.mainMenu);
            this.Name = "frmMain";
            this.Text = "Company Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.theDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.theBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileExitMenu;
        private System.Windows.Forms.ToolStripMenuItem jumpToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CompaniesMenu;
        private System.Windows.Forms.ToolStripMenuItem ClientsMenu;
        private System.Windows.Forms.ToolStripMenuItem projectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EmployeesMenu;
        private System.Windows.Forms.ToolStripMenuItem depotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private DetailCtrl theDetail;
        private System.Windows.Forms.DataGridView theDataGridView;
        private System.Windows.Forms.BindingSource theBindingSource;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem DataAddNewMenu;
        private System.Windows.Forms.ToolStripMenuItem DataUpdateMenu;
        private System.Windows.Forms.ToolStripMenuItem DataDeleteMenu;
    }
}