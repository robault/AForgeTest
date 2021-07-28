// Two Cameras Vision
//
// Copyright © Andrew Kirillov, 2009
// andrew.kirillov@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TwoCamerasVision
{
    public partial class DetectedObjectsForm : Form
    {
        public DetectedObjectsForm( )
        {
            InitializeComponent( );
        }

        // On form closing - hide it instead
        private void DetectedObjectsForm_FormClosing( object sender, FormClosingEventArgs e )
        {
            this.Hide( );
            e.Cancel = true;
        }

        // Update object's picture
        public void UpdateObjectPicture( int objectNumber, Bitmap picture )
        {
            Image oldPicture = null;

            switch ( objectNumber )
            {
                case 0:
                    oldPicture = pictureBox1.Image;
                    pictureBox1.Image = picture;
                    break;
                case 1:
                    oldPicture = pictureBox2.Image;
                    pictureBox2.Image = picture;
                    break;
            }

            if ( oldPicture != null )
            {
                oldPicture.Dispose( );
            }
        }
    }
}
