namespace Updater
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.download_new = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.mainTabPage = new System.Windows.Forms.TabPage();
            this.logList = new System.Windows.Forms.ListView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.fullUpdateButton = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1.SuspendLayout();
            this.mainTabPage.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.download_new);
            this.tabPage1.Controls.Add(this.label19);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(819, 719);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Справка";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // download_new
            // 
            this.download_new.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.download_new.Location = new System.Drawing.Point(9, 162);
            this.download_new.Name = "download_new";
            this.download_new.Size = new System.Drawing.Size(477, 61);
            this.download_new.TabIndex = 1;
            this.download_new.Text = "Скачать новую версию программы";
            this.download_new.UseVisualStyleBackColor = true;
            this.download_new.Click += new System.EventHandler(this.download_new_Click);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(6, 3);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(385, 130);
            this.label19.TabIndex = 0;
            this.label19.Text = resources.GetString("label19.Text");
            // 
            // mainTabPage
            // 
            this.mainTabPage.Controls.Add(this.logList);
            this.mainTabPage.Controls.Add(this.toolStrip1);
            this.mainTabPage.Controls.Add(this.statusStrip1);
            this.mainTabPage.Controls.Add(this.fullUpdateButton);
            this.mainTabPage.Controls.Add(this.progressBar);
            this.mainTabPage.Location = new System.Drawing.Point(4, 22);
            this.mainTabPage.Name = "mainTabPage";
            this.mainTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.mainTabPage.Size = new System.Drawing.Size(819, 719);
            this.mainTabPage.TabIndex = 0;
            this.mainTabPage.Text = "Главная";
            this.mainTabPage.UseVisualStyleBackColor = true;
            // 
            // logList
            // 
            this.logList.Location = new System.Drawing.Point(6, 70);
            this.logList.MultiSelect = false;
            this.logList.Name = "logList";
            this.logList.ShowItemToolTips = true;
            this.logList.Size = new System.Drawing.Size(810, 621);
            this.logList.TabIndex = 21;
            this.logList.UseCompatibleStateImageBehavior = false;
            this.logList.View = System.Windows.Forms.View.List;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(813, 25);
            this.toolStrip1.TabIndex = 20;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.statusStrip1.Location = new System.Drawing.Point(3, 694);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(813, 22);
            this.statusStrip1.TabIndex = 19;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // fullUpdateButton
            // 
            this.fullUpdateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.fullUpdateButton.Location = new System.Drawing.Point(6, 31);
            this.fullUpdateButton.Name = "fullUpdateButton";
            this.fullUpdateButton.Size = new System.Drawing.Size(138, 33);
            this.fullUpdateButton.TabIndex = 8;
            this.fullUpdateButton.Text = "ОБНОВИТЬ";
            this.fullUpdateButton.UseVisualStyleBackColor = true;
            this.fullUpdateButton.Click += new System.EventHandler(this.fullUpdateButton_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(146, 31);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(670, 33);
            this.progressBar.TabIndex = 17;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.mainTabPage);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(827, 745);
            this.tabControl1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 745);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Менеджер обновления справочников трейдера 1.1.0.2";
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.mainTabPage.ResumeLayout(false);
            this.mainTabPage.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TabPage mainTabPage;
        private System.Windows.Forms.Button fullUpdateButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ListView logList;
        private System.Windows.Forms.Button download_new;
    }
}

