using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using Microsoft.Surface.Presentation.Controls;
using System.Xml;
using System.Windows.Documents;

namespace SurfaceApplication1
{
    /**
     * This class defines the SideBar that is on the right side of the Surface. 
     * It is initially set to a "new tab" screen that allows users to select an app - i.e. Search, Listen, Annotate, Save.
     * Primary Coder: Alison Y. Chang
     * */
    public class SideBar
    {
        public TabControl tabBar;
        private List<SideBarTab> tabItems;
        private SideBarTab tabAdd;
        public SurfaceWindow1 surfaceWindow;
        public List<SavedPage> savedPages;

        // This constructor defines the look of the "new tab", which displays all apps for a user to choose from.
        public SideBar(SurfaceWindow1 surfaceWindow, TabControl tabBar)
        {
            savedPages = new List<SavedPage>();

            this.surfaceWindow = surfaceWindow;
            this.tabBar = tabBar;
            tabItems = new List<SideBarTab>();


            tabAdd = new SideBarTab(this);
            tabAdd.Header = "  +  ";
            tabAdd.FontSize = 25;
            tabAdd.FontFamily = new FontFamily("Cambria");

            Canvas newTabCanvas = new Canvas();
            newTabCanvas.Height = 899;
            newTabCanvas.Width = 550;
            tabAdd.Content = newTabCanvas;


            // Search button, which triggers new SearchTab
            Button searchButton = new Button();
            searchButton.Style = tabBar.FindResource("RoundButtonTemplate") as Style;
            searchButton.Click += new RoutedEventHandler(SearchButton_Selected);
            searchButton.TouchDown += new EventHandler<TouchEventArgs>(SearchButton_Selected);

            Grid searchGrid = new Grid();

            Image searchIm = new Image();
            searchIm.Source = new BitmapImage(new Uri(@"..\..\icons\magnifyingglass.png", UriKind.Relative));
            searchIm.Style = tabBar.FindResource("ButtonImageTemplate") as Style;

            TextBlock searchText = new TextBlock();
            searchText.Text = "SEARCH";
            searchText.Style = tabBar.FindResource("ButtonTextTemplate") as Style;

            searchGrid.Children.Add(searchIm);
            searchGrid.Children.Add(searchText);
            searchButton.Content = searchGrid;
            Canvas.SetLeft(searchButton, 68.0);
            Canvas.SetTop(searchButton, 350.0);
            newTabCanvas.Children.Add(searchButton);



            // Annotate button, which triggers new AnnotateTab 
            Button annotateButton = new Button();
            annotateButton.Style = tabBar.FindResource("RoundButtonTemplate") as Style;
            annotateButton.Click += new RoutedEventHandler(AnnotateButton_Selected);
            annotateButton.TouchDown += new EventHandler<TouchEventArgs>(AnnotateButton_Selected);

            Grid annotateGrid = new Grid();

            Image annotateIm = new Image();
            annotateIm.Source = new BitmapImage(new Uri(@"..\..\icons\pencil.jpg", UriKind.Relative));
            annotateIm.Style = tabBar.FindResource("ButtonImageTemplate") as Style;

            TextBlock annotateText = new TextBlock();
            annotateText.Style = tabBar.FindResource("ButtonTextTemplate") as Style;
            annotateText.Text = "ANNOTATE";
            annotateText.Margin = new Thickness(0, 0, 0, 0);
            annotateText.RenderTransformOrigin = new Point(0.5, 0.5);
            annotateText.RenderTransform = new RotateTransform(45);
            annotateText.FontSize = 22.5;

            annotateGrid.Children.Add(annotateIm);
            annotateGrid.Children.Add(annotateText);
            annotateButton.Content = annotateGrid;
            Canvas.SetLeft(annotateButton, 219.0);
            Canvas.SetTop(annotateButton, 350.0);
            newTabCanvas.Children.Add(annotateButton);



            // Saved pages button, which triggers new SavedPagesTab
            Button savedPagesButton = new Button();
            savedPagesButton.Style = tabBar.FindResource("RoundButtonTemplate") as Style;
            savedPagesButton.Click += new RoutedEventHandler(SavedPagesButton_Selected);
            savedPagesButton.TouchDown += new EventHandler<TouchEventArgs>(SavedPagesButton_Selected);

            Image savedPagesIm = new Image();
            savedPagesIm.Source = new BitmapImage(new Uri(@"..\..\icons\save.png", UriKind.Relative));
            savedPagesIm.Style = tabBar.FindResource("ButtonImageTemplate") as Style;

            TextBlock savedPagesText = new TextBlock();
            savedPagesText.Style = tabBar.FindResource("ButtonTextTemplate") as Style;
            savedPagesText.Text = "SAVED";

            Grid savedPagesGrid = new Grid();

            savedPagesGrid.Children.Add(savedPagesIm);
            savedPagesGrid.Children.Add(savedPagesText);
            savedPagesButton.Content = savedPagesGrid;
            Canvas.SetLeft(savedPagesButton, 370.0);
            Canvas.SetTop(savedPagesButton, 480.0);
            newTabCanvas.Children.Add(savedPagesButton);



            // Study button, which triggers a new StudyTab 
            Button studyButton = new Button();
            studyButton.Style = tabBar.FindResource("RoundButtonTemplate") as Style;
            studyButton.Click += new RoutedEventHandler(StudyButton_Selected);
            studyButton.TouchDown += new EventHandler<TouchEventArgs>(StudyButton_Selected);

            Grid studyGrid = new Grid();

            Image studyIm = new Image();
            studyIm.Source = new BitmapImage(new Uri(@"..\..\icons\musicnote.png", UriKind.Relative));
            studyIm.Style = tabBar.FindResource("ButtonImageTemplate") as Style;

            TextBlock studyText = new TextBlock();
            studyText.Style = tabBar.FindResource("ButtonTextTemplate") as Style;
            studyText.Text = "STUDY";

            studyGrid.Children.Add(studyIm);
            studyGrid.Children.Add(studyText);
            studyButton.Content = studyGrid;
            Canvas.SetLeft(studyButton, 370.0);
            Canvas.SetTop(studyButton, 350.0);
            newTabCanvas.Children.Add(studyButton);


            tabItems.Add(tabAdd);
            tabBar.DataContext = tabItems;
            tabBar.SelectedIndex = 0;
        }

        private void SearchButton_Selected(object sender, RoutedEventArgs e)
        {
            tabBar.DataContext = null;
            SearchTab newTab = new SearchTab(this, surfaceWindow);
            int count = tabItems.Count;
            tabItems.Insert(count - 1, newTab);
            tabBar.DataContext = tabItems;
            tabBar.SelectedItem = newTab;
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
