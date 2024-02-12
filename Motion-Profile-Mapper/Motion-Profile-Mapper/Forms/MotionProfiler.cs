namespace MotionProfileMapper
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

    using MotionProfile.SegmentedProfile;
    using Renci.SshNet.Sftp;
    using MotionProfileMapper.Forms;
    using MotionProfileMapper.VelocityGenerate;
    using Menu = Forms.Menu;


    /// <summary>
    /// Defines the <see cref="MotionProfiler" />
    /// </summary>
    public partial class MotionProfiler : Form
    {
        /// <summary>
        /// Defines the fieldHeight
        /// </summary>
        ///
        // 2019: 8230
        // 2023: 7.908 (half field)
        // 2024: 9.01988 (> half field)
        private double fieldHeight = 9.01988;

        /// <summary>
        /// Defines the fieldWidth
        /// </summary>
        // 2019: 8230//mm
        // 2023: 8.016
        // 2024: 8.21055
        private double fieldWidth = 8.21055;

        internal int padding = 1;
        public List<ControlPoint> controlPointArray = new List<ControlPoint>();
        //public OutputPoints outputPoints = new OutputPoints();

        // new
        public static List<Profile> profiles = new List<Profile>();

        static List<UndoHolder> undo = new List<UndoHolder>();
        static List<UndoHolder> redo = new List<UndoHolder>();


        public int newProfileCount = 0;
        public int newPathCount = 0;
        public double pointSize = 0.1;
        public bool splineMode = false;

        static Profile selectedProfile = null;
        static ProfilePath selectedPath = null;
        ControlPoint placingPoint = null;

        ControlPoint clickedPoint = null;
        ProfilePath clickedPointPath = null;

        ControlPoint preMoveClickedPoint = null;
        ProfilePath preMoveClickedPointPath = null;
        UndoHolder preMoveHolder = null;

        ControlPoint snappedPoint = null;
        ProfilePath snappedPointPath = null;

        private DateTime timeOfUpload;

        bool skipUpdate = false;

        Menu menu;

        public static MotionProfiler motionProfiler;


        /// <summary>
        /// Initializes a new instance of the <see cref="MotionProfiler"/> class.
        /// </summary>
        public MotionProfiler(Menu menu)
        {
            motionProfiler = this;
            this.menu = menu;
            InitializeComponent();
        }

        /// <summary>
        /// Load Form 1
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            SetupMainField();
            MotionProfiler_Resize(null, null);


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

            mainField.Series.Add("background");
            mainField.Series["background"].ChartArea = "field";
            mainField.Series["background"].ChartType = SeriesChartType.Point;
            mainField.Series["background"].Color = Color.Transparent;
            mainField.Series["background"].MarkerSize = 0;
            mainField.Series["background"].BorderWidth = 0;
            mainField.Series["background"].Points.AddXY(0, 0);
            mainField.Series["background"].Points.AddXY(fieldWidth, fieldHeight);

            mainField.Images.Add(new NamedImage("red", new Bitmap(MotionProfileMapper.Properties.Resources._2024_red)));
            //mainField.Images.Add(new NamedImage("red-colored", new Bitmap(VelocityMap.Properties.Resources._2023_red_colored)));
            mainField.Images.Add(new NamedImage("blue", new Bitmap(MotionProfileMapper.Properties.Resources._2024_blue)));
            //mainField.Images.Add(new NamedImage("blue-colored", new Bitmap(VelocityMap.Properties.Resources._2023_blue_colored)));
            mainField.ChartAreas["field"].BackImageWrapMode = ChartImageWrapMode.Scaled;

            if (MotionProfileMapper.Properties.Settings.Default.defaultAllianceIsRed)
                mainField.ChartAreas["field"].BackImage = "red";
            else
                mainField.ChartAreas["field"].BackImage = "blue";
        }

        private void setBackground(bool blue, bool colored)
        {
            mainField.ChartAreas["field"].BackImage =
                (blue ? "blue" : "red") + (colored ? "-colored" : "");
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
                double x = Math.Round((double)chart.ChartAreas[0].AxisX.PixelPositionToValue(e.X), 3);
                double y = Math.Round((double)chart.ChartAreas[0].AxisY.PixelPositionToValue(e.Y), 3);

                if (x > 0 && y > 0 && x <= fieldWidth && y <= fieldHeight)
                {
                    placingPoint = new ControlPoint(selectedPath, x, y, 0);

                    ControlPointTable.Rows.Add(Math.Round(placingPoint.X, 3), Math.Round(placingPoint.Y, 3), placingPoint.Rotation);
                    selectPoint(ControlPointTable.Rows.Count - 1);
                    DrawPoint(placingPoint, selectedPath, true);
                    if (selectedPath.ControlPoints.Count > 0)
                        DrawPoint(selectedPath.ControlPoints.Last(), selectedPath, false);
                }
            }
            else
            {

                selectedPath.addControlPoint(placingPoint);
                placingPoint = null;

                if (selectedPath != selectedProfile.Paths.Last()
                        && selectedProfile.Paths[selectedProfile.Paths.IndexOf(selectedPath) + 1].SnapToPrevious)
                {
                    selectedProfile.Paths[selectedProfile.Paths.IndexOf(selectedPath) + 1].snap(selectedPath);
                }
                UpdateField();
            }

        }

        private void mainField_MouseUp(object sender, MouseEventArgs e)
        {
            if (clickedPoint != null)
            {
                if (!clickedPoint.Equals(preMoveClickedPoint))
                {
                    saveUndoState("New Point",true, preMoveHolder);
                }
                selectedProfile.forceEdit();
                clickedPoint = null;
                clickedPointPath = null;
                preMoveClickedPoint = null;
                preMoveClickedPointPath = null;
                preMoveHolder = null;
                snappedPoint = null;
                snappedPointPath = null;

                UpdateField();
            }
            System.Windows.Forms.Cursor.Clip = new Rectangle();


        }

        /// <summary>
        /// Checks for a point selection for clicking and dragging.
        /// </summary>
        private void MainField_MouseDown(object sender, MouseEventArgs e)
        {
            if (placingPoint != null || !e.Button.HasFlag(MouseButtons.Left)) return;
            if (noSelectedProfile() || noSelectedPath()) return;

            Chart chart = (Chart)sender;

            Point p = new Point((int)chart.ChartAreas[0].AxisX.ValueToPixelPosition(fieldWidth - .01), (int)chart.ChartAreas[0].AxisY.ValueToPixelPosition(fieldHeight - .02));

            p = chart.PointToScreen(p);

            System.Drawing.Rectangle bounds = new System.Drawing.Rectangle(p.X, p.Y, (int)chart.ChartAreas[0].AxisX.ValueToPixelPosition(0) - (int)chart.ChartAreas[0].AxisX.ValueToPixelPosition(fieldWidth - .01), (int)chart.ChartAreas[0].AxisY.ValueToPixelPosition(0) - (int)chart.ChartAreas[0].AxisY.ValueToPixelPosition(fieldHeight - .02));
            System.Windows.Forms.Cursor.Clip = bounds;

            double clickedX = Math.Round((double)chart.ChartAreas[0].AxisX.PixelPositionToValue(e.X), 3);
            double clickedY = Math.Round((double)chart.ChartAreas[0].AxisY.PixelPositionToValue(e.Y), 3);

            ProfilePath path = selectedPath;

            foreach (ControlPoint point in path.ControlPoints)
            {
                if (Math.Abs(clickedX - point.X) < pointSize && Math.Abs(clickedY - point.Y) < pointSize)
                {
                    clickedPoint = point;
                    clickedPointPath = path;

                    preMoveClickedPoint = new ControlPoint(point, path);
                    preMoveClickedPointPath = new ProfilePath(path, selectedProfile);

                    List<Profile> ps = new List<Profile>();
                    foreach (Profile pro in profiles)
                    {
                        ps.Add(new Profile(pro));
                    }

                    UndoHolder holder = new UndoHolder();
                    holder.profiles = ps;
                    holder.reason = "Control Point Move"; 
                    if (selectedPath != null)
                    {
                        holder.selectedPathIndex = selectedProfile.Paths.IndexOf(selectedPath);
                        holder.selectedProfileIndex = profiles.IndexOf(selectedProfile);
                    }
                    preMoveHolder = holder;


                    if (clickedPointPath == selectedPath) selectPoint(path.ControlPoints.IndexOf(point));

                    if (clickedPoint == clickedPointPath.ControlPoints[0]
                        && clickedPointPath.SnapToPrevious)
                    {
                        int pathIndex = selectedProfile.Paths.IndexOf(clickedPointPath);
                        snappedPointPath = selectedProfile.Paths[pathIndex - 1];
                        snappedPoint = snappedPointPath.ControlPoints.Last();
                    }
                    else if (clickedPoint == clickedPointPath.ControlPoints.Last()
                        && clickedPointPath != selectedProfile.Paths.Last())
                    {
                        int pathIndex = selectedProfile.Paths.IndexOf(clickedPointPath);
                        snappedPointPath = selectedProfile.Paths[pathIndex + 1];
                        if (selectedProfile.Paths[pathIndex + 1].SnapToPrevious)
                            snappedPoint = snappedPointPath.ControlPoints[0];
                    }
                }
            }
        }

        /// <summary>
        /// The event that is called when the user mouse while above the main field.
        /// </summary>
        /// 

        private void MainField_MouseMove(object sender, MouseEventArgs e)
        {

            Chart chart = (Chart)sender;

            //if the user is holding the left button while moving the mouse allow them to move the point.
            if (clickedPoint != null && e.Button.HasFlag(MouseButtons.Left))
            {
                double newX = 0.0;
                double newY = 0.0;
                try
                {

                    newX = Math.Round((double)chart.ChartAreas[0].AxisX.PixelPositionToValue(e.X), 3);
                    newY = Math.Round((double)chart.ChartAreas[0].AxisY.PixelPositionToValue(e.Y), 3);

                }
                catch
                {
                    return;
                }

                clickedPoint.quickChangeX(newX);
                clickedPoint.quickChangeY(newY);
                if (snappedPoint != null)
                {
                    snappedPoint.quickChangeX(newX);
                    snappedPoint.quickChangeY(newY);
                }

                if (clickedPointPath == selectedPath)
                {
                    ControlPointTable.SelectedRows[0].Cells[0].Value = Math.Round(newX, 3);
                    ControlPointTable.SelectedRows[0].Cells[1].Value = Math.Round(newY, 3);
                }

                DrawPath(clickedPointPath, true);
                resetTrackBar();
                if (snappedPoint != null) DrawPath(snappedPointPath, true);
            }
            else
            {
                System.Windows.Forms.Cursor.Clip = new Rectangle();
            }
            if (placingPoint != null)
            {

                Point p = new Point((int)chart.ChartAreas[0].AxisX.ValueToPixelPosition(fieldWidth - .01), (int)chart.ChartAreas[0].AxisY.ValueToPixelPosition(fieldHeight - .02));

                p = chart.PointToScreen(p);

                System.Drawing.Rectangle bounds = new System.Drawing.Rectangle(p.X, p.Y, (int)chart.ChartAreas[0].AxisX.ValueToPixelPosition(0) - (int)chart.ChartAreas[0].AxisX.ValueToPixelPosition(fieldWidth - .01), (int)chart.ChartAreas[0].AxisY.ValueToPixelPosition(0) - (int)chart.ChartAreas[0].AxisY.ValueToPixelPosition(fieldHeight - .02));
                System.Windows.Forms.Cursor.Clip = bounds;

                double x = Math.Round((double)chart.ChartAreas[0].AxisX.PixelPositionToValue(e.X), 3);
                double y = Math.Round((double)chart.ChartAreas[0].AxisY.PixelPositionToValue(e.Y), 3);

                placingPoint.quickChangeRotation((int)(Math.Atan2(x - placingPoint.X, y - placingPoint.Y) * 180 / Math.PI));
                ControlPointTable.Rows[ControlPointTable.Rows.Count - 1].Cells[2].Value = placingPoint.Rotation;

                mainField.Series[placingPoint.Id + "-Rotation"].Points.Clear();
                double x1 = (double)(placingPoint.X + pointSize * Math.Sin((placingPoint.Rotation) * Math.PI / 180));
                double y1 = (double)(placingPoint.Y + pointSize * Math.Cos((placingPoint.Rotation) * Math.PI / 180));
                mainField.Series[placingPoint.Id + "-Rotation"].Points.AddXY(x1, y1);
                double x2 = (double)(placingPoint.X + 0.600 * Math.Sin((placingPoint.Rotation) * Math.PI / 180));
                double y2 = (double)(placingPoint.Y + 0.600 * Math.Cos((placingPoint.Rotation) * Math.PI / 180));
                mainField.Series[placingPoint.Id + "-Rotation"].Points.AddXY(x2, y2);
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
                        selectedPath.ControlPoints[e.RowIndex].X = newValue;
                        break;
                    case 1:
                        selectedPath.ControlPoints[e.RowIndex].Y = newValue;
                        break;
                    case 2:
                        selectedPath.ControlPoints[e.RowIndex].Rotation = (int)newValue;
                        break;
                }
                if (e.RowIndex == 0 && selectedPath.SnapToPrevious)
                    selectedProfile.Paths[selectedProfile.Paths.IndexOf(selectedPath) - 1].snapLast(selectedPath.ControlPoints[0]);
                if (e.RowIndex == selectedPath.ControlPoints.Count - 1 && selectedPath != selectedProfile.Paths.Last()
                        && selectedProfile.Paths[selectedProfile.Paths.IndexOf(selectedPath) + 1].SnapToPrevious)
                {
                    selectedProfile.Paths[selectedProfile.Paths.IndexOf(selectedPath) + 1].snap(selectedPath);
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
                            = Math.Round(selectedPath.ControlPoints[e.RowIndex].X, 3);
                        break;
                    case 1:
                        ControlPointTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value
                            = Math.Round(selectedPath.ControlPoints[e.RowIndex].Y, 3);
                        break;
                    case 2:
                        ControlPointTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value
                            = selectedPath.ControlPoints[e.RowIndex].Rotation;
                        break;
                }
            }
        }

        private void DrawPoint(ControlPoint point, ProfilePath path, bool selected = false)
        {


            string Series = point.Id;

            int seriesIndex = mainField.Series.IndexOf(Series);
            if (seriesIndex != -1) mainField.Series.RemoveAt(seriesIndex);

            mainField.Series.Add(Series);
            mainField.Series[Series].ChartArea = "field";
            mainField.Series[Series].ChartType = SeriesChartType.Point;
            mainField.Series[Series].Color = path == selectedPath ? Color.Lime : Color.Green;
            if (selected)
                mainField.Series[Series].Color = Color.Purple;
            mainField.Series[Series].MarkerSize = 10;
            mainField.Series[Series].MarkerStyle = MarkerStyle.Diamond;
            mainField.Series[Series].LabelForeColor = Color.White;

            mainField.Series[Series].Points.AddXY(point.X, point.Y);
            if (path == selectedPath)
            {
                if (point.Path.ControlPoints.Contains(point))
                    mainField.Series[Series].Points.Last().Label = "" + (point.Path.ControlPoints.IndexOf(point) + 1);
                else
                    mainField.Series[Series].Points.Last().Label = "" + (point.Path.ControlPoints.Count + 1);
            }

            Series = point.Id + "-Rotation";

            seriesIndex = mainField.Series.IndexOf(Series);
            if (seriesIndex != -1) mainField.Series.RemoveAt(seriesIndex);

            mainField.Series.Add(Series);
            mainField.Series[Series].ChartType = SeriesChartType.Line;
            mainField.Series[Series].BorderWidth = 2;
            mainField.Series[Series].Color = path == selectedPath ? Color.Red : Color.DarkRed;

            double x1 = (double)(point.X + pointSize * Math.Sin((point.Rotation) * Math.PI / 180));
            double y1 = (double)(point.Y + pointSize * Math.Cos((point.Rotation) * Math.PI / 180));
            mainField.Series[Series].Points.AddXY(x1, y1);
            double x2 = (double)(point.X + (path == selectedPath ? 0.6 : 0.3) * Math.Sin((point.Rotation) * Math.PI / 180));
            double y2 = (double)(point.Y + (path == selectedPath ? 0.6 : 0.3) * Math.Cos((point.Rotation) * Math.PI / 180));
            mainField.Series[Series].Points.AddXY(x2, y2);

            if (point == placingPoint) return;
            mainField.Series[path.Id + "-path"].Points.AddXY(point.X, point.Y);

            if (path == selectedPath)
            {
                int seriesIndex1 = mainField.Series.IndexOf(point.Id + "Rectangle");
                if (seriesIndex1 != -1) mainField.Series.RemoveAt(seriesIndex1);
                mainField.Series.Add(point.Id + "Rectangle");
                mainField.Series[point.Id + "Rectangle"].ChartType = SeriesChartType.Line;
                mainField.Series[point.Id + "Rectangle"].BorderWidth = 2;
                mainField.Series[point.Id + "Rectangle"].Color = Color.GreenYellow;

                Translation2d fl = new Translation2d(-Properties.Settings.Default.FrameLength / 2, Properties.Settings.Default.FrameWidth / 2).rotateBy(Rotation2d.fromDegrees(-point.Rotation)).plus(new Translation2d(point.X, point.Y));
                Translation2d fr = new Translation2d(Properties.Settings.Default.FrameLength / 2, Properties.Settings.Default.FrameWidth / 2).rotateBy(Rotation2d.fromDegrees(-point.Rotation)).plus(new Translation2d(point.X, point.Y));
                Translation2d br = new Translation2d(Properties.Settings.Default.FrameLength / 2, -Properties.Settings.Default.FrameWidth / 2).rotateBy(Rotation2d.fromDegrees(-point.Rotation)).plus(new Translation2d(point.X, point.Y));
                Translation2d bl = new Translation2d(-Properties.Settings.Default.FrameLength / 2, -Properties.Settings.Default.FrameWidth / 2).rotateBy(Rotation2d.fromDegrees(-point.Rotation)).plus(new Translation2d(point.X, point.Y));
            }
        }

        private void DrawPath(ProfilePath path, bool quickDraw)
        {
            if (!showPathsCheckbox.Checked && path != selectedPath) return;

            int seriesIndex = mainField.Series.IndexOf(path.Id + "-path");
            if (seriesIndex != -1) mainField.Series.RemoveAt(seriesIndex);
            seriesIndex = mainField.Series.IndexOf(path.Id + "-points");
            if (seriesIndex != -1) mainField.Series.RemoveAt(seriesIndex);
            seriesIndex = mainField.Series.IndexOf(path.Id + "-padding");
            if (seriesIndex != -1) mainField.Series.RemoveAt(seriesIndex);


            mainField.Series.Add(path.Id + "-path");
            mainField.Series[path.Id + "-path"].ChartArea = "field";
            mainField.Series[path.Id + "-path"].ChartType = SeriesChartType.Point;
            mainField.Series[path.Id + "-path"].Color = path == selectedPath ? Color.Aqua : Color.Blue;
            mainField.Series[path.Id + "-path"].MarkerSize = 2;
            mainField.Series[path.Id + "-path"].BorderWidth = 2;

            List<ControlPoint> ps = new List<ControlPoint>();
            if (selectedPath == path)
                foreach (DataGridViewRow row in ControlPointTable.SelectedRows)
                {
                    ps.Add(selectedPath.ControlPoints[row.Index]);
                }

            foreach (ControlPoint point in path.ControlPoints)
            {
                DrawPoint(point, path, ps.Contains(point));
            }

            if (path.ControlPoints.Count < 2) return;

            path.generate(quickDraw);


            kinematicsChart.Series["Position"].Points.Clear();
            kinematicsChart.Series["Velocity"].Points.Clear();
            kinematicsChart.Series["Acceleration"].Points.Clear();

            foreach (State s in path.getPoints())
            {
                double time = s.getTime();
                kinematicsChart.Series["Position"].Points.AddXY(time, s.getPathState().getDistance());
                kinematicsChart.Series["Velocity"].Points.AddXY(time, s.getVelocity());
                kinematicsChart.Series["Acceleration"].Points.AddXY(time, s.getAcceleration());


                Pose2d state = s.getPathState().getPose2d();

                mainField.Series[path.Id + "-path"].Points.AddXY(state.getX(), state.getY());

            }
        }



        public void UpdateField()
        {
            if (skipUpdate)
                return;

            kinematicsChart.Series["Position"].Points.Clear();
            kinematicsChart.Series["Velocity"].Points.Clear();
            kinematicsChart.Series["Acceleration"].Points.Clear();

            mainField.Series.Clear();

            mainField.Series.Add("background");
            mainField.Series["background"].ChartArea = "field";
            mainField.Series["background"].ChartType = SeriesChartType.Point;
            mainField.Series["background"].Color = Color.Transparent;
            mainField.Series["background"].MarkerSize = 0;
            mainField.Series["background"].BorderWidth = 0;
            mainField.Series["background"].Points.AddXY(0, 0);
            mainField.Series["background"].Points.AddXY(fieldWidth, fieldHeight);


            if (noSelectedProfile()) return;

            if (placingPoint != null)
            {
                DrawPoint(placingPoint, selectedPath);
            }

            foreach (ProfilePath path in selectedProfile.Paths)
            {
                if (path == selectedPath) continue;
                DrawPath(path, false);
            }
            if (!noSelectedPath()) DrawPath(selectedPath, false);

            setStatus("", false);

            resetTrackBar();


        }

        private void SaveAllProfiles(object sender, EventArgs e)
        {
            if (profiles.Count == 0) return;

            FolderBrowserDialog browser = new FolderBrowserDialog();
            browser.Description = "Save all motion profiles to folder\n\nCAUTION: Existing profiles will be overridden!";

            String mpBasePath = "";

            if (!Directory.Exists(Properties.Settings.Default.mpSavePath))
            {
                if (browser.ShowDialog() != DialogResult.OK) return;
                mpBasePath = System.IO.Path.GetDirectoryName(browser.SelectedPath);
            }
            else
            {
                mpBasePath = Properties.Settings.Default.mpSavePath;
            }


            Cursor = Cursors.WaitCursor;
            setStatus("Saving profiles to file system...", false);
            foreach (Profile profile in profiles)
            {
                string mpPath = System.IO.Path.Combine(mpBasePath, profile.Name.Replace(' ', '_') + ".mp");
                //string javaPath = System.IO.Path.Combine(mpBasePath, profile.Name.Replace(' ', '_') + ".java");
                using (var writer = new StreamWriter(mpPath))
                {
                    writer.Write(profile.toJSON().ToString());
                }
                //using (var writer = new StreamWriter(javaPath))
                //{
                //    writer.Write(profile.toJava());
                //}
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
            browser.RestoreDirectory = true;
            browser.FileName = selectedProfile.Name.Replace(' ', '_');
            browser.Filter = "Motion Profile|*.mp;";
            browser.Title = "Save motion profile file";


            String mpBasePath = "";

            if (!Directory.Exists(Properties.Settings.Default.javaSavePath) || !Directory.Exists(Properties.Settings.Default.iniSavePath))
            {
                if (browser.ShowDialog() != DialogResult.OK || browser.FileName.Trim().Length <= 3) return;
                mpBasePath = System.IO.Path.GetDirectoryName(browser.FileName.Trim());
            }
            else
            {
                mpBasePath = Properties.Settings.Default.mpSavePath;
            }

            Cursor = Cursors.WaitCursor;
            setStatus("Saving profile to file system...", false);
            // Write mp file to load from
            string filePath = System.IO.Path.Combine(
                mpBasePath,
                System.IO.Path.GetFileNameWithoutExtension(browser.FileName.Trim()) + ".mp"
            );
            using (var writer = new StreamWriter(filePath))
            {
                writer.Write(selectedProfile.toJSON().ToString());
            }

            // Write java file to pre-compile into robot
            //string pointPath = System.IO.Path.Combine(
            //    System.IO.Path.GetDirectoryName(browser.FileName.Trim()),
            //    System.IO.Path.GetFileNameWithoutExtension(browser.FileName.Trim()) + ".java"
            //);
            //using (var writer = new StreamWriter(pointPath))
            //{
            //    writer.Write(selectedProfile.toJava());
            //}

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
                        JObject read = JObject.Parse(fileReader.ReadToEnd());
                        saveUndoState("Load File",true,null,false);
                        profiles.Add(new Profile(read));
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

        private void LoadProfilesFromRIO(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            ConnectionInfo info = new ConnectionInfo(Properties.Settings.Default.IpAddress,
                Properties.Settings.Default.Username, new PasswordAuthenticationMethod(Properties.Settings.Default.Username, Properties.Settings.Default.Password));

            info.Timeout = TimeSpan.FromSeconds(5);

            SftpClient sftp = new SftpClient(info);
            /*SftpClient sftp = new SftpClient(
                Properties.Settings.Default.IpAddress,
                Properties.Settings.Default.Username,
                Properties.Settings.Default.Password
            );*/
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
                if(sftp.ListDirectory(Properties.Settings.Default.RioLocation).Count()>0)
                    
                foreach (SftpFile file in sftp.ListDirectory(Properties.Settings.Default.RioLocation))
                {
                    if (!file.Name.Contains(".mp")) continue;
                    foundFiles = true;
                        saveUndoState("Load File", true, null, false);
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
            catch (Renci.SshNet.Common.SshOperationTimeoutException exception)
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
        bool skipSelectProfile = false;
        private void selectProfile(int index = -1)
        {
            if (skipSelectProfile)
                return;
            skipSelectProfile = true;
            // -1 reselects the current profile
            Console.WriteLine("SelectProfile");
            if (index != -1) selectedProfile = profiles[index];
            pathTable.Rows.Clear();

            if (placingPoint != null) placingPoint = null;

            if (!noSelectedProfile())
            {
                profileTable.ClearSelection();
                foreach (ProfilePath path in selectedProfile.Paths)
                {
                    pathTable.Rows.Add(path.Name);
                }
                
                profileTable.Rows[profiles.IndexOf(selectedProfile)].Selected = true;
            }
            skipSelectProfile = false;
            if (index == -1 || selectedProfile.PathCount == 0) selectPath();
            else selectPath(0);


            if (!noSelectedProfile())
                setAllianceMode(selectedProfile.isRed);
            UpdateField();
        }

        private void profileTable_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            //Console.WriteLine("row Enter");

            //selectProfile(e.RowIndex);
        }

        private void newProfileButton_Click(object sender, EventArgs e)
        {
            saveUndoState("New Profile",true, null, false);
            profiles.Add(new Profile());
            skipSelectProfile = true;
            int index = profileTable.Rows.Add(profiles.Last().Name, profiles.Last().Edited);
            skipSelectProfile = false;
            selectProfile(profiles.Count - 1);
            profileTable.CurrentCell = profileTable.Rows[index].Cells[0];
            profileTable.BeginEdit(false);
        }

        private void deleteProfileButton_Click(object sender, EventArgs e)
        {
            if (profiles.Count == 0)
            {
                profileTable.ClearSelection();
                return;
            }

            saveUndoState("Delete Profile", true, null, false);

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
        }

        private void editProfileButton_Click(object sender, EventArgs e)
        {
            if (!noSelectedProfile() && profileTable.CurrentCell != null) profileTable.BeginEdit(false);
        }

        private void newPathButton_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile()) return;

            string newPathName = "Path " + (selectedProfile.Paths.Count + 1);

            if (Properties.Settings.Default.SnapNewPaths && selectedProfile.Paths.Count > 0)
                selectedProfile.newPath(newPathName, splineMode, selectedProfile.Paths.Last());
            else selectedProfile.newPath(newPathName, splineMode);

            int newIndex = pathTable.Rows.Add(newPathName);
            selectPath(newIndex);
            pathTable.CurrentCell = pathTable.Rows[newIndex].Cells[0];
        }

        private void deletePathButton_Click(object sender, EventArgs e)
        {
            if (profiles.Count == 0 || profiles[profileTable.SelectedRows[0].Index].PathCount == 0)
            {
                pathTable.ClearSelection();
                return;
            }

            saveUndoState("Delete Path");

            int pathIndex = selectedProfile.Paths.IndexOf(selectedPath);
            selectedProfile.Paths.RemoveAt(pathIndex);
            pathTable.Rows.RemoveAt(pathIndex);
            selectProfile();
            selectPath(Math.Min(pathIndex, selectedProfile.Paths.Count - 1));

        }

        private void pathTable_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (pathTable.Rows[e.RowIndex].Cells[0].Value.ToString().Trim() == "")
            {
                pathTable.Rows[e.RowIndex].Cells[0].Value = selectedProfile.Paths[e.RowIndex].Name;
            }
            else
            {
                ProfilePath editedPath = selectedProfile.Paths[e.RowIndex];
                editedPath.Name = pathTable.Rows[e.RowIndex].Cells[0].Value.ToString().Trim();
            }


        }

        private void pathOrderUp_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile() || noSelectedPath()) return;

            ProfilePath tempPath = selectedPath;
            selectedProfile.movePathOrderUp(selectedPath);
            selectProfile();
            selectPath(selectedProfile.Paths.IndexOf(tempPath));

        }

        private void pathOrderDown_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile() || noSelectedPath()) return;

            selectedProfile.movePathOrderDown(selectedPath);
            ProfilePath tempPath = selectedPath;
            selectProfile();
            selectPath(selectedProfile.Paths.IndexOf(tempPath));

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
            if (selectedProfile.Paths.IndexOf(selectedPath) == -1)
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
            selectPath(e.RowIndex);
        }

        private void selectPath(int index = -1)
        {
            // -1 reselects current path i think
            ControlPointTable.Rows.Clear();

            if (placingPoint != null) placingPoint = null;

            if (index != -1) selectedPath = selectedProfile.Paths[index];

            if (!noSelectedPath())
            {
                skipUpdate = true;
                setSplineMode(selectedPath.IsSpline);
                skipUpdate = false;
            }


            if (!noSelectedPath())
            {
                foreach (ControlPoint point in selectedPath.ControlPoints)
                {
                    ControlPointTable.Rows.Add(Math.Round(point.X, 3), Math.Round(point.Y, 3), point.Rotation);
                }
                pathTable.Rows[selectedProfile.Paths.IndexOf(selectedPath)].Selected = true;
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
            settings.ShowDialog();
        }

        private void editPathButton_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile() || noSelectedPath()) return;

            PathSettings settings = new PathSettings(selectedPath, pathTable.Rows[selectedProfile.Paths.IndexOf(selectedPath)].Cells[0]);
            settings.ShowDialog();
            DrawPath(selectedPath, false);
        }

        public void updateEditTime(Profile p)
        {
            if (profiles.Contains(p))
                profileTable.Rows[profiles.IndexOf(p)].Cells[1].Value = p.Edited;
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
                    sftp.UploadFile(txtStream, System.IO.Path.Combine(
                        Properties.Settings.Default.RioLocation,
                        profile.Name.Replace(' ', '_') + ".txt"
                    ));
                    // Upload mp file for profiler to read for editing
                    MemoryStream mpStream = new MemoryStream(Encoding.UTF8.GetBytes(profile.toJSON().ToString()));
                    sftp.UploadFile(mpStream, System.IO.Path.Combine(
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
                        System.IO.Path.Combine(Properties.Settings.Default.RioLocation, profile.Name.Replace(' ', '_') + ".txt")
                    );
                    if (profile.toTxt() != reader.ReadToEnd()) verified = false;
                }

                if (invalidProfiles.Count > 0) MessageBox.Show(
                    "One or more profiles were not deployed due to being invalid",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                if (verified)
                {
                    setStatus("Profile(s) uploaded and verified successfully", false);
                    timeOfUpload = DateTime.Now;
                }
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
            defaults.ShowDialog();
            UpdateField();
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
            if (!noSelectedProfile()) selectedProfile.isRed = radioRed.Checked;
            setBackground(false, false);
        }

        private void radioBlue_CheckedChanged(object sender, EventArgs e)
        {
            if (!noSelectedProfile()) selectedProfile.isRed = !radioBlue.Checked;
            setBackground(true, false);
        }

        private void duplicateProfileButton_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile()) return;

            profiles.Add(new Profile(selectedProfile));
            profileTable.Rows.Add(profiles.Last().Name, profiles.Last().Edited);
            selectProfile(profiles.Count - 1);
        }

        private void shiftPathButton_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile() || noSelectedPath()) return;

            ShiftPath shiftPath = new ShiftPath(selectedPath, this.selectPath);
            shiftPath.ShowDialog();
        }

        private void previewButton_Click(object sender, EventArgs e)
        {
            if (noPointsInPath()) return;

            Forms.Preview preview = new Forms.Preview(selectedProfile.toTxt().Replace(' ', ' '));
            preview.ShowDialog();
        }

        private void deletePointButton_Click(object sender, EventArgs e)
        {
            if (noSelectedPath() || ControlPointTable.SelectedRows.Count == 0) return;

            if (placingPoint != null)
            {
                placingPoint = null;
                ControlPointTable.Rows.RemoveAt(ControlPointTable.RowCount - 1);
                UpdateField();
                return;
            }
            selectedPath.deleteControlPoint(ControlPointTable.SelectedRows[0].Index);

            if (ControlPointTable.SelectedRows[0].Index == 0 && selectedPath.SnapToPrevious)
                selectedProfile.Paths[selectedProfile.Paths.IndexOf(selectedPath) - 1].snapLast(selectedPath.ControlPoints[0]);
            if (ControlPointTable.SelectedRows[0].Index == selectedPath.ControlPoints.Count
                        && selectedPath != selectedProfile.Paths.Last()
                        && selectedProfile.Paths[selectedProfile.Paths.IndexOf(selectedPath) + 1].SnapToPrevious)
            {
                selectedProfile.Paths[selectedProfile.Paths.IndexOf(selectedPath) + 1].snap(selectedPath);
            }
            selectPath();
        }

        private void mirrorPathButton_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile() || noSelectedPath()) return;

            MirrorPath mirrorPath = new MirrorPath(selectedProfile, selectedPath, selectPath, fieldWidth);
            mirrorPath.ShowDialog();
        }

        private void infoButton_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }

        private void pathTable_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, pathTable.RowHeadersDefaultCellStyle.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }
        private void reverseButton_Click(object sender, EventArgs e)
        {
            if (noSelectedProfile() || noSelectedPath()) return;

            selectedPath.reverse();
            selectPath();
        }

        private void setSplineMode(bool isSpline)
        {
            radioLine.Checked = !isSpline;
            radioSpline.Checked = isSpline;
        }

        private void setAllianceMode(bool isRed)
        {
            radioRed.Checked = isRed;
            radioBlue.Checked = !isRed;
        }

        private void radioLine_CheckedChanged(object sender, EventArgs e)
        {
            if (!skipUpdate)
            {
                splineMode = false;
                if (!noSelectedPath()) selectedPath.IsSpline = splineMode;
                UpdateField();
            }
        }

        private void radioSpline_CheckedChanged(object sender, EventArgs e)
        {
            if (!skipUpdate)
            {
                splineMode = true;
                if (!noSelectedPath()) selectedPath.IsSpline = splineMode;
                UpdateField();
            }
        }

        private void MotionProfiler_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.menu.Close();
        }

        private void resetTrackBar()
        {
            //trackBar.Value = 0;
            trackBar_ValueChanged(null, null);
            stopTimer();
        }

        private void trackBar_ValueChanged(object sender, EventArgs e)
        {

            if (noSelectedPath()) return;

            double percent = (double)trackBar.Value / (double)trackBar.Maximum;

            if (selectedPath.gen == null)
            {
                return;
            }

            if (selectedPath.ControlPoints.Count < 2)
            {
                return;
            }

            double time = selectedPath.gen.getDuration() * percent;


            State s = selectedPath.gen.calculate(time);

            PState ps = s.getPathState();

            double x = ps.getPose2d().getX();
            double y = ps.getPose2d().getY();

            Rotation2d rot = Rotation2d.fromDegrees(0).minus(ps.getPose2d().getRotation());

            int seriesIndex1 = mainField.Series.IndexOf("robotOutline");
            if (seriesIndex1 != -1) mainField.Series.RemoveAt(seriesIndex1);
            mainField.Series.Add("robotOutline");
            mainField.Series["robotOutline"].ChartType = SeriesChartType.Line;
            mainField.Series["robotOutline"].BorderWidth = 2;
            mainField.Series["robotOutline"].Color = Color.GreenYellow;


            int seriesIndex2 = mainField.Series.IndexOf("robotOutlineAngleMark");
            if (seriesIndex2 != -1) mainField.Series.RemoveAt(seriesIndex2);
            mainField.Series.Add("robotOutlineAngleMark");
            mainField.Series["robotOutlineAngleMark"].ChartType = SeriesChartType.Line;
            mainField.Series["robotOutlineAngleMark"].BorderWidth = 3;
            mainField.Series["robotOutlineAngleMark"].Color = Color.Blue;

            Translation2d fl = new Translation2d(-Properties.Settings.Default.FrameLength / 2, Properties.Settings.Default.FrameWidth / 2).rotateBy(rot).plus(new Translation2d(x, y));
            Translation2d fr = new Translation2d(Properties.Settings.Default.FrameLength / 2, Properties.Settings.Default.FrameWidth / 2).rotateBy(rot).plus(new Translation2d(x, y));
            Translation2d br = new Translation2d(Properties.Settings.Default.FrameLength / 2, -Properties.Settings.Default.FrameWidth / 2).rotateBy(rot).plus(new Translation2d(x, y));
            Translation2d bl = new Translation2d(-Properties.Settings.Default.FrameLength / 2, -Properties.Settings.Default.FrameWidth / 2).rotateBy(rot).plus(new Translation2d(x, y));

            Translation2d CenterMark = new Translation2d(0, Properties.Settings.Default.FrameWidth / 2).rotateBy(rot).plus(new Translation2d(x, y));
            Translation2d CenterMark2 = new Translation2d(0, Properties.Settings.Default.FrameWidth / 2 + .25).rotateBy(rot).plus(new Translation2d(x, y));

            mainField.Series["robotOutline"].Points.AddXY(fl.getX(), fl.getY());
            mainField.Series["robotOutline"].Points.AddXY(fr.getX(), fr.getY());
            mainField.Series["robotOutline"].Points.AddXY(br.getX(), br.getY());
            mainField.Series["robotOutline"].Points.AddXY(bl.getX(), bl.getY());
            mainField.Series["robotOutline"].Points.AddXY(fl.getX(), fl.getY());

            mainField.Series["robotOutlineAngleMark"].Points.AddXY(CenterMark.getX(), CenterMark.getY());
            mainField.Series["robotOutlineAngleMark"].Points.AddXY(CenterMark2.getX(), CenterMark2.getY());

        }
        DateTime startTime = DateTime.Now;
        private void timer1_Tick(object sender, EventArgs e)
        {

            if (noSelectedPath() || selectedPath.gen == null)
            {
                stopTimer();
                return;
            }

            TimeSpan time = DateTime.Now - startTime;
            if (time.TotalSeconds + timeOffset > selectedPath.gen.getDuration())
            {
                trackBar.Value = trackBar.Maximum;
                stopTimer();
                return;
            }
            int trackbarvalue = (int)(((time.TotalSeconds + timeOffset) / selectedPath.gen.getDuration()) * trackBar.Maximum);

            if (trackbarvalue <= trackBar.Maximum && trackbarvalue >= trackBar.Minimum)
                trackBar.Value = trackbarvalue;
            trackBar_ValueChanged(null, null);

        }
        private void stopTimer()
        {
            timer1.Stop();
            playButton.IconChar = FontAwesome.Sharp.IconChar.Play;
        }

        private double timeOffset = 0.0;

        private void startTimer()
        {
            timeOffset = 0.0;
            timer1.Start();
            playButton.IconChar = FontAwesome.Sharp.IconChar.Pause;
            double percent = (double)trackBar.Value / (double)trackBar.Maximum;

            if (selectedPath.gen == null) return;

            if (percent != 1.0) timeOffset = selectedPath.gen.getDuration() * percent;

        }
        private void playButton_Click(object sender, EventArgs e)
        {
            if (noSelectedPath() || selectedPath.gen == null)
            {
                stopTimer();
                return;
            }

            if (playButton.IconChar == FontAwesome.Sharp.IconChar.Pause)
            {
                stopTimer();
                return;
            }

            startTime = DateTime.Now;
            startTimer();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            menu.constants.Show();
            this.Hide();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = DateTime.Now - timeOfUpload;


            timeSinceUpload.Text = "Last Upload: " + ts.ToString("h'h 'm'm 's's'");
        }

        private void MotionProfiler_Resize(object sender, EventArgs e)
        {
            double hw = fieldWidth / fieldHeight;
            double wh = fieldHeight / fieldWidth;
            if (panel1.Width <= panel1.Height* hw)
            {
                mainField.Width = (int)(panel1.Width);

                mainField.Height = (int)(panel1.Width * wh);

                mainField.Location = new Point(panel1.Location.X + (int)(panel1.Width / 2.0) - mainField.Width / 2, panel1.Location.Y + (int)(panel1.Height / 2.0) - mainField.Height / 2 - 50);
            }
            if (panel1.Height <= panel1.Width * wh)
            {
                mainField.Height = (int)(panel1.Height);

                mainField.Width = (int)(panel1.Height * hw);

                mainField.Location = new Point(panel1.Location.X + (int)(panel1.Width / 2.0) - mainField.Width / 2, panel1.Location.Y + (int)(panel1.Height / 2.0) - mainField.Height / 2 - 50);
            }
        }

        public static void saveUndoState(string reason, bool clearRedo = true, UndoHolder holder = null, bool selectPOI = true)
        {
            if (clearRedo)
                redo.Clear();


            if (holder == null)
            {
                List<Profile> profiles = MotionProfiler.profiles;
                List<Profile> ps = new List<Profile>();
                foreach (Profile p in profiles)
                {
                    ps.Add(new Profile(p));
                }

                UndoHolder h = new UndoHolder();
                h.profiles = ps;
                
                h.usePOI = selectPOI;
                if (reason == "Load File" || reason == "Delete Profile" || reason == "New Profile")
                    h.usePOI = false;
                h.reason = reason;
                if (selectedPath != null)
                {
                    h.selectedPathIndex = selectedProfile.Paths.IndexOf(selectedPath);
                    h.selectedProfileIndex = profiles.IndexOf(selectedProfile);
                }
                undo.Add(h);
            }
            else
            {

                undo.Add(holder);
            }
            Console.WriteLine("---UNDO---");
            foreach (UndoHolder ho in undo)
            {
                Console.WriteLine(ho.reason);
            }
            Console.WriteLine("---REDO---");
            foreach (UndoHolder ho in redo)
            {
                Console.WriteLine(ho.reason);
            }

        }

        public static void saveRedoState(string reason)
        {
            List<Profile> ps = new List<Profile>();
            foreach (Profile p in profiles)
            {
                ps.Add(new Profile(p));
            }

            UndoHolder holder = new UndoHolder();
            holder.profiles = ps;
            holder.reason = reason;
            if (selectedPath != null)
            {
                holder.selectedPathIndex = selectedProfile.Paths.IndexOf(selectedPath);
                holder.selectedProfileIndex = profiles.IndexOf(selectedProfile);
            }
            redo.Add(holder);
            Console.WriteLine("---UNDO---");
            foreach (UndoHolder ho in undo)
            {
                Console.WriteLine(ho.reason);
            }
            Console.WriteLine("---REDO---");
            foreach (UndoHolder ho in redo)
            {
                Console.WriteLine(ho.reason);
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (placingPoint != null)
            {
                placingPoint = null;
                ControlPointTable.Rows.RemoveAt(ControlPointTable.RowCount - 1);
                UpdateField();
                return;
            }

            if (undo.Count > 0)
            {
                if(profileTable.SelectedRows.Count == 0 && profiles.Count()> undo.Last().selectedProfileIndex && undo.Last().usePOI)
                {
                    selectProfile(undo.Last().selectedProfileIndex);
                    return;
                }
                if (profileTable.Rows.Count > 0 && profiles.Count() > undo.Last().selectedProfileIndex && undo.Last().selectedProfileIndex != profileTable.SelectedRows[0].Index && undo.Last().selectedProfileIndex != -1 && undo.Last().usePOI)
                {
                    selectProfile(undo.Last().selectedProfileIndex);
                    return;
                }

                skipUpdate = true;
                saveRedoState(undo.Last().reason);

                int selectedProfileIndex = -1;
                int selectedPathIndex = -1;

                if (pathTable.SelectedRows.Count > 0)
                {
                    selectedPathIndex = pathTable.SelectedCells[0].RowIndex;
                    pathTable.ClearSelection();
                }

                if (profileTable.SelectedCells.Count > 0)
                {
                    selectedProfileIndex = profileTable.SelectedCells[0].RowIndex;
                    profileTable.ClearSelection();
                }
                profileTable.Rows.Clear();

                profiles = undo.Last().profiles;

                foreach (Profile p in profiles)
                {
                    profileTable.Rows.Add(p.Name, p.Edited);
                }


                if (undo.Last().selectedProfileIndex != -1)
                {
                    selectProfile(undo.Last().selectedProfileIndex);
                }
                else if (profileTable.Rows.Count == 0)
                {
                    selectProfile();
                }
                else if (profileTable.Rows.Count - 1 >= selectedProfileIndex)
                {
                    selectProfile(selectedProfileIndex);
                }
                else if (profileTable.Rows.Count >= 1)
                {
                    selectProfile(profileTable.Rows.Count - 1);
                }


                if (undo.Last().selectedPathIndex != -1)
                {
                    selectPath(undo.Last().selectedPathIndex);
                }
                else if (pathTable.Rows.Count == 0)
                {
                    selectPath();
                }
                else if (pathTable.Rows.Count - 1 >= selectedPathIndex)
                {
                    selectPath(selectedPathIndex);
                }
                else if (pathTable.Rows.Count >= 1)
                {
                    selectPath(pathTable.Rows.Count - 1);
                }

                undo.Remove(undo.Last());

                skipUpdate = false;
                UpdateField();
                Console.WriteLine("---UNDO---");
                foreach (UndoHolder ho in undo)
                {
                    Console.WriteLine(ho.reason);
                }
                Console.WriteLine("---REDO---");
                foreach (UndoHolder ho in redo)
                {
                    Console.WriteLine(ho.reason);
                }
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            placingPoint = null;
            if (redo.Count > 0)
            {
                if (profileTable.Rows.Count > 0 && redo.Last().selectedProfileIndex != profileTable.SelectedRows[0].Index && undo.Last().selectedProfileIndex != -1 && undo.Last().usePOI)
                {
                    selectProfile(redo.Last().selectedProfileIndex);
                    return;
                }
                skipUpdate = true;
                saveUndoState(redo.Last().reason,false);
                int selectedIndex = -1;
                if (profileTable.SelectedCells.Count > 0)
                {
                    selectedIndex = profileTable.SelectedCells[0].RowIndex;
                    profileTable.ClearSelection();
                }

                int selectedPathIndex = -1;

                if (pathTable.SelectedRows.Count > 0)
                {
                    selectedPathIndex = pathTable.SelectedCells[0].RowIndex;
                    pathTable.ClearSelection();
                }

                profileTable.Rows.Clear();

                profiles = redo.Last().profiles;



                foreach (Profile p in profiles)
                {
                    profileTable.Rows.Add(p.Name, p.Edited);
                }

                if (redo.Last().selectedProfileIndex != -1)
                {
                    selectProfile(redo.Last().selectedProfileIndex);
                }
                else if (profileTable.Rows.Count == 0)
                {
                    selectProfile();
                }
                else if (profileTable.Rows.Count - 1 >= selectedIndex)
                {
                    selectProfile(selectedIndex);
                }
                else if (profileTable.Rows.Count >= 1)
                {
                    selectProfile(profileTable.Rows.Count - 1);
                }

                if (redo.Last().selectedPathIndex != -1)
                {
                    selectPath(redo.Last().selectedPathIndex);
                }
                else if (pathTable.Rows.Count == 0)
                {
                    selectPath();
                }
                else if (pathTable.Rows.Count - 1 >= selectedPathIndex)
                {
                    selectPath(selectedPathIndex);
                }
                else if (pathTable.Rows.Count >= 1)
                {
                    selectPath(pathTable.Rows.Count - 1);
                }
                redo.Remove(redo.Last());

                skipUpdate = false;
                UpdateField();
                Console.WriteLine("---UNDO---");
                foreach (UndoHolder ho in undo)
                {
                    Console.WriteLine(ho.reason);
                }
                Console.WriteLine("---REDO---");
                foreach (UndoHolder ho in redo)
                {
                    Console.WriteLine(ho.reason);
                }
            }
        }

        private void ControlPointTable_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (noSelectedPath())
                return;
            DrawPath(selectedPath, false);

        }

        private void profileTable_SelectionChanged(object sender, EventArgs e)
        {
            Console.WriteLine("SelectionChange");
            if(profileTable.SelectedRows.Count>0)
                selectProfile(profileTable.SelectedRows[0].Index);
        }
    }
}
