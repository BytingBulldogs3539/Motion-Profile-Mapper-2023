using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MotionProfileMapper.Forms {
    public partial class Preview : Form {
        public Preview(string displayText) {
            InitializeComponent();
            this.previewBox.Text = displayText;
        }
    }
}
