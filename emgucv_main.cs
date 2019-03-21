using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.Cuda;
using FaceDetection;

namespace Nhan_dang_khuon_mat
{
    public partial class Form1 : Form
    {
        //declaring global variables
        private Capture capture = null;        //takes images from camera as image frames
        private bool captureInProgress; // checks if capture is executing
        
        
        public Form1()
        {
            InitializeComponent();
            CvInvoke.UseOpenCL = false;
            try
            {
                capture = new Capture();
                capture.ImageGrabbed += ProcessFrame;
            }
            catch (NullReferenceException excpt)
            {
                MessageBox.Show(excpt.Message);
            }
        }
        
        
        private void ProcessFrame(object sender, EventArgs arg)
        {
            Mat frame = new Mat();
            capture.Retrieve(frame, 0);
            Mat image = frame; //Read the files as an 8-bit Bgr image  
            long detectionTime;
            List faces = new List();
            List eyes = new List();
            //The cuda cascade classifier doesn't seem to be able to load "haarcascade_frontalface_default.xml" file in this release
            //disabling CUDA module for now
            bool tryUseCuda = false;
            bool tryUseOpenCL = true;
            DetectFace.Detect(
              image, "haarcascade_frontalface_default.xml", "haarcascade_eye.xml",
              faces, eyes,
              tryUseCuda,
              tryUseOpenCL,
              out detectionTime);
            foreach (Rectangle face in faces)
            {
                CvInvoke.Rectangle(image, face, new Bgr(Color.Purple).MCvScalar, 3);
                Bitmap c = frame.Bitmap;
                Bitmap bmp = new Bitmap(face.Size.Width, face.Size.Height);
                Graphics g = Graphics.FromImage(bmp);
                g.DrawImage(c, 0, 0, face, GraphicsUnit.Pixel);
            }
            foreach (Rectangle eye in eyes)
                CvInvoke.Rectangle(image, eye, new Bgr(Color.Green).MCvScalar, 2);
            imageBox1 .Image = frame;
        }
        
        
        private void ReleaseData()
        {
            if (capture != null)
                capture.Dispose();
        }
        
       
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (capture != null)
            {
                if (captureInProgress)
                {  //stop the capture
                    btnStart.Text = "Start Capture";
                    capture.Pause();
                }
                else
                {
                    //start the capture
                    btnStart.Text = "Stop";
                    capture.Start();
                }
                captureInProgress = !captureInProgress;
            }
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            if (capture != null) capture.FlipVertical = !capture.FlipVertical;
        }


        private void button2_Click_1(object sender, EventArgs e)
        {
            if (capture != null) capture.FlipHorizontal = !capture.FlipHorizontal;
        }
    }
}
