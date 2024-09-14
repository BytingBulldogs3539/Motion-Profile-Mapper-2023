package motion.profile.mapper;

import edu.wpi.first.math.geometry.Pose2d;
import edu.wpi.first.math.geometry.Rotation2d;
import edu.wpi.first.math.geometry.Translation2d;

/**
 * Represents a point on a spline path with a translation and rotation.
 * Automatically adds the point to the path upon creation.
 */
public class ControlPoint {
    private Translation2d translation2d = new Translation2d(0, 0);
    private Rotation2d rotation2d = new Rotation2d();
    private final Path path;

    /**
     * Constructs a SplinePoint with the specified path.
     * 
     * @param path The path to which this point belongs.
     */
    public ControlPoint(Path path) {
        this.path = path;
    }

    /**
     * Constructs a SplinePoint with the specified coordinates, rotation, and path.
     * 
     * @param translation2d The translation of the point.
     * @param rotation      The rotation of the point.
     * @param path          The path to which this point belongs.
     */
    public ControlPoint(Translation2d translation2d, Rotation2d rotation, Path path) {
        this.path = path;
        this.translation2d = translation2d;
        this.rotation2d = rotation;
    }

    // Setters
    /**
     * Sets the translation of this point to the specified coordinates.
     * 
     * @param x The new x-coordinate.
     * @param y The new y-coordinate.
     */
    public void setXY(double x, double y) {
        this.translation2d = new Translation2d(x, y);
    }

    /**
     * Sets the x-coordinate of this point.
     * 
     * @param x The new x-coordinate.
     */
    public void setX(double x) {
        this.translation2d = new Translation2d(x, getY());
    }

    /**
     * Sets the y-coordinate of this point.
     * 
     * @param y The new y-coordinate.
     */
    public void setY(double y) {
        this.translation2d = new Translation2d(getX(), y);
    }

    /**
     * Sets the rotation of this point in degrees.
     * 
     * @param degrees The new rotation in degrees.
     */
    public void setRotationDegrees(double degrees) {
        rotation2d = Rotation2d.fromDegrees(degrees);
    }

    /**
     * Sets the translation of this point.
     * 
     * @param translation The new translation.
     */
    public void setTranslation(Translation2d translation) {
        this.translation2d = translation;
    }

    /**
     * Sets the rotation of this point.
     * 
     * @param rotation The new rotation.
     */
    public void setRotation(Rotation2d rotation) {
        rotation2d = rotation;
    }

    /**
     * Sets the pose of this point.
     * 
     * @param pose The new pose.
     */
    public void setPose(Pose2d pose) {
        this.translation2d = pose.getTranslation();
        this.rotation2d = pose.getRotation();
    }

    // Getters
    /**
     * Gets the pose of this point.
     * 
     * @return The current pose.
     */
    public Pose2d getPose() {
        return new Pose2d(getTranslation(), getRotation());
    }

    /**
     * Gets the translation of this point.
     * 
     * @return The current translation.
     */
    public Translation2d getTranslation() {
        return translation2d;
    }

    /**
     * Gets the rotation of this point.
     * 
     * @return The current rotation.
     */
    public Rotation2d getRotation() {
        return rotation2d;
    }

    /**
     * Gets the path to which this point belongs.
     * 
     * @return The path.
     */
    public Path getPath() {
        return path;
    }

    /**
     * Gets the x-coordinate of this point.
     * 
     * @return The x-coordinate.
     */
    public double getX() {
        return this.translation2d.getX();
    }

    /**
     * Gets the y-coordinate of this point.
     * 
     * @return The y-coordinate.
     */
    public double getY() {
        return this.translation2d.getY();
    }

    /**
     * Gets the rotation of this point in degrees.
     * 
     * @return The rotation in degrees.
     */
    public double getRotationDegrees() {
        return this.rotation2d.getDegrees();
    }
}