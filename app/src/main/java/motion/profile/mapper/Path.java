package motion.profile.mapper;

import java.util.ArrayList;

import javafx.collections.FXCollections;
import javafx.collections.ObservableList;

public class Path {

    private final ObservableList<SplinePoint> splinePoints = FXCollections.observableArrayList(new ArrayList<SplinePoint>());

    private String name;

    public Path(String name) {
        this.name = name;
    }

    public void addPoint(SplinePoint point) {
        splinePoints.add(point);
    }

    public void removePoint(int index) {
        if (index >= 0 && index < splinePoints.size()) {
            splinePoints.remove(index);
        }
    }

    public ObservableList<SplinePoint> getSplinePoints() {
        return splinePoints;
    }

    public void clearPoints() {
        splinePoints.clear();
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getName() {
        return name;
    }

    @Override
    public String toString() {
        return name;
    }
}
