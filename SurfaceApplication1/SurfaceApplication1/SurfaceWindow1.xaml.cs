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
using System.Diagnostics;

namespace SurfaceApplication1
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        private readonly Stopwatch doubleTapSW = new Stopwatch();
        private Point lastTapLocation;
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

        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }


        private void prev_Click(object sender, RoutedEventArgs e)
        {
            goToPage(tabArray[tabNumber]._page - 1);
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            goToPage(tabArray[tabNumber]._page + 1);
        }

        private void goToPage(int page)
        {
            currentTab()._page = page;
            if (page > maxPage)
                tabArray[tabNumber]._page = maxPage;
            if (tabArray[tabNumber]._page < minPage)
                tabArray[tabNumber]._page = minPage;
            loadPage();
        }

        private void loadPage()
        {
            Tab currentTab = this.currentTab();

            foreach (Grid g in currentTab._gridsV)
                currentTab._vGrid.Children.Remove(g);
            foreach (Grid g in currentTab._gridsR)
                currentTab._rGrid.Children.Remove(g);

            currentTab._translationBoxesV = Translate.getBoxes(currentTab._page);
            currentTab._translationBoxesR = Translate.getBoxes(currentTab._page + 1);

            foreach(TranslationBox tb in currentTab._translationBoxesV){
                Grid g = new Grid();
                TextBlock t = new TextBlock();
                double width, x, y, height;
                x = tb.getTopLeft().X;
                y = tb.getTopLeft().Y;
                width = tb.getBottomRight().X - tb.getTopLeft().X;
                height = tb.getBottomRight().Y - tb.getTopLeft().Y;

                ColumnDefinition c1 = new ColumnDefinition();
                c1.Width = new GridLength(x, GridUnitType.Star);
                ColumnDefinition c2 = new ColumnDefinition();
                c2.Width = new GridLength(width, GridUnitType.Star);
                ColumnDefinition c3 = new ColumnDefinition();
                c3.Width = new GridLength(maxPageWidth - x - width, GridUnitType.Star);
                RowDefinition r1 = new RowDefinition();
                r1.Height = new GridLength(y, GridUnitType.Star);
                RowDefinition r2 = new RowDefinition();
                r2.Height = new GridLength(height, GridUnitType.Star);
                RowDefinition r3 = new RowDefinition();
                r3.Height = new GridLength(maxPageHeight - y - height, GridUnitType.Star);

                g.ColumnDefinitions.Add(c1);
                g.ColumnDefinitions.Add(c2);
                g.ColumnDefinitions.Add(c3);
                g.RowDefinitions.Add(r1);
                g.RowDefinitions.Add(r2);
                g.RowDefinitions.Add(r3);

                Grid.SetRow(t, 0);
                Grid.SetColumn(t, 0);
                g.Children.Add(t);
                t.Width = width;
                t.Height = height;
                t.Foreground = Brushes.Black;
                t.Background = Brushes.White;

                t.Text = tb.getOldFr();
                currentTab._gridsV.Add(g);
                currentTab._textBlocksV.Add(t);
            }

            currentTab._vSVI.Width = minPageWidth;
            currentTab._vSVI.Height = minPageHeight;
            currentTab._rSVI.Width = minPageWidth;
            currentTab._rSVI.Height = minPageHeight;

            int pageNumber = currentTab._page;
            int versoNum = 2 * pageNumber + 10;
            int rectoNum = 2 * pageNumber + 11;

            Image verso = currentTab._verso;
            Image recto = currentTab._recto;

            if (!workers.versoImageChange.CancellationPending)
                workers.versoImageChange.CancelAsync();
            if (!workers.rectoImageChange.CancellationPending)
                workers.rectoImageChange.CancelAsync();

            if (!workers.versoImageChange.IsBusy)
                workers.updateVersoImage(currentTab, false);

            if (!workers.rectoImageChange.IsBusy)
                workers.updateRectoImage(currentTab, false);

            pageNumberText.Text = pageNumber.ToString();
            currentTab._tab.Header = (pageNumber - 1).ToString() + "v / " + pageNumber.ToString() + "r";

            pageSlider.Value = currentTab._page;
            
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
            SSC.ScatterViewItem vScatterItem = new SSC.ScatterViewItem();
            SSC.ScatterViewItem rScatterItem = new SSC.ScatterViewItem();

            vScatterItem.PreviewTouchDown += new EventHandler<TouchEventArgs>(OnPreviewTouchDown);
            rScatterItem.PreviewTouchDown += new EventHandler<TouchEventArgs>(OnPreviewTouchDown);
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
            vScatterView.Background = Brushes.White;
            rScatterView.Width = minPageWidth + 2 * buffer;
            rScatterView.Height = minPageHeight + 2 * buffer;
            rScatterView.Background = Brushes.White;
            vScatterItem.IsManipulationEnabled = true;
            rScatterItem.IsManipulationEnabled = true;
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

            Grid vGrid = new Grid();
            Grid rGrid = new Grid();

            vScatterItem.Content = vGrid;
            rScatterItem.Content = rGrid;
            vGrid.Children.Add(verso);
            rGrid.Children.Add(recto);
            verso.Stretch = Stretch.UniformToFill;
            recto.Stretch = Stretch.UniformToFill;

            c_v.Children.Add(vScatterView);
            c_r.Children.Add(rScatterView);
            vScatterView.Items.Add(vScatterItem);
            rScatterView.Items.Add(rScatterItem);

            tabArray.Insert(count, new Tab(1, tab, verso, recto, can, vGrid, rGrid, c_s, delBtn));
            
            can.Children.Add(c_v);
            can.Children.Add(c_r);
            can.Children.Add(c_s);
            tab.Content = can;
            tabBar.Items.Insert(tabArray.Count - 1, tab);
            tabBar.SelectedIndex = tabArray.Count - 1;

            int pageNumber = tabArray[tabNumber]._page;
            int versoNum = 2 * pageNumber + 10;
            int rectoNum = 2 * pageNumber + 11;

            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri("pack://application:,,,/loading.gif", UriKind.Absolute);
            src.EndInit();

            verso.Source = src;
            recto.Source = src;

            pageNumberText.Text = pageNumber.ToString();
            currentTab()._tab.Header = (pageNumber - 1).ToString() + "v / " + pageNumber.ToString() + "r";

            return tab;
        }

        /*
         * returns the tab that the user is currently using
         */
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

        protected virtual void OnDoubleTouchDown(ScatterViewItem s)
        {
            s.Width = (double)maxPageWidth / 2;
            s.Height = (double)maxPageHeight / 2;
        }

        private bool IsDoubleTap(TouchEventArgs e)
        {
            Point currentTapLocation = e.GetTouchPoint(this).Position;
            bool tapsAreCloseInDistance = Math.Abs(currentTapLocation.X - lastTapLocation.X) < 20 && Math.Abs(currentTapLocation.Y - lastTapLocation.Y) < 20;
            lastTapLocation = currentTapLocation;

            TimeSpan elapsed = doubleTapSW.Elapsed;
            doubleTapSW.Restart();
            bool tapsAreCloseInTime = (elapsed != TimeSpan.Zero && elapsed < TimeSpan.FromSeconds(0.7));

            return tapsAreCloseInDistance && tapsAreCloseInTime;
        }

        private void OnPreviewTouchDown(object sender, TouchEventArgs e)
        {
            if (IsDoubleTap(e))
                OnDoubleTouchDown((ScatterViewItem)sender);
        }

    }
}