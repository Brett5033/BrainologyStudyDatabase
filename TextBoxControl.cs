using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrainologyStudyDatabase
{
    public partial class TextBoxControl : UserControl
    {
        public int id;
        public MetroFramework.Controls.MetroLabel lbl;
        public MetroFramework.Controls.MetroTextBox txt;
        public TextBoxControl()
        {
            InitializeComponent();
            lbl = TBC_LBL;
            txt = TBC_TXT;
        }
    }
}
