package motion.profile.mapper;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.stream.Collectors;

import javafx.beans.Observable;
import javafx.beans.property.SimpleStringProperty;
import javafx.collections.FXCollections;
import javafx.collections.ListChangeListener;
import javafx.collections.ObservableList;
import javafx.scene.chart.XYChart;
import javafx.util.Callback;

public class Path {

    Callback<ControlPoint, Observable[]> extractor = new Callback<ControlPoint, Observable[]>() {
        @Override
        public Observable[] call(ControlPoint p) {
            return new Observable[] {p.getXStringProp(), p.getYStringProp(), p.getRotationStringProp()};
        }
    };
    private final ObservableList<ControlPoint> splinePoints = FXCollections.observableArrayList(extractor);
    private final ObservableList<XYChart.Data<Number, Number>> chartData = FXCollections.observableArrayList();

    private final ObservableList<XYChart.Data<Number, Number>> splineChartData = FXCollections.observableArrayList();

    private SimpleStringProperty name = new SimpleStringProperty();
    private SimpleDateFormat dateFormatter = new SimpleDateFormat("MM/dd/yy HH:mm:ss a");
    private SimpleStringProperty modifiedTime = new SimpleStringProperty();
    private ParametricSpline spline;

    public Path(String name) {
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
            spline = new ParametricSpline(splinePoints);
            splineChartData.clear();
            for (double i = 0; i < spline.getLength(); i+=0.1) {
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
        });
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
