namespace cdcrush.forms
{
	partial class FormMain
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
            this.info_status = new System.Windows.Forms.RichTextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabRestore = new System.Windows.Forms.TabPage();
            this.panelRestore1 = new cdcrush.forms.PanelRestore();
            this.tabCrush = new System.Windows.Forms.TabPage();
            this.panelCompress1 = new cdcrush.forms.PanelCompress();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.tabInfo = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabRestore.SuspendLayout();
            this.tabCrush.SuspendLayout();
            this.tabInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // info_status
            // 
            this.info_status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.info_status.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.info_status.Location = new System.Drawing.Point(12, 361);
            this.info_status.Name = "info_status";
            this.info_status.ReadOnly = true;
            this.info_status.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.info_status.Size = new System.Drawing.Size(462, 15);
            this.info_status.TabIndex = 19;
            this.info_status.Text = "Ready.";
            this.info_status.WordWrap = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 334);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(458, 24);
            this.progressBar1.TabIndex = 12;
            this.progressBar1.Value = 50;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabRestore);
            this.tabControl1.Controls.Add(this.tabCrush);
            this.tabControl1.Controls.Add(this.tabSettings);
            this.tabControl1.Controls.Add(this.tabInfo);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(462, 322);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 20;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabRestore
            // 
            this.tabRestore.Controls.Add(this.panelRestore1);
            this.tabRestore.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabRestore.Location = new System.Drawing.Point(4, 25);
            this.tabRestore.Name = "tabRestore";
            this.tabRestore.Padding = new System.Windows.Forms.Padding(3);
            this.tabRestore.Size = new System.Drawing.Size(454, 293);
            this.tabRestore.TabIndex = 0;
            this.tabRestore.Text = "Restore a CD";
            this.tabRestore.UseVisualStyleBackColor = true;
            // 
            // panelRestore1
            // 
            this.panelRestore1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRestore1.Location = new System.Drawing.Point(3, 3);
            this.panelRestore1.MaximumSize = new System.Drawing.Size(600, 280);
            this.panelRestore1.MinimumSize = new System.Drawing.Size(450, 280);
            this.panelRestore1.Name = "panelRestore1";
            this.panelRestore1.Size = new System.Drawing.Size(450, 280);
            this.panelRestore1.TabIndex = 0;
            // 
            // tabCrush
            // 
            this.tabCrush.Controls.Add(this.panelCompress1);
            this.tabCrush.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabCrush.Location = new System.Drawing.Point(4, 25);
            this.tabCrush.Name = "tabCrush";
            this.tabCrush.Padding = new System.Windows.Forms.Padding(3);
            this.tabCrush.Size = new System.Drawing.Size(454, 293);
            this.tabCrush.TabIndex = 1;
            this.tabCrush.Text = "Compress a CD";
            this.tabCrush.UseVisualStyleBackColor = true;
            // 
            // panelCompress1
            // 
            this.panelCompress1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCompress1.Location = new System.Drawing.Point(3, 3);
            this.panelCompress1.MaximumSize = new System.Drawing.Size(600, 280);
            this.panelCompress1.MinimumSize = new System.Drawing.Size(450, 280);
            this.panelCompress1.Name = "panelCompress1";
            this.panelCompress1.Size = new System.Drawing.Size(450, 280);
            this.panelCompress1.TabIndex = 0;
            // 
            // tabSettings
            // 
            this.tabSettings.Location = new System.Drawing.Point(4, 25);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new System.Drawing.Size(454, 293);
            this.tabSettings.TabIndex = 2;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // tabInfo
            // 
            this.tabInfo.Controls.Add(this.label2);
            this.tabInfo.Controls.Add(this.label1);
            this.tabInfo.Location = new System.Drawing.Point(4, 25);
            this.tabInfo.Name = "tabInfo";
            this.tabInfo.Size = new System.Drawing.Size(454, 293);
            this.tabInfo.TabIndex = 3;
            this.tabInfo.Text = "Info";
            this.tabInfo.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label2.Location = new System.Drawing.Point(290, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Version 1.3";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(227, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "CDCRUSH";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 381);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.info_status);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimumSize = new System.Drawing.Size(502, 420);
            this.Name = "FormMain";
            this.Text = "cdcrush";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabRestore.ResumeLayout(false);
            this.tabCrush.ResumeLayout(false);
            this.tabInfo.ResumeLayout(false);
            this.tabInfo.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RichTextBox info_status;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabRestore;
		private System.Windows.Forms.TabPage tabCrush;
		private System.Windows.Forms.TabPage tabSettings;
		private PanelRestore panelRestore1;
		private System.Windows.Forms.TabPage tabInfo;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private PanelCompress panelCompress1;

	}
}