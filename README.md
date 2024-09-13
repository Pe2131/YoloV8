# Real-Time Object Detection with YOLOv8 in C# WinForms

This project is a C# WinForms application that demonstrates real-time object detection using a webcam feed. The object detection is powered by the YOLOv8 model (You Only Look Once), implemented via the `YoloDotNet` library, with image processing performed using `Emgu.CV`, `SkiaSharp`, and `SixLabors.ImageSharp`.

## Features

- **Real-time Object Detection**: Utilizes the YOLOv8 model to detect objects in real-time via a connected webcam.
- **Flexible Processing**: Supports both CPU and GPU processing.
- **User-Friendly Interface**: Simple start/stop functionality to control the webcam feed and object detection.
- **Visual Feedback**: Detected objects are drawn on the webcam feed with bounding boxes and labels.

## Getting Started

### Prerequisites

To run this project, you need to have the following installed:

- [.NET Framework](https://dotnet.microsoft.com/) (version compatible with your project setup)
- [YOLOv8 ONNX Model](https://github.com/ultralytics/yolov5/releases) - Ensure you have a valid ONNX model for YOLOv8.
- [Emgu.CV](https://www.emgu.com/wiki/index.php/Main_Page) - For capturing and processing webcam frames.
- [SkiaSharp](https://github.com/mono/SkiaSharp) - For efficient image manipulation and rendering.
- [SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp) - For handling image formats and transformations.
- [YoloDotNet](https://github.com/alexeygrigorev/YoloDotNet) - Library for object detection with YOLO models.

### Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/yourusername/YoloV8-RealTime-ObjectDetection.git
   cd YoloV8-RealTime-ObjectDetection

2. **Restore NuGet Packages:**:
Open the project in Visual Studio or any other IDE that supports .NET development. Restore the NuGet packages to ensure all dependencies are installed.

3. **Download YOLOv8 ONNX Model**: Download the YOLOv8 ONNX model file and place it in a directory within your project. Update the model path in the Form1_Load method:
```bash
OnnxModel = @"F:\c#project\YoloV5\YoloObjectDetection\Yolo\yolov8s.onnx";
```
Adjust the path based on where you save the model.

4. **Run the Application**: Build and run the application. Click the "Start" button to begin the webcam feed and start detecting objects in real-time.