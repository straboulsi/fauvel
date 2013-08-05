using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;

namespace SurfaceApplication1
{
    public class SideBarTab : TabItem
    {
        public Grid headerGrid;
        public Button deleteTabButton;
        public Image headerImage;
        public Canvas canvas;
        private SideBar mySideBar;

        public SideBarTab(SideBar mySideBar) : base()
        {
            this.mySideBar = mySideBar;
            canvas = new Canvas();
            headerGrid = new Grid();
            Content = canvas;
            Header = headerGrid;

            deleteTabButton = new Button();

            headerGrid.Width = 100;
            headerGrid.Height = 40;
            headerImage = new Image();
            headerImage.Opacity = 0.3;
            headerGrid.Children.Add(headerImage);

            deleteTabButton.Content = (string)"X";
            deleteTabButton.FontFamily = new FontFamily("Arial");
            deleteTabButton.FontSize = 35;
            deleteTabButton.Width = 70;
            deleteTabButton.Height = 40;
            deleteTabButton.Opacity = 0.7;
            Canvas.SetLeft(deleteTabButton, 476);
            Canvas.SetTop(deleteTabButton, 1);
            canvas.Children.Add(deleteTabButton);
            deleteTabButton.Click += new RoutedEventHandler(mySideBar.deleteTab);
            deleteTabButton.TouchDown += new EventHandler<TouchEventArgs>(mySideBar.deleteTab);
        }
    }
}
