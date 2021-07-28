namespace TwoCamerasVision
{
    partial class TuneObjectFilterForm
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
            this.label2 = new System.Windows.Forms.Label( );
            this.label3 = new System.Windows.Forms.Label( );
            this.redSlider = new AForge.Controls.ColorSlider( );
            this.greenSlider = new AForge.Controls.ColorSlider( );
            this.blueSlider = new AForge.Controls.ColorSlider( );
            this.SuspendLayout( );
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 10, 20 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 60, 13 );
            this.label1.TabIndex = 0;
            this.label1.Text = "Red range:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 10, 45 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 69, 13 );
            this.label2.TabIndex = 2;
            this.label2.Text = "Green range:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point( 10, 70 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 61, 13 );
            this.label3.TabIndex = 4;
            this.label3.Text = "Blue range:";
            // 
            // redSlider
            // 
            this.redSlider.EndColor = System.Drawing.Color.Red;
            this.redSlider.Location = new System.Drawing.Point( 85, 20 );
            this.redSlider.Name = "redSlider";
            this.redSlider.Size = new System.Drawing.Size( 270, 25 );
            this.redSlider.TabIndex = 5;
            this.redSlider.Type = AForge.Controls.ColorSlider.ColorSliderType.InnerGradient;
            this.redSlider.ValuesChanged += new System.EventHandler( this.redSlider_ValuesChanged );
            // 
            // greenSlider
            // 
            this.greenSlider.EndColor = System.Drawing.Color.Lime;
            this.greenSlider.Location = new System.Drawing.Point( 85, 45 );
            this.greenSlider.Name = "greenSlider";
            this.greenSlider.Size = new System.Drawing.Size( 270, 25 );
            this.greenSlider.TabIndex = 6;
            this.greenSlider.Type = AForge.Controls.ColorSlider.ColorSliderType.InnerGradient;
            this.greenSlider.ValuesChanged += new System.EventHandler( this.greenSlider_ValuesChanged );
            // 
            // blueSlider
            // 
            this.blueSlider.EndColor = System.Drawing.Color.Blue;
            this.blueSlider.Location = new System.Drawing.Point( 85, 70 );
            this.blueSlider.Name = "blueSlider";
            this.blueSlider.Size = new System.Drawing.Size( 270, 25 );
            this.blueSlider.TabIndex = 7;
            this.blueSlider.Type = AForge.Controls.ColorSlider.ColorSliderType.InnerGradient;
            this.blueSlider.ValuesChanged += new System.EventHandler( this.blueSlider_ValuesChanged );
            // 
            // TuneObjectFilterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 361, 100 );
            this.Controls.Add( this.blueSlider );
            this.Controls.Add( this.greenSlider );
            this.Controls.Add( this.redSlider );
            this.Controls.Add( this.label3 );
            this.Controls.Add( this.label2 );
            this.Controls.Add( this.label1 );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "TuneObjectFilterForm";
            this.ShowInTaskbar = false;
            this.Text = "Tune Object Filter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.TuneObjectFilterForm_FormClosing );
            this.ResumeLayout( false );
            this.PerformLayout( );

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private AForge.Controls.ColorSlider redSlider;
        private AForge.Controls.ColorSlider greenSlider;
        private AForge.Controls.ColorSlider blueSlider;
    }
}