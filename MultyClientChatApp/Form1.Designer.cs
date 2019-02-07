namespace MultyClientChatApp
{
    partial class MultyChatApp
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
            this.sendMsgBtn = new System.Windows.Forms.Button();
            this.btnListen = new System.Windows.Forms.Button();
            this.chatBox = new System.Windows.Forms.TextBox();
            this.txtServerIp = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.msgBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // sendMsgBtn
            // 
            this.sendMsgBtn.Location = new System.Drawing.Point(566, 399);
            this.sendMsgBtn.Name = "sendMsgBtn";
            this.sendMsgBtn.Size = new System.Drawing.Size(80, 30);
            this.sendMsgBtn.TabIndex = 0;
            this.sendMsgBtn.Text = "btnSend";
            this.sendMsgBtn.UseVisualStyleBackColor = true;
            this.sendMsgBtn.Click += new System.EventHandler(this.btnSend);
            // 
            // btnListen
            // 
            this.btnListen.Location = new System.Drawing.Point(651, 54);
            this.btnListen.Name = "btnListen";
            this.btnListen.Size = new System.Drawing.Size(142, 62);
            this.btnListen.TabIndex = 1;
            this.btnListen.Text = "listen";
            this.btnListen.UseVisualStyleBackColor = true;
            this.btnListen.Click += new System.EventHandler(this.button2_Click);
            // 
            // chatBox
            // 
            this.chatBox.Location = new System.Drawing.Point(32, 54);
            this.chatBox.Multiline = true;
            this.chatBox.Name = "chatBox";
            this.chatBox.Size = new System.Drawing.Size(575, 320);
            this.chatBox.TabIndex = 2;
            this.chatBox.Text = "Lets chat!";
            this.chatBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // txtServerIp
            // 
            this.txtServerIp.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.txtServerIp.Location = new System.Drawing.Point(17, 56);
            this.txtServerIp.Name = "txtServerIp";
            this.txtServerIp.Size = new System.Drawing.Size(100, 22);
            this.txtServerIp.TabIndex = 3;
            this.txtServerIp.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "server IP";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(17, 84);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(120, 24);
            this.button3.TabIndex = 6;
            this.button3.Text = "btnConnect";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // msgBox
            // 
            this.msgBox.AccessibleDescription = "Type your message here";
            this.msgBox.AccessibleName = "Type your message here";
            this.msgBox.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.msgBox.Location = new System.Drawing.Point(32, 403);
            this.msgBox.Name = "msgBox";
            this.msgBox.Size = new System.Drawing.Size(528, 22);
            this.msgBox.TabIndex = 7;
            this.msgBox.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.txtServerIp);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(613, 145);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(180, 114);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connect to the server";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // MultyChatApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.msgBox);
            this.Controls.Add(this.chatBox);
            this.Controls.Add(this.btnListen);
            this.Controls.Add(this.sendMsgBtn);
            this.Controls.Add(this.groupBox1);
            this.Name = "MultyChatApp";
            this.Text = "Multy chat App";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button sendMsgBtn;
        private System.Windows.Forms.Button btnListen;
        private System.Windows.Forms.TextBox chatBox;
        private System.Windows.Forms.TextBox txtServerIp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox msgBox;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

