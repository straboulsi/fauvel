using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SurfaceApplication1
{
    /**
     * Primary Coder: Brendan Chou
     * */
    public class SavedPage
    {
        public Button button;
        public double width;
        public Image image;
        public int pageNum;
        public Point center;
        private SurfaceWindow1 SurfaceWindow;
        public SurfaceWindow1.language language;




        public SavedPage(int p, double w, Point c, SurfaceWindow1.language l, SurfaceWindow1 SurfaceWindow)
        {
            center = c;
            width = w;
            pageNum = p;
            language = l;
            this.SurfaceWindow = SurfaceWindow;

            button = new Button();
            button.Width = 170;
            button.Height = 119;
            button.Content = image;
            button.Click += new RoutedEventHandler(buttonPress);

            button.Content = pageNum.ToString();
        }

        private void buttonPress(object sender, RoutedEventArgs e)
        {
            SurfaceWindow.goToSavedPage(this);
        }
    }
}
