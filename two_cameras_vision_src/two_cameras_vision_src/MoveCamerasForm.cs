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

using Phidgets;
using Phidgets.Events;

namespace TwoCamerasVision
{
    public partial class MoveCamerasForm : Form
    {
        // Phidget servo controller
        AdvancedServo servo = null;

        public MoveCamerasForm( )
        {
            InitializeComponent( );
            EnableControls( false );

            servo0Label.Text = servo1Label.Text = string.Empty;
        }

        // On form load
        private void MoveCamerasForm_Load( object sender, EventArgs e )
        {
            try
            {
                servo = new AdvancedServo( );

                servo.Attach += new AttachEventHandler( servo_Attach );
                servo.Detach += new DetachEventHandler( servo_Detach );

                servo.open( );
            }
            catch
            {
            }
        }

        // On form closing - hide it instead
        private void MoveCamerasForm_FormClosing( object sender, FormClosingEventArgs e )
        {
            this.Hide( );
            e.Cancel = true;
        }

        // Phidgets device is attached
        private void servo_Attach( object sender, AttachEventArgs e )
        {
            deviceNameBox.Text   = servo.Name;
            serialNumberBox.Text = servo.SerialNumber.ToString( );
            versionBox.Text      = servo.Version.ToString( );

            servo.servos[0].Engaged = false;
            servo.servos[1].Engaged = false;

            System.Diagnostics.Debug.WriteLine( servo.servos[0].VelocityMin );

            servo.servos[0].VelocityLimit = 50;
            servo.servos[1].VelocityLimit = 50;

            servo.servos[0].Acceleration = servo.servos[0].AccelerationMin;
            servo.servos[1].Acceleration = servo.servos[1].AccelerationMin;

            onCheck.Checked = false;

            // servo 0 initialization
            minServo0.Minimum = maxServo0.Minimum = (int) servo.servos[0].PositionMin;
            minServo0.Maximum = maxServo0.Maximum = (int) servo.servos[0].PositionMax;
            minServo0.Value = minServo0.Minimum;
            maxServo0.Value = maxServo0.Maximum;

            UpdateServo0TrackBar( );
            servo0TrackBar.Value = (int) ( ( maxServo0.Value - minServo0.Value ) / 2 + minServo0.Value ) * 100;

            // servo 1 initialization
            minServo1.Minimum = maxServo1.Minimum = (int) servo.servos[1].PositionMin;
            minServo1.Maximum = maxServo1.Maximum = (int) servo.servos[1].PositionMax;
            minServo1.Value = minServo1.Minimum;
            maxServo1.Value = maxServo1.Maximum;

            UpdateServo1TrackBar( );
            servo1TrackBar.Value = (int) ( ( maxServo1.Value - minServo1.Value ) / 2 + minServo1.Value ) * 100;

            EnableControls( true );
        }

        // Phidgets device is detached
        private void servo_Detach( object sender, DetachEventArgs e )
        {
            deviceNameBox.Text   = "Device was detached";
            serialNumberBox.Text = string.Empty;
            versionBox.Text      = string.Empty;

            EnableControls( false );
        }

        // Enable/disable servo controls
        private void EnableControls( bool enable )
        {
            minServo0.Enabled = enable;
            maxServo0.Enabled = enable;
            minServo1.Enabled = enable;
            maxServo1.Enabled = enable;

            servo0TrackBar.Enabled = enable;
            servo1TrackBar.Enabled = enable;

            onCheck.Enabled = enable;
        }

        // Changed minimum value for servo 0
        private void minServo0_ValueChanged( object sender, EventArgs e )
        {
            UpdateServo0TrackBar( );
        }

        // Changed maximum value for servo 0
        private void maxServo0_ValueChanged( object sender, EventArgs e )
        {
            UpdateServo0TrackBar( );
        }

        // Changed minimum value for servo 1
        private void minServo1_ValueChanged( object sender, EventArgs e )
        {
            UpdateServo1TrackBar( );
        }

        // Changed maximum value for servo 1
        private void maxServo1_ValueChanged( object sender, EventArgs e )
        {
            UpdateServo1TrackBar( );
        }

        // Update min and max values for 0 servo's trackbar
        private void UpdateServo0TrackBar( )
        {
            int min = Math.Min( (int) minServo0.Value, (int) maxServo0.Value );
            int max = Math.Max( (int) minServo0.Value, (int) maxServo0.Value );

            servo0TrackBar.SetRange( min * 100, max * 100 );
        }

        // Update min and max values for 1 servo's trackbar
        private void UpdateServo1TrackBar( )
        {
            int min = Math.Min( (int) minServo1.Value, (int) maxServo1.Value );
            int max = Math.Max( (int) minServo1.Value, (int) maxServo1.Value );

            servo1TrackBar.SetRange( min * 100, max * 100 );
        }

        // Servo 0 track bar's value is changed
        private void servo0TrackBar_ValueChanged( object sender, EventArgs e )
        {
            double position = (double) servo0TrackBar.Value / 100;

            servo0Label.Text = position.ToString( "F2" );

            if ( onCheck.Checked )
            {
                servo.servos[0].Position = position;
            }
        }

        // Servo 1 track bar's value is changed
        private void servo1TrackBar_ValueChanged( object sender, EventArgs e )
        {
            double position = (double) servo1TrackBar.Value / 100;

            servo1Label.Text = position.ToString( "F2" );

            if ( onCheck.Checked )
            {
                servo.servos[1].Position = position;
            }
        }

        // Turn on/off servos
        private void onCheck_CheckedChanged( object sender, EventArgs e )
        {
            if ( onCheck.Checked )
            {
                servo.servos[0].Position = (double) servo0TrackBar.Value / 100;
                servo.servos[1].Position = (double) servo1TrackBar.Value / 100;
            }
            
            servo.servos[0].Engaged = servo.servos[1].Engaged = onCheck.Checked;
        }

        // On follow object turn on/off
        private void followObjectCheck_CheckedChanged( object sender, EventArgs e )
        {
            servo0TrackBar.Enabled = servo1TrackBar.Enabled = !followObjectCheck.Checked;
        }

        // Delegate to enable async calls for setting control's property
        private delegate void UpdateValueCallback( System.Windows.Forms.TrackBar control, int value );

        // Thread safe updating of track bar's value
        private void UpdateValue( System.Windows.Forms.TrackBar control, int value )
        {
            if ( control.InvokeRequired )
            {
                UpdateValueCallback d = new UpdateValueCallback( UpdateValue );
                Invoke( d, new object[] { control, value } );
            }
            else
            {
                control.Value += value;
            }
        }

        // Run motors for the specified amount of degrees
        public void RunMotors( float motor1Angle, float motor2Angle )
        {
            if ( followObjectCheck.Checked )
            {
                UpdateValue( servo0TrackBar, (int) ( motor1Angle * 100 ) );
                UpdateValue( servo1TrackBar, (int) ( motor2Angle * 100 ) );
            }
        }
    }
}
