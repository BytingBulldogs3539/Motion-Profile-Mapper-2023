package motion.profile.mapper;

import java.util.List;
import java.util.Optional;

import org.apache.commons.math3.analysis.interpolation.SplineInterpolator;
import org.apache.commons.math3.analysis.polynomials.PolynomialSplineFunction;

import edu.wpi.first.math.geometry.Translation2d;

public class ParametricSpline {
    private PolynomialSplineFunction splineX = null;
    private PolynomialSplineFunction splineY = null;
    private Optional<Double> length = Optional.empty();

    public ParametricSpline(List<ControlPoint> controlPoints) {
        if (controlPoints == null || controlPoints.size() < 3) {
            return;
        }
        SplineInterpolator interpolater = new SplineInterpolator();

        double[] x = new double[controlPoints.size()];
        double[] y = new double[controlPoints.size()];
        double[] t = new double[controlPoints.size()];

        for (int i = 0; i < controlPoints.size(); i++) {
            x[i] = controlPoints.get(i).getX();
            y[i] = controlPoints.get(i).getY();
            t[i] = i / (double) (controlPoints.size()-1);

        }
        

        splineX = interpolater.interpolate(t, x);
        splineY = interpolater.interpolate(t, y);
    }

    public double getX(double t) {
        if (splineX == null) {
            return 0.0;
        }
        if (t > 1) {
            return splineX.value(1);
        }
        return splineX.value(t);

    }

    public double getY(double t) {
        if (splineX == null) {
            return 0.0;
        }
        if (t > 1) {
            return splineY.value(1);
        }
        return splineY.value(t);
    }

    public Translation2d getTranslation2d(double t) {
        return new Translation2d(getX(t), getY(t));
    }

    /**
     * Calculate the length of the spline using numerical integration.
     * 
     * @return The length of the spline.
     */
    public double calculateLength() {
        if (length.isPresent()) {
            return length.get();
        }
        if (splineX == null || splineY == null) {
            return 0.0;
        }
        PolynomialSplineFunction derivativeX = splineX.polynomialSplineDerivative();
        PolynomialSplineFunction derivativeY = splineY.polynomialSplineDerivative();

        double length = 0.0;
        int numSteps = 1000; // Number of steps for numerical integration
        double tMin = 0.0;
        double tMax = 1.0;
        double dt = (tMax - tMin) / numSteps;

        for (int i = 0; i < numSteps; i++) {
            double t1 = tMin + i * dt;
            double t2 = tMin + (i + 1) * dt;
            double dx1 = derivativeX.value(t1);
            double dy1 = derivativeY.value(t1);
            double dx2 = derivativeX.value(t2);
            double dy2 = derivativeY.value(t2);

            double ds1 = Math.sqrt(dx1 * dx1 + dy1 * dy1);
            double ds2 = Math.sqrt(dx2 * dx2 + dy2 * dy2);

            length += 0.5 * (ds1 + ds2) * dt; // Trapezoidal rule
        }
        this.length = Optional.of(length);
        return length;
    }
}
