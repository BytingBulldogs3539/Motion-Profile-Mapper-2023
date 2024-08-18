package motion.profile.mapper;

import javafx.collections.ObservableList;

// Enum to represent the type of change in the table
enum RedoUndoChangeType {
    ADD, REMOVE, UPDATE
}

public class RedoUndoChange {
    private final RedoUndoChangeType type;
    private final ObservableList<Path> paths;

    public RedoUndoChange(RedoUndoChangeType type, ObservableList<Path> paths) {
        this.type = type;
        this.paths = paths;
    }

    public RedoUndoChangeType getType() {
        return type;
    }

    public ObservableList<Path> getPaths() {
        return paths;
    }
}
