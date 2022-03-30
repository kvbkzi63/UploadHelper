using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UploadHelper.Helper;

namespace UploadHelper
{
    public partial class MyFormBase : Form
    {
 
        public string _SetMessage { get; set; }
        //Your window Constructor
        public MyFormBase()
        {
            InitializeComponent();
        }
        public void SetMessage(string message)
        {
            label1.Text = message;
        }

    }
}
