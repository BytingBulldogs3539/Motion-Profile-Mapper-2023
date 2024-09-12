package motion.profile.mapper;

import java.util.List;

import motion.profile.mapper.CubicSpline.CubicSpline2D;

public class Path {
    private CubicSpline2D spline = null;
    private LinearSpline2D linearSpline = null;
    private PathType type;

    public enum PathType {
        CUBIC, LINEAR
    }

    public Path(List<ControlPoint> controlPoints, PathType type) {
        this.type = type;
        if (controlPoints == null || controlPoints.size() < 2) {
            return;
        }
        if (controlPoints.size() == 2 || type == PathType.LINEAR) {
            linearSpline = new LinearSpline2D(controlPoints);
        } else {
            spline = new CubicSpline2D(controlPoints);
        }
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
}