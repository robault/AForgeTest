namespace AForgeTest
{
    partial class ObjectTracking
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cameraListComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.trackingButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.feedFPSLabel = new System.Windows.Forms.Label();
            this.cameraFeedVideoSourcePlayer = new AForge.Controls.VideoSourcePlayer();
            this.messageLabel = new System.Windows.Forms.Label();
            this.devicePropertiesLinkLabel = new System.Windows.Forms.LinkLabel();
            this.captureImagePictureBox = new System.Windows.Forms.PictureBox();
            this.processedFPSLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.imageToTracjPictureBox = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.colorToTrackPictureBox = new System.Windows.Forms.PictureBox();
            this.objectToTrackSelectionPictureBox = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.redRangeLabel = new System.Windows.Forms.Label();
            this.greenRangeLabel = new System.Windows.Forms.Label();
            this.blueRangeLabel = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.objectCenterLabel = new System.Windows.Forms.Label();
            this.trackRobotCheckBox = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.robotCenterLabel = new System.Windows.Forms.Label();
            this.robotPositionLabel = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.trackingCheckBox = new System.Windows.Forms.CheckBox();
            this.accelerationNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.xJoystickLabel = new System.Windows.Forms.Label();
            this.overrideMovementCheckBox = new System.Windows.Forms.CheckBox();
            this.yJoystickLabel = new System.Windows.Forms.Label();
            this.objectSelectionCheckBox = new System.Windows.Forms.CheckBox();
            this.moveByKeysCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.captureImagePictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageToTracjPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorToTrackPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectToTrackSelectionPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.accelerationNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // cameraListComboBox
            // 
            this.cameraListComboBox.FormattingEnabled = true;
            this.cameraListComboBox.Location = new System.Drawing.Point(15, 25);
            this.cameraListComboBox.Name = "cameraListComboBox";
            this.cameraListComboBox.Size = new System.Drawing.Size(200, 21);
            this.cameraListComboBox.TabIndex = 0;
            this.cameraListComboBox.SelectedIndexChanged += new System.EventHandler(this.cameraListComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Camera:";
            // 
            // trackingButton
            // 
            this.trackingButton.Location = new System.Drawing.Point(341, 448);
            this.trackingButton.Name = "trackingButton";
            this.trackingButton.Size = new System.Drawing.Size(80, 23);
            this.trackingButton.TabIndex = 2;
            this.trackingButton.Text = "Start Tracking";
            this.trackingButton.UseVisualStyleBackColor = true;
            this.trackingButton.Click += new System.EventHandler(this.trackingButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Camera Feed:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(120, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "fps:";
            // 
            // feedFPSLabel
            // 
            this.feedFPSLabel.AutoSize = true;
            this.feedFPSLabel.Location = new System.Drawing.Point(150, 77);
            this.feedFPSLabel.Name = "feedFPSLabel";
            this.feedFPSLabel.Size = new System.Drawing.Size(25, 13);
            this.feedFPSLabel.TabIndex = 6;
            this.feedFPSLabel.Text = "000";
            // 
            // cameraFeedVideoSourcePlayer
            // 
            this.cameraFeedVideoSourcePlayer.Location = new System.Drawing.Point(15, 92);
            this.cameraFeedVideoSourcePlayer.Name = "cameraFeedVideoSourcePlayer";
            this.cameraFeedVideoSourcePlayer.Size = new System.Drawing.Size(160, 120);
            this.cameraFeedVideoSourcePlayer.TabIndex = 7;
            this.cameraFeedVideoSourcePlayer.VideoSource = null;
            this.cameraFeedVideoSourcePlayer.NewFrame += new AForge.Controls.VideoSourcePlayer.NewFrameHandler(this.cameraFeedVideoSourcePlayer_NewFrame);
            // 
            // messageLabel
            // 
            this.messageLabel.AutoSize = true;
            this.messageLabel.ForeColor = System.Drawing.Color.Red;
            this.messageLabel.Location = new System.Drawing.Point(353, 540);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(10, 13);
            this.messageLabel.TabIndex = 8;
            this.messageLabel.Text = ".";
            // 
            // devicePropertiesLinkLabel
            // 
            this.devicePropertiesLinkLabel.AutoSize = true;
            this.devicePropertiesLinkLabel.Location = new System.Drawing.Point(161, 49);
            this.devicePropertiesLinkLabel.Name = "devicePropertiesLinkLabel";
            this.devicePropertiesLinkLabel.Size = new System.Drawing.Size(54, 13);
            this.devicePropertiesLinkLabel.TabIndex = 9;
            this.devicePropertiesLinkLabel.TabStop = true;
            this.devicePropertiesLinkLabel.Text = "Properties";
            this.devicePropertiesLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.devicePropertiesLinkLabel_LinkClicked);
            // 
            // captureImagePictureBox
            // 
            this.captureImagePictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.captureImagePictureBox.Location = new System.Drawing.Point(181, 92);
            this.captureImagePictureBox.Name = "captureImagePictureBox";
            this.captureImagePictureBox.Size = new System.Drawing.Size(160, 120);
            this.captureImagePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.captureImagePictureBox.TabIndex = 10;
            this.captureImagePictureBox.TabStop = false;
            // 
            // processedFPSLabel
            // 
            this.processedFPSLabel.AutoSize = true;
            this.processedFPSLabel.Location = new System.Drawing.Point(316, 76);
            this.processedFPSLabel.Name = "processedFPSLabel";
            this.processedFPSLabel.Size = new System.Drawing.Size(25, 13);
            this.processedFPSLabel.TabIndex = 12;
            this.processedFPSLabel.Text = "000";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(286, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(24, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "fps:";
            // 
            // imageToTracjPictureBox
            // 
            this.imageToTracjPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageToTracjPictureBox.Location = new System.Drawing.Point(341, 231);
            this.imageToTracjPictureBox.Name = "imageToTracjPictureBox";
            this.imageToTracjPictureBox.Size = new System.Drawing.Size(80, 60);
            this.imageToTracjPictureBox.TabIndex = 14;
            this.imageToTracjPictureBox.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(338, 215);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Object to track:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(358, 299);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Color:";
            // 
            // colorToTrackPictureBox
            // 
            this.colorToTrackPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorToTrackPictureBox.Location = new System.Drawing.Point(398, 297);
            this.colorToTrackPictureBox.Name = "colorToTrackPictureBox";
            this.colorToTrackPictureBox.Size = new System.Drawing.Size(20, 15);
            this.colorToTrackPictureBox.TabIndex = 16;
            this.colorToTrackPictureBox.TabStop = false;
            // 
            // objectToTrackSelectionPictureBox
            // 
            this.objectToTrackSelectionPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.objectToTrackSelectionPictureBox.Location = new System.Drawing.Point(15, 231);
            this.objectToTrackSelectionPictureBox.Name = "objectToTrackSelectionPictureBox";
            this.objectToTrackSelectionPictureBox.Size = new System.Drawing.Size(320, 240);
            this.objectToTrackSelectionPictureBox.TabIndex = 17;
            this.objectToTrackSelectionPictureBox.TabStop = false;
            this.objectToTrackSelectionPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.objectToTrackSelectionPictureBox_Paint);
            this.objectToTrackSelectionPictureBox.DoubleClick += new System.EventHandler(this.objectToTrackSelectionPictureBox_DoubleClick);
            this.objectToTrackSelectionPictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.objectToTrackSelectionPictureBox_MouseClick);
            this.objectToTrackSelectionPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.objectToTrackSelectionPictureBox_MouseDown);
            this.objectToTrackSelectionPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.objectToTrackSelectionPictureBox_MouseMove);
            this.objectToTrackSelectionPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.objectToTrackSelectionPictureBox_MouseUp);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 215);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(111, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Select object to track:";
            this.label7.Paint += new System.Windows.Forms.PaintEventHandler(this.objectToTrackSelectionPictureBox_Paint);
            this.label7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.objectToTrackSelectionPictureBox_MouseDown);
            this.label7.MouseMove += new System.Windows.Forms.MouseEventHandler(this.objectToTrackSelectionPictureBox_MouseMove);
            this.label7.MouseUp += new System.Windows.Forms.MouseEventHandler(this.objectToTrackSelectionPictureBox_MouseUp);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(350, 324);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(30, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Red:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(341, 347);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(39, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "Green:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(349, 372);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(31, 13);
            this.label10.TabIndex = 21;
            this.label10.Text = "Blue:";
            // 
            // redRangeLabel
            // 
            this.redRangeLabel.AutoSize = true;
            this.redRangeLabel.Location = new System.Drawing.Point(386, 324);
            this.redRangeLabel.Name = "redRangeLabel";
            this.redRangeLabel.Size = new System.Drawing.Size(54, 13);
            this.redRangeLabel.TabIndex = 22;
            this.redRangeLabel.Text = "000 / 000";
            // 
            // greenRangeLabel
            // 
            this.greenRangeLabel.AutoSize = true;
            this.greenRangeLabel.Location = new System.Drawing.Point(386, 347);
            this.greenRangeLabel.Name = "greenRangeLabel";
            this.greenRangeLabel.Size = new System.Drawing.Size(54, 13);
            this.greenRangeLabel.TabIndex = 23;
            this.greenRangeLabel.Text = "000 / 000";
            // 
            // blueRangeLabel
            // 
            this.blueRangeLabel.AutoSize = true;
            this.blueRangeLabel.Location = new System.Drawing.Point(386, 372);
            this.blueRangeLabel.Name = "blueRangeLabel";
            this.blueRangeLabel.Size = new System.Drawing.Size(54, 13);
            this.blueRangeLabel.TabIndex = 24;
            this.blueRangeLabel.Text = "000 / 000";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(347, 92);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(75, 13);
            this.label12.TabIndex = 26;
            this.label12.Text = "Object Center:";
            // 
            // objectCenterLabel
            // 
            this.objectCenterLabel.AutoSize = true;
            this.objectCenterLabel.Location = new System.Drawing.Point(386, 105);
            this.objectCenterLabel.Name = "objectCenterLabel";
            this.objectCenterLabel.Size = new System.Drawing.Size(30, 13);
            this.objectCenterLabel.TabIndex = 27;
            this.objectCenterLabel.Text = "0 / 0";
            // 
            // trackRobotCheckBox
            // 
            this.trackRobotCheckBox.AutoSize = true;
            this.trackRobotCheckBox.Location = new System.Drawing.Point(341, 425);
            this.trackRobotCheckBox.Name = "trackRobotCheckBox";
            this.trackRobotCheckBox.Size = new System.Drawing.Size(83, 17);
            this.trackRobotCheckBox.TabIndex = 28;
            this.trackRobotCheckBox.Text = "TrackRobot";
            this.trackRobotCheckBox.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(347, 129);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(79, 13);
            this.label13.TabIndex = 29;
            this.label13.Text = "Robot Position:";
            // 
            // robotCenterLabel
            // 
            this.robotCenterLabel.AutoSize = true;
            this.robotCenterLabel.Location = new System.Drawing.Point(388, 142);
            this.robotCenterLabel.Name = "robotCenterLabel";
            this.robotCenterLabel.Size = new System.Drawing.Size(30, 13);
            this.robotCenterLabel.TabIndex = 30;
            this.robotCenterLabel.Text = "0 / 0";
            // 
            // robotPositionLabel
            // 
            this.robotPositionLabel.AutoSize = true;
            this.robotPositionLabel.Location = new System.Drawing.Point(388, 178);
            this.robotPositionLabel.Name = "robotPositionLabel";
            this.robotPositionLabel.Size = new System.Drawing.Size(30, 13);
            this.robotPositionLabel.TabIndex = 32;
            this.robotPositionLabel.Text = "0 / 0";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(347, 165);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(80, 13);
            this.label15.TabIndex = 31;
            this.label15.Text = "Actual Position:";
            // 
            // trackingCheckBox
            // 
            this.trackingCheckBox.AutoSize = true;
            this.trackingCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.trackingCheckBox.Location = new System.Drawing.Point(364, 9);
            this.trackingCheckBox.Name = "trackingCheckBox";
            this.trackingCheckBox.Size = new System.Drawing.Size(76, 17);
            this.trackingCheckBox.TabIndex = 33;
            this.trackingCheckBox.Text = "Is Tracking";
            this.trackingCheckBox.UseVisualStyleBackColor = true;
            // 
            // accelerationNumericUpDown
            // 
            this.accelerationNumericUpDown.Location = new System.Drawing.Point(389, 26);
            this.accelerationNumericUpDown.Name = "accelerationNumericUpDown";
            this.accelerationNumericUpDown.Size = new System.Drawing.Size(45, 20);
            this.accelerationNumericUpDown.TabIndex = 34;
            this.accelerationNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(299, 29);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(84, 13);
            this.label14.TabIndex = 35;
            this.label14.Text = "Tracking speed:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(225, 477);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(48, 13);
            this.label11.TabIndex = 37;
            this.label11.Text = "Joystick:";
            // 
            // xJoystickLabel
            // 
            this.xJoystickLabel.AutoSize = true;
            this.xJoystickLabel.Location = new System.Drawing.Point(279, 478);
            this.xJoystickLabel.Name = "xJoystickLabel";
            this.xJoystickLabel.Size = new System.Drawing.Size(25, 13);
            this.xJoystickLabel.TabIndex = 38;
            this.xJoystickLabel.Text = "000";
            // 
            // overrideMovementCheckBox
            // 
            this.overrideMovementCheckBox.AutoSize = true;
            this.overrideMovementCheckBox.Enabled = false;
            this.overrideMovementCheckBox.Location = new System.Drawing.Point(15, 477);
            this.overrideMovementCheckBox.Name = "overrideMovementCheckBox";
            this.overrideMovementCheckBox.Size = new System.Drawing.Size(116, 17);
            this.overrideMovementCheckBox.TabIndex = 39;
            this.overrideMovementCheckBox.Text = "Overide Movement";
            this.overrideMovementCheckBox.UseVisualStyleBackColor = true;
            this.overrideMovementCheckBox.CheckedChanged += new System.EventHandler(this.overrideMovementCheckBox_CheckedChanged);
            // 
            // yJoystickLabel
            // 
            this.yJoystickLabel.AutoSize = true;
            this.yJoystickLabel.Location = new System.Drawing.Point(310, 478);
            this.yJoystickLabel.Name = "yJoystickLabel";
            this.yJoystickLabel.Size = new System.Drawing.Size(25, 13);
            this.yJoystickLabel.TabIndex = 40;
            this.yJoystickLabel.Text = "000";
            // 
            // objectSelectionCheckBox
            // 
            this.objectSelectionCheckBox.AutoSize = true;
            this.objectSelectionCheckBox.Location = new System.Drawing.Point(15, 500);
            this.objectSelectionCheckBox.Name = "objectSelectionCheckBox";
            this.objectSelectionCheckBox.Size = new System.Drawing.Size(102, 17);
            this.objectSelectionCheckBox.TabIndex = 41;
            this.objectSelectionCheckBox.Text = "Object selection";
            this.objectSelectionCheckBox.UseVisualStyleBackColor = true;
            // 
            // moveByKeysCheckBox
            // 
            this.moveByKeysCheckBox.AutoSize = true;
            this.moveByKeysCheckBox.Location = new System.Drawing.Point(15, 524);
            this.moveByKeysCheckBox.Name = "moveByKeysCheckBox";
            this.moveByKeysCheckBox.Size = new System.Drawing.Size(144, 17);
            this.moveByKeysCheckBox.TabIndex = 42;
            this.moveByKeysCheckBox.Text = "Use keyboard movement";
            this.moveByKeysCheckBox.UseVisualStyleBackColor = true;
            this.moveByKeysCheckBox.CheckedChanged += new System.EventHandler(this.moveByKeysCheckBox_CheckedChanged);
            this.moveByKeysCheckBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.moveByKeysCheckBox_KeyDown);
            this.moveByKeysCheckBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.moveByKeysCheckBox_KeyUp);
            // 
            // ObjectTracking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 560);
            this.Controls.Add(this.moveByKeysCheckBox);
            this.Controls.Add(this.objectSelectionCheckBox);
            this.Controls.Add(this.yJoystickLabel);
            this.Controls.Add(this.overrideMovementCheckBox);
            this.Controls.Add(this.xJoystickLabel);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.accelerationNumericUpDown);
            this.Controls.Add(this.trackingCheckBox);
            this.Controls.Add(this.robotPositionLabel);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.robotCenterLabel);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.trackRobotCheckBox);
            this.Controls.Add(this.objectCenterLabel);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.blueRangeLabel);
            this.Controls.Add(this.greenRangeLabel);
            this.Controls.Add(this.redRangeLabel);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.objectToTrackSelectionPictureBox);
            this.Controls.Add(this.colorToTrackPictureBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.imageToTracjPictureBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.processedFPSLabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.captureImagePictureBox);
            this.Controls.Add(this.devicePropertiesLinkLabel);
            this.Controls.Add(this.messageLabel);
            this.Controls.Add(this.cameraFeedVideoSourcePlayer);
            this.Controls.Add(this.feedFPSLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.trackingButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cameraListComboBox);
            this.DoubleBuffered = true;
            this.Name = "ObjectTracking";
            this.Text = "ObjectTracking";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ObjectTracking_FormClosing);
            this.Load += new System.EventHandler(this.ObjectTracking_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ObjectTracking_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ObjectTracking_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.captureImagePictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageToTracjPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorToTrackPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectToTrackSelectionPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.accelerationNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cameraListComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button trackingButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label feedFPSLabel;
        private AForge.Controls.VideoSourcePlayer cameraFeedVideoSourcePlayer;
        private System.Windows.Forms.Label messageLabel;
        private System.Windows.Forms.LinkLabel devicePropertiesLinkLabel;
        private System.Windows.Forms.PictureBox captureImagePictureBox;
        private System.Windows.Forms.Label processedFPSLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox imageToTracjPictureBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox colorToTrackPictureBox;
        private System.Windows.Forms.PictureBox objectToTrackSelectionPictureBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label redRangeLabel;
        private System.Windows.Forms.Label greenRangeLabel;
        private System.Windows.Forms.Label blueRangeLabel;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label objectCenterLabel;
        private System.Windows.Forms.CheckBox trackRobotCheckBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label robotCenterLabel;
        private System.Windows.Forms.Label robotPositionLabel;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox trackingCheckBox;
        private System.Windows.Forms.NumericUpDown accelerationNumericUpDown;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label xJoystickLabel;
        private System.Windows.Forms.CheckBox overrideMovementCheckBox;
        private System.Windows.Forms.Label yJoystickLabel;
        private System.Windows.Forms.CheckBox objectSelectionCheckBox;
        private System.Windows.Forms.CheckBox moveByKeysCheckBox;
    }
}