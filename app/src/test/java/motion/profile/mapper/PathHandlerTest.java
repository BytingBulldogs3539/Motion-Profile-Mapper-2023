package motion.profile.mapper;

import static org.junit.jupiter.api.Assertions.*;

import java.util.Arrays;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import com.fasterxml.jackson.databind.ObjectMapper;

import edu.wpi.first.math.geometry.Rotation2d;
import edu.wpi.first.math.geometry.Translation2d;
import motion.profile.mapper.Path.PathType;

public class PathHandlerTest {

    private PathHandler pathHandler;

    @BeforeEach
    public void setUp() {
        pathHandler = new PathHandler("TestPath", PathType.CUBIC);
    }

    @Test
    public void testConstructor() {
        assertEquals("TestPath", pathHandler.getName());
        assertTrue(pathHandler.isSpline());
        assertNotNull(pathHandler.getSplineControlPoints());
        assertNotNull(pathHandler.getControlPointData());
        assertNotNull(pathHandler.getSplineChartData());
        assertNotNull(pathHandler.getModifiedTimeProp());
    }

    @Test
    public void testAddControlPoint() {
        ControlPointHandler point = new ControlPointHandler(1, 2, 3, pathHandler);
        pathHandler.addControlPoint(point);
        assertTrue(pathHandler.getSplineControlPoints().contains(point));
        assertTrue(pathHandler.getControlPointData().contains(point.getDataPoint()));
    }

    @Test
    public void testRemovePointByIndex() {
        ControlPointHandler point = new ControlPointHandler(1, 2, 3, pathHandler);
        pathHandler.removePoint(0);
        assertFalse(pathHandler.getSplineControlPoints().contains(point));
        assertFalse(pathHandler.getControlPointData().contains(point.getDataPoint()));
    }

    @Test
    public void testRemovePointByObject() {
        ControlPointHandler point = new ControlPointHandler(1, 2, 3, pathHandler);
        pathHandler.removePoint(point);
        assertFalse(pathHandler.getSplineControlPoints().contains(point));
        assertFalse(pathHandler.getControlPointData().contains(point.getDataPoint()));
    }

    @Test
    public void testSetSplineMode() {
        pathHandler.setSplineMode(false);
        assertFalse(pathHandler.isSpline());
        pathHandler.setSplineMode(true);
        assertTrue(pathHandler.isSpline());
    }

    @Test
    public void testClearPoints() {
        ControlPointHandler point = new ControlPointHandler(1, 2, 3, pathHandler);
        pathHandler.addControlPoint(point);
        pathHandler.clearPoints();
        assertTrue(pathHandler.getSplineControlPoints().isEmpty());
        assertTrue(pathHandler.getControlPointData().isEmpty());
    }

    @Test
    public void testSetName() {
        pathHandler.setName("NewName");
        assertEquals("NewName", pathHandler.getName());
        assertEquals("NewName", pathHandler.getNameProp().get());
    }

    @Test
    public void testUpdateModifiedTime() {
        String oldTime = pathHandler.getModifiedTimeProp().get();
        pathHandler.updateModifiedTime();
        String newTime = pathHandler.getModifiedTimeProp().get();
        assertNotEquals(oldTime, newTime);
    }

        // JSON Serialization/Deserialization Test
    @Test
    public void testPathJsonSerialization() throws Exception {
        ObjectMapper mapper = new ObjectMapper();

        // Create an instance of Path
        ControlPoint point1 = new ControlPoint(new Translation2d(1.0, 2.0), Rotation2d.fromDegrees(45.0), null);
        ControlPoint point2 = new ControlPoint(new Translation2d(3.0, 4.0), Rotation2d.fromDegrees(90.0), null);
        Path path = new Path(Arrays.asList(point1, point2), "TestPath", Path.PathType.CUBIC);
        path.setModifiedTime("2021-01-01T00:00:00");

        // Serialize to JSON
        String jsonString = mapper.writeValueAsString(path);
        System.out.println("Serialized JSON: " + jsonString);

        // Deserialize from JSON
        Path deserializedPath = mapper.readValue(jsonString, Path.class);
        assertEquals("TestPath", deserializedPath.getName());
        assertEquals(Path.PathType.CUBIC, deserializedPath.getType());
        assertEquals(2, deserializedPath.getControlPoints().size());
        assertEquals(1.0, deserializedPath.getControlPoints().get(0).getX());
        assertEquals(2.0, deserializedPath.getControlPoints().get(0).getY());
        assertEquals(45.0, deserializedPath.getControlPoints().get(0).getRotationDegrees(), 1e-9);
        assertEquals(3.0, deserializedPath.getControlPoints().get(1).getX());
        assertEquals(4.0, deserializedPath.getControlPoints().get(1).getY());
        assertEquals(90.0, deserializedPath.getControlPoints().get(1).getRotationDegrees(), 1e-9);
        assertEquals("2021-01-01T00:00:00", deserializedPath.getModifiedTime());
    }

    @Test
    public void testPathHandlerJsonSerialization() throws Exception {
        ObjectMapper mapper = new ObjectMapper();

        PathHandler path = new PathHandler("TestPath", PathType.CUBIC);

        // Create an instance of Path
        ControlPointHandler point1 = new ControlPointHandler(new Translation2d(1.0, 2.0), Rotation2d.fromDegrees(45.0), path);
        ControlPointHandler point2 = new ControlPointHandler(new Translation2d(3.0, 4.0), Rotation2d.fromDegrees(90.0), path);
        path.addControlPoint(point1);
        path.addControlPoint(point2);

        // Serialize to JSON
        String jsonString = mapper.writeValueAsString(path);
        System.out.println("Serialized JSON: " + jsonString);

        // Deserialize from JSON
        Path deserializedPath = mapper.readValue(jsonString, Path.class);
        assertEquals("TestPath", deserializedPath.getName());
        assertEquals(Path.PathType.CUBIC, deserializedPath.getType());
        assertEquals(path.getControlPoints().size(), deserializedPath.getControlPoints().size());
        assertEquals(path.getControlPoints().get(0).getX(), deserializedPath.getControlPoints().get(0).getX());
        assertEquals(path.getControlPoints().get(0).getY(), deserializedPath.getControlPoints().get(0).getY());
        assertEquals(path.getControlPoints().get(0).getRotationDegrees(), deserializedPath.getControlPoints().get(0).getRotationDegrees(), 1e-9);
        assertEquals(path.getControlPoints().get(1).getX(), deserializedPath.getControlPoints().get(1).getX());
        assertEquals(path.getControlPoints().get(1).getY(), deserializedPath.getControlPoints().get(1).getY());
        assertEquals(path.getControlPoints().get(1).getRotationDegrees(), deserializedPath.getControlPoints().get(1).getRotationDegrees(), 1e-9);
        assertEquals(path.getModifiedTime(), deserializedPath.getModifiedTime());
    }
}