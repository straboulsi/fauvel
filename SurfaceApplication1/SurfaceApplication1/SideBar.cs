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
        public List<MainTab> TabList { get; set; }
        public int TabNumber { get; set; }

        private MainTab currentTab()
        {
            return TabList[TabNumber];
        }
        // This constructor defines the look of the "new tab", which displays all apps for a user to choose from.
        public SideBar(SurfaceWindow1 surfaceWindow, TabControl tabBar)
        {
            savedPages = new List<SavedPage>();

            this.surfaceWindow = surfaceWindow;
            this.tabBar = tabBar;
            tabItems = new List<SideBarTab>();

            tabAdd = new SideBarTab(this);
            tabAdd.Header = "+";
            tabAdd.Width = 50;
            tabAdd.FontSize = 25;
            tabAdd.FontFamily = new FontFamily("Cambria");

            Canvas newTabCanvas = new Canvas();
            newTabCanvas.Height = 900; 
            newTabCanvas.Width = 550;
            tabAdd.Content = newTabCanvas;

            newTabCanvas.Children.Add(addApplication("Search", "search.png", SearchButton_Selected, 100, 290, true));
            newTabCanvas.Children.Add(addApplication("Annotate", "pencil.png", AnnotateButton_Selected, 100, 480, false));
            newTabCanvas.Children.Add(addApplication("Saved Pages", "save.png", SavedPagesButton_Selected, 320, 480, false));
            newTabCanvas.Children.Add(addApplication("Music", "music.png", StudyButton_Selected, 320, 290, true));
            
            tabItems.Add(tabAdd);
            tabBar.DataContext = tabItems;
            tabBar.SelectedIndex = 0;
        }

        /**
         * Adds an application icon/etc to the SideBar new tab page.
         * */
        private StackPanel addApplication(String name, String image, RoutedEventHandler method, int x, int y, bool enabled)
        {
            StackPanel appPanel = new StackPanel();
            appPanel.Width = 130;
            appPanel.Orientation = Orientation.Vertical;
            appPanel.HorizontalAlignment = HorizontalAlignment.Center;
            Canvas.SetLeft(appPanel, x);
            Canvas.SetTop(appPanel, y);
            
            TextBlock text = new TextBlock();
            Button button = new Button();
            button.Width = 100;
            button.Height = 100;
            if (enabled)
            {
                button.Click += new RoutedEventHandler(method);
                button.TouchDown += new EventHandler<TouchEventArgs>(method);
            }
            else
            {
                button.Opacity = 0.5;
                text.Opacity = 0.5;
                button.IsEnabled = false;
            }
            button.Style = tabBar.FindResource("RoundButtonTemplate") as Style;

            Image img = new Image();
            img.Source = new BitmapImage(new Uri(@"..\..\icons\" + image, UriKind.Relative));
            img.Opacity = 0.7;
            button.Content = img;

            text.Text = name;
            text.TextWrapping = TextWrapping.NoWrap;
            text.FontSize = 25; 
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.Margin = new Thickness(0, 5, 0, 0);

     

            appPanel.Children.Add(button);
            appPanel.Children.Add(text);


            return appPanel;
        }


        /**
         * Opening a new SearchTab.
         * */
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

        /**
         * Opening a new AnnotationTab. 
         * NOTE: The annotation tab has not been implemented yet.
         * This method will open a blank tab with the annotation icon in its header.
         * */
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

        /**
         * Opening a new StudyTab (for music studying).
         * */
        private void StudyButton_Selected(object sender, RoutedEventArgs e)
        {
            MainTab tab = currentTab();
            int pagenumber = tab._page;
            var pieceList = Study.GetPieces(pagenumber);
            tabBar.DataContext = null;
            int count = tabItems.Count;
            StudyTab newTab = new StudyTab(this, surfaceWindow, pieceList);
            tabItems.Insert(count - 1, newTab);
            tabBar.DataContext = tabItems;
            tabBar.SelectedItem = newTab;

        }

        /**
         * Opening a new SavedPages Tab, to view the pages/settings that a user has "saved".
         * */
        private void SavedPagesButton_Selected(object sender, RoutedEventArgs e)
        {
            int count = tabItems.Count;
            tabBar.DataContext = null;
            SavedPagesTab newTab = new SavedPagesTab(this);
            newTab.HeaderTemplate = tabBar.FindResource("NewSavedPagesTab") as DataTemplate;
            newTab.Width = 100;
            tabItems.Insert(count - 1, newTab);
            tabBar.DataContext = tabItems;
            tabBar.SelectedItem = newTab;
        }


        /**
         * Deletes a tab from the SideBar.
         * */
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


        /**
         * Saves a page from the main UI.
         * Can be reopened from the SavedPages Tab.
         * */
        public void savePage(int pageNum, double width, Point center, SurfaceWindow1.language lang)
        {
            savedPages.Add(new SavedPage(pageNum, width, center, lang, surfaceWindow));
        }
    }
}
