namespace VelocityMap
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Renci.SshNet;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Windows.Forms.DataVisualization.Charting;
    using MotionProfile;
    using static MotionProfile.ControlPoint;
    using MotionProfile.Spline;

    using MathNet.Numerics.Interpolation;

    using MotionProfile.SegmentedProfile;
    using System.Windows.Controls;
    using Renci.SshNet.Sftp;


    /// <summary>
    /// Defines the <see cref="MainForm" />
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Defines the fieldHeight
        /// </summary>
        ///
        // OLD 2019: 8230
        private int fieldHeight = 7908;

        /// <summary>
        /// Defines the fieldWidth
        /// </summary>
        // OLD 2019: 8230
        private int fieldWidth = 8016;

        internal int padding = 1;
        public List<ControlPoint> controlPointArray = new List<ControlPoint>();
        //public OutputPoints outputPoints = new OutputPoints();

        // new
        public double maxVelocity = 0;
        public double maxAcceleration = 0;
        public double maxJerk = 0;

        public List<Profile> profiles = new List<Profile>();
        public int newProfileCount = 0; // lol
        public int newPathCount = 0;
        public int pointSize = 100;

        Profile selectedProfile = null;
        ProfilePath selectedPath = null;
        ControlPoint placingPoint = null;

        ControlPoint clickedPoint = null;
        ProfilePath clickedPointPath = null;

        bool editing = false;
        int editedCell = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Load Form 1
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            SetupMainField();
        }

        /// <summary>
        /// Configures what the main field looks like.
        /// </summary>
        private void SetupMainField()
        {
            mainField.ChartAreas["field"].Axes[0].Minimum = 0;
            mainField.ChartAreas["field"].Axes[0].Maximum = fieldWidth;
            mainField.ChartAreas["field"].Axes[0].Interval = 1000;

            mainField.ChartAreas["field"].Axes[1].Minimum = 0;
            mainField.ChartAreas["field"].Axes[1].Maximum = fieldHeight;
            mainField.ChartAreas["field"].Axes[1].Interval = 1000;

            mainField.Series["background"].Points.AddXY(0, 0);
            mainField.Series["background"].Points.AddXY(fieldWidth, fieldHeight);

            Bitmap b = new Bitmap(VelocityMap.Properties.Resources._2019_field);
            NamedImage backImage = new NamedImage("Background", b);
            mainField.Images.Add(backImage);
            mainField.ChartAreas["field"].BackImageWrapMode = ChartImageWrapMode.Scaled;
            mainField.ChartAreas["field"].BackImage = "Background";

            maxVelocityInput.Text = Properties.Settings.Default.MaxVel.ToString();
            maxAccelerationInput.Text = Properties.Settings.Default.MaxAcc.ToString();
            maxRotationVelocityInput.Text = Properties.Settings.Default.MaxRotVel.ToString();
            maxRotationAccelerationInput.Text = Properties.Settings.Default.MaxRotAcc.ToString();
        }

        private void selectPoint(int index)
        {
            ControlPointTable.Rows[index].Selected = true;
        }

        private void MainField_MouseClick(object sender, MouseEventArgs e)
        {
            if (clickedPoint != null || e.Button != MouseButtons.Left) return;
            if (noSelectedProfile() || noSelectedPath()) return;

            if (placingPoint != null)
            {
                selectedPath.addControlPoint(placingPoint);
                placingPoint = null;
                ProfileEdit();
                UpdateField();
            }
            else
            {
                Chart chart = (Chart)sender;
                int x = (int)chart.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
                int y = (int)chart.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);

                if (x > 0 && y > 0 && x <= fieldWidth && y <= fieldHeight)
                {
                    placingPoint = new ControlPoint(x, y, 90);

                    ControlPointTable.Rows.Add(placingPoint.X, placingPoint.Y, placingPoint.Heading);
                    selectPoint(ControlPointTable.Rows.Count - 1);
                    DrawPoint(placingPoint, selectedPath);
                }
            }
        }

        private void mainField_MouseUp(object sender, MouseEventArgs e)
        {
            if (clickedPoint != null)
            {
                clickedPoint = null;
                UpdateField();
                ProfileEdit();
            }
        }

        /// <summary>
        /// Checks for a point selection for clicking and dragging.
        /// </summary>
        private void MainField_MouseDown(object sender, MouseEventArgs e)
        {
            if (placingPoint != null || !e.Button.HasFlag(MouseButtons.Left)) return;
            if (noSelectedProfile() || noSelectedPath()) return;

            Chart chart = (Chart)sender;
            int clickedX = (int)chart.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
            int clickedY = (int)chart.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);

            foreach (ProfilePath path in selectedProfile.paths)
            {
                foreach (ControlPoint point in path.controlPoints)
                {
                    if (Math.Abs(clickedX - point.X) < pointSize && Math.Abs(clickedY - point.Y) < pointSize)
                    {
                        clickedPoint = point;
                        clickedPointPath = path;
                        if (clickedPointPath == selectedPath) selectPoint(path.controlPoints.IndexOf(point));
                    }
                }
            }
        }

        /// <summary>
        /// The event that is called when the user mouse while above the main field.
        /// </summary>
        private void MainField_MouseMove(object sender, MouseEventArgs e)
        {
            Chart chart = (Chart)sender;

            //if the user is holding the left button while moving the mouse allow them to move the point.
            if (clickedPoint != null && e.Button.HasFlag(MouseButtons.Left))
            {
                int newX = (int)chart.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
                int newY = (int)chart.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);

                clickedPoint.X = newX;
                clickedPoint.Y = newY;
                if (clickedPointPath == selectedPath)
                {
                    ControlPointTable.SelectedRows[0].Cells[0].Value = newX;
                    ControlPointTable.SelectedRows[0].Cells[1].Value = newY;
                }

                DrawPath(clickedPointPath);
            }
            if (placingPoint != null)
            {
                int x = (int)chart.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
                int y = (int)chart.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);

                placingPoint.Heading = (int)(Math.Atan2(y - placingPoint.Y, x - placingPoint.X) * 180 / Math.PI + 360) % 360;
                ControlPointTable.Rows[ControlPointTable.Rows.Count - 1].Cells[2].Value = placingPoint.Heading;

                mainField.Series[placingPoint.Id].Points.Clear();
                int x1 = (int)(placingPoint.X + pointSize * Math.Cos(placingPoint.Heading * Math.PI / 180));
                int y1 = (int)(placingPoint.Y + pointSize * Math.Sin(placingPoint.Heading * Math.PI / 180));
                mainField.Series[placingPoint.Id].Points.AddXY(x1, y1);
                int x2 = (int)(placingPoint.X + 600 * Math.Cos(placingPoint.Heading * Math.PI / 180));
                int y2 = (int)(placingPoint.Y + 600 * Math.Sin(placingPoint.Heading * Math.PI / 180));
                mainField.Series[placingPoint.Id].Points.AddXY(x2, y2);
            }
        }

        /// <summary>
        /// The currently selected row from the commandPointList table.
        /// </summary>
        private int commandRowIndex;

        /// <summary>
        /// The currently selected row from the RioFilesList table.
        /// </summary>
        private int RioFilesRowIndex;


        /// <summary>
        /// Mirrors the selected path along the vertical axis.
        /// </summary>
        private void Invert_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile() || noSelectedPath()) return;
            selectedProfile.mirrorPath(selectedPath, fieldWidth);
            selectPath();
            ProfileEdit();
        }

        /// <summary>
        /// Mirrors all selected profile paths along the vertical axis.
        /// </summary>
        private void invertAll_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile() || noSelectedPath()) return;
            selectedProfile.mirrorAllPaths(fieldWidth);
            selectPath();
            ProfileEdit();
        }

        /// <summary>
        /// The event that is called when a rows state is changed ex: the row is selected.
        /// </summary>
        private void ControlPointsTable_RowStateChange(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (e.Row.Cells[0].Value == null && e.Row.Cells[1].Value == null && e.Row.Cells[2].Value == null) return;

            if (e.Row.Cells[0].Value == null || e.Row.Cells[0].Value.ToString() == "") e.Row.Cells[0].Value = 100;
            if (e.Row.Cells[1].Value == null || e.Row.Cells[1].Value.ToString() == "") e.Row.Cells[1].Value = 100;
            if (e.Row.Cells[2].Value == null || e.Row.Cells[2].Value.ToString() == "") e.Row.Cells[2].Value = 0;

            //If the state change is not a selection we don't care about it.
            if (e.StateChanged != DataGridViewElementStates.Selected) return;

            //Check to see if the row is selected because the selected event contains both unselecting and selecting.
            if (e.Row.Selected == true) UpdateField();
        }

        /// <summary>
        /// The event that is called when a rows state is changed ex: the row is selected.
        /// </summary>
        private void CommandPoints_RowStateChange(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            // Disabled
            return;

            if (e.StateChanged != DataGridViewElementStates.Selected)
            {
                return;
            }

            foreach (DataGridViewRow row in rioCommandsTable.Rows)
            {
                if (RowContainData(row, true))
                {
                    if (mainField.Series["path"].Points.Count >= int.Parse(row.Cells[0].Value.ToString()))
                    {
                        DataPoint p = mainField.Series["path"].Points[int.Parse(row.Cells[0].Value.ToString())];
                        p.Color = Color.Red;
                    }
                }
            }

            if (RowContainData(e.Row, true))
            {
                if (mainField.Series["path"].Points.Count >= int.Parse(e.Row.Cells[0].Value.ToString()))
                {
                    DataPoint p = mainField.Series["path"].Points[int.Parse(e.Row.Cells[0].Value.ToString())];
                    p.Color = Color.Blue;
                    p.MarkerStyle = MarkerStyle.Triangle;
                    p.MarkerSize = 10;
                }
            }
        }

        /// <summary>
        /// The event that is called when the user stopes editing a cell.
        /// </summary>
        private void ControlPoints_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //Check to see if the user is editing a cell that is in the third column.

            if (ControlPointTable.CurrentRow.Cells[0].Value == null && ControlPointTable.CurrentRow.Cells[1].Value == null && ControlPointTable.CurrentRow.Cells[1].Value == null)
            {
                return;
            }
            if (ControlPointTable.CurrentRow.Cells[0].Value == null || ControlPointTable.CurrentRow.Cells[0].Value.ToString() == "")
            {
                ControlPointTable.CurrentRow.Cells[0].Value = 100;
            }
            if (ControlPointTable.CurrentRow.Cells[1].Value == null || ControlPointTable.CurrentRow.Cells[1].Value.ToString() == "")
            {
                ControlPointTable.CurrentRow.Cells[1].Value = 100;
            }
            if (ControlPointTable.CurrentRow.Cells[2].Value == null || ControlPointTable.CurrentRow.Cells[2].Value.ToString() == "")
            {
                ControlPointTable.CurrentRow.Cells[2].Value = "+";
            }

            try
            {
                float.Parse(ControlPointTable.CurrentRow.Cells[0].Value.ToString());
            }
            catch (Exception)
            {
                ControlPointTable.CurrentRow.Cells[0].Value = 100;
            }
            try
            {
                float.Parse(ControlPointTable.CurrentRow.Cells[1].Value.ToString());
            }
            catch (Exception)
            {
                ControlPointTable.CurrentRow.Cells[1].Value = 100;
            }

            if (e.ColumnIndex == 2)
            {
                //If the cell contains a + or a - the ignore it. Else change the cell text to be a + signs.
                if (ControlPointTable.CurrentCell.Value.ToString() == "+" || ControlPointTable.CurrentCell.Value.ToString() == "-")
                {
                }
                else
                {
                    ControlPointTable.CurrentCell.Value = "+";
                }
            }
            UpdateField();
        }
        private void CommandPoints_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Disabled
            return;

            try
            {
                int.Parse(rioCommandsTable.CurrentRow.Cells[0].Value.ToString());
            }
            catch (Exception)
            {
                rioCommandsTable.CurrentRow.Cells[0].Value = 0;
            }

            UpdateField();
        }

        /// <summary>
        /// The event that is called when the user releases the mouse button while above the controlpoints cell.
        /// </summary>
        private void ControlPoints_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Disabled
            return;

            //make sure that the button that was released was the right mouse button.
            if (e.Button == MouseButtons.Right)
            {
                //Make sure that the cell that was selected was a cell that is real
                if (e.RowIndex >= 0)
                {
                    //on mouse up select that row.
                    this.ControlPointTable.Rows[e.RowIndex].Selected = true;
                    //When the row is selected set the rowindex to the index of the row that was just selected. (aka update the rowIndex value)
                    //this.clickedPointIndex = e.RowIndex;
                    //set the tables currentcell to the cell we just clicked.
                    this.ControlPointTable.CurrentCell = this.ControlPointTable.Rows[e.RowIndex].Cells[1];
                    //since we right clicked we open a context strip with things that allow us to delete and move the current row.
                    var relativeMousePosition = this.ControlPointTable.PointToClient(System.Windows.Forms.Cursor.Position);
                    this.contextMenuStrip2.Show(this.ControlPointTable, relativeMousePosition);
                }
            }

        }
        private void CommandPoints_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Disabled
            return;

            //make sure that the button that was released was the right mouse button.
            if (e.Button == MouseButtons.Right)
            {
                //Make sure that the cell that was selected was a cell that is real
                if (e.RowIndex >= 0)
                {
                    //on mouse up select that row.
                    this.rioCommandsTable.Rows[e.RowIndex].Selected = true;
                    //When the row is selected set the rowindex to the index of the row that was just selected. (aka update the rowIndex value)
                    this.commandRowIndex = e.RowIndex;
                    //set the tables currentcell to the cell we just clicked.
                    this.rioCommandsTable.CurrentCell = this.rioCommandsTable.Rows[e.RowIndex].Cells[1];
                    //since we right clicked we open a context strip with things that allow us to delete and move the current row.
                    var relativeMousePosition = this.rioCommandsTable.PointToClient(System.Windows.Forms.Cursor.Position);
                    this.commandPointListMenuStrip.Show(this.rioCommandsTable, relativeMousePosition);
                }
            }
        }

        /// <summary>
        /// The event that is called when the user clicks the delete button in the context strip.
        /// </summary>
        private void Delete_Click(object sender, EventArgs e)
        {
            //Delete the row that is selected.
            //ControlPointTable.Rows.RemoveAt(clickedPointIndex);
            //Reload the points because we just deleted one and we need the rest of the program to know.
            UpdateField();
        }

        private void Delete_Click_commandPoints(object sender, EventArgs e)
        {
            //Make sure we are not deleting the always blank last row.
            if (commandRowIndex != rioCommandsTable.RowCount - 1)
            {
                //Delete the row that is selected.
                rioCommandsTable.Rows.RemoveAt(commandRowIndex);
            }
            //Reload the points because we just deleted one and we need the rest of the program to know.
            UpdateField();
        }

        /// <summary>
        /// Clear path button clicked event
        /// </summary>
        private void ClearCP_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile() || noSelectedPath()) return;
            selectedPath.clearPoints();
            selectPath();
            ProfileEdit();
        }

        /// <summary>
        /// The event that is called when the user clicks the insert above button in the context stip.
        /// </summary>
        private void InsertAbove_Click(object sender, EventArgs e)
        {
            //insert a new row at the selected index. (this will push the current index down one.)
            mainField.Series["cp"].Points.AddXY(100, 100);
            //ControlPointTable.Rows.Insert(clickedPointIndex, 100, 100, "+");
            UpdateField();
        }

        /// <summary>
        /// The event that is called when the user clicks the insert above button in the context stip.
        /// </summary>
        private void InsertAbove_Click_commandPoints(object sender, EventArgs e)
        {
            //insert a new row at the selected index. (this will push the current index down one.)
            rioCommandsTable.Rows.Insert(commandRowIndex);

        }

        /// <summary>
        /// The event that is called when the user clicks the insert below button in the context stip.
        /// </summary>

        private void InsertBelow_Click(object sender, EventArgs e)
        {
            //insert a new row at the selected index plus one.
            //ControlPointTable.Rows.Insert(clickedPointIndex + 1, 100, 100, "+");
            mainField.Series["cp"].Points.AddXY(100, 100);

            UpdateField();
        }

        /// <summary>
        /// The event that is called when the user clicks the insert below button in the context stip.
        /// </summary>

        private void InsertBelow_Click_commandPoints(object sender, EventArgs e)
        {
            //insert a new row at the selected index plus one.
            if (!(rioCommandsTable.Rows.Count >= commandRowIndex))
                rioCommandsTable.Rows.Insert(commandRowIndex + 1);

        }

        /// <summary>
        /// The event that is called when the user clicks the move up button in the context stip.
        /// </summary>
        private void BtnUp_Click(object sender, EventArgs e)
        {
            //lets convert our object name because I copied this from the internet and am to lazy to change it.
            DataGridView dgv = ControlPointTable;
            try
            {
                int totalRows = dgv.Rows.Count;
                // get index of the row for the selected cell
                int rowIndex = dgv.SelectedCells[0].OwningRow.Index;
                if (rowIndex == 0)
                    return;
                // get index of the column for the selected cell
                int colIndex = dgv.SelectedCells[0].OwningColumn.Index;
                DataGridViewRow selectedRow = dgv.Rows[rowIndex];
                dgv.Rows.Remove(selectedRow);
                dgv.Rows.Insert(rowIndex - 1, selectedRow);
                dgv.ClearSelection();
                dgv.Rows[rowIndex - 1].Cells[colIndex].Selected = true;
            }
            catch { }
            UpdateField();
        }

        /// <summary>
        /// The event that is called when the user clicks the move up button in the context stip.
        /// </summary>
        private void BtnUp_Click_commandPoints(object sender, EventArgs e)
        {
            //lets convert our object name because I copied this from the internet and am to lazy to change it.
            DataGridView dgv = rioCommandsTable;
            try
            {
                int totalRows = dgv.Rows.Count;
                // get index of the row for the selected cell
                int commandRowIndex = dgv.SelectedCells[0].OwningRow.Index;
                if (commandRowIndex == 0)
                    return;
                // get index of the column for the selected cell
                int colIndex = dgv.SelectedCells[0].OwningColumn.Index;
                DataGridViewRow selectedRow = dgv.Rows[commandRowIndex];
                dgv.Rows.Remove(selectedRow);
                dgv.Rows.Insert(commandRowIndex - 1, selectedRow);
                dgv.ClearSelection();
                dgv.Rows[commandRowIndex - 1].Cells[colIndex].Selected = true;
            }
            catch { }
        }

        /// <summary>
        /// The event that is called when the user clicks the move down button in the context stip.
        /// </summary>
        private void BtnDown_Click(object sender, EventArgs e)
        {
            DataGridView dgv = ControlPointTable;
            try
            {
                //lets convert our object name because I copied this from the internet and am to lazy to change it.

                int totalRows = dgv.Rows.Count;
                // get index of the row for the selected cell
                int rowIndex = dgv.SelectedCells[0].OwningRow.Index;
                if (rowIndex == totalRows - 2)
                    return;

                // get index of the column for the selected cell
                int colIndex = dgv.SelectedCells[0].OwningColumn.Index;
                DataGridViewRow selectedRow = dgv.Rows[rowIndex];
                dgv.Rows.Remove(selectedRow);
                dgv.Rows.Insert(rowIndex + 1, selectedRow);
                dgv.ClearSelection();
                dgv.Rows[rowIndex + 1].Cells[colIndex].Selected = true;
            }
            catch { }
            UpdateField();
        }

        /// <summary>
        /// The event that is called when the user clicks the move down button in the context stip.
        /// </summary>
        private void BtnDown_Click_commandPoints(object sender, EventArgs e)
        {
            DataGridView dgv = rioCommandsTable;
            try
            {
                //lets convert our object name because I copied this from the internet and am to lazy to change it.

                int totalRows = dgv.Rows.Count;
                // get index of the row for the selected cell
                int commandRowIndex = dgv.SelectedCells[0].OwningRow.Index;
                if (commandRowIndex == totalRows - 2)
                    return;
                // get index of the column for the selected cell
                int colIndex = dgv.SelectedCells[0].OwningColumn.Index;
                DataGridViewRow selectedRow = dgv.Rows[commandRowIndex];
                dgv.Rows.Remove(selectedRow);
                dgv.Rows.Insert(commandRowIndex + 1, selectedRow);
                dgv.ClearSelection();
                dgv.Rows[commandRowIndex + 1].Cells[colIndex].Selected = true;
            }
            catch { }

        }

        private void DrawPoint(ControlPoint point, ProfilePath path)
        {
            mainField.Series[path.id + "-points"].Points.AddXY(point.X, point.Y);

            if (mainField.Series.IndexOf(point.Id) == -1)
            {
                mainField.Series.Add(point.Id);
                mainField.Series[point.Id].ChartType = SeriesChartType.Line;
                mainField.Series[point.Id].Color = path == selectedPath ? Color.Red : Color.DarkRed;
                mainField.Series[point.Id].BorderWidth = 2;
            }
            mainField.Series[point.Id].Points.Clear();

            int x1 = (int)(point.X + pointSize * Math.Cos(point.Heading * Math.PI / 180));
            int y1 = (int)(point.Y + pointSize * Math.Sin(point.Heading * Math.PI / 180));
            mainField.Series[point.Id].Points.AddXY(x1, y1);
            int x2 = (int)(point.X + (path == selectedPath ? 600 : 300) * Math.Cos(point.Heading * Math.PI / 180));
            int y2 = (int)(point.Y + (path == selectedPath ? 600 : 300) * Math.Sin(point.Heading * Math.PI / 180));
            mainField.Series[point.Id].Points.AddXY(x2, y2);
        }

        private void DrawPath(ProfilePath path)
        {
            if (!showPathsCheckbox.Checked && path != selectedPath) return;

            int seriesIndex = mainField.Series.IndexOf(path.id + "-path");
            if (seriesIndex != -1) mainField.Series.RemoveAt(seriesIndex);
            seriesIndex = mainField.Series.IndexOf(path.id + "-left");
            if (seriesIndex != -1) mainField.Series.RemoveAt(seriesIndex);
            seriesIndex = mainField.Series.IndexOf(path.id + "-right");
            if (seriesIndex != -1) mainField.Series.RemoveAt(seriesIndex);
            seriesIndex = mainField.Series.IndexOf(path.id + "-points");
            if (seriesIndex != -1) mainField.Series.RemoveAt(seriesIndex);

            mainField.Series.Add(path.id + "-path");
            mainField.Series[path.id + "-path"].ChartArea = "field";
            mainField.Series[path.id + "-path"].ChartType = SeriesChartType.Line;
            mainField.Series[path.id + "-path"].Color = path == selectedPath ? Color.Aqua : Color.Blue;
            mainField.Series[path.id + "-path"].MarkerSize = 2;

            mainField.Series.Add(path.id + "-left");
            mainField.Series[path.id + "-left"].ChartArea = "field";
            mainField.Series[path.id + "-left"].ChartType = SeriesChartType.Line;
            mainField.Series[path.id + "-left"].Color = Color.LightGray;
            mainField.Series[path.id + "-left"].MarkerSize = 2;
            //mainField.Series[path.id + "-left"].BorderWidth = 2;

            mainField.Series.Add(path.id + "-right");
            mainField.Series[path.id + "-right"].ChartArea = "field";
            mainField.Series[path.id + "-right"].ChartType = SeriesChartType.Line;
            mainField.Series[path.id + "-right"].Color = Color.LightGray;
            mainField.Series[path.id + "-right"].MarkerSize = 2;

            mainField.Series.Add(path.id + "-points");
            mainField.Series[path.id + "-points"].ChartArea = "field";
            mainField.Series[path.id + "-points"].ChartType = SeriesChartType.Point;
            mainField.Series[path.id + "-points"].Color = path == selectedPath ? Color.Lime : Color.Green;
            mainField.Series[path.id + "-points"].MarkerSize = 10;
            mainField.Series[path.id + "-points"].MarkerStyle = MarkerStyle.Diamond;

            foreach (ControlPoint point in path.controlPoints)
            {
                DrawPoint(point, path);
            }

            if (path.controlPoints.Count < 2) return;

            //outputPoints = new OutputPoints();
            double Posoffset = 0;
            double Timeoffset = 0;
            List<SplinePoint> pointList = new List<SplinePoint>();

            SplinePath.GenSpline(path.controlPoints);
            List<VelocityPoint> velocityPoints = new VelocityGenerator(
                (double)Properties.Settings.Default.MaxVel,
                (double)Properties.Settings.Default.MaxAcc,
                (double)Properties.Settings.Default.MaxJerk,
                .025
            ).GeneratePoints(SplinePath.getLength());
            List<ControlPointSegment> splineSegments = SplinePath.GenSpline(path.controlPoints, velocityPoints);

            SplinePoint lastPoint = splineSegments[0].points[0];
            SplinePoint currentPoint;
            foreach (ControlPointSegment segment in splineSegments)
            {
                if (segment.points.Count == 0)
                {
                    MessageBox.Show("Generation Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                currentPoint = segment.points[Math.Min(2, segment.points.Count - 1)];
                double pointDistance = distance(currentPoint, lastPoint);
                path.controlPoints[splineSegments.IndexOf(segment)].setTangents(
                    (currentPoint.X - lastPoint.X) / pointDistance,
                    (currentPoint.Y - lastPoint.Y) / pointDistance
                );
                // Additional calculation for point at the end of the path
                if (segment == splineSegments.Last())
                {
                    lastPoint = segment.points[Math.Max(0, segment.points.Count - 3)];
                    currentPoint = segment.points.Last();
                    pointDistance = distance(currentPoint, lastPoint);
                    path.controlPoints.Last().setTangents(
                        (currentPoint.X - lastPoint.X) / pointDistance,
                        (currentPoint.Y - lastPoint.Y) / pointDistance
                    );
                }
                lastPoint = segment.points[Math.Max(0, segment.points.Count - 3)];

                foreach (SplinePoint point in segment.points)
                {
                    mainField.Series[path.id + "-path"].Points.AddXY((int)point.X, (int)point.Y);
                    pointList.Add(point);
                }
            }

            foreach (SplinePoint point in buildOffsetPoints(-Properties.Settings.Default.TrackWidth, pointList))
            {
                mainField.Series[path.id + "-left"].Points.AddXY((int)point.X, (int)point.Y);
            }
            foreach (SplinePoint point in buildOffsetPoints(Properties.Settings.Default.TrackWidth, pointList))
            {
                mainField.Series[path.id + "-right"].Points.AddXY((int)point.X, (int)point.Y);
            }

            if (path != selectedPath) return;
            kinematicsChart.Series["Position"].Points.Clear();
            kinematicsChart.Series["Velocity"].Points.Clear();
            kinematicsChart.Series["Acceleration"].Points.Clear();
            //AngleChart.Series["Angle"].Points.Clear();
            foreach (VelocityPoint point in velocityPoints)
            {
                kinematicsChart.Series["Position"].Points.AddXY(point.Time + Timeoffset, point.Pos + Posoffset);
                kinematicsChart.Series["Velocity"].Points.AddXY(point.Time + Timeoffset, point.Vel);
                kinematicsChart.Series["Acceleration"].Points.AddXY(point.Time + Timeoffset, point.Acc);
                //AngleChart.Series["Angle"].Points.AddXY(point.Time + Timeoffset, outputPoints.angle[x]);
            }
        }

        private void UpdateField()
        {
            kinematicsChart.Series["Position"].Points.Clear();
            kinematicsChart.Series["Velocity"].Points.Clear();
            kinematicsChart.Series["Acceleration"].Points.Clear();
            //AngleChart.Series["Angle"].Points.Clear();
            for (int series = 1; series < mainField.Series.Count; series++)
            {
                mainField.Series[series].Points.Clear();
            }

            if (noSelectedProfile()) return;

            if (placingPoint != null)
            {
                DrawPoint(placingPoint, selectedPath);
            }

            foreach (ProfilePath path in selectedProfile.paths)
            {
                DrawPath(path);
            }
            setStatus("", false);
        }

        public void UpdateTables()
        {
            profileTable.Rows.Clear();
            pathTable.Rows.Clear();
            ControlPointTable.Rows.Clear();

            foreach (Profile profile in profiles)
            {
                profileTable.Rows.Add(profile.Name, profile.Edited);
            }
            if (!noSelectedProfile())
            {
                foreach (ProfilePath path in selectedProfile.paths)
                {
                    pathTable.Rows.Add(path.Name);
                }
                if (!noSelectedPath())
                {
                    foreach (ControlPoint point in selectedPath.controlPoints)
                    {
                        ControlPointTable.Rows.Add(point.X, point.Y, point.Heading);
                    }
                }
            }
        }

        public List<SplinePoint> buildOffsetPoints(float offset, List<SplinePoint> pointList)
        {
            List<SplinePoint> splinePoints = new List<SplinePoint>();
            SplinePoint lastPoint = new SplinePoint(0, 0, 0);

            foreach (SplinePoint point in pointList)
            {
                if (lastPoint.X != 0 && lastPoint.Y != 0)
                {
                    splinePoints.Add(new OffsetSegment(lastPoint, point).perp(offset/2));
                }
                lastPoint = point;
            }

            return splinePoints;
        }

        private void SaveAllProfiles(object sender, EventArgs e)
        {
            if (profiles.Count == 0) return;

            FolderBrowserDialog browser = new FolderBrowserDialog();
            browser.Description = "Save all motion profiles to folder\n\nCAUTION: Existing profiles will be overridden!";

            if (browser.ShowDialog() != DialogResult.OK) return;

            foreach (Profile profile in profiles)
            {
                string filePath = Path.Combine(browser.SelectedPath, profile.Name.Replace(' ', '_') + ".mp");
                using (var writer = new StreamWriter(filePath))
                {
                    writer.Write(profile.toJSON().ToString());
                }
            }
        }

        /// <summary>
        /// Save the selected motion profile to a file.
        /// </summary>
        private void SaveSelectedProfile(object sender, EventArgs e)
        {
            if (noSelectedProfile()) return;

            SaveFileDialog browser = new SaveFileDialog();
            browser.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            browser.FileName = selectedProfile.Name.Replace(' ', '_');
            browser.Filter = "Motion Profile|*.mp;";
            browser.Title = "Save motion profile file";

            if (browser.ShowDialog() != DialogResult.OK || browser.FileName.Trim().Length > 3) return;

            string filePath = Path.Combine(
                Path.GetDirectoryName(browser.FileName.Trim()),
                Path.GetFileNameWithoutExtension(browser.FileName.Trim()) + ".mp"
            );

            using (var writer = new StreamWriter(filePath))
            {
                writer.Write(selectedProfile.toJSON().ToString());
            }
        }

        /// <summary>
        /// Loads profile json files into the profiler from a file dialog
        /// </summary>
        private void LoadProfilesFromFiles(object sender, EventArgs e)
        {
            openFilesDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            openFilesDialog.FileName = "";
            openFilesDialog.Filter = "MotionProfile Data (*.mp)|*.mp";
            openFilesDialog.Title = "Select motion profile files to load";
            openFilesDialog.Multiselect = true;

            if (openFilesDialog.ShowDialog() != DialogResult.OK) return;

            foreach (string filename in openFilesDialog.FileNames)
            {
                using (StreamReader fileReader = new StreamReader(filename))
                {
                    try
                    {
                        profiles.Add(new Profile(JObject.Parse(fileReader.ReadToEnd())));
                        profileTable.Rows.Add(profiles.Last().Name, profiles.Last().Edited);
                    }
                    catch
                    {
                        MessageBox.Show("Error loading file " + filename, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Deploys the currently selected profile directly to the robot for testing
        /// </summary>
        private void DeploySelectedProfile(object sender, EventArgs e)
        {
            if (noSelectedProfile() || !selectedProfile.isValid())
            {
                setStatus("Selected profile is invalid", true);
                return;
            }
            Cursor = Cursors.WaitCursor;

            SftpClient sftp = new SftpClient(
                Properties.Settings.Default.IpAddress,
                Properties.Settings.Default.Username,
                Properties.Settings.Default.Password
            );

            try
            {
                setStatus("Establishing RIO connection...", false);
                sftp.Connect();

                if (!sftp.Exists(Properties.Settings.Default.RioLocation)) sftp.CreateDirectory(Properties.Settings.Default.RioLocation);

                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(selectedProfile.toJSON().ToString()));
                sftp.UploadFile(stream, Path.Combine(Properties.Settings.Default.RioLocation, "test_deploy.mp"));
                
                setStatus("Profile uploaded successfully", false);
                sftp.Disconnect();
            }
            catch (Renci.SshNet.Common.SshConnectionException exception)
            {
                Console.WriteLine("SshConnectionException, source: {0}", exception.StackTrace);
                setStatus("Failed to establish connection", true);
            }
            catch (System.Net.Sockets.SocketException exception)
            {
                Console.WriteLine("SocketException, source: {0}", exception.StackTrace);
                setStatus("Failed to establish connection", true);
            }
            catch (Renci.SshNet.Common.SftpPermissionDeniedException exception)
            {
                Console.WriteLine("SftpPermissionDeniedException, source: {0}", exception.StackTrace);
                setStatus("Permission denied", true);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception, source: {0}", exception.StackTrace);
                setStatus("Failed to upload profile to RIO", true);
            }

            Cursor = Cursors.Default;
        }

        private void LoadProfilesFromRIO(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            SftpClient sftp = new SftpClient(
                Properties.Settings.Default.IpAddress,
                Properties.Settings.Default.Username,
                Properties.Settings.Default.Password
            );

            try
            {
                setStatus("Establishing RIO connection...", false);
                sftp.Connect();

                if (!sftp.Exists(Properties.Settings.Default.RioLocation))
                {
                    sftp.CreateDirectory(Properties.Settings.Default.RioLocation);
                    setStatus("No motion profiles found at RIO directory", false);
                    return;
                }

                bool foundFiles = false;
                foreach (SftpFile file in sftp.ListDirectory(Properties.Settings.Default.RioLocation))
                {
                    if (!file.Name.Contains(".mp")) continue;
                    foundFiles = true;

                    StreamReader reader = sftp.OpenText(file.FullName);
                    profiles.Add(new Profile(JObject.Parse(reader.ReadToEnd())));
                    profileTable.Rows.Add(profiles.Last().Name, profiles.Last().Edited);
                }
                if (foundFiles) setStatus("Profiles loaded from RIO", false);
                else setStatus("No motion profiles found at RIO directory", false);
                
                sftp.Disconnect();
            }
            catch (Renci.SshNet.Common.SshConnectionException exception)
            {
                Console.WriteLine("SshConnectionException, source: {0}", exception.StackTrace);
                setStatus("Failed to establish connection", true);
            }
            catch (System.Net.Sockets.SocketException exception)
            {
                Console.WriteLine("SocketException, source: {0}", exception.StackTrace);
                setStatus("Failed to establish connection", true);
            }
            catch (Renci.SshNet.Common.SftpPermissionDeniedException exception)
            {
                Console.WriteLine("SftpPermissionDeniedException, source: {0}", exception.StackTrace);
                setStatus("Permission denied", true);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception, source: {0}", exception.StackTrace);
                setStatus("Failed to load RIO profiles", true);
            }

            Cursor = Cursors.Default;
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
        bool isFileMenuItemOpen = false;

        private void FileToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            ToolStripMenuItem TSMI = sender as ToolStripMenuItem;
            TSMI.ForeColor = Color.Black;
            isFileMenuItemOpen = true;
        }

        private void FileToolStripMenuItem_DropDownClosed(object sender, EventArgs e)
        {
            ToolStripMenuItem TSMI = sender as ToolStripMenuItem;
            TSMI.ForeColor = Color.White;
            isFileMenuItemOpen = false;
        }

        private void FileToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            ToolStripMenuItem TSMI = sender as ToolStripMenuItem;
            TSMI.ForeColor = Color.Black;
        }

        private void FileToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            ToolStripMenuItem TSMI = sender as ToolStripMenuItem;
            if (isFileMenuItemOpen)
                TSMI.ForeColor = Color.Black;
            else
                TSMI.ForeColor = Color.White;
        }

        private void RioFiles_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            //make sure that the button that was released was the right mouse button.
            if (e.Button == MouseButtons.Right)
            {
                //Make sure that the cell that was selected was a cell that is real
                if (e.RowIndex >= 0)
                {
                    //on mouse up select that row.
                    this.profileTable.Rows[e.RowIndex].Selected = true;
                    //When the row is selected set the rowindex to the index of the row that was just selected. (aka update the rowIndex value)
                    //this.clickedPointIndex = e.RowIndex;
                    //set the tables currentcell to the cell we just clicked.
                    this.profileTable.CurrentCell = this.profileTable.Rows[e.RowIndex].Cells[1];
                    //since we right clicked we open a context strip with things that allow us to delete and move the current row.
                    var relativeMousePosition = this.ControlPointTable.PointToClient(System.Windows.Forms.Cursor.Position);
                    this.rioFilesContextMenuStrip.Show(this.ControlPointTable, relativeMousePosition);
                }
            }
        }

        private void RioFilesLoad(object sender, EventArgs e)
        {

            if (profileTable.Rows[RioFilesRowIndex].Cells[0].Value == null)
            {
                return;
            }
            if (profileTable.Rows[RioFilesRowIndex].Cells[0].Value.ToString().Equals(""))
            {
                return;
            }
            if (!MessageBox.Show("Your current profile will be over written are you sure you would like to contine?", "Warning!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning).Equals(DialogResult.Yes))
                return;
            SftpClient sftp = new SftpClient(Properties.Settings.Default.IpAddress, Properties.Settings.Default.Username, Properties.Settings.Default.Password);

            try
            {
                sftp.Connect();
            }
            catch (Exception e1)
            {
                //Make sure that we are connected to the robot.
                Console.WriteLine("IOException source: {0}", e1.StackTrace);
                MessageBox.Show("Unable to connect to host!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            String RioProfilePath = Path.Combine(Properties.Settings.Default.RioLocation, profileTable.Rows[RioFilesRowIndex].Cells[0].Value.ToString().Replace(".json", ".mp"));
            String tempFileName = Path.Combine(Path.GetTempPath(), profileTable.Rows[RioFilesRowIndex].Cells[0].Value.ToString());
            if (!sftp.Exists(Properties.Settings.Default.RioLocation))
            {
                MessageBox.Show("Could not find motion profile path on rio!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!sftp.Exists(RioProfilePath))
            {
                MessageBox.Show("Could not find specified motion profile on rio!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                using (var file = File.OpenWrite(tempFileName))
                {
                    sftp.DownloadFile(RioProfilePath, file);

                }
                using (var reader1 = new System.IO.StreamReader(tempFileName))
                {
                    //First clear out our points.
                    ControlPointTable.Rows.Clear();
                    rioCommandsTable.Rows.Clear();
                    //Read the file and load our points and other variables.
                    string json = reader1.ReadToEnd();

                    JObject o = JObject.Parse(json);

                    maxVelocityInput.Text = (string)o["Max Velocity"];
                    //wheel.Text = (string)o["Wheel Diameter"];

                    //profileNameInput.Text = (string)o["Profile Name"];

                    JArray a = (JArray)o["Points"];

                    for (int x = 0; x <= a.Count - 1; x++)
                    {
                        ControlPointTable.Rows.Add(float.Parse((string)a[x][0]), float.Parse((string)a[x][1]), (string)a[x][2]);
                    }

                    JArray CommandPointsArray = (JArray)o["CommandPoints"];

                    for (int x = 0; x <= CommandPointsArray.Count - 1; x++)
                    {
                        rioCommandsTable.Rows.Add(int.Parse((string)CommandPointsArray[x][0]), (string)CommandPointsArray[x][1]);
                    }
                }
                //Run the apply so that it looks like where we left off.
                UpdateField();
            }
            catch (Exception)
            {
                MessageBox.Show("Could not download specified motion profile on rio!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void RioFiles_RowStateChange(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (e.StateChanged != DataGridViewElementStates.Selected)
            {
                return;
            }
            if (e.Row.Selected == true)
            {
                RioFilesRowIndex = e.Row.Index;
            }
        }

        private void About_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.Show();
        }

        private Boolean RowContainData(DataGridViewRow row, Boolean scanWholeRow)
        {
            if (!scanWholeRow)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null)
                        if (!cell.Value.ToString().Equals(""))
                            return true;
                }
            }
            else
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value == null)
                    {
                        return false;
                    }
                    if (cell.Value.ToString().Equals(""))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private void ControlPointTable_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //updateControlPointArray();
        }

        private void ControlPointTable_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            //updateControlPointArray();
        }
        private void updateControlPointArray()
        {
            controlPointArray.Clear();
            foreach (DataGridViewRow row in ControlPointTable.Rows)
            {
                //Make sure that the row contains something that we care about.
                //If the x cell is not empty.
                if (row.Cells[0].Value == null && row.Cells[1].Value == null && row.Cells[2].Value == null)
                {
                    continue;
                }
                if (row.Cells[0].Value == null || row.Cells[0].Value.ToString().Equals(""))
                {
                    row.Cells[0].Value = 0;
                }
                if (row.Cells[1].Value == null || row.Cells[1].Value.ToString().Equals(""))
                {
                    row.Cells[1].Value = 0;
                }
                if (row.Cells[2].Value == null || row.Cells[2].Value.ToString().Equals(""))
                {
                    row.Cells[2].Value = 0;
                }

                //Add the data to the control point array.
                //controlPointArray.Add(new ControlPoint(float.Parse(row.Cells[0].Value.ToString()), float.Parse(row.Cells[1].Value.ToString()), '+', row.Selected));
            }
            //DrawControlPoints();
        }
        public String[] indexcolors = new String[]{
        "#000000", "#FFFF00", "#1CE6FF", "#FF34FF", "#FF4A46", "#008941", "#006FA6", "#A30059",
        "#FFDBE5", "#7A4900", "#0000A6", "#63FFAC", "#B79762", "#004D43", "#8FB0FF", "#997D87",
        "#5A0007", "#809693", "#FEFFE6", "#1B4400", "#4FC601", "#3B5DFF", "#4A3B53", "#FF2F80",
        "#61615A", "#BA0900", "#6B7900", "#00C2A0", "#FFAA92", "#FF90C9", "#B903AA", "#D16100",
        "#DDEFFF", "#000035", "#7B4F4B", "#A1C299", "#300018", "#0AA6D8", "#013349", "#00846F",
        "#372101", "#FFB500", "#C2FFED", "#A079BF", "#CC0744", "#C0B9B2", "#C2FF99", "#001E09",
        "#00489C", "#6F0062", "#0CBD66", "#EEC3FF", "#456D75", "#B77B68", "#7A87A1", "#788D66",
        "#885578", "#FAD09F", "#FF8A9A", "#D157A0", "#BEC459", "#456648", "#0086ED", "#886F4C",
        "#34362D", "#B4A8BD", "#00A6AA", "#452C2C", "#636375", "#A3C8C9", "#FF913F", "#938A81",
        "#575329", "#00FECF", "#B05B6F", "#8CD0FF", "#3B9700", "#04F757", "#C8A1A1", "#1E6E00",
        "#7900D7", "#A77500", "#6367A9", "#A05837", "#6B002C", "#772600", "#D790FF", "#9B9700",
        "#549E79", "#FFF69F", "#201625", "#72418F", "#BC23FF", "#99ADC0", "#3A2465", "#922329",
        "#5B4534", "#FDE8DC", "#404E55", "#0089A3", "#CB7E98", "#A4E804", "#324E72", "#6A3A4C",
        "#83AB58", "#001C1E", "#D1F7CE", "#004B28", "#C8D0F6", "#A3A489", "#806C66", "#222800",
        "#BF5650", "#E83000", "#66796D", "#DA007C", "#FF1A59", "#8ADBB4", "#1E0200", "#5B4E51",
        "#C895C5", "#320033", "#FF6832", "#66E1D3", "#CFCDAC", "#D0AC94", "#7ED379", "#012C58",
        "#7A7BFF", "#D68E01", "#353339", "#78AFA1", "#FEB2C6", "#75797C", "#837393", "#943A4D",
        "#B5F4FF", "#D2DCD5", "#9556BD", "#6A714A", "#001325", "#02525F", "#0AA3F7", "#E98176",
        "#DBD5DD", "#5EBCD1", "#3D4F44", "#7E6405", "#02684E", "#962B75", "#8D8546", "#9695C5",
        "#E773CE", "#D86A78", "#3E89BE", "#CA834E", "#518A87", "#5B113C", "#55813B", "#E704C4",
        "#00005F", "#A97399", "#4B8160", "#59738A", "#FF5DA7", "#F7C9BF", "#643127", "#513A01",
        "#6B94AA", "#51A058", "#A45B02", "#1D1702", "#E20027", "#E7AB63", "#4C6001", "#9C6966",
        "#64547B", "#97979E", "#006A66", "#391406", "#F4D749", "#0045D2", "#006C31", "#DDB6D0",
        "#7C6571", "#9FB2A4", "#00D891", "#15A08A", "#BC65E9", "#FFFFFE", "#C6DC99", "#203B3C",
        "#671190", "#6B3A64", "#F5E1FF", "#FFA0F2", "#CCAA35", "#374527", "#8BB400", "#797868",
        "#C6005A", "#3B000A", "#C86240", "#29607C", "#402334", "#7D5A44", "#CCB87C", "#B88183",
        "#AA5199", "#B5D6C3", "#A38469", "#9F94F0", "#A74571", "#B894A6", "#71BB8C", "#00B433",
        "#789EC9", "#6D80BA", "#953F00", "#5EFF03", "#E4FFFC", "#1BE177", "#BCB1E5", "#76912F",
        "#003109", "#0060CD", "#D20096", "#895563", "#29201D", "#5B3213", "#A76F42", "#89412E",
        "#1A3A2A", "#494B5A", "#A88C85", "#F4ABAA", "#A3F3AB", "#00C6C8", "#EA8B66", "#958A9F",
        "#BDC9D2", "#9FA064", "#BE4700", "#658188", "#83A485", "#453C23", "#47675D", "#3A3F00",
        "#061203", "#DFFB71", "#868E7E", "#98D058", "#6C8F7D", "#D7BFC2", "#3C3E6E", "#D83D66",
        "#2F5D9B", "#6C5E46", "#D25B88", "#5B656C", "#00B57F", "#545C46", "#866097", "#365D25",
        "#252F99", "#00CCFF", "#674E60", "#FC009C", "#92896B", "#1E2324", "#DEC9B2", "#9D4948",
        "#85ABB4", "#342142", "#D09685", "#A4ACAC", "#00FFFF", "#AE9C86", "#742A33", "#0E72C5",
        "#AFD8EC", "#C064B9", "#91028C", "#FEEDBF", "#FFB789", "#9CB8E4", "#AFFFD1", "#2A364C",
        "#4F4A43", "#647095", "#34BBFF", "#807781", "#920003", "#B3A5A7", "#018615", "#F1FFC8",
        "#976F5C", "#FF3BC1", "#FF5F6B", "#077D84", "#F56D93", "#5771DA", "#4E1E2A", "#830055",
        "#02D346", "#BE452D", "#00905E", "#BE0028", "#6E96E3", "#007699", "#FEC96D", "#9C6A7D",
        "#3FA1B8", "#893DE3", "#79B4D6", "#7FD4D9", "#6751BB", "#B28D2D", "#E27A05", "#DD9CB8",
        "#AABC7A", "#980034", "#561A02", "#8F7F00", "#635000", "#CD7DAE", "#8A5E2D", "#FFB3E1",
        "#6B6466", "#C6D300", "#0100E2", "#88EC69", "#8FCCBE", "#21001C", "#511F4D", "#E3F6E3",
        "#FF8EB1", "#6B4F29", "#A37F46", "#6A5950", "#1F2A1A", "#04784D", "#101835", "#E6E0D0",
        "#FF74FE", "#00A45F", "#8F5DF8", "#4B0059", "#412F23", "#D8939E", "#DB9D72", "#604143",
        "#B5BACE", "#989EB7", "#D2C4DB", "#A587AF", "#77D796", "#7F8C94", "#FF9B03", "#555196",
        "#31DDAE", "#74B671", "#802647", "#2A373F", "#014A68", "#696628", "#4C7B6D", "#002C27",
        "#7A4522", "#3B5859", "#E5D381", "#FFF3FF", "#679FA0", "#261300", "#2C5742", "#9131AF",
        "#AF5D88", "#C7706A", "#61AB1F", "#8CF2D4", "#C5D9B8", "#9FFFFB", "#BF45CC", "#493941",
        "#863B60", "#B90076", "#003177", "#C582D2", "#C1B394", "#602B70", "#887868", "#BABFB0",
        "#030012", "#D1ACFE", "#7FDEFE", "#4B5C71", "#A3A097", "#E66D53", "#637B5D", "#92BEA5",
        "#00F8B3", "#BEDDFF", "#3DB5A7", "#DD3248", "#B6E4DE", "#427745", "#598C5A", "#B94C59",
        "#8181D5", "#94888B", "#FED6BD", "#536D31", "#6EFF92", "#E4E8FF", "#20E200", "#FFD0F2",
        "#4C83A1", "#BD7322", "#915C4E", "#8C4787", "#025117", "#A2AA45", "#2D1B21", "#A9DDB0",
        "#FF4F78", "#528500", "#009A2E", "#17FCE4", "#71555A", "#525D82", "#00195A", "#967874",
        "#555558", "#0B212C", "#1E202B", "#EFBFC4", "#6F9755", "#6F7586", "#501D1D", "#372D00",
        "#741D16", "#5EB393", "#B5B400", "#DD4A38", "#363DFF", "#AD6552", "#6635AF", "#836BBA",
        "#98AA7F", "#464836", "#322C3E", "#7CB9BA", "#5B6965", "#707D3D", "#7A001D", "#6E4636",
        "#443A38", "#AE81FF", "#489079", "#897334", "#009087", "#DA713C", "#361618", "#FF6F01",
        "#006679", "#370E77", "#4B3A83", "#C9E2E6", "#C44170", "#FF4526", "#73BE54", "#C4DF72",
        "#ADFF60", "#00447D", "#DCCEC9", "#BD9479", "#656E5B", "#EC5200", "#FF6EC2", "#7A617E",
        "#DDAEA2", "#77837F", "#A53327", "#608EFF", "#B599D7", "#A50149", "#4E0025", "#C9B1A9",
        "#03919A", "#1B2A25", "#E500F1", "#982E0B", "#B67180", "#E05859", "#006039", "#578F9B",
        "#305230", "#CE934C", "#B3C2BE", "#C0BAC0", "#B506D3", "#170C10", "#4C534F", "#224451",
        "#3E4141", "#78726D", "#B6602B", "#200441", "#DDB588", "#497200", "#C5AAB6", "#033C61",
        "#71B2F5", "#A9E088", "#4979B0", "#A2C3DF", "#784149", "#2D2B17", "#3E0E2F", "#57344C",
        "#0091BE", "#E451D1", "#4B4B6A", "#5C011A", "#7C8060", "#FF9491", "#4C325D", "#005C8B",
        "#E5FDA4", "#68D1B6", "#032641", "#140023", "#8683A9", "#CFFF00", "#A72C3E", "#34475A",
        "#B1BB9A", "#B4A04F", "#8D918E", "#A168A6", "#813D3A", "#425218", "#DA8386", "#776133",
        "#563930", "#8498AE", "#90C1D3", "#B5666B", "#9B585E", "#856465", "#AD7C90", "#E2BC00",
        "#E3AAE0", "#B2C2FE", "#FD0039", "#009B75", "#FFF46D", "#E87EAC", "#DFE3E6", "#848590",
        "#AA9297", "#83A193", "#577977", "#3E7158", "#C64289", "#EA0072", "#C4A8CB", "#55C899",
        "#E78FCF", "#004547", "#F6E2E3", "#966716", "#378FDB", "#435E6A", "#DA0004", "#1B000F",
        "#5B9C8F", "#6E2B52", "#011115", "#E3E8C4", "#AE3B85", "#EA1CA9", "#FF9E6B", "#457D8B",
        "#92678B", "#00CDBB", "#9CCC04", "#002E38", "#96C57F", "#CFF6B4", "#492818", "#766E52",
        "#20370E", "#E3D19F", "#2E3C30", "#B2EACE", "#F3BDA4", "#A24E3D", "#976FD9", "#8C9FA8",
        "#7C2B73", "#4E5F37", "#5D5462", "#90956F", "#6AA776", "#DBCBF6", "#DA71FF", "#987C95",
        "#52323C", "#BB3C42", "#584D39", "#4FC15F", "#A2B9C1", "#79DB21", "#1D5958", "#BD744E",
        "#160B00", "#20221A", "#6B8295", "#00E0E4", "#102401", "#1B782A", "#DAA9B5", "#B0415D",
        "#859253", "#97A094", "#06E3C4", "#47688C", "#7C6755", "#075C00", "#7560D5", "#7D9F00",
        "#C36D96", "#4D913E", "#5F4276", "#FCE4C8", "#303052", "#4F381B", "#E5A532", "#706690",
        "#AA9A92", "#237363", "#73013E", "#FF9079", "#A79A74", "#029BDB", "#FF0169", "#C7D2E7",
        "#CA8869", "#80FFCD", "#BB1F69", "#90B0AB", "#7D74A9", "#FCC7DB", "#99375B", "#00AB4D",
        "#ABAED1", "#BE9D91", "#E6E5A7", "#332C22", "#DD587B", "#F5FFF7", "#5D3033", "#6D3800",
        "#FF0020", "#B57BB3", "#D7FFE6", "#C535A9", "#260009", "#6A8781", "#A8ABB4", "#D45262",
        "#794B61", "#4621B2", "#8DA4DB", "#C7C890", "#6FE9AD", "#A243A7", "#B2B081", "#181B00",
        "#286154", "#4CA43B", "#6A9573", "#A8441D", "#5C727B", "#738671", "#D0CFCB", "#897B77",
        "#1F3F22", "#4145A7", "#DA9894", "#A1757A", "#63243C", "#ADAAFF", "#00CDE2", "#DDBC62",
        "#698EB1", "#208462", "#00B7E0", "#614A44", "#9BBB57", "#7A5C54", "#857A50", "#766B7E",
        "#014833", "#FF8347", "#7A8EBA", "#274740", "#946444", "#EBD8E6", "#646241", "#373917",
        "#6AD450", "#81817B", "#D499E3", "#979440", "#011A12", "#526554", "#B5885C", "#A499A5",
        "#03AD89", "#B3008B", "#E3C4B5", "#96531F", "#867175", "#74569E", "#617D9F", "#E70452",
        "#067EAF", "#A697B6", "#B787A8", "#9CFF93", "#311D19", "#3A9459", "#6E746E", "#B0C5AE",
        "#84EDF7", "#ED3488", "#754C78", "#384644", "#C7847B", "#00B6C5", "#7FA670", "#C1AF9E",
        "#2A7FFF", "#72A58C", "#FFC07F", "#9DEBDD", "#D97C8E", "#7E7C93", "#62E674", "#B5639E",
        "#FFA861", "#C2A580", "#8D9C83", "#B70546", "#372B2E", "#0098FF", "#985975", "#20204C",
        "#FF6C60", "#445083", "#8502AA", "#72361F", "#9676A3", "#484449", "#CED6C2", "#3B164A",
        "#CCA763", "#2C7F77", "#02227B", "#A37E6F", "#CDE6DC", "#CDFFFB", "#BE811A", "#F77183",
        "#EDE6E2", "#CDC6B4", "#FFE09E", "#3A7271", "#FF7B59", "#4E4E01", "#4AC684", "#8BC891",
        "#BC8A96", "#CF6353", "#DCDE5C", "#5EAADD", "#F6A0AD", "#E269AA", "#A3DAE4", "#436E83",
        "#002E17", "#ECFBFF", "#A1C2B6", "#50003F", "#71695B", "#67C4BB", "#536EFF", "#5D5A48",
        "#890039", "#969381", "#371521", "#5E4665", "#AA62C3", "#8D6F81", "#2C6135", "#410601",
        "#564620", "#E69034", "#6DA6BD", "#E58E56", "#E3A68B", "#48B176", "#D27D67", "#B5B268",
        "#7F8427", "#FF84E6", "#435740", "#EAE408", "#F4F5FF", "#325800", "#4B6BA5", "#ADCEFF",
        "#9B8ACC", "#885138", "#5875C1", "#7E7311", "#FEA5CA", "#9F8B5B", "#A55B54", "#89006A",
        "#AF756F", "#2A2000", "#7499A1", "#FFB550", "#00011E", "#D1511C", "#688151", "#BC908A",
        "#78C8EB", "#8502FF", "#483D30", "#C42221", "#5EA7FF", "#785715", "#0CEA91", "#FFFAED",
        "#B3AF9D", "#3E3D52", "#5A9BC2", "#9C2F90", "#8D5700", "#ADD79C", "#00768B", "#337D00",
        "#C59700", "#3156DC", "#944575", "#ECFFDC", "#D24CB2", "#97703C", "#4C257F", "#9E0366",
        "#88FFEC", "#B56481", "#396D2B", "#56735F", "#988376", "#9BB195", "#A9795C", "#E4C5D3",
        "#9F4F67", "#1E2B39", "#664327", "#AFCE78", "#322EDF", "#86B487", "#C23000", "#ABE86B",
        "#96656D", "#250E35", "#A60019", "#0080CF", "#CAEFFF", "#323F61", "#A449DC", "#6A9D3B",
        "#FF5AE4", "#636A01", "#D16CDA", "#736060", "#FFBAAD", "#D369B4", "#FFDED6", "#6C6D74",
        "#927D5E", "#845D70", "#5B62C1", "#2F4A36", "#E45F35", "#FF3B53", "#AC84DD", "#762988",
        "#70EC98", "#408543", "#2C3533", "#2E182D", "#323925", "#19181B", "#2F2E2C", "#023C32",
        "#9B9EE2", "#58AFAD", "#5C424D", "#7AC5A6", "#685D75", "#B9BCBD", "#834357", "#1A7B42",
        "#2E57AA", "#E55199", "#316E47", "#CD00C5", "#6A004D", "#7FBBEC", "#F35691", "#D7C54A",
        "#62ACB7", "#CBA1BC", "#A28A9A", "#6C3F3B", "#FFE47D", "#DCBAE3", "#5F816D", "#3A404A",
        "#7DBF32", "#E6ECDC", "#852C19", "#285366", "#B8CB9C", "#0E0D00", "#4B5D56", "#6B543F",
        "#E27172", "#0568EC", "#2EB500", "#D21656", "#EFAFFF", "#682021", "#2D2011", "#DA4CFF",
        "#70968E", "#FF7B7D", "#4A1930", "#E8C282", "#E7DBBC", "#A68486", "#1F263C", "#36574E",
        "#52CE79", "#ADAAA9", "#8A9F45", "#6542D2", "#00FB8C", "#5D697B", "#CCD27F", "#94A5A1",
        "#790229", "#E383E6", "#7EA4C1", "#4E4452", "#4B2C00", "#620B70", "#314C1E", "#874AA6",
        "#E30091", "#66460A", "#EB9A8B", "#EAC3A3", "#98EAB3", "#AB9180", "#B8552F", "#1A2B2F",
        "#94DDC5", "#9D8C76", "#9C8333", "#94A9C9", "#392935", "#8C675E", "#CCE93A", "#917100",
        "#01400B", "#449896", "#1CA370", "#E08DA7", "#8B4A4E", "#667776", "#4692AD", "#67BDA8",
        "#69255C", "#D3BFFF", "#4A5132", "#7E9285", "#77733C", "#E7A0CC", "#51A288", "#2C656A",
        "#4D5C5E", "#C9403A", "#DDD7F3", "#005844", "#B4A200", "#488F69", "#858182", "#D4E9B9",
        "#3D7397", "#CAE8CE", "#D60034", "#AA6746", "#9E5585", "#BA6200"

    };

        private void aboutButton_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.Show();
        }

        private void selectProfile(int index = -1)
        {
            // -1 reselects the current profile
            if (index != -1) selectedProfile = profiles[index];
            pathTable.Rows.Clear();

            if (!noSelectedProfile())
            {
                foreach (ProfilePath path in selectedProfile.paths)
                {
                    pathTable.Rows.Add(path.Name);
                }
                profileTable.Rows[profiles.IndexOf(selectedProfile)].Selected = true;
            }
            if (index == -1 || selectedProfile.PathCount == 0) selectPath();
            else selectPath(0);
        }

        private void profileTable_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (editing && profiles.Count > 0)
            {
                editing = false;
                selectProfile(editedCell);
            }
            else selectProfile(e.RowIndex);
        }

        private void newProfileButton_Click(object sender, EventArgs e)
        {
            profiles.Add(new Profile());
            profileTable.Rows.Add(profiles.Last().Name, profiles.Last().Edited);
            
            selectProfile(profiles.Count - 1);
        }

        private void deleteProfileButton_Click(object sender, EventArgs e)
        {
            if (profiles.Count == 0)
            {
                editing = false;
                profileTable.ClearSelection();
                return;
            }
            if (editing) editedCell--;

            int profileIndex = profiles.IndexOf(selectedProfile);
            profiles.RemoveAt(profileIndex);
            profileTable.Rows.RemoveAt(profileIndex);
            selectProfile(Math.Min(profileIndex, profiles.Count - 1));
        }

        private void profileTable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            profileTable.BeginEdit(false);
        }

        private void profileTable_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            profiles[e.RowIndex].Name = profileTable.Rows[e.RowIndex].Cells[0].Value.ToString();
            ProfileEdit();

            editing = true;
            editedCell = e.RowIndex;
        }

        private void editProfileButton_Click(object sender, EventArgs e)
        {
            if (profileTable.CurrentCell != null) profileTable.BeginEdit(false);
        }

        private void newPathButton_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile()) return;
            
            string newPathName = "new path " + ++newPathCount;
            selectedProfile.newPath(newPathName);
            int newIndex = pathTable.Rows.Add(newPathName);

            //if (newIndex > 0) selectPath(newIndex);
            selectPath(newIndex);
            ProfileEdit();
        }

        private void deletePathButton_Click(object sender, EventArgs e)
        {
            if (profiles.Count == 0 || profiles[profileTable.SelectedRows[0].Index].PathCount == 0)
            {
                editing = false;
                pathTable.ClearSelection();
                return;
            }

            if (editing) editedCell--;

            int pathIndex = selectedProfile.paths.IndexOf(selectedPath);
            selectedProfile.paths.RemoveAt(pathIndex);
            pathTable.Rows.RemoveAt(pathIndex);
            selectPath(Math.Min(pathIndex, selectedProfile.paths.Count - 1));
            ProfileEdit();
        }

        private void pathTable_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (pathTable.Rows[e.RowIndex].Cells[0].Value.ToString().Trim() == "")
            {
                pathTable.Rows[e.RowIndex].Cells[0].Value = selectedProfile.paths[e.RowIndex].Name;
            }
            else
            {
                ProfilePath selpa = selectedProfile.paths[e.RowIndex];
                selpa.Name = pathTable.Rows[e.RowIndex].Cells[0].Value.ToString();
                //selectedProfile.paths[e.RowIndex].Name = pathTable.Rows[e.RowIndex].Cells[0].Value.ToString();
            }
            editing = true;
            editedCell = e.RowIndex;
            ProfileEdit();
        }

        private void pathOrderUp_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile() || noSelectedPath()) return;

            selectedProfile.movePathOrderUp(selectedPath);
            selectProfile();
            selectPath();
            ProfileEdit();
        }

        private void pathOrderDown_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile() || noSelectedPath()) return;

            selectedProfile.movePathOrderDown(selectedPath);

            selectProfile();
            selectPath();
            ProfileEdit();
        }

        /// <summary>
        /// Set the status message at the top of the field display
        /// </summary>
        private void setStatus(string message, bool error)
        {
            infoLabel.Text = message;
            infoLabel.ForeColor = error ? Color.Red : Color.Black;
        }

        private bool noSelectedProfile()
        {
            if (profiles.IndexOf(selectedProfile) == -1)
            {
                setStatus("Create or select a profile", true);
                return true;
            }
            return false;
        }

        private bool noSelectedPath()
        {
            if (noSelectedProfile()) return true;
            if (selectedProfile.paths.IndexOf(selectedPath) == -1)
            {
                setStatus("Create or select a path", true);
                return true;
            }
            return false;
        }

        private bool noPointsInPath()
        {
            if (noSelectedProfile() || noSelectedPath()) return true;
            if (selectedPath.isEmpty())
            {
                setStatus("Click on the field to create points", true);
                return true;
            }
            return false;
        }

        private void pathTable_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (editing && selectedProfile.PathCount > 0)
            {
                editing = false;
                selectPath(editedCell);
            }
            else selectPath(e.RowIndex);
        }

        private void selectPath(int index = -1)
        {
            ControlPointTable.Rows.Clear();

            if (index != -1) selectedPath = selectedProfile.paths[index];

            if (!noSelectedPath())
            {
                foreach (ControlPoint point in selectedPath.controlPoints)
                {
                    ControlPointTable.Rows.Add((int)point.X, (int)point.Y, (int)point.Heading);
                }
                pathTable.Rows[selectedProfile.paths.IndexOf(selectedPath)].Selected = true;
            }

            if (!noPointsInPath()) selectPoint(ControlPointTable.Rows.Count - 1);

            UpdateField();
        }

        private void pathTable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            pathTable.BeginEdit(false);
        }

        private void showPathsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateField();
        }

        private void rioConectionButton_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
        }

        private void editPathButton_Click(object sender, EventArgs e)
        {
            if (pathTable.CurrentCell != null) pathTable.BeginEdit(false);
        }

        private void ProfileEdit()
        {
            if (noSelectedProfile()) return;
            string edited = selectedProfile.newEdit();
            profileTable.Rows[profiles.IndexOf(selectedProfile)].Cells[1].Value = edited;
        }

        private double distance(SplinePoint p1, SplinePoint p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        private void maxVelocityInput_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.MaxVel = double.Parse(maxVelocityInput.Text);
                Properties.Settings.Default.Save();
                setStatus("", false);
            }
            catch (Exception)
            {
                setStatus("Max velocity must be a number", true);
            }
        }

        private void maxAccelerationInput_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.MaxAcc = double.Parse(maxAccelerationInput.Text);
                Properties.Settings.Default.Save();
                setStatus("", false);
            }
            catch (Exception)
            {
                setStatus("Max acceleration must be a number", true);
            }
        }

        private void saveToRioButton_Click(object sender, EventArgs e)
        {
            if (profiles.Count == 0)
            {
                setStatus("No profiles to save to RIO", true);
                return;
            }
            Cursor = Cursors.WaitCursor;

            SftpClient sftp = new SftpClient(
                Properties.Settings.Default.IpAddress,
                Properties.Settings.Default.Username,
                Properties.Settings.Default.Password
            );

            try
            {
                setStatus("Establishing RIO connection...", false);
                sftp.Connect();

                if (!sftp.Exists(Properties.Settings.Default.RioLocation)) sftp.CreateDirectory(Properties.Settings.Default.RioLocation);

                bool invalidProfiles = false;
                foreach (Profile profile in profiles)
                {
                    if (!profile.isValid())
                    {
                        invalidProfiles = true;
                        continue;
                    }
                    MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(profile.toJSON().ToString()));
                    sftp.UploadFile(stream, Path.Combine(
                        Properties.Settings.Default.RioLocation,
                        profile.Name.Replace(' ', '_') + ".mp"
                    ));
                }

                if (invalidProfiles) MessageBox.Show(
                    "One or more profiles were not deployed due to being invalid",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                setStatus("Profile(s) uploaded successfully", false);
                sftp.Disconnect();
            }
            catch (Renci.SshNet.Common.SshConnectionException exception)
            {
                Console.WriteLine("SshConnectionException, source: {0}", exception.StackTrace);
                setStatus("Failed to establish connection", true);
            }
            catch (System.Net.Sockets.SocketException exception)
            {
                Console.WriteLine("SocketException, source: {0}", exception.StackTrace);
                setStatus("Failed to establish connection", true);
            }
            catch (Renci.SshNet.Common.SftpPermissionDeniedException exception)
            {
                Console.WriteLine("SftpPermissionDeniedException, source: {0}", exception.StackTrace);
                setStatus("Permission denied", true);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception, source: {0}", exception.StackTrace);
                setStatus("Failed to upload profile to RIO", true);
            }

            Cursor = Cursors.Default;
        }
    }
}

/*List<double> testx = new List<double>();
                List<double> testy = new List<double>();
                foreach (ControlPoint point in selectedProfile.paths[path].controlPoints)
                {
                    testx.Add(point.X);
                    testy.Add(point.Y);
                }
                MathNet.Numerics.Interpolation.CubicSpline splinetest = MathNet.Numerics.Interpolation.CubicSpline.InterpolateNatural(testx, testy);
                List<double> derivs = new List<double>();
                foreach (double x in testx)
                {
                    derivs.Add(splinetest.Differentiate(x));
                }

                MathNet.Numerics.Interpolation.CubicSpline hermite = MathNet.Numerics.Interpolation.CubicSpline.InterpolateHermite(testx, testy, derivs);

                List<double> output = new List<double>();
                for (int time = 1; time < selectedProfile.paths[path].controlPoints.Count; time++)
                {
                    ControlPoint p = selectedProfile.paths[path].controlPoints[time];
                    double dx = p.X - selectedProfile.paths[path].controlPoints[time - 1].X;
                    for (int i = 0; i < 4; i++)
                    {
                        output.Add(hermite.Interpolate(p.X - dx + 0.25 * i * dx));
                        //MathNet.Numerics.Interpolation.CubicSpline.inter
                    }
                }*/