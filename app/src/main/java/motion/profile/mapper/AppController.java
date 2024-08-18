package motion.profile.mapper;

import edu.wpi.first.math.util.Units;
import javafx.beans.property.SimpleObjectProperty;
import javafx.collections.FXCollections;
import javafx.fxml.FXML;
import javafx.geometry.Bounds;
import javafx.scene.Cursor;
import javafx.scene.Node;
import javafx.scene.chart.NumberAxis;
import javafx.scene.chart.ScatterChart;
import javafx.scene.chart.XYChart;
import javafx.scene.control.Button;
import javafx.scene.control.ListView;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;
import javafx.scene.control.cell.TextFieldTableCell;
import javafx.scene.input.MouseEvent;

public class AppController {

    @FXML
    private ListView<String> listView;

    @FXML
    private Button clearButton;

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

    private int i = 3;
    private XYChart.Series<Number, Number> series = new XYChart.Series<>();
    private boolean isDragging = false;

    @FXML
    public void initialize() {
        listView.setItems(FXCollections.observableArrayList("Item 1", "Item 2", "Item 3"));
        scatterChart.getData().add(series);
        scatterChart.setTitle("Line Chart");
        scatterChart.legendVisibleProperty().set(false);
        scatterChart.setAnimated(false);

        pointsTableView.setEditable(true);

        xColumn.setCellFactory(TextFieldTableCell.forTableColumn());
        yColumn.setCellFactory(TextFieldTableCell.forTableColumn());

        // Where 'nameColumn' is a TableColumn<Person, String> and Person has a "name" property
        xColumn.setCellValueFactory((TableColumn.CellDataFeatures<SplinePoint, String> data)
                -> new SimpleObjectProperty<>(String.format("%.3f", data.getValue().getX())));
        yColumn.setCellValueFactory((TableColumn.CellDataFeatures<SplinePoint, String> data)
                -> new SimpleObjectProperty<>(String.format("%.3f", data.getValue().getY())));
        // pointsTableView.getColumns().addAll(xColumn, yColumn);

        // Add listeners to update the graph when the table is edited
        xColumn.setOnEditCommit(event -> {
            SplinePoint point = event.getRowValue();
            double newX = Double.parseDouble(event.getNewValue());
            point.setX(newX);
            updateGraph();
        });

        yColumn.setOnEditCommit(event -> {
            SplinePoint point = event.getRowValue();
            double newY = Double.parseDouble(event.getNewValue());
            point.setY(newY);
            updateGraph();
        });

        // Set axis bounds to ensure all data points are visible
        xAxis.setAutoRanging(false);
        yAxis.setAutoRanging(false);
        xAxis.setLowerBound(0);
        xAxis.setUpperBound(Units.inchesToMeters(323.276819));
        yAxis.setLowerBound(0);
        yAxis.setUpperBound(Units.inchesToMeters(651.223));

        clearButton.setOnMouseClicked((MouseEvent event) -> {
            series.getData().clear();
        });

        button2.setOnMouseClicked((MouseEvent event) -> {
            i++;
            listView.getItems().add("New Item " + i);
        });

        scatterChart.setOnMousePressed(event -> {
            isDragging = false;
        });

        scatterChart.setOnMouseDragged(event -> {
            isDragging = true;
        });

        scatterChart.setOnMouseClicked((MouseEvent event) -> {
            if (!isDragging) {
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
                SplinePoint tableDataPoint = new SplinePoint(xValue, yValue);
                pointsTableView.getItems().add(tableDataPoint);

            }
        });
    }

    private void updateGraph() {
    series.getData().clear();
    for (SplinePoint point : pointsTableView.getItems()) {
        series.getData().add(new XYChart.Data<>(point.getX(), point.getY()));
    }
}

    private void addDragHandlers(XYChart.Data<Number, Number> dataPoint) {
        Node node = dataPoint.getNode();

        node.setOnMousePressed(event -> {
            node.setCursor(Cursor.MOVE);
        });

        node.setOnMouseReleased(event -> {
            node.setCursor(Cursor.HAND);
        });

        node.setOnMouseReleased(event -> {
            node.setCursor(Cursor.HAND);
            node.setStyle(""); // Reset to default style when dragging ends
        });

    node.setOnMouseDragged(event -> {
        double xValue = xAxis.getValueForDisplay(scatterChart.getXAxis().sceneToLocal(event.getSceneX(), event.getSceneY()).getX()).doubleValue();
        double yValue = yAxis.getValueForDisplay(scatterChart.getYAxis().sceneToLocal(event.getSceneX(), event.getSceneY()).getY()).doubleValue();
        dataPoint.setXValue(xValue);
        dataPoint.setYValue(yValue);

        // Update the corresponding table entry
        int index = series.getData().indexOf(dataPoint);
        SplinePoint point = pointsTableView.getItems().get(index);
        point.setX(xValue);
        point.setY(yValue);
        pointsTableView.refresh();
    });
    }
}
