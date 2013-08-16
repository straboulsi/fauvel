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
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Windows.Threading;

namespace DigitalFauvel
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        private bool rightSwipe = false, leftSwipe = false, dtOut = false; // dtOut is double tap to zoom out
        public static System.Windows.Media.Brush glowColor = Brushes.Orange; // Used in SearchTab to draw attention to changed search settings
        public enum language { None = 0, OldFrench = 1, French = 2, English = 3 }; 
        List<Tab> tabArray = new List<Tab>();
        SideBar sideBar;
        private Stopwatch rightSwipeWatch, leftSwipeWatch;
        private readonly Stopwatch doubleTapSW = new Stopwatch();
        private Point lastTapLocation, leftSwipeStart, rightSwipeStart;
        public static Image slideImage1, slideImage2;
        public int scatterBuffer = 5000, swipeLength = 25, swipeHeight = 6;
        public static int maxPageWidth = 5250, maxPageHeight = 7350, minPageWidth = 650, minPageHeight = 910, minPageLong = 1274, 
            tabNumber = 0, minPage = 0, maxPage = 95;
        public static XmlDocument xml, engXml, layoutXml, modFrXml;
        public TextBlock testText;


        
        public SurfaceWindow1()
        {
            InitializeComponent();

            sideBar = new SideBar(this, tabDynamic);
            testText = pageNumberText;

            try
            {
                // Loads the Xml documents
                xml = new XmlDocument();
                xml.Load(@"..\..\XML\OriginalTextXML.xml");
                engXml = new XmlDocument();
                engXml.Load(@"..\..\XML\EnglishXML.xml");
                layoutXml = new XmlDocument();
                layoutXml.Load(@"..\..\XML\LayoutXML.xml");
                modFrXml = new XmlDocument();
                modFrXml.Load(@"..\..\XML\ModernFrenchXML.xml");
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
            }




            // slider actions
            pageSlider.AddHandler(UIElement.ManipulationDeltaEvent, new EventHandler<ManipulationDeltaEventArgs>(slider_ManipulationDelta), true);
            pageSlider.AddHandler(UIElement.ManipulationCompletedEvent, new EventHandler<ManipulationCompletedEventArgs>(slider_ManipulationCompleted), true);
            slideImage1 = SliderImage1;
            slideImage2 = SliderImage2;

            //other initialization
            TabItem newTabButton = new TabItem();
            newTabButton.Header = "+";
            newTabButton.Width = 50;
            newTabButton.Height = 40;
            newTabButton.FontSize = 25;
            newTabButton.FontFamily = new FontFamily("Cambria");
            tabBar.Items.Add(newTabButton);

            Counterpart.makeCounterpartList();
            createTab(2);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        private void showBoxes(object sender, RoutedEventArgs e)
        {
            if (currentTab()._rBoxesGrid.Visibility == System.Windows.Visibility.Visible)
                currentTab()._rBoxesGrid.Visibility = System.Windows.Visibility.Hidden;
            else
                currentTab()._rBoxesGrid.Visibility = System.Windows.Visibility.Visible;

            if (currentTab()._vBoxesGrid.Visibility == System.Windows.Visibility.Visible)
                currentTab()._vBoxesGrid.Visibility = System.Windows.Visibility.Hidden;
            else
                currentTab()._vBoxesGrid.Visibility = System.Windows.Visibility.Visible;
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            Tab tab = currentTab();
            if (tab._page != 0)
            {
                int newPage = tab._page - 2;
                newPage -= newPage % 2;
                goToPage(newPage);
            }
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            Tab tab = currentTab();
            if (tab._page != 94)
            {
                int newPage = tab._page + 2;
                newPage -= newPage % 2;
                goToPage(newPage);
            }
        }

        private void goToPage(int page)
        {
            Tab tab = currentTab();

            tab._page = page;
            if (page > maxPage)
                tab._page = maxPage - 1;
            if (tab._page < minPage)
                tab._page = minPage;
            loadPage();
        }

        private void loadPage()
        {
            Tab currentTab = this.currentTab();
            
            currentTab._SVI.Width = currentTab._SVI.MinWidth;
            currentTab._SVI.Height = currentTab._SVI.MinHeight;
            currentTab._SVI.Center = new Point(currentTab._SV.Width / 2, currentTab._SV.Height / 2);

            int pageNumber = currentTab._page;

            Image verso = currentTab._verso;
            Image recto = currentTab._recto;

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("smallpages/" + (currentTab._page + 10).ToString() + ".jpg", UriKind.Relative);
            image.EndInit();
            image.Freeze();
            verso.Source = image;
            BitmapImage image2 = new BitmapImage();
            image2.BeginInit();
            image2.UriSource = new Uri("smallpages/" + (currentTab._page + 11).ToString() + ".jpg", UriKind.Relative);
            image2.EndInit();
            image2.Freeze();
            recto.Source = image2;

            currentTab._worker.updateVersoImage(false);
            currentTab._worker.updateRectoImage(false);

            currentTab._vTranslationGrid.Children.Clear();
            currentTab._rTranslationGrid.Children.Clear();

            currentTab._vBoxesGrid.Children.Clear();
            currentTab._rBoxesGrid.Children.Clear();

            currentTab._worker.updateGhostBoxes();

            currentTab._worker.updateTranslations();

            String pageText = PageNamer.getPageText(currentTab._page);
            pageNumberText.Text = pageText;
            currentTab._headerTB.Text = pageText;

            pageSlider.Value = currentTab._page / 2;

        }

        public TabItem createTab(int page)
        {
            int buffer = scatterBuffer;
            int count = tabArray.Count;
            TabItem tab = new TabItem();
            tab.FontSize = 25;
            tab.Name = string.Format("tabItem_{0}", count);
            DockPanel heda = new DockPanel();
            tab.Header = heda;
            Button delBtn = new Button();
            delBtn.Height = 24;
            delBtn.Width = 24;
            delBtn.Margin = new Thickness(10, 0, 0, 0);
            delBtn.PreviewTouchDown += new EventHandler<TouchEventArgs>(btnDelete_Touch);
            delBtn.Click += new RoutedEventHandler(btnDelete_Click);
            TextBlock hedatext = new TextBlock();
            hedatext.Text = "yo";
            Image img = new Image();
            BitmapImage ex = new BitmapImage();
            ex.BeginInit();
            ex.UriSource = new Uri("icons/ex.png", UriKind.Relative);
            ex.EndInit();
            img.Source = ex;
            img.Width = 16;
            img.Height = 16;
            delBtn.Content = img;

            heda.Children.Add(hedatext);
            heda.Children.Add(delBtn);

            Grid vSwipeGrid = new Grid();
            Grid rSwipeGrid = new Grid();
            Grid vSwipeHolderGrid = new Grid();
            Grid rSwipeHolderGrid = new Grid();
            Grid can = new Grid();
            can.ClipToBounds = true;
            Image verso = new Image();
            Image recto = new Image();
            verso.Stretch = Stretch.UniformToFill;
            recto.Stretch = Stretch.UniformToFill;
            SSC.ScatterView ScatterView = new SSC.ScatterView();
            SSC.ScatterViewItem ScatterItem = new SSC.ScatterViewItem();

            ScatterItem.PreviewTouchDown += new EventHandler<TouchEventArgs>(OnPreviewTouchDown);
            ScatterItem.PreviewTouchDown += new EventHandler<TouchEventArgs>(leftSwipeDetectionStart);
            ScatterItem.PreviewTouchMove += new EventHandler<TouchEventArgs>(leftSwipeDetectionMove);
            ScatterItem.PreviewTouchUp += new EventHandler<TouchEventArgs>(leftSwipeDetectionStop);
            ScatterItem.PreviewTouchDown += new EventHandler<TouchEventArgs>(rightSwipeDetectionStart);
            ScatterItem.PreviewTouchMove += new EventHandler<TouchEventArgs>(rightSwipeDetectionMove);
            ScatterItem.PreviewTouchUp += new EventHandler<TouchEventArgs>(rightSwipeDetectionStop);
            ScatterItem.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(wheelIt);
            ScatterItem.SizeChanged += new SizeChangedEventHandler(changeTextBoxSize);

            ScatterView.Width = 2 * minPageWidth + 2 * buffer;
            ScatterView.Height = minPageHeight + 2 * buffer;
            ScatterView.Margin = new Thickness(-buffer, -buffer, 0, 0);
            ScatterItem.IsManipulationEnabled = true;
            ScatterItem.AddHandler(UIElement.ManipulationDeltaEvent, new EventHandler<ManipulationDeltaEventArgs>(scatter_ManipulationDelta), true);
            ScatterItem.AddHandler(UIElement.ManipulationCompletedEvent, new EventHandler<ManipulationCompletedEventArgs>(scatter_ManipulationCompleted), true);

            ScatterItem.Background = Brushes.Aqua;
            ScatterItem.Width = minPageWidth * 2;
            ScatterItem.Height = minPageHeight;
            ScatterItem.MaxWidth = maxPageWidth * 2;
            ScatterItem.MaxHeight = maxPageHeight;
            ScatterItem.MinWidth = minPageWidth * 2;
            ScatterItem.MinHeight = minPageHeight;
            ScatterItem.CanMove = true;
            ScatterItem.CanRotate = false;
            ScatterItem.CanScale = true;
            ScatterItem.Center = new Point(buffer + minPageWidth, buffer + minPageHeight / 2);

            Grid ScatterGrid = new Grid();
            Grid vGrid = new Grid();
            Grid rGrid = new Grid();
            Grid vTranslationGrid = new Grid();
            Grid rTranslationGrid = new Grid();
            Grid vBoxesGrid = new Grid();
            Grid rBoxesGrid = new Grid();

            rBoxesGrid.Visibility = Visibility.Hidden;
            vBoxesGrid.Visibility = Visibility.Hidden;

            ScatterItem.Content = ScatterGrid;
            ColumnDefinition col1 = new ColumnDefinition();
            col1.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition col2 = new ColumnDefinition();
            col2.Width = new GridLength(1, GridUnitType.Star);
            ScatterGrid.ColumnDefinitions.Add(col1);
            ScatterGrid.ColumnDefinitions.Add(col2);
            Grid.SetRow(vGrid, 0);
            Grid.SetColumn(vGrid, 0);
            Grid.SetRow(rGrid, 1);
            Grid.SetColumn(rGrid, 1);
            ScatterGrid.Children.Add(vGrid);
            ScatterGrid.Children.Add(rGrid);

            vGrid.Children.Add(verso);
            rGrid.Children.Add(recto);
            vGrid.Children.Add(vTranslationGrid);
            rGrid.Children.Add(rTranslationGrid);
            vGrid.Children.Add(vBoxesGrid);
            rGrid.Children.Add(rBoxesGrid);
            vGrid.Children.Add(vSwipeHolderGrid);
            rGrid.Children.Add(rSwipeHolderGrid);

            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition c2 = new ColumnDefinition();
            c2.Width = new GridLength(1, GridUnitType.Star);
            RowDefinition r1 = new RowDefinition();
            r1.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition r2 = new RowDefinition();
            r2.Height = new GridLength(1, GridUnitType.Star);
            rSwipeHolderGrid.ColumnDefinitions.Add(c1);
            rSwipeHolderGrid.ColumnDefinitions.Add(c2);
            rSwipeHolderGrid.RowDefinitions.Add(r1);
            rSwipeHolderGrid.RowDefinitions.Add(r2);
            ColumnDefinition cc1 = new ColumnDefinition();
            cc1.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition cc2 = new ColumnDefinition();
            cc2.Width = new GridLength(1, GridUnitType.Star);
            RowDefinition rr1 = new RowDefinition();
            rr1.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition rr2 = new RowDefinition();
            rr2.Height = new GridLength(1, GridUnitType.Star);
            vSwipeHolderGrid.ColumnDefinitions.Add(cc1);
            vSwipeHolderGrid.ColumnDefinitions.Add(cc2);
            vSwipeHolderGrid.RowDefinitions.Add(rr1);
            vSwipeHolderGrid.RowDefinitions.Add(rr2);
            Grid.SetRow(rSwipeGrid, 2);
            Grid.SetColumn(rSwipeGrid, 2);
            Grid.SetRow(vSwipeGrid, 2);
            Grid.SetColumn(vSwipeGrid, 2);
            rSwipeHolderGrid.Children.Add(rSwipeGrid);
            vSwipeHolderGrid.Children.Add(vSwipeGrid);
            rSwipeGrid.HorizontalAlignment = HorizontalAlignment.Left;
            rSwipeGrid.VerticalAlignment = VerticalAlignment.Top;
            vSwipeGrid.HorizontalAlignment = HorizontalAlignment.Left;
            vSwipeGrid.VerticalAlignment = VerticalAlignment.Top;

            vSwipeGrid.Width = swipeLength;
            vSwipeGrid.Height = swipeHeight;
            rSwipeGrid.Width = swipeLength;
            rSwipeGrid.Height = swipeHeight;
            vSwipeGrid.Visibility = Visibility.Hidden;
            rSwipeGrid.Visibility = Visibility.Hidden;
            vSwipeGrid.Background = Brushes.WhiteSmoke;
            rSwipeGrid.Background = Brushes.WhiteSmoke;
            vSwipeGrid.Children.Add(new Canvas());
            rSwipeGrid.Children.Add(new Canvas());

            can.Children.Add(ScatterView);
            ScatterView.Items.Add(ScatterItem);

            language lang = language.None;
            if(tabArray.Count > tabNumber && currentTab() != null)
                lang = currentTab()._currentLanguage;

            tabArray.Insert(count, new Tab(page, tab, verso, recto, can, vGrid, rGrid, delBtn, ScatterView, ScatterItem, vSwipeGrid, rSwipeGrid, vTranslationGrid, rTranslationGrid, vBoxesGrid, rBoxesGrid, hedatext, lang));

            tab.Content = can;
            tabBar.Items.Insert(tabArray.Count - 1, tab);
            tabBar.SelectedIndex = tabArray.Count - 1;

            loadPage();

            return tab;
        }

        private void wheelIt(object sender, MouseWheelEventArgs e)
        {
            ScatterViewItem svi = (ScatterViewItem)sender;
            Point mPos = e.MouseDevice.GetPosition(svi);
            int d = e.Delta;
            ScatterViewItem item = (ScatterViewItem)sender;
            double pcent = item.Width / item.MinWidth;
            double width = item.Width + 2 * d * pcent;
            double height = item.Height + d * 1.4 * pcent;
            if (height > item.MaxHeight)
                height = item.MaxHeight;
            if (height < item.MinHeight)
                height = item.MinHeight;
            if (width > item.MaxWidth)
                width = item.MaxWidth;
            if (width < item.MinWidth)
                width = item.MinWidth;
            item.Height = height;
            item.Width = width;
            limitScatter(svi);
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
                TabItem newTab = createTab(currentTab()._page);
            }

            tabArray[tabNumber]._delButton.Visibility = Visibility.Collapsed;

            for (int i = 0; i < tabArray.Count; i++)
            {
                if (tabArray[i]._tab.Equals(tab))
                    tabNumber = i;
            }
            tabArray[tabNumber]._delButton.Visibility = Visibility.Visible;
            updateLanguageButton();
        }

        private void btnDelete_Touch(object sender, TouchEventArgs e)
        {
            btnDelete_Click(sender, null);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (tabBar.Items.Count > 2)
            {
                Tab ct = currentTab();
                TabItem ti = ct._tab;
                if (tabNumber >= tabBar.Items.Count - 2)
                    tabBar.SelectedItem = tabArray[tabNumber - 1]._tab;
                else
                    tabBar.SelectedItem = tabArray[tabNumber + 1]._tab;
                
                tabArray.Remove(ct);
                tabBar.Items.Remove(ti);
            }
            TabItem tab = tabBar.SelectedItem as TabItem;
            for (int i = 0; i < tabArray.Count; i++)
            {
                if (tabArray[i]._tab.Equals(tab))
                    tabNumber = i;
            }
        }

        private void slider_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            int height = 850;
            SurfaceSlider slider = (SurfaceSlider)sender;
            double val = slider.Value;
            int onVal = (int)Math.Round(val);
            SliderText.Text = PageNamer.getPageText(2 * onVal);

            SliderText.Margin = new Thickness(SliderDisplay.Width / 2 - SliderText.ActualWidth / 2, -swipeHeight, 0, 0);
            double middle = 1160 + (slider.Width - 30) * onVal / slider.Maximum;
            SliderDisplay.Margin = new Thickness(middle, height, 0, 0);
            SliderDisplay.Visibility = System.Windows.Visibility.Visible;

            SliderImage1.Margin = new Thickness(0, 0, 0, 0);

            currentTab()._worker.updateSlideImage(onVal);
        }

        /*
         * Called when the slider is released. Goes to the page and hides the slider display
         */
        private void slider_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            SliderDisplay.Visibility = System.Windows.Visibility.Hidden;
            SurfaceSlider slider = (SurfaceSlider)sender;
            goToPage((int)(2 * Math.Round(slider.Value)));
        }

        /*
         * These methods are called when the pages are manipulated. They call limitScatter.
         */
        private void scatter_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (!currentTab()._worker.largeVersoLoaded)
                currentTab()._worker.updateVersoImage(true);
            if (!currentTab()._worker.largeRectoLoaded)
                currentTab()._worker.updateRectoImage(true);

            limitScatter((ScatterViewItem)sender);
        }
        private void scatter_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            limitScatter((ScatterViewItem)sender);
        }

        private void changeTextBoxSize(object sender, SizeChangedEventArgs e)
        {
            ScatterViewItem i = (ScatterViewItem)sender;
            Tab tab = currentTab();
            int rCount, vCount;
            if (tab._textBlocksR != null && tab._textBlocksV != null)
            {
                rCount = tab._textBlocksR.Count;
                vCount = tab._textBlocksV.Count;

                double ratio = i.Height / minPageHeight;

                for (int j = 0; j < vCount; j++)
                {
                    TextBlock tb = tab._textBlocksV[j];
                    tb.FontSize = TranslationBox.minFontSize * ratio;
                    tb.LineHeight = tab._translationBoxesV[j].lineHeight * ratio;
                }
                for (int j = 0; j < rCount; j++)
                {
                    TextBlock tb = tab._textBlocksR[j];
                    tb.FontSize = TranslationBox.minFontSize * ratio;
                    tb.LineHeight = tab._translationBoxesR[j].lineHeight * ratio;
                }
            }
        }

        /*
         * Prevents the page images from showing the background of their bounding boxes.
         */
        private void limitScatter(ScatterViewItem i)
        {
            double x, y, w, h;
            bool fullscreen = false;
            x = i.Center.X;
            y = i.Center.Y;
            w = i.ActualWidth / 2;
            h = i.ActualHeight / 2;
            if (x > scatterBuffer + w)
                i.Center = new Point(scatterBuffer + w, y);
            if (x < scatterBuffer + 2 * minPageWidth - w)
                i.Center = new Point(scatterBuffer + 2 * minPageWidth - w, y);

            x = i.Center.X;
            y = i.Center.Y;
            w = i.ActualWidth / 2;
            h = i.ActualHeight / 2;
            if (y > scatterBuffer + h)
                i.Center = new Point(x, scatterBuffer + h);
            if (y < scatterBuffer + minPageHeight - h)
                i.Center = new Point(x, scatterBuffer + minPageHeight - h);

            if (i.Width < i.MinWidth + 1)
                fullscreen = true;

            if (fullscreen)
                dtOut = false;
        }

        /*
         * Prevents the page slider from having intertia.
         */
        private void pageSliderFlicked(object sender, SSC.Primitives.FlickEventArgs e)
        {
            e.Handled = true;
        }

        protected virtual void OnDoubleTouchDown(ScatterViewItem s, Point p)
        {
            if (!dtOut)
            {
                Rect rect = getRectFromPoint(true, p);
                if(!rect.Equals(Rect.Empty))
                {
                    resizePageToRect(rect);
                    dtOut = true;
                }
            }
            else
            {
                resizePagetoSmall(s);
                dtOut = false;
            }
            lastTapLocation = new Point(-1000, -1000);
        }

        private Rect getRectFromPoint(bool verso, Point p)
        {
            if (p.X < maxPageWidth)
            {
                foreach (BoundingBox bb in currentTab()._vGhostBoxes)
                    if (p.X > bb.X && p.Y > bb.Y && p.X < bb.X + bb.Width && p.Y < bb.Y + bb.Height)
                        return new Rect(bb.X, bb.Y, bb.Width, bb.Height);
            }
            else
            {
                foreach (BoundingBox bb in currentTab()._rGhostBoxes)
                    if (p.X > bb.X + maxPageWidth && p.Y > bb.Y && p.X < bb.X + maxPageWidth + bb.Width && p.Y < bb.Y + bb.Height)
                        return new Rect(bb.X + maxPageWidth, bb.Y, bb.Width, bb.Height); 
            }
            return Rect.Empty;
        }

        private void resizePagetoSmall(ScatterViewItem s)
        {
            double endWidth, endHeight;
            endWidth = s.MinWidth;
            endHeight = s.MinHeight;

            double xcenter = ((ScatterView)s.Parent).Width / 2;
            double ycenter = ((ScatterView)s.Parent).Height / 2;

            Storyboard stb = new Storyboard();
            DoubleAnimation moveWidth = new DoubleAnimation();
            DoubleAnimation moveHeight = new DoubleAnimation();
            PointAnimation moveCenter = new PointAnimation();

            Point endPoint = new Point(xcenter, ycenter);
            moveCenter.From = s.ActualCenter;
            moveCenter.To = endPoint;
            moveWidth.From = s.ActualWidth;
            moveWidth.To = endWidth;
            moveHeight.From = s.ActualHeight;
            moveHeight.To = endHeight;
            moveCenter.Duration = new Duration(TimeSpan.FromMilliseconds(150));
            moveWidth.Duration = new Duration(TimeSpan.FromMilliseconds(150));
            moveHeight.Duration = new Duration(TimeSpan.FromMilliseconds(150));
            moveWidth.FillBehavior = FillBehavior.Stop;
            moveHeight.FillBehavior = FillBehavior.Stop;
            moveCenter.FillBehavior = FillBehavior.Stop;
            stb.Children.Add(moveCenter);
            stb.Children.Add(moveWidth);
            stb.Children.Add(moveHeight);
            Storyboard.SetTarget(moveCenter, s);
            Storyboard.SetTarget(moveWidth, s);
            Storyboard.SetTarget(moveHeight, s);
            Storyboard.SetTargetProperty(moveCenter, new PropertyPath(ScatterViewItem.CenterProperty));
            Storyboard.SetTargetProperty(moveWidth, new PropertyPath(ScatterViewItem.WidthProperty));
            Storyboard.SetTargetProperty(moveHeight, new PropertyPath(ScatterViewItem.HeightProperty));
            s.Width = endWidth;
            s.Height = endHeight;
            s.Center = endPoint;
            stb.Begin(this);
        }

        public void resizePageToRect(Rect r)
        {
            ScatterViewItem s = currentTab()._SVI;
            bool sizeToHeight = 2 * r.Height > r.Width * 1.4;
            double h = r.Height;
            double w = r.Width;
            double endWidth, endHeight;
            double multiplier = 1;

            if (sizeToHeight && h > minPageHeight)
                multiplier = minPageHeight / h;
            if (!sizeToHeight && w > minPageWidth * 2)
                multiplier = minPageWidth * 2 / w;

            endWidth = s.MaxWidth * multiplier;
            endHeight = s.MaxHeight * multiplier;
            double x, y, xcenter, ycenter;
            x = (r.Left + r.Width / 2) / maxPageWidth / 2;
            y = (r.Top + r.Height / 2) / maxPageHeight;
            xcenter = ((ScatterView)s.Parent).Width / 2;
            ycenter = ((ScatterView)s.Parent).Height / 2;

            xcenter += (.5 - x) * endWidth;
            ycenter += (.5 - y) * endHeight;

            Point endPoint = new Point(xcenter, ycenter);

            Storyboard stb = new Storyboard();
            DoubleAnimation moveWidth = new DoubleAnimation();
            DoubleAnimation moveHeight = new DoubleAnimation();
            PointAnimation moveCenter = new PointAnimation();
            moveCenter.From = s.Center;
            moveCenter.To = endPoint;
            moveWidth.From = s.Width;
            moveWidth.To = endWidth;
            moveHeight.From = s.Height;
            moveHeight.To = endHeight;
            int milliseconds = 150;
            moveCenter.Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds));
            moveWidth.Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds));
            moveHeight.Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds));
            moveWidth.FillBehavior = FillBehavior.Stop;
            moveHeight.FillBehavior = FillBehavior.Stop;
            moveCenter.FillBehavior = FillBehavior.Stop;
            stb.Children.Add(moveCenter);
            stb.Children.Add(moveWidth);
            stb.Children.Add(moveHeight);
            Storyboard.SetTarget(moveCenter, s);
            Storyboard.SetTarget(moveWidth, s);
            Storyboard.SetTarget(moveHeight, s);
            Storyboard.SetTargetProperty(moveCenter, new PropertyPath(ScatterViewItem.CenterProperty));
            Storyboard.SetTargetProperty(moveWidth,  new PropertyPath(ScatterViewItem.WidthProperty));
            Storyboard.SetTargetProperty(moveHeight, new PropertyPath(ScatterViewItem.HeightProperty));

            s.Width = endWidth;
            s.Height = endHeight;
            s.Center = endPoint;
            stb.Begin(this);

            scatter_ManipulationDelta(s, null);
        }

        private bool IsDoubleTap(TouchEventArgs e)
        {
            Point currentTapLocation = e.GetTouchPoint(this).Position;
            bool tapsAreCloseInDistance = Math.Abs(currentTapLocation.X - lastTapLocation.X) < 30 && Math.Abs(currentTapLocation.Y - lastTapLocation.Y) < 30;
            lastTapLocation = currentTapLocation;

            TimeSpan elapsed = doubleTapSW.Elapsed;
            doubleTapSW.Restart();
            bool tapsAreCloseInTime = (elapsed != TimeSpan.Zero && elapsed < TimeSpan.FromSeconds(0.7));

            bool isDoubleTap = tapsAreCloseInDistance && tapsAreCloseInTime;
            if (isDoubleTap)
                e.Handled = true;
            return isDoubleTap;
        }

        private void OnPreviewTouchDown(object sender, TouchEventArgs e)
        {
            int numTouches = ((ScatterViewItem)sender).TouchesOver.Count();
            if (numTouches > 1)
            {
                lastTapLocation = new Point(-1000, -1000);
            }
            else
            {
                ScatterViewItem item = (ScatterViewItem)sender;
                Point p = e.TouchDevice.GetTouchPoint(item).Position;
                double x = p.X * item.MaxWidth / item.Width;
                double y = p.Y * item.MaxWidth / item.Width;
                Point newPoint = new Point(x,y);
                if (IsDoubleTap(e))
                    OnDoubleTouchDown((ScatterViewItem)sender, newPoint);
                else
                    changeTranslationGrids(newPoint);
            }
        }

        private void OnPreviewTouchUp(object sender, TouchEventArgs e)
        {
        }

        public void changeLanguage(language l)
        {
            Tab tab = currentTab();
            tab._currentLanguage = l;
            updateLanguageButton();
            tab._worker.setTranslateText(tab._currentLanguage);
            makeEnlargedTranslationGridReadable();
        }

        public void changeLanguage(int l)
        {
            Tab tab = currentTab();
            if (l == 1)
                tab._currentLanguage = language.OldFrench;
            else if (l == 2)
                tab._currentLanguage = language.French;
            else if (l == 3)
                tab._currentLanguage = language.English;
            else
                tab._currentLanguage = language.None;
            updateLanguageButton();
            tab._worker.setTranslateText(tab._currentLanguage);
            makeEnlargedTranslationGridReadable();
        }

        private void languageChanged(object sender, SelectionChangedEventArgs e)
        {
            Tab tab = currentTab();
            prevlanguageButton.IsEnabled = true;
            tab._previousLanguage = tab._currentLanguage;
            ListBox box = (ListBox)sender;
            if (box.SelectedItem == null)
                return;
            if (box.SelectedIndex == 0)
                tab._currentLanguage = language.None;
            if (box.SelectedIndex == 1)
                tab._currentLanguage = language.OldFrench;
            if (box.SelectedIndex == 2)
                tab._currentLanguage = language.French;
            if (box.SelectedIndex == 3)
                tab._currentLanguage = language.English;
            updateLanguageButton();
            tab._worker.setTranslateText(tab._currentLanguage);

            languageBox.Visibility = Visibility.Collapsed;
            makeEnlargedTranslationGridReadable();
        }

        public void updateLanguageButton()
        {
            Tab tab = currentTab();
            if (tab._currentLanguage == language.None)
                languageButton.Content = "None";
            if (tab._currentLanguage == language.OldFrench)
                languageButton.Content = "Old French";
            if (tab._currentLanguage == language.French)
                languageButton.Content = "French";
            if (tab._currentLanguage == language.English)
                languageButton.Content = "English";
        }

        public void rightSwipeDetectionStart(object sender, TouchEventArgs e)
        {
            ScatterViewItem item = (ScatterViewItem)sender;
            if (item.Width < minPageWidth * 2 + 2 && item.TouchesOver.Count<TouchDevice>() == 1 && item.TouchesOver.ElementAt<TouchDevice>(0).GetPosition(item).X > minPageWidth + swipeLength)
            {
                rightSwipe = true;
                rightSwipeStart = item.TouchesOver.ElementAt<TouchDevice>(0).GetPosition(item);
                rightSwipeWatch = new Stopwatch();
                rightSwipeWatch.Start();
            }
            else
            {
                rightSwipe = false;
                currentTab()._rSwipeGrid.Visibility = Visibility.Hidden;
            }
        }
        public void rightSwipeDetectionMove(object sender, TouchEventArgs e)
        {
            ScatterViewItem item = (ScatterViewItem)sender;
            if (rightSwipe)
            {
                double x = rightSwipeStart.X - swipeLength - minPageWidth;
                double y = rightSwipeStart.Y - swipeHeight / 2;
                if (x < 0)
                    x = 0;
                if (y < 0)
                    y = 0;
                if (x > minPageWidth)
                    x = minPageWidth;
                if (y > minPageHeight)
                    y = minPageHeight;
                Tab tab = currentTab();
                tab._rSwipeGrid.Visibility = Visibility.Visible;
                Grid holder = (Grid)tab._rSwipeGrid.Parent;
                holder.ColumnDefinitions[0].Width = new GridLength(x, GridUnitType.Star);
                holder.ColumnDefinitions[1].Width = new GridLength(minPageWidth - x, GridUnitType.Star);
                holder.RowDefinitions[0].Height = new GridLength(y, GridUnitType.Star);
                holder.RowDefinitions[1].Height = new GridLength(minPageHeight - y, GridUnitType.Star);
                double dist = rightSwipeStart.X - item.TouchesOver.ElementAt<TouchDevice>(0).GetPosition(item).X;
                Canvas fill = (Canvas)tab._rSwipeGrid.Children[0];
                fill.Background = Brushes.Orange;
                fill.HorizontalAlignment = HorizontalAlignment.Right;
                fill.Height = swipeHeight;
                if (dist < 0)
                    dist = 0;
                if (dist > swipeLength)
                {
                    dist = swipeLength;
                    fill.Background = Brushes.Green;
                }
                fill.Width = dist;
            }
        }
        public void rightSwipeDetectionStop(object sender, TouchEventArgs e)
        {
            rightSwipe = false;
            currentTab()._rSwipeGrid.Visibility = System.Windows.Visibility.Hidden;
            ScatterViewItem item = (ScatterViewItem)sender;
            Point second = item.TouchesOver.ElementAt<TouchDevice>(0).GetPosition(item);
            if (rightSwipe)
            {
                if (((Canvas)currentTab()._rSwipeGrid.Children[0]).Background == Brushes.Green || (Math.Abs(second.X - rightSwipeStart.X) < 10 && Math.Abs(second.Y - rightSwipeStart.Y) < 10 && rightSwipeWatch.ElapsedMilliseconds < 400 && rightSwipeStart.X > 2 * minPageWidth - 150))
                    next_Click(null, null);
            }
        }
        public void leftSwipeDetectionStart(object sender, TouchEventArgs e)
        {
            ScatterViewItem item = (ScatterViewItem)sender;
            if (item.Width < minPageWidth * 2 + 2 && item.TouchesOver.Count<TouchDevice>() == 1 && item.TouchesOver.ElementAt<TouchDevice>(0).GetPosition(item).X < minPageWidth - swipeLength)
            {
                leftSwipe = true;
                leftSwipeStart = item.TouchesOver.ElementAt<TouchDevice>(0).GetPosition(item);
                leftSwipeWatch = new Stopwatch();
                leftSwipeWatch.Start();
            }
            else
            {
                leftSwipe = false;
                currentTab()._vSwipeGrid.Visibility = Visibility.Hidden;
            }
        }
        public void leftSwipeDetectionMove(object sender, TouchEventArgs e)
        {
            ScatterViewItem item = (ScatterViewItem)sender;
            if (leftSwipe)
            {
                double x = leftSwipeStart.X;
                double y = leftSwipeStart.Y - swipeHeight / 2;
                if (x < 0)
                    x = 0;
                if (y < 0)
                    y = 0;
                if (x > minPageWidth)
                    x = minPageWidth;
                if (y > minPageHeight)
                    y = minPageHeight;
                Tab tab = currentTab();
                tab._vSwipeGrid.Visibility = Visibility.Visible;
                Grid holder = (Grid)tab._vSwipeGrid.Parent;
                holder.ColumnDefinitions[0].Width = new GridLength(x, GridUnitType.Star);
                holder.ColumnDefinitions[1].Width = new GridLength(minPageWidth - x, GridUnitType.Star);
                holder.RowDefinitions[0].Height = new GridLength(y, GridUnitType.Star);
                holder.RowDefinitions[1].Height = new GridLength(minPageHeight - y, GridUnitType.Star);
                double dist = item.TouchesOver.ElementAt<TouchDevice>(0).GetPosition(item).X - leftSwipeStart.X;
                Canvas fill = (Canvas)tab._vSwipeGrid.Children[0];
                fill.Background = Brushes.Orange;
                fill.HorizontalAlignment = HorizontalAlignment.Left;
                fill.Height = swipeHeight;
                if (dist < 0)
                    dist = 0;
                if (dist > swipeLength)
                {
                    dist = swipeLength;
                    fill.Background = Brushes.Green;
                }
                fill.Width = dist;
            }
        }
        public void leftSwipeDetectionStop(object sender, TouchEventArgs e)
        {
            leftSwipe = false;
            currentTab()._vSwipeGrid.Visibility = System.Windows.Visibility.Hidden;
            ScatterViewItem item = (ScatterViewItem)sender;
            Point second = item.TouchesOver.ElementAt<TouchDevice>(0).GetPosition(item);
            if (leftSwipe)
            {
                if (((Canvas)currentTab()._vSwipeGrid.Children[0]).Background == Brushes.Green || (Math.Abs(second.X - leftSwipeStart.X) < 10 && Math.Abs(second.Y - leftSwipeStart.Y) < 10 && leftSwipeWatch.ElapsedMilliseconds < 400 && leftSwipeStart.X < 150))
                    prev_Click(null, null);
            }
        }

        public void changeTranslationGrids(Point p)
        {
            Tab tab = currentTab();
            List<TranslationBox> translationBoxes, otherTranslationBoxes;
            Grid translationGrid, otherTranslationGrid;
            if (p.X < maxPageWidth)
            {
                translationBoxes = tab._translationBoxesV;
                translationGrid = tab._vTranslationGrid;
                otherTranslationBoxes = tab._translationBoxesR;
                otherTranslationGrid = tab._rTranslationGrid;
            }
            else
            {
                translationBoxes = tab._translationBoxesR;
                translationGrid = tab._rTranslationGrid;
                otherTranslationBoxes = tab._translationBoxesV;
                otherTranslationGrid = tab._vTranslationGrid;
                p.X -= maxPageWidth;
            }

            double width, x, y, height, truewidth;
            TranslationBox tb;
            TextBlock textBlock;
            Grid g;

            for (int i = 0; i < translationBoxes.Count; i++)
            {
                tb = translationBoxes[i];
                g = (Grid)translationGrid.Children[i];
                textBlock = (TextBlock)g.Children[0];
                truewidth = textBlock.ActualWidth * maxPageHeight / tab._SVI.Height;
                x = tb.getTopLeft().X;
                y = tb.getTopLeft().Y;
                width = (tb.getBottomRight().X - tb.getTopLeft().X);
                height = (tb.getBottomRight().Y - tb.getTopLeft().Y);

                if (p.X > x && p.X < x + width && p.Y > y && p.Y < y + height) // tap on it
                {
                    g.ColumnDefinitions[0].Width = new GridLength(x, GridUnitType.Star);
                    g.ColumnDefinitions[1].Width = new GridLength(maxPageWidth - x, GridUnitType.Star);
                    g.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Star);
                    tb.expanded = true;
                }
                else
                {
                    g.ColumnDefinitions[0].Width = new GridLength(x, GridUnitType.Star);
                    g.ColumnDefinitions[1].Width = new GridLength(width, GridUnitType.Star);
                    g.ColumnDefinitions[2].Width = new GridLength(maxPageWidth - x - width, GridUnitType.Star);
                    tb.expanded = false;
                }
            }

            for (int i = 0; i < otherTranslationBoxes.Count; i++)
            {
                tb = otherTranslationBoxes[i];
                g = (Grid)otherTranslationGrid.Children[i];
                x = tb.getTopLeft().X;
                y = tb.getTopLeft().Y;
                width = (tb.getBottomRight().X - tb.getTopLeft().X);
                height = (tb.getBottomRight().Y - tb.getTopLeft().Y);

                g.ColumnDefinitions[0].Width = new GridLength(x, GridUnitType.Star);
                g.ColumnDefinitions[1].Width = new GridLength(width, GridUnitType.Star);
                g.ColumnDefinitions[2].Width = new GridLength(maxPageWidth - x - width, GridUnitType.Star);
                tb.expanded = false;
            }

            makeEnlargedTranslationGridReadable();
        }

        private void makeEnlargedTranslationGridReadable()
        {
            Tab tab = currentTab();
            List<TranslationBox> translationBoxes, otherTranslationBoxes;
            Grid translationGrid, otherTranslationGrid;

            translationBoxes = tab._translationBoxesV;
            translationGrid = tab._vTranslationGrid;
            otherTranslationBoxes = tab._translationBoxesR;
            otherTranslationGrid = tab._rTranslationGrid;

            double width, x, y, height, truewidth;
            TranslationBox tb;
            TextBlock textBlock;
            Grid g;
            int lowIndex = 0;

            for (int i = 0; i < translationBoxes.Count; i++)
            {
                tb = translationBoxes[i];

                if (tb.expanded)
                {
                    g = (Grid)translationGrid.Children[i];
                    textBlock = (TextBlock)g.Children[0];
                    g.SetValue(Grid.ZIndexProperty, 100);
                    truewidth = textBlock.ActualWidth * maxPageHeight / tab._SVI.Height;
                    x = tb.getTopLeft().X;
                    y = tb.getTopLeft().Y;
                    width = (tb.getBottomRight().X - tb.getTopLeft().X);
                    height = (tb.getBottomRight().Y - tb.getTopLeft().Y);

                    if (truewidth + x > maxPageWidth)
                    {
                        g.ColumnDefinitions[0].Width = new GridLength(maxPageWidth - truewidth, GridUnitType.Star);
                        g.ColumnDefinitions[1].Width = new GridLength(truewidth, GridUnitType.Star);
                    }
                    else
                    {
                        g.ColumnDefinitions[0].Width = new GridLength(x, GridUnitType.Star);
                        g.ColumnDefinitions[1].Width = new GridLength(maxPageWidth - x, GridUnitType.Star);
                    }
                    g.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Star);
                }
                else
                {
                    g = (Grid)translationGrid.Children[i];
                    g.SetValue(Grid.ZIndexProperty, lowIndex++);
                }
            }

            for (int i = 0; i < otherTranslationBoxes.Count; i++)
            {
                tb = otherTranslationBoxes[i];

                if (tb.expanded) // tap on it
                {
                    g = (Grid)otherTranslationGrid.Children[i];
                    textBlock = (TextBlock)g.Children[0];
                    g.SetValue(Grid.ZIndexProperty, 100);
                    truewidth = textBlock.ActualWidth * maxPageHeight / tab._SVI.Height;
                    x = tb.getTopLeft().X;
                    y = tb.getTopLeft().Y;
                    width = (tb.getBottomRight().X - tb.getTopLeft().X);
                    height = (tb.getBottomRight().Y - tb.getTopLeft().Y);

                    if (truewidth + x > maxPageWidth)
                    {
                        g.ColumnDefinitions[0].Width = new GridLength(maxPageWidth - truewidth, GridUnitType.Star);
                        g.ColumnDefinitions[1].Width = new GridLength(truewidth, GridUnitType.Star);
                    }
                    else
                    {
                        g.ColumnDefinitions[0].Width = new GridLength(x, GridUnitType.Star);
                        g.ColumnDefinitions[1].Width = new GridLength(maxPageWidth - x, GridUnitType.Star);
                    }
                    g.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Star);
                }
                else
                {
                    g = (Grid)otherTranslationGrid.Children[i];
                    g.SetValue(Grid.ZIndexProperty, lowIndex++);
                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
            tb.Foreground = Brushes.Black;
        }

        private void pageSlider_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            SurfaceSlider slider = (SurfaceSlider)sender;
            slider.Value = ((e.TouchDevice.GetTouchPoint(slider).Position.X - 16) / (slider.Width - 32)) * slider.Maximum;
            slider_ManipulationDelta(sender, null);
        }

        private void pageSlider_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            slider_ManipulationCompleted(sender, null);
        }

        private void languageVisibility(object sender, TouchEventArgs e)
        {
            languageBox.SelectedItem = null;
            if (languageBox.Visibility == System.Windows.Visibility.Collapsed)
                languageBox.Visibility = System.Windows.Visibility.Visible;
            else
                languageBox.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void languageVisibility(object sender, RoutedEventArgs e)
        {
            languageVisibility(sender, null);
        }
        

        private void languagePrevious(object sender, TouchEventArgs e)
        {
            languageBox.SelectedIndex = (int)currentTab()._previousLanguage;
        }
        private void languagePrevious(object sender, RoutedEventArgs e)
        {
            languagePrevious(sender, null);
        }

        private void savePage(object sender, TouchEventArgs e)
        {
            Tab tab = currentTab();
            int pageNum = tab._page;
            double width = tab._SVI.Width;
            Point center = tab._SVI.Center;
            language lang = tab._currentLanguage;
            sideBar.savePage(pageNum, width, center, lang);
        }
        private void savePage(object sender, RoutedEventArgs e)
        {
            savePage(sender, null);
        }

        public void goToSavedPage(SavedPage sp)
        {
            Tab tab = currentTab();
            goToPage(sp.pageNum);
            tab._SVI.Width = sp.width;
            tab._SVI.Height = tab._SVI.MinHeight * tab._SVI.Width / tab._SVI.MinWidth;
            tab._previousLanguage = tab._currentLanguage;
            tab._currentLanguage = sp.language;
            tab._SVI.Center = sp.center;
        }

    }
}