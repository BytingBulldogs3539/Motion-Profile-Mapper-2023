package motion.profile.mapper;

import edu.wpi.first.math.geometry.Pose2d;
import edu.wpi.first.math.geometry.Rotation2d;
import edu.wpi.first.math.geometry.Translation2d;
import javafx.animation.PauseTransition;
import javafx.beans.property.SimpleDoubleProperty;
import javafx.scene.Cursor;
import javafx.scene.Node;
import javafx.scene.chart.XYChart;
import javafx.scene.control.ContextMenu;
import javafx.scene.control.MenuItem;
import javafx.util.Duration;

/**
 * Represents a point on a spline path with a translation and rotation.
 * Automatically adds the point to the path upon creation.
 */
public class ControlPoint {
    private final SimpleDoubleProperty x = new SimpleDoubleProperty();
    private final SimpleDoubleProperty y = new SimpleDoubleProperty();
    private final SimpleDoubleProperty rotation = new SimpleDoubleProperty(); // Stored in degrees
    private final PathHandler path;
    private final XYChart.Data<Number, Number> dataPoint = new XYChart.Data<>(getX(), getY());

    // TODO: convert this class to be just a handler. Dont store anything except the
    // properties and datapoints and ui things.

    /**
     * Constructs a SplinePoint with the specified coordinates, rotation, and path.
     * 
     * @param x       The x-coordinate of the point.
     * @param y       The y-coordinate of the point.
     * @param degrees The rotation of the point in degrees.
     * @param path    The path to which this point belongs.
     */
    public ControlPoint(double x, double y, double degrees, PathHandler path) {
        this.path = path;
        setXY(x, y);
        setRotationDegrees(degrees);
        path.addPoint(this);
        addHandlers();
        addContextMenu();
    }

    private void addHandlers() {
        Node node = getDataPoint().getNode();

        node.setCursor(Cursor.HAND);

        node.setOnMousePressed(event -> {
            node.setCursor(Cursor.MOVE);
            App.controller.setDragging(true); // Set dragging flag to true
            event.consume();
        });
        node.setOnMouseClicked(event -> {
            App.controller.selectAndScrollTo(this);
            event.consume();
        });

        node.setOnMouseReleased(event -> {
            node.setCursor(Cursor.HAND);
            PauseTransition pause = new PauseTransition(Duration.millis(20)); // Adds 20ms delay to prevent accidental
                                                                              // double clicks
            pause.setOnFinished(e -> App.controller.setDragging(false));
            pause.play();
        });

        node.setOnMouseDragged(event -> {
            // Get the chart's x and y axis limits
            double xLowerBound = App.controller.getXAxis().getLowerBound();
            double xUpperBound = App.controller.getXAxis().getUpperBound();
            double yLowerBound = App.controller.getYAxis().getLowerBound();
            double yUpperBound = App.controller.getYAxis().getUpperBound();

            // Get the new translation
            Translation2d newTranslation = App.controller.getChartMouseClickPosition(event);

            // Bound the new translation within the axis limits
            double boundedX = Math.max(xLowerBound, Math.min(newTranslation.getX(), xUpperBound));
            double boundedY = Math.max(yLowerBound, Math.min(newTranslation.getY(), yUpperBound));

            // Round the coordinates to three decimal places
            boundedX = Math.round(boundedX * 1000.0) / 1000.0;
            boundedY = Math.round(boundedY * 1000.0) / 1000.0;

            // Set the bounded translation
            setTranslation(new Translation2d(boundedX, boundedY));
            // Update the corresponding table entry
            App.controller.selectAndScrollTo(this);

        });

        getDataPoint().nodeProperty().addListener((observable, oldValue, newValue) -> {
            if (newValue != null) {
                newValue.setViewOrder(10);
            }
        });
    }

    private void addContextMenu() {
        Node node = getDataPoint().getNode();
        ContextMenu contextMenu = new ContextMenu();
        MenuItem deleteItem = new MenuItem("Delete");

        deleteItem.setOnAction(event -> {
            path.removePoint(this);
        });

        contextMenu.getItems().add(deleteItem);

        node.setOnContextMenuRequested(event -> {
            contextMenu.show(node, event.getScreenX(), event.getScreenY());
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
        setX(x);
        setY(y);
    }

    /**
     * Sets the x-coordinate of this point.
     * 
     * @param x The new x-coordinate.
     */
    public void setX(double x) {
        this.x.set(x);
        this.dataPoint.setXValue(getX());
        updateModifiedTime();
    }

    /**
     * Sets the y-coordinate of this point.
     * 
     * @param y The new y-coordinate.
     */
    public void setY(double y) {
        this.y.set(y);
        this.dataPoint.setYValue(getY());
        updateModifiedTime();
    }

    /**
     * Sets the rotation of this point in degrees.
     * 
     * @param degrees The new rotation in degrees.
     */
    public void setRotationDegrees(double degrees) {
        this.rotation.set(degrees);
        updateModifiedTime();
    }

    /**
     * Sets the translation of this point.
     * 
     * @param translation The new translation.
     */
    public void setTranslation(Translation2d translation) {
        setX(translation.getX());
        setY(translation.getY());
    }

    /**
     * Sets the rotation of this point.
     * 
     * @param rotation The new rotation.
     */
    public void setRotation(Rotation2d rotation) {
        setRotationDegrees(rotation.getDegrees());
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
        return new Pose2d(getTranslation(), getRotation());
    }

    /**
     * Gets the translation of this point.
     * 
     * @return The current translation.
     */
    public Translation2d getTranslation() {
        return new Translation2d(getX(), getY());
    }

    /**
     * Gets the rotation of this point.
     * 
     * @return The current rotation.
     */
    public Rotation2d getRotation() {
        return Rotation2d.fromDegrees(getRotationDegrees());
    }

    /**
     * Gets the path to which this point belongs.
     * 
     * @return The path.
     */
    public PathHandler getPath() {
        return path;
    }

    /**
     * Gets the x-coordinate of this point.
     * 
     * @return The x-coordinate.
     */
    public double getX() {
        return this.x.get();
    }

    /**
     * Gets the y-coordinate of this point.
     * 
     * @return The y-coordinate.
     */
    public double getY() {
        return this.y.get();
    }

    /**
     * Gets the rotation of this point in degrees.
     * 
     * @return The rotation in degrees.
     */
    public double getRotationDegrees() {
        return this.rotation.get();
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
    public SimpleDoubleProperty getXProp() {
        return x;
    }

    /**
     * Gets the y-coordinate property of this point.
     * 
     * @return The y-coordinate property.
     */
    public SimpleDoubleProperty getYProp() {
        return y;
    }

    /**
     * Gets the rotation property of this point.
     * 
     * @return The rotation property.
     */
    public SimpleDoubleProperty getRotationProp() {
        return rotation;
    }

    // /**
    // * Gets the x-coordinate property of this point.
    // *
    // * @return The x-coordinate property.
    // */
    // public SimpleStringProperty getXStringProp() {
    // return xStringProp;
    // }

    // /**
    // * Gets the y-coordinate property of this point.
    // *
    // * @return The y-coordinate property.
    // */
    // public SimpleStringProperty getYStringProp() {
    // return yStringProp;
    // }

    // /**
    // * Gets the rotation property of this point.
    // *
    // * @return The rotation property.
    // */
    // public SimpleStringProperty getRotationStringProp() {
    // return rotationStringProp;
    // }

}