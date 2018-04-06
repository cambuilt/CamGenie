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
    public partial class DotRemove : Form
    {
        public int MinimumDotHeight = 0;
        public int MinimumDotWidth = 0;
        public int MaximumDotWidth = 0;
        public int MaximumDotHeight = 0;

        public DotRemove()
        {
            InitializeComponent();
        }

        private void DotRemove_FormClosing(object sender, FormClosingEventArgs e)
        {
            MinimumDotHeight = int.Parse(_tbMinimumDotHeight.Text);
            MinimumDotWidth = int.Parse(_tbMinimumDotWidth.Text);
            MaximumDotHeight = int.Parse(_tbMaximumDotHeight.Text);
            MaximumDotWidth = int.Parse(_tbMaximumDotWidth.Text);
        }
    }
}
