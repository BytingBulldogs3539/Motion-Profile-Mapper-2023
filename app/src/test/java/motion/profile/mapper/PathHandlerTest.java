package motion.profile.mapper;

import static org.junit.jupiter.api.Assertions.*;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

public class PathHandlerTest {

    private PathHandler pathHandler;

    @BeforeEach
    public void setUp() {
        pathHandler = new PathHandler("TestPath", true);
    }

    @Test
    public void testConstructor() {
        assertEquals("TestPath", pathHandler.getName());
        assertTrue(pathHandler.isSpline());
        assertNotNull(pathHandler.getSplineControlPoints());
        assertNotNull(pathHandler.getChartData());
        assertNotNull(pathHandler.getSplineChartData());
        assertNotNull(pathHandler.getModifiedTimeProp());
    }

    @Test
    public void testAddControlPoint() {
        ControlPointHandler point = new ControlPointHandler(1, 2, 3, pathHandler);
        assertTrue(pathHandler.getSplineControlPoints().contains(point));
        assertTrue(pathHandler.getChartData().contains(point.getDataPoint()));
    }

    @Test
    public void testRemovePointByIndex() {
        ControlPointHandler point = new ControlPointHandler(1, 2, 3, pathHandler);
        pathHandler.removePoint(0);
        assertFalse(pathHandler.getSplineControlPoints().contains(point));
        assertFalse(pathHandler.getChartData().contains(point.getDataPoint()));
    }

    @Test
    public void testRemovePointByObject() {
        ControlPointHandler point = new ControlPointHandler(1, 2, 3, pathHandler);
        pathHandler.removePoint(point);
        assertFalse(pathHandler.getSplineControlPoints().contains(point));
        assertFalse(pathHandler.getChartData().contains(point.getDataPoint()));
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
        assertTrue(pathHandler.getChartData().isEmpty());
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
}