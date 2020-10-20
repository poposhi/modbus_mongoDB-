namespace modbus_mongoDB建立
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timertrickAtBeginning = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.timer_read_upload = new System.Windows.Forms.Timer(this.components);
            this.lv = new System.Windows.Forms.ListView();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.timer_set_pq = new System.Windows.Forms.Timer(this.components);
            this.button6 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // timertrickAtBeginning
            // 
            this.timertrickAtBeginning.Tick += new System.EventHandler(this.timertrickAtBeginning_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(68, 92);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 34);
            this.button1.TabIndex = 0;
            this.button1.Text = "恢復故障";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(68, 164);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(85, 34);
            this.button2.TabIndex = 1;
            this.button2.Text = "開始上傳";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(77, 224);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(188, 164);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(163, 34);
            this.button3.TabIndex = 3;
            this.button3.Text = "讀取資料並且上傳";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // timer_read_upload
            // 
            this.timer_read_upload.Interval = 1000;
            this.timer_read_upload.Tick += new System.EventHandler(this.timer_read_upload_Tick);
            // 
            // lv
            // 
            this.lv.HideSelection = false;
            this.lv.Location = new System.Drawing.Point(357, 164);
            this.lv.Name = "lv";
            this.lv.Size = new System.Drawing.Size(444, 256);
            this.lv.TabIndex = 149;
            this.lv.UseCompatibleStateImageBehavior = false;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(734, 92);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(85, 49);
            this.button4.TabIndex = 150;
            this.button4.Text = "設定功率";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(188, 224);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(109, 34);
            this.button5.TabIndex = 151;
            this.button5.Text = "測試讀取ems";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // timer_set_pq
            // 
            this.timer_set_pq.Interval = 1000;
            this.timer_set_pq.Tick += new System.EventHandler(this.timer_set_pq_Tick);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(55, 296);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(279, 34);
            this.button6.TabIndex = 152;
            this.button6.Text = "啟動每秒鐘任務 (讀取上傳_讀取設定點)";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 450);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.lv);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timertrickAtBeginning;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Timer timer_read_upload;
        private System.Windows.Forms.ListView lv;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Timer timer_set_pq;
        private System.Windows.Forms.Button button6;
    }
}

