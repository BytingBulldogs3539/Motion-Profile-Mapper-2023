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
    using VelocityMap.Forms;


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
        private double fieldHeight = 7.908;

        /// <summary>
        /// Defines the fieldWidth
        /// </summary>
        // OLD 2019: 8230
        // 8.00354?
        private double fieldWidth = 8.016;

        internal int padding = 1;
        public List<ControlPoint> controlPointArray = new List<ControlPoint>();
        //public OutputPoints outputPoints = new OutputPoints();

        // new
        public List<Profile> profiles = new List<Profile>();
        public int newProfileCount = 0; // lol
        public int newPathCount = 0;
        public double pointSize = 0.1;

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
            mainField.ChartAreas["field"].Axes[0].Interval = 1;
            mainField.ChartAreas["field"].Axes[0].IsReversed = true;

            mainField.ChartAreas["field"].Axes[1].Minimum = 0;
            mainField.ChartAreas["field"].Axes[1].Maximum = fieldHeight;
            mainField.ChartAreas["field"].Axes[1].Interval = 1;

            mainField.Series["background"].Points.AddXY(0, 0);
            mainField.Series["background"].Points.AddXY(fieldWidth, fieldHeight);

            mainField.Images.Add(new NamedImage("red", new Bitmap(VelocityMap.Properties.Resources._2023_red)));
            mainField.Images.Add(new NamedImage("red-colored", new Bitmap(VelocityMap.Properties.Resources._2023_red_colored)));
            mainField.Images.Add(new NamedImage("blue", new Bitmap(VelocityMap.Properties.Resources._2023_blue)));
            mainField.Images.Add(new NamedImage("blue-colored", new Bitmap(VelocityMap.Properties.Resources._2023_blue_colored)));
            mainField.ChartAreas["field"].BackImageWrapMode = ChartImageWrapMode.Scaled;
            mainField.ChartAreas["field"].BackImage = "red";
        }

        private void setBackground(bool blue, bool colored)
        {
            mainField.ChartAreas["field"].BackImage = 
                (blue? "blue" : "red") + (colored? "-colored" : "");
        }

        private void selectPoint(int index)
        {
            ControlPointTable.Rows[index].Selected = true;
        }

        private void MainField_MouseClick(object sender, MouseEventArgs e)
        {
            if (clickedPoint != null || e.Button != MouseButtons.Left) return;
            if (noSelectedProfile() || noSelectedPath()) return;

            if (placingPoint == null)
            {
                Chart chart = (Chart)sender;
                double x = (double)chart.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
                double y = (double)chart.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);

                if (x > 0 && y > 0 && x <= fieldWidth && y <= fieldHeight)
                {
                    placingPoint = new ControlPoint(x, y, 0);

                    ControlPointTable.Rows.Add(Math.Round(placingPoint.X, 3), Math.Round(placingPoint.Y, 3), placingPoint.Heading);
                    selectPoint(ControlPointTable.Rows.Count - 1);
                    DrawPoint(placingPoint, selectedPath);
                }                
            }
            else
            {
                selectedPath.addControlPoint(placingPoint);
                placingPoint = null;
                ProfileEdit();
                UpdateField();
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
            double clickedX = (double)chart.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
            double clickedY = (double)chart.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);

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
                double newX = (double)chart.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
                double newY = (double)chart.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);

                clickedPoint.X = newX;
                clickedPoint.Y = newY;
                if (clickedPointPath == selectedPath)
                {
                    ControlPointTable.SelectedRows[0].Cells[0].Value = Math.Round(newX, 3);
                    ControlPointTable.SelectedRows[0].Cells[1].Value = Math.Round(newY, 3);
                }

                DrawPath(clickedPointPath);
            }
            if (placingPoint != null)
            {
                double x = (double)chart.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
                double y = (double)chart.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);

                //placingPoint.Heading = (int)(Math.Atan2(y - placingPoint.Y, x - placingPoint.X) * 180 / Math.PI + 360) % 360;
                placingPoint.Heading = (int)(Math.Atan2(y - placingPoint.Y, x - placingPoint.X) * 180 / Math.PI + 360 + 270) % 360;
                ControlPointTable.Rows[ControlPointTable.Rows.Count - 1].Cells[2].Value = placingPoint.Heading;

                mainField.Series[placingPoint.Id].Points.Clear();
                double x1 = (double)(placingPoint.X + pointSize * Math.Cos((placingPoint.Heading - 270) * Math.PI / 180));
                double y1 = (double)(placingPoint.Y + pointSize * Math.Sin((placingPoint.Heading - 270) * Math.PI / 180));
                mainField.Series[placingPoint.Id].Points.AddXY(x1, y1);
                double x2 = (double)(placingPoint.X + 0.600 * Math.Cos((placingPoint.Heading - 270) * Math.PI / 180));
                double y2 = (double)(placingPoint.Y + 0.600 * Math.Sin((placingPoint.Heading - 270) * Math.PI / 180));
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

        private void ControlPoints_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                double newValue = double.Parse(ControlPointTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                switch (e.ColumnIndex)
                {
                    case 0:
                        selectedPath.controlPoints[e.RowIndex].X = newValue;
                        break;
                    case 1:
                        selectedPath.controlPoints[e.RowIndex].Y = newValue;
                        break;
                    case 2:
                        selectedPath.controlPoints[e.RowIndex].Heading = (int)newValue;
                        break;
                }
                UpdateField();
            }
            catch (Exception)
            {
                setStatus("Data values must be numbers", true);
                switch (e.ColumnIndex)
                {
                    case 0:
                        ControlPointTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value
                            = Math.Round(selectedPath.controlPoints[e.RowIndex].X, 3);
                        break;
                    case 1:
                        ControlPointTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value
                            = Math.Round(selectedPath.controlPoints[e.RowIndex].Y, 3);
                        break;
                    case 2:
                        ControlPointTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value
                            = selectedPath.controlPoints[e.RowIndex].Heading;
                        break;
                }
            }
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

            double x1 = (double)(point.X + pointSize * Math.Cos((point.Heading - 270) * Math.PI / 180));
            double y1 = (double)(point.Y + pointSize * Math.Sin((point.Heading - 270) * Math.PI / 180));
            mainField.Series[point.Id].Points.AddXY(x1, y1);
            double x2 = (double)(point.X + (path == selectedPath ? 0.600 : 0.300) * Math.Cos((point.Heading - 270) * Math.PI / 180));
            double y2 = (double)(point.Y + (path == selectedPath ? 0.600 : 0.300) * Math.Sin((point.Heading - 270) * Math.PI / 180));
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
                (double)Properties.Settings.Default.MaxVel * 1000,
                (double)Properties.Settings.Default.MaxAcc * 1000,
                (double)Properties.Settings.Default.MaxJerk * 1000,
                .0025
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
                    mainField.Series[path.id + "-path"].Points.AddXY(point.X, point.Y);
                    pointList.Add(point);
                }
            }

            foreach (SplinePoint point in buildOffsetPoints(-Properties.Settings.Default.TrackWidth, pointList))
            {
                mainField.Series[path.id + "-left"].Points.AddXY(point.X, point.Y);
            }
            foreach (SplinePoint point in buildOffsetPoints(Properties.Settings.Default.TrackWidth, pointList))
            {
                mainField.Series[path.id + "-right"].Points.AddXY(point.X, point.Y);
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

        public void UpdateField()
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
                    pathTable.Rows.Add($"[ {pathTable.RowCount + 1} ]", path.Name);
                }
                if (!noSelectedPath())
                {
                    foreach (ControlPoint point in selectedPath.controlPoints)
                    {
                        ControlPointTable.Rows.Add(Math.Round(point.X, 3), Math.Round(point.Y, 3), point.Heading);
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

            Cursor = Cursors.WaitCursor;
            setStatus("Saving profiles to file system...", false);
            foreach (Profile profile in profiles)
            {
                string mpPath = Path.Combine(browser.SelectedPath, profile.Name.Replace(' ', '_') + ".mp");
                string javaPath = Path.Combine(browser.SelectedPath, profile.Name.Replace(' ', '_') + ".java");
                using (var writer = new StreamWriter(mpPath))
                {
                    writer.Write(profile.toJSON().ToString());
                }
                using (var writer = new StreamWriter(javaPath))
                {
                    writer.Write(profile.toJava());
                }
            }
            setStatus("Profiles saved to file system", false);
            Cursor = Cursors.Default;
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

            if (browser.ShowDialog() != DialogResult.OK || browser.FileName.Trim().Length <= 3) return;

            Cursor = Cursors.WaitCursor;
            setStatus("Saving profile to file system...", false);
            // Write mp file to load from
            string filePath = Path.Combine(
                Path.GetDirectoryName(browser.FileName.Trim()),
                Path.GetFileNameWithoutExtension(browser.FileName.Trim()) + ".mp"
            );
            using (var writer = new StreamWriter(filePath))
            {
                writer.Write(selectedProfile.toJSON().ToString());
            }

            // Write java file to pre-compile into robot
            string pointPath = Path.Combine(
                Path.GetDirectoryName(browser.FileName.Trim()),
                Path.GetFileNameWithoutExtension(browser.FileName.Trim()) + ".java"
            );
            using (var writer = new StreamWriter(pointPath))
            {
                writer.Write(selectedProfile.toJava());
            }

            setStatus("Profile saved to file system", false);
            Cursor = Cursors.Default;
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

            Cursor = Cursors.WaitCursor;
            setStatus("Loading profiles from local files...", false);
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

            setStatus("Profiles loaded", false);
            Cursor = Cursors.Default;
        }

        /// <summary>
        /// Deploys the currently selected profile directly to the robot for testing
        /// </summary>
        private void DeploySelectedProfile(object sender, EventArgs e)
        {
            // OLD: NO LONGER USING THIS METHOD TO DEPLOY FOR TESTING
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

                setStatus("Uploading test profile...", false);
                if (!sftp.Exists(Properties.Settings.Default.RioLocation)) sftp.CreateDirectory(Properties.Settings.Default.RioLocation);

                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(selectedProfile.toTxt().ToString()));
                sftp.UploadFile(stream, Path.Combine(Properties.Settings.Default.RioLocation, "_test.txt"));
                
                setStatus("Test profile uploaded successfully", false);
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

                    //maxVelocityInput.Text = (string)o["Max Velocity"];
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

        private void selectProfile(int index = -1)
        {
            // -1 reselects the current profile
            if (index != -1) selectedProfile = profiles[index];
            pathTable.Rows.Clear();

            if (!noSelectedProfile())
            {
                foreach (ProfilePath path in selectedProfile.paths)
                {
                    pathTable.Rows.Add($"[ {pathTable.RowCount + 1} ]", path.Name);
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

            if (e.RowIndex == profileTable.RowCount - 1) return;
            editing = true;
            editedCell = e.RowIndex;
        }

        private void editProfileButton_Click(object sender, EventArgs e)
        {
            if (!noSelectedProfile() && profileTable.CurrentCell != null) profileTable.BeginEdit(false);
        }

        private void newPathButton_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile()) return;
            
            string newPathName = "new path " + ++newPathCount;
            selectedProfile.newPath(newPathName);
            int newIndex = pathTable.Rows.Add($"[ {pathTable.RowCount + 1} ]", newPathName);

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
            selectProfile();
            selectPath(Math.Min(pathIndex, selectedProfile.paths.Count - 1));
            ProfileEdit();
        }

        private void pathTable_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (pathTable.Rows[e.RowIndex].Cells[1].Value.ToString().Trim() == "")
            {
                pathTable.Rows[e.RowIndex].Cells[1].Value = selectedProfile.paths[e.RowIndex].Name;
            }
            else
            {
                ProfilePath editedPath = selectedProfile.paths[e.RowIndex];
                editedPath.Name = pathTable.Rows[e.RowIndex].Cells[1].Value.ToString().Trim();
                //selectedProfile.paths[e.RowIndex].Name = pathTable.Rows[e.RowIndex].Cells[1].Value.ToString();
            }
            editing = true;
            editedCell = e.RowIndex;
            ProfileEdit();
        }

        private void pathOrderUp_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile() || noSelectedPath()) return;

            ProfilePath tempPath = selectedPath;
            selectedProfile.movePathOrderUp(selectedPath);
            selectProfile();
            selectPath(selectedProfile.paths.IndexOf(tempPath));
            ProfileEdit();
        }

        private void pathOrderDown_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile() || noSelectedPath()) return;

            selectedProfile.movePathOrderDown(selectedPath);
            ProfilePath tempPath = selectedPath;
            selectProfile();
            selectPath(selectedProfile.paths.IndexOf(tempPath));
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
            // -1 reselects current path i think
            ControlPointTable.Rows.Clear();

            if (index != -1) selectedPath = selectedProfile.paths[index];

            if (!noSelectedPath())
            {
                foreach (ControlPoint point in selectedPath.controlPoints)
                {
                    ControlPointTable.Rows.Add(Math.Round(point.X, 3), Math.Round(point.Y, 3), point.Heading);
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
            if (noSelectedProfile() || noSelectedPath()) return;

            PathSettings settings = new PathSettings(selectedPath, pathTable.Rows[selectedProfile.paths.IndexOf(selectedPath)].Cells[1]);
            settings.Show();
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

                List<Profile> invalidProfiles = new List<Profile>();
                foreach (Profile profile in profiles)
                {
                    if (!profile.isValid())
                    {
                        invalidProfiles.Add(profile);
                        continue;
                    }
                    // Upload txt file for robot to read in auton
                    MemoryStream txtStream = new MemoryStream(Encoding.UTF8.GetBytes(profile.toTxt().ToString()));
                    sftp.UploadFile(txtStream, Path.Combine(
                        Properties.Settings.Default.RioLocation,
                        profile.Name.Replace(' ', '_') + ".txt"
                    ));
                    // Upload mp file for profiler to read for editing
                    MemoryStream mpStream = new MemoryStream(Encoding.UTF8.GetBytes(profile.toJSON().ToString()));
                    sftp.UploadFile(mpStream, Path.Combine(
                        Properties.Settings.Default.RioLocation,
                        profile.Name.Replace(' ', '_') + ".mp"
                    ));
                }

                setStatus("Verifying file contents...", false);
                bool verified = true;
                foreach (Profile profile in profiles)
                {
                    if (invalidProfiles.Contains(profile)) continue;
                    StreamReader reader = sftp.OpenText(
                        Path.Combine(Properties.Settings.Default.RioLocation, profile.Name.Replace(' ', '_') + ".txt")
                    );
                    if (profile.toTxt() != reader.ReadToEnd()) verified = false;
                }

                if (invalidProfiles.Count > 0) MessageBox.Show(
                    "One or more profiles were not deployed due to being invalid",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                if (verified) setStatus("Profile(s) uploaded and verified successfully", false);
                else setStatus("Failed to verify uploaded file content", true);
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

        private void defaultsButton_Click(object sender, EventArgs e)
        {
            Forms.Defaults defaults = new Forms.Defaults();
            defaults.Show();
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27 && placingPoint != null)
            {
                placingPoint = null;
                ControlPointTable.Rows.RemoveAt(ControlPointTable.RowCount - 1);
                UpdateField();
            }
        }

        private void radioRed_CheckedChanged(object sender, EventArgs e)
        {
            setBackground(false, false);
        }

        private void radioBlue_CheckedChanged(object sender, EventArgs e)
        {
            setBackground(true, false);
        }

        private void duplicateProfileButton_Click(object sender, EventArgs e)
        {
            profiles.Add(new Profile(selectedProfile));
            profileTable.Rows.Add(profiles.Last().Name, profiles.Last().Edited);
            selectProfile(profiles.Count - 1);
        }

        private void shiftPathButton_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile() || noSelectedPath()) return;

            ShiftPath shiftPath = new ShiftPath(selectedPath, this.selectPath);
            shiftPath.Show();
        }

        private void previewButton_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile()) return;

            Forms.Preview preview = new Forms.Preview(selectedProfile.toTxt().Replace(' ', '\t'));
            preview.Show();
        }

        private void deletePointButton_Click(object sender, EventArgs e)
        {
            if (noSelectedPath() && ControlPointTable.SelectedRows.Count == 0) return;

            selectedPath.controlPoints.RemoveAt(ControlPointTable.Rows.IndexOf(ControlPointTable.SelectedRows[0]));
            selectPath();
        }

        private void mirrorPathButton_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile() || noSelectedPath()) return;
            selectedProfile.mirrorPath(selectedPath, fieldWidth);
            selectPath();
            ProfileEdit();
        }

        private void invertAllPathsButton_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile() || noSelectedPath()) return;
            selectedProfile.mirrorAllPaths(fieldWidth);
            selectPath();
            ProfileEdit();
        }

        private void infoButton_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.Show();
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