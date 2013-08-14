using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;

namespace DigitalFauvel
{
    /**
     * This class defines the basic format of a SideBarTab.
     * Each specialized tab (search tabs, annotation tabs, etc) extends this SideBarTab to add more methods and properties.
     * Primary Coders: Alison Y. Chang and Brendan Chou
     * */
    public class SideBarTab : TabItem
    {
        public Button deleteTabButton;
        public Canvas canvas;
        public Grid headerGrid;
        public Image headerImage;
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
            headerImage.Opacity = 0.2;
            headerGrid.Children.Add(headerImage);

            Image ex = new Image();
            ex.Source = new BitmapImage(new Uri(@"..\..\icons\exLarge.png", UriKind.Relative));

            deleteTabButton.Content = ex;
            deleteTabButton.Width = 40;
            deleteTabButton.Height = 40;
            deleteTabButton.Opacity = 0.7;
            Canvas.SetLeft(deleteTabButton, 560);
            Canvas.SetTop(deleteTabButton, 3);
            canvas.Children.Add(deleteTabButton);
            deleteTabButton.Click += new RoutedEventHandler(mySideBar.deleteTab);
            deleteTabButton.TouchDown += new EventHandler<TouchEventArgs>(mySideBar.deleteTab);
        }
    }
}
