package motion.profile.mapper;

import java.util.List;

import edu.wpi.first.math.geometry.Translation2d;
import motion.profile.mapper.CubicSpline.CubicSpline2D;

public class ParametricSpline {
    private CubicSpline2D spline = null;

    public ParametricSpline(List<ControlPoint> controlPoints) {
        if (controlPoints == null || controlPoints.size() < 3) {
            return;
        }

        double[] x = new double[controlPoints.size()];
        double[] y = new double[controlPoints.size()];

        for (int i = 0; i < controlPoints.size(); i++) {
            x[i] = controlPoints.get(i).getX();
            y[i] = controlPoints.get(i).getY();
        }
        spline = new CubicSpline2D(x, y);
    }

    public double getX(double t) {
        if (spline == null) {
            return 0.0;
        }
        if (t > getLength()) {
            return spline.calcPosition(getLength()).getX();
        }
        return spline.calcPosition(t).getX();
    }

    public double getY(double t) {
        if (spline == null) {
            return 0.0;
        }
        if (t > getLength()) {
            return spline.calcPosition(getLength()).getY();
        }
        return spline.calcPosition(t).getY();
    }

    public Translation2d getTranslation2d(double t) {
        return new Translation2d(getX(t), getY(t));
    }

    public double getLength() {
        if(spline == null) {
            return 0.0;
        }
        return spline.getLength();
    }
}
