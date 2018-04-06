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
    public partial class LineRemove : Form
    {
        public bool RemoveHorizontalLines = true;
        public bool RemoveVerticalLines = false;
        public int MinimumLength = 0;
        public int MaximumWidth = 0;
        public int MaximumGap = 0;

        public LineRemove()
        {
            InitializeComponent();
        }

        private void LineRemove_FormClosing(object sender, FormClosingEventArgs e)
        {
            RemoveHorizontalLines = checkBoxRemoveHorizontalLines.Checked;
            RemoveVerticalLines = checkBoxRemoveVerticalLines.Checked;
            MinimumLength = int.Parse(textBoxMinimumLength.Text);
            MaximumWidth = int.Parse(textBoxMaximumWidth.Text);
            MaximumGap = int.Parse(textBoxMaximumGap.Text);
        }
    }
}
