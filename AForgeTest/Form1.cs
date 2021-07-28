using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using System.Threading;
using AForge.Imaging.Filters;
using AForge;
using AForge.Imaging;

namespace AForgeTest
{
    public partial class Form1 : Form
    {
        FilterInfoCollection videoDevices;
        private string device;

        // Video device
        public string VideoDevice
        {
            get { return device; }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            device = videoDevices[0].MonikerString; //1 is Microsoft LifeCam
            VideoCaptureDevice videoSource = new VideoCaptureDevice(device);

            // set busy cursor
            this.Cursor = Cursors.WaitCursor;

            // stop current video source
            videoSourcePlayer1.SignalToStop();

            // wait 2 seconds until camera stops
            for (int i = 0; (i < 50) && (videoSourcePlayer1.IsRunning); i++)
            {
                Thread.Sleep(100);
            }
            if (videoSourcePlayer1.IsRunning)
                videoSourcePlayer1.Stop();

            videoSourcePlayer1.BorderColor = Color.Black;
            this.Cursor = Cursors.Default;

            // start new video source
            videoSourcePlayer1.VideoSource = videoSource;
            videoSourcePlayer1.Start();

            this.Cursor = Cursors.Default;
        }

        bool selecting = false;
        Rectangle selection = new Rectangle();
        int x = 0;
        int y = 0;
        int width = 0;
        int height = 0;
        Bitmap originalImage;

        //detect and object
        //http://www.aforgenet.com/articles/step_to_stereo_vision/
        //http://www.codeproject.com/KB/cs/Webcam_Tic_Tac_Toe.aspx
        //blob tracking
        //http://stackoverflow.com/questions/2636020/recognize-objects-in-image

        private void videoSourcePlayer1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selecting = true;
                x = e.X;
                y = e.Y;
            }
        }

        private void videoSourcePlayer1_MouseMove(object sender, MouseEventArgs e)
        {
            if (selecting)
            {
                width = e.X - x;
                if ((e.Y - y) > 2)
                    height = int.Parse(
                            (
                                (
                                    3 * (e.X - x)
                                    ) / 4
                            ).ToString()
                        );

                videoSourcePlayer1.Refresh();
            }
        }

        private void videoSourcePlayer1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selecting = false;
                imageToTracjPictureBox.Image = ResizeImage(Crop(originalImage, new Rectangle(x, y, width, height)), 64, 48, false);
            }
        }

        private void videoSourcePlayer1_Paint(object sender, PaintEventArgs e)
        {
            if (selecting)
            {
                Pen pen = Pens.GreenYellow;
                e.Graphics.DrawRectangle(pen, new Rectangle(x, y, width, height));
            }
        }

        private void videoSourcePlayer1_NewFrame(object sender, ref Bitmap image)
        {
            originalImage = (Bitmap)image.Clone();

            //// create filter
            //ColorFiltering colorFilter = new ColorFiltering();
            //// configure the filter
            //colorFilter.Red = new IntRange(0, 100);
            //colorFilter.Green = new IntRange(0, 200);
            //colorFilter.Blue = new IntRange(150, 255);
            //// apply the filter
            //Bitmap objectImage = colorFilter.Apply(image);

            //// create blob counter and configure it
            //BlobCounter blobCounter = new BlobCounter();
            //blobCounter1.MinWidth = 25;                    // set minimum size of
            //blobCounter1.MinHeight = 25;                   // objects we look for
            //blobCounter1.FilterBlobs = true;               // filter blobs by size
            //blobCounter1.ObjectsOrder = ObjectsOrder.Size; // order found object by size
            //// grayscaling
            //Bitmap grayImage = grayFilter.Apply(objectImage);
            //// locate blobs 
            //blobCounter.ProcessImage(grayImage);
            //Rectangle[] rects = blobCounter.GetObjectRectangles();
            //// draw rectangle around the biggest blob
            //if (rects.Length > 0)
            //{
            //    Rectangle objectRect = rects[0];
            //    Graphics g = Graphics.FromImage(image);

            //    using (Pen pen = new Pen(Color.FromArgb(160, 255, 160), 3))
            //    {
            //        g.DrawRectangle(pen, objectRect);
            //    }

            //    g.Dispose();
            //}


        }

        public static Bitmap Crop(Bitmap bitmap, Rectangle rectangle)
        {
            if (bitmap == null)
                throw new ArgumentException("Bitmap cannot be null.");

            try
            {
                Bitmap croppedBitmap = (Bitmap)bitmap.Clone();

                //constrain passed in size to the dimensions of the bitmap
                if (rectangle.Left + rectangle.Width > bitmap.Width)
                    rectangle.Width = bitmap.Width - rectangle.Width;

                if (rectangle.Top + rectangle.Height > bitmap.Height)
                    rectangle.Height = bitmap.Height - rectangle.Height;

                //crop
                bitmap = (Bitmap)croppedBitmap.Clone(rectangle, bitmap.PixelFormat);
            }
            catch
            {
                throw;
            }

            GC.Collect();

            return bitmap;
        }

        public static Bitmap ResizeImage(Bitmap bitmap, int width, int height, bool useBilinear)
        {
            if (bitmap == null)
                throw new ArgumentException("Bitmap cannot be null.");

            Bitmap bTemp = (Bitmap)bitmap.Clone();
            bitmap = new Bitmap(width, height, bTemp.PixelFormat);

            double nXFactor = (double)bTemp.Width / (double)width;
            double nYFactor = (double)bTemp.Height / (double)height;

            try
            {
                if (useBilinear)
                {
                    double fraction_x, fraction_y, one_minus_x, one_minus_y;
                    int ceil_x, ceil_y, floor_x, floor_y;
                    Color c1 = new Color();
                    Color c2 = new Color();
                    Color c3 = new Color();
                    Color c4 = new Color();
                    byte red, green, blue;

                    byte b1, b2;

                    int nWidth = bitmap.Width;
                    int nHeight = bitmap.Height;

                    for (int x = 0; x < nWidth; ++x)
                        for (int y = 0; y < nHeight; ++y)
                        {
                            // Setup

                            floor_x = (int)Math.Floor(x * nXFactor);
                            floor_y = (int)Math.Floor(y * nYFactor);
                            ceil_x = floor_x + 1;
                            if (ceil_x >= bTemp.Width) ceil_x = floor_x;
                            ceil_y = floor_y + 1;
                            if (ceil_y >= bTemp.Height) ceil_y = floor_y;
                            fraction_x = x * nXFactor - floor_x;
                            fraction_y = y * nYFactor - floor_y;
                            one_minus_x = 1.0 - fraction_x;
                            one_minus_y = 1.0 - fraction_y;

                            c1 = bTemp.GetPixel(floor_x, floor_y);
                            c2 = bTemp.GetPixel(ceil_x, floor_y);
                            c3 = bTemp.GetPixel(floor_x, ceil_y);
                            c4 = bTemp.GetPixel(ceil_x, ceil_y);

                            // Blue
                            b1 = (byte)(one_minus_x * c1.B + fraction_x * c2.B);

                            b2 = (byte)(one_minus_x * c3.B + fraction_x * c4.B);

                            blue = (byte)(one_minus_y * (double)(b1) + fraction_y * (double)(b2));

                            // Green
                            b1 = (byte)(one_minus_x * c1.G + fraction_x * c2.G);

                            b2 = (byte)(one_minus_x * c3.G + fraction_x * c4.G);

                            green = (byte)(one_minus_y * (double)(b1) + fraction_y * (double)(b2));

                            // Red
                            b1 = (byte)(one_minus_x * c1.R + fraction_x * c2.R);

                            b2 = (byte)(one_minus_x * c3.R + fraction_x * c4.R);

                            red = (byte)(one_minus_y * (double)(b1) + fraction_y * (double)(b2));

                            bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(255, red, green, blue));
                        }
                }
                else
                {
                    for (int x = 0; x < bitmap.Width; ++x)
                        for (int y = 0; y < bitmap.Height; ++y)
                            bitmap.SetPixel(x, y, bTemp.GetPixel((int)(Math.Floor(x * nXFactor)), (int)(Math.Floor(y * nYFactor))));
                }
            }
            catch
            {
                throw;
            }

            GC.Collect();

            return bitmap;

        }
    }
}
