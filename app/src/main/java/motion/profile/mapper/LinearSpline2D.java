package motion.profile.mapper;

import java.util.List;

import edu.wpi.first.math.geometry.Translation2d;

public class LinearSpline2D {
    private final List<ControlPoint> points;

    public LinearSpline2D(List<ControlPoint> points) {
        this.points = points;
    }

    public double getLength() {
        double length = 0.0;
        for (int i = 1; i < points.size(); i++) {
            length += points.get(i).getTranslation().getDistance(points.get(i - 1).getTranslation());
        }
        return length;
    }

    public Translation2d getTranslation(double t) {
        double totalLength = getLength();

        if (t <= 0.0) {
            return points.get(0).getTranslation();
        } else if (t >= totalLength) {
            return points.get(points.size() - 1).getTranslation();
        }

        double accumulatedLength = 0.0;

        for (int i = 1; i < points.size(); i++) {
            double segmentLength = points.get(i).getTranslation().getDistance(points.get(i - 1).getTranslation());
            if (accumulatedLength + segmentLength >= t) {
                double segmentT = (t - accumulatedLength) / segmentLength;
                return points.get(i - 1).getTranslation().interpolate(points.get(i).getTranslation(), segmentT);
            }
            accumulatedLength += segmentLength;
        }

        return points.get(points.size() - 1).getTranslation();
    }
}