package motion.profile.mapper;

import javafx.collections.ObservableList;

// Enum to represent the type of change in the table
enum RedoUndoChangeType {
    ADD, REMOVE, UPDATE
}

public class RedoUndoChange {
    private final RedoUndoChangeType type;
    private final ObservableList<PathHandler> paths;

    public RedoUndoChange(RedoUndoChangeType type, ObservableList<PathHandler> paths) {
        this.type = type;
        this.paths = paths;
    }

    public RedoUndoChangeType getType() {
        return type;
    }

    public ObservableList<PathHandler> getPaths() {
        return paths;
    }
}
