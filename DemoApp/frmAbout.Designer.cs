namespace DemoApp
{
	partial class frmAbout
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
			this.button1 = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.button2 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.button1.BackColor = System.Drawing.Color.Transparent;
			this.button1.BackgroundImage = global::DemoApp.Properties.Resources.MeckEd_PayPal_donate_button;
			this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.button1.Location = new System.Drawing.Point(138, 392);
			this.button1.Name = "button1";
			this.button1.Padding = new System.Windows.Forms.Padding(10);
			this.button1.Size = new System.Drawing.Size(203, 104);
			this.button1.TabIndex = 1;
			this.button1.UseVisualStyleBackColor = false;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox1.BackgroundImage = global::DemoApp.Properties.Resources.beg_with_border;
			this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.pictureBox1.Location = new System.Drawing.Point(9, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(460, 363);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.Location = new System.Drawing.Point(138, 516);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(203, 44);
			this.button2.TabIndex = 2;
			this.button2.Text = "确定";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// frmAbout
			// 
			this.ActiveBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(50)))), ((int)(((byte)(138)))));
			this.ActiveShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(95)))), ((int)(((byte)(197)))));
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(478, 578);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.pictureBox1);
			this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.InactiveShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(95)))), ((int)(((byte)(197)))));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "frmAbout";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ModernUI Demo Application";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
	}
}

