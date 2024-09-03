package motion.profile.mapper;

import java.util.ArrayList;
import java.util.Stack;

import atlantafx.base.theme.PrimerDark;
import atlantafx.base.theme.PrimerLight;
import edu.wpi.first.math.geometry.Translation2d;
import edu.wpi.first.math.util.Units;
import javafx.application.Application;
import javafx.application.Platform;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.fxml.FXML;
import javafx.geometry.Bounds;
import javafx.scene.Node;
import javafx.scene.chart.NumberAxis;
import javafx.scene.chart.ScatterChart;
import javafx.scene.chart.XYChart;
import javafx.scene.control.Button;
import javafx.scene.control.ContextMenu;
import javafx.scene.control.MenuItem;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;
import javafx.scene.control.ToggleButton;
import javafx.scene.control.cell.TextFieldTableCell;
import javafx.scene.input.KeyCode;
import javafx.scene.input.KeyEvent;
import javafx.scene.input.MouseEvent;
import javafx.scene.layout.BorderPane;
import javafx.scene.layout.VBox;

public class AppController {

    @FXML
    private TableView<Path> pathsTableView;
    @FXML
    private TableColumn<Path, String> nameColumn;
    @FXML
    private TableColumn<Path, String> modifiedColumn;

    @FXML
    private Button addPathButton;

    @FXML
    private Button button2;

    @FXML
    private ScatterChart<Number, Number> scatterChart;

    @FXML
    private NumberAxis xAxis;
    @FXML
    private ToggleButton themeToggleButton;

    @FXML
    private NumberAxis yAxis;
    @FXML
    private TableView<ControlPoint> pointsTableView;
    @FXML
    private TableColumn<ControlPoint, String> xColumn;
    @FXML
    private TableColumn<ControlPoint, String> yColumn;
    @FXML
    private TableColumn<ControlPoint, String> rColumn;
    @FXML
    private VBox scatterContainer;
    @FXML
    private BorderPane mainBorderPane;

    public static Stack<XYChart.Data<Number, Number>> undoStack = new Stack<>();
    public static Stack<XYChart.Data<Number, Number>> redoStack = new Stack<>();

    private XYChart.Series<Number, Number> controlPointSeries = new XYChart.Series<>();
    private XYChart.Series<Number, Number> pathPointsSeries = new XYChart.Series<>();

    private ObservableList<Path> paths = FXCollections.observableArrayList(new ArrayList<Path>());

    double fieldWidth = Units.inchesToMeters(651.223);
    double fieldHeight = Units.inchesToMeters(323.276819);

    @FXML
    public void initialize() {

        Application.setUserAgentStylesheet(new PrimerDark().getUserAgentStylesheet());

        // Configure the scatter chart
        configureScatterChart();

        // Configure the paths table
        configurePathsTable();

        // Configure the points table
        configurePointsTable();

        themeToggleButton.setOnAction(event -> {
            if (themeToggleButton.isSelected()) {
                Application.setUserAgentStylesheet(new PrimerLight().getUserAgentStylesheet());
                themeToggleButton.setText("Dark Mode");
            } else {
                Application.setUserAgentStylesheet(new PrimerDark().getUserAgentStylesheet());
                themeToggleButton.setText("Light Mode");
            }
        });

        addPathButton.setOnMouseClicked((MouseEvent event) -> {
            addNewPath();
        });

        // Configure undo/redo key bindings
        Platform.runLater(() -> {
            mainBorderPane.getScene().addEventFilter(KeyEvent.KEY_PRESSED, event -> {
                if (event.isControlDown() && event.getCode() == KeyCode.Z) {
                    if (event.isShiftDown()) {
                        redo();
                    } else {
                        undo();
                    }
                    event.consume();
                }
            });
        });
    }

    private void addNewPath() {
        int i = 1;
        boolean foundName = true;
        while (foundName) {
            int finalI = i;
            if (paths.stream().anyMatch(p -> p.getName().get().equals("Path " + finalI))) {
                i++;
            } else {
                foundName = false;
            }
        }
        Path newPath = new Path("Path " + i);
        paths.add(newPath);
        pathsTableView.getSelectionModel().select(newPath);
        pathsTableView.scrollTo(newPath);
    }

    private void configurePointsTable() {
        pointsTableView.setEditable(true);

        xColumn.setCellFactory(TextFieldTableCell.forTableColumn());
        yColumn.setCellFactory(TextFieldTableCell.forTableColumn());
        rColumn.setCellFactory(TextFieldTableCell.forTableColumn());

        xColumn.setCellValueFactory(
                (TableColumn.CellDataFeatures<ControlPoint, String> data) -> data.getValue().getXStringProp());
        yColumn.setCellValueFactory(
                (TableColumn.CellDataFeatures<ControlPoint, String> data) -> data.getValue().getYStringProp());
        rColumn.setCellValueFactory(
                (TableColumn.CellDataFeatures<ControlPoint, String> data) -> data.getValue().getRotationStringProp());

        // Add listeners to update the graph when the table is edited
        xColumn.setOnEditCommit(event -> {
            ControlPoint point = event.getRowValue();
            double newX = Double.parseDouble(event.getNewValue());
            point.setX(newX);
        });

        yColumn.setOnEditCommit(event -> {
            ControlPoint point = event.getRowValue();
            double newY = Double.parseDouble(event.getNewValue());
            point.setY(newY);
        });

        rColumn.setOnEditCommit(event -> {
            ControlPoint point = event.getRowValue();
            double newR = Double.parseDouble(event.getNewValue());
            point.setRotationDegrees(newR);
        });

        // Add listener to change the color of the corresponding point when a table row
        // is selected
        pointsTableView.getSelectionModel().selectedItemProperty()
                .addListener((obs, oldSelection, newSelection) -> {

                    if (oldSelection != null) {
                        oldSelection.getDataPoint().getNode().setStyle(""); // Reset to default style
                    }
                    if (newSelection != null) {
                        newSelection.getDataPoint().getNode().setStyle("-fx-background-color: green;"); // Change color
                                                                                                        // to red
                    }
                });

        ContextMenu contextMenu = new ContextMenu();
        MenuItem deletePoint = new MenuItem("Delete");
        contextMenu.getItems().add(deletePoint);

        // Set context menu on the ListView
        pointsTableView.setContextMenu(contextMenu);

        // Add event handler for delete menu item
        deletePoint.setOnAction(event -> {
            ControlPoint selectedPoint = pointsTableView.getSelectionModel().getSelectedItem();
            if (selectedPoint != null) {
                Path path = selectedPoint.getPath();
                path.removePoint(selectedPoint);
            }
        });
    }

    private void configurePathsTable() {
        pathsTableView.setItems(paths);
        pathsTableView.setEditable(true);

        nameColumn.setCellValueFactory(cellData -> cellData.getValue().getName());
        nameColumn.setCellFactory(TextFieldTableCell.forTableColumn());
        nameColumn.setOnEditCommit(event -> {
            Path path = event.getRowValue();
            path.setName(event.getNewValue());
        });
        modifiedColumn.setCellValueFactory(cellData -> cellData.getValue().getModifiedTime());

        // Create context menu for deleting paths
        ContextMenu contextMenu = new ContextMenu();
        MenuItem deleteItem = new MenuItem("Delete");
        MenuItem addItem = new MenuItem("Add");
        contextMenu.getItems().add(deleteItem);
        contextMenu.getItems().add(addItem);

        // Set context menu on the ListView
        pathsTableView.setContextMenu(contextMenu);

        pathsTableView.getSelectionModel().selectedItemProperty().addListener((obs, oldSelection, newSelection) -> {
            loadPointsForPath(newSelection);
        });

        // Add event handler for delete menu item
        deleteItem.setOnAction(event -> {
            Path selectedPath = pathsTableView.getSelectionModel().getSelectedItem();
            if (selectedPath != null) {
                // Remove the path from the chart and table
                paths.remove(selectedPath);
            }
        });
        addItem.setOnAction(event -> {
            addNewPath();
        });
    }

    // Configure the scatter chart
    private void configureScatterChart() {
        scatterChart.getData().add(controlPointSeries);
        scatterChart.getData().add(pathPointsSeries);

        scatterChart.legendVisibleProperty().set(false);
        scatterChart.setAnimated(false);

        // Set the axis bounds to the dimensions of the field.
        xAxis.setAutoRanging(false);
        yAxis.setAutoRanging(false);
        xAxis.setLowerBound(0);
        xAxis.setUpperBound(fieldWidth);
        yAxis.setLowerBound(0);
        yAxis.setUpperBound(fieldHeight);

        // Display grid lines every 1 unit
        xAxis.setTickUnit(1);
        yAxis.setTickUnit(1);

        // Add listener to add points to the chart when clicked
        scatterChart.setOnMouseClicked((MouseEvent event) -> {
            // Add the data point to the selected path if there is one
            Path selectedPath = pathsTableView.getSelectionModel().getSelectedItem();
            if (selectedPath != null) {
                // Get the bounds of the chart plot area
                Node plotArea = scatterChart.lookup(".chart-plot-background");
                Bounds plotAreaBounds = plotArea.localToScene(plotArea.getBoundsInLocal());

                // Convert scene coordinates to local coordinates relative to the plot area
                double xLocal = event.getSceneX() - plotAreaBounds.getMinX();
                double yLocal = event.getSceneY() - plotAreaBounds.getMinY();

                // Convert local coordinates to data coordinates
                double xValue = xAxis.getValueForDisplay(xLocal).doubleValue();
                double yValue = yAxis.getValueForDisplay(yLocal).doubleValue();

                // Add the data point to the table
                ControlPoint tableDataPoint = new ControlPoint(xValue, yValue, 0, selectedPath);
                pointsTableView.getSelectionModel().select(tableDataPoint);
                pointsTableView.scrollTo(tableDataPoint);
                // undoStack.push(dataPoint);
                // redoStack.clear(); // Clear redo stack whenever a new point is added
            }
        });

        // Ensure the chart maintains a "1:1" ratio
        scatterContainer.widthProperty().addListener((obs, oldWidth, newWidth) -> updateChartSize());
        scatterContainer.heightProperty().addListener((obs, oldHeight, newHeight) -> updateChartSize());
    }

    public Translation2d getChartMouseClickPosition(MouseEvent event) {
        double xValue = xAxis
                .getValueForDisplay(
                        scatterChart.getXAxis().sceneToLocal(event.getSceneX(), event.getSceneY()).getX())
                .doubleValue();
        double yValue = yAxis
                .getValueForDisplay(
                        scatterChart.getYAxis().sceneToLocal(event.getSceneX(), event.getSceneY()).getY())
                .doubleValue();
        return new Translation2d(xValue, yValue);
    }

    private void undo() {
        // if (!undoStack.isEmpty()) {
        // XYChart.Data<Number, Number> dataPoint = undoStack.pop();
        // series.getData().remove(dataPoint);
        // redoStack.push(dataPoint);
        // }
    }

    private void redo() {
        // if (!redoStack.isEmpty()) {
        // XYChart.Data<Number, Number> dataPoint = redoStack.pop();
        // series.getData().add(dataPoint);
        // addDragHandlers(dataPoint);
        // undoStack.push(dataPoint);
        // }
    }

    private void updateChartSize() {
        double width = scatterContainer.getWidth() - 50; // Subtract 50 to allow the pane to be resized this introduces
                                                         // a lag when large and fast resizes are done
        double height = scatterContainer.getHeight() - 50; // TODO: Find a better way to do this

        if (height * (fieldWidth / fieldHeight) > width) {
            scatterChart.setMinWidth(width);
            scatterChart.setMaxWidth(width);
            scatterChart.setMinHeight(width * (fieldHeight / fieldWidth));
            scatterChart.setMaxHeight(width * (fieldHeight / fieldWidth));
        } else {
            scatterChart.setMinHeight(height);
            scatterChart.setMaxHeight(height);
            scatterChart.setMinWidth(height * (fieldWidth / fieldHeight));
            scatterChart.setMaxWidth(height * (fieldWidth / fieldHeight));
        }

    }

    private void loadPointsForPath(Path path) {
        // Clear existing points
        if (path == null) {
            controlPointSeries.setData(null);
            pathPointsSeries.setData(null);
            pointsTableView.setItems(null);
        }
        pointsTableView.setItems(path.getSplinePoints());
        controlPointSeries.setData(path.getChartData());
        pathPointsSeries.setData(path.getSplineChartData());
    }

    public boolean selectAndScrollTo(ControlPoint point) {
        if (pointsTableView.getItems().contains(point)) {
            pointsTableView.getSelectionModel().select(point);
            pointsTableView.scrollTo(point);
            return true;
        }
        return false;
    }
}
