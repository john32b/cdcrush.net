namespace cdcrush.forms
{
	partial class PanelCompress
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.TableLayoutPanel table_IO;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label9;
			System.Windows.Forms.Label label10;
			System.Windows.Forms.Label label8;
			System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
			System.Windows.Forms.Label label11;
			this.btn_input_in = new System.Windows.Forms.Button();
			this.input_in = new System.Windows.Forms.TextBox();
			this.input_out = new System.Windows.Forms.TextBox();
			this.btn_input_out = new System.Windows.Forms.Button();
			this.combo_data_c = new System.Windows.Forms.ComboBox();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.combo_audio_c = new System.Windows.Forms.ComboBox();
			this.combo_audio_q = new System.Windows.Forms.ComboBox();
			this.info_tracks = new System.Windows.Forms.TextBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.info_cdtitle = new System.Windows.Forms.TextBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.btn_CRUSH = new System.Windows.Forms.Button();
			this.info_size1 = new System.Windows.Forms.TextBox();
			this.info_size0 = new System.Windows.Forms.TextBox();
			this.chk_encodedCue = new System.Windows.Forms.CheckBox();
			this.btn_chksm = new System.Windows.Forms.Button();
			table_IO = new System.Windows.Forms.TableLayoutPanel();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			label10 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			label11 = new System.Windows.Forms.Label();
			table_IO.SuspendLayout();
			tableLayoutPanel2.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// table_IO
			// 
			table_IO.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			table_IO.ColumnCount = 2;
			table_IO.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			table_IO.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			table_IO.Controls.Add(this.btn_input_in, 1, 1);
			table_IO.Controls.Add(label1, 0, 0);
			table_IO.Controls.Add(this.input_in, 0, 1);
			table_IO.Controls.Add(this.input_out, 0, 3);
			table_IO.Controls.Add(this.btn_input_out, 1, 3);
			table_IO.Controls.Add(label2, 0, 2);
			table_IO.Location = new System.Drawing.Point(3, 3);
			table_IO.Name = "table_IO";
			table_IO.RowCount = 4;
			table_IO.RowStyles.Add(new System.Windows.Forms.RowStyle());
			table_IO.RowStyles.Add(new System.Windows.Forms.RowStyle());
			table_IO.RowStyles.Add(new System.Windows.Forms.RowStyle());
			table_IO.RowStyles.Add(new System.Windows.Forms.RowStyle());
			table_IO.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			table_IO.Size = new System.Drawing.Size(449, 82);
			table_IO.TabIndex = 17;
			// 
			// btn_input_in
			// 
			this.btn_input_in.Dock = System.Windows.Forms.DockStyle.Top;
			this.btn_input_in.Location = new System.Drawing.Point(407, 16);
			this.btn_input_in.Name = "btn_input_in";
			this.btn_input_in.Size = new System.Drawing.Size(39, 23);
			this.btn_input_in.TabIndex = 2;
			this.btn_input_in.Text = "...";
			this.btn_input_in.UseVisualStyleBackColor = true;
			this.btn_input_in.Click += new System.EventHandler(this.btn_input_in_Click);
			// 
			// label1
			// 
			label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label1.Location = new System.Drawing.Point(3, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(75, 13);
			label1.TabIndex = 0;
			label1.Text = "Input File :";
			// 
			// input_in
			// 
			this.input_in.Dock = System.Windows.Forms.DockStyle.Top;
			this.input_in.Location = new System.Drawing.Point(3, 16);
			this.input_in.Name = "input_in";
			this.input_in.Size = new System.Drawing.Size(398, 20);
			this.input_in.TabIndex = 1;
			// 
			// input_out
			// 
			this.input_out.Dock = System.Windows.Forms.DockStyle.Top;
			this.input_out.Location = new System.Drawing.Point(3, 58);
			this.input_out.Name = "input_out";
			this.input_out.Size = new System.Drawing.Size(398, 20);
			this.input_out.TabIndex = 1;
			// 
			// btn_input_out
			// 
			this.btn_input_out.Dock = System.Windows.Forms.DockStyle.Top;
			this.btn_input_out.Location = new System.Drawing.Point(407, 58);
			this.btn_input_out.Name = "btn_input_out";
			this.btn_input_out.Size = new System.Drawing.Size(39, 23);
			this.btn_input_out.TabIndex = 2;
			this.btn_input_out.Text = "...";
			this.btn_input_out.UseVisualStyleBackColor = true;
			this.btn_input_out.Click += new System.EventHandler(this.btn_input_out_Click);
			// 
			// label2
			// 
			label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label2.Location = new System.Drawing.Point(3, 42);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(118, 13);
			label2.TabIndex = 0;
			label2.Text = "Destination Folder :";
			// 
			// label4
			// 
			label4.ForeColor = System.Drawing.SystemColors.ControlDark;
			label4.Location = new System.Drawing.Point(3, 84);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(178, 13);
			label4.TabIndex = 19;
			label4.Text = "Leave blank for same folder as input";
			// 
			// label5
			// 
			label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label5.Location = new System.Drawing.Point(3, 0);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(53, 13);
			label5.TabIndex = 0;
			label5.Text = "CD Title";
			// 
			// label6
			// 
			label6.Dock = System.Windows.Forms.DockStyle.Top;
			label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label6.Location = new System.Drawing.Point(3, 0);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(59, 13);
			label6.TabIndex = 6;
			label6.Text = "Tracks";
			// 
			// label7
			// 
			label7.Dock = System.Windows.Forms.DockStyle.Top;
			label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label7.Location = new System.Drawing.Point(182, 0);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(130, 13);
			label7.TabIndex = 9;
			label7.Text = "Quality";
			// 
			// label3
			// 
			label3.Dock = System.Windows.Forms.DockStyle.Top;
			label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label3.Location = new System.Drawing.Point(68, 0);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(108, 13);
			label3.TabIndex = 5;
			label3.Text = "Codec";
			// 
			// label9
			// 
			label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			label9.AutoSize = true;
			label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label9.Location = new System.Drawing.Point(282, 250);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(79, 13);
			label9.TabIndex = 23;
			label9.Text = "Source Size:";
			// 
			// label10
			// 
			label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			label10.AutoSize = true;
			label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label10.Location = new System.Drawing.Point(276, 269);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(85, 13);
			label10.TabIndex = 24;
			label10.Text = "Crushed Size:";
			// 
			// label8
			// 
			label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			label8.AutoSize = true;
			label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label8.ForeColor = System.Drawing.SystemColors.ControlDark;
			label8.Location = new System.Drawing.Point(29, 255);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(175, 13);
			label8.TabIndex = 26;
			label8.Text = "Will create subfolder on output path";
			// 
			// tableLayoutPanel2
			// 
			tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			tableLayoutPanel2.AutoSize = true;
			tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			tableLayoutPanel2.ColumnCount = 1;
			tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel2.Controls.Add(label11, 0, 0);
			tableLayoutPanel2.Controls.Add(this.combo_data_c, 0, 1);
			tableLayoutPanel2.Location = new System.Drawing.Point(137, 181);
			tableLayoutPanel2.Name = "tableLayoutPanel2";
			tableLayoutPanel2.RowCount = 2;
			tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 34.21053F));
			tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65.78947F));
			tableLayoutPanel2.Size = new System.Drawing.Size(315, 41);
			tableLayoutPanel2.TabIndex = 27;
			// 
			// label11
			// 
			label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label11.Location = new System.Drawing.Point(3, 0);
			label11.Name = "label11";
			label11.Size = new System.Drawing.Size(161, 14);
			label11.TabIndex = 0;
			label11.Text = "FreeArc Compression Level";
			// 
			// combo_data_c
			// 
			this.combo_data_c.Dock = System.Windows.Forms.DockStyle.Top;
			this.combo_data_c.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.combo_data_c.FormattingEnabled = true;
			this.combo_data_c.Items.AddRange(new object[] {
            "1 - Fastest [2.053 ratio]",
            "2 - [5.088 ratio]",
            "3 - [5.555 ratio]",
            "4 - Default [128mb RAM for pack/unpack] [6.184 ratio]",
            "5 - [256mb RAM for pack/unpack] [6.297 ratio]",
            "6 - [512mb RAM for pack/unpack]",
            "7 - Not recommended",
            "8 - Not recommended",
            "9 - Not recommended"});
			this.combo_data_c.Location = new System.Drawing.Point(3, 17);
			this.combo_data_c.Name = "combo_data_c";
			this.combo_data_c.Size = new System.Drawing.Size(309, 21);
			this.combo_data_c.TabIndex = 1;
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.ColumnCount = 3;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.72023F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.41588F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.00756F));
			this.tableLayoutPanel3.Controls.Add(label7, 2, 0);
			this.tableLayoutPanel3.Controls.Add(this.combo_audio_c, 0, 1);
			this.tableLayoutPanel3.Controls.Add(label3, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this.combo_audio_q, 1, 1);
			this.tableLayoutPanel3.Controls.Add(this.info_tracks, 0, 1);
			this.tableLayoutPanel3.Controls.Add(label6, 0, 0);
			this.tableLayoutPanel3.Location = new System.Drawing.Point(137, 142);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 2;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.Size = new System.Drawing.Size(315, 40);
			this.tableLayoutPanel3.TabIndex = 24;
			// 
			// combo_audio_c
			// 
			this.combo_audio_c.Dock = System.Windows.Forms.DockStyle.Top;
			this.combo_audio_c.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.combo_audio_c.FormattingEnabled = true;
			this.combo_audio_c.Location = new System.Drawing.Point(68, 16);
			this.combo_audio_c.Name = "combo_audio_c";
			this.combo_audio_c.Size = new System.Drawing.Size(108, 21);
			this.combo_audio_c.TabIndex = 8;
			this.combo_audio_c.SelectedIndexChanged += new System.EventHandler(this.combo_audio_c_SelectedIndexChanged);
			// 
			// combo_audio_q
			// 
			this.combo_audio_q.Dock = System.Windows.Forms.DockStyle.Top;
			this.combo_audio_q.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.combo_audio_q.FormattingEnabled = true;
			this.combo_audio_q.Location = new System.Drawing.Point(182, 16);
			this.combo_audio_q.Name = "combo_audio_q";
			this.combo_audio_q.Size = new System.Drawing.Size(130, 21);
			this.combo_audio_q.TabIndex = 4;
			// 
			// info_tracks
			// 
			this.info_tracks.BackColor = System.Drawing.SystemColors.Info;
			this.info_tracks.Dock = System.Windows.Forms.DockStyle.Top;
			this.info_tracks.Enabled = false;
			this.info_tracks.Location = new System.Drawing.Point(3, 16);
			this.info_tracks.Name = "info_tracks";
			this.info_tracks.ReadOnly = true;
			this.info_tracks.Size = new System.Drawing.Size(59, 20);
			this.info_tracks.TabIndex = 7;
			this.info_tracks.Text = "25";
			this.info_tracks.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.info_cdtitle, 0, 1);
			this.tableLayoutPanel1.Controls.Add(label5, 0, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(137, 107);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(315, 32);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// info_cdtitle
			// 
			this.info_cdtitle.BackColor = System.Drawing.SystemColors.Info;
			this.info_cdtitle.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.info_cdtitle.Dock = System.Windows.Forms.DockStyle.Top;
			this.info_cdtitle.Location = new System.Drawing.Point(3, 16);
			this.info_cdtitle.Name = "info_cdtitle";
			this.info_cdtitle.Size = new System.Drawing.Size(309, 13);
			this.info_cdtitle.TabIndex = 1;
			this.info_cdtitle.Text = "Dummy Info, CD title with a really long name etc";
			this.info_cdtitle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.info_cdtitle.WordWrap = false;
			// 
			// pictureBox1
			// 
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox1.Image = global::cdcrush.Properties.Resources.dropimage;
			this.pictureBox1.Location = new System.Drawing.Point(3, 103);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(128, 128);
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
			// 
			// btn_CRUSH
			// 
			this.btn_CRUSH.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_CRUSH.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btn_CRUSH.ForeColor = System.Drawing.Color.Green;
			this.btn_CRUSH.Location = new System.Drawing.Point(3, 292);
			this.btn_CRUSH.Name = "btn_CRUSH";
			this.btn_CRUSH.Size = new System.Drawing.Size(449, 39);
			this.btn_CRUSH.TabIndex = 22;
			this.btn_CRUSH.Text = "_dynamic_name_";
			this.btn_CRUSH.UseVisualStyleBackColor = true;
			this.btn_CRUSH.Click += new System.EventHandler(this.btn_CRUSH_Click);
			// 
			// info_size1
			// 
			this.info_size1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.info_size1.BackColor = System.Drawing.SystemColors.Info;
			this.info_size1.Location = new System.Drawing.Point(367, 247);
			this.info_size1.Name = "info_size1";
			this.info_size1.ReadOnly = true;
			this.info_size1.Size = new System.Drawing.Size(83, 20);
			this.info_size1.TabIndex = 8;
			this.info_size1.Text = "650MB";
			this.info_size1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// info_size0
			// 
			this.info_size0.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.info_size0.BackColor = System.Drawing.SystemColors.Info;
			this.info_size0.Location = new System.Drawing.Point(367, 266);
			this.info_size0.Name = "info_size0";
			this.info_size0.ReadOnly = true;
			this.info_size0.Size = new System.Drawing.Size(83, 20);
			this.info_size0.TabIndex = 22;
			this.info_size0.Text = "68MB";
			this.info_size0.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// chk_encodedCue
			// 
			this.chk_encodedCue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chk_encodedCue.AutoSize = true;
			this.chk_encodedCue.Location = new System.Drawing.Point(3, 269);
			this.chk_encodedCue.Name = "chk_encodedCue";
			this.chk_encodedCue.Size = new System.Drawing.Size(205, 17);
			this.chk_encodedCue.TabIndex = 25;
			this.chk_encodedCue.Text = "Convert to .CUE/Encoded Audio Files";
			this.chk_encodedCue.UseVisualStyleBackColor = true;
			this.chk_encodedCue.CheckedChanged += new System.EventHandler(this.chk_encodedCue_CheckedChanged);
			// 
			// btn_chksm
			// 
			this.btn_chksm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_chksm.Location = new System.Drawing.Point(139, 223);
			this.btn_chksm.Name = "btn_chksm";
			this.btn_chksm.Size = new System.Drawing.Size(81, 21);
			this.btn_chksm.TabIndex = 28;
			this.btn_chksm.Text = "Checksums";
			this.btn_chksm.UseVisualStyleBackColor = true;
			this.btn_chksm.Click += new System.EventHandler(this.btn_chksm_Click);
			// 
			// PanelCompress
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btn_chksm);
			this.Controls.Add(tableLayoutPanel2);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.tableLayoutPanel3);
			this.Controls.Add(label8);
			this.Controls.Add(this.chk_encodedCue);
			this.Controls.Add(this.info_size0);
			this.Controls.Add(label10);
			this.Controls.Add(label9);
			this.Controls.Add(this.info_size1);
			this.Controls.Add(this.btn_CRUSH);
			this.Controls.Add(label4);
			this.Controls.Add(table_IO);
			this.Controls.Add(this.pictureBox1);
			this.Name = "PanelCompress";
			this.Size = new System.Drawing.Size(455, 335);
			this.Load += new System.EventHandler(this.PanelCompress_Load);
			table_IO.ResumeLayout(false);
			table_IO.PerformLayout();
			tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.ComboBox combo_audio_q;
		private System.Windows.Forms.Button btn_input_in;
		private System.Windows.Forms.TextBox input_in;
		private System.Windows.Forms.TextBox input_out;
		private System.Windows.Forms.Button btn_input_out;
		private System.Windows.Forms.TextBox info_tracks;
		private System.Windows.Forms.Button btn_CRUSH;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TextBox info_cdtitle;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.ComboBox combo_audio_c;
		private System.Windows.Forms.TextBox info_size0;
		private System.Windows.Forms.TextBox info_size1;
		private System.Windows.Forms.CheckBox chk_encodedCue;
		private System.Windows.Forms.ComboBox combo_data_c;
		private System.Windows.Forms.Button btn_chksm;
	}
}
