namespace UploadHelper
{
    partial class Validations
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Validations));
            this.panel1 = new System.Windows.Forms.Panel();
            this.txt_UploadFolder = new UploadHelper.UCL.WaterTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_PW = new UploadHelper.UCL.WaterTextBox();
            this.txt_Key = new UploadHelper.UCL.WaterTextBox();
            this.btn_OK = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txt_UploadFolder);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txt_PW);
            this.panel1.Controls.Add(this.txt_Key);
            this.panel1.Controls.Add(this.btn_OK);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(478, 109);
            this.panel1.TabIndex = 0;
            // 
            // txt_UploadFolder
            // 
            this.txt_UploadFolder.Location = new System.Drawing.Point(109, 76);
            this.txt_UploadFolder.Name = "txt_UploadFolder";
            this.txt_UploadFolder.Size = new System.Drawing.Size(261, 27);
            this.txt_UploadFolder.TabIndex = 8;
            this.txt_UploadFolder.Text = "D:\\LackyWave\\Upload\\";
            this.txt_UploadFolder.WatermarkText = "PassWord";
            this.txt_UploadFolder.MouseClick += new System.Windows.Forms.MouseEventHandler(this.waterTextBox1_MouseClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 79);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "Default Path";
            // 
            // txt_PW
            // 
            this.txt_PW.Location = new System.Drawing.Point(109, 44);
            this.txt_PW.Name = "txt_PW";
            this.txt_PW.Size = new System.Drawing.Size(261, 27);
            this.txt_PW.TabIndex = 6;
            this.txt_PW.WatermarkText = "PassWord";
            // 
            // txt_Key
            // 
            this.txt_Key.Location = new System.Drawing.Point(109, 11);
            this.txt_Key.Name = "txt_Key";
            this.txt_Key.Size = new System.Drawing.Size(261, 27);
            this.txt_Key.TabIndex = 5;
            this.txt_Key.WatermarkText = "API_Key";
            // 
            // btn_OK
            // 
            this.btn_OK.Location = new System.Drawing.Point(376, 12);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(90, 59);
            this.btn_OK.TabIndex = 4;
            this.btn_OK.Text = "確定";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 47);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Api_Key";
            // 
            // Validations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 109);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Validations";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "驗證";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_OK;
        private UCL.WaterTextBox txt_Key;
        private UCL.WaterTextBox txt_PW;
        private UCL.WaterTextBox txt_UploadFolder;
        private System.Windows.Forms.Label label3;
    }
}