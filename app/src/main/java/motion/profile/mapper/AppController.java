package motion.profile.mapper;

import java.util.ArrayList;
import java.util.Optional;
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
import javafx.scene.control.Alert;
import javafx.scene.control.Button;
import javafx.scene.control.ButtonType;
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
    private TableView<PathHandler> pathsTableView;
    @FXML
    private TableColumn<PathHandler, String> nameColumn;
    @FXML
    private TableColumn<PathHandler, String> modifiedColumn;

    @FXML
    private Button addPathButton;

    @FXML
    private ToggleButton splineToggleButton;

    @FXML
    private ScatterChart<Number, Number> scatterChart;

    @FXML
    private NumberAxis xAxis;
    @FXML
    private ToggleButton themeToggleButton;

    @FXML
    private NumberAxis yAxis;
    @FXML
    private TableView<ControlPointHandler> pointsTableView;
    @FXML
    private TableColumn<ControlPointHandler, String> xColumn;
    @FXML
    private TableColumn<ControlPointHandler, String> yColumn;
    @FXML
    private TableColumn<ControlPointHandler, String> rColumn;
    @FXML
    private VBox scatterContainer;
    @FXML
    private BorderPane mainBorderPane;

    public static Stack<XYChart.Data<Number, Number>> undoStack = new Stack<>();
    public static Stack<XYChart.Data<Number, Number>> redoStack = new Stack<>();

    private XYChart.Series<Number, Number> controlPointSeries = new XYChart.Series<>();
    private XYChart.Series<Number, Number> pathPointsSeries = new XYChart.Series<>();

    private ObservableList<PathHandler> paths = FXCollections.observableArrayList(new ArrayList<PathHandler>());

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

        splineToggleButton.setOnAction(event -> {
            PathHandler selectedPath = pathsTableView.getSelectionModel().getSelectedItem();
            if (selectedPath != null) {
                selectedPath.setSplineMode(splineToggleButton.isSelected());
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
            if (paths.stream().anyMatch(p -> p.getName().equals("Path " + finalI))) {
                i++;
            } else {
                foundName = false;
            }
        }
        PathHandler newPath = new PathHandler("Path " + i, splineToggleButton.isSelected());
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
                data -> data.getValue().getXProp().asString());
        yColumn.setCellValueFactory(
                data -> data.getValue().getYProp().asString());
        rColumn.setCellValueFactory(
                data -> data.getValue().getRotationProp().asString());

        // Add listeners to update the graph when the table is edited
        xColumn.setOnEditCommit(event -> {
            ControlPointHandler point = event.getRowValue();
            double newX = Double.parseDouble(event.getNewValue());
            point.setX(newX);
        });

        yColumn.setOnEditCommit(event -> {
            ControlPointHandler point = event.getRowValue();
            double newY = Double.parseDouble(event.getNewValue());
            point.setY(newY);
        });

        rColumn.setOnEditCommit(event -> {
            ControlPointHandler point = event.getRowValue();
            double newR = Double.parseDouble(event.getNewValue());
            point.setRotationDegrees(newR);
        });

        // Add listener to change the color of the corresponding point when a table row
        // is selected
        pointsTableView.getSelectionModel().selectedItemProperty()
                .addListener((obs, oldSelection, newSelection) -> {

                    if (oldSelection != null) {
                        oldSelection.getDataPoint().getNode().setStyle("-fx-background-color: blue;"); // Reset to
                                                                                                       // default style
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
            ControlPointHandler selectedPoint = pointsTableView.getSelectionModel().getSelectedItem();
            if (selectedPoint != null) {
                PathHandler path = selectedPoint.getPathHandler();
                path.removePoint(selectedPoint);
            }
        });
    }

    private void configurePathsTable() {
        pathsTableView.setItems(paths);
        pathsTableView.setEditable(true);

        nameColumn.setCellValueFactory(cellData -> cellData.getValue().getNameProp());
        nameColumn.setCellFactory(TextFieldTableCell.forTableColumn());
        nameColumn.setOnEditCommit(event -> {
            event.getRowValue().setName(event.getNewValue());
        });
        modifiedColumn.setCellValueFactory(cellData -> cellData.getValue().getModifiedTimeProp());

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
            splineToggleButton.setSelected(newSelection.isSpline());
        });

        // Add event handler for delete menu item
        deleteItem.setOnAction(event -> {
            PathHandler selectedPath = pathsTableView.getSelectionModel().getSelectedItem();
            if (selectedPath != null) {
                // Create a confirmation dialog
                Alert alert = new Alert(Alert.AlertType.CONFIRMATION);
                alert.setTitle("Delete Path");
                alert.setHeaderText("Are you sure you want to delete " + selectedPath.getName() + "?");
                alert.setContentText("This action cannot be undone.");

                // Show the dialog and wait for the user's response
                Optional<ButtonType> result = alert.showAndWait();
                if (result.isPresent() && result.get() == ButtonType.OK) {
                    // Remove the path from the chart and table
                    paths.remove(selectedPath);
                }
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

        scatterChart.getStylesheets().add(getClass().getResource("/2024_chart_style.css").toExternalForm());

        // Add listener to add points to the chart when clicked
        scatterChart.setOnMouseClicked((
                MouseEvent event) -> {
            if (!isDragging()) { // Check if dragging is not in progress

                // Add the data point to the selected path if there is one
                PathHandler selectedPath = pathsTableView.getSelectionModel().getSelectedItem();
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

                    // Round the coordinates to three decimal places
                    xValue = Math.round(xValue * 1000.0) / 1000.0;
                    yValue = Math.round(yValue * 1000.0) / 1000.0;

                    if (xValue >= 0 && xValue <= fieldWidth && yValue >= 0 && yValue <= fieldHeight) {
                        // Add the data point to the table
                        ControlPointHandler tableDataPoint = new ControlPointHandler(xValue, yValue, 0, selectedPath);
                        pointsTableView.getSelectionModel().select(tableDataPoint);
                        pointsTableView.scrollTo(tableDataPoint);
                        // undoStack.push(dataPoint);
                        // redoStack.clear(); // Clear redo stack whenever a new point is added
                    }
                }
            }
        });
        // Ensure the chart maintains a "1:1" ratio
        scatterContainer.widthProperty().addListener((obs, oldWidth, newWidth) ->

        updateChartSize());
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

    public NumberAxis getXAxis() {
        return xAxis;
    }

    public NumberAxis getYAxis() {
        return yAxis;
    }

    private boolean dragging = false;

    public void setDragging(boolean dragging) {
        this.dragging = dragging;
    }

    public boolean isDragging() {
        return dragging;
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

    private void loadPointsForPath(PathHandler path) {
        // Clear existing points
        if (path == null) {
            controlPointSeries.setData(null);
            pathPointsSeries.setData(null);
            pointsTableView.setItems(null);
            return;
        }
        pointsTableView.setItems(path.getSplineControlPoints());
        controlPointSeries.setData(path.getChartData());
        pathPointsSeries.setData(path.getSplineChartData());
    }

    public boolean selectAndScrollTo(ControlPointHandler point) {
        if (pointsTableView.getItems().contains(point)) {
            pointsTableView.getSelectionModel().select(point);
            pointsTableView.scrollTo(point);
            return true;
        }
        return false;
    }
}
