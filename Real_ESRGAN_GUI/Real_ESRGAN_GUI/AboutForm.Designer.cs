
namespace Real_ESRGAN_GUI
{
    partial class AboutForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.ButtonConfirm = new System.Windows.Forms.Button();
            this.LinkLabelLicense = new System.Windows.Forms.LinkLabel();
            this.LabelCopyRight = new System.Windows.Forms.Label();
            this.LinkLabelGitHub = new System.Windows.Forms.LinkLabel();
            this.MainLabel = new System.Windows.Forms.Label();
            this.MainPic = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.MainPic)).BeginInit();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonConfirm
            // 
            this.ButtonConfirm.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tableLayoutPanel.SetColumnSpan(this.ButtonConfirm, 5);
            this.ButtonConfirm.Location = new System.Drawing.Point(109, 227);
            this.ButtonConfirm.Name = "ButtonConfirm";
            this.ButtonConfirm.Size = new System.Drawing.Size(160, 40);
            this.ButtonConfirm.TabIndex = 5;
            this.ButtonConfirm.Text = "确定";
            this.ButtonConfirm.UseVisualStyleBackColor = true;
            this.ButtonConfirm.Click += new System.EventHandler(this.CONFIRM_BUTTON_CLICK);
            // 
            // LinkLabelLicense
            // 
            this.LinkLabelLicense.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LinkLabelLicense.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.LinkLabelLicense, 5);
            this.LinkLabelLicense.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LinkLabelLicense.Location = new System.Drawing.Point(3, 171);
            this.LinkLabelLicense.Name = "LinkLabelLicense";
            this.LinkLabelLicense.Size = new System.Drawing.Size(372, 53);
            this.LinkLabelLicense.TabIndex = 4;
            this.LinkLabelLicense.TabStop = true;
            this.LinkLabelLicense.Text = "许可证";
            this.LinkLabelLicense.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LinkLabelLicense.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.TELEGRAM_LABEL_LINK_CLICKED);
            // 
            // LabelCopyRight
            // 
            this.LabelCopyRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelCopyRight.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.LabelCopyRight, 5);
            this.LabelCopyRight.Font = new System.Drawing.Font("Ink Free", 7F);
            this.LabelCopyRight.Location = new System.Drawing.Point(3, 274);
            this.LabelCopyRight.Name = "LabelCopyRight";
            this.LabelCopyRight.Size = new System.Drawing.Size(372, 53);
            this.LabelCopyRight.TabIndex = 3;
            this.LabelCopyRight.Text = "Copyright© 2024 LatteYogurt , All rights reserved.";
            this.LabelCopyRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LinkLabelGitHub
            // 
            this.LinkLabelGitHub.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LinkLabelGitHub.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.LinkLabelGitHub, 5);
            this.LinkLabelGitHub.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LinkLabelGitHub.Location = new System.Drawing.Point(3, 118);
            this.LinkLabelGitHub.Name = "LinkLabelGitHub";
            this.LinkLabelGitHub.Size = new System.Drawing.Size(372, 53);
            this.LinkLabelGitHub.TabIndex = 1;
            this.LinkLabelGitHub.TabStop = true;
            this.LinkLabelGitHub.Text = "Github主页";
            this.LinkLabelGitHub.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LinkLabelGitHub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.WEBSITE_LABEL_LINK_CLICKED);
            // 
            // MainLabel
            // 
            this.MainLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainLabel.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.MainLabel, 5);
            this.MainLabel.Font = new System.Drawing.Font("Segoe Print", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainLabel.Location = new System.Drawing.Point(3, 68);
            this.MainLabel.Name = "MainLabel";
            this.MainLabel.Size = new System.Drawing.Size(372, 50);
            this.MainLabel.TabIndex = 0;
            this.MainLabel.Text = "Real ESRGAN GUI";
            this.MainLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainPic
            // 
            this.MainPic.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.MainPic.ErrorImage = null;
            this.MainPic.Image = ((System.Drawing.Image)(resources.GetObject("MainPic.Image")));
            this.MainPic.InitialImage = null;
            this.MainPic.Location = new System.Drawing.Point(155, 18);
            this.MainPic.Name = "MainPic";
            this.MainPic.Size = new System.Drawing.Size(64, 47);
            this.MainPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.MainPic.TabIndex = 2;
            this.MainPic.TabStop = false;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 5;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel.Controls.Add(this.LabelCopyRight, 0, 6);
            this.tableLayoutPanel.Controls.Add(this.ButtonConfirm, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.MainPic, 2, 1);
            this.tableLayoutPanel.Controls.Add(this.LinkLabelLicense, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.MainLabel, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.LinkLabelGitHub, 0, 3);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 8;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(378, 344);
            this.tableLayoutPanel.TabIndex = 1;
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 344);
            this.Controls.Add(this.tableLayoutPanel);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "AboutForm";
            this.Text = "关于 Real ESRGAN GUI";
            this.Load += new System.EventHandler(this.ABOUT_FORM_LOAD);
            ((System.ComponentModel.ISupportInitialize)(this.MainPic)).EndInit();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label MainLabel;
        private System.Windows.Forms.LinkLabel LinkLabelGitHub;
        private System.Windows.Forms.PictureBox MainPic;
        private System.Windows.Forms.Label LabelCopyRight;
        private System.Windows.Forms.LinkLabel LinkLabelLicense;
        private System.Windows.Forms.Button ButtonConfirm;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
    }
}