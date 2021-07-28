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

using AForge;

namespace TwoCamerasVision
{
    public partial class TuneObjectFilterForm : Form
    {
        private IntRange redRange   = new IntRange( 0, 255 );
        private IntRange greenRange = new IntRange( 0, 255 );
        private IntRange blueRange  = new IntRange( 0, 255 );

        // Red range
        public IntRange RedRange
        {
            get { return redRange; }
            set
            {
                redRange = value;
                redSlider.Min = value.Min;
                redSlider.Max = value.Max;
            }
        }
        // Green range
        public IntRange GreenRange
        {
            get { return greenRange; }
            set
            {
                greenRange = value;
                greenSlider.Min = value.Min;
                greenSlider.Max = value.Max;
            }
        }
        // Blue range
        public IntRange BlueRange
        {
            get { return blueRange; }
            set
            {
                blueRange = value;
                blueSlider.Min = value.Min;
                blueSlider.Max = value.Max;
            }
        }

        public event EventHandler OnFilterUpdate;

        public TuneObjectFilterForm( )
        {
            InitializeComponent( );
        }

        // On form closing - hide it instead
        private void TuneObjectFilterForm_FormClosing( object sender, FormClosingEventArgs e )
        {
            this.Hide( );
            e.Cancel = true;
        }

        // Red range was changed
        private void redSlider_ValuesChanged( object sender, EventArgs e )
        {
            redRange.Min = redSlider.Min;
            redRange.Max = redSlider.Max;

            if ( OnFilterUpdate != null )
                OnFilterUpdate( this, null );
        }

        // Green range was changed
        private void greenSlider_ValuesChanged( object sender, EventArgs e )
        {
            greenRange.Min = greenSlider.Min;
            greenRange.Max = greenSlider.Max;
            
            if ( OnFilterUpdate != null )
                OnFilterUpdate( this, null );
        }

        // Blue range was changed
        private void blueSlider_ValuesChanged( object sender, EventArgs e )
        {
            blueRange.Min = blueSlider.Min;
            blueRange.Max = blueSlider.Max;

            if ( OnFilterUpdate != null )
                OnFilterUpdate( this, null );
        }
    }
}
