package motion.profile.mapper;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import com.fasterxml.jackson.databind.ObjectMapper;

import edu.wpi.first.math.geometry.Pose2d;
import edu.wpi.first.math.geometry.Rotation2d;
import edu.wpi.first.math.geometry.Translation2d;
import javafx.beans.property.SimpleDoubleProperty;
import javafx.scene.chart.XYChart;

public class ControlPointHandlerTest {

    private PathHandler mockPathHandler;
    private ControlPointHandler controlPointHandler;

    @BeforeEach
    public void setUp() {
        mockPathHandler = mock(PathHandler.class);
        controlPointHandler = new ControlPointHandler(new Translation2d(1.0, 2.0), Rotation2d.fromDegrees(45.0), mockPathHandler);
    }


    // Constructor Tests
    @Test
    public void testConstructorWithTranslationAndRotation() {
        mockPathHandler.addControlPoint(controlPointHandler);
        assertEquals(1.0, controlPointHandler.getX());
        assertEquals(2.0, controlPointHandler.getY());
        assertEquals(45.0, controlPointHandler.getRotationDegrees(), 1e-9);
        verify(mockPathHandler).addControlPoint(controlPointHandler);
    }

    @Test
    public void testConstructorWithCoordinates() {
        ControlPointHandler handler = new ControlPointHandler(3.0, 4.0, 90.0, mockPathHandler);
        mockPathHandler.addControlPoint(handler);
        assertEquals(3.0, handler.getX());
        assertEquals(4.0, handler.getY());
        assertEquals(90.0, handler.getRotationDegrees(), 1e-9);
        verify(mockPathHandler).addControlPoint(handler);
    }

    // Setter Tests
    @Test
    public void testSetX() {
        controlPointHandler.setX(5.0);
        assertEquals(5.0, controlPointHandler.getX());
        assertEquals(5.0, controlPointHandler.getXProp().get());
        verify(mockPathHandler).updateModifiedTime();
    }

    @Test
    public void testSetY() {
        controlPointHandler.setY(6.0);
        assertEquals(6.0, controlPointHandler.getY());
        assertEquals(6.0, controlPointHandler.getYProp().get());
        verify(mockPathHandler).updateModifiedTime();
    }

    @Test
    public void testSetXY() {
        controlPointHandler.setXY(7.0, 8.0);
        assertEquals(7.0, controlPointHandler.getX());
        assertEquals(8.0, controlPointHandler.getY());
        verify(mockPathHandler, times(2)).updateModifiedTime();
    }

    @Test
    public void testSetRotationDegrees() {
        controlPointHandler.setRotationDegrees(30.0);
        assertEquals(30.0, controlPointHandler.getRotationDegrees(), 1e-9);
        assertEquals(30.0, controlPointHandler.getRotationProp().get(), 1e-9);
        verify(mockPathHandler).updateModifiedTime();
    }

    @Test
    public void testSetRotation2d() {
        controlPointHandler.setRotation(Rotation2d.fromDegrees(60.0));
        assertEquals(60.0, controlPointHandler.getRotationDegrees(), 1e-9);
        assertEquals(60.0, controlPointHandler.getRotationProp().get(), 1e-9);
        verify(mockPathHandler).updateModifiedTime();
    }

    @Test
    public void testSetTranslation() {
        controlPointHandler.setTranslation(new Translation2d(9.0, 10.0));
        assertEquals(9.0, controlPointHandler.getX());
        assertEquals(10.0, controlPointHandler.getY());
        verify(mockPathHandler, times(2)).updateModifiedTime();
    }

    @Test
    public void testSetPose() {
        controlPointHandler.setPose(new Pose2d(new Translation2d(10.0, 11.0), Rotation2d.fromDegrees(66.0)));
        assertEquals(10.0, controlPointHandler.getX());
        assertEquals(11.0, controlPointHandler.getY());
        assertEquals(66.0, controlPointHandler.getRotationDegrees(), 1e-9);
        verify(mockPathHandler, times(3)).updateModifiedTime();
    }


    @Test
    public void testForceSetX() {
        controlPointHandler.forceSetX(11.0);
        assertEquals(11.0, controlPointHandler.getX());
        assertEquals(11.0, controlPointHandler.getXProp().get());
    }

    @Test
    public void testForceSetY() {
        controlPointHandler.forceSetY(12.0);
        assertEquals(12.0, controlPointHandler.getY());
        assertEquals(12.0, controlPointHandler.getYProp().get());
    }

    @Test
    public void testForceSetRotation() {
        controlPointHandler.forceSetRotation(75.0);
        assertEquals(75.0, controlPointHandler.getRotationDegrees(), 1e-9);
        assertEquals(75.0, controlPointHandler.getRotationProp().get(), 1e-9);
    }

    // Getter Tests
    @Test
    public void testGetDataPoint() {
        XYChart.Data<Number, Number> dataPoint = controlPointHandler.getDataPoint();
        assertEquals(controlPointHandler.getX(), dataPoint.getXValue());
        assertEquals(controlPointHandler.getY(), dataPoint.getYValue());
    }

    @Test
    public void testGetXProp() {
        SimpleDoubleProperty xProp = controlPointHandler.getXProp();
        assertEquals(controlPointHandler.getX(), xProp.get());
    }

    @Test
    public void testGetYProp() {
        SimpleDoubleProperty yProp = controlPointHandler.getYProp();
        assertEquals(controlPointHandler.getY(), yProp.get());
    }

    @Test
    public void testGetRotationProp() {
        SimpleDoubleProperty rotationProp = controlPointHandler.getRotationProp();
        assertEquals(controlPointHandler.getRotationDegrees(), rotationProp.get());
    }

    @Test
    public void testGetPathHandler() {
        assertEquals(mockPathHandler, controlPointHandler.getPathHandler());
    }
    // JSON Serialization/Deserialization Test
    @Test
    public void testControlPointJsonSerialization() throws Exception {
        ObjectMapper mapper = new ObjectMapper();

        // Create an instance of ControlPoint
        Path path = new Path("examplePath", Path.PathType.CUBIC);
        ControlPoint controlPoint = new ControlPoint(new Translation2d(1.0, 2.0), Rotation2d.fromDegrees(45.0), path);

        // Serialize to JSON
        String jsonString = mapper.writeValueAsString(controlPoint);
        System.out.println("Serialized JSON: " + jsonString);

        // Deserialize from JSON
        ControlPoint deserializedControlPoint = mapper.readValue(jsonString, ControlPoint.class);
        assertEquals(1.0, deserializedControlPoint.getX());
        assertEquals(2.0, deserializedControlPoint.getY());
        assertEquals(45.0, deserializedControlPoint.getRotationDegrees(), 1e-9);
    }
    // JSON Serialization/Deserialization Test
    @Test
    public void testControlPointHandlerJsonSerialization() throws Exception {
        ObjectMapper mapper = new ObjectMapper();

        // Serialize to JSON
        String jsonString = mapper.writeValueAsString(controlPointHandler);
        System.out.println("Serialized JSON: " + jsonString);

        // Deserialize from JSON
        ControlPoint deserializedHandler = mapper.readValue(jsonString, ControlPoint.class);
        assertEquals(controlPointHandler.getX(), deserializedHandler.getX());
        assertEquals(controlPointHandler.getY(), deserializedHandler.getY());
        assertEquals(controlPointHandler.getRotationDegrees(), deserializedHandler.getRotationDegrees(), 1e-9);
    }
}