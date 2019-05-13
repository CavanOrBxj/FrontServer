namespace FrontServer
{
    partial class MainForm
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.MQRecvMsg = new System.Windows.Forms.RichTextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.SystemMsg = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.TCPRecvMsg = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TCPSendMsg = new System.Windows.Forms.RichTextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_reback_address = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_reback_cycle = new System.Windows.Forms.Button();
            this.txt_reback_cycle = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // MQRecvMsg
            // 
            this.MQRecvMsg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MQRecvMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MQRecvMsg.Location = new System.Drawing.Point(3, 17);
            this.MQRecvMsg.Name = "MQRecvMsg";
            this.MQRecvMsg.ReadOnly = true;
            this.MQRecvMsg.Size = new System.Drawing.Size(808, 212);
            this.MQRecvMsg.TabIndex = 37;
            this.MQRecvMsg.Text = "";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(0, -2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(831, 781);
            this.tabControl1.TabIndex = 41;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(823, 755);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "系统运行打印";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.SystemMsg);
            this.groupBox4.Location = new System.Drawing.Point(0, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(814, 746);
            this.groupBox4.TabIndex = 44;
            this.groupBox4.TabStop = false;
            // 
            // SystemMsg
            // 
            this.SystemMsg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SystemMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SystemMsg.Location = new System.Drawing.Point(3, 17);
            this.SystemMsg.Name = "SystemMsg";
            this.SystemMsg.ReadOnly = true;
            this.SystemMsg.Size = new System.Drawing.Size(808, 726);
            this.SystemMsg.TabIndex = 37;
            this.SystemMsg.Text = "";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(823, 755);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "数据打印";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.TCPRecvMsg);
            this.groupBox2.Location = new System.Drawing.Point(8, 514);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(814, 232);
            this.groupBox2.TabIndex = 45;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "TCP接收消息";
            // 
            // TCPRecvMsg
            // 
            this.TCPRecvMsg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TCPRecvMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TCPRecvMsg.Location = new System.Drawing.Point(3, 17);
            this.TCPRecvMsg.Name = "TCPRecvMsg";
            this.TCPRecvMsg.ReadOnly = true;
            this.TCPRecvMsg.Size = new System.Drawing.Size(808, 212);
            this.TCPRecvMsg.TabIndex = 37;
            this.TCPRecvMsg.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.TCPSendMsg);
            this.groupBox1.Location = new System.Drawing.Point(8, 268);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(814, 232);
            this.groupBox1.TabIndex = 44;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "TCP发送消息";
            // 
            // TCPSendMsg
            // 
            this.TCPSendMsg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TCPSendMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TCPSendMsg.Location = new System.Drawing.Point(3, 17);
            this.TCPSendMsg.Name = "TCPSendMsg";
            this.TCPSendMsg.ReadOnly = true;
            this.TCPSendMsg.Size = new System.Drawing.Size(808, 212);
            this.TCPSendMsg.TabIndex = 37;
            this.TCPSendMsg.Text = "";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.MQRecvMsg);
            this.groupBox3.Location = new System.Drawing.Point(8, 22);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(814, 232);
            this.groupBox3.TabIndex = 43;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "MQ接收消息";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.button5);
            this.tabPage3.Controls.Add(this.button7);
            this.tabPage3.Controls.Add(this.button6);
            this.tabPage3.Controls.Add(this.button1);
            this.tabPage3.Controls.Add(this.button4);
            this.tabPage3.Controls.Add(this.button3);
            this.tabPage3.Controls.Add(this.button2);
            this.tabPage3.Controls.Add(this.groupBox5);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(823, 755);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "参数设置";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(84, 386);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(228, 77);
            this.button7.TabIndex = 10;
            this.button7.Text = "输出通道获取";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(84, 289);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(228, 77);
            this.button6.TabIndex = 9;
            this.button6.Text = "输入通道获取";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(84, 192);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(228, 77);
            this.button1.TabIndex = 8;
            this.button1.Text = "数据库插入测试";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(427, 286);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(427, 240);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(427, 192);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.txt_reback_address);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.btn_reback_cycle);
            this.groupBox5.Controls.Add(this.txt_reback_cycle);
            this.groupBox5.Location = new System.Drawing.Point(31, 30);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(734, 134);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "回传参数设置";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(62, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "回传地址";
            // 
            // txt_reback_address
            // 
            this.txt_reback_address.Location = new System.Drawing.Point(140, 66);
            this.txt_reback_address.Name = "txt_reback_address";
            this.txt_reback_address.Size = new System.Drawing.Size(196, 21);
            this.txt_reback_address.TabIndex = 5;
            this.txt_reback_address.Text = "192.168.100.100:7202";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(248, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "秒";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(62, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "回传周期";
            // 
            // btn_reback_cycle
            // 
            this.btn_reback_cycle.Location = new System.Drawing.Point(396, 65);
            this.btn_reback_cycle.Name = "btn_reback_cycle";
            this.btn_reback_cycle.Size = new System.Drawing.Size(75, 23);
            this.btn_reback_cycle.TabIndex = 2;
            this.btn_reback_cycle.Text = "设置";
            this.btn_reback_cycle.UseVisualStyleBackColor = true;
            this.btn_reback_cycle.Click += new System.EventHandler(this.btn_reback_cycle_Click);
            // 
            // txt_reback_cycle
            // 
            this.txt_reback_cycle.Location = new System.Drawing.Point(140, 39);
            this.txt_reback_cycle.Name = "txt_reback_cycle";
            this.txt_reback_cycle.Size = new System.Drawing.Size(100, 21);
            this.txt_reback_cycle.TabIndex = 1;
            this.txt_reback_cycle.Text = "30";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(84, 484);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(228, 77);
            this.button5.TabIndex = 11;
            this.button5.Text = "天安服务器验签";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click_1);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(833, 779);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainForm";
            this.Text = "前端服务器";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.RichTextBox MQRecvMsg;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RichTextBox SystemMsg;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox TCPRecvMsg;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox TCPSendMsg;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_reback_cycle;
        private System.Windows.Forms.TextBox txt_reback_cycle;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_reback_address;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
    }
}

