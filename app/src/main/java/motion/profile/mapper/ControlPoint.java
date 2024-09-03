package motion.profile.mapper;

import edu.wpi.first.math.geometry.Pose2d;
import edu.wpi.first.math.geometry.Rotation2d;
import edu.wpi.first.math.geometry.Translation2d;
import javafx.beans.property.SimpleStringProperty;
import javafx.scene.Cursor;
import javafx.scene.Node;
import javafx.scene.chart.XYChart;

/**
 * Represents a point on a spline path with a translation and rotation.
 * Automatically adds the point to the path upon creation.
 */
public class ControlPoint {
    private Translation2d translation = new Translation2d();
    private Rotation2d rotation = new Rotation2d();
    private final Path path;
    private final XYChart.Data<Number, Number> dataPoint = new XYChart.Data<>(getX(), getY());
    public SimpleStringProperty xStringProp = new SimpleStringProperty();
    public SimpleStringProperty yStringProp = new SimpleStringProperty();
    public SimpleStringProperty rotationStringProp = new SimpleStringProperty();

    /**
     * Constructs a SplinePoint with the specified coordinates, rotation, and path.
     * 
     * @param x       The x-coordinate of the point.
     * @param y       The y-coordinate of the point.
     * @param degrees The rotation of the point in degrees.
     * @param path    The path to which this point belongs.
     */
    public ControlPoint(double x, double y, double degrees, Path path) {
        this.path = path;
        setTranslation(new Translation2d(x, y));
        setRotation(Rotation2d.fromDegrees(degrees));
        path.addPoint(this);
        addDragHandlers();
    }

    private void addDragHandlers() {

         // Retrieve the node and apply the style
         dataPoint.nodeProperty().addListener((observable, oldValue, newValue) -> {
            if (newValue != null) {
                newValue.toFront();
            }
        });

        Node node = getDataPoint().getNode();
        node.setCursor(Cursor.HAND);
        node.setOnMousePressed(event -> {
            node.setCursor(Cursor.MOVE);

        });
        node.setOnMouseClicked(event -> {
            App.controller.selectAndScrollTo(this);
            event.consume();
        });

        node.setOnMouseReleased(event -> {
            node.setCursor(Cursor.HAND);
        });

        node.setOnMouseDragged(event -> {
            setTranslation(App.controller.getChartMouseClickPosition(event));
            // Update the corresponding table entry
            App.controller.selectAndScrollTo(this);
            
        });
    }

    /**
     * Updates the modified time of the path to which this point belongs. Sets to
     * current time.
     */
    public void updateModifiedTime() {
        path.updateModifiedTime();
    }

    /**
     * Sets the translation of this point to the specified coordinates.
     * 
     * @param x The new x-coordinate.
     * @param y The new y-coordinate.
     */
    public void setXY(double x, double y) {
        setTranslation(new Translation2d(x, y));
    }

    /**
     * Sets the x-coordinate of this point.
     * 
     * @param x The new x-coordinate.
     */
    public void setX(double x) {
        setTranslation(new Translation2d(x, translation.getY()));
    }

    /**
     * Sets the y-coordinate of this point.
     * 
     * @param y The new y-coordinate.
     */
    public void setY(double y) {
        setTranslation(new Translation2d(translation.getX(), y));
    }

    /**
     * Sets the rotation of this point in degrees.
     * 
     * @param degrees The new rotation in degrees.
     */
    public void setRotationDegrees(double degrees) {
        setRotation(Rotation2d.fromDegrees(degrees));
    }

    /**
     * Sets the translation of this point.
     * 
     * @param translation The new translation.
     */
    public void setTranslation(Translation2d translation) {
        this.translation = translation;
        this.dataPoint.setXValue(getX());
        this.dataPoint.setYValue(getY());
        xStringProp.setValue(String.format("%.3f", getX()));
        yStringProp.setValue(String.format("%.3f", getY()));
        updateModifiedTime();
    }

    /**
     * Sets the rotation of this point.
     * 
     * @param rotation The new rotation.
     */
    public void setRotation(Rotation2d rotation) {
        this.rotation = rotation;
        rotationStringProp.setValue(String.format("%.3f", getRotationDegrees()));
        updateModifiedTime();
    }

    /**
     * Sets the pose of this point.
     * 
     * @param pose The new pose.
     */
    public void setPose(Pose2d pose) {
        setTranslation(pose.getTranslation());
        setRotation(pose.getRotation());
    }

    /**
     * Gets the pose of this point.
     * 
     * @return The current pose.
     */
    public Pose2d getPose() {
        return new Pose2d(translation, rotation);
    }

    /**
     * Gets the translation of this point.
     * 
     * @return The current translation.
     */
    public Translation2d getTranslation() {
        return translation;
    }

    /**
     * Gets the rotation of this point.
     * 
     * @return The current rotation.
     */
    public Rotation2d getRotation() {
        return rotation;
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
        return translation.getX();
    }

    /**
     * Gets the y-coordinate of this point.
     * 
     * @return The y-coordinate.
     */
    public double getY() {
        return translation.getY();
    }

    /**
     * Gets the rotation of this point in degrees.
     * 
     * @return The rotation in degrees.
     */
    public double getRotationDegrees() {
        return rotation.getDegrees();
    }

    /**
     * Gets the data point associated with this spline point.
     * 
     * @return The data point.
     */
    public XYChart.Data<Number, Number> getDataPoint() {
        return dataPoint;
    }


    /**
     * Gets the x-coordinate property of this point.
     * 
     * @return The x-coordinate property.
     */
    public SimpleStringProperty getXStringProp() {
        return xStringProp;
    }

    /**
     * Gets the y-coordinate property of this point.
     * 
     * @return The y-coordinate property.
     */
    public SimpleStringProperty getYStringProp() {
        return yStringProp;
    }

    /**
     * Gets the rotation property of this point.
     * 
     * @return The rotation property.
     */
    public SimpleStringProperty getRotationStringProp() {
        return rotationStringProp;
    }

}