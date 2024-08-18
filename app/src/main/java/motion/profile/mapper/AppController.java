package motion.profile.mapper;

import java.util.ArrayList;
import java.util.List;
import java.util.Stack;

import edu.wpi.first.math.util.Units;
import javafx.application.Platform;
import javafx.beans.property.SimpleObjectProperty;
import javafx.beans.property.SimpleStringProperty;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.fxml.FXML;
import javafx.geometry.Bounds;
import javafx.scene.Cursor;
import javafx.scene.Node;
import javafx.scene.chart.NumberAxis;
import javafx.scene.chart.ScatterChart;
import javafx.scene.chart.XYChart;
import javafx.scene.control.Button;
import javafx.scene.control.ContextMenu;
import javafx.scene.control.MenuItem;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;
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
    private NumberAxis yAxis;
    @FXML
    private TableView<SplinePoint> pointsTableView;
    @FXML
    private TableColumn<SplinePoint, String> xColumn;
    @FXML
    private TableColumn<SplinePoint, String> yColumn;
    @FXML
    private TableColumn<SplinePoint, String> rColumn;
    @FXML
    private VBox scatterContainer;
    @FXML
    private BorderPane mainBorderPane;

    public static Stack<XYChart.Data<Number, Number>> undoStack = new Stack<>();
    public static Stack<XYChart.Data<Number, Number>> redoStack = new Stack<>();

    private XYChart.Series<Number, Number> series = new XYChart.Series<>();

    private ObservableList<Path> paths = FXCollections.observableArrayList(new ArrayList<Path>());

    double fieldWidth = Units.inchesToMeters(651.223);
    double fieldHeight = Units.inchesToMeters(323.276819);

    @FXML
    public void initialize() {

        // Configure the scatter chart
        configureScatterChart();

        // Configure the paths table
        configurePathsTable();

        // Configure the points table
        configurePointsTable();

        addPathButton.setOnMouseClicked((MouseEvent event) -> {
            int i = 1;
            boolean foundName = true;
            while (foundName) {
                int finalI = i;
                if (paths.stream().anyMatch(p -> p.getName().equals("Path " + finalI))) {
                    i++;
                } else {
                    foundName = false;
                }
            }
            Path newPath = new Path("Path " + i);
            paths.add(newPath);
        });

        // Add listener to load points when a path is selected
        pathsTableView.getSelectionModel().selectedItemProperty().addListener((obs, oldSelection, newSelection) -> {
            if (newSelection != null) {
                loadPointsForPath(newSelection);
            }
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

        // Create context menu for deleting paths
        ContextMenu contextMenu = new ContextMenu();
        MenuItem deleteItem = new MenuItem("Delete");
        MenuItem addItem = new MenuItem("Add");
        contextMenu.getItems().add(deleteItem);
        contextMenu.getItems().add(addItem);

        // Set context menu on the ListView
        pathsTableView.setContextMenu(contextMenu);

        // Add event handler for delete menu item
        deleteItem.setOnAction(event -> {
            Path selectedPath = pathsTableView.getSelectionModel().getSelectedItem();
            if (selectedPath != null) {
                paths.remove(selectedPath);
            }
        });
    }

    private void configurePointsTable() {
        pointsTableView.setEditable(true);

        xColumn.setCellFactory(TextFieldTableCell.forTableColumn());
        yColumn.setCellFactory(TextFieldTableCell.forTableColumn());
        rColumn.setCellFactory(TextFieldTableCell.forTableColumn());

        xColumn.setCellValueFactory(
                (TableColumn.CellDataFeatures<SplinePoint, String> data) -> new SimpleObjectProperty<>(
                        String.format("%.3f", data.getValue().getX())));
        yColumn.setCellValueFactory(
                (TableColumn.CellDataFeatures<SplinePoint, String> data) -> new SimpleObjectProperty<>(
                        String.format("%.3f", data.getValue().getY())));
        rColumn.setCellValueFactory(
                (TableColumn.CellDataFeatures<SplinePoint, String> data) -> new SimpleObjectProperty<>(
                        String.format("%.3f", data.getValue().getRotationDegrees())));

        // Add listeners to update the graph when the table is edited
        xColumn.setOnEditCommit(event -> {
            SplinePoint point = event.getRowValue();
            double newX = Double.parseDouble(event.getNewValue());
            point.setX(newX);
            updateGraph(point.getPath());
        });

        yColumn.setOnEditCommit(event -> {
            SplinePoint point = event.getRowValue();
            double newY = Double.parseDouble(event.getNewValue());
            point.setY(newY);
            updateGraph(point.getPath());
        });

        rColumn.setOnEditCommit(event -> {
            SplinePoint point = event.getRowValue();
            double newR = Double.parseDouble(event.getNewValue());
            point.setRotationDegrees(newR);
            updateGraph(point.getPath());
        });

        // Add listener to change the color of the corresponding point when a table row
        // is selected
        pointsTableView.getSelectionModel().selectedItemProperty()
                .addListener((obs, oldSelection, newSelection) -> {

                    if (oldSelection != null) {
                        int oldIndex = pointsTableView.getItems().indexOf(oldSelection);
                        if (newSelection != null && newSelection.getPath() == oldSelection.getPath()) {
                            series.getData().get(oldIndex).getNode().setStyle(""); // Reset to default style
                        }
                    }
                    if (newSelection != null) {
                        int newIndex = pointsTableView.getItems().indexOf(newSelection);
                        series.getData().get(newIndex).getNode().setStyle("-fx-background-color: green;"); // Change
                                                                                                           // color
                                                                                                           // to
                                                                                                           // red
                    }
                });
    }

    private void configurePathsTable() {
        pathsTableView.setItems(paths);
        pathsTableView.setEditable(true);

        nameColumn.setCellValueFactory(cellData -> new SimpleStringProperty(cellData.getValue().getName()));
        nameColumn.setCellFactory(TextFieldTableCell.forTableColumn());
        nameColumn.setOnEditCommit(event -> {
            Path path = event.getRowValue();
            path.setName(event.getNewValue());
        });
        modifiedColumn.setCellValueFactory(cellData -> new SimpleStringProperty("")); // Placeholder for modified
                                                                                      // status
    }
    
    // Configure the scatter chart 
    private void configureScatterChart() {
        scatterChart.getData().add(series);
        scatterChart.legendVisibleProperty().set(false);
        scatterChart.setAnimated(false);

        //Set the axis bounds to the dimensions of the field.
        xAxis.setAutoRanging(false);
        yAxis.setAutoRanging(false);
        xAxis.setLowerBound(0);
        xAxis.setUpperBound(fieldWidth);
        yAxis.setLowerBound(0);
        yAxis.setUpperBound(fieldHeight);

        //Display grid lines every 1 unit
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

                // Add the data point to the series
                XYChart.Data<Number, Number> dataPoint = new XYChart.Data<>(xValue, yValue);
                series.getData().add(dataPoint);
                addDragHandlers(dataPoint);

                // Add the data point to the table
                SplinePoint tableDataPoint = new SplinePoint(xValue, yValue, 0, selectedPath);
                pointsTableView.getSelectionModel().select(tableDataPoint);
                pointsTableView.scrollTo(tableDataPoint);
                //undoStack.push(dataPoint);
                redoStack.clear(); // Clear redo stack whenever a new point is added
            }
        });

        // Ensure the chart maintains a "1:1" ratio
        scatterContainer.widthProperty().addListener((obs, oldWidth, newWidth) -> updateChartSize());
        scatterContainer.heightProperty().addListener((obs, oldHeight, newHeight) -> updateChartSize());
    }

    private void undo() {
        // if (!undoStack.isEmpty()) {
        //     XYChart.Data<Number, Number> dataPoint = undoStack.pop();
        //     series.getData().remove(dataPoint);
        //     redoStack.push(dataPoint);
        // }
    }

    private void redo() {
        // if (!redoStack.isEmpty()) {
        //     XYChart.Data<Number, Number> dataPoint = redoStack.pop();
        //     series.getData().add(dataPoint);
        //     addDragHandlers(dataPoint);
        //     undoStack.push(dataPoint);
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
        pointsTableView.setItems(path.getSplinePoints());
        updateGraph(path);
    }

    private void updateGraph(Path path) {
        series.getData().clear();
        List<SplinePoint> points = path.getSplinePoints();
        // Add points to the table and chart
        for (SplinePoint point : points) {
            XYChart.Data<Number, Number> dataPoint = new XYChart.Data<>(point.getX(), point.getY());
            series.getData().add(dataPoint);
            addDragHandlers(dataPoint);
        }
    }

    private void addDragHandlers(XYChart.Data<Number, Number> dataPoint) {
        Node node = dataPoint.getNode();

        node.setOnMousePressed(event -> {
            node.setCursor(Cursor.MOVE);

        });
        node.setOnMouseClicked(event -> {
            int index = series.getData().indexOf(dataPoint);
            SplinePoint point = pointsTableView.getItems().get(index);
            pointsTableView.getSelectionModel().select(point);
            pointsTableView.scrollTo(point);

            event.consume();
        });

        node.setOnMouseReleased(event -> {
            node.setCursor(Cursor.HAND);
        });

        node.setOnMouseDragged(event -> {
            double xValue = xAxis
                    .getValueForDisplay(
                            scatterChart.getXAxis().sceneToLocal(event.getSceneX(), event.getSceneY()).getX())
                    .doubleValue();
            double yValue = yAxis
                    .getValueForDisplay(
                            scatterChart.getYAxis().sceneToLocal(event.getSceneX(), event.getSceneY()).getY())
                    .doubleValue();
            dataPoint.setXValue(xValue);
            dataPoint.setYValue(yValue);

            // Update the corresponding table entry
            int index = series.getData().indexOf(dataPoint);
            SplinePoint point = pointsTableView.getItems().get(index);
            pointsTableView.getSelectionModel().select(point);
            pointsTableView.scrollTo(point);
            point.setX(xValue);
            point.setY(yValue);
            pointsTableView.refresh();
        });
    }
}
