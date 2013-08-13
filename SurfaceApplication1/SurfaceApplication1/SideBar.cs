using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using System.Xml;
using System.Windows.Documents;

namespace DigitalFauvel
{
    /**
     * This class defines the SideBar that is on the right side of the Surface. 
     * It is initially set to a "new tab" screen that allows users to select an app - i.e. Search, Listen, Annotate, Save.
     * Primary Coder: Alison Y. Chang
     * */
    public class SideBar
    {

        public List<SavedPage> savedPages;
        private List<SideBarTab> tabItems;
        private SideBarTab tabAdd;
        public SurfaceWindow1 surfaceWindow;
        public TabControl tabBar;

        // This constructor defines the look of the "new tab", which displays all apps for a user to choose from.
        public SideBar(SurfaceWindow1 surfaceWindow, TabControl tabBar)
        {
            savedPages = new List<SavedPage>();

            this.surfaceWindow = surfaceWindow;
            this.tabBar = tabBar;
            tabItems = new List<SideBarTab>();

            tabAdd = new SideBarTab(this);
            tabAdd.Header = "+";
            tabAdd.FontSize = 25;
            tabAdd.FontFamily = new FontFamily("Cambria");

            Canvas newTabCanvas = new Canvas();
            newTabCanvas.Height = 900; // 899
            newTabCanvas.Width = 550;
            tabAdd.Content = newTabCanvas;

            newTabCanvas.Children.Add(addApplication("Search", "search.png", SearchButton_Selected, 0, 0));
            newTabCanvas.Children.Add(addApplication("Annotate", "pencil.png", AnnotateButton_Selected, 110, 0));
            newTabCanvas.Children.Add(addApplication("Saved Pages", "save.png", SavedPagesButton_Selected, 220, 0));
            newTabCanvas.Children.Add(addApplication("Music", "music.png", StudyButton_Selected, 330, 0));
            
            tabItems.Add(tabAdd);
            tabBar.DataContext = tabItems;
            tabBar.SelectedIndex = 0;
        }

        private Canvas addApplication(String name, String image, RoutedEventHandler method, int x, int y)
        {
            Canvas canvas = new Canvas();
            canvas.Width = 100;
            canvas.Height = 120;
            Button button = new Button();
            button.Width = 100;
            button.Height = 100;
            button.Click += new RoutedEventHandler(method);
            button.TouchDown += new EventHandler<TouchEventArgs>(method);

            Image img = new Image();
            img.Source = new BitmapImage(new Uri(@"..\..\icons\" + image, UriKind.Relative));

            TextBlock text = new TextBlock();
            text.Text = name;
            text.FontSize = 14;

            button.Content = img;
            Canvas.SetLeft(button, 0);
            Canvas.SetTop(button, 0);
            Canvas.SetLeft(canvas, x);
            Canvas.SetTop(canvas, y);
            Canvas.SetLeft(text, 50 - text.Width / 2);
            Canvas.SetTop(text, 103);
            canvas.Children.Add(button);
            canvas.Children.Add(text);

            return canvas;
        }

        private void SearchButton_Selected(object sender, RoutedEventArgs e)
        {
            tabBar.DataContext = null;
            SearchTab newTab = new SearchTab(this, surfaceWindow);
            int count = tabItems.Count;
            tabItems.Insert(count - 1, newTab);
            tabBar.DataContext = tabItems;
            tabBar.SelectedItem = newTab;
            SurfaceKeyboard.IsVisible = true;
        }

        private void AnnotateButton_Selected(object sender, RoutedEventArgs e)
        {
            tabBar.DataContext = null;
            SideBarTab newTab = new SideBarTab(this);
            int count = tabItems.Count;
            newTab.HeaderTemplate = tabBar.FindResource("NewAnnotateTab") as DataTemplate;
            tabItems.Insert(count - 1, newTab);
            tabBar.DataContext = tabItems;
            tabBar.SelectedItem = newTab;
        }

        private void StudyButton_Selected(object sender, RoutedEventArgs e)
        {
            tabBar.DataContext = null;
            int count = tabItems.Count;
            StudyTab newTab = new StudyTab(this, surfaceWindow);
            newTab.HeaderTemplate = tabBar.FindResource("NewStudyTab") as DataTemplate;
            tabItems.Insert(count - 1, newTab);
            tabBar.DataContext = tabItems;
            tabBar.SelectedItem = newTab;

        }

        private void SavedPagesButton_Selected(object sender, RoutedEventArgs e)
        {
            int count = tabItems.Count;
            tabBar.DataContext = null;
            SavedPagesTab newTab = new SavedPagesTab(this);
            newTab.Header = "Saved Pages";
            newTab.Width = 100;
            tabItems.Insert(count - 1, newTab);
            tabBar.DataContext = tabItems;
            tabBar.SelectedItem = newTab;
        }



        public void deleteTab(object sender, RoutedEventArgs e)
        {
            SideBarTab selectedTab = tabBar.SelectedItem as SideBarTab;
            tabBar.DataContext = null;

            tabItems.Remove(selectedTab);
            tabBar.DataContext = tabItems;

            if (selectedTab == null || selectedTab.Equals(selectedTab))
                selectedTab = tabItems[0];
            
            tabBar.SelectedItem = selectedTab;
        }


        public void savePage(int pageNum, double width, Point center, SurfaceWindow1.language lang)
        {
            savedPages.Add(new SavedPage(pageNum, width, center, lang, surfaceWindow));
        }
    }
}
