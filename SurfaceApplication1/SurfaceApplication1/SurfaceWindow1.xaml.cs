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
using System.Windows.Media.Animation;
using System.Threading;
using System.ComponentModel;

namespace SurfaceApplication1
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        public static int maxPageWidth = 5250;
        public static int maxPageHeight = 7350;
        public static int minPageWidth = 664;
        public static int minPageHeight = 930;
        public static int tabNumber = 0;
        public static int minPage = 0;
        public static int maxPage = 88;
        public static Image slideImage1, slideImage2;
        int scatterBuffer = 3000;
        List<Tab> tabArray = new List<Tab>();
        Workers workers = new Workers();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

            // slider actions
            pageSlider.AddHandler(UIElement.ManipulationDeltaEvent, new EventHandler<ManipulationDeltaEventArgs>(slider_ManipulationDelta), true);
            pageSlider.AddHandler(UIElement.ManipulationCompletedEvent, new EventHandler<ManipulationCompletedEventArgs>(slider_ManipulationCompleted), true);
            slideImage1 = SliderImage1;
            slideImage2 = SliderImage2;

            //other initialization
            maxPageText.Text = maxPage.ToString();
            TabItem newTabButton = new TabItem();
            newTabButton.Header = "+";
            tabBar.Items.Add(newTabButton);
            createTab(1);

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
            goToPage(tabArray[tabNumber].pageNumber - 1);
        }


        private void next_Click(object sender, RoutedEventArgs e)
        {
            goToPage(tabArray[tabNumber].pageNumber + 1);
        }

        private void goToPage(int page)
        {
            tabArray[tabNumber].pageNumber = page;
            if (tabArray[tabNumber].pageNumber > maxPage)
                tabArray[tabNumber].pageNumber = maxPage;
            if (tabArray[tabNumber].pageNumber < minPage)
                tabArray[tabNumber].pageNumber = minPage;
            loadPage();
        }

        private void loadPage()
        {
            tabArray[tabNumber]._vSVI.Width = minPageWidth;
            tabArray[tabNumber]._vSVI.Height = minPageHeight;
            tabArray[tabNumber]._rSVI.Width = minPageWidth;
            tabArray[tabNumber]._rSVI.Height = minPageHeight;

            int pageNumber = tabArray[tabNumber].pageNumber;
            int versoNum = 2 * pageNumber + 10;
            int rectoNum = 2 * pageNumber + 11;

            Image verso = tabArray[tabNumber]._verso;
            Image recto = tabArray[tabNumber]._recto;

            if (!workers.versoImageChange.CancellationPending)
                workers.versoImageChange.CancelAsync();
            if (!workers.rectoImageChange.CancellationPending)
                workers.rectoImageChange.CancelAsync();

            if (!workers.versoImageChange.IsBusy)
                workers.updateVersoImage(currentTab(), false);

            if (!workers.rectoImageChange.IsBusy)
                workers.updateRectoImage(currentTab(), false);

            pageNumberText.Text = pageNumber.ToString();
            tabArray[tabNumber]._tab.Header = (pageNumber-1).ToString() + "v / " + pageNumber.ToString() + "r";

            pageSlider.Value = currentTab().pageNumber;
            
        }

        private TabItem createTab(int page)
        {
            int buffer = scatterBuffer;
            int count = tabArray.Count;
            TabItem tab = new TabItem();
            tab.Header = string.Format("0v / 1r");
            tab.FontSize = 25;
            tab.Name = string.Format("tabItem_{0}", count);
            tab.HeaderTemplate = tabBar.FindResource("TabHeader") as DataTemplate;

            Button delBtn = new Button();

            Canvas can = new Canvas();
            Canvas c_v = new Canvas();
            Canvas c_r = new Canvas();
            Canvas c_s = new Canvas();
            Image verso = new Image();
            Image recto = new Image();
            verso.Stretch = Stretch.UniformToFill;
            recto.Stretch = Stretch.UniformToFill;
            SSC.ScatterView vScatterView = new SSC.ScatterView();
            SSC.ScatterView rScatterView = new SSC.ScatterView();

            vScatterView.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            vScatterView.ClipToBounds = true;
            rScatterView.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            rScatterView.ClipToBounds = true;
            rScatterView.Margin = new System.Windows.Thickness(minPageWidth, 0, 0, 0);
            c_s.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            c_s.Width = 560;
            c_s.ClipToBounds = true;
            c_v.Width = minPageWidth;
            c_v.Height = minPageHeight;
            c_r.Width = minPageWidth;
            c_r.Height = minPageHeight;
            c_r.Margin = new Thickness(minPageWidth, 0, 0, 0);
            c_v.ClipToBounds = true;
            c_r.ClipToBounds = true;
            
            vScatterView.Width = minPageWidth + 2 * buffer;
            vScatterView.Height = minPageHeight + 2 * buffer;
            vScatterView.Margin = new Thickness(-buffer, -buffer, 0, 0);
            rScatterView.Margin = new Thickness(-buffer, -buffer, 0, 0);
            vScatterView.Background = Brushes.Maroon;
            rScatterView.Width = minPageWidth + 2 * buffer;
            rScatterView.Height = minPageHeight + 2 * buffer;
            rScatterView.Background = Brushes.Navy;

            SSC.ScatterViewItem vScatterItem = new SSC.ScatterViewItem();
            SSC.ScatterViewItem rScatterItem = new SSC.ScatterViewItem();
            vScatterItem.IsManipulationEnabled = true;
            rScatterItem.IsManipulationEnabled = true;
            vScatterItem.SizeChanged += new SizeChangedEventHandler(scatter_SizeChanged);
            rScatterItem.SizeChanged += new SizeChangedEventHandler(scatter_SizeChanged);
            vScatterItem.AddHandler(UIElement.ManipulationDeltaEvent, new EventHandler<ManipulationDeltaEventArgs>(scatter_ManipulationDelta), true);
            rScatterItem.AddHandler(UIElement.ManipulationDeltaEvent, new EventHandler<ManipulationDeltaEventArgs>(scatter_ManipulationDelta), true);
            vScatterItem.AddHandler(UIElement.ManipulationCompletedEvent, new EventHandler<ManipulationCompletedEventArgs>(scatter_ManipulationCompleted), true);
            rScatterItem.AddHandler(UIElement.ManipulationCompletedEvent, new EventHandler<ManipulationCompletedEventArgs>(scatter_ManipulationCompleted), true);

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
            vScatterItem.Center = new Point(buffer + minPageWidth / 2, buffer + minPageHeight / 2);
            rScatterItem.Center = new Point(buffer + minPageWidth / 2, buffer + minPageHeight / 2);

            vScatterItem.SizeChanged += new SizeChangedEventHandler (makeVersoImageLarge);
            rScatterItem.SizeChanged += new SizeChangedEventHandler (makeRectoImageLarge);

            Canvas vCanvas = new Canvas();
            Canvas rCanvas = new Canvas();
            Viewbox vVB = new Viewbox();

            vCanvas.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            vCanvas.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            vCanvas.Background = Brushes.Olive;
            vCanvas.ClipToBounds = true;
            vCanvas.Children.Add(vVB);
            vVB.Stretch = Stretch.UniformToFill;

            vScatterItem.Content = verso;
            rScatterItem.Content = recto;

            c_v.Children.Add(vScatterView);
            c_r.Children.Add(rScatterView);
            vScatterView.Items.Add(vScatterItem);
            rScatterView.Items.Add(rScatterItem);

            verso.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            verso.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            recto.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            recto.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            tabArray.Insert(count, new Tab(1, tab, verso, recto, can, vScatterItem, rScatterItem, c_s, delBtn));
            
            can.Children.Add(c_v);
            can.Children.Add(c_r);
            can.Children.Add(c_s);
            tab.Content = can;
            tabBar.Items.Insert(tabArray.Count - 1, tab);
            tabBar.SelectedIndex = tabArray.Count - 1;

            int pageNumber = tabArray[tabNumber].pageNumber;
            int versoNum = 2 * pageNumber + 10;
            int rectoNum = 2 * pageNumber + 11;

            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri("pack://application:,,,/cow.jpg", UriKind.Absolute);
            src.EndInit();

            verso.Source = src;
            recto.Source = src;

            pageNumberText.Text = pageNumber.ToString();
            currentTab()._tab.Header = (pageNumber - 1).ToString() + "v / " + pageNumber.ToString() + "r";

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

            loadPage();
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

        private void slider_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            int height = 850;
            SurfaceSlider slider = (SurfaceSlider)sender;
            double val = slider.Value;
            int onVal = (int)Math.Round(val);
            SliderText.Text = (onVal).ToString();

            SliderText.Margin = new Thickness(SliderDisplay.Width / 2 - SliderText.ActualWidth / 2, -20, 0, 0);
            double middle = 1160 + (slider.Width - 30) * onVal / slider.Maximum;
            SliderDisplay.Margin = new Thickness(middle, height, 0, 0);
            SliderDisplay.Opacity = 1;

            if (!workers.slideImageChange.CancellationPending)
                workers.slideImageChange.CancelAsync();
            if (!workers.slideImageChange.IsBusy)
                workers.updateSlideImage(onVal);
        }

        /*
         * Called when the slider is released. Goes to the page and hides the slider display
         */
        private void slider_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            SliderDisplay.Opacity = 0;
            SurfaceSlider slider = (SurfaceSlider)sender;
            goToPage((int)Math.Round(slider.Value));
        }

        /*
         * Make the verso page load a high-res image if the user zooms.
         */
        private void makeVersoImageLarge(object sender, SizeChangedEventArgs e)
        {
            if (!workers.bigV)
            {
                ScatterViewItem item = (ScatterViewItem)sender;
                if (item.ActualWidth > minPageWidth)
                {
                    if (!workers.versoImageChange.CancellationPending)
                        workers.versoImageChange.CancelAsync();
                    if (!workers.rectoImageChange.IsBusy)
                        workers.updateVersoImage(currentTab(), true);
                }
            }
        }

        /*
         * Make the recto page load a high-res image if the user zooms.
         */
        private void makeRectoImageLarge(object sender, SizeChangedEventArgs e)
        {
            if (!workers.bigR)
            {
                ScatterViewItem item = (ScatterViewItem)sender;
                if (item.ActualWidth > minPageWidth)
                {
                    if (!workers.rectoImageChange.CancellationPending)
                        workers.rectoImageChange.CancelAsync();
                    if (!workers.rectoImageChange.IsBusy)
                        workers.updateRectoImage(currentTab(), true);
                }
            }
        }

        /*
         * These methods are called when the pages are manipulated. They call limitScatter.
         */
        private void scatter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            limitScatter((ScatterViewItem)sender);
        }
        private void scatter_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            limitScatter((ScatterViewItem)sender);
        }
        private void scatter_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            limitScatter((ScatterViewItem)sender);
        }

        /*
         * Prevents the page images from showing the background of their bounding boxes.
         */
        private void limitScatter(ScatterViewItem i)
        {
            double x,y,w,h;
            x = i.Center.X;
            y = i.Center.Y;
            w = i.ActualWidth / 2;
            h = i.ActualHeight / 2;
            if (x > scatterBuffer + w)
                i.Center = new Point(scatterBuffer + w, y);
            if (x < scatterBuffer + minPageWidth - w)
                i.Center = new Point(scatterBuffer + minPageWidth - w, y);

            x = i.Center.X;
            y = i.Center.Y;
            w = i.ActualWidth / 2;
            h = i.ActualHeight / 2;
            if (y > scatterBuffer + h)
                i.Center = new Point(x, scatterBuffer + h);
            if (y < scatterBuffer + minPageHeight - h)
                i.Center = new Point(x, scatterBuffer + minPageHeight - h);
        }

        /*
         * Prevents the page slider from having intertia.
         */
        private void pageSliderFlicked(object sender, SSC.Primitives.FlickEventArgs e)
        {
            e.Handled = true;
        }

    }
}