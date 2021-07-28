using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Imaging.Filters;
using AForge.Imaging;
using System.Threading;
using AForge.Video;
using System.Timers;
using System.Drawing.Imaging;
using Phidgets;
using Phidgets.Events;
using System.Runtime.InteropServices;

namespace AForgeTest
{
    public partial class ObjectTracking : Form
    {
        #region Enums, Properties & Fields

        private InterfaceKit ifKit;
        private CheckBox[] digiInArray = new CheckBox[16];
        private CheckBox[] digiOutArray = new CheckBox[16];
        private CheckBox[] digiOutDispArray = new CheckBox[16];
        private TextBox[] sensorInArray = new TextBox[8];

        private enum MessageType
        {
            Success,
            Informational,
            Warning,
            Failure
        }

        /// <summary>
        /// List of video input devices (webcams) connected to the PC.
        /// </summary>
        private FilterInfoCollection VideoDevices { get; set; }
        private VideoCaptureDevice VideoSource { get; set; }

        private Bitmap CameraImage { get; set; }
        private System.Timers.Timer CameraFPSTimer { get; set; }

        private System.Timers.Timer displayTimer;
        private int DisplaySeconds;

        private ColorFiltering ColorFilter { get; set; }
        private GrayscaleBT709 GrayscaleFilter { get; set; }
        /// <summary>
        /// Object used to keep track of Blob[] objects in an image.
        /// </summary>
        private BlobCounter BlobCounter { get; set; }
        private Thread TrackingThread { get; set; }
        private bool IsTracking { get; set; }
        /// <summary>
        /// The coordinates of the object to track within an image. X = x1, Y = y1, Width = (x2 - x1), Height = (y2 - y1)
        /// </summary>
        private Rectangle ObjectCoordinates { get; set; }

        private long[] FPS;
        private List<DateTime> FrameTimes;

        System.Timers.Timer processImageTimer;

        bool selecting = false;
        int x = 0;
        int y = 0;
        int width = 0;
        int height = 0;
        Bitmap originalImage;

        Histogram histogram;

        //object center coordinates
        int x1, y1;

        string lastFps;
        System.Timers.Timer ProcessedFPSTimer;

        private AdvancedServo advServo; //Declare an advancedservo object

        int imageCenterX;
        int imageCenterY;

        int lastSv1Position;
        int lastSv2Position;
        int lastSv3Position;
        int lastSv4Position;

        #endregion
        
        //constructor
        public ObjectTracking()
        {
            InitializeComponent();

            SetupCamera();
            SetupFilters();
            ConnectCamera(true);

            trackingButton.Enabled = false;
        }
        
        #region Private Methods
        
        private void SetupCamera()
        {
            try
            {
                //VideoDevices.Clear();
                VideoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (VideoDevices.Count == 0)
                    throw new Exception("No video input devices detected.");

                for (int i = 1, n = VideoDevices.Count; i <= n; i++)
                {
                    string cameraName = i + " : " + VideoDevices[i - 1].Name;
                    cameraListComboBox.Items.Add(cameraName);
                }

                //(0 = Logitech, 1 = LifeCam)
                cameraListComboBox.SelectedIndex = 0;

                Message(cameraListComboBox.Items.Count.ToString() + " cameras found.",
                    MessageType.Success, 2);
            }
            catch
            {
                trackingButton.Enabled = false;
                cameraListComboBox.Items.Add("No cameras found.");
                cameraListComboBox.SelectedIndex = 0;
                cameraListComboBox.Enabled = false;

                Message("No cameras found. Make sure a webcam is connected and that the drivers installed.",
                    MessageType.Warning, 10);
            }
        }

        private void SetupFilters()
        {
            // WTF?
            //GrayscaleFilter = new Grayscale.CommonAlgorithms.BT709();

            //AForge.Imaging.Filters.GrayscaleBT709 is obsolete: Use Grayscale.CommonAlgorithms.BT709 object instead

            GrayscaleFilter = new GrayscaleBT709();

            // color of object to track
            ColorFilter = new ColorFiltering();
            ColorFilter.Red = new AForge.IntRange(0, 100);
            ColorFilter.Green = new AForge.IntRange(0, 200);
            ColorFilter.Blue = new AForge.IntRange(150, 255);

            // configure blob counters
            BlobCounter = new BlobCounter();
            BlobCounter.MinWidth = 25; //blob size
            BlobCounter.MinHeight = 25;
            BlobCounter.FilterBlobs = true;
            BlobCounter.ObjectsOrder = ObjectsOrder.Size;
        }

        private void SetupTrackingColorFilters(Histogram histogram)
        {
            GrayscaleFilter = new GrayscaleBT709();

            int lowRed = 0;
            int highRed = 255;

            int lowGreen = 0;
            int highGreen = 255;

            int lowBlue = 0;
            int highBlue = 255;

            int red = histogram.RedAVG;
            int green = histogram.GreenAVG;
            int blue = histogram.BlueAVG;

            int range = 25; //+-50 = range of 100

            //too small range makes the blobs too small and it won't track (10 ish)
            //too large and it detects other objects too well (50ish)
            //25 isn't bad.

            if (red - range > 0)
                lowRed = red - range;
            if (red + range < 255)
                highRed = red + range;

            if (green - range > 0)
                lowGreen = green - range;
            if (green + range < 255)
                highGreen = green + range;

            if (blue - range > 0)
                lowBlue = blue - range;
            if (blue + range < 255)
                highBlue = blue + range;

            // color of object to track
            ColorFilter = new ColorFiltering();
            ColorFilter.Red = new AForge.IntRange(lowRed, highRed);
            ColorFilter.Green = new AForge.IntRange(lowGreen, highGreen);
            ColorFilter.Blue = new AForge.IntRange(lowBlue, highBlue);

            // configure blob counters
            BlobCounter = new BlobCounter();
            BlobCounter.MinWidth = 10; //blob size
            BlobCounter.MinHeight = 10;
            BlobCounter.FilterBlobs = true;
            BlobCounter.ObjectsOrder = ObjectsOrder.Size;

            redRangeLabel.Text = lowRed.ToString() + " / " + highRed.ToString();
            greenRangeLabel.Text = lowGreen.ToString() + " / " + highGreen.ToString();
            blueRangeLabel.Text = lowBlue.ToString() + " / " + highBlue.ToString();
        }

        private void ConnectCamera(bool connect)
        {
            if (connect)
            {
                try
                {
                    cameraFeedVideoSourcePlayer.SignalToStop();
                    cameraFeedVideoSourcePlayer.WaitForStop();

                    VideoSource = new VideoCaptureDevice(VideoDevices[cameraListComboBox.SelectedIndex].MonikerString);
                    VideoSource.DesiredFrameRate = 30;
                    VideoSource.DesiredFrameSize = new Size(320, 240);
                    cameraFeedVideoSourcePlayer.VideoSource = VideoSource;
                    cameraFeedVideoSourcePlayer.Start();

                    //FPS count for live feed
                    FPS = new long[10];
                    Control.CheckForIllegalCrossThreadCalls = false; //HACK!
                    CameraFPSTimer = new System.Timers.Timer();
                    CameraFPSTimer.Interval = 1000; //every 1 second
                    CameraFPSTimer.Elapsed += new System.Timers.ElapsedEventHandler(CameraFPSTimer_Elapsed);
                    CameraFPSTimer.Start();
                }
                catch
                {
                    Message("Cannot connect to camera. Try restarting the application.",
                        MessageType.Failure, 0);
                }
            }
            else
            {
                cameraFeedVideoSourcePlayer.SignalToStop();
                cameraFeedVideoSourcePlayer.WaitForStop();
                cameraFeedVideoSourcePlayer.VideoSource = null;
            }
        }

        private void CameraFPSTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                IVideoSource videoSource = cameraFeedVideoSourcePlayer.VideoSource;

                if (videoSource != null)
                {
                    int frameCount = videoSource.FramesReceived;

                    if (frameCount > 0)
                        feedFPSLabel.Text = frameCount.ToString();
                }

                if (CameraFPSTimer != null)
                {
                    CameraFPSTimer.Stop();
                    CameraFPSTimer.Start();
                }
            }
            catch
            { }
        }

        private void AddFPSTime()
        {
            //for (int i = 0; i < FPS.Length - 1; i++)
            //{
            //    //move everything up one
            //    FPS[i + 1] = FPS[i];
            //}

            //FPS[0] = DateTime.Now.Ticks;

            //if (FrameTimes.Length > 0)
            //{
            //    for (int i = 0; i <= FrameTimes.Length - 1; i++)
            //    {
            //        //move everything up one
            //        FrameTimes[i + 1] = FrameTimes[i];
            //    }
            //}
            
            //FrameTimes[0] = DateTime.Now;

            //bool t;
            //if (FrameTimes.Length == 10)
            //    t = true;

            //add one to the end
            FrameTimes.Add(DateTime.Now);
            //make sure we have 10 by removing the oldest one
            while (FrameTimes.Count > 10)
            {
                FrameTimes.RemoveAt(0);
            }
        }
        
        private string AverageProcessedFPS()
        {
            //long start = FPS[9];
            //long end = FPS[0];

            //DateTime startDT = DateTime.Now;

            DateTime start = FrameTimes[0];
            DateTime end = FrameTimes[9];
            double totMs = (end - start).TotalMilliseconds;

            double fpsCalc = 10 / totMs * 1000;

            string fps = String.Format("{0:0}", fpsCalc);

            if (lastFps != String.Empty)
            {
                //subtract 1 so it looks like it is changing all the time
                if (fps == lastFps)
                {
                    int conv = int.Parse(fps);
                    fps = (conv - 1).ToString();
                }
            }

            lastFps = fps;
            
            return fps;
        }
                
        private void Tracking(bool enabled)
        {
            if (enabled)
            {
                if (ProcessedFPSTimer == null)
                {
                    //FPS = new long[10];

                    ProcessedFPSTimer = new System.Timers.Timer();
                    ProcessedFPSTimer.Interval = 1000; //every 1 second
                    ProcessedFPSTimer.Elapsed += new ElapsedEventHandler(ProcessedFPSTimer_Elapsed);
                    ProcessedFPSTimer.Start();
                }

                if (trackRobotCheckBox.Checked)
                {
                    //set these values to slow the servos down, so they don't burn up
                    //sv1.SpeedRamping = true;
                    //sv2.SpeedRamping = true;
                    //sv3.SpeedRamping = true;
                    //sv4.SpeedRamping = true;

                    ////slow
                    //sv1.VelocityLimit = 90;
                    //sv2.VelocityLimit = 90;
                    //sv3.VelocityLimit = 90;
                    //sv4.VelocityLimit = 90;

                    //sv1.Acceleration = 2000;
                    //sv2.Acceleration = 2000;
                    //sv3.Acceleration = 2000;
                    //sv4.Acceleration = 2000;

                    ////fast
                    //sv1.VelocityLimit = 200;
                    //sv2.VelocityLimit = 200;
                    //sv3.VelocityLimit = 200;
                    //sv4.VelocityLimit = 200;

                    //sv1.Acceleration = 8000;
                    //sv2.Acceleration = 8000;
                    //sv3.Acceleration = 8000;
                    //sv4.Acceleration = 8000;

                    //engage the servos for movement
                    sv1.Engaged = true;
                    sv2.Engaged = true;
                    sv3.Engaged = true;
                    sv4.Engaged = true;

                    //set positions
                    sv1.Position = 95;
                    sv2.Position = 165;
                    sv3.Position = 90;// 150;
                    sv4.Position = 180;// 100;

                    lastSv1Position = 95;
                    lastSv2Position = 165;
                    lastSv3Position = 90;// 150;
                    lastSv4Position = 180;// 100;

                    //sv1.Position = 90;
                    //sv2.Position = 90;
                    //sv3.Position = 90;
                    //sv4.Position = 90;

                    //lastSv1Position = 90;
                    //lastSv2Position = 90;
                    //lastSv3Position = 90;
                    //lastSv4Position = 90;

                    trackRobotCheckBox.Enabled = false;
                }
            }
            else
            {
                if (ProcessedFPSTimer != null)
                {
                    ProcessedFPSTimer.Stop();
                    ProcessedFPSTimer = null;
                }

                trackRobotCheckBox.Enabled = true;

                //reset positions
                sv1.Position = 95;
                sv2.Position = 165;
                sv3.Position = 150;
                sv4.Position = 100;

                //enagage Phidgets servo controller
                sv1.Engaged = false;
                sv2.Engaged = false;
                sv3.Engaged = false;
                sv4.Engaged = false;
            }

            IsTracking = enabled;
            SetUITracking(enabled);
        }

        private void ProcessedFPSTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                processedFPSLabel.Text = AverageProcessedFPS().ToString();
            }
            catch
            { }
        }

        private void SetUITracking(bool enable)
        {
            if (enable)
            {

            }
            else
            {
                processedFPSLabel.Text = "000";
            }
        }

        private Bitmap DrawLiveMarker(Bitmap bitmap, Color color)
        {
            if (bitmap != null)
            {
                try
                {
                    Graphics graphicsObject = Graphics.FromImage(bitmap);
                    Brush brush = new SolidBrush(color);
                    graphicsObject.FillEllipse(brush, 5, 5, 15, 15);
                    graphicsObject.DrawImage(bitmap, 0, 0);
                }
                catch
                {

                }
            }

            return bitmap;
        }
        
        //DETECT MAIN METHOD
        private Bitmap DetectObjects(Bitmap bitmap, bool drawOnImage)
        {
            try
            {
                // color
                bitmap = ColorFilter.Apply(bitmap);

                // lock image for further processing
                BitmapData objectData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

                // grayscaling
                UnmanagedImage grayImage = GrayscaleFilter.Apply(new UnmanagedImage(objectData));

                // unlock image
                bitmap.UnlockBits(objectData);

                // locate blobs 
                BlobCounter.ProcessImage(grayImage);
                //BlobCounter.ObjectsOrder = ObjectsOrder.Size;
                Rectangle[] rects = BlobCounter.GetObjectsRectangles();

                if (drawOnImage)
                {
                    if (rects.Length > 0)
                    {
                        Rectangle blobs = rects[0];

                        // draw rectangle around derected object
                        Graphics graphicsObject = Graphics.FromImage(bitmap);

                        using (Pen pen = new Pen(Color.Red, 3))
                            graphicsObject.DrawRectangle(pen, blobs);

                        graphicsObject.Dispose();

                        //int x1;
                        //int y1;

                        // get object's center coordinates relative to image center
                        //lock (this)
                        //{
                            //x1 = (blobs.Left + blobs.Right - bitmap.Width) / 2;
                            //y1 = (bitmap.Height - (blobs.Top + blobs.Bottom)) / 2;
                            // map to [-1, 1] range
                            //x1 /= (bitmap.Width / 2);
                            //y1 /= (bitmap.Height / 2);
                        int x = ((blobs.Right - blobs.Left) / 2) + blobs.Left;
                        int y = ((blobs.Bottom - blobs.Top) / 2) + blobs.Top;
                        imageCenterX = bitmap.Width / 2;
                        imageCenterY = bitmap.Height / 2;

                        int[] normalizedImageCenter = NormalizeCoordinatesForServo(imageCenterX, imageCenterY, bitmap.Width, bitmap.Height);
                        int[] normalizedObjectCenter = NormalizeCoordinatesForServo(x, y, bitmap.Width, bitmap.Height);



                        int xToMove = normalizedImageCenter[0] - normalizedObjectCenter[0]; 
                        int yToMove = (normalizedImageCenter[1] - normalizedObjectCenter[1]) * -1; //invert Y because the servo is inverterted

                        //}
                        
                        objectCenterLabel.Text = x.ToString() + " / " + y.ToString();
                        robotCenterLabel.Text = (lastSv3Position + xToMove).ToString() + " / " + (lastSv1Position + yToMove).ToString();

                        //move servos to center the object in it's field of view
                        try
                        {
                            //int[] normalizedCoordinates = NormalizeCoordinatesForServo(x1, y1, bitmap.Width, bitmap.Height);
                            //int[] normalizedCoordinates = NormalizeCoordinatesForServo(xToMove, yToMove, bitmap.Width, bitmap.Height);
                            //robotCenterLabel.Text = (normalizedCoordinates[0] * -1).ToString() + " / " + normalizedCoordinates[1].ToString();    
                            if (!overrideMovementCheckBox.Checked)
                            {
                                /*
                                 * A better way of doing this is to move the servos by 1 pixel and see if they have "zeroed" the 
                                 * difference from the object to the center of the webcam.
                                 */

                                ////up/down
                                //sv1.Position = lastSv1Position + yToMove;
                                ////left/right
                                //sv3.Position = lastSv3Position + xToMove;

                                int range = 10;

                                int trackingSpeed = Convert.ToInt32(accelerationNumericUpDown.Value);

                                if (xToMove <= (0 - range))
                                {
                                    sv3.Position = lastSv3Position + trackingSpeed;
                                    lastSv3Position = lastSv3Position + trackingSpeed;
                                }
                                else if (xToMove >= (0 + range))
                                {
                                    sv3.Position = lastSv3Position - trackingSpeed;
                                    lastSv3Position = lastSv3Position - trackingSpeed;
                                }

                                if (yToMove <= (0 - range))
                                {
                                    sv1.Position = lastSv1Position - trackingSpeed;
                                    lastSv1Position = lastSv1Position - trackingSpeed;
                                }
                                else if (yToMove >= (0 + range))
                                {
                                    sv1.Position = lastSv1Position + trackingSpeed;
                                    lastSv1Position = lastSv1Position + trackingSpeed;
                                }

                                robotPositionLabel.Text = sv3.Position.ToString() + " / " + sv1.Position.ToString();
                            }
                        }
                        catch (PhidgetException ex)
                        {
                            //MessageBox.Show(ex.Description);
                        }
                    }
                }
            }
            catch
            {

            }
            
            return bitmap;
        }

        /// <summary>
        /// Sets a message in the UI.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="displaySeconds">if "0" then the message will be displayed permanently, otherwise it will go away after the set time</param>
        private void Message(string message, MessageType type, int displaySeconds)
        {
            messageLabel.Text = message;

            switch (type)
            {
                case MessageType.Success:
                    messageLabel.ForeColor = Color.Green;
                    break;
                case MessageType.Informational:
                    messageLabel.ForeColor = Color.Black;
                    break;
                case MessageType.Warning:
                    messageLabel.ForeColor = Color.Orange;
                    break;
                case MessageType.Failure:
                    messageLabel.ForeColor = Color.Red;
                    break;
                default:
                    messageLabel.ForeColor = Color.Green;
                    break;
            }

            //TODO: add ability to temporarily display a message, make it fade or disapear after some displaySeconds value
            if (displaySeconds > 0)
            {
                DisplaySeconds = displaySeconds;

                displayTimer = new System.Timers.Timer();
                displayTimer.Interval = (displaySeconds * 1000);
                displayTimer.Elapsed += new System.Timers.ElapsedEventHandler(displayTimer_Elapsed);
                displayTimer.Start();
            }
        }

        private void CropImage()
        {
            Bitmap bitmap = Imaging.Crop(originalImage, new Rectangle(x, y, width, height));
            imageToTracjPictureBox.Image = Imaging.Resize(bitmap, imageToTracjPictureBox.Width, imageToTracjPictureBox.Height, false);
        }

        private Bitmap GetChannelAverageImage(int red, int green, int blue)
        {
            Bitmap bitmap = new Bitmap(24, 24);
            Graphics graphics = Graphics.FromImage(bitmap);
            Color color = Color.FromArgb(red, green, blue);
            Pen pen = new Pen(color, 24);
            graphics.DrawRectangle(pen, 0, 0, 24, 24);
            return bitmap;
        }

        private void openCmdLine(Phidget p)
        {
            openCmdLine(p, null);
        }

        private void openCmdLine(Phidget p, String pass)
        {
            int serial = -1;
            int port = 5001;
            String host = null;
            bool remote = false, remoteIP = false;
            string[] args = Environment.GetCommandLineArgs();
            String appName = args[0];

            try
            { //Parse the flags
                for (int i = 1; i < args.Length; i++)
                {
                    if (args[i].StartsWith("-"))
                        switch (args[i].Remove(0, 1).ToLower())
                        {
                            case "n":
                                serial = int.Parse(args[++i]);
                                break;
                            case "r":
                                remote = true;
                                break;
                            case "s":
                                remote = true;
                                host = args[++i];
                                break;
                            case "p":
                                pass = args[++i];
                                break;
                            case "i":
                                remoteIP = true;
                                host = args[++i];
                                if (host.Contains(":"))
                                {
                                    port = int.Parse(host.Split(':')[1]);
                                    host = host.Split(':')[0];
                                }
                                break;
                            default:
                                goto usage;
                        }
                    else
                        goto usage;
                }

                if (remoteIP)
                    p.open(serial, host, port, pass);
                else if (remote)
                    p.open(serial, host, pass);
                else
                    p.open(serial);
                return; //success
            }
            catch { }
        usage:
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Invalid Command line arguments." + Environment.NewLine);
            sb.AppendLine("Usage: " + appName + " [Flags...]");
            sb.AppendLine("Flags:\t-n   serialNumber\tSerial Number, omit for any serial");
            sb.AppendLine("\t-r\t\tOpen remotely");
            sb.AppendLine("\t-s   serverID\tServer ID, omit for any server");
            sb.AppendLine("\t-i   ipAddress:port\tIp Address and Port. Port is optional, defaults to 5001");
            sb.AppendLine("\t-p   password\tPassword, omit for no password" + Environment.NewLine);
            sb.AppendLine("Examples: ");
            sb.AppendLine(appName + " -n 50098");
            sb.AppendLine(appName + " -r");
            sb.AppendLine(appName + " -s myphidgetserver");
            sb.AppendLine(appName + " -n 45670 -i 127.0.0.1:5001 -p paswrd");
            MessageBox.Show(sb.ToString(), "Argument Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            Application.Exit();
        }

        /// <summary>
        /// Pass in the mid point "x" and "y" of your object to track along with the width and height of the bitmap you found it in and the values will be normalized within 0 and 180, the bounds of a Phidgets servo position
        /// </summary>
        private int[] NormalizeCoordinatesForServo(int x, int y, int width, int height)
        {
            //keep track of last position, get current position then add or subtract that to the new coordinates to move

            //normal servo positions are 0 to 180 (for HS322 and HS-81)
            //this method takes the position x,y inside of width,height and smooshes it between 0 and 180

            int[] newCoordinateValues = new int[2]; //"x" and "y"

            if (x != 0 && y != 0)
            {
                double widthRatio = 180.0 / width;
                double heightRatio = 180.0 / height;

                double xNormalized = 0;
                double yNormalized = 0;

                if (x > 0)
                    xNormalized = x * widthRatio;

                if (y > 0)
                    yNormalized = y * heightRatio;

                newCoordinateValues[0] = Convert.ToInt32(xNormalized);
                newCoordinateValues[1] = Convert.ToInt32(yNormalized);
            }
            else
            {
                newCoordinateValues[0] = width / 2;
                newCoordinateValues[1] = height / 2;
            }
            return newCoordinateValues;
        }

        #endregion

        private void ObjectTracking_Load(object sender, EventArgs e)
        {
            advServo = new AdvancedServo();

            advServo.Attach += new AttachEventHandler(advServo_Attach);
            advServo.Detach += new DetachEventHandler(advServo_Detach);
            advServo.Error += new ErrorEventHandler(advServo_Error);

            advServo.CurrentChange += new CurrentChangeEventHandler(advServo_CurrentChange);
            advServo.PositionChange += new PositionChangeEventHandler(advServo_PositionChange);
            advServo.VelocityChange += new VelocityChangeEventHandler(advServo_VelocityChange);

            //This assumes that if there is a command line argument, it is a serial number
            //and we try to open that specific device. Otherwise, open any device.
            openCmdLine(advServo);

            ifKit = new InterfaceKit();

            ifKit.Attach += new AttachEventHandler(ifKit_Attach);
            ifKit.Detach += new DetachEventHandler(ifKit_Detach);
            ifKit.Error += new ErrorEventHandler(ifKit_Error);

            ifKit.InputChange += new InputChangeEventHandler(ifKit_InputChange);
            ifKit.OutputChange += new OutputChangeEventHandler(ifKit_OutputChange);
            ifKit.SensorChange += new SensorChangeEventHandler(ifKit_SensorChange);

            //Open the Phidget using the command line arguments
            openCmdLine(ifKit);

            //not sure if I need to "check" this, it is in the ifkit_full example
            ifKit.ratiometric = true;

            //left right control timer
            turn_tmr = new System.Timers.Timer();
            turn_tmr.Enabled = true;
            turn_tmr.Interval = _elapsedTimeLR;
            turn_tmr.Elapsed += new ElapsedEventHandler(turn_tmr_Elapsed);

            //up down control timer
            vert_tmr = new System.Timers.Timer();
            vert_tmr.Enabled = true;
            vert_tmr.Interval = _elapsedTimeUD;
            vert_tmr.Elapsed += new ElapsedEventHandler(vert_tmr_Elapsed);

            if(objectSelectionCheckBox.Checked)
                trackingButton.Enabled = true;
            else
                trackingButton.Enabled = false;
        }

        #region Phidgets Interface kit - Joystick movement

        int pixels = 1;
        int sensorModifier = 100;

        void vert_tmr_Elapsed(object sender, ElapsedEventArgs e)
        {
            //calculate turn speed (num pixels to move)
            //int pixels = 1;
            //if (ifKit.sensors[0].Value > sensorMidPoint + 100)
            //{
            //    pixels = (((ifKit.sensors[0].Value - sensorMidPoint) * 2) / 50);
            //}
            //else if (ifKit.sensors[0].Value < sensorMidPoint - 100)
            //{
            //    pixels = (((sensorMidPoint - ifKit.sensors[0].Value) * 2) / 50);
            //}

            pixels = Convert.ToInt32(accelerationNumericUpDown.Value);

            //up and down
            if (ifKit.sensors[0].Value > sensorMidPoint + 15 + sensorModifier) //470 (upper range of resting mid point was 460)
            {
                //go up
                //if (sensorFloatingPoint_UpDown < (999 - pixels)) //let's get a little bit of a buffer from 999
                //{
                //    sensorFloatingPoint_UpDown = sensorFloatingPoint_UpDown + (pixels);
                    if (sv1.Position + pixels < 180)
                        sv1.Position += pixels;
                    else
                        sv1.Position = 180;
                //}
            }
            else if (ifKit.sensors[0].Value < sensorMidPoint - 10 - sensorModifier) //440 (lower range of resting mid point was 450)
            {
                //go down
                //if (sensorFloatingPoint_UpDown > (0 + pixels)) //let's get a little bit of a buffer from 0
                //{
                //    sensorFloatingPoint_UpDown = sensorFloatingPoint_UpDown - (pixels);
                    if (sv1.Position - pixels > 0)
                        sv1.Position -= pixels;
                    else
                        sv1.Position = 0;
                //}
            }
            else
            {
                vert_tmr.Stop();
            }
        }

        void turn_tmr_Elapsed(object sender, ElapsedEventArgs e)
        {
            //calculate turn speed (num pixels to move)
            //int pixels = 1;
            //if (ifKit.sensors[1].Value > sensorMidPoint + 100)
            //{
            //    pixels = (((ifKit.sensors[1].Value - sensorMidPoint) * 2) / 50);
            //}
            //else if (ifKit.sensors[1].Value < sensorMidPoint - 100)
            //{
            //    pixels = (((sensorMidPoint - ifKit.sensors[1].Value) * 2) / 50);
            //}

            pixels = Convert.ToInt32(accelerationNumericUpDown.Value);

            //pixels = pixels * 2;

            //turn left and right
            if (ifKit.sensors[1].Value > sensorMidPoint + 70 + sensorModifier) //520 (upper range of resting mid point was 510)
            {
                //go left
                //sensorFloatingPoint_LeftRight = sensorFloatingPoint_LeftRight + (pixels);
                if (sv3.Position + pixels < 180)
                    sv3.Position += pixels;
                else
                    sv3.Position = 180;
            }
            else if (ifKit.sensors[1].Value < sensorMidPoint - 100 - sensorModifier) //425 (lower range of resting mid point was 415)
            {
                //go right
                //sensorFloatingPoint_LeftRight = sensorFloatingPoint_LeftRight - (pixels);
                if (sv3.Position - pixels > 0)
                    sv3.Position -= pixels;
                else
                    sv3.Position = 0;
            }
            else
            {
                turn_tmr.Stop();
            }
        }


        private static int _joylow = 400;
        private static int _joyhigh = 600;
        private static int sensorMidPoint = 450;
        private static int sensorFloatingPoint_UpDown = 450;
        private static int sensorFloatingPoint_LeftRight = 450;

        //left and right movement detection
        public System.Timers.Timer turn_tmr = new System.Timers.Timer();
        private static int _elapsedTimeLR = 10;

        //up and down movement detection
        public System.Timers.Timer vert_tmr = new System.Timers.Timer();
        private static int _elapsedTimeUD = 10;


        void ifKit_SensorChange(object sender, SensorChangeEventArgs e)
        {
            //sensorInArray[e.Index].Text = e.Value.ToString();

            //if (advSensorForm != null)
            //    advSensorForm.SetValue(e.Index, e.Value);

            xJoystickLabel.Text = ifKit.sensors[1].Value.ToString();
            yJoystickLabel.Text = ifKit.sensors[0].Value.ToString();

            try
            {
                if (overrideMovementCheckBox.Checked)
                {
                    switch (e.Index)
                    {
                        case 0: //Joystick Up and Down
                            {
                                //if (ifKit.sensors[0].Value < _joylow || ifKit.sensors[0].Value > _joyhigh)
                                //{
                                    //up down control timer
                                    if (vert_tmr.Enabled == false)
                                    {
                                        vert_tmr = new System.Timers.Timer();
                                        vert_tmr.Enabled = true;
                                        vert_tmr.Interval = _elapsedTimeUD;
                                        vert_tmr.Elapsed += new ElapsedEventHandler(vert_tmr_Elapsed);
                                        vert_tmr.Start();
                                    }
                                //}
                            }
                            break;
                        case 1: //Joystick Left and Right
                            {
                                //if (ifKit.sensors[1].Value < _joylow || ifKit.sensors[1].Value > _joyhigh)
                                //{
                                    //left right control timer
                                    if (turn_tmr.Enabled == false)
                                    {
                                        turn_tmr = new System.Timers.Timer();
                                        turn_tmr.Enabled = true;
                                        turn_tmr.Interval = _elapsedTimeLR;
                                        turn_tmr.Elapsed += new ElapsedEventHandler(turn_tmr_Elapsed);
                                        turn_tmr.Start();
                                    }
                                //}
                            }
                            break;
                    }
                }
            }
            catch
            {

            }
        }

        void ifKit_OutputChange(object sender, OutputChangeEventArgs e)
        {
            //digiOutDispArray[e.Index].Checked = e.Value;
        }

        void ifKit_InputChange(object sender, InputChangeEventArgs e)
        {
            //digiInArray[e.Index].Checked = e.Value;
        }

        void ifKit_Error(object sender, ErrorEventArgs e)
        {
            Phidget phid = (Phidget)sender;
            DialogResult result;
            //switch (e.Type)
            //{
            //    case PhidgetException.ErrorType.PHIDGET_ERREVENT_BADPASSWORD:
            //        phid.close();
            //        TextInputBox dialog = new TextInputBox("Error Event",
            //            "Authentication error: This server requires a password.", "Please enter the password, or cancel.");
            //        result = dialog.ShowDialog();
            //        if (result == DialogResult.OK)
            //            openCmdLine(phid, dialog.password);
            //        else
            //            Environment.Exit(0);
            //        break;
            //    case PhidgetException.ErrorType.PHIDGET_ERREVENT_PACKETLOST:
            //        //Ignore this error - it's not useful in this context.
            //        return;
            //    default:
            //        if (!errorBox.Visible)
            //            errorBox.Show();
            //        break;
            //}
            //errorBox.addMessage(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + ": " + e.Description);
        }

        void ifKit_Detach(object sender, DetachEventArgs e)
        {
            InterfaceKit ifKit = (InterfaceKit)sender;

            int i;
            for (i = 0; i < 16; i++)
            {
                //digiInArray[i].Visible = false;
                //digiInArray[i].Checked = false;
                //((Label)digitalInputsGroupBox.Controls["digitalInputLabel" + i.ToString()]).Visible = false;
            }
            for (i = 0; i < 16; i++)
            {
                //digiOutArray[i].Enabled = false;
                //digiOutArray[i].Visible = false;
                //digiOutDispArray[i].Visible = false;

                //((Label)digitalOutputsGroupBox.Controls["digitalOutputLabel" + i.ToString()]).Visible = false;
                //digiOutDispArray[i].Checked = false;
            }
            for (i = 0; i < 8; i++)
            {
                //sensorInArray[i].Visible = false;
                //sensorInArray[i].Text = "";
                //((Label)analogInputsGroupBox.Controls["analogInputLabel" + i.ToString()]).Visible = false;
            }
        }

        void ifKit_Attach(object sender, AttachEventArgs e)
        {
            InterfaceKit ifKit = (InterfaceKit)sender;
            //attachedTxt.Text = ifKit.Attached.ToString();
            //nameTxt.Text = ifKit.Name;
            //serialTxt.Text = ifKit.SerialNumber.ToString();
            //versionTxt.Text = ifKit.Version.ToString();
            //digiInNumTxt.Text = ifKit.inputs.Count.ToString();
            //digiOutNumTxt.Text = ifKit.outputs.Count.ToString();
            //sensorInNumTxt.Text = ifKit.sensors.Count.ToString();

            int i;
            for (i = 0; i < ifKit.inputs.Count; i++)
            {
                //digiInArray[i].Visible = true;
                //digiInArray[i].ForeColor = Color.Wheat;
                //((Label)digitalInputsGroupBox.Controls["digitalInputLabel" + i.ToString()]).Visible = true;
            }

            for (i = 0; i < ifKit.outputs.Count; i++)
            {
                //digiOutArray[i].Visible = true;
                ////digiOutArray[i].Checked = ifKit.outputs[i];
                //digiOutArray[i].Enabled = true;
                //digiOutDispArray[i].Visible = true;

                //((Label)digitalOutputsGroupBox.Controls["digitalOutputLabel" + i.ToString()]).Visible = true;
            }

            if (ifKit.sensors.Count > 0)
            {
                for (i = 0; i < ifKit.sensors.Count; i++)
                {
                    ifKit.sensors[i].Sensitivity = 10;
                    //sensorInArray[i].Visible = true;
                    //((Label)analogInputsGroupBox.Controls["analogInputLabel" + i.ToString()]).Visible = true;
                }
            }
        }

        //this is set in the attach event
        void SetSensorSensitivity(int value) //try "10"
        {
            try
            {
                for (int i = 0; i < ifKit.sensors.Count; i++)
                {
                    ifKit.sensors[i].Sensitivity = value;
                }
                //sensitivityTxt.Text = inputTrk.Value.ToString();
            }
            catch (PhidgetException ex)
            {
                MessageBox.Show(ex.Description);
            }
        }

        #endregion

        #region Phidgets Event Handlers

        void advServo_VelocityChange(object sender, VelocityChangeEventArgs e)
        {
            //if (e.Index == (int)servoCmb.SelectedItem)
            //{
            //    actual_velocityTxt.Text = e.Velocity.ToString();
            //}
        }

        void advServo_PositionChange(object sender, PositionChangeEventArgs e)
        {
            //if (e.Index == (int)servoCmb.SelectedItem)
            //{
            //    actual_positionTxt.Text = e.Position.ToString();
            //    stoppedCheckBox.Checked = advServo.servos[e.Index].Stopped;
            //}
        }

        void advServo_CurrentChange(object sender, CurrentChangeEventArgs e)
        {
            //if (e.Index == (int)servoCmb.SelectedItem)
            //{
            //    currentTxt.Text = e.Current.ToString();
            //}
        }

        void advServo_Error(object sender, ErrorEventArgs e)
        {
            Phidget phid = (Phidget)sender;
            DialogResult result;
            switch (e.Type)
            {
                case PhidgetException.ErrorType.PHIDGET_ERREVENT_BADPASSWORD:
                    phid.close();
                    //TextInputBox dialog = new TextInputBox("Error Event",
                    //    "Authentication error: This server requires a password.", "Please enter the password, or cancel.");
                    //result = dialog.ShowDialog();
                    //if (result == DialogResult.OK)
                    //    openCmdLine(phid, dialog.password);
                    //else
                    //    Environment.Exit(0);
                    break;
                default:
                    //if (!errorBox.Visible)
                    //    errorBox.Show();
                    break;
            }
            //errorBox.addMessage(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + ": " + e.Description);
        }

        void advServo_Detach(object sender, DetachEventArgs e)
        {
            AdvancedServo detached = (AdvancedServo)sender;

            sv1.Engaged = false;
            sv2.Engaged = false;
            sv3.Engaged = false;
            sv4.Engaged = false;
        }

        AdvancedServo attached;
        AdvancedServoServo sv1;
        AdvancedServoServo sv2;
        AdvancedServoServo sv3;
        AdvancedServoServo sv4;

        void advServo_Attach(object sender, AttachEventArgs e)
        {
            attached = (AdvancedServo)sender;

            //setup servos
            sv1 = attached.servos[0]; //HS322HD up/down
            sv2 = attached.servos[1]; //HITEC_815BB inspect up/down
            sv3 = attached.servos[2]; //HS322HD left right
            sv4 = attached.servos[3]; //HITEC_815BB raise up/down

            sv1.Type = ServoServo.ServoType.HITEC_HS322HD;
            sv2.Type = ServoServo.ServoType.HITEC_815BB;
            sv3.Type = ServoServo.ServoType.HITEC_HS322HD;
            sv4.Type = ServoServo.ServoType.HITEC_815BB;
        }

        #endregion

        private void ObjectTracking_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (displayTimer != null)
            {
                displayTimer.Stop();
                displayTimer = null;
            }

            if (CameraFPSTimer != null)
            {
                CameraFPSTimer.Stop();
                CameraFPSTimer = null;
            }

            cameraFeedVideoSourcePlayer.SignalToStop();
            cameraFeedVideoSourcePlayer.WaitForStop();

            advServo.Attach -= new AttachEventHandler(advServo_Attach);
            advServo.Detach -= new DetachEventHandler(advServo_Detach);
            advServo.Error -= new ErrorEventHandler(advServo_Error);

            advServo.CurrentChange -= new CurrentChangeEventHandler(advServo_CurrentChange);
            advServo.PositionChange -= new PositionChangeEventHandler(advServo_PositionChange);
            advServo.VelocityChange -= new VelocityChangeEventHandler(advServo_VelocityChange);

            ifKit.Attach -= new AttachEventHandler(ifKit_Attach);
            ifKit.Detach -= new DetachEventHandler(ifKit_Detach);
            ifKit.InputChange -= new InputChangeEventHandler(ifKit_InputChange);
            ifKit.OutputChange -= new OutputChangeEventHandler(ifKit_OutputChange);
            ifKit.SensorChange -= new SensorChangeEventHandler(ifKit_SensorChange);
            ifKit.Error -= new ErrorEventHandler(ifKit_Error);
            
            //run any events in the message queue - otherwise close will hang if there are any outstanding events
            Application.DoEvents();

            advServo.close();
            advServo = null;

            ifKit.close();
            ifKit = null;
        }

        #region Timed Event Handlers

        // SETS FPS ERROR RATE THINGY

        private void cameraFeedVideoSourcePlayer_NewFrame(object sender, ref Bitmap image)
        {
            //Bitmap capture = (Bitmap)image.Clone();
            CameraImage = (Bitmap)image.Clone();

            if (!selecting)
            {
                Bitmap capturedImage = (Bitmap)image.Clone();
                //objectToTrackSelectionPictureBox.Image = (Bitmap)image.Clone();

                //draw cross hairs
                Graphics g = Graphics.FromImage(capturedImage);
                Brush brush = new SolidBrush(Color.White);
                Pen pen = new Pen(brush);
                g.DrawLine(pen, new Point(160, 0), new Point(160, 240));
                g.DrawLine(pen, new Point(0, 120), new Point(320, 120));
                g.DrawImage(capturedImage, 0, 0);

                objectToTrackSelectionPictureBox.Image = capturedImage;
            }
            
            //if (capture != null)
            //{
            //    if (capture.Size != desiredSize)
            //        capture = Imaging.Resize(CameraImage, 320, 240, false);

            //    if (!selecting)
            //    {
            //        try
            //        {
            //            //this is what dies, when it dies it never comes back
            //            //if (capture != null)
            //            //    objectToTrackSelectionPictureBox.Image = capture;
            //            //objectToTrackSelectionPictureBox.Refresh();
            //            //objectToTrackSelectionPictureBox.Invalidate();
            //        }
            //        catch
            //        {

            //        }
            //    }

            //}

            try
            {
                if (processImageTimer == null)
                {
                    processImageTimer = new System.Timers.Timer();
                    //50 = 22-24 fps, 31 = instable, 32 = 28-30 fps, 33 = 22-24 fps
                    if (cameraListComboBox.SelectedIndex == 0) //logitech
                        processImageTimer.Interval = 33;
                    if (cameraListComboBox.SelectedIndex == 1) //lifecam
                        processImageTimer.Interval = 50;
                    processImageTimer.Elapsed += new ElapsedEventHandler(processImageTimer_Elapsed);
                    processImageTimer.Start();
                }
            }
            catch
            {

            }
        }

        private void displayTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Thread.Sleep(DisplaySeconds * 1000);
                messageLabel.Text = "";
                DisplaySeconds = 0;

                if (displayTimer != null)
                {
                    displayTimer.Stop();
                    displayTimer = null;
                }
            }
            catch
            {

            }
        }

        private void processImageTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (IsTracking)
                {
                    trackingCheckBox.Checked = true;

                    //captureImagePictureBox.Image = DetectObjects(DrawLiveMarker(CameraImage, Color.Green), true);
                    captureImagePictureBox.Image = DetectObjects(CameraImage, true);
                    AddFPSTime();

                    //if (!selecting)
                    //{
                    //    try
                    //    {
                    //        objectToTrackSelectionPictureBox.Image = CameraImage;
                    //    }
                    //    catch
                    //    {

                    //    }
                    //}
                }
                else
                {
                    trackingCheckBox.Checked = false;

                    captureImagePictureBox.Image = null;
                }
                //    captureImagePictureBox.Image = DetectObjects(DrawLiveMarker(CameraImage, Color.Red), true);



                //interesting, didn't really work that well
                //PasteMap pasteMap = new PasteMap(CameraImage);
                //captureImagePictureBox.Image = pasteMap.getProcessedImage;
            }
            catch
            {

            }
        }

        private void trackingButton_Click(object sender, EventArgs e)
        {
            if (trackingButton.Text == "Start Tracking")
            {
                FrameTimes = new List<DateTime>();

                Tracking(true);
                SetUITracking(true);

                trackingButton.Text = "Stop Tracking";

                overrideMovementCheckBox.Enabled = true;

                Message("Tracking started.",
                    MessageType.Success, 2);
            }
            else
            {
                FrameTimes = null;

                Tracking(false);
                SetUITracking(false);

                trackingButton.Text = "Start Tracking";

                overrideMovementCheckBox.Checked = false;
                overrideMovementCheckBox.Enabled = false;

                Message("Tracking stopped.",
                    MessageType.Informational, 2);
            }
        }

        private void cameraListComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConnectCamera(false);
            ConnectCamera(true);
        }
        
        private void devicePropertiesLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if ((VideoSource != null) && (VideoSource is VideoCaptureDevice))
            {
                try
                {
                    ((VideoCaptureDevice)VideoSource).DisplayPropertyPage(this.Handle);
                }
                catch (NotSupportedException)
                {
                    MessageBox.Show("The video source does not support configuration property page.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Event Handlers

        private void objectToTrackSelectionPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (objectSelectionCheckBox.Checked)
                    {
                        originalImage = new Bitmap(objectToTrackSelectionPictureBox.Image);

                        //keep track of where we started selecting
                        selecting = true;
                        x = e.X;
                        y = e.Y;
                    }
                }
            }
            catch
            { }
        }

        private void objectToTrackSelectionPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            //keep track of the dimensions we are selecting
            if (objectSelectionCheckBox.Checked)
            {
                if (selecting)
                {
                    width = e.X - x;
                    height = e.Y - y;
                    //call Paint on the picture box
                    objectToTrackSelectionPictureBox.Refresh();
                }
            }
        }

        private void objectToTrackSelectionPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            //set our selection to the picture box to show what we selected
            if (objectSelectionCheckBox.Checked)
            {
                selecting = false;
                CropImage();

                histogram = new Histogram((Bitmap)imageToTracjPictureBox.Image, Color.Black);
                colorToTrackPictureBox.Image = GetChannelAverageImage(histogram.RedAVG, histogram.GreenAVG, histogram.BlueAVG);

                trackingButton.Enabled = true;
                SetupTrackingColorFilters(histogram);
            }
        }

        private void objectToTrackSelectionPictureBox_Paint(object sender, PaintEventArgs e)
        {
            //draw our selection rectangle
            if (selecting)
            {
                Pen pen = Pens.GreenYellow;
                e.Graphics.DrawRectangle(pen, new Rectangle(x, y, width, height));
            }
        }

        #endregion

        private void overrideMovementCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //if (overrideMovementCheckBox.Checked)
            //{
            //    if (trackingButton.Enabled == false)
            //    {
            //        //tracking is running, so stop it
            //        trackingButton.Enabled = true;
            //        trackingButton_Click(sender, e);
            //    }
            //}
            //else
            //{

            //}
        }

        ////needs threads
        //System.Timers.Timer leftRightTimer = new System.Timers.Timer();
        //System.Timers.Timer upDownTimer = new System.Timers.Timer();

        private void objectToTrackSelectionPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            #region Fruitless
            
            ////don't do this if an object is being selected by click dragging
            ////if (!selecting)
            ////{
            //    int midX = CameraImage.Width / 2;
            //    int midY = CameraImage.Height / 2;

            //    if (e.Button == System.Windows.Forms.MouseButtons.Right)
            //    {
            //        ////move to
            //        //bool centerAchieved = false;

            //        //while (centerAchieved == false)
            //        //{
            //        int mouseX = e.Location.X;
            //        int mouseY = e.Location.Y;

            //        int[] normalizedPosition = NormalizeCoordinatesForServo(mouseX, mouseY, CameraImage.Width, CameraImage.Height);

            //        //sv3.Position += midX - mouseX; //= normalizedPosition[0];
            //        //sv1.Position = normalizedPosition[1];

            //        if (mouseX > midX)
            //        {
            //            //go right
            //            sv3.Position += midX - mouseX;
            //        }
            //        else if (mouseX < midX)
            //        {
            //            //go left
            //            sv3.Position -= midX - mouseX;
            //        }

            //        #region First Try

            //        ////recreate conditions for turn_tmr and vert_tmr but without the check on the sensor, check the click point while adding or subtracting
            //        ////to it until it is within some tolerance of the midX and midY

            //        ////leftRightTimer = new System.Timers.Timer();
            //        ////leftRightTimer.Enabled = true;
            //        ////leftRightTimer.Interval = 10;
            //        ////leftRightTimer.Elapsed += new ElapsedEventHandler(leftRightTimer_Elapsed);

            //        ////upDownTimer = new System.Timers.Timer();
            //        ////upDownTimer.Enabled = true;
            //        ////upDownTimer.Interval = 10;
            //        ////upDownTimer.Elapsed += new ElapsedEventHandler(upDownTimer_Elapsed);

            //        ////turn left or right
            //        //while (mouseX != midX)
            //        //{
            //        //    //modify click position
            //        //    if (mouseX > midX)
            //        //    {
            //        //        //sv3.Position = sv3.Position + (double)accelerationNumericUpDown.Value;
            //        //        moveX = Convert.ToInt32(accelerationNumericUpDown.Value) * -1;
            //        //        //leftRightTimer = new System.Timers.Timer();
            //        //        //leftRightTimer.Enabled = true;
            //        //        //leftRightTimer.Interval = 10;
            //        //        //leftRightTimer.Elapsed += new ElapsedEventHandler(leftRightTimer_Elapsed);
            //        //        if (leftRightTimer.Enabled == false)
            //        //        {
            //        //            leftRightTimer.Enabled = true;
            //        //            leftRightTimer.Start();
            //        //            mouseX = mouseX - moveX;
            //        //        }
            //        //    }
            //        //    else if (mouseX < midX)
            //        //    {
            //        //        //sv3.Position = sv3.Position - (double)accelerationNumericUpDown.Value;
            //        //        moveX = Convert.ToInt32(accelerationNumericUpDown.Value);
            //        //        //leftRightTimer = new System.Timers.Timer();
            //        //        //leftRightTimer.Enabled = true;
            //        //        //leftRightTimer.Interval = 10;
            //        //        //leftRightTimer.Elapsed += new ElapsedEventHandler(leftRightTimer_Elapsed);
            //        //        if (leftRightTimer.Enabled == false)
            //        //        {
            //        //            leftRightTimer.Enabled = true;
            //        //            leftRightTimer.Start();
            //        //            mouseX = mouseX + moveX;
            //        //        }
            //        //    }
            //        //}

            //        ////move up or down
            //        //while (mouseY != midY)
            //        //{
            //        //    //modify click position
            //        //    if (mouseY > midY)
            //        //    {
            //        //        //sv3.Position = sv3.Position + (double)accelerationNumericUpDown.Value;
            //        //        moveY = Convert.ToInt32(accelerationNumericUpDown.Value) * -1;
            //        //        //upDownTimer = new System.Timers.Timer();
            //        //        //upDownTimer.Enabled = true;
            //        //        //upDownTimer.Interval = 10;
            //        //        //upDownTimer.Elapsed += new ElapsedEventHandler(upDownTimer_Elapsed);
            //        //        if (upDownTimer.Enabled == false)
            //        //        {
            //        //            upDownTimer.Enabled = true;
            //        //            upDownTimer.Start();
            //        //            mouseY = mouseY - moveY;
            //        //        }
            //        //    }
            //        //    else if (mouseY < midY)
            //        //    {
            //        //        //sv3.Position = sv3.Position - (double)accelerationNumericUpDown.Value;
            //        //        moveY = Convert.ToInt32(accelerationNumericUpDown.Value);
            //        //        //upDownTimer = new System.Timers.Timer();
            //        //        //upDownTimer.Enabled = true;
            //        //        //upDownTimer.Interval = 10;
            //        //        //upDownTimer.Elapsed += new ElapsedEventHandler(upDownTimer_Elapsed);
            //        //        if (upDownTimer.Enabled == false)
            //        //        {
            //        //            upDownTimer.Enabled = true;
            //        //            upDownTimer.Start();
            //        //            mouseY = mouseY + moveY;
            //        //        }
            //        //    }
            //        //}

            //        #endregion

            //        //check for click position within midX and midY + tolerance of some kind
            //        //if (mouseX == midX && mouseY == midY)
            //        //    centerAchieved = true;
            //        //}
            //    }
            ////}

            #endregion

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                try
                {
                    //move then "shoot" if shoot is enabled on move

                    //this is close

                    //what I need to do is recognize 0-180:0-180 = 0-320:0-240 <-- normalize the values

                    //this cursor position thing does not work, otherwise this code might work
                    //the position of the mouse on screen
                    Point cursor = e.Location; //Cursor.Position;
                    //the picture box upper left corner
                    //Point pictureBox = objectToTrackSelectionPictureBox.Location;

                    int cursorX = cursor.X; // - pictureBox.X;
                    int cursorY = cursor.Y; // - pictureBox.Y;

                    int imageCenterX = CameraImage.Width / 2;
                    int imageCenterY = CameraImage.Height / 2;

                    int xToMove = cursorX - imageCenterX; //becomes negative when cursor position is over half way
                    int yToMove = cursorY - imageCenterY;

                    //int[] normalizedMoves = NormalizeCoordinatesForServo(xToMove, yToMove, CameraImage.Width, CameraImage.Height);

                    int[] newCoordinateValues = new int[2]; //"x" and "y"

                    //if (xToMove != 0 && yToMove != 0)
                    //{
                    double widthRatio = (180.0 / CameraImage.Width) / 2;
                    double heightRatio = (180.0 / CameraImage.Height) / 2;

                    double xNormalized = 0;
                    double yNormalized = 0;

                    //if (xToMove > 0)
                    xNormalized = xToMove * widthRatio;

                    //if (yToMove > 0)
                    yNormalized = yToMove * heightRatio;

                    newCoordinateValues[0] = Convert.ToInt32(xNormalized);
                    newCoordinateValues[1] = Convert.ToInt32(yNormalized);
                    //}
                    //else
                    //{
                    //    newCoordinateValues[0] = CameraImage.Width / 2;
                    //    newCoordinateValues[1] = CameraImage.Height / 2;
                    //}

                    int x = newCoordinateValues[0] / 2; // normalizedMoves[0];
                    int y = newCoordinateValues[1] / 2; // normalizedMoves[1];

                    if (sv3 != null && sv1 != null)
                    {
                        if (sv3.Position + x > 180)
                            x = 180 - Convert.ToInt32(sv3.Position) - x; //gets movement right to 180
                        else if (sv3.Position + x < 0)
                            x = Convert.ToInt32(sv3.Position); //gets to zero

                        if (sv1.Position + y > 180)
                            y = 180 - Convert.ToInt32(sv1.Position) - y;
                        else if (sv1.Position + y < 0)
                            y = Convert.ToInt32(sv1.Position);

                        sv3.Position += x;
                        sv1.Position += y;
                    }
                }
                catch
                { }
            }
        }

        private void objectToTrackSelectionPictureBox_DoubleClick(object sender, EventArgs e)
        {
            
        }

        bool upPressed = false;
        bool downPressed = false;
        bool leftPressed = false;
        bool rightPressed = false;

        private void ObjectTracking_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //which key was pressed?
            //start timer for movement
            if (moveByKeysCheckBox.Checked)
            {
                switch (e.KeyData)
                {
                    case Keys.W: //up
                        {
                            upPressed = true;
                            while (upPressed)
                            {
                                try
                                {
                                    int movePositionCount = Convert.ToInt32(accelerationNumericUpDown.Value);
                                    sv3.Position += movePositionCount;
                                }
                                catch
                                { }
                            }
                        }
                        break;
                    case Keys.S: //down
                        {
                            downPressed = true;
                            while (downPressed)
                            {
                                try
                                {
                                    int movePositionCount = Convert.ToInt32(accelerationNumericUpDown.Value);
                                    sv3.Position -= movePositionCount;
                                }
                                catch
                                { }
                            }
                        }
                        break;
                    case Keys.A: //left
                        {
                            leftPressed = true;
                            while (leftPressed)
                            {
                                try
                                {
                                    int movePositionCount = Convert.ToInt32(accelerationNumericUpDown.Value);
                                    sv1.Position += movePositionCount;
                                }
                                catch
                                { }
                            }
                        }
                        break;
                    case Keys.D: //right
                        {
                            rightPressed = true;
                            while (rightPressed)
                            {
                                try
                                {
                                    int movePositionCount = Convert.ToInt32(accelerationNumericUpDown.Value);
                                    sv1.Position -= movePositionCount;
                                }
                                catch
                                { }
                            }
                        }
                        break;
                }
            }
        }

        private void ObjectTracking_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //which key was pressed?
            //stop timer for movement
            switch (e.KeyData)
            {
                case Keys.W: //up
                    {
                        upPressed = false;
                    }
                    break;
                case Keys.S: //down
                    {
                        downPressed = false;
                    }
                    break;
                case Keys.A: //left
                    {
                        leftPressed = false;
                    }
                    break;
                case Keys.D: //right
                    {
                        rightPressed = false;
                    }
                    break;
            }
        }

        private void moveByKeysCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (moveByKeysCheckBox.Checked)
            {
                objectSelectionCheckBox.Checked = false;
                overrideMovementCheckBox.Checked = true;
            }
            else
            {
                objectSelectionCheckBox.Checked = true;
                overrideMovementCheckBox.Checked = false;
            }

            moveByKeysCheckBox.Focus();
        }

        private void moveByKeysCheckBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (moveByKeysCheckBox.Checked)
            {
                horizontalTimer = new System.Timers.Timer();
                horizontalTimer.AutoReset = true;
                horizontalTimer.Interval = 20;
                horizontalTimer.Enabled = true;
                horizontalTimer.Elapsed += new ElapsedEventHandler(horizontalTimer_Elapsed);

                verticalTimer = new System.Timers.Timer();
                verticalTimer.AutoReset = true;
                verticalTimer.Interval = 20;
                verticalTimer.Enabled = true;
                verticalTimer.Elapsed += new ElapsedEventHandler(verticalTimer_Elapsed);

                switch (e.KeyData)
                {
                    case Keys.W: //up
                        {
                            vertMoveBy = Convert.ToInt32(accelerationNumericUpDown.Value) * -1;
                            MoveVertical();
                        }
                        break;
                    case Keys.S: //down
                        {
                            vertMoveBy = Convert.ToInt32(accelerationNumericUpDown.Value);
                            MoveVertical();
                        }
                        break;
                    case Keys.A: //left
                        {
                            horizMoveBy = Convert.ToInt32(accelerationNumericUpDown.Value) * -1;
                            MoveHorizontal();
                        }
                        break;
                    case Keys.D: //right
                        {
                            horizMoveBy = Convert.ToInt32(accelerationNumericUpDown.Value);
                            MoveHorizontal();
                        }
                        break;
                }
            }
        }

        private void moveByKeysCheckBox_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //switch (e.KeyData)
            //{
            //    case Keys.W: //up
            //        {
            //            upPressed = false;
            //        }
            //        break;
            //    case Keys.S: //down
            //        {
            //            downPressed = false;
            //        }
            //        break;
            //    case Keys.A: //left
            //        {
            //            leftPressed = false;
            //        }
            //        break;
            //    case Keys.D: //right
            //        {
            //            rightPressed = false;
            //        }
            //        break;
            //}
        }

        System.Timers.Timer verticalTimer;
        int vertMoveBy;
        System.Timers.Timer horizontalTimer;
        int horizMoveBy;

        private void MoveVertical()
        {
            try
            {
                //verticalTimer = new System.Timers.Timer();
                //verticalTimer.AutoReset = true;
                //verticalTimer.Interval = 20;
                verticalTimer.Enabled = true;
                //verticalTimer.Elapsed += new ElapsedEventHandler(verticalTimer_Elapsed);
                verticalTimer.Start();
            }
            catch { }
        }

        private void MoveHorizontal()
        {
            try
            {
                //horizontalTimer = new System.Timers.Timer();
                //horizontalTimer.AutoReset = true;
                //horizontalTimer.Interval = 20;
                horizontalTimer.Enabled = true;
                //horizontalTimer.Elapsed += new ElapsedEventHandler(horizontalTimer_Elapsed);
                horizontalTimer.Start();
            }
            catch { }
        }
        
        void verticalTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Keyboard.IsKeyDown(Keys.W) == true || Keyboard.IsKeyDown(Keys.S) == true)
            {
                int currentPosition = Convert.ToInt32(sv1.Position);
                if ((currentPosition + vertMoveBy) >= 0 && (currentPosition + vertMoveBy) <= 180)
                {
                    sv1.Position = currentPosition + vertMoveBy;
                    verticalTimer.Stop();
                    MoveVertical();
                }
            }
            else
            {
                verticalTimer.Stop();
            }
        }

        void horizontalTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Keyboard.IsKeyDown(Keys.A) == true || Keyboard.IsKeyDown(Keys.D) == true)
            {
                int currentPosition = Convert.ToInt32(sv3.Position);
                if ((currentPosition + horizMoveBy) >= 0 && (currentPosition + horizMoveBy) <= 180)
                {
                    sv3.Position = currentPosition + horizMoveBy;
                    horizontalTimer.Stop();
                    MoveHorizontal();
                }
            }
            else
            {
                horizontalTimer.Stop();
            }
        }

        //void upDownTimer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    sv1.Position = sv1.Position + moveY;
        //    upDownTimer.Stop();
        //}

        //void leftRightTimer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    sv3.Position  = sv3.Position + moveX;
        //    //System.Threading.Thread.Sleep(500);
        //    leftRightTimer.Stop();
        //}
    }

    #region Supporting Classes

    public abstract class Keyboard
    {
        [Flags]
        private enum KeyStates
        {
            Up = 0,
            Down = 1,
            Toggled = 2
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetKeyState(int keyCode);

        private static KeyStates GetKeyState(Keys key)
        {
            KeyStates state = KeyStates.Up;

            short retVal = GetKeyState((int)key);

            //If the high-order bit is 1, the key is down
            //otherwise, it is up.
            if ((retVal & 0x8000) == 0x8000)
                state |= KeyStates.Down;

            //If the low-order bit is 1, the key is toggled.
            if ((retVal & 1) == 1)
                state |= KeyStates.Toggled; //numlock, scroll-lock and caps lock

            return state;
        }

        public static bool IsKeyDown(Keys key)
        {
            return KeyStates.Down == (GetKeyState(key) & KeyStates.Down);
        }

        public static bool IsKeyToggled(Keys key)
        {
            return KeyStates.Toggled == (GetKeyState(key) & KeyStates.Toggled);
        }
    }

    class Pixel
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }

        public Pixel(int red, int green, int blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public bool DoesMatch(int red, int green, int blue)
        {
            if (Red == red &&
                Green == green &&
                Blue == blue)
                return true;
            else
                return false;
        }
    }

    class Histogram
    {
        public int[] Red { get; set; }
        public int[] Green { get; set; }
        public int[] Blue { get; set; }
        public int[] Luminosity { get; set; }
        public int[] RGB { get; set; }

        public Bitmap RedHistogram { get; set; }
        public Bitmap GreenHistogram { get; set; }
        public Bitmap BlueHistogram { get; set; }
        public Bitmap LuminosityHistogram { get; set; }
        public Bitmap RGBHistogram { get; set; }

        public int[] RedHistogramData { get; set; }
        public int[] GreenHistogramData { get; set; }
        public int[] BlueHistogramData { get; set; }
        public int[] RGBHistogramData { get; set; }

        public int RedAVG { get; set; }
        public int GreenAVG { get; set; }
        public int BlueAVG { get; set; }

        public Bitmap RedChannelImage { get; set; }
        public Bitmap GreenChannelImage { get; set; }
        public Bitmap BlueChannelImage { get; set; }

        public Bitmap BlackChannelImage { get; set; }
        public Bitmap WhiteChannelImage { get; set; }
        public Bitmap BlackAndWhiteImage { get; set; }

        public Bitmap OriginalImage { get; set; }

        public List<KeyValuePair<Pixel, int>> Pixels { get; set; }

        public Histogram(Bitmap bitmap, System.Drawing.Color color)
        {
            OriginalImage = bitmap;

            InitializeChannelData();
            Load(bitmap);

            RedHistogram = HistogramImage(Red, color);
            GreenHistogram = HistogramImage(Green, color);
            BlueHistogram = HistogramImage(Blue, color);
            LuminosityHistogram = HistogramImage(Luminosity, color);
            RGBHistogram = HistogramImage(RGB, color);
        }

        private void InitializeChannelData()
        {
            Red = new int[256];
            Green = new int[256];
            Blue = new int[256];
            Luminosity = new int[256];
            RGB = new int[256];

            //set the initial values for the arrays
            for (int i = 0; i < 256; i++)
            {
                Red[i] = 0;
                Green[i] = 0;
                Blue[i] = 0;
                Luminosity[i] = 0;
                RGB[i] = 0;
            }
        }

        private void Load(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentException("The image passed in cannot be null.");

            //setup the channel bitmaps
            RedChannelImage = new Bitmap(bitmap);
            GreenChannelImage = new Bitmap(bitmap);
            BlueChannelImage = new Bitmap(bitmap);

            BlackChannelImage = new Bitmap(bitmap);
            WhiteChannelImage = new Bitmap(bitmap);
            BlackAndWhiteImage = new Bitmap(bitmap);

            //get the pixel array data so we can loop through it
            BitmapData bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            BitmapData redBitmapData = RedChannelImage.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData greenBitmapData = GreenChannelImage.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData blueBitmapData = BlueChannelImage.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            BitmapData blackBitmapData = BlackChannelImage.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData whiteBitmapData = WhiteChannelImage.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData blackAndWhiteBitmapData = BlackAndWhiteImage.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            //average color var [r, g, b]
            int[] totals = new int[] { 0, 0, 0 };

            unsafe
            {
                //setup the starting pixel in the bitmap data and the width of that data in the array
                byte* scanLinePixel = (byte*)bitmapData.Scan0;
                int bitmapStride = bitmapData.Stride - bitmapData.Width * 3;

                byte* redScanLinePixel = (byte*)redBitmapData.Scan0;
                byte* greenScanLinePixel = (byte*)greenBitmapData.Scan0;
                byte* blueScanLinePixel = (byte*)blueBitmapData.Scan0;

                byte* blackScanLinePixel = (byte*)blackBitmapData.Scan0;
                byte* whiteScanLinePixel = (byte*)whiteBitmapData.Scan0;
                byte* blackAndWhiteScanLinePixel = (byte*)blackAndWhiteBitmapData.Scan0;

                Pixels = new List<KeyValuePair<Pixel, int>>();

                //loop though the data from top to bottom then within each line from left to right
                for (int height = 0; height < bitmapData.Height; height++)
                {
                    for (int width = 0; width < bitmapData.Width; width++)
                    {
                        //get the values of our pixel channels
                        int mean = (int)(0.114 * scanLinePixel[0] + 0.587 * scanLinePixel[1] + 0.299 * scanLinePixel[2]);
                        int rgbAverage = (scanLinePixel[0] + scanLinePixel[1] + scanLinePixel[2]) / 3;
                        int red = scanLinePixel[2];
                        int green = scanLinePixel[1];
                        int blue = scanLinePixel[0];

                        //Pixels list
                        Pixel pixel = new Pixel(red, green, blue);

                        //go through our list (took 36 seconds to do this...NOT counting the internal processing)
                        //for (int i = 0; i < Pixels.Count; i++)
                        //{

                        //        //KeyValuePair<Pixel, int> pixelCount = Pixels[i];

                        //        ////if the current pixel matches one we already have
                        //        //if (pixel.DoesMatch(pixelCount.Key.Red, pixelCount.Key.Green, pixelCount.Key.Blue))
                        //        //{
                        //        //    ////incriment the count
                        //        //    //int count = pixelCount.Value;
                        //        //    //Pixels.Remove(pixelCount);
                        //        //    //Pixels.Add(new KeyValuePair<Pixel, int>(pixel, count + 1));
                        //        //}
                        //        //else
                        //        //{
                        //        //    //otherwise add it to the list
                        //        //    //Pixels.Add(new KeyValuePair<Pixel, int>(pixel, 1));
                        //        //}
                        //    }

                        //    //foreach (KeyValuePair<Pixel, int> pixelCount in Pixels)
                        //    //{
                        //    //    //if the current pixel matches one we already have
                        //    //    if (pixel.DoesMatch(pixelCount.Key.Red, pixelCount.Key.Green, pixelCount.Key.Blue))
                        //    //    {
                        //    //        //incriment the count
                        //    //        int count = pixelCount.Value;
                        //    //        Pixels.Remove(pixelCount);
                        //    //        Pixels.Add(new KeyValuePair<Pixel, int>(pixel, count + 1));
                        //    //    }
                        //    //    else
                        //    //    {
                        //    //        //otherwise add it to the list
                        //    //        Pixels.Add(new KeyValuePair<Pixel, int>(pixel, 1));
                        //    //    }

                        //}

                        Pixels.Add(new KeyValuePair<Pixel, int>(pixel, 1));

                        //incriment the position within the array if that pixel value (=position) is found, not sure I explained that right...
                        Luminosity[mean]++;
                        Red[red]++;
                        Green[green]++;
                        Blue[blue]++;
                        RGB[rgbAverage]++;

                        //add value to totals so we can average later
                        totals[0] += red;
                        totals[1] += green;
                        totals[2] += blue;

                        //set values for our channel bitmaps
                        redScanLinePixel[0] = scanLinePixel[0];
                        redScanLinePixel[1] = 0;
                        redScanLinePixel[2] = 0;

                        greenScanLinePixel[0] = 0;
                        greenScanLinePixel[1] = scanLinePixel[1];
                        greenScanLinePixel[2] = 0;

                        blueScanLinePixel[0] = 0;
                        blueScanLinePixel[1] = 0;
                        blueScanLinePixel[2] = scanLinePixel[2];

                        //b&w
                        int pixelAVG = Convert.ToInt32((red + green + blue) / 3);
                        //this cuts it at 50%
                        if (pixelAVG > 127) //a white pixel
                        {
                            blackScanLinePixel[0] = 255;
                            blackScanLinePixel[1] = 255;
                            blackScanLinePixel[2] = 255;

                            whiteScanLinePixel[0] = 0;
                            whiteScanLinePixel[1] = 0;
                            whiteScanLinePixel[2] = 0;
                        }
                        else //a black pixel
                        {
                            blackScanLinePixel[0] = 0;
                            blackScanLinePixel[1] = 0;
                            blackScanLinePixel[2] = 0;

                            whiteScanLinePixel[0] = 255;
                            whiteScanLinePixel[1] = 255;
                            whiteScanLinePixel[2] = 255;
                        }

                        //sets our greyscale image (ie: black and white)
                        blackAndWhiteScanLinePixel[0] =
                            blackAndWhiteScanLinePixel[1] =
                            blackAndWhiteScanLinePixel[2] =
                                (byte)(.299 * red + .587 * green + .114 * blue);

                        //next pixel to the right
                        scanLinePixel += 3;
                        redScanLinePixel += 3;
                        greenScanLinePixel += 3;
                        blueScanLinePixel += 3;
                        blackScanLinePixel += 3;
                        whiteScanLinePixel += 3;
                        blackAndWhiteScanLinePixel += 3;
                    }

                    //next line down
                    scanLinePixel += bitmapStride;
                    redScanLinePixel += bitmapStride;
                    greenScanLinePixel += bitmapStride;
                    blueScanLinePixel += bitmapStride;
                    blackScanLinePixel += bitmapStride;
                    whiteScanLinePixel += bitmapStride;
                    blackAndWhiteScanLinePixel += bitmapStride;
                }
            }
            bitmap.UnlockBits(bitmapData);
            RedChannelImage.UnlockBits(redBitmapData);
            GreenChannelImage.UnlockBits(greenBitmapData);
            BlueChannelImage.UnlockBits(blueBitmapData);
            BlackChannelImage.UnlockBits(blackBitmapData);
            WhiteChannelImage.UnlockBits(whiteBitmapData);
            BlackAndWhiteImage.UnlockBits(blackAndWhiteBitmapData);

            //set the average color calculations
            int size = bitmap.Width * bitmap.Height;
            RedAVG = totals[0] / size;
            GreenAVG = totals[1] / size;
            BlueAVG = totals[2] / size;

            RedHistogramData = Red;
            GreenHistogramData = Green;
            BlueHistogramData = Blue;
            RGBHistogramData = RGB;
        }

        private Bitmap HistogramImage(int[] channel, System.Drawing.Color color)
        {
            if (channel == null)
                throw new ArgumentException("The int array 'channel' cannot be null. Be sure to call the method CreateHistogram before calling this method.");

            //set the size to 256 for the possible values of 0 to 255 of pixel data
            Bitmap bitmap = new Bitmap(256, 256);

            //find the max value within the channel
            int maxValue = 0;

            foreach (int value in channel)
            {
                if (value > maxValue)
                    maxValue = value;
            }

            //get the ratio to normalize with
            double ratio = 256.0 / maxValue;

            //setup the image we draw to
            Graphics graphics = Graphics.FromImage(bitmap);
            Pen pen = new Pen(color, 1); //pixel width of "1"

            //loop through the passed in array and clamp the values within the bounds of our image/array of 256 (height)
            for (int i = 0; i < 256; i++)
            {
                int channelValue = channel[i];
                double clampedValue = 0;

                //normalize (or "clamp") the value by multiplying the value by the ration created by dividing the desired max of 256 by the maximum value we found in the array
                if (maxValue > 0)
                    clampedValue = channelValue * ratio;

                //set the values for the line we draw
                Point bottom = new Point(i, 0);
                Point top = new Point(i, Convert.ToInt32(clampedValue));

                //draw it
                graphics.DrawLine(pen, bottom, top);
            }

            //draw out our new image 
            graphics.DrawImage(bitmap, new Rectangle(0, 0, 256, 256));

            //flip it
            System.Drawing.Image image = bitmap;
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            bitmap = new Bitmap(image);

            return bitmap;
        }

        public Color[] ThreeMostProminantColors()
        {
            Color[] colors = new Color[] { Color.Black, Color.Black, Color.Black };

            #region Notes

            //var topThree = RGBHistogramData.OrderByDescending(i => i).Take(3);

            //normalize histograms or just loop through them?
            //get top 3 but will they all be in the same position? does it matter?

            //loop/step every X number and record the average, keep track of 3 averages and if a higher is found replace the lowest of the 
            //3 with the new higher average, then you have the 3 most common of each color channel
            //the problem still exists where the channels don't match up

            //int[] redCommon = new int[] {0, 0, 0};
            //int[] greenCommon = new int[] { 0, 0, 0 };
            //int[] blueCommon = new int[] { 0, 0, 0 };

            //for (int i = 0; i < Red.Length; i++)
            //{
            //    int redValue = Red[i];
            //    int leastSoFar = redCommon.Min<int>();

            //    //if we found one greater than the least on we have so far
            //    if (redValue > leastSoFar)
            //    {
            //        //find the least
            //        for (int j = 0; j < redCommon.Length; j++)
            //        {
            //            if (redCommon[j] == leastSoFar)
            //            {
            //                //and replace it
            //                redCommon[j] = redValue;
            //                break; //stop after first on replaced
            //            }
            //        }
            //    }
            //}

            //colors[0] = Color.FromArgb(redCommon[0], 0, 0);

            //

            //i could loop through the grb histo and find the 3 most common then loop through the pixels and find the positions of the 3 most common
            //based on the indeces of the most common colors in the full histo...

            //NO NO NO

            //int[] mostCommonPixels = new int[] { 0, 0, 0 };

            //for (int i = 0; i < RGBHistogramData.Length; i++)
            //{
            //    int value = RGBHistogramData[i];
            //    int leastSoFar = mostCommonPixels.Min<int>();

            //    //if we found one greater than the least on we have so far
            //    if (value > leastSoFar)
            //    {
            //        //find the least
            //        for (int j = 0; j < mostCommonPixels.Length; j++)
            //        {
            //            if (mostCommonPixels[j] == leastSoFar)
            //            {
            //                //and replace it
            //                mostCommonPixels[j] = value;
            //                break; //stop after first on replaced
            //            }
            //        }
            //    }
            //}

            //YES YES YES

            #endregion

            //get the pixel array data so we can loop through it
            BitmapData bitmapData = OriginalImage.LockBits(new System.Drawing.Rectangle(0, 0, OriginalImage.Width, OriginalImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            unsafe
            {
                //setup the starting pixel in the bitmap data and the width of that data in the array
                byte* scanLinePixel = (byte*)bitmapData.Scan0;
                int bitmapStride = bitmapData.Stride - bitmapData.Width * 3;

                //loop though the data from top to bottom then within each line from left to right
                for (int height = 0; height < bitmapData.Height; height++)
                {
                    for (int width = 0; width < bitmapData.Width; width++)
                    {
                        //get the values of our pixel channels
                        int red = scanLinePixel[2];
                        int green = scanLinePixel[1];
                        int blue = scanLinePixel[0];

                        //next pixel to the right
                        scanLinePixel += 3;
                    }

                    //next line down
                    scanLinePixel += bitmapStride;
                }
            }

            OriginalImage.UnlockBits(bitmapData);

            return colors;
        }
    }

    public static class Imaging
    {
        public static bool IsNull(object parameter)
        {
            bool isNull = true;

            if (parameter != null)
                isNull = false;

            return isNull;
        }

        public class ConvolutionMatrix
        {
            public int TopLeft = 0, TopMid = 0, TopRight = 0;
            public int MidLeft = 0, Pixel = 1, MidRight = 0;
            public int BottomLeft = 0, BottomMid = 0, BottomRight = 0;
            public int Factor = 1;
            public int Offset = 0;
            public void SetAll(int value)
            {
                TopLeft = TopMid = TopRight = MidLeft = Pixel = MidRight = BottomLeft = BottomMid = BottomRight = value;
            }
        }

        private static Bitmap Conv3x3(Bitmap bitmap, ConvolutionMatrix matrix)
        {
            // Avoid divide by zero errors
            if (0 == matrix.Factor) return null;

            Bitmap bSrc = (Bitmap)bitmap.Clone();

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            int stride2 = stride * 2;
            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr SrcScan0 = bmSrc.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pSrc = (byte*)(void*)SrcScan0;

                int nOffset = stride - bitmap.Width * 3;
                int nWidth = bitmap.Width - 2;
                int nHeight = bitmap.Height - 2;

                int nPixel;

                for (int y = 0; y < nHeight; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {
                        nPixel = ((((pSrc[2] * matrix.TopLeft) + (pSrc[5] * matrix.TopMid) + (pSrc[8] * matrix.TopRight) +
                            (pSrc[2 + stride] * matrix.MidLeft) + (pSrc[5 + stride] * matrix.Pixel) + (pSrc[8 + stride] * matrix.MidRight) +
                            (pSrc[2 + stride2] * matrix.BottomLeft) + (pSrc[5 + stride2] * matrix.BottomMid) + (pSrc[8 + stride2] * matrix.BottomRight)) / matrix.Factor) + matrix.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[5 + stride] = (byte)nPixel;

                        nPixel = ((((pSrc[1] * matrix.TopLeft) + (pSrc[4] * matrix.TopMid) + (pSrc[7] * matrix.TopRight) +
                            (pSrc[1 + stride] * matrix.MidLeft) + (pSrc[4 + stride] * matrix.Pixel) + (pSrc[7 + stride] * matrix.MidRight) +
                            (pSrc[1 + stride2] * matrix.BottomLeft) + (pSrc[4 + stride2] * matrix.BottomMid) + (pSrc[7 + stride2] * matrix.BottomRight)) / matrix.Factor) + matrix.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[4 + stride] = (byte)nPixel;

                        nPixel = ((((pSrc[0] * matrix.TopLeft) + (pSrc[3] * matrix.TopMid) + (pSrc[6] * matrix.TopRight) +
                            (pSrc[0 + stride] * matrix.MidLeft) + (pSrc[3 + stride] * matrix.Pixel) + (pSrc[6 + stride] * matrix.MidRight) +
                            (pSrc[0 + stride2] * matrix.BottomLeft) + (pSrc[3 + stride2] * matrix.BottomMid) + (pSrc[6 + stride2] * matrix.BottomRight)) / matrix.Factor) + matrix.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[3 + stride] = (byte)nPixel;

                        p += 3;
                        pSrc += 3;
                    }
                    p += nOffset;
                    pSrc += nOffset;
                }
            }

            bitmap.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);

            return bitmap;
        }

        public static Bitmap GrayScale(Bitmap bitmap)
        {
            if (IsNull(bitmap))
                throw new ArgumentException("Bitmap cannot be null.");

            try
            {
                // GDI+ still lies to us - the return format is BGR, NOT RGB.
                BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;

                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;

                    int nOffset = stride - bitmap.Width * 3;

                    byte red, green, blue;
                    int nWidth = bitmap.Width;
                    int nHeight = bitmap.Height;

                    for (int y = 0; y < nHeight; ++y)
                    {
                        for (int x = 0; x < nWidth; ++x)
                        {
                            blue = p[0];
                            green = p[1];
                            red = p[2];

                            p[0] = p[1] = p[2] = (byte)(.299 * red + .587 * green + .114 * blue);

                            p += 3;
                        }
                        p += nOffset;
                    }
                }

                bitmap.UnlockBits(bmData);
            }
            catch
            {
                throw;
            }

            GC.Collect();

            return bitmap;
        }

        public static Bitmap GaussianBlur(Bitmap bitmap, byte blur)
        {
            if (IsNull(bitmap))
                throw new ArgumentException("Bitmap cannot be null.");

            try
            {
                ConvolutionMatrix m = new ConvolutionMatrix();
                m.SetAll(1);
                m.Pixel = blur;
                m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = 2;
                m.Factor = blur + 12;

                bitmap = Conv3x3(bitmap, m);
            }
            catch
            {
                throw;
            }

            GC.Collect();

            return bitmap;
        }

        public static Bitmap Resize(Bitmap bitmap, int width, int height, bool useBilinear)
        {
            //if (IsNull(bitmap))
            //    throw new ArgumentException("Bitmap cannot be null.");

            if (bitmap != null)
            {
                try
                {
                    Bitmap bTemp = (Bitmap)bitmap.Clone();
                    bitmap = new Bitmap(width, height, bTemp.PixelFormat);

                    double nXFactor = (double)bTemp.Width / (double)width;
                    double nYFactor = (double)bTemp.Height / (double)height;

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
                    //throw;
                }

                GC.Collect();
            }

            return bitmap;

        }

        public static Bitmap Crop(Bitmap bitmap, Rectangle rectangle)
        {
            if (IsNull(bitmap))
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
                //throw;
            }

            GC.Collect();

            return bitmap;
        }

        public static Bitmap Crop2(Bitmap myBitmap, int nX, int nY, int nW, int nH)
        {
            if (nX < 0 || nX >= nW || nW > myBitmap.Width || nY < 0 || nY >= nH || nH > myBitmap.Height)
                throw new ArgumentException();
            Rectangle compressionRectangle = new Rectangle(nX, nY, nW, nH);
            return myBitmap.Clone(compressionRectangle, System.Drawing.Imaging.PixelFormat.DontCare);
        }
    }

    //interesting, didn't really work that well
    public class PasteMap
    {
        private Bitmap image;
        private Bitmap processedImage;
        private Rectangle[] rectangels;

        private void init(Bitmap image)
        {
            this.image = image;
        }

        public PasteMap(Bitmap bitmap)
        {
            init(bitmap);
            process();
        }

        public void process()
        {
            processedImage = image;
            processedImage = applyFilters(processedImage);
            processedImage = filterWhite(processedImage);
            rectangels = extractRectangles(processedImage);
            //rectangels = filterRectangles(rectangels); 
            processedImage = drawRectangelsToImage(processedImage, rectangels);
        }

        public Bitmap getProcessedImage
        {
            get
            {
                return processedImage;
            }
        }

        public Rectangle[] getRectangles
        {
            get
            {
                return rectangels;
            }
        }

        private Bitmap applyFilters(Bitmap image)
        {
            image = new ContrastCorrection(2).Apply(image);
            image = new GaussianBlur(10, 10).Apply(image);
            return image;
        }

        private Bitmap filterWhite(Bitmap image)
        {
            Bitmap test = new Bitmap(image.Width, image.Height);

            for (int width = 0; width < image.Width; width++)
            {
                for (int height = 0; height < image.Height; height++)
                {
                    if (image.GetPixel(width, height).R > 200 &&
                        image.GetPixel(width, height).G > 200 &&
                        image.GetPixel(width, height).B > 200)
                    {
                        test.SetPixel(width, height, Color.White);
                    }
                    else
                        test.SetPixel(width, height, Color.Black);
                }
            }
            return test;
        }

        private Rectangle[] extractRectangles(Bitmap image)
        {
            BlobCounter bc = new BlobCounter();
            bc.FilterBlobs = true;
            bc.MinWidth = 5;
            bc.MinHeight = 5;
            // process binary image 
            bc.ProcessImage(image);
            Blob[] blobs = bc.GetObjects(image, false);
            // process blobs 
            List<Rectangle> rects = new List<Rectangle>();
            foreach (Blob blob in blobs)
            {
                if (blob.Area > 1000)
                {
                    rects.Add(blob.Rectangle);
                }
            }

            return rects.ToArray();
        }

        private Rectangle[] filterRectangles(Rectangle[] rects)
        {
            List<Rectangle> Rectangles = new List<Rectangle>();
            foreach (Rectangle rect in rects)
            {
                if (rect.Width > 75 && rect.Height > 75)
                    Rectangles.Add(rect);
            }

            return Rectangles.ToArray();
        }

        private Bitmap drawRectangelsToImage(Bitmap image, Rectangle[] rects)
        {
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            foreach (Rectangle rect in rects)
                Drawing.FillRectangle(data, rect, Color.Red);
            image.UnlockBits(data);
            return image;
        }
    } 

    #endregion
}

