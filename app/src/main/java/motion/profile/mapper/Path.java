package motion.profile.mapper;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.stream.Collectors;

import javafx.beans.property.SimpleStringProperty;
import javafx.collections.FXCollections;
import javafx.collections.ListChangeListener;
import javafx.collections.ObservableList;
import javafx.scene.chart.XYChart;

public class Path {

    private final ObservableList<SplinePoint> splinePoints = FXCollections.observableArrayList();
    private final ObservableList<XYChart.Data<Number, Number>> chartData = FXCollections.observableArrayList();
    private SimpleStringProperty name = new SimpleStringProperty();
    private SimpleDateFormat dateFormatter = new SimpleDateFormat("MM/dd/yy HH:mm:ss a");
    private SimpleStringProperty modifiedTime = new SimpleStringProperty();

    public Path(String name) {
        setName(name);
        updateModifiedTime();
        initializeListeners();
    }

    private void initializeListeners() {
        splinePoints.addListener((ListChangeListener.Change<? extends SplinePoint> change) -> {
            while (change.next()) {
                    if (change.wasRemoved()) {
                        chartData.removeAll(change.getRemoved().stream()
                            .map(SplinePoint::getDataPoint)
                            .collect(Collectors.toList()));
                    }
                    if (change.wasAdded()) {
                        chartData.addAll(change.getAddedSubList().stream()
                            .map(SplinePoint::getDataPoint)
                            .collect(Collectors.toList()));
                    }
            }
            printChartData();
        });
    }

    private void printChartData() {
        System.out.println(chartData.size() + " points in chart data:");
        for (XYChart.Data<Number, Number> data : chartData) {
            Number x = data.getXValue();
            Number y = data.getYValue();
            System.out.println("X: " + x + ", Y: " + y);
        }
    }

    public void updateModifiedTime() {
        modifiedTime.set(dateFormatter.format(Calendar.getInstance().getTime()));
    }

    public SimpleStringProperty getModifiedTime() {
        return modifiedTime;
    }

    public void addPoint(SplinePoint point) {
        splinePoints.add(point);
        updateModifiedTime();
    }

    public void removePoint(int index) {
        if (index >= 0 && index < splinePoints.size()) {
            splinePoints.remove(index);
        }
        updateModifiedTime();
    }

    public void removePoint(SplinePoint point) {
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

    public ObservableList<SplinePoint> getSplinePoints() {
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
