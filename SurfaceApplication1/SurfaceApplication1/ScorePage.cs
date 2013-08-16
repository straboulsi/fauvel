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
     * This class defines a page of a score, for display in StudyTab.cs.
     * The page is an image displayed in a grid, for easy addition into StudyTab's StackPanel.
     * 
     * Primary coder: Alison Y. Chang
     * */
    public class ScorePage : Grid
    {
        public Image musicImage;

        public ScorePage(String pageName)
        {
            musicImage = new Image();
            musicImage.Source = new BitmapImage(new Uri(@"..\..\music\" + pageName + ".png", UriKind.Relative)); 
            musicImage.Width = 560;

            Height = 860;
            Children.Add(musicImage);

        }


        

    }
}
