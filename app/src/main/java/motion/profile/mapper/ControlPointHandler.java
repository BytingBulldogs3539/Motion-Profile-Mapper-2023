package motion.profile.mapper;

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

public class ControlPointHandler extends ControlPoint {
    private final SimpleDoubleProperty xProp = new SimpleDoubleProperty();
    private final SimpleDoubleProperty yProp = new SimpleDoubleProperty();
    private final SimpleDoubleProperty rotationProp = new SimpleDoubleProperty(); // Stored in degrees
    private final PathHandler pathHandler;
    private final XYChart.Data<Number, Number> dataPoint = new XYChart.Data<>(getX(), getY());

    /**
     * Creates a new control point with the given translation and rotation.
     * 
     * @param translation The translation of the control point.
     * @param rotation    The rotation of the control point.
     * @param path        The path to which this control point belongs.
     */
    public ControlPointHandler(Translation2d translation, Rotation2d rotation, PathHandler pathHandler) {
        super(pathHandler);
        this.pathHandler = pathHandler;
        setX(translation.getX());
        setY(translation.getY());
        setRotation(rotation.getDegrees());
        this.dataPoint.XValueProperty().bind(xProp);
        this.dataPoint.YValueProperty().bind(yProp);
        pathHandler.addControlPoint(this);
        addHandlers();
        addContextMenu();
    }
    /**
     * Creates a new control point with the given translation and rotation.
     * 
     * @param x The x-coordinate of the control point.
     * @param y The y-coordinate of the control point.
     * @param rotation The rotation of the control point in degrees.
     * @param pathHandler The path to which this control point belongs.
     */
    public ControlPointHandler(double x, double y, double rotation, PathHandler pathHandler) {
        this(new Translation2d(x,y), Rotation2d.fromDegrees(rotation), pathHandler);
    }

    /**
     * Sets the x-coordinate of this point.
     * 
     * @param x The new x-coordinate.
     */
    public void setX(double x) {
        if(x!=getX()) {
            super.setX(x);
            this.xProp.set(x);
            updateModifiedTime();
        }
    }

    /**
     * Sets the y-coordinate of this point.
     * 
     * @param y The new y-coordinate.
     */
    public void setY(double y) {
        if(y!=getY()) {
            super.setY(y);
            this.yProp.set(y);
            updateModifiedTime();
        }
    }

    /**
     * Sets the x and y coordinates of this point.
     * 
     * @param x The new x-coordinate.
     * @param y The new y-coordinate.
     */
    public void setXY(double x, double y) {
        setX(x);
        setY(y);
    }

    /**
     * Sets the rotation of this point.
     * 
     * @param translation The new rotation in degrees.
     */
    public void setRotation(double rotation) {
        if(rotation!=getRotationDegrees()) {
            super.setRotationDegrees(rotation);
            this.rotationProp.set(rotation);
            updateModifiedTime();
        }
    }
    public void setRotation(Rotation2d rotation) {
        setRotation(rotation.getDegrees());
    }

    public PathHandler getPathHandler() {
        return pathHandler;
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
            setXY(boundedX, boundedY);
            // Update the corresponding table entry
            App.controller.selectAndScrollTo(this);

        });

        getDataPoint().nodeProperty().addListener((observable, oldValue, newValue) -> {
            if (newValue != null) {
                newValue.setViewOrder(10);
            }
        });
    }

    public void setTranslation(Translation2d translation) {
        setX(translation.getX());
        setY(translation.getY());
    }


    private void addContextMenu() {
        Node node = getDataPoint().getNode();
        ContextMenu contextMenu = new ContextMenu();
        MenuItem deleteItem = new MenuItem("Delete");

        deleteItem.setOnAction(event -> {
            pathHandler.removePoint(this);
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
        pathHandler.updateModifiedTime();
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
        return xProp;
    }

    /**
     * Gets the y-coordinate property of this point.
     * 
     * @return The y-coordinate property.
     */
    public SimpleDoubleProperty getYProp() {
        return yProp;
    }

    /**
     * Gets the rotation property of this point.
     * 
     * @return The rotation property.
     */
    public SimpleDoubleProperty getRotationProp() {
        return rotationProp;
    }

}
