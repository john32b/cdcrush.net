namespace cdcrush.forms
{
	partial class FormChecksums
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
			if(disposing && (components != null)) {
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
			this.btn_close = new System.Windows.Forms.Button();
			this.textbox = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// btn_close
			// 
			this.btn_close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btn_close.Location = new System.Drawing.Point(12, 332);
			this.btn_close.Name = "btn_close";
			this.btn_close.Size = new System.Drawing.Size(75, 23);
			this.btn_close.TabIndex = 1;
			this.btn_close.Text = "Close";
			this.btn_close.UseVisualStyleBackColor = true;
			this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
			// 
			// textbox
			// 
			this.textbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textbox.BackColor = System.Drawing.SystemColors.Window;
			this.textbox.DetectUrls = false;
			this.textbox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textbox.Location = new System.Drawing.Point(12, 12);
			this.textbox.Name = "textbox";
			this.textbox.ReadOnly = true;
			this.textbox.Size = new System.Drawing.Size(600, 314);
			this.textbox.TabIndex = 2;
			this.textbox.Text = "";
			this.textbox.WordWrap = false;
			// 
			// FormChecksums
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(624, 361);
			this.Controls.Add(this.textbox);
			this.Controls.Add(this.btn_close);
			this.Name = "FormChecksums";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "CD Information";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormChecksums_FormClosing);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Button btn_close;
		private System.Windows.Forms.RichTextBox textbox;
	}
}