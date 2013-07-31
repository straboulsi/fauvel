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

namespace SurfaceApplication1
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        private bool rightSwipe = false;
        private bool leftSwipe = false;
        private bool optionsShown = false;
        private Stopwatch rightSwipeWatch, leftSwipeWatch;
        private readonly Stopwatch doubleTapSW = new Stopwatch();
        private Point lastTapLocation;
        private Point leftSwipeStart, rightSwipeStart;
        public static int maxPageWidth = 5250;
        public static int maxPageHeight = 7350;
        public static int minPageWidth = 650;
        public static int minPageHeight = 910;
        public static int minPageLong = 1274;
        public static int tabNumber = 0;
        public static int minPage = 0;
        public static int maxPage = 95;
        public static Image slideImage1, slideImage2;
        int scatterBuffer = 5000;
        int swipeLength = 25;
        int swipeHeight = 6;
        List<Tab> tabArray = new List<Tab>();
        public enum language { None = 0, OldFrench = 1, French = 2, English = 3};
        public static language currentLanguage = language.None;
        public static language previousLanguage = language.None;
        bool dtOut = false; // double tap to zoom out

        private List<TabItem> SidebarTabItems;
        private TabItem tabAdd;
        private Boolean defaultOptionsChanged;
        public static XmlDocument xml, engXml, layoutXml, modFrXml;
        public int veryFirstLine, veryLastLine;

        public enum searchLanguage { oldFrench = 1, modernFrench = 2, English = 3 };
        public static searchLanguage currentSearchLanguage = searchLanguage.oldFrench;

        public String pageToFind; // for opening/navigating to a new tab from a search result closeup


        public SurfaceWindow1()
        {
            InitializeComponent();

            //Search bar initialization
            veryFirstLine = 1;
            veryLastLine = 5986;
            SidebarTabItems = new List<TabItem>();

            // Lots of search sidebar things
            tabAdd = new TabItem();
            tabAdd.Header = "  +  ";
            tabAdd.Height = 40;

            Canvas newTabCanvas = new Canvas();
            newTabCanvas.Height = 899;
            newTabCanvas.Width = 550;
            tabAdd.Content = newTabCanvas;

            Button searchButton = new Button();
            searchButton.Style = tabDynamic.FindResource("RoundButtonTemplate") as Style;
            searchButton.Click += new RoutedEventHandler(SearchButton_Selected);
            searchButton.TouchDown += new EventHandler<TouchEventArgs>(SearchButton_Selected);

            Grid searchGrid = new Grid();

            Image searchIm = new Image();
            searchIm.Source = new BitmapImage(new Uri(@"..\..\icons\magnifyingglass.png", UriKind.Relative));
            searchIm.Style = tabDynamic.FindResource("ButtonImageTemplate") as Style;

            TextBlock searchText = new TextBlock();
            searchText.Text = "SEARCH";
            searchText.Style = tabDynamic.FindResource("ButtonTextTemplate") as Style;

            searchGrid.Children.Add(searchIm);
            searchGrid.Children.Add(searchText);
            searchButton.Content = searchGrid;
            Canvas.SetLeft(searchButton, 68.0);
            Canvas.SetTop(searchButton, 350.0);
            newTabCanvas.Children.Add(searchButton);

            Button annotateButton = new Button();
            annotateButton.Style = tabDynamic.FindResource("RoundButtonTemplate") as Style;
            annotateButton.Click += new RoutedEventHandler(AnnotateButton_Selected);
            annotateButton.TouchDown += new EventHandler<TouchEventArgs>(AnnotateButton_Selected);

            Grid annotateGrid = new Grid();

            Image annotateIm = new Image();
            annotateIm.Source = new BitmapImage(new Uri(@"..\..\icons\pencil.jpg", UriKind.Relative));
            annotateIm.Style = tabDynamic.FindResource("ButtonImageTemplate") as Style;

            TextBlock annotateText = new TextBlock();
            annotateText.Style = tabDynamic.FindResource("ButtonTextTemplate") as Style;
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

            Button studyButton = new Button();
            studyButton.Style = tabDynamic.FindResource("RoundButtonTemplate") as Style;
            studyButton.Click += new RoutedEventHandler(StudyButton_Selected);
            studyButton.TouchDown += new EventHandler<TouchEventArgs>(StudyButton_Selected);


            Grid studyGrid = new Grid();

            Image studyIm = new Image();
            studyIm.Source = new BitmapImage(new Uri(@"..\..\icons\musicnote.png", UriKind.Relative));
            studyIm.Style = tabDynamic.FindResource("ButtonImageTemplate") as Style;

            TextBlock studyText = new TextBlock();
            studyText.Style = tabDynamic.FindResource("ButtonTextTemplate") as Style;
            studyText.Text = "STUDY";

            studyGrid.Children.Add(studyIm);
            studyGrid.Children.Add(studyText);
            studyButton.Content = studyGrid;
            Canvas.SetLeft(studyButton, 370.0);
            Canvas.SetTop(studyButton, 350.0);
            newTabCanvas.Children.Add(studyButton);

            SidebarTabItems.Add(tabAdd);
            tabDynamic.DataContext = SidebarTabItems;
            tabDynamic.SelectedIndex = 0;

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
            tabBar.Items.Add(newTabButton);
            createTab(1);
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
            int newPage = tab._page - 2;
            newPage -= newPage % 2;
            goToPage(newPage);
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            Tab tab = currentTab();
            int newPage = tab._page + 2;
            newPage -= newPage % 2;
            goToPage(newPage);
        }

        private void goToPage(int page)
        {
            Tab tab = currentTab();
            tab._page = page;
            if (page > maxPage)
            {
                tab._page = maxPage - 1;
            }
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

        private TabItem createTab(int page)
        {
            int buffer = scatterBuffer;
            int count = tabArray.Count;
            TabItem tab = new TabItem();
            tab.FontSize = 25;
            tab.Name = string.Format("tabItem_{0}", count);
            DockPanel heda = new DockPanel();
            tab.Header = heda;
            Button delBtn = new Button();
            delBtn.Height = 30;
            delBtn.Width = 30;
            delBtn.Margin = new Thickness(10, 0, 0, 0);
            delBtn.PreviewTouchDown += new EventHandler<TouchEventArgs>(btnDelete_Touch);
            delBtn.Click += new RoutedEventHandler(btnDelete_Click);
            TextBlock hedatext = new TextBlock();
            hedatext.Text = "yo";
            TextBlock ex = new TextBlock();
            ex.Text = "x";
            ex.FontSize = 16;
            delBtn.Content = ex;

            heda.Children.Add(hedatext);
            heda.Children.Add(delBtn);

            Grid vSwipeGrid = new Grid();
            Grid rSwipeGrid = new Grid();
            Grid vSwipeHolderGrid = new Grid();
            Grid rSwipeHolderGrid = new Grid();
            Canvas can = new Canvas();
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

            ScatterView.Width = 2 * minPageWidth + 2 * buffer;
            ScatterView.Height = minPageHeight + 2 * buffer;
            ScatterView.Margin = new Thickness(-buffer, -buffer, 0, 0);
            ScatterItem.IsManipulationEnabled = true;
            ScatterItem.AddHandler(UIElement.ManipulationDeltaEvent, new EventHandler<ManipulationDeltaEventArgs>(scatter_ManipulationDelta), true);
            ScatterItem.AddHandler(UIElement.ManipulationCompletedEvent, new EventHandler<ManipulationCompletedEventArgs>(scatter_ManipulationCompleted), true);

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
            rSwipeGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            rSwipeGrid.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            vSwipeGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            vSwipeGrid.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            vSwipeGrid.Width = swipeLength;
            vSwipeGrid.Height = swipeHeight;
            rSwipeGrid.Width = swipeLength;
            rSwipeGrid.Height = swipeHeight;
            vSwipeGrid.Visibility = System.Windows.Visibility.Hidden;
            rSwipeGrid.Visibility = System.Windows.Visibility.Hidden;
            vSwipeGrid.Background = Brushes.WhiteSmoke;
            rSwipeGrid.Background = Brushes.WhiteSmoke;
            vSwipeGrid.Children.Add(new Canvas());
            rSwipeGrid.Children.Add(new Canvas());

            can.Children.Add(ScatterView);
            ScatterView.Items.Add(ScatterItem);

            tabArray.Insert(count, new Tab(2, tab, verso, recto, can, vGrid, rGrid, delBtn, ScatterView, ScatterItem, vSwipeGrid, rSwipeGrid, vTranslationGrid, rTranslationGrid, vBoxesGrid, rBoxesGrid, hedatext));

            tab.Content = can;
            tabBar.Items.Insert(tabArray.Count - 1, tab);
            tabBar.SelectedIndex = tabArray.Count - 1;

            loadPage();

            return tab;
        }

        /* 
         * The following method was written when tabs weren't switching via touch
         * It may not end up being needed... We'll see.
         */
        //private void SearchTabItem_TouchDown(object sender, TouchEventArgs e)
        //{
        //    SearchTab tab = sender as SearchTab;
        //    TabControl control = tab.Parent as TabControl;
        //    control.SelectedItem = tab;
        //    e.Handled = true;
        //}

        private void wheelIt(object sender, MouseWheelEventArgs e)
        {
            loadPage();
            int d = e.Delta;
            ScatterViewItem item = (ScatterViewItem)sender;
            double width = item.Width + 2 * d;
            double height = item.Height + d * 1.4;
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

            tabArray[tabNumber]._delButton.Visibility = System.Windows.Visibility.Collapsed;

            for (int i = 0; i < tabArray.Count; i++)
            {
                if (tabArray[i]._tab.Equals(tab))
                    tabNumber = i;
            }
            tabArray[tabNumber]._delButton.Visibility = System.Windows.Visibility.Visible;
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
                {
                    tabBar.SelectedItem = tabArray[tabNumber + 1]._tab;
                }
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
            SliderDisplay.Opacity = 1;

            SliderImage1.Margin = new Thickness(0, 0, 0, 0);
            SliderImage2.Opacity = 100;

            currentTab()._worker.updateSlideImage(onVal);
        }

        /*
         * Called when the slider is released. Goes to the page and hides the slider display
         */
        private void slider_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            SliderDisplay.Opacity = 0;
            SurfaceSlider slider = (SurfaceSlider)sender;
            goToPage((int)Math.Round(2 * slider.Value));
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

        /*
         * Prevents the page images from showing the background of their bounding boxes.
         */
        private void limitScatter(ScatterViewItem i)
        {
            /*int rCount, vCount;
            rCount = currentTab()._textBlocksR.Count;
            vCount = currentTab()._textBlocksV.Count;

            double ratio = i.Height / minPageHeight;

            for (int j = 0; j < vCount; j++)
            {
                TextBlock tb = currentTab()._textBlocksV[j];
                tb.FontSize = 10 * ratio;
                double lines = tb.ActualHeight / tb.LineHeight;
                double heightRestriction = currentTab()._translationBoxesV[j].height * i.Height / i.MaxHeight;
                double diff = heightRestriction - tb.ActualHeight;
                tb.LineHeight += diff / lines;
            }
            for(int j = 0; j < rCount; j++)
            {
                TextBlock tb = currentTab()._textBlocksR[j];
                tb.FontSize = 10 * ratio;
                double lines = tb.ActualHeight / tb.LineHeight;
                double heightRestriction = currentTab()._translationBoxesR[j].height * i.Height / i.MaxHeight;
                double diff = heightRestriction - tb.ActualHeight;
                tb.LineHeight += diff / lines;
            }*/

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
                resizePageToRect(s, getRectFromPoint(true, p));
                dtOut = true;
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
            return new Rect(200, 200, 1600, 2000);
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

            s.Center = endPoint;
            s.Width = endWidth;
            s.Height = endHeight;
            stb.Begin(this);
        }

        private void resizePageToRect(ScatterViewItem s, Rect r)
        {
            bool sizeToHeight = r.Height > r.Width * 1.4;
            double h = r.Height;
            double w = r.Width;
            double endWidth, endHeight;
            double multiplier = 1;
            if (sizeToHeight)
            {
                if (h > minPageHeight)
                    multiplier = minPageHeight / h;
            }
            else
            {
                if (w > minPageWidth)
                    multiplier = minPageWidth / w;
            }

            endWidth = s.MaxWidth * multiplier;
            endHeight = s.MaxHeight * multiplier;
            double x, y, xcenter, ycenter;
            x = (r.Left + r.Width / 2) / maxPageWidth;
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
            Storyboard.SetTargetProperty(moveWidth,  new PropertyPath(ScatterViewItem.WidthProperty));
            Storyboard.SetTargetProperty(moveHeight, new PropertyPath(ScatterViewItem.HeightProperty));

            s.Center = endPoint;
            s.Width = endWidth;
            s.Height = endHeight;
            stb.Begin(this);
        }

        private bool IsDoubleTap(TouchEventArgs e)
        {
            Point currentTapLocation = e.GetTouchPoint(this).Position;
            bool tapsAreCloseInDistance = Math.Abs(currentTapLocation.X - lastTapLocation.X) < 30 && Math.Abs(currentTapLocation.Y - lastTapLocation.Y) < 30;
            lastTapLocation = currentTapLocation;

            TimeSpan elapsed = doubleTapSW.Elapsed;
            doubleTapSW.Restart();
            bool tapsAreCloseInTime = (elapsed != TimeSpan.Zero && elapsed < TimeSpan.FromSeconds(0.7));

            return tapsAreCloseInDistance && tapsAreCloseInTime;
        }

        private void OnPreviewTouchDown(object sender, TouchEventArgs e)
        {
            if (((ScatterViewItem)sender).TouchesOver.Count() > 1)
                lastTapLocation = new Point(-1000, -1000);
            ScatterViewItem item = (ScatterViewItem)sender;
            if (IsDoubleTap(e))
                OnDoubleTouchDown((ScatterViewItem)sender, e.TouchDevice.GetTouchPoint(item).Position);
        }

        private void OnPreviewTouchUp(object sender, TouchEventArgs e)
        {
        }

        private void languageChanged(object sender, SelectionChangedEventArgs e)
        {
            prevlanguageButton.IsEnabled = true;
            previousLanguage = currentLanguage;
            ListBox box = (ListBox)sender;
            if (box.SelectedIndex == 0)
            {
                currentLanguage = language.None;
                languageButton.Content = "None";
            }
            if (box.SelectedIndex == 1)
            {
                currentLanguage = language.OldFrench;
                languageButton.Content = "Old French";
            }
            if (box.SelectedIndex == 2)
            {
                currentLanguage = language.French;
                languageButton.Content = "French";
            }
            if (box.SelectedIndex == 3)
            {
                currentLanguage = language.English;
                languageButton.Content = "English";
            }
            currentTab()._worker.setTranslateText(currentLanguage);

            languageBox.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void rightSwipeDetectionStart(object sender, TouchEventArgs e)
        {
            ScatterViewItem item = (ScatterViewItem)sender;
            if (item.Width == minPageWidth && item.TouchesOver.Count<TouchDevice>() == 1 && item.TouchesOver.ElementAt<TouchDevice>(0).GetPosition(item).X > swipeLength)
            {
                rightSwipe = true;
                rightSwipeStart = item.TouchesOver.ElementAt<TouchDevice>(0).GetPosition(item);
                rightSwipeWatch = new Stopwatch();
                rightSwipeWatch.Start();
            }
            else
            {
                rightSwipe = false;
                currentTab()._rSwipeGrid.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        public void rightSwipeDetectionMove(object sender, TouchEventArgs e)
        {
            ScatterViewItem item = (ScatterViewItem)sender;
            if (rightSwipe)
            {
                double x = rightSwipeStart.X - swipeLength;
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
                tab._rSwipeGrid.Visibility = System.Windows.Visibility.Visible;
                Grid holder = (Grid)tab._rSwipeGrid.Parent;
                holder.ColumnDefinitions[0].Width = new GridLength(x, GridUnitType.Star);
                holder.ColumnDefinitions[1].Width = new GridLength(minPageWidth - x, GridUnitType.Star);
                holder.RowDefinitions[0].Height = new GridLength(y, GridUnitType.Star);
                holder.RowDefinitions[1].Height = new GridLength(minPageHeight - y, GridUnitType.Star);
                double dist = rightSwipeStart.X - item.TouchesOver.ElementAt<TouchDevice>(0).GetPosition(item).X;
                Canvas fill = (Canvas)tab._rSwipeGrid.Children[0];
                fill.Background = Brushes.Orange;
                fill.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
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
            ScatterViewItem item = (ScatterViewItem)sender;
            Point second = item.TouchesOver.ElementAt<TouchDevice>(0).GetPosition(item);
            if (rightSwipe)
            {
                if (((Canvas)currentTab()._rSwipeGrid.Children[0]).Background == Brushes.Green /*(second.X < rightSwipeStart.X - swipeLength && rightSwipeWatch.ElapsedMilliseconds < 1000)*/ || (Math.Abs(second.X - rightSwipeStart.X) < 10 && Math.Abs(second.Y - rightSwipeStart.Y) < 10 && rightSwipeWatch.ElapsedMilliseconds < 400 && rightSwipeStart.X > minPageWidth - 100))
                    next_Click(null, null);
            }
            rightSwipe = false;
            currentTab()._rSwipeGrid.Visibility = System.Windows.Visibility.Hidden;
        }
        public void leftSwipeDetectionStart(object sender, TouchEventArgs e)
        {
            ScatterViewItem item = (ScatterViewItem)sender;
            if (item.Width == minPageWidth && item.TouchesOver.Count<TouchDevice>() == 1 && item.TouchesOver.ElementAt<TouchDevice>(0).GetPosition(item).X < minPageWidth - swipeLength)
            {
                leftSwipe = true;
                leftSwipeStart = item.TouchesOver.ElementAt<TouchDevice>(0).GetPosition(item);
                leftSwipeWatch = new Stopwatch();
                leftSwipeWatch.Start();
            }
            else
            {
                leftSwipe = false;
                currentTab()._vSwipeGrid.Visibility = System.Windows.Visibility.Hidden;
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
                tab._vSwipeGrid.Visibility = System.Windows.Visibility.Visible;
                Grid holder = (Grid)tab._vSwipeGrid.Parent;
                holder.ColumnDefinitions[0].Width = new GridLength(x, GridUnitType.Star);
                holder.ColumnDefinitions[1].Width = new GridLength(minPageWidth - x, GridUnitType.Star);
                holder.RowDefinitions[0].Height = new GridLength(y, GridUnitType.Star);
                holder.RowDefinitions[1].Height = new GridLength(minPageHeight - y, GridUnitType.Star);
                double dist = item.TouchesOver.ElementAt<TouchDevice>(0).GetPosition(item).X - leftSwipeStart.X;
                Canvas fill = (Canvas)tab._vSwipeGrid.Children[0];
                fill.Background = Brushes.Orange;
                fill.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
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
            ScatterViewItem item = (ScatterViewItem)sender;
            Point second = item.TouchesOver.ElementAt<TouchDevice>(0).GetPosition(item);
            if (leftSwipe)
            {
                if (((Canvas)currentTab()._vSwipeGrid.Children[0]).Background == Brushes.Green /*(second.X < rightSwipeStart.X - 100 && rightSwipeWatch.ElapsedMilliseconds < 1000)*/ || (Math.Abs(second.X - leftSwipeStart.X) < 10 && Math.Abs(second.Y - leftSwipeStart.Y) < 10 && leftSwipeWatch.ElapsedMilliseconds < 400 && leftSwipeStart.X > 100))
                    prev_Click(null, null);
            }
            leftSwipe = false;
            currentTab()._vSwipeGrid.Visibility = System.Windows.Visibility.Hidden;
        }


        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
            tb.Foreground = Brushes.Black;
        }




        /// Here come all the searchsidebar functions!!

        private void SearchButton_Selected(object sender, RoutedEventArgs e)
        {
            tabDynamic.DataContext = null;
            TabItem newTab = this.AddSearchTabItem();
            tabDynamic.DataContext = SidebarTabItems;
            tabDynamic.SelectedItem = newTab;
            
        }

        private void AnnotateButton_Selected(object sender, RoutedEventArgs e)
        {
            tabDynamic.DataContext = null;
            TabItem newTab = this.AddAnnotateTabItem();
            tabDynamic.DataContext = SidebarTabItems;
            tabDynamic.SelectedItem = newTab;
        }

        private void StudyButton_Selected(object sender, RoutedEventArgs e)
        {
            tabDynamic.DataContext = null;
            TabItem newTab = this.AddStudyTabItem();
            tabDynamic.DataContext = SidebarTabItems;
            tabDynamic.SelectedItem = newTab;
        }


        private TabItem AddSearchTabItem()
        {
            int count = SidebarTabItems.Count;


            SearchTab tab = new SearchTab();
            tab.Name = string.Format("tab{0}", count);

            tab.moreOptions.Click += new RoutedEventHandler(Show_Options);
            tab.moreOptions.TouchDown += new EventHandler<TouchEventArgs>(Show_Options);
            tab.deleteTabButton.Click += new RoutedEventHandler(SearchbtnDelete_Click);
            tab.deleteTabButton.TouchDown += new EventHandler<TouchEventArgs>(SearchbtnDelete_Click);
            tab.fewerOptions.Click += new RoutedEventHandler(Hide_Options);
            tab.fewerOptions.TouchDown += new EventHandler<TouchEventArgs>(Hide_Options);
            tab.searchQueryBox.GotFocus += new RoutedEventHandler(Clear_SearchBox);
            tab.searchQueryBox.TouchDown += new EventHandler<TouchEventArgs>(Clear_SearchBox);
            tab.goSearch.Click += new RoutedEventHandler(newSearch);
            tab.goSearch.TouchDown += new EventHandler<TouchEventArgs>(newSearch);
            tab.searchQueryBox.PreviewKeyDown += new KeyEventHandler(Enter_Clicked);
            tab.caseSensitive.TouchDown += new EventHandler<TouchEventArgs>(changeCheck);
            tab.wholePhraseOnly.TouchDown += new EventHandler<TouchEventArgs>(changeCheck);
            tab.wholeWordOnly.TouchDown += new EventHandler<TouchEventArgs>(changeCheck);

            //tab.selectLanguage.TouchDown += new EventHandler<TouchEventArgs>(displaySearchLanguages);
            //tab.selectLanguage.SelectionChanged += new SelectionChangedEventHandler(searchLanguageChanged);
            tab.selectLanguage.Visibility = Visibility.Collapsed;
            tab.selectLanguageButton.TouchDown += new EventHandler<TouchEventArgs>(displaySearchLanguages);
            tab.selectLanguageButton.Click += new RoutedEventHandler(displaySearchLanguages);
            tab.oldFrench.Selected += new RoutedEventHandler(searchLanguageChanged);
            tab.modernFrench.Selected += new RoutedEventHandler(searchLanguageChanged);
            tab.English.Selected += new RoutedEventHandler(searchLanguageChanged);


            //tab.selectLanguage.Style = tabDynamic.FindResource("RoundSurfaceListBoxTemplate") as Style;

            // insert tab item right before the last (+) tab item
            SidebarTabItems.Insert(count - 1, tab);
            return tab;
        }

        private void displaySearchLanguages(object sender, RoutedEventArgs e)
        {
            SearchTab selectedTab = tabDynamic.SelectedItem as SearchTab;
            if (selectedTab.selectLanguage.Visibility == Visibility.Collapsed | selectedTab.selectLanguage.Visibility == Visibility.Hidden)
            {
                selectedTab.selectLanguage.Visibility = Visibility.Visible;
                selectedTab.selectLanguageButton.Visibility = Visibility.Collapsed;
            }
            else
                selectedTab.selectLanguage.Visibility = Visibility.Collapsed;

        }

        private void searchLanguageChanged(object sender, RoutedEventArgs e)
        {
            SearchTab selectedTab = tabDynamic.SelectedItem as SearchTab;
            SurfaceListBoxItem box = (SurfaceListBoxItem)sender;
            selectedTab.selectLanguageButton.Content = box.Content;
            selectedTab.selectLanguage.Visibility = Visibility.Hidden;
            selectedTab.selectLanguageButton.Visibility = Visibility.Visible;
            if (box == selectedTab.oldFrench)
                currentSearchLanguage = searchLanguage.oldFrench;
            else if (box == selectedTab.modernFrench)
                currentSearchLanguage = searchLanguage.modernFrench;
            else if (box == selectedTab.English)
                currentSearchLanguage = searchLanguage.English;
        }

        private void Show_Options(object sender, RoutedEventArgs e)
        {
            optionsShown = true;
            SearchTab selectedTab = tabDynamic.SelectedItem as SearchTab;
            selectedTab.topLine.Visibility = Visibility.Hidden;
            selectedTab.caseSensitive.Visibility = Visibility.Visible;
            selectedTab.bottomLine.Visibility = Visibility.Visible;
            selectedTab.fewerOptions.Visibility = Visibility.Visible;
            selectedTab.wholeWordOnly.Visibility = Visibility.Visible;
            selectedTab.wholePhraseOnly.Visibility = Visibility.Visible;
            selectedTab.moreOptions.Visibility = Visibility.Hidden;
            selectedTab.selectLanguageButton.Visibility = Visibility.Visible;
            if (selectedTab.searchResults.IsVisible)
                compressResults();

        }

        private void Hide_Options(object sender, RoutedEventArgs e)
        {
            SearchTab selectedTab = tabDynamic.SelectedItem as SearchTab;
            selectedTab.topLine.Visibility = Visibility.Visible;
            selectedTab.moreOptions.Visibility = Visibility.Visible;
            selectedTab.caseSensitive.Visibility = Visibility.Hidden;
            selectedTab.selectLanguage.Visibility = Visibility.Hidden;
            selectedTab.bottomLine.Visibility = Visibility.Hidden;
            selectedTab.fewerOptions.Visibility = Visibility.Hidden;
            selectedTab.wholeWordOnly.Visibility = Visibility.Hidden;
            selectedTab.wholePhraseOnly.Visibility = Visibility.Hidden;
            selectedTab.selectLanguageButton.Visibility = Visibility.Hidden;

            checkForChanges();

            if (defaultOptionsChanged == true)
                selectedTab.moreOptions.Background = Brushes.MediumTurquoise;

            else
                selectedTab.moreOptions.ClearValue(Control.BackgroundProperty);

            if (selectedTab.searchResults.IsVisible)
                expandResults();

            optionsShown = false;
        }

        private void changeCheck(object sender, TouchEventArgs e)
        {
            SearchTab selectedTab = tabDynamic.SelectedItem as SearchTab;
            CheckBox thisbox = sender as CheckBox;

            // The following if loop is accounting for the fact that all search functions are currently set to whole phrase only
            // There is not yet a way to search for several words appearing near each other
            // Once that function is implemented, then the "Match whole phrase only" CheckBox will function like the other CheckBoxes
            if (thisbox == selectedTab.wholePhraseOnly)
            {
                MessageBox.Show(string.Format("Oops! This function has not been added yet."),
                    "Unmark \"Match whole phrase only\"", MessageBoxButton.OK);
            }
            else
            {
                if (thisbox.IsChecked == true)
                    thisbox.IsChecked = false;
                else if (thisbox.IsChecked == false)
                    thisbox.IsChecked = true;
            }
        }

        private void newSearch(object sender, RoutedEventArgs e)
        {
            SearchTab selectedTab = tabDynamic.SelectedItem as SearchTab;
            String searchQuery = selectedTab.searchQueryBox.Text;

            if (searchQuery == "Enter text" || searchQuery == "")
                MessageBox.Show(string.Format("Please enter words to search for!"),
                    "Search", MessageBoxButton.OK);

            else
            {

                selectedTab.searchTabHeader.Text = selectedTab.searchQueryBox.Text;
                selectedTab.searchResults.Visibility = Visibility.Visible;
                selectedTab.poetryTab.Content = selectedTab.poetryCanvas;

                int caseType = 0;
                int wordType = 0;
                if (selectedTab.caseSensitive.IsChecked == true)
                    caseType = 1;
                if (selectedTab.wholeWordOnly.IsChecked == true)
                    wordType = 1;

                // Poetry results

                List<SearchResult> poetryResults = new List<SearchResult>();
                if (currentSearchLanguage == searchLanguage.oldFrench)
                    poetryResults = Translate.searchOldFrPoetry(searchQuery, caseType, wordType, xml, engXml, layoutXml);
                else if(currentSearchLanguage == searchLanguage.modernFrench)
                    poetryResults = Translate.searchModFrPoetry(searchQuery, caseType, wordType, modFrXml, engXml, layoutXml);
                else if (currentSearchLanguage == searchLanguage.English)
                    poetryResults = Translate.searchEngPoetry(searchQuery, caseType, wordType, xml, engXml, layoutXml);

                SurfaceListBox poetryLB = new SurfaceListBox();
                poetryLB.Style = tabDynamic.FindResource("SearchResultSurfaceListBox") as Style;
                poetryLB.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
                poetryLB.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Visible);
                selectedTab.poetryScroll.Content = poetryLB;

                foreach (SearchResult result in poetryResults)
                {
                    ResultBoxItem resultRBI = new ResultBoxItem();
                    convertSearchResultToResultBoxItem(result, resultRBI);
                    poetryLB.Items.Add(resultRBI);
                }

                selectedTab.poetryTab.Header = "Poetry (" + poetryResults.Count + ")";

                if (poetryResults.Count == 0)
                {
                    TextBlock noResults = new TextBlock();
                    noResults.Text = "Sorry, your search returned no poetry results.";
                    selectedTab.poetryTab.Content = noResults;
                }
                else
                    selectedTab.poetryTab.Content = selectedTab.poetryCanvas;



                // Lyric results
                List<SearchResult> lyricResults = new List<SearchResult>();
                if (currentSearchLanguage == searchLanguage.oldFrench)
                    lyricResults = Translate.searchLyrics(searchQuery, caseType, wordType, xml, layoutXml);
                else if (currentSearchLanguage == searchLanguage.modernFrench)
                    lyricResults = Translate.searchLyrics(searchQuery, caseType, wordType, modFrXml, layoutXml);

                ListBox lyricsLB = new ListBox();
                lyricsLB.Style = tabDynamic.FindResource("SearchResultSurfaceListBox") as Style;
                lyricsLB.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
                selectedTab.lyricsScroll.Content = lyricsLB;


                foreach (SearchResult result in lyricResults)
                {
                    ResultBoxItem resultRBI = new ResultBoxItem();
                    convertSearchResultToResultBoxItem(result, resultRBI);
                    lyricsLB.Items.Add(resultRBI);
                }

                selectedTab.lyricsTab.Header = "Lyrics (" + lyricResults.Count + ")";

                if (lyricResults.Count == 0)
                {
                    TextBlock noResults = new TextBlock();
                    noResults.Text = "Sorry, your search returned no music lyric results.";
                    selectedTab.lyricsTab.Content = noResults;
                }
                else
                    selectedTab.lyricsTab.Content = selectedTab.lyricsCanvas;



                // Image results
                List<SearchResult> imageResults = Translate.searchPicCaptions(searchQuery, caseType, wordType, xml, layoutXml);
                ListBox imagesLB = new ListBox();
                imagesLB.Style = tabDynamic.FindResource("SearchResultSurfaceListBox") as Style;
                imagesLB.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
                selectedTab.imagesScroll.Content = imagesLB;

                foreach (SearchResult result in imageResults)
                {
                    ResultBoxItem resultRBI = new ResultBoxItem();
                    convertSearchResultToResultBoxItem(result, resultRBI);
                    imagesLB.Items.Add(resultRBI);
                }

                selectedTab.imagesTab.Header = "Images (" + imageResults.Count + ")";

                if (imageResults.Count == 0)
                {
                    TextBlock noResults = new TextBlock();
                    noResults.Text = "Sorry, your search returned no image results.";
                    selectedTab.imagesTab.Content = noResults;
                }
                else
                    selectedTab.imagesTab.Content = selectedTab.imagesCanvas;

                if (optionsShown == true) 
                    compressResults();


                // Auto flip to a tab with results if the current one has none
                if (selectedTab.searchResults.SelectedItem == selectedTab.poetryTab && poetryResults.Count == 0)
                {
                    if (lyricResults.Count != 0)
                        selectedTab.searchResults.SelectedItem = selectedTab.lyricsTab;
                    else if (imageResults.Count != 0)
                        selectedTab.searchResults.SelectedItem = selectedTab.imagesTab;
                }
                else if (selectedTab.searchResults.SelectedItem == selectedTab.lyricsTab && lyricResults.Count == 0)
                {
                    if (poetryResults.Count != 0)
                        selectedTab.searchResults.SelectedItem = selectedTab.poetryTab;
                    else if (imageResults.Count != 0)
                        selectedTab.searchResults.SelectedItem = selectedTab.imagesTab;
                }
                else if (selectedTab.searchResults.SelectedItem == selectedTab.imagesTab && imageResults.Count == 0)
                {
                    if (poetryResults.Count != 0)
                        selectedTab.searchResults.SelectedItem = selectedTab.poetryTab;
                    else if (lyricResults.Count != 0)
                        selectedTab.searchResults.SelectedItem = selectedTab.lyricsTab;
                }
            }
        }

        private void convertSearchResultToResultBoxItem(SearchResult sr, ResultBoxItem rbi)
        {
            rbi.folioInfo.Text = sr.folio;
            rbi.resultType = sr.resultType;
            if(rbi.resultType == 1)
                rbi.lineInfo.Text = Convert.ToString(sr.lineNum);
            rbi.resultThumbnail = sr.thumbnail;
            rbi.excerpt1 = sr.excerpt1;
            rbi.excerpt2 = sr.excerpt2;
            rbi.excerpt3 = sr.excerpt3;
            rbi.Height = 80; // temp taller than desired, so scrollbar shows
            rbi.Style = tabDynamic.FindResource("SearchResultSurfaceListBoxItem") as Style; // Not sure if this works..
            rbi.resultText.Text = sr.text1 + "\r\n" + sr.text2;
            rbi.Selected += new RoutedEventHandler(Result_Closeup);
        }

        private void compressResults()
        {
            SearchTab selectedTab = tabDynamic.SelectedItem as SearchTab;
            selectedTab.searchResults.Height = 537;
            Canvas.SetTop(selectedTab.searchResults, 320);
            selectedTab.poetryPanel.Height = 242;
            Canvas.SetTop(selectedTab.poetryPanel, 220);
            selectedTab.poetryScroll.Height = 200;
            selectedTab.lyricsPanel.Height = 242;
            Canvas.SetTop(selectedTab.lyricsPanel, 220);
            selectedTab.lyricsScroll.Height = 200;
            selectedTab.imagesPanel.Height = 242;
            Canvas.SetTop(selectedTab.imagesPanel, 220);
            selectedTab.imagesScroll.Height = 200;

        }

        private void expandResults()
        {
            SearchTab selectedTab = tabDynamic.SelectedItem as SearchTab;
            selectedTab.searchResults.Height = 677;
            Canvas.SetTop(selectedTab.searchResults, 180);
            selectedTab.poetryPanel.Height = 300;
            Canvas.SetTop(selectedTab.poetryPanel, 331);
            selectedTab.poetryScroll.Height = 325;
            selectedTab.lyricsPanel.Height = 300;
            Canvas.SetTop(selectedTab.lyricsPanel, 331);
            selectedTab.lyricsScroll.Height = 325;
            selectedTab.imagesPanel.Height = 300;
            Canvas.SetTop(selectedTab.imagesPanel, 331);
            selectedTab.imagesScroll.Height = 325;
        }

        private void Enter_Clicked(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                newSearch(sender, e);
                e.Handled = true;
            }
        }

        private void Result_Closeup(object sender, RoutedEventArgs e)
        {
            SearchTab selectedTab = tabDynamic.SelectedItem as SearchTab;
            ResultBoxItem selectedResult = e.Source as ResultBoxItem;

            Image closeupImage = new Image();
            closeupImage.Source = selectedResult.resultThumbnail.Source;
            closeupImage.Width = 185;
            closeupImage.Height = 176;
            closeupImage.Margin = new Thickness(0, 0, 10, 0);


            TextBlock closeupText = new TextBlock();
            closeupText.TextWrapping = TextWrapping.Wrap;
            closeupText.FontSize = 15;
            closeupText.Width = 275;
            closeupText.VerticalAlignment = VerticalAlignment.Center;

            if (selectedResult.resultType == 1)
            {
                selectedTab.poetryPanel.Children.Clear();
                selectedTab.poetryPanel.Children.Add(closeupImage);
                selectedTab.poetryPanel.Children.Add(closeupText);
                selectedTab.poetryPanel.TouchDown += new EventHandler<TouchEventArgs>(goToFolio);
            }

            else if (selectedResult.resultType == 2)
            {
                selectedTab.lyricsPanel.Children.Clear();
                selectedTab.lyricsPanel.Children.Add(closeupImage);
                selectedTab.lyricsPanel.Children.Add(closeupText);
                selectedTab.lyricsPanel.TouchDown += new EventHandler<TouchEventArgs>(goToFolio);
            }

            else if (selectedResult.resultType == 3)
            {
                selectedTab.imagesPanel.Children.Clear();
                selectedTab.imagesPanel.Children.Add(closeupImage);
                selectedTab.imagesPanel.Children.Add(closeupText);
                selectedTab.imagesPanel.TouchDown += new EventHandler<TouchEventArgs>(goToFolio);
            }

            pageToFind = selectedResult.folioInfo.Text;

            closeupText.Inlines.Add(new Run { FontFamily = new FontFamily("Cambria"), Text = selectedResult.excerpt1, FontWeight = FontWeights.Normal });
            closeupText.Inlines.Add(new Run { FontFamily = new FontFamily("Cambria"), FontWeight = FontWeights.Bold, Text = selectedResult.excerpt2 });
            closeupText.Inlines.Add(new Run { FontFamily = new FontFamily("Cambria"), FontWeight = FontWeights.Normal, Text = selectedResult.excerpt3 });
        }


        private void goToFolio(object sender, TouchEventArgs e)
        {
            if (MessageBox.Show(string.Format("Open a new tab to this folio?"),
              "Go to Folio", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                createTab(1);
                Tab tab = currentTab();

                if (pageToFind.StartsWith("Fo"))
                    pageToFind = pageToFind.Substring(2);
                String imageName = Thumbnailer.getImageName(pageToFind, layoutXml);
                int pageNum = Convert.ToInt32(imageName.Substring(0, imageName.IndexOf(".jpg")));
                if (pageNum % 2 == 1) // If odd, meaning it's a Fo_r, we want to aim for the previous page.
                    pageNum--;
                goToPage(pageNum - 10);
                loadPage();
            }
        }


        private Boolean checkForChanges()
        {
            SearchTab selectedTab = tabDynamic.SelectedItem as SearchTab;
            if (selectedTab.caseSensitive.IsChecked == true | selectedTab.wholeWordOnly.IsChecked == true |
                selectedTab.wholePhraseOnly.IsChecked == false | (selectedTab.selectLanguage.SelectedIndex != 0 && selectedTab.selectLanguage.SelectedIndex != 1))
                defaultOptionsChanged = true;

            else
                defaultOptionsChanged = false;


            return defaultOptionsChanged;
        }

        private void Clear_SearchBox(object sender, RoutedEventArgs e)
        {
            SearchTab selectedTab = tabDynamic.SelectedItem as SearchTab;
            if (selectedTab.searchQueryBox.Text == "Enter text")
            {
                selectedTab.searchQueryBox.Foreground = Brushes.Black;
                selectedTab.searchQueryBox.Text = "";
            }
            else
                selectedTab.searchQueryBox.SelectAll();

            selectedTab.searchQueryBox.Focus();
        }



        private TabItem AddAnnotateTabItem()
        {
            int count = SidebarTabItems.Count;

            // create new tab item - eventually replace with AnnotateTab tab = new AnnotateTab();
            // Then, add all listeners here
            TabItem tab = new TabItem(); 
            tab.Name = string.Format("tab{0}", count);
            tab.HeaderTemplate = tabDynamic.FindResource("NewAnnotateTab") as DataTemplate; // can be replaced if AnnotateTab object exists



            // insert tab item right before the last (+) tab item
            SidebarTabItems.Insert(count - 1, tab);
            return tab;
        }

        private TabItem AddLearnerTab()
        {
            int count = SidebarTabItems.Count;


            TabItem tab = new TabItem();
            tab.Name = string.Format("tab{0}", count);
            tab.Header = "HI";
            tab.Width = 100;

            SurfaceScrollViewer ssv = new SurfaceScrollViewer();
            ssv.Height = 500;
            ssv.Width = 300;
            ssv.Background = Brushes.Aquamarine;
            Canvas.SetTop(ssv, 50);
            Canvas.SetLeft(ssv, 100);

            Canvas learnerCanvas = new Canvas();
            tab.Content = learnerCanvas;
            learnerCanvas.Children.Add(ssv);


            // insert tab item right before the last (+) tab item
            SidebarTabItems.Insert(count - 1, tab);
            return tab;
        }

        private TabItem AddStudyTabItem()
        {
            int count = SidebarTabItems.Count;

            // create new tab item - eventually replace with MusicTab tab = new MusicTab();
            // Then, add all listeners here
            TabItem tab = new TabItem();
            tab.Name = string.Format("tab{0}", count);
            tab.HeaderTemplate = tabDynamic.FindResource("NewStudyTab") as DataTemplate; // can be replaced if StudyTab object exists



            // insert tab item right before the last (+) tab item
            SidebarTabItems.Insert(count - 1, tab);
            return tab;
        }

        private void searchTabDynamic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tab = tabDynamic.SelectedItem as TabItem;

            if (tab != null && tab.Header != null)
            {
                if (tab.Header.Equals("+") && SidebarTabItems.Count > 2)
                {

                }
                else
                {

                }
            }
        }


        private void SearchbtnDelete_Click(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show(string.Format("Are you sure you want to remove this tab?"),
                 "Remove Tab", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                TabItem selectedTab = tabDynamic.SelectedItem as TabItem;
                tabDynamic.DataContext = null;

                SidebarTabItems.Remove(selectedTab);
                tabDynamic.DataContext = SidebarTabItems;
                if (selectedTab == null || selectedTab.Equals(selectedTab))
                {
                    selectedTab = SidebarTabItems[0];
                }
                tabDynamic.SelectedItem = selectedTab;
            }
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
            if (languageBox.Visibility == System.Windows.Visibility.Collapsed)
                languageBox.Visibility = System.Windows.Visibility.Visible;
            else
                languageBox.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void languagePrevious(object sender, TouchEventArgs e)
        {
            languageBox.SelectedIndex = (int)previousLanguage;
        }

    }
}