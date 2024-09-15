package motion.profile.mapper;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.List;
import java.util.stream.Collectors;

import com.fasterxml.jackson.annotation.JsonIgnore;

import javafx.beans.Observable;
import javafx.beans.property.SimpleBooleanProperty;
import javafx.beans.property.SimpleStringProperty;
import javafx.beans.value.WeakChangeListener;
import javafx.collections.FXCollections;
import javafx.collections.ListChangeListener;
import javafx.collections.ObservableList;
import javafx.scene.chart.XYChart;
import javafx.util.Callback;

public class PathHandler extends Path {

    @JsonIgnore
    private final ObservableList<XYChart.Data<Number, Number>> controlPointData = FXCollections.observableArrayList();
    @JsonIgnore
    private final ObservableList<XYChart.Data<Number, Number>> splineChartData = FXCollections.observableArrayList();
    @JsonIgnore
    private final SimpleBooleanProperty isSplineProp = new SimpleBooleanProperty(false);
    @JsonIgnore
    private final SimpleStringProperty nameProp = new SimpleStringProperty();
    @JsonIgnore
    private final SimpleStringProperty modifiedTimeProp = new SimpleStringProperty();
    @JsonIgnore
    private final SimpleDateFormat dateFormatter = new SimpleDateFormat("MM/dd/yy HH:mm:ss a");

    @JsonIgnore
    private final Callback<ControlPointHandler, Observable[]> extractor = p -> new Observable[] {
            p.getXProp(), p.getYProp(), p.getRotationProp()
    };

    @JsonIgnore
    private final ObservableList<ControlPointHandler> splineControlPoints = FXCollections
            .observableArrayList(extractor);

    public PathHandler(String name, PathType type) {
        super(name, type);
        this.nameProp.set(name);
        initializeListeners();
    }
    public PathHandler(Path path)
    {
        super(path.getName(), path.getType());
        this.nameProp.set(path.getName());
        this.modifiedTimeProp.set(path.getModifiedTime());
        initializeListeners();
        for(ControlPoint point : path.getControlPoints())
        {
            ControlPointHandler cpHandler = new ControlPointHandler(point, this);
            this.forceAddControlPoint(cpHandler);
        }

    }

    // Getters
    @JsonIgnore
    public SimpleStringProperty getNameProp() {
        return nameProp;
    }

    @JsonIgnore
    public ObservableList<ControlPointHandler> getSplineControlPoints() {
        return splineControlPoints;
    }

    @JsonIgnore
    public ObservableList<XYChart.Data<Number, Number>> getControlPointData() {
        return controlPointData;
    }

    @JsonIgnore
    public ObservableList<XYChart.Data<Number, Number>> getSplineChartData() {
        return splineChartData;
    }

    @JsonIgnore
    public SimpleStringProperty getModifiedTimeProp() {
        return modifiedTimeProp;
    }

    // Setters
    public void setName(String name) {
        super.setName(name);
        this.nameProp.set(name);
    }

    public void setSplineMode(boolean isSpline) {
        if (isSpline != isSpline()) {
            super.setType(isSpline ? Path.PathType.CUBIC : Path.PathType.LINEAR);
            this.isSplineProp.set(isSpline);
            generateSplineData();
        }
    }

    public void setControlPoints(List<ControlPoint> controlPoints) {
        throw new UnsupportedOperationException("Cannot set control points directly on PathHandler");
    }

    public void setModifiedTime(String modifiedTime) {
        super.setModifiedTime(modifiedTime);
        modifiedTimeProp.set(modifiedTime);
    }

    // Public Methods
    public void addControlPoint(ControlPointHandler point) {
        forceAddControlPoint(point);
        updateModifiedTime();
        generateSplineData();
    }

    /*
     * Adds a control point to the spline without updating the modified time or generating the spline data.
     */
    public void forceAddControlPoint(ControlPointHandler point) {
        super.addControlPoint(point);
        splineControlPoints.add(point);
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

    public void clearPoints() {
        super.clearControlPoints();
        splineControlPoints.clear();
    }

    public void updateModifiedTime() {
        String time = dateFormatter.format(Calendar.getInstance().getTime());
        super.setModifiedTime(time);
        modifiedTimeProp.set(time);
    }

    @Override
    public String toString() {
        return getName();
    }

    // Private Methods
    private void initializeListeners() {
        splineControlPoints.addListener((ListChangeListener.Change<? extends ControlPointHandler> change) -> {
            while (change.next()) {
                if (change.wasRemoved()) {
                    controlPointData.removeAll(change.getRemoved().stream()
                            .map(ControlPointHandler::getDataPoint)
                            .collect(Collectors.toList()));
                }
                if (change.wasAdded()) {
                    controlPointData.addAll(change.getAddedSubList().stream()
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
            data.nodeProperty().addListener(new WeakChangeListener<>((observable, oldValue, newValue) -> {
                if (newValue != null) {
                    newValue.setStyle(
                            "-fx-background-color: red; -fx-padding: 2px;");
                    newValue.toBack();
                }
            }));
            splineChartData.add(data);
        }
        for(ControlPointHandler point : splineControlPoints)
        {
            if(point.getDataPoint().getNode()!=null)
            {
                point.getDataPoint().getNode().toFront();
            }
        }
    }
}