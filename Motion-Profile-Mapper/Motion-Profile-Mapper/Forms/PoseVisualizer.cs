using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MotionProfileMapper.Forms
{
    public partial class PoseVisualizer : Form
    {
        Action closeMain;
        private double fieldWidth = 15.816;
        private double fieldHeight = 8.016;

        public PoseVisualizer(Action closeMain)
        {
            this.closeMain = closeMain;
            InitializeComponent();
            SetupMainField();
        }

        private void SetupMainField()
        {
            mainField.ChartAreas["field"].Axes[0].Minimum = 0;
            mainField.ChartAreas["field"].Axes[0].Maximum = fieldWidth;
            mainField.ChartAreas["field"].Axes[0].Interval = 1;
            mainField.ChartAreas["field"].Axes[0].MajorGrid.LineColor = Color.Gray;

            mainField.ChartAreas["field"].Axes[1].Minimum = 0;
            mainField.ChartAreas["field"].Axes[1].Maximum = fieldHeight;
            mainField.ChartAreas["field"].Axes[1].Interval = 1;
            mainField.ChartAreas["field"].Axes[1].MajorGrid.LineColor = Color.Gray;

            mainField.Series["background"].Points.AddXY(0, 0);
            mainField.Series["background"].Points.AddXY(fieldWidth, fieldHeight);
            mainField.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            mainField.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            mainField.ChartAreas["field"].AxisY2.Enabled = AxisEnabled.True;
            mainField.ChartAreas["field"].AxisY2.LabelStyle.Enabled = false;
            mainField.ChartAreas["field"].AxisY2.MajorGrid.Enabled = false;
            mainField.ChartAreas["field"].AxisY2.MajorTickMark.Enabled = false;
            mainField.ChartAreas["field"].AxisX2.Enabled = AxisEnabled.True;
            mainField.ChartAreas["field"].AxisX2.LabelStyle.Enabled = false;
            mainField.ChartAreas["field"].AxisX2.MajorGrid.Enabled = false;
            mainField.ChartAreas["field"].AxisX2.MajorTickMark.Enabled = false;

            mainField.Images.Add(new NamedImage("2023-full-field", setOpacity(MotionProfileMapper.Properties.Resources._2023_full_field, 0.5f)));
            mainField.ChartAreas["field"].BackImageWrapMode = ChartImageWrapMode.Scaled;
            mainField.ChartAreas["field"].BackImage = "2023-full-field";
            mainField.ForeColor = Color.Black;
        }

        private Bitmap setOpacity(Image image, float opacity)
        {
            Bitmap opaque = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(opaque))
            {
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = opacity;
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                g.DrawImage(
                    image,
                    new Rectangle(0, 0, opaque.Width, opaque.Height),
                    0, 0,
                    image.Width, image.Height,
                    GraphicsUnit.Pixel, attributes
                );
            }
            return opaque;
        }

        private void loadFromRIOButton_Click(object sender, EventArgs e)
        {

        }

        private void PoseVisualizer_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.closeMain();
        }

        private void GridCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (GridCheckBox.CheckState == CheckState.Unchecked)
            {
                mainField.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                mainField.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            }
            else
            {
                mainField.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
                mainField.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            }
        }
    }
}
