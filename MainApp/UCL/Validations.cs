using MainApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UploadHelper
{
    public partial class Validations : Form
    {
        public AuthModel AuthModel { get; set; }
        public Validations()
        {
            InitializeComponent(); 
        }
        public Validations(AuthModel model)
        {
            InitializeComponent();
            txt_Key.Text = model.api_key;
            txt_Key.Enabled = false;
            txt_PW.Text = model.password;
            txt_PW.Enabled = false;
            txt_UploadFolder.Text = model.uploadPath;
                 
        }
        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_Key.Text) || string.IsNullOrWhiteSpace(txt_Key.Text))
            {
                MessageBox.Show("Key/Password不得為空");
                return;

            }
            else if (string.IsNullOrWhiteSpace(txt_UploadFolder.Text))
            {

                MessageBox.Show("上傳目錄不得為空");
                return; 
            }
            else
            {
                try
                {
                    if (!(System.IO.Directory.Exists(txt_UploadFolder.Text)))
                    {
                        System.IO.Directory.CreateDirectory(txt_UploadFolder.Text);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    DialogResult = DialogResult.Cancel;
                    this.Close(); 
                }
 
                this.AuthModel = new AuthModel();
                this.AuthModel.api_key = txt_Key.Text;
                this.AuthModel.password = txt_PW.Text;
                this.AuthModel.uploadPath = txt_UploadFolder.Text;
                DialogResult = DialogResult.OK;
                this.Close();
            }
 
        }

        private void waterTextBox1_MouseClick(object sender, MouseEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                txt_UploadFolder.Text = folderBrowserDialog.SelectedPath;
            }
        }
    }
}
