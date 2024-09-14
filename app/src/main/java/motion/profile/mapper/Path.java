package motion.profile.mapper;

import java.util.ArrayList;
import java.util.List;

import motion.profile.mapper.CubicSpline.CubicSpline2D;

public class Path {
    private CubicSpline2D spline = null;
    private LinearSpline2D linearSpline = null;
    private PathType type;
    private List<ControlPoint> controlPoints = new ArrayList<>();
    private String name;
    private String modifiedTime;

    public enum PathType {
        CUBIC, LINEAR
    }

    // Constructors
    public Path(List<ControlPoint> controlPoints, String name, PathType type) {
        this.controlPoints = controlPoints;
        this.type = type;
        this.name = name;
        generateSpline();
    }

    public Path(String name, PathType type) {
        this.type = type;
        this.name = name;
        generateSpline();
    }

    public Path(List<ControlPoint> controlPoints, String name) {
        this(controlPoints, name, PathType.CUBIC);
    }

    // Getters
    public String getName() {
        return name;
    }

    public String getModifiedTime() {
        return modifiedTime;
    }

    public List<ControlPoint> getControlPoints() {
        return controlPoints;
    }

    public double getX(double t) {
        if (spline != null) {
            if (t > getLength()) {
                return spline.calcPosition(getLength()).getX();
            }
            return spline.calcPosition(t).getX();
        } else if (linearSpline != null) {
            return linearSpline.getTranslation(t).getX();
        }
        return 0.0;
    }

    public double getY(double t) {
        if (spline != null) {
            if (t > getLength()) {
                return spline.calcPosition(getLength()).getY();
            }
            return spline.calcPosition(t).getY();
        } else if (linearSpline != null) {
            return linearSpline.getTranslation(t).getY();
        }
        return 0.0;
    }

    public double getLength() {
        if (spline != null) {
            return spline.getLength();
        } else if (linearSpline != null) {
            return linearSpline.getLength();
        }
        return 0.0;
    }

    // Setters
    public void setName(String name) {
        this.name = name;
    }

    public void setType(PathType type) {
        this.type = type;
    }

    public void setControlPoints(List<ControlPoint> controlPoints) {
        this.controlPoints = controlPoints;
    }

    public void setModifiedTime(String modifiedTime) {
        this.modifiedTime = modifiedTime;
    }

    // Public Methods
    public boolean isSpline() {
        switch (type) {
            case CUBIC:
                return true;
            case LINEAR:
                return false;
            default:
                return false;
        }
    }

    public void addControlPoint(ControlPoint controlPoint) {
        controlPoints.add(controlPoint);
    }

    public void removeControlPoint(ControlPoint controlPoint) {
        controlPoints.remove(controlPoint);
    }

    public void removeControlPoint(int index) {
        controlPoints.remove(index);
    }

    public void removeAllControlPoints(List<? extends ControlPoint> list) {
        controlPoints.removeAll(list);
    }

    public void clearControlPoints() {
        controlPoints.clear();
    }

    public void addAllControlPoints(List<? extends ControlPoint> list) {
        this.controlPoints.addAll(list);
    }

    public boolean generateSpline() {
        if (controlPoints == null || controlPoints.size() < 2) {
            return false;
        }
        if (controlPoints.size() == 2 || type == PathType.LINEAR) {
            linearSpline = new LinearSpline2D(controlPoints);
            spline = null;
        } else {
            spline = new CubicSpline2D(controlPoints);
            linearSpline = null;
        }
        return true;
    }
}