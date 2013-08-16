using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
     * Used in StudyTab.cs to define a display of just one voice from a polyphonic music object.
     * Takes in the name of the voice, i.e. triplum, duplum, or tenor.
     * The MusicExpander object is typically used for the full score, while its implementation, MusicPartExpander, is used for each part.
     * 
     * Primary coder: Alison Y. Chang (based on Jamie Chong's original StudyTab.cs)
     * */
    class MusicExpander : Expander
    {
        public MusicExpander(String voiceName)
        {
            IsExpanded = false;
            ExpandDirection = ExpandDirection.Down;
            Width = 560;
            BorderBrush = Brushes.Black;
            MinHeight = 30; // Overwrite to "20" for single part expander
            MaxHeight = 2400; // Overwrite to "860" for single part expander


        }


    }
}
