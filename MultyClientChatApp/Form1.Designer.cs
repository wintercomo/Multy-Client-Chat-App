﻿namespace MultyClientChatApp
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
            this.txtServerIp = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.connectButton = new System.Windows.Forms.Button();
            this.msgBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chatBox = new System.Windows.Forms.ListBox();
            this.portBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bufferSize = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
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
            this.sendMsgBtn.Click += new System.EventHandler(this.BtnSend);
            // 
            // txtServerIp
            // 
            this.txtServerIp.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.txtServerIp.Location = new System.Drawing.Point(17, 56);
            this.txtServerIp.Name = "txtServerIp";
            this.txtServerIp.Size = new System.Drawing.Size(100, 22);
            this.txtServerIp.TabIndex = 3;
            this.txtServerIp.Text = "127.0.0.1";
            this.txtServerIp.TextChanged += new System.EventHandler(this.txtServerIp_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "server IP";
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(17, 126);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(120, 24);
            this.connectButton.TabIndex = 6;
            this.connectButton.Text = "btnConnect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.BtnConnect);
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
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.portBox);
            this.groupBox1.Controls.Add(this.connectButton);
            this.groupBox1.Controls.Add(this.txtServerIp);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(613, 145);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(180, 171);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connect to the server";
            // 
            // chatBox
            // 
            this.chatBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chatBox.FormattingEnabled = true;
            this.chatBox.ItemHeight = 20;
            this.chatBox.Location = new System.Drawing.Point(12, 11);
            this.chatBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chatBox.Name = "chatBox";
            this.chatBox.Size = new System.Drawing.Size(595, 384);
            this.chatBox.TabIndex = 10;
            this.chatBox.SelectedIndexChanged += new System.EventHandler(this.chatBox_SelectedIndexChanged);
            // 
            // portBox
            // 
            this.portBox.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.portBox.Location = new System.Drawing.Point(17, 98);
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(100, 22);
            this.portBox.TabIndex = 7;
            this.portBox.Text = "9000";
            this.portBox.TextChanged += new System.EventHandler(this.portBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Port:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // bufferSize
            // 
            this.bufferSize.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.bufferSize.Location = new System.Drawing.Point(630, 62);
            this.bufferSize.Name = "bufferSize";
            this.bufferSize.Size = new System.Drawing.Size(100, 22);
            this.bufferSize.TabIndex = 9;
            this.bufferSize.Text = "9000";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(627, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 17);
            this.label3.TabIndex = 9;
            this.label3.Text = "Buffer size";
            // 
            // MultyChatApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.bufferSize);
            this.Controls.Add(this.chatBox);
            this.Controls.Add(this.msgBox);
            this.Controls.Add(this.sendMsgBtn);
            this.Controls.Add(this.groupBox1);
            this.Name = "MultyChatApp";
            this.Text = "Multy chat App";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button sendMsgBtn;
        private System.Windows.Forms.TextBox txtServerIp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.TextBox msgBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox chatBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox portBox;
        private System.Windows.Forms.TextBox bufferSize;
        private System.Windows.Forms.Label label3;
    }
}

