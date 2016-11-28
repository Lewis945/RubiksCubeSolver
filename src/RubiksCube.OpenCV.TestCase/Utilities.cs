using Emgu.CV;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase
{
    public class Utilities
    {
        public static void WriteVideo(string path)
        {
            var viewer = new ImageViewer();

            int counter = 0;

            const int fps = 24;

            Task.Run(() =>
            {
                using (var capture = new Capture())
                {
                    var frameWidth = (int)capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth);
                    var frameHeight = (int)capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight);

                    using (var writer = new VideoWriter(path, -1, fps, new Size(frameWidth, frameHeight), true))
                    {
                        while (true)
                        {
                            var frame = capture.QueryFrame();
                            viewer.Image = frame;
                            writer.Write(frame);

                            counter++;
                        }
                    }
                }
            });

            viewer.ShowDialog();
        }
    }
}
