namespace TwoCamerasVision
{
    partial class MainForm
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
            this.groupBox2 = new System.Windows.Forms.GroupBox( );
            this.camera2Combo = new System.Windows.Forms.ComboBox( );
            this.videoSourcePlayer2 = new AForge.Controls.VideoSourcePlayer( );
            this.groupBox1 = new System.Windows.Forms.GroupBox( );
            this.camera1Combo = new System.Windows.Forms.ComboBox( );
            this.videoSourcePlayer1 = new AForge.Controls.VideoSourcePlayer( );
            this.stopButton = new System.Windows.Forms.Button( );
            this.startButton = new System.Windows.Forms.Button( );
            this.moveCameraButton = new System.Windows.Forms.Button( );
            this.groupBox3 = new System.Windows.Forms.GroupBox( );
            this.showDetectedObjectsButton = new System.Windows.Forms.Button( );
            this.groupBox4 = new System.Windows.Forms.GroupBox( );
            this.tuneObjectFilterButton = new System.Windows.Forms.Button( );
            this.objectDetectionCheck = new System.Windows.Forms.CheckBox( );
            this.groupBox2.SuspendLayout( );
            this.groupBox1.SuspendLayout( );
            this.groupBox3.SuspendLayout( );
            this.groupBox4.SuspendLayout( );
            this.SuspendLayout( );
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add( this.camera2Combo );
            this.groupBox2.Controls.Add( this.videoSourcePlayer2 );
            this.groupBox2.Location = new System.Drawing.Point( 360, 130 );
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size( 342, 303 );
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Camera 2";
            // 
            // camera2Combo
            // 
            this.camera2Combo.FormattingEnabled = true;
            this.camera2Combo.Location = new System.Drawing.Point( 10, 20 );
            this.camera2Combo.Name = "camera2Combo";
            this.camera2Combo.Size = new System.Drawing.Size( 322, 21 );
            this.camera2Combo.TabIndex = 2;
            // 
            // videoSourcePlayer2
            // 
            this.videoSourcePlayer2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.videoSourcePlayer2.ForeColor = System.Drawing.Color.White;
            this.videoSourcePlayer2.Location = new System.Drawing.Point( 10, 50 );
            this.videoSourcePlayer2.Name = "videoSourcePlayer2";
            this.videoSourcePlayer2.Size = new System.Drawing.Size( 322, 242 );
            this.videoSourcePlayer2.TabIndex = 1;
            this.videoSourcePlayer2.VideoSource = null;
            this.videoSourcePlayer2.NewFrame += new AForge.Controls.VideoSourcePlayer.NewFrameHandler( this.videoSourcePlayer2_NewFrame );
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add( this.camera1Combo );
            this.groupBox1.Controls.Add( this.videoSourcePlayer1 );
            this.groupBox1.Location = new System.Drawing.Point( 10, 130 );
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size( 342, 303 );
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Camera 1";
            // 
            // camera1Combo
            // 
            this.camera1Combo.FormattingEnabled = true;
            this.camera1Combo.Location = new System.Drawing.Point( 10, 20 );
            this.camera1Combo.Name = "camera1Combo";
            this.camera1Combo.Size = new System.Drawing.Size( 322, 21 );
            this.camera1Combo.TabIndex = 3;
            // 
            // videoSourcePlayer1
            // 
            this.videoSourcePlayer1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.videoSourcePlayer1.ForeColor = System.Drawing.Color.White;
            this.videoSourcePlayer1.Location = new System.Drawing.Point( 10, 50 );
            this.videoSourcePlayer1.Name = "videoSourcePlayer1";
            this.videoSourcePlayer1.Size = new System.Drawing.Size( 322, 242 );
            this.videoSourcePlayer1.TabIndex = 0;
            this.videoSourcePlayer1.VideoSource = null;
            this.videoSourcePlayer1.NewFrame += new AForge.Controls.VideoSourcePlayer.NewFrameHandler( this.videoSourcePlayer1_NewFrame );
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point( 115, 20 );
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size( 100, 23 );
            this.stopButton.TabIndex = 7;
            this.stopButton.Text = "S&top Cameras";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler( this.stopButton_Click );
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point( 10, 20 );
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size( 100, 23 );
            this.startButton.TabIndex = 6;
            this.startButton.Text = "&Start Cameras";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler( this.startButton_Click );
            // 
            // moveCameraButton
            // 
            this.moveCameraButton.Location = new System.Drawing.Point( 559, 20 );
            this.moveCameraButton.Name = "moveCameraButton";
            this.moveCameraButton.Size = new System.Drawing.Size( 123, 23 );
            this.moveCameraButton.TabIndex = 8;
            this.moveCameraButton.Text = "&Move Cameras";
            this.moveCameraButton.UseVisualStyleBackColor = true;
            this.moveCameraButton.Click += new System.EventHandler( this.moveCameraButton_Click );
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add( this.startButton );
            this.groupBox3.Controls.Add( this.moveCameraButton );
            this.groupBox3.Controls.Add( this.stopButton );
            this.groupBox3.Location = new System.Drawing.Point( 10, 10 );
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size( 692, 55 );
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Cameras Control";
            // 
            // showDetectedObjectsButton
            // 
            this.showDetectedObjectsButton.Location = new System.Drawing.Point( 543, 20 );
            this.showDetectedObjectsButton.Name = "showDetectedObjectsButton";
            this.showDetectedObjectsButton.Size = new System.Drawing.Size( 139, 23 );
            this.showDetectedObjectsButton.TabIndex = 10;
            this.showDetectedObjectsButton.Text = "Show Detected &Objects";
            this.showDetectedObjectsButton.UseVisualStyleBackColor = true;
            this.showDetectedObjectsButton.Click += new System.EventHandler( this.showDetectedObjectsButton_Click );
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add( this.tuneObjectFilterButton );
            this.groupBox4.Controls.Add( this.objectDetectionCheck );
            this.groupBox4.Controls.Add( this.showDetectedObjectsButton );
            this.groupBox4.Location = new System.Drawing.Point( 10, 70 );
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size( 692, 55 );
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Object Detection";
            // 
            // tuneObjectFilterButton
            // 
            this.tuneObjectFilterButton.Location = new System.Drawing.Point( 201, 20 );
            this.tuneObjectFilterButton.Name = "tuneObjectFilterButton";
            this.tuneObjectFilterButton.Size = new System.Drawing.Size( 109, 23 );
            this.tuneObjectFilterButton.TabIndex = 12;
            this.tuneObjectFilterButton.Text = "&Tune Object Filter";
            this.tuneObjectFilterButton.UseVisualStyleBackColor = true;
            this.tuneObjectFilterButton.Click += new System.EventHandler( this.tuneObjectFilterButton_Click );
            // 
            // objectDetectionCheck
            // 
            this.objectDetectionCheck.AutoSize = true;
            this.objectDetectionCheck.Location = new System.Drawing.Point( 10, 25 );
            this.objectDetectionCheck.Name = "objectDetectionCheck";
            this.objectDetectionCheck.Size = new System.Drawing.Size( 148, 17 );
            this.objectDetectionCheck.TabIndex = 11;
            this.objectDetectionCheck.Text = "Turn On Object &Detection";
            this.objectDetectionCheck.UseVisualStyleBackColor = true;
            this.objectDetectionCheck.CheckedChanged += new System.EventHandler( this.objectDetectionCheck_CheckedChanged );
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 711, 442 );
            this.Controls.Add( this.groupBox4 );
            this.Controls.Add( this.groupBox3 );
            this.Controls.Add( this.groupBox2 );
            this.Controls.Add( this.groupBox1 );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Two Cameras Vision";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.MainForm_FormClosing );
            this.groupBox2.ResumeLayout( false );
            this.groupBox1.ResumeLayout( false );
            this.groupBox3.ResumeLayout( false );
            this.groupBox4.ResumeLayout( false );
            this.groupBox4.PerformLayout( );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox camera2Combo;
        private AForge.Controls.VideoSourcePlayer videoSourcePlayer2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox camera1Combo;
        private AForge.Controls.VideoSourcePlayer videoSourcePlayer1;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button moveCameraButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button showDetectedObjectsButton;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button tuneObjectFilterButton;
        private System.Windows.Forms.CheckBox objectDetectionCheck;
    }
}

