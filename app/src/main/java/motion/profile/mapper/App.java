package motion.profile.mapper;

import javafx.application.Application;
import javafx.application.Platform;
import javafx.fxml.FXMLLoader;
import javafx.scene.Scene;
import javafx.stage.Stage;

public class App extends Application {

    public static Stage primaryStage;

    @Override
    public void start(Stage stage) throws Exception {
        primaryStage = stage;
        FXMLLoader loader = new FXMLLoader(getClass().getResource("/layout.fxml"));
        Scene scene = new Scene(loader.load());
        stage.setTitle("Motion Profile Mapper");
        stage.setScene(scene);

        // TODO: see if this is necessary once we build

        // Temporarily set the stage to always be on top
        stage.setAlwaysOnTop(true);
        stage.show();

        // Bring the stage to the front after it has been shown and then set always on
        // top to false
        Platform.runLater(() -> {
            stage.toFront();
            stage.setAlwaysOnTop(false);
        });

    }

    public static void main(String[] args) {
        launch();
    }
}
