package motion.profile.mapper;

import com.fasterxml.jackson.annotation.JsonIgnore;

import edu.wpi.first.math.geometry.Pose2d;
import edu.wpi.first.math.geometry.Rotation2d;
import edu.wpi.first.math.geometry.Translation2d;
import javafx.animation.PauseTransition;
import javafx.beans.property.SimpleDoubleProperty;
import javafx.beans.value.WeakChangeListener;
import javafx.scene.Cursor;
import javafx.scene.Node;
import javafx.scene.chart.XYChart;
import javafx.scene.control.ContextMenu;
import javafx.scene.control.MenuItem;
import javafx.util.Duration;

public class ControlPointHandler extends ControlPoint {
    @JsonIgnore
    private final SimpleDoubleProperty xProp = new SimpleDoubleProperty();
    @JsonIgnore
    private final SimpleDoubleProperty yProp = new SimpleDoubleProperty();
    @JsonIgnore
    private final SimpleDoubleProperty rotationProp = new SimpleDoubleProperty(); // Stored in degrees
    @JsonIgnore
    private final PathHandler pathHandler;
    @JsonIgnore
    private final XYChart.Data<Number, Number> dataPoint = new XYChart.Data<>(getX(), getY());

    /**
     * Creates a new control point with the given translation and rotation.
     * 
     * @param translation The translation of the control point.
     * @param rotation    The rotation of the control point.
     * @param pathHandler The path to which this control point belongs.
     */
    public ControlPointHandler(Translation2d translation, Rotation2d rotation, PathHandler pathHandler) {
        super(pathHandler);
        this.pathHandler = pathHandler;
        initializeNodePropertyListener();
        forceSetX(translation.getX());
        forceSetY(translation.getY());
        forceSetRotation(rotation.getDegrees());
        bindDataPointProperties();
    }

    /**
     * Creates a new control point with the given translation and rotation.
     * 
     * @param x           The x-coordinate of the control point.
     * @param y           The y-coordinate of the control point.
     * @param rotation    The rotation of the control point in degrees.
     * @param pathHandler The path to which this control point belongs.
     */
    public ControlPointHandler(double x, double y, double rotation, PathHandler pathHandler) {
        this(new Translation2d(x, y), Rotation2d.fromDegrees(rotation), pathHandler);
    }

    public ControlPointHandler(ControlPoint controlPoint, PathHandler pathHandler) {
        this(controlPoint.getTranslation(), controlPoint.getRotation(), pathHandler);
    }

    // Getters
    @JsonIgnore
    public XYChart.Data<Number, Number> getDataPoint() {
        return dataPoint;
    }

    @JsonIgnore
    public SimpleDoubleProperty getXProp() {
        return xProp;
    }

    @JsonIgnore
    public SimpleDoubleProperty getYProp() {
        return yProp;
    }

    @JsonIgnore
    public SimpleDoubleProperty getRotationProp() {
        return rotationProp;
    }

    @JsonIgnore
    public PathHandler getPathHandler() {
        return pathHandler;
    }

    // Setters
    @Override
    public void setX(double x) {
        if (x != getX()) {
            super.setX(x);
            xProp.set(x);
            updateModifiedTime();
        }
    }

    @Override
    public void setY(double y) {
        if (y != getY()) {
            super.setY(y);
            yProp.set(y);
            updateModifiedTime();
        }
    }

    @Override
    public void setXY(double x, double y) {
        setX(x);
        setY(y);
    }

    @Override
    public void setRotationDegrees(double rotation) {
        if (rotation != getRotationDegrees()) {
            super.setRotationDegrees(rotation);
            rotationProp.set(rotation);
            updateModifiedTime();
        }
    }

    @Override
    public void setRotation(Rotation2d rotation) {
        setRotationDegrees(rotation.getDegrees());
    }

    @Override
    public void setTranslation(Translation2d translation) {
        setX(translation.getX());
        setY(translation.getY());
    }

    @Override
    public void setPose(Pose2d pose) {
        setTranslation(pose.getTranslation());
        setRotation(pose.getRotation());
    }

    /*
     * Forces the x-coordinate of this control point to be the given value. It does
     * not update the modified time of the path.
     */
    public void forceSetX(double x) {
        super.setX(x);
        xProp.set(x);
    }

    /*
     * Forces the y-coordinate of this control point to be the given value. It does
     * not update the modified time of the path.
     */
    public void forceSetY(double y) {
        super.setY(y);
        yProp.set(y);
    }

    /*
     * Forces the rotation of this control point to be the given value. It does not
     * update the modified time of the path.
     */
    public void forceSetRotation(double rotation) {
        super.setRotationDegrees(rotation);
        rotationProp.set(rotation);
    }

    // Private Methods
    private void initializeNodePropertyListener() {
        dataPoint.nodeProperty().addListener(new WeakChangeListener<>((observable, oldValue, newValue) -> {
            if (newValue != null) {
                addHandlers();
                addContextMenu();
            }
        }));
    }

    private void bindDataPointProperties() {
        dataPoint.XValueProperty().bind(xProp);
        dataPoint.YValueProperty().bind(yProp);
    }

    private void addHandlers() {
        Node node = getDataPoint().getNode();
        node.setCursor(Cursor.HAND);

        node.setOnMousePressed(event -> handleMousePressed(node, event));
        node.setOnMouseClicked(event -> handleMouseClicked(event));
        node.setOnMouseReleased(event -> handleMouseReleased(node));
        node.setOnMouseDragged(event -> handleMouseDragged(event));
        node.setStyle("-fx-background-color: blue;"); // Reset to default style
        node.toFront();
    }

    private void handleMousePressed(Node node, javafx.scene.input.MouseEvent event) {
        node.setCursor(Cursor.MOVE);
        App.controller.setDragging(true); // Set dragging flag to true
        event.consume();
    }

    private void handleMouseClicked(javafx.scene.input.MouseEvent event) {
        App.controller.selectAndScrollTo(this);
        event.consume();
    }

    private void handleMouseReleased(Node node) {
        node.setCursor(Cursor.HAND);
        PauseTransition pause = new PauseTransition(Duration.millis(20)); // Adds 20ms delay to prevent accidental
                                                                          // double clicks
        pause.setOnFinished(e -> App.controller.setDragging(false));
        pause.play();
    }

    private void handleMouseDragged(javafx.scene.input.MouseEvent event) {
        Translation2d newTranslation = getBoundedTranslation(event);
        setXY(newTranslation.getX(), newTranslation.getY());
        App.controller.selectAndScrollTo(this);
    }

    private Translation2d getBoundedTranslation(javafx.scene.input.MouseEvent event) {
        double xLowerBound = App.controller.getXAxis().getLowerBound();
        double xUpperBound = App.controller.getXAxis().getUpperBound();
        double yLowerBound = App.controller.getYAxis().getLowerBound();
        double yUpperBound = App.controller.getYAxis().getUpperBound();

        Translation2d newTranslation = App.controller.getChartMouseClickPosition(event);

        double boundedX = Math.max(xLowerBound, Math.min(newTranslation.getX(), xUpperBound));
        double boundedY = Math.max(yLowerBound, Math.min(newTranslation.getY(), yUpperBound));

        boundedX = Math.round(boundedX * 1000.0) / 1000.0;
        boundedY = Math.round(boundedY * 1000.0) / 1000.0;

        return new Translation2d(boundedX, boundedY);
    }

    private void addContextMenu() {
        Node node = getDataPoint().getNode();
        ContextMenu contextMenu = createContextMenu();
        node.setOnContextMenuRequested(event -> contextMenu.show(node, event.getScreenX(), event.getScreenY()));
    }

    private ContextMenu createContextMenu() {
        ContextMenu contextMenu = new ContextMenu();
        MenuItem deleteItem = createDeleteMenuItem();
        contextMenu.getItems().add(deleteItem);
        return contextMenu;
    }

    private MenuItem createDeleteMenuItem() {
        MenuItem deleteItem = new MenuItem("Delete");
        deleteItem.setOnAction(event -> pathHandler.removePoint(this));
        return deleteItem;
    }

    private void updateModifiedTime() {
        pathHandler.updateModifiedTime();
    }
}