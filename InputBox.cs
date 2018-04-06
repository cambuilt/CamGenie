using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CamGenie
{
    public partial class InputBox : Form
    {
        public string InputValue = "";

        public InputBox(string message, string defaultValue)
        {
            InitializeComponent();
            labelMessage.Text = message;
            textBoxInputValue.Text = defaultValue;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            InputValue = textBoxInputValue.Text;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
