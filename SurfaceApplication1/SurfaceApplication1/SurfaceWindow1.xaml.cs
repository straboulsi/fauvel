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
        int maxPageWidth = 5250;
        int maxPageHeight = 7350;
        int minPageWidth = 680;
        int minPageHeight = 930;
        int tabNumber = 0;
        int minPage = 0;
        int maxPage = 88;
        bool wantBigR = false;
        bool wantBigV = false;
        int scatterBuffer = 3000;
        List<Tab> tabArray = new List<Tab>();
        BackgroundWorker changeVersoImageBackground = new BackgroundWorker();
        BackgroundWorker changeRectoImageBackground = new BackgroundWorker();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

            changeVersoImageBackground.WorkerSupportsCancellation = true;
            changeRectoImageBackground.WorkerSupportsCancellation = true;

            changeVersoImageBackground.DoWork += (s, e) =>
            {
                e.Result = null;
                int pn = 2 * tabArray[tabNumber].pageNumber + 10;
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                if(!wantBigV)
                    image.DecodePixelWidth = minPageWidth;
                image.CacheOption = BitmapCacheOption.OnDemand;
                if (changeVersoImageBackground.CancellationPending) return;
                image.UriSource = new Uri("pages/" + pn.ToString() + ".jpg", UriKind.Relative);
                if (changeVersoImageBackground.CancellationPending) return;
                image.EndInit();
                image.Freeze();
                e.Result = image;
            };
            changeVersoImageBackground.RunWorkerCompleted += (s, e) =>
            {
                if (e.Result != null)
                {
                    BitmapImage bitmapImage = e.Result as BitmapImage;
                    tabArray[tabNumber]._verso.Source = bitmapImage;
                    changeVersoImageBackground.Dispose();
                }
                else
                {
                    changeVersoImageBackground.Dispose();
                    changeVersoImageBackground.RunWorkerAsync();
                }
            };

            changeRectoImageBackground.DoWork += (s, e) =>
            {
                int pn = 2 * tabArray[tabNumber].pageNumber + 11;
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                if (!wantBigR)
                    image.DecodePixelWidth = minPageWidth;
                image.CacheOption = BitmapCacheOption.OnDemand;
                if (changeRectoImageBackground.CancellationPending) return;
                image.UriSource = new Uri("pages/" + pn.ToString() + ".jpg", UriKind.Relative);
                if (changeRectoImageBackground.CancellationPending) return;
                image.EndInit();
                image.Freeze();
                e.Result = image;
            };
            changeRectoImageBackground.RunWorkerCompleted += (s, e) =>
            {
                if (e.Result != null)
                {
                    BitmapImage bitmapImage = e.Result as BitmapImage;
                    tabArray[tabNumber]._recto.Source = bitmapImage;
                    changeRectoImageBackground.Dispose();
                }
                else
                {
                    changeRectoImageBackground.Dispose();
                    changeRectoImageBackground.RunWorkerAsync();
                }
            };

            // slider actions
            pageSlider.AddHandler(UIElement.ManipulationDeltaEvent, new EventHandler<ManipulationDeltaEventArgs>(slider_ManipulationDelta), true);
            pageSlider.AddHandler(UIElement.ManipulationCompletedEvent, new EventHandler<ManipulationCompletedEventArgs>(slider_ManipulationCompleted), true);

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

            if (!changeVersoImageBackground.CancellationPending)
                changeVersoImageBackground.CancelAsync();
            if (!changeRectoImageBackground.CancellationPending)
                changeRectoImageBackground.CancelAsync();

            wantBigR = false;
            wantBigV = false;

            if(!changeVersoImageBackground.IsBusy)
                changeVersoImageBackground.RunWorkerAsync();

            if (!changeRectoImageBackground.IsBusy)
                changeRectoImageBackground.RunWorkerAsync();

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
            //vVB.Child = verso;

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
            tabBar.SelectedItem = tab;

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
            tabArray[tabNumber]._tab.Header = (pageNumber - 1).ToString() + "v / " + pageNumber.ToString() + "r";

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
            //SliderDisplay1.Width = 60;
            //SliderDisplay2.Width = 80;
            SliderDisplay3.Width = 100;
            //SliderDisplay4.Width = 80;
            //SliderDisplay5.Width = 60;
            //SliderImage1.Width = SliderDisplay1.Width;
            //SliderImage2.Width = SliderDisplay2.Width;
            SliderImage3.Width = SliderDisplay3.Width;
            //SliderImage4.Width = SliderDisplay4.Width;
            //SliderImage5.Width = SliderDisplay5.Width;

            //SliderImage3.Source = getPageImage(onVal.ToString());

            SliderText3.Text = (onVal).ToString();
            SliderText3.FontSize = 50;
            SliderText3.Foreground = Brushes.Black;
            SliderText3.Margin = new Thickness(SliderDisplay3.Width / 2 - SliderText3.ActualWidth / 2, -20, 0, 0);
            double middle = 1210 + (slider.Width - 30) * onVal / slider.Maximum;
            //SliderDisplay1.Margin = new Thickness(middle - 160, height, 0, 0);
            //SliderDisplay2.Margin = new Thickness(middle - 90, height, 0, 0);
            SliderDisplay3.Margin = new Thickness(middle, height, 0, 0);
            //SliderDisplay4.Margin = new Thickness(middle + 110, height, 0, 0);
            //SliderDisplay5.Margin = new Thickness(middle + 200, height, 0, 0);
            DoubleAnimation anim = new DoubleAnimation();
            anim.Duration = new Duration(TimeSpan.FromSeconds(.05));
            anim.From = 1;
            anim.To = 1;
            //SliderDisplay1.BeginAnimation(OpacityProperty, anim);
            //SliderDisplay2.BeginAnimation(OpacityProperty, anim);
            SliderDisplay3.BeginAnimation(OpacityProperty, anim);
            //SliderDisplay4.BeginAnimation(OpacityProperty, anim);
            //SliderDisplay5.BeginAnimation(OpacityProperty, anim);
        }

        private void slider_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation();
            anim.Duration = new Duration(TimeSpan.FromSeconds(.5));
            anim.From = 1;
            anim.To = 0;
            //SliderDisplay1.BeginAnimation(OpacityProperty, anim);
            //SliderDisplay2.BeginAnimation(OpacityProperty, anim);
            SliderDisplay3.BeginAnimation(OpacityProperty, anim);
            //SliderDisplay4.BeginAnimation(OpacityProperty, anim);
            //SliderDisplay5.BeginAnimation(OpacityProperty, anim);
            SurfaceSlider slider = (SurfaceSlider)sender;
            goToPage((int)Math.Round(slider.Value));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            changeVersoImageBackground.RunWorkerAsync();
        }

        private void makeVersoImageLarge(object sender, SizeChangedEventArgs e)
        {
            if (!wantBigV)
            {
                ScatterViewItem item = (ScatterViewItem)sender;
                if (item.ActualWidth > minPageWidth)
                {
                    if (!changeVersoImageBackground.CancellationPending)
                        changeVersoImageBackground.CancelAsync();
                    wantBigV = true;
                    if(!changeVersoImageBackground.IsBusy)
                        changeVersoImageBackground.RunWorkerAsync();
                }
            }
        }

        private void makeRectoImageLarge(object sender, SizeChangedEventArgs e)
        {
            if (!wantBigR)
            {
                ScatterViewItem item = (ScatterViewItem)sender;
                if (item.ActualWidth > minPageWidth)
                {
                    if (!changeRectoImageBackground.CancellationPending)
                        changeRectoImageBackground.CancelAsync();
                    wantBigR = true;
                    if (!changeRectoImageBackground.IsBusy)
                        changeRectoImageBackground.RunWorkerAsync();
                }
            }
        }


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

        private void pageSliderFlicked(object sender, SSC.Primitives.FlickEventArgs e)
        {
            e.Handled = true;
        }

    }
}