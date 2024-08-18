package motion.profile.mapper;

import edu.wpi.first.math.geometry.Pose2d;
import edu.wpi.first.math.geometry.Rotation2d;
import edu.wpi.first.math.geometry.Translation2d;

public class SplinePoint {
    private Translation2d translation;
    private Rotation2d rotation;
    private final Path path;

    // Automatically adds the point to the path on creation.
    public SplinePoint(double x, double y, double degrees, Path path) {
        this.translation = new Translation2d(x, y);
        this.rotation = Rotation2d.fromDegrees(degrees);
        this.path = path;
        path.addPoint(this);
    }

    public Path getPath() {
        return path;
    }

    public double getX() {
        return translation.getX();
    }

    public double getY() {
        return translation.getY();
    }

    public double getRotationDegrees() {
        return rotation.getDegrees();
    }

    public void setX(double x) {
        this.translation = new Translation2d(x, translation.getY());
    }

    public void setY(double y) {
        this.translation = new Translation2d(translation.getX(), y);
    }

    public void setRotationDegrees(double degrees) {
        this.rotation = Rotation2d.fromDegrees(degrees);
    }

    public Translation2d getTranslation() {
        return translation;
    }

    public Rotation2d getRotation() {
        return rotation;
    }

    public void setTranslation(Translation2d translation) {
        this.translation = translation;
    }

    public void setRotation(Rotation2d rotation) {
        this.rotation = rotation;
    }

    public Pose2d getPose() {
        return new Pose2d(translation, rotation);
    }

    public void setPose(Pose2d pose) {
        this.translation = pose.getTranslation();
        this.rotation = pose.getRotation();
    }
}