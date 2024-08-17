package motion.profile.mapper;

import atlantafx.base.theme.PrimerDark;
import javafx.application.Application;
import javafx.scene.Cursor;
import javafx.scene.Node;
import javafx.scene.Scene;
import javafx.scene.chart.LineChart;
import javafx.scene.chart.NumberAxis;
import javafx.scene.chart.XYChart;
import javafx.scene.control.Button;
import javafx.scene.control.ListView;
import javafx.scene.image.Image;
import javafx.scene.image.ImageView;
import javafx.scene.input.MouseEvent;
import javafx.scene.layout.BorderPane;
import javafx.scene.layout.StackPane;
import javafx.scene.layout.VBox;
import javafx.stage.Stage;

public class App extends Application {

    int i = 3;
    ListView<String> listView = new ListView<>();

    // Create a LineChart
    NumberAxis xAxis = new NumberAxis();
    NumberAxis yAxis = new NumberAxis();
    LineChart<Number, Number> lineChart = new LineChart<>(xAxis, yAxis);

    // Load the image from resources
    Image image = new Image(getClass().getResourceAsStream("/2024-Field-Full.png"));
    ImageView imageView = new ImageView(image);

    // Use a StackPane to layer the image and the chart
    StackPane chartPane = new StackPane(imageView, lineChart);

    BorderPane borderPane = new BorderPane();

    @Override
    public void start(Stage stage) {
        listView.getItems().add("Item 1");
        listView.getItems().add("Item 2");
        listView.getItems().add("Item 3");

        Button clearButton = new Button("Clear");
        Button button2 = new Button("Button 2");

        VBox buttonBox = new VBox(clearButton, button2);
        buttonBox.setSpacing(10); // Optional: Add some spacing between buttons

        lineChart.setTitle("Sample Chart");

        XYChart.Series<Number, Number> series = new XYChart.Series<>();
        series.setName("Data Series");
        series.getData().add(new XYChart.Data<>(1, 23));
        series.getData().add(new XYChart.Data<>(2, 14));
        series.getData().add(new XYChart.Data<>(3, 15));
        series.getData().add(new XYChart.Data<>(4, 24));
        series.getData().add(new XYChart.Data<>(5, 34));
        series.getData().add(new XYChart.Data<>(6, 36));
        series.getData().add(new XYChart.Data<>(7, 22));
        series.getData().add(new XYChart.Data<>(8, 45));
        series.getData().add(new XYChart.Data<>(9, 43));
        series.getData().add(new XYChart.Data<>(10, 17));
        lineChart.getData().add(series);

        clearButton.setOnMouseClicked((MouseEvent event) -> {
            series.getData().clear();
        });
        button2.setOnMouseClicked((MouseEvent event) -> {
            i++;
            listView.getItems().add("New Item " + i);
        });

        // Add mouse click event handler to the chart
        lineChart.setOnMouseClicked((MouseEvent event) -> {
            double xValue = xAxis.getValueForDisplay(lineChart.getXAxis().sceneToLocal(event.getSceneX(), event.getSceneY()).getX()).doubleValue();
            double yValue = yAxis.getValueForDisplay(lineChart.getYAxis().sceneToLocal(event.getSceneX(), event.getSceneY()).getY()).doubleValue();
            XYChart.Data<Number, Number> dataPoint = new XYChart.Data<>(xValue, yValue);
            //series.getData().add(dataPoint);
            // Add drag functionality to the new data point
            addDragHandlers(dataPoint);
        });

        imageView.setFitWidth(400); // Adjust the width as needed
        imageView.setFitHeight(300); // Adjust the height as needed
        imageView.setPreserveRatio(true);

        borderPane.setLeft(listView);
        borderPane.setBottom(buttonBox);
        borderPane.setRight(chartPane);
        borderPane.setStyle("-fx-border-color: black; -fx-border-width: 2;");

        Scene scene = new Scene(new StackPane(borderPane), 800, 600);
        stage.setScene(scene);
        stage.show();
    }

    private void addDragHandlers(XYChart.Data<Number, Number> dataPoint) {
        Node node = dataPoint.getNode();

        node.setOnMousePressed(event -> {
            node.setCursor(Cursor.MOVE);
        });

        node.setOnMouseReleased(event -> {
            node.setCursor(Cursor.HAND);
        });

        node.setOnMouseDragged(event -> {
            double xValue = xAxis.getValueForDisplay(lineChart.getXAxis().sceneToLocal(event.getSceneX(), event.getSceneY()).getX()).doubleValue();
            double yValue = yAxis.getValueForDisplay(lineChart.getYAxis().sceneToLocal(event.getSceneX(), event.getSceneY()).getY()).doubleValue();
            dataPoint.setXValue(xValue);
            dataPoint.setYValue(yValue);
        });

        node.setCursor(Cursor.HAND);
    }

    public static void main(String[] args) {
        Application.setUserAgentStylesheet(new PrimerDark().getUserAgentStylesheet());
        launch();
    }
}
