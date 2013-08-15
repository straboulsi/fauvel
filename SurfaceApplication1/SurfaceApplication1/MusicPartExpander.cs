using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DigitalFauvel
{
    /**
     * Expander for a single voice in polyphonic music.
     * 
     * Primary coder: Alison Y. Chang - based on Jamie Chong's original StudyTab.cs
     * */
    class MusicPartExpander : MusicExpander
    {
        public Image img;
        public ListBoxItem nameLBI, muteLBI, soloLBI, spaceLBI; // Not sure why Jamie chose to use LBI; could change this
        public StackPanel buttonSP;
        public ToggleButton muteTB, soloTB;

        /**
         * Expects the tag without _t
         * tag + voice (i.e. 2vMo2_v1) should match image name
         * */
        public MusicPartExpander(String voiceName, String tag) : base (voiceName)
        {
            img = new Image();
            img.Source = new BitmapImage(new Uri(@"..\..\musicz\" + tag + "_" + voiceName+ ".png", UriKind.Relative));
            img.Width = 560;
            Content = img;

            buttonSP = new StackPanel();
            buttonSP.Orientation = Orientation.Horizontal;

            nameLBI = new ListBoxItem();
            muteLBI = new ListBoxItem();
            soloLBI = new ListBoxItem();
            spaceLBI = new ListBoxItem();

            muteTB = new ToggleButton();
            soloTB = new ToggleButton();

            muteTB.Content = "M";
            muteTB.FontSize = 14;
            muteTB.Height = 20;
            muteTB.Width = 20;
            soloTB.Content = "S";
            soloTB.FontSize = 14;
            soloTB.Height = 20;
            soloTB.Width = 20;

            nameLBI.Content = voiceName; // Assuming audio files will also be named .duplum and "Duplum" will be fed into this constructor
            muteLBI.Content = muteTB;
            soloLBI.Content = soloTB;
            spaceLBI.Content = "  ";

            buttonSP.Children.Add(nameLBI);
            buttonSP.Children.Add(muteLBI);
            buttonSP.Children.Add(soloLBI);
            buttonSP.Children.Add(spaceLBI);

            Header = buttonSP;
        }

    }
}
