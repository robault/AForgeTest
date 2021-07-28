namespace TwoCamerasVision
{
    partial class MoveCamerasForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose( );
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent( )
        {
            this.label1 = new System.Windows.Forms.Label( );
            this.deviceNameBox = new System.Windows.Forms.TextBox( );
            this.label2 = new System.Windows.Forms.Label( );
            this.serialNumberBox = new System.Windows.Forms.TextBox( );
            this.label3 = new System.Windows.Forms.Label( );
            this.versionBox = new System.Windows.Forms.TextBox( );
            this.groupBox1 = new System.Windows.Forms.GroupBox( );
            this.onCheck = new System.Windows.Forms.CheckBox( );
            this.groupBox2 = new System.Windows.Forms.GroupBox( );
            this.servo0Label = new System.Windows.Forms.Label( );
            this.maxServo0 = new System.Windows.Forms.NumericUpDown( );
            this.servo0TrackBar = new System.Windows.Forms.TrackBar( );
            this.minServo0 = new System.Windows.Forms.NumericUpDown( );
            this.groupBox3 = new System.Windows.Forms.GroupBox( );
            this.servo1Label = new System.Windows.Forms.Label( );
            this.maxServo1 = new System.Windows.Forms.NumericUpDown( );
            this.servo1TrackBar = new System.Windows.Forms.TrackBar( );
            this.minServo1 = new System.Windows.Forms.NumericUpDown( );
            this.followObjectCheck = new System.Windows.Forms.CheckBox( );
            this.groupBox1.SuspendLayout( );
            this.groupBox2.SuspendLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.maxServo0 ) ).BeginInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.servo0TrackBar ) ).BeginInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.minServo0 ) ).BeginInit( );
            this.groupBox3.SuspendLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.maxServo1 ) ).BeginInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.servo1TrackBar ) ).BeginInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.minServo1 ) ).BeginInit( );
            this.SuspendLayout( );
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 10, 23 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 38, 13 );
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // deviceNameBox
            // 
            this.deviceNameBox.Location = new System.Drawing.Point( 70, 20 );
            this.deviceNameBox.Name = "deviceNameBox";
            this.deviceNameBox.ReadOnly = true;
            this.deviceNameBox.Size = new System.Drawing.Size( 300, 20 );
            this.deviceNameBox.TabIndex = 1;
            this.deviceNameBox.Text = "Phidgets advanced servo controller was not found";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 10, 48 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 56, 13 );
            this.label2.TabIndex = 2;
            this.label2.Text = "Serial No.:";
            // 
            // serialNumberBox
            // 
            this.serialNumberBox.Location = new System.Drawing.Point( 70, 45 );
            this.serialNumberBox.Name = "serialNumberBox";
            this.serialNumberBox.ReadOnly = true;
            this.serialNumberBox.Size = new System.Drawing.Size( 117, 20 );
            this.serialNumberBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point( 220, 48 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 45, 13 );
            this.label3.TabIndex = 4;
            this.label3.Text = "Version:";
            // 
            // versionBox
            // 
            this.versionBox.Location = new System.Drawing.Point( 270, 45 );
            this.versionBox.Name = "versionBox";
            this.versionBox.ReadOnly = true;
            this.versionBox.Size = new System.Drawing.Size( 100, 20 );
            this.versionBox.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add( this.followObjectCheck );
            this.groupBox1.Controls.Add( this.onCheck );
            this.groupBox1.Controls.Add( this.label1 );
            this.groupBox1.Controls.Add( this.label3 );
            this.groupBox1.Controls.Add( this.versionBox );
            this.groupBox1.Controls.Add( this.deviceNameBox );
            this.groupBox1.Controls.Add( this.serialNumberBox );
            this.groupBox1.Controls.Add( this.label2 );
            this.groupBox1.Location = new System.Drawing.Point( 10, 10 );
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size( 380, 105 );
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device Information";
            // 
            // onCheck
            // 
            this.onCheck.AutoSize = true;
            this.onCheck.Location = new System.Drawing.Point( 10, 80 );
            this.onCheck.Name = "onCheck";
            this.onCheck.Size = new System.Drawing.Size( 135, 17 );
            this.onCheck.TabIndex = 6;
            this.onCheck.Text = "Turn On Servo 0 and 1";
            this.onCheck.UseVisualStyleBackColor = true;
            this.onCheck.CheckedChanged += new System.EventHandler( this.onCheck_CheckedChanged );
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add( this.servo0Label );
            this.groupBox2.Controls.Add( this.maxServo0 );
            this.groupBox2.Controls.Add( this.servo0TrackBar );
            this.groupBox2.Controls.Add( this.minServo0 );
            this.groupBox2.Location = new System.Drawing.Point( 10, 120 );
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size( 380, 70 );
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Servo 0";
            // 
            // servo0Label
            // 
            this.servo0Label.Location = new System.Drawing.Point( 315, 50 );
            this.servo0Label.Name = "servo0Label";
            this.servo0Label.Size = new System.Drawing.Size( 54, 13 );
            this.servo0Label.TabIndex = 3;
            this.servo0Label.Text = "label4";
            // 
            // maxServo0
            // 
            this.maxServo0.Enabled = false;
            this.maxServo0.Location = new System.Drawing.Point( 315, 23 );
            this.maxServo0.Name = "maxServo0";
            this.maxServo0.Size = new System.Drawing.Size( 45, 20 );
            this.maxServo0.TabIndex = 2;
            this.maxServo0.ValueChanged += new System.EventHandler( this.maxServo0_ValueChanged );
            // 
            // servo0TrackBar
            // 
            this.servo0TrackBar.Enabled = false;
            this.servo0TrackBar.LargeChange = 500;
            this.servo0TrackBar.Location = new System.Drawing.Point( 60, 20 );
            this.servo0TrackBar.Name = "servo0TrackBar";
            this.servo0TrackBar.Size = new System.Drawing.Size( 250, 45 );
            this.servo0TrackBar.TabIndex = 1;
            this.servo0TrackBar.TickFrequency = 1000;
            this.servo0TrackBar.ValueChanged += new System.EventHandler( this.servo0TrackBar_ValueChanged );
            // 
            // minServo0
            // 
            this.minServo0.Enabled = false;
            this.minServo0.Location = new System.Drawing.Point( 10, 23 );
            this.minServo0.Name = "minServo0";
            this.minServo0.Size = new System.Drawing.Size( 45, 20 );
            this.minServo0.TabIndex = 0;
            this.minServo0.ValueChanged += new System.EventHandler( this.minServo0_ValueChanged );
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add( this.servo1Label );
            this.groupBox3.Controls.Add( this.maxServo1 );
            this.groupBox3.Controls.Add( this.servo1TrackBar );
            this.groupBox3.Controls.Add( this.minServo1 );
            this.groupBox3.Location = new System.Drawing.Point( 10, 200 );
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size( 380, 70 );
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Servo 1";
            // 
            // servo1Label
            // 
            this.servo1Label.Location = new System.Drawing.Point( 315, 50 );
            this.servo1Label.Name = "servo1Label";
            this.servo1Label.Size = new System.Drawing.Size( 54, 13 );
            this.servo1Label.TabIndex = 4;
            this.servo1Label.Text = "label4";
            // 
            // maxServo1
            // 
            this.maxServo1.Enabled = false;
            this.maxServo1.Location = new System.Drawing.Point( 315, 23 );
            this.maxServo1.Name = "maxServo1";
            this.maxServo1.Size = new System.Drawing.Size( 45, 20 );
            this.maxServo1.TabIndex = 2;
            this.maxServo1.ValueChanged += new System.EventHandler( this.maxServo1_ValueChanged );
            // 
            // servo1TrackBar
            // 
            this.servo1TrackBar.Enabled = false;
            this.servo1TrackBar.LargeChange = 500;
            this.servo1TrackBar.Location = new System.Drawing.Point( 60, 20 );
            this.servo1TrackBar.Name = "servo1TrackBar";
            this.servo1TrackBar.Size = new System.Drawing.Size( 250, 45 );
            this.servo1TrackBar.TabIndex = 1;
            this.servo1TrackBar.TickFrequency = 1000;
            this.servo1TrackBar.ValueChanged += new System.EventHandler( this.servo1TrackBar_ValueChanged );
            // 
            // minServo1
            // 
            this.minServo1.Enabled = false;
            this.minServo1.Location = new System.Drawing.Point( 10, 23 );
            this.minServo1.Name = "minServo1";
            this.minServo1.Size = new System.Drawing.Size( 45, 20 );
            this.minServo1.TabIndex = 0;
            this.minServo1.ValueChanged += new System.EventHandler( this.minServo1_ValueChanged );
            // 
            // followObjectCheck
            // 
            this.followObjectCheck.AutoSize = true;
            this.followObjectCheck.Location = new System.Drawing.Point( 200, 80 );
            this.followObjectCheck.Name = "followObjectCheck";
            this.followObjectCheck.Size = new System.Drawing.Size( 88, 17 );
            this.followObjectCheck.TabIndex = 7;
            this.followObjectCheck.Text = "Follow object";
            this.followObjectCheck.UseVisualStyleBackColor = true;
            this.followObjectCheck.CheckedChanged += new System.EventHandler( this.followObjectCheck_CheckedChanged );
            // 
            // MoveCamerasForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 400, 281 );
            this.Controls.Add( this.groupBox3 );
            this.Controls.Add( this.groupBox2 );
            this.Controls.Add( this.groupBox1 );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "MoveCamerasForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Move Cameras";
            this.Load += new System.EventHandler( this.MoveCamerasForm_Load );
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.MoveCamerasForm_FormClosing );
            this.groupBox1.ResumeLayout( false );
            this.groupBox1.PerformLayout( );
            this.groupBox2.ResumeLayout( false );
            this.groupBox2.PerformLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.maxServo0 ) ).EndInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.servo0TrackBar ) ).EndInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.minServo0 ) ).EndInit( );
            this.groupBox3.ResumeLayout( false );
            this.groupBox3.PerformLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.maxServo1 ) ).EndInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.servo1TrackBar ) ).EndInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.minServo1 ) ).EndInit( );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox deviceNameBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox serialNumberBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox versionBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown maxServo0;
        private System.Windows.Forms.TrackBar servo0TrackBar;
        private System.Windows.Forms.NumericUpDown minServo0;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown maxServo1;
        private System.Windows.Forms.TrackBar servo1TrackBar;
        private System.Windows.Forms.NumericUpDown minServo1;
        private System.Windows.Forms.CheckBox onCheck;
        private System.Windows.Forms.Label servo0Label;
        private System.Windows.Forms.Label servo1Label;
        private System.Windows.Forms.CheckBox followObjectCheck;
    }
}