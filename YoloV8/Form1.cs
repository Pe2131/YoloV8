using Emgu.CV;
using Emgu.CV.Structure;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoloDotNet;
using YoloDotNet.Enums;
using YoloDotNet.Extensions;
using YoloDotNet.Models;

namespace YoloV8
{
    public partial class Form1 : Form
    {
        private Yolo Yolo; // Declare Yolo at class level for object detection
        private CancellationTokenSource cancellationTokenSource; // For handling cancellation of async tasks
        private CancellationToken cancellationToken; // Token for cancelling the webcam feed

        public Form1()
        {
            InitializeComponent(); // Initialize UI components
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Instantiate a new Yolo object with specified options
            Yolo = new Yolo(new YoloOptions
            {
                OnnxModel = @"F:\c#project\YoloV5\YoloObjectDetection\Yolo\yolov8s.onnx", // Path to YOLOv8 ONNX model
                ModelType = ModelType.ObjectDetection, // Specify model type
                Cuda = false, // Set to true for GPU acceleration if available
                GpuId = 0, // GPU ID for CUDA (default is 0)
                PrimeGpu = false, // Pre-allocate GPU memory before first use
            });
        }

        private async Task WebCamAsync(CancellationToken cancellationToken)
        {
            using var capture = new VideoCapture(0, VideoCapture.API.DShow); // Initialize webcam capture
            capture.Set(Emgu.CV.CvEnum.CapProp.FrameCount, 30); // Set frame count
            capture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 640); // Set frame width
            capture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 480); // Set frame height

            using var stream = new MemoryStream(); // Memory stream for image processing

            while (!cancellationToken.IsCancellationRequested) // Run until cancellation is requested
            {
                using (var frame = capture.QueryFrame().ToBitmap()) // Capture a frame
                {
                    stream.SetLength(0); // Clear the stream for new image
                    frame.Save(stream, ImageFormat.Bmp); // Save frame to stream in BMP format
                    stream.Position = 0; // Reset stream position

                    // Create SKImage from the stream
                    var img = SKImage.FromEncodedData(stream.ToArray());
                    if (img == null)
                    {
                        // Handle the case where img is null (invalid image)
                        continue; // Skip this iteration if the image is invalid
                    }

                    // Run object detection
                    var result = Yolo.RunObjectDetection(img);
                    if (result == null || result.Count == 0)
                    {
                        Console.WriteLine("No objects detected."); // Log if no objects detected
                        continue; // Skip drawing if no objects are detected
                    }

                    // Draw detections on the image
                    img = DrawDetections(img, result);

                    // Convert SKImage to Bitmap and set it to PictureBox
                    pictureBox1.Image = ImageToBitmap(img);
                }
            }
        }

        private static SKImage DrawDetections(SKImage img, List<ObjectDetection> results)
        {
            using (var surface = SKSurface.Create(img.Info)) // Create surface for drawing
            {
                var canvas = surface.Canvas;
                canvas.DrawImage(img, 0, 0); // Draw the original image

                // Set paint for drawing rectangles
                var rectPaint = new SKPaint
                {
                    Color = SKColors.Red,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 3 // Rectangle stroke width
                };

                // Set paint for drawing text
                var textPaint = new SKPaint
                {
                    Color = SKColors.Red,
                    TextSize = 20,
                    IsAntialias = true,
                    Typeface = SKTypeface.FromFamilyName("Arial") // Font for labels
                };

                foreach (var result in results) // Iterate through detection results
                {
                    // Calculate the bounding box
                    var left = result.BoundingBox.MidX - (result.BoundingBox.Width / 2);
                    var top = result.BoundingBox.MidY - (result.BoundingBox.Height / 2);
                    var right = result.BoundingBox.MidX + (result.BoundingBox.Width / 2);
                    var bottom = result.BoundingBox.MidY + (result.BoundingBox.Height / 2);

                    var rect = new SKRect(left, top, right, bottom);
                    canvas.DrawRect(rect, rectPaint); // Draw the bounding rectangle

                    // Draw the label above the rectangle
                    var label = result.Label.Name; // Get the label name
                    var textX = left; // Position the text at the left of the rectangle
                    var textY = top - 5; // Position the text above the rectangle (5 pixels above)

                    // Draw the label
                    canvas.DrawText(label, textX, textY, textPaint);
                }

                return surface.Snapshot(); // Return the modified image with detections
            }
        }

        private static Bitmap ImageToBitmap(SKImage img)
        {
            // Create a memory stream to hold the image data
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Encode the SKImage to SKData in PNG format
                using (SKData skData = img.Encode(SKEncodedImageFormat.Png, 100)) // Encode to SKData
                {
                    // Write the SKData to the memory stream
                    skData.SaveTo(memoryStream);
                }

                memoryStream.Position = 0; // Reset the stream position to the beginning

                // Create a Bitmap from the memory stream
                Bitmap bitmap = new Bitmap(memoryStream);
                return bitmap; // Return the Bitmap
            }
        }

        private void start_Click(object sender, EventArgs e)
        {
            cancellationTokenSource = new CancellationTokenSource(); // Create a cancellation token source
            cancellationToken = cancellationTokenSource.Token; // Get the cancellation token
            Task.Run(() => WebCamAsync(cancellationToken), cancellationToken); // Start webcam async task
        }
    }
}