package motion.profile.mapper;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.stream.Collectors;

import javafx.beans.Observable;
import javafx.beans.property.SimpleBooleanProperty;
import javafx.beans.property.SimpleStringProperty;
import javafx.collections.FXCollections;
import javafx.collections.ListChangeListener;
import javafx.collections.ObservableList;
import javafx.scene.chart.XYChart;
import javafx.util.Callback;

public class PathHandler {

    Callback<ControlPoint, Observable[]> extractor = new Callback<ControlPoint, Observable[]>() {
        @Override
        public Observable[] call(ControlPoint p) {
            return new Observable[] { p.getXProp(), p.getYProp(), p.getRotationProp() };
        }
    };
    private final ObservableList<ControlPoint> splinePoints = FXCollections.observableArrayList(extractor);
    private final ObservableList<XYChart.Data<Number, Number>> chartData = FXCollections.observableArrayList();

    private final ObservableList<XYChart.Data<Number, Number>> splineChartData = FXCollections.observableArrayList();
    private final SimpleBooleanProperty isSpline = new SimpleBooleanProperty(false);

    // TODO: convert this class to be just a handler. Dont store anything except the
    // properties and datapoints and ui things.

    private SimpleStringProperty name = new SimpleStringProperty();
    private SimpleDateFormat dateFormatter = new SimpleDateFormat("MM/dd/yy HH:mm:ss a");
    private SimpleStringProperty modifiedTime = new SimpleStringProperty();
    private Path spline;

    public PathHandler(String name) {
        setName(name);
        updateModifiedTime();
        initializeListeners();

    }

    private void initializeListeners() {
        splinePoints.addListener((ListChangeListener.Change<? extends ControlPoint> change) -> {
            while (change.next()) {
                if (change.wasRemoved()) {
                    chartData.removeAll(change.getRemoved().stream()
                            .map(ControlPoint::getDataPoint)
                            .collect(Collectors.toList()));
                }
                if (change.wasAdded()) {
                    chartData.addAll(change.getAddedSubList().stream()
                            .map(ControlPoint::getDataPoint)
                            .collect(Collectors.toList()));
                }
            }
            generateSpline();
        });
        isSpline.addListener((observable, oldValue, newValue) -> {
            generateSpline();
        });
    }

    private void generateSpline() {
        Path.PathType pathType = isSpline.get() ? Path.PathType.CUBIC : Path.PathType.LINEAR;
        spline = new Path(splinePoints, pathType);
        splineChartData.clear();
        for (double i = 0; i < spline.getLength(); i += 0.1) {
            XYChart.Data<Number, Number> data = new XYChart.Data<>(spline.getX(i), spline.getY(i));

            // Retrieve the node and apply the style
            data.nodeProperty().addListener((observable, oldValue, newValue) -> {
                if (newValue != null) {
                    newValue.setStyle(
                            "-fx-background-color: red; -fx-padding: 2px;");
                    newValue.setViewOrder(1);
                }
            });
            splineChartData.add(data);
        }
    }

    public ObservableList<XYChart.Data<Number, Number>> getSplineChartData() {
        return splineChartData;
    }

    public void updateModifiedTime() {
        modifiedTime.set(dateFormatter.format(Calendar.getInstance().getTime()));
    }

    public SimpleStringProperty getModifiedTime() {
        return modifiedTime;
    }

    public void setIsSpline(boolean isSpline) {
        this.isSpline.set(isSpline);
    }

    public SimpleBooleanProperty getIsSpline() {
        return isSpline;
    }

    public void addPoint(ControlPoint point) {
        splinePoints.add(point);
        updateModifiedTime();
    }

    public void removePoint(int index) {
        if (index >= 0 && index < splinePoints.size()) {
            splinePoints.remove(index);
        }
        updateModifiedTime();
    }

    public void removePoint(ControlPoint point) {
        splinePoints.remove(point);
        updateModifiedTime();
    }

    public void clearPoints() {
        splinePoints.clear();
    }

    public void setName(String name) {
        this.name.set(name);
    }

    public SimpleStringProperty getName() {
        return name;
    }

    public ObservableList<ControlPoint> getSplinePoints() {
        return splinePoints;
    }

    public ObservableList<XYChart.Data<Number, Number>> getChartData() {
        return this.chartData;
    }

    @Override
    public String toString() {
        return name.get();
    }
}
