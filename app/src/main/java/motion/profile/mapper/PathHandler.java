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


public class PathHandler extends Path {

    Callback<ControlPointHandler, Observable[]> extractor = new Callback<ControlPointHandler, Observable[]>() {
        @Override
        public Observable[] call(ControlPointHandler p) {
            return new Observable[] { p.getXProp(), p.getYProp(), p.getRotationProp() };
        }
    };
    private final ObservableList<ControlPointHandler> splineControlPoints = FXCollections.observableArrayList(extractor);
    private final ObservableList<XYChart.Data<Number, Number>> chartData = FXCollections.observableArrayList();

    private final ObservableList<XYChart.Data<Number, Number>> splineChartData = FXCollections.observableArrayList();
    private final SimpleBooleanProperty isSpline = new SimpleBooleanProperty(false);
    private SimpleDateFormat dateFormatter = new SimpleDateFormat("MM/dd/yy HH:mm:ss a");

    private final SimpleStringProperty name = new SimpleStringProperty();
    private final SimpleStringProperty modifiedTimeProp = new SimpleStringProperty();

    public PathHandler(String name, boolean isSpline) {
        super(name, isSpline ? Path.PathType.CUBIC : Path.PathType.LINEAR);
        this.name.set(name);
        updateModifiedTime();
        splineControlPoints.addListener((ListChangeListener.Change<? extends ControlPointHandler> change) -> {
            while (change.next()) {
                if (change.wasRemoved()) {
                    chartData.removeAll(change.getRemoved().stream()
                            .map(ControlPointHandler::getDataPoint)
                            .collect(Collectors.toList()));
                }
                if (change.wasAdded()) {
                    chartData.addAll(change.getAddedSubList().stream()
                            .map(ControlPointHandler::getDataPoint)
                            .collect(Collectors.toList()));
                }
            }
            generateSplineData();
        });
    }

    private void generateSplineData() {
        generateSpline();
        splineChartData.clear();
        for (double i = 0; i < getLength(); i += 0.1) {
            XYChart.Data<Number, Number> data = new XYChart.Data<>(getX(i), getY(i));

            // Retrieve the node and apply the style
            data.nodeProperty().addListener((observable, oldValue, newValue) -> {
                if (newValue != null) {
                    newValue.setStyle(
                            "-fx-background-color: red; -fx-padding: 2px;");
                    newValue.setViewOrder(1);
                }
            });
            splineChartData.add(data);
            // data.getNode().setStyle(
            // "-fx-background-color: red; -fx-padding: 2px;");
            // data.getNode().setViewOrder(1);
        }
    }

    public void setSplineMode(boolean isSpline) {
        if(isSpline!=isSpline())
        {
            super.setType(isSpline ? Path.PathType.CUBIC : Path.PathType.LINEAR);
            this.isSpline.set(isSpline);
            generateSplineData();
        }

    }

    public void addControlPoint(ControlPointHandler point) {
        splineControlPoints.add(point);
        super.addControlPoint(point);
        generateSplineData();
        updateModifiedTime();
    }

    public void removePoint(int index) {
        if (index >= 0 && index < splineControlPoints.size()) {
            super.removeControlPoint(index);
            splineControlPoints.remove(index);
            generateSplineData();
            updateModifiedTime();
        }
    }

    public void removePoint(ControlPointHandler point) {
        super.removeControlPoint(point);
        splineControlPoints.remove(point);
        updateModifiedTime();
    }

    public void updateModifiedTime() {
        String time = dateFormatter.format(Calendar.getInstance().getTime());
        super.setModifiedTime(time);
        modifiedTimeProp.set(time);
    }

    public void clearPoints() {
        super.clearControlPoints();
        splineControlPoints.clear();
    }

    public void setName(String name) {
        super.setName(name);
        this.name.set(name);
    }

    public SimpleStringProperty getNameProp() {
        return name;
    }

    public ObservableList<ControlPointHandler> getSplineControlPoints() {
        return splineControlPoints;
    }

    public ObservableList<XYChart.Data<Number, Number>> getChartData() {
        return chartData;
    }

    public ObservableList<XYChart.Data<Number, Number>> getSplineChartData() {
        return splineChartData;
    }

    public SimpleStringProperty getModifiedTimeProp() {
        return modifiedTimeProp;
    }

    @Override
    public String toString() {
        return getName();
    }
}
