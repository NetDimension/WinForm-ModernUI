namespace DemoApp
{
	partial class frmMain
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(50)))), ((int)(((byte)(138)))));
			this.label1.Location = new System.Drawing.Point(20, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(421, 31);
			this.label1.TabIndex = 0;
			this.label1.Text = "NetDimension ModernUI Container";
			this.label1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label1_MouseDown);
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.AutoSize = true;
			this.button1.Location = new System.Drawing.Point(493, 412);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(103, 35);
			this.button1.TabIndex = 1;
			this.button1.Text = "Close (&C)";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(35, 80);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(560, 170);
			this.label2.TabIndex = 2;
			this.label2.Text = resources.GetString("label2.Text");
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.AutoSize = true;
			this.button2.Location = new System.Drawing.Point(376, 412);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(95, 35);
			this.button2.TabIndex = 3;
			this.button2.Text = "GitHub";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button3
			// 
			this.button3.AutoSize = true;
			this.button3.Location = new System.Drawing.Point(40, 253);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(126, 35);
			this.button3.TabIndex = 4;
			this.button3.Text = "Glow Effect";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// frmMain
			// 
			this.ActiveBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(50)))), ((int)(((byte)(138)))));
			this.ActiveShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(95)))), ((int)(((byte)(197)))));
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(638, 478);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.InactiveShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(95)))), ((int)(((byte)(197)))));
			this.Name = "frmMain";
			this.ShadowEffect = NetDimension.WinForm.FormShadowType.DropShadow;
			this.Text = "frmMain";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
	}
}