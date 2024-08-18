package motion.profile.mapper;

import javafx.scene.chart.XYChart;

// Enum to represent the type of change in the table
enum RedoUndoChangeType {
    ADD, REMOVE, UPDATE
}

public class RedoUndoChange {
    private final RedoUndoChangeType type;
    private final XYChart.Data<Number, Number> data;

    public RedoUndoChange(RedoUndoChangeType type, XYChart.Data<Number, Number> data) {
        this.type = type;
        this.data = data;
    }

    public RedoUndoChangeType getType() {
        return type;
    }

    public XYChart.Data<Number, Number> getData() {
        return data;
    }
}
