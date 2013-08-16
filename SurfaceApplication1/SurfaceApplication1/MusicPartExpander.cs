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
     * IMPORTANT: The "tag" and "voiceName" parameters MUST match the names of the saved audio and image files!
     * For example, tag "2vMo2" and voiceName "tenor" will look for "2vMo2_tenor" (.png and .wma)
     * 
     * Primary coder: Alison Y. Chang (based on Jamie Chong's original StudyTab.cs)
     * */
    class MusicPartExpander : MusicExpander
    {
        public Image img;
        public List<MusicPartExpander> otherVoices;
        public ListBoxItem nameLBI, muteLBI, soloLBI, spaceLBI; // Not sure why Jamie chose to use LBIs
        public MediaPlayer player;
        public StackPanel buttonSP;
        public ToggleButton muteTB, soloTB;

        /**
         * Expects the tag without _t
         * IMPORTANT: The "tag" and "voiceName" parameters MUST match the names of the saved audio and image files!
         * For example, tag "2vMo2" and voiceName "tenor" will look for "2vMo2_tenor" (.png and .wma)
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

            nameLBI.Content = UppercaseFirst(voiceName); // Assuming audio files will also be named .duplum and "Duplum" will be fed into this constructor
            muteLBI.Content = muteTB;
            soloLBI.Content = soloTB;
            spaceLBI.Content = "  ";

            buttonSP.Children.Add(nameLBI);
            buttonSP.Children.Add(muteLBI);
            buttonSP.Children.Add(soloLBI);
            buttonSP.Children.Add(spaceLBI);

            Header = buttonSP;

            player = new MediaPlayer();
            player.Open(new Uri(@"..\..\musicz\" + tag + "_" + voiceName + ".wma", UriKind.Relative));



          
        }

        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

    }
}
