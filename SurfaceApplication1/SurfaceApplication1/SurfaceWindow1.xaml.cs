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
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using SSC = Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using System.Collections;

namespace SurfaceApplication1
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        int maxPageWidth = 5461;
        int maxPageHeight = 7700;
        int minPageWidth = 680;
        int minPageHeight = 930;
        int tabNumber = 0;
        int minPage = 1;
        int maxPage = 88;
        List<Tab> tabArray = new List<Tab>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

            maxPageText.Text = maxPage.ToString();
            TabItem newTabButton = new TabItem();
            newTabButton.Header = "+";
            tabBar.Items.Add(newTabButton);
            createTab(1);
            loadPage();

            AddWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
            
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }


        private void prev_Click(object sender, RoutedEventArgs e)
        {
            tabArray[tabNumber].pageNumber--;
            if (tabArray[tabNumber].pageNumber < minPage)
                tabArray[tabNumber].pageNumber = minPage;
            loadPage();
        }


        private void next_Click(object sender, RoutedEventArgs e)
        {
            tabArray[tabNumber].pageNumber++;
            if (tabArray[tabNumber].pageNumber > maxPage)
                tabArray[tabNumber].pageNumber = maxPage;
            loadPage();
        }


        private void loadPage()
        {
            int pageNumber = tabArray[tabNumber].pageNumber;
            int versoNum = 2 * pageNumber + 10;
            int rectoNum = 2 * pageNumber + 11;

            Image verso = tabArray[tabNumber]._verso;
            Image recto = tabArray[tabNumber]._recto;

            verso.Source = getPageImage(versoNum.ToString());
            verso.Stretch = Stretch.UniformToFill;

            recto.Source = getPageImage(rectoNum.ToString());
            recto.Stretch = Stretch.UniformToFill;

            pageNumberText.Text = pageNumber.ToString();
            tabArray[tabNumber]._tab.Header = (pageNumber-1).ToString() + "v / " + pageNumber.ToString() + "r";
        }


        private BitmapImage getPageImage(String str)
        {
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.CacheOption = BitmapCacheOption.None;
            src.UriSource = new Uri("pack://application:,,,/pages/" + str + ".jpg", UriKind.Absolute);
            src.EndInit();
            return src;
        }


        private TabItem createTab(int page)
        {
            int count = tabArray.Count;
            TabItem tab = new TabItem();
            tab.Header = string.Format("0v / 1r");
            tab.FontSize = 25;
            tab.Name = string.Format("tabItem_{0}", count);
            tab.HeaderTemplate = tabBar.FindResource("TabHeader") as DataTemplate;

            Button delBtn = new Button();

            Canvas can = new Canvas();
            Canvas c_s = new Canvas();
            Image verso = new Image();
            Image recto = new Image();
            SSC.ScatterView vScatterView = new SSC.ScatterView();
            SSC.ScatterView rScatterView = new SSC.ScatterView();

            vScatterView.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            vScatterView.ClipToBounds = true;
            rScatterView.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            rScatterView.ClipToBounds = true;
            rScatterView.Margin = new System.Windows.Thickness(680, 0, 0, 0);
            c_s.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            c_s.Width = 560;
            c_s.ClipToBounds = true;
            
            vScatterView.Width = minPageWidth;
            vScatterView.Height = minPageHeight;
            vScatterView.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
            vScatterView.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
            vScatterView.Background = Brushes.Maroon;
            
            rScatterView.Width = minPageWidth;
            rScatterView.Height = minPageHeight;
            rScatterView.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
            rScatterView.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
            rScatterView.Background = Brushes.Navy;

            SSC.ScatterViewItem vScatterItem = new SSC.ScatterViewItem();
            SSC.ScatterViewItem rScatterItem = new SSC.ScatterViewItem();
            vScatterItem.Width = minPageWidth;
            vScatterItem.Height = minPageHeight;
            rScatterItem.Width = minPageWidth;
            rScatterItem.Height = minPageHeight;
            vScatterItem.MaxWidth = maxPageWidth;
            vScatterItem.MaxHeight = maxPageHeight;
            rScatterItem.MaxWidth = maxPageWidth;
            rScatterItem.MaxHeight = maxPageHeight;
            vScatterItem.MinWidth = minPageWidth;
            vScatterItem.MinHeight = minPageHeight;
            rScatterItem.MinWidth = minPageWidth;
            rScatterItem.MinHeight = minPageHeight;
            vScatterItem.CanMove = true;
            vScatterItem.CanRotate = false;
            vScatterItem.CanScale = true;
            rScatterItem.CanMove = true;
            rScatterItem.CanRotate = false;
            rScatterItem.CanScale = true;
            vScatterItem.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
            vScatterItem.Center = new Point(minPageWidth / 2, minPageHeight / 2);
            rScatterItem.Center = new Point(minPageWidth / 2, minPageHeight / 2);

            Canvas vCanvas = new Canvas();
            Canvas rCanvas = new Canvas();
            Viewbox vVB = new Viewbox();

            vCanvas.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            vCanvas.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            vCanvas.Background = Brushes.Olive;
            vCanvas.ClipToBounds = true;
            vCanvas.Children.Add(vVB);
            vVB.Stretch = Stretch.UniformToFill;
            vVB.Child = verso;

            vScatterItem.Content = vCanvas;
            rScatterItem.Content = rCanvas;

            vScatterView.Items.Add(vScatterItem);
            rScatterView.Items.Add(rScatterItem);

            BitmapCache mybitmapCache = new BitmapCache(1);
            mybitmapCache.SnapsToDevicePixels = true;
            verso.CacheMode = mybitmapCache;
            recto.CacheMode = mybitmapCache;

            verso.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            verso.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            recto.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            recto.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            tabArray.Insert(count, new Tab(1, tab, verso, recto, can, vScatterItem, rScatterItem, c_s, delBtn));
            
            can.Children.Add(vScatterView);
            can.Children.Add(rScatterView);
            can.Children.Add(c_s);
            tab.Content = can;
            tabBar.Items.Insert(tabArray.Count - 1, tab);
            tabBar.SelectedItem = tab;
            loadPage();
            return tab;
        }

        private Tab currentTab()
        {
            return tabArray[tabNumber];
        }

        private void tabBar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tab = tabBar.SelectedItem as TabItem;

            if (tab != null && tab.Header != null && tab.Header.Equals("+"))
            {
                TabItem newTab = createTab(1);
            }

            for (int i = 0; i < tabArray.Count; i++)
            {
                tabArray[tabNumber]._delButton.Visibility = System.Windows.Visibility.Collapsed;
                if (tabArray[i]._tab.Equals(tab))
                    tabNumber = i;
                
            }
            tabArray[tabNumber]._delButton.Visibility = System.Windows.Visibility.Visible;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (tabBar.Items.Count > 2)
            {
                TabItem selectedTab = tabBar.SelectedItem as TabItem;
                tabBar.Items.Remove(tabBar.Items[tabNumber]);
                tabArray.Remove(tabArray[tabNumber]);
            }
        }
    }
}