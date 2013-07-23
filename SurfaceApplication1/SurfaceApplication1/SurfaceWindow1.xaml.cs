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

namespace SurfaceApplication1
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        private bool rightSwipe = false;
        private bool leftSwipe = false;
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
        public enum language { None, English, OldFrench, French };
        public static language currentLanguage = language.None;
        bool dtOut = false; // double tap to zoom out

        private List<TabItem> SidebarTabItems;
        private TabItem tabAdd;
        private Boolean defaultOptionsChanged;
        public static XmlDocument xml, engXml, layoutXml;
        public int veryFirstLine, veryLastLine;


        public SurfaceWindow1()
        {
            InitializeComponent();

            //Search bar initialization
            veryFirstLine = 1;
            veryLastLine = 5986;
            SidebarTabItems = new List<TabItem>();

            // Lots of search sidebar things
            tabAdd = new TabItem();
            tabAdd.Header = "+";
            tabAdd.Height = 40;
            tabAdd.Width = 50;

            Canvas newTabCanvas = new Canvas();
            newTabCanvas.Height = 899;
            newTabCanvas.Width = 550;
            tabAdd.Content = newTabCanvas;

            Button searchButton = new Button();
            searchButton.Style = tabDynamic.FindResource("RoundButtonTemplate") as Style;
            searchButton.Click += new RoutedEventHandler(SearchButton_Clicked);

            Grid searchGrid = new Grid();

            Image searchIm = new Image();
            searchIm.Source = new BitmapImage(new Uri(@"/magnifyingglass.png", UriKind.Relative));
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
            annotateButton.Click += new RoutedEventHandler(AnnotateButton_Clicked);


            Grid annotateGrid = new Grid();

            Image annotateIm = new Image();
            annotateIm.Source = new BitmapImage(new Uri(@"/pencil.jpg", UriKind.Relative));
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
            studyButton.Click += new RoutedEventHandler(StudyButton_Clicked);


            Grid studyGrid = new Grid();

            Image studyIm = new Image();
            studyIm.Source = new BitmapImage(new Uri(@"/musicnote.png", UriKind.Relative));
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
                xml.Load("XMLFinalContentFile.xml");
                engXml = new XmlDocument();
                engXml.Load("EnglishXML.xml");
                layoutXml = new XmlDocument();
                layoutXml.Load("layout_full.xml");
            }
            catch (Exception e)
            {
                String temp = e.StackTrace;
            }

            Translate.getTranslationOverlay("34v", xml, engXml, layoutXml);
            Console.Read();

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

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            Tab tab = currentTab();
            if (tab._twoPage)
            {
                int newPage = tab._page - 2;
                newPage -= newPage % 2;
                goToPage(newPage);
            }
            else
            {
                goToPage(tab._page - 1);
            }
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            Tab tab = currentTab();
            if (tab._twoPage)
            {
                int newPage = tab._page + 2;
                newPage -= newPage % 2;
                goToPage(newPage);
            }
            else
            {
                goToPage(tab._page + 1);
            }
        }

        private void goToPage(int page)
        {
            Tab tab = currentTab();
            tab._page = page;
            if (page > maxPage)
            {
                if (currentTab()._twoPage)
                    tab._page = maxPage - 1;
                else
                    tab._page = maxPage;
            }
            if (tab._page < minPage)
                tab._page = minPage;
            loadPage();
        }

        private void loadPage()
        {
            Tab currentTab = this.currentTab();

            currentTab._worker.updateTranslations();

            currentTab._worker.updateGhostBoxes();

            currentTab._vSVI.Width = currentTab._vSVI.MinWidth;
            currentTab._vSVI.Height = currentTab._vSVI.MinHeight;
            currentTab._rSVI.Width = currentTab._rSVI.MinWidth;
            currentTab._rSVI.Height = currentTab._rSVI.MinHeight;
            currentTab._rSVI.Center = new Point(currentTab._rSV.Width / 2, currentTab._rSV.Height / 2);
            currentTab._vSVI.Center = new Point(currentTab._vSV.Width / 2, currentTab._vSV.Height / 2);

            int pageNumber = currentTab._page;

            Image verso = currentTab._verso;
            Image recto = currentTab._recto;

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("smallpages/" + (currentTab._page + 10).ToString() + ".jpg", UriKind.Relative);
            image.EndInit();
            image.Freeze();
            verso.Source = image;

            if (currentTab._twoPage)
            {
                BitmapImage image2 = new BitmapImage();
                image2.BeginInit();
                image2.UriSource = new Uri("smallpages/" + (currentTab._page + 11).ToString() + ".jpg", UriKind.Relative);
                image2.EndInit();
                image2.Freeze();
                recto.Source = image2;
            }

            currentTab._worker.updateVersoImage(false);
            currentTab._worker.updateRectoImage(false);

            String pageText = PageNamer.getPageText(currentTab._page, currentTab._twoPage);
            pageNumberText.Text = pageText;
            currentTab._headerTB.Text = pageText;

            if (currentTab._twoPage)
                pageSlider.Value = currentTab._page / 2;
            else
                pageSlider.Value = currentTab._page;

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
            ex.FontSize = 20;
            delBtn.Content = ex;

            heda.Children.Add(hedatext);
            heda.Children.Add(delBtn);

            Grid vSwipeGrid = new Grid();
            Grid rSwipeGrid = new Grid();
            Grid vSwipeHolderGrid = new Grid();
            Grid rSwipeHolderGrid = new Grid();
            Canvas can = new Canvas();
            Canvas c_v = new Canvas();
            Canvas c_r = new Canvas();
            Image verso = new Image();
            Image recto = new Image();
            verso.Stretch = Stretch.UniformToFill;
            recto.Stretch = Stretch.UniformToFill;
            SSC.ScatterView vScatterView = new SSC.ScatterView();
            SSC.ScatterView rScatterView = new SSC.ScatterView();
            SSC.ScatterViewItem vScatterItem = new SSC.ScatterViewItem();
            SSC.ScatterViewItem rScatterItem = new SSC.ScatterViewItem();

            vScatterItem.PreviewTouchDown += new EventHandler<TouchEventArgs>(OnPreviewTouchDown);
            vScatterItem.PreviewTouchDown += new EventHandler<TouchEventArgs>(leftSwipeDetectionStart);
            vScatterItem.PreviewTouchMove += new EventHandler<TouchEventArgs>(leftSwipeDetectionMove);
            vScatterItem.PreviewTouchUp += new EventHandler<TouchEventArgs>(leftSwipeDetectionStop);
            rScatterItem.PreviewTouchDown += new EventHandler<TouchEventArgs>(OnPreviewTouchDown);
            rScatterItem.PreviewTouchDown += new EventHandler<TouchEventArgs>(rightSwipeDetectionStart);
            rScatterItem.PreviewTouchMove += new EventHandler<TouchEventArgs>(rightSwipeDetectionMove);
            rScatterItem.PreviewTouchUp += new EventHandler<TouchEventArgs>(rightSwipeDetectionStop);
            vScatterItem.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(wheelIt);
            rScatterItem.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(wheelIt);
            vScatterView.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            rScatterView.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            rScatterView.Margin = new System.Windows.Thickness(minPageWidth, 0, 0, 0);
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
            vScatterItem.Center = new Point(buffer + minPageWidth / 2, buffer + minPageHeight / 2);
            rScatterItem.Center = new Point(buffer + minPageWidth / 2, buffer + minPageHeight / 2);

            vScatterItem.SizeChanged += new SizeChangedEventHandler(makeVersoImageLarge);
            rScatterItem.SizeChanged += new SizeChangedEventHandler(makeRectoImageLarge);

            Grid vGrid = new Grid();
            Grid rGrid = new Grid();
            Grid vTranslationGrid = new Grid();
            Grid rTranslationGrid = new Grid();
            Grid vBoxesGrid = new Grid();
            Grid rBoxesGrid = new Grid();

            vScatterItem.Content = vGrid;
            rScatterItem.Content = rGrid;
            vGrid.Children.Add(verso);
            rGrid.Children.Add(recto);
            vGrid.Children.Add(vTranslationGrid);
            rGrid.Children.Add(rTranslationGrid);
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

            c_v.Children.Add(vScatterView);
            c_r.Children.Add(rScatterView);
            vScatterView.Items.Add(vScatterItem);
            rScatterView.Items.Add(rScatterItem);

            tabArray.Insert(count, new Tab(2, tab, verso, recto, can, c_v, c_r, vGrid, rGrid, delBtn, vScatterView, rScatterView, vScatterItem, rScatterItem, vSwipeGrid, rSwipeGrid, vTranslationGrid, rTranslationGrid, vBoxesGrid, rBoxesGrid, hedatext));

            can.Children.Add(c_v);
            can.Children.Add(c_r);
            tab.Content = can;
            tabBar.Items.Insert(tabArray.Count - 1, tab);
            tabBar.SelectedIndex = tabArray.Count - 1;

            return tab;
        }

        private void wheelIt(object sender, MouseWheelEventArgs e)
        {
            int d = e.Delta;
            ScatterViewItem item = (ScatterViewItem)sender;
            double width = item.Width + d;
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

        private void swapOrientation(object sender, RoutedEventArgs e)
        {
            if (currentTab()._twoPage)
            {
                setToOnePage(true);
            }
            else
            {
                setToTwoPages();
            }

        }

        private void setToTwoPages()
        {
            Tab tab = currentTab();
            Canvas c_v = tab._c_v;
            Canvas c_r = tab._c_r;
            ScatterView vScatterView = tab._vSV;
            ScatterView rScatterView = tab._rSV;
            ScatterViewItem vScatterItem = (ScatterViewItem)vScatterView.Items[0];
            ScatterViewItem rScatterItem = (ScatterViewItem)rScatterView.Items[0];

            tab._page -= tab._page % 2;
            pageSlider.Maximum = 47;
            pageSlider.Value = tab._page / 2;
            pageNumberText.Text = PageNamer.getPageText(tab._page, tab._twoPage);

            c_v.Width = minPageWidth;
            c_r.Width = minPageWidth;
            c_v.Visibility = System.Windows.Visibility.Visible;
            c_r.Visibility = System.Windows.Visibility.Visible;
            c_r.Margin = new Thickness(minPageWidth, 0, 0, 0);

            vScatterView.Width = minPageWidth + 2 * scatterBuffer;
            rScatterView.Width = minPageWidth + 2 * scatterBuffer;

            vScatterItem.MinWidth = minPageWidth;
            vScatterItem.MinHeight = minPageHeight;
            rScatterItem.MinWidth = minPageWidth;
            rScatterItem.MinHeight = minPageHeight;
            vScatterItem.Orientation = 0;
            rScatterItem.Orientation = 0;
            vScatterItem.Width = vScatterItem.MinWidth;
            vScatterItem.Height = vScatterItem.MinHeight;
            rScatterItem.Width = rScatterItem.MinWidth;
            rScatterItem.Height = rScatterItem.MinHeight;

            vScatterItem.Center = new Point(vScatterView.Width / 2, vScatterView.Height / 2);
            rScatterItem.Center = new Point(rScatterView.Width / 2, rScatterView.Height / 2);

            currentTab()._twoPage = true;

            int pageNumber = currentTab()._page;
            String pageText = PageNamer.getPageText(pageNumber, currentTab()._twoPage);
            pageNumberText.Text = pageText;
            currentTab()._headerTB.Text = pageText;

            loadPage();
        }

        private string getPageFromNumber(int number)
        {
            /*  br
                bv
                tr
                tv*/
            return null;
        }

        private void setToOnePage(bool left)
        {
            Tab tab = currentTab();
            Canvas c_v = tab._c_v;
            Canvas c_r = tab._c_r;
            if (left)
            {
                c_v.Visibility = System.Windows.Visibility.Visible;
                c_r.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                c_v.Visibility = System.Windows.Visibility.Collapsed;
                c_r.Visibility = System.Windows.Visibility.Visible;
            }
            ScatterView vScatterView = tab._vSV;
            ScatterView rScatterView = tab._rSV;
            ScatterViewItem vScatterItem = (ScatterViewItem)vScatterView.Items[0];
            ScatterViewItem rScatterItem = (ScatterViewItem)rScatterView.Items[0];

            pageSlider.Maximum = 95;
            pageSlider.Value = tab._page;

            if (currentTab()._twoPage)
            {
                c_v.Width = minPageLong;
                c_r.Width = minPageLong;
                c_r.Margin = new Thickness(0);
                vScatterView.Width = minPageLong + 2 * scatterBuffer;
                rScatterView.Width = minPageLong + 2 * scatterBuffer;
                vScatterItem.MinWidth = minPageHeight;
                vScatterItem.MinHeight = minPageLong;
                rScatterItem.MinWidth = minPageHeight;
                rScatterItem.MinHeight = minPageLong;
                vScatterItem.Orientation = 270;
                rScatterItem.Orientation = 270;
            }

            vScatterItem.Width = vScatterItem.MinWidth;
            vScatterItem.Height = vScatterItem.MinHeight;
            rScatterItem.Width = rScatterItem.MinWidth;
            rScatterItem.Height = rScatterItem.MinHeight;
            vScatterItem.Center = new Point(scatterBuffer + minPageLong / 2, scatterBuffer + minPageHeight / 2);
            rScatterItem.Center = new Point(scatterBuffer + minPageLong / 2, scatterBuffer + minPageHeight / 2);

            currentTab()._twoPage = false;

            int pageNumber = currentTab()._page;
            String pageText = PageNamer.getPageText(pageNumber, currentTab()._twoPage);
            pageNumberText.Text = pageText;
            currentTab()._headerTB.Text = pageText;
        }

        /*
         * returns the tab that the user is currently using
         */
        private Tab currentTab()
        {
            return tabArray[tabNumber];
        }

        /*
         * 
         */
        private void tabBar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tab = tabBar.SelectedItem as TabItem;

            if (tab != null && tab.Header != null && tab.Header.Equals("+"))
            {
                TabItem newTab = createTab(1);
            }

            tabArray[tabNumber]._delButton.Visibility = System.Windows.Visibility.Collapsed;

            for (int i = 0; i < tabArray.Count; i++)
            {
                if (tabArray[i]._tab.Equals(tab))
                    tabNumber = i;
            }
            tabArray[tabNumber]._delButton.Visibility = System.Windows.Visibility.Visible;

            loadPage();
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
            if (currentTab()._twoPage)
            {
                SliderText.Text = PageNamer.getPageText(2 * onVal, true);
            }
            else
            {
                SliderText.Text = PageNamer.getPageText(onVal, false);
            }

            SliderText.Margin = new Thickness(SliderDisplay.Width / 2 - SliderText.ActualWidth / 2, -swipeHeight, 0, 0);
            double middle = 1160 + (slider.Width - 30) * onVal / slider.Maximum;
            SliderDisplay.Margin = new Thickness(middle, height, 0, 0);
            SliderDisplay.Opacity = 1;
            if (currentTab()._twoPage)
            {
                SliderImage1.Margin = new Thickness(0, 0, 0, 0);
                SliderImage2.Opacity = 100;
            }
            else
            {
                SliderImage1.Margin = new Thickness(50, 0, 0, 0);
                SliderImage2.Opacity = 0;
            }

            currentTab()._worker.updateSlideImage(onVal);
        }

        /*
         * Called when the slider is released. Goes to the page and hides the slider display
         */
        private void slider_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            SliderDisplay.Opacity = 0;
            SurfaceSlider slider = (SurfaceSlider)sender;
            if (currentTab()._twoPage)
                goToPage((int)Math.Round(2 * slider.Value));
            else
                goToPage((int)Math.Round(slider.Value));
        }

        /*
         * Make the verso page load a high-res image if the user zooms.
         */
        private void makeVersoImageLarge(object sender, SizeChangedEventArgs e)
        {
            if (!currentTab()._worker.bigV)
            {
                ScatterViewItem item = (ScatterViewItem)sender;
                if (item.ActualWidth > minPageWidth)
                {
                    currentTab()._worker.updateVersoImage(true);
                }
            }
        }

        /*
         * Make the recto page load a high-res image if the user zooms.
         */
        private void makeRectoImageLarge(object sender, SizeChangedEventArgs e)
        {
            if (!currentTab()._worker.bigR)
            {
                ScatterViewItem item = (ScatterViewItem)sender;
                if (item.ActualWidth > minPageWidth)
                {
                        currentTab()._worker.updateRectoImage(true);
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
            double x, y, w, h;
            bool fullscreen = false;
            if (currentTab()._twoPage)
            {
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

                if (i.Width < i.MinWidth + 1)
                    fullscreen = true;
            }
            else
            {
                x = i.Center.X;
                y = i.Center.Y;
                w = i.Height / 2;
                h = i.Width / 2;
                if (x > scatterBuffer + w)
                    i.Center = new Point(scatterBuffer + w, y);
                if (x < scatterBuffer + minPageLong - w)
                    i.Center = new Point(scatterBuffer + minPageLong - w, y);

                x = i.Center.X;
                y = i.Center.Y;
                w = i.Height / 2;
                h = i.Width / 2;
                if (y > scatterBuffer + h)
                    i.Center = new Point(x, scatterBuffer + h);
                if (y < scatterBuffer + minPageHeight - h)
                    i.Center = new Point(x, scatterBuffer + minPageHeight - h);

                if (i.Width < i.MinWidth + 1)
                    fullscreen = true;
            }

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
                if (!currentTab()._twoPage && h > minPageLong)
                    multiplier = minPageLong / h;
                if (currentTab()._twoPage && h > minPageHeight)
                    multiplier = minPageHeight / h;
            }
            else
            {
                if (!currentTab()._twoPage && w > minPageHeight)
                    multiplier = minPageHeight / w;
                if (currentTab()._twoPage && w > minPageWidth)
                    multiplier = minPageWidth / w;
            }

            endWidth = s.MaxWidth * multiplier;
            endHeight = s.MaxHeight * multiplier;
            double x, y, xcenter, ycenter;
            x = (r.Left + r.Width / 2) / maxPageWidth;
            y = (r.Top + r.Height / 2) / maxPageHeight;
            xcenter = ((ScatterView)s.Parent).Width / 2;
            ycenter = ((ScatterView)s.Parent).Height / 2;

            if (currentTab()._twoPage)
            {
                xcenter += (.5 - x) * endWidth;
                ycenter += (.5 - y) * endHeight;
            }
            else
            {
                xcenter += (.5 - y) * endHeight;
                ycenter += (x - .5) * endWidth;
            }

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
            ListBox box = (ListBox)sender;
            if (box.SelectedIndex == 0)
                currentLanguage = language.None;
            if (box.SelectedIndex == 1)
                currentLanguage = language.OldFrench;
            if (box.SelectedIndex == 2)
                currentLanguage = language.French;
            if (box.SelectedIndex == 3)
                currentLanguage = language.English;
            currentTab()._worker.setTranslateText(currentLanguage);
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

        private void SearchButton_Clicked(object sender, RoutedEventArgs e)
        {
            tabDynamic.DataContext = null;
            TabItem newTab = this.AddSearchTabItem();
            tabDynamic.DataContext = SidebarTabItems;
            tabDynamic.SelectedItem = newTab;
        }

        private void AnnotateButton_Clicked(object sender, RoutedEventArgs e)
        {
            tabDynamic.DataContext = null;
            TabItem newTab = this.AddAnnotateTabItem();
            tabDynamic.DataContext = SidebarTabItems;
            tabDynamic.SelectedItem = newTab;
        }

        private void StudyButton_Clicked(object sender, RoutedEventArgs e)
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
            tab.deleteTabButton.Click += new RoutedEventHandler(SearchbtnDelete_Click);
            tab.lessOptions.Click += new RoutedEventHandler(Hide_Options);
            tab.searchQueryBox.GotFocus += new RoutedEventHandler(Enter_Search);
            tab.goSearch.Click += new RoutedEventHandler(newSearch);
            tab.searchQueryBox.PreviewKeyDown += new KeyEventHandler(Enter_Clicked);

            // insert tab item right before the last (+) tab item
            SidebarTabItems.Insert(count - 1, tab);
            return tab;
        }

        private void newSearch(object sender, RoutedEventArgs e)
        {
            Stopwatch sw1 = new Stopwatch();
            sw1.Start();

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
                if (selectedTab.selectLanguage.SelectedIndex == 0)
                    poetryResults = Translate.searchFrPoetry(searchQuery, caseType, wordType, xml, engXml, layoutXml);
                else if (selectedTab.selectLanguage.SelectedIndex == 2)
                    poetryResults = Translate.searchEngPoetry(searchQuery, caseType, wordType, xml, engXml, layoutXml);

                ListBox poetryLB = new ListBox();
                poetryLB.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
                selectedTab.poetryScroll.Content = poetryLB;

                foreach (SearchResult result in poetryResults)
                {
                    ResultBoxItem resultRBI = new ResultBoxItem();
                    resultRBI.folioInfo.Text = result.folio;
                    resultRBI.lineInfo.Text = Convert.ToString(result.lineNum);
                    resultRBI.resultType = result.resultType;

                    resultRBI.resultThumbnail = result.thumbnail; /// Add this in once we have a method to get thumbnails
                    ///resultRBI.resultThumbnail.Source = new BitmapImage(new Uri(@"/poetry.jpg", UriKind.Relative));
                    resultRBI.excerpt = result.excerpt;


                    resultRBI.resultText.Text = result.text1 + "\r\n" + result.text2;
                    poetryLB.Items.Add(resultRBI);
                    resultRBI.Selected += new RoutedEventHandler(Result_Closeup);
                }

                selectedTab.poetryTab.Header = "Poetry (" + poetryResults.Count + ")";

                if (poetryResults.Count == 0)
                {
                    TextBlock noResults = new TextBlock();
                    noResults.Text = "Sorry, your search returned no results.";
                    selectedTab.poetryTab.Content = noResults;
                }
                else
                    selectedTab.poetryTab.Content = selectedTab.poetryCanvas;



                // Lyric results
                List<SearchResult> lyricResults = Translate.searchLyrics(searchQuery, caseType, wordType, xml, layoutXml);
                ListBox lyricsLB = new ListBox();
                lyricsLB.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
                selectedTab.lyricsScroll.Content = lyricsLB;



                foreach (SearchResult result in lyricResults)
                {
                    ResultBoxItem resultRBI = new ResultBoxItem();
                    resultRBI.folioInfo.Text = result.folio;
                    resultRBI.resultType = result.resultType;
                    resultRBI.resultThumbnail = result.thumbnail;
                    ///resultRBI.resultThumbnail.Source = result.thumbnail.Source; /// Add this in once we have a method to get thumbnails
                    ///resultRBI.resultThumbnail.Source = new BitmapImage(new Uri(@"/music.jpg", UriKind.Relative));
                    resultRBI.excerpt = result.excerpt;
                    resultRBI.resultText.Text = result.text1;
                    lyricsLB.Items.Add(resultRBI);
                    resultRBI.Selected += new RoutedEventHandler(Result_Closeup);
                }

                selectedTab.lyricsTab.Header = "Lyrics (" + lyricResults.Count + ")";

                if (lyricResults.Count == 0)
                {
                    TextBlock noResults = new TextBlock();
                    noResults.Text = "Sorry, your search returned no results.";
                    selectedTab.lyricsTab.Content = noResults;
                }
                else
                    selectedTab.lyricsTab.Content = selectedTab.lyricsCanvas;



                // Image results
                List<SearchResult> imagesResults = Translate.searchPicCaptions(searchQuery, caseType, wordType, xml, layoutXml);
                ListBox imagesLB = new ListBox();
                imagesLB.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
                selectedTab.imagesScroll.Content = imagesLB;

                foreach (SearchResult result in imagesResults)
                {
                    ResultBoxItem resultRBI = new ResultBoxItem();
                    resultRBI.folioInfo.Text = result.folio;
                    resultRBI.resultType = result.resultType;
                    resultRBI.resultThumbnail = result.thumbnail;
                    resultRBI.excerpt = result.excerpt;
                    resultRBI.resultText.Text = result.text1 + "\r\n" + result.text2;
                    resultRBI.resultText.VerticalAlignment = VerticalAlignment.Top;
                    imagesLB.Items.Add(resultRBI);
                    resultRBI.Selected += new RoutedEventHandler(Result_Closeup);
                }

                selectedTab.imagesTab.Header = "Images (" + imagesResults.Count + ")";

                if (imagesResults.Count == 0)
                {
                    TextBlock noResults = new TextBlock();
                    noResults.Text = "Sorry, your search returned no results.";
                    selectedTab.imagesTab.Content = noResults;
                }
                else
                    selectedTab.imagesTab.Content = selectedTab.imagesCanvas;



                if (selectedTab.optionsCanvas.IsVisible)
                    compressResults();


                sw1.Stop();
                TimeSpan ts1 = sw1.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts1.Hours, ts1.Minutes, ts1.Seconds,
            ts1.Milliseconds / 10);
                Console.WriteLine("Total time for a search: " + elapsedTime);
            }

        }





        private void compressResults()
        {
            SearchTab selectedTab = tabDynamic.SelectedItem as SearchTab;
            selectedTab.searchResults.Height = 525;
            Canvas.SetTop(selectedTab.searchResults, 320);
            selectedTab.poetryPanel.Height = 230;
            Canvas.SetTop(selectedTab.poetryPanel, 220);
            selectedTab.poetryScroll.Height = 200;
            selectedTab.lyricsPanel.Height = 230;
            Canvas.SetTop(selectedTab.lyricsPanel, 220);
            selectedTab.lyricsScroll.Height = 200;
            selectedTab.imagesPanel.Height = 230;
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
            String temp = selectedResult.excerpt;




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
                int lineNum = Convert.ToInt32(selectedResult.lineInfo.Text);
                closeupText.Text = selectedResult.excerpt;
                int firstLine = lineNum - 5;
                int lastLine = lineNum + 5;
                if (firstLine < veryFirstLine)
                    firstLine = veryFirstLine;
                if (lastLine > veryLastLine)
                    lastLine = veryLastLine;
                closeupText.Text += "\r\n\r\nLines " + (firstLine) + " to " + (lastLine);
                selectedTab.poetryPanel.Children.Add(closeupImage);
                selectedTab.poetryPanel.Children.Add(closeupText);
            }

            else if (selectedResult.resultType == 2)
            {
                selectedTab.lyricsPanel.Children.Clear();
                closeupText.Text = selectedResult.excerpt;
                selectedTab.lyricsPanel.Children.Add(closeupImage);
                selectedTab.lyricsPanel.Children.Add(closeupText);
            }

            else if (selectedResult.resultType == 3)
            {
                selectedTab.imagesPanel.Children.Clear();
                closeupText.Text = selectedResult.excerpt;
                selectedTab.imagesPanel.Children.Add(closeupImage);
                selectedTab.imagesPanel.Children.Add(closeupText);
            }

            /// closeupText.Text = boldWords(selectedTab.searchQueryBox.Text, closeupText.Text);



        }

        private String boldWords(String wordToBold, String stringToTransform)
        {
            stringToTransform = stringToTransform.Replace(wordToBold, "bleh");


            return stringToTransform;
        }

        private void Show_Options(object sender, RoutedEventArgs e)
        {
            SearchTab selectedTab = tabDynamic.SelectedItem as SearchTab;
            selectedTab.optionsCanvas.Visibility = Visibility.Visible;
            selectedTab.topLine.Visibility = Visibility.Hidden;
            selectedTab.moreOptions.Visibility = Visibility.Hidden;
            if (selectedTab.searchResults.IsVisible)
                compressResults();
        }

        private void Hide_Options(object sender, RoutedEventArgs e)
        {
            SearchTab selectedTab = tabDynamic.SelectedItem as SearchTab;
            selectedTab.optionsCanvas.Visibility = Visibility.Hidden;
            selectedTab.topLine.Visibility = Visibility.Visible;
            selectedTab.moreOptions.Visibility = Visibility.Visible;

            checkForChanges();

            if (defaultOptionsChanged == true)
                selectedTab.moreOptions.Background = Brushes.MediumTurquoise;

            else
                selectedTab.moreOptions.ClearValue(Control.BackgroundProperty);

            if (selectedTab.searchResults.IsVisible)
                expandResults();

        }

        private Boolean checkForChanges()
        {
            SearchTab selectedTab = tabDynamic.SelectedItem as SearchTab;
            if (selectedTab.caseSensitive.IsChecked == true | selectedTab.wholeWordOnly.IsChecked == true |
                selectedTab.wholePhraseOnly.IsChecked == true | selectedTab.selectLanguage.SelectedIndex != 0)
                defaultOptionsChanged = true;

            else
                defaultOptionsChanged = false;


            return defaultOptionsChanged;
        }

        private void Enter_Search(object sender, RoutedEventArgs e)
        {
            SearchTab selectedTab = tabDynamic.SelectedItem as SearchTab;
            if (selectedTab.searchQueryBox.Text == "Enter text")
            {
                selectedTab.searchQueryBox.Foreground = Brushes.Black;
                selectedTab.searchQueryBox.Text = "";
            }
        }



        private TabItem AddAnnotateTabItem()
        {
            int count = SidebarTabItems.Count;

            // create new tab item
            TabItem tab = new TabItem();
            tab.Name = string.Format("tab{0}", count);
            tab.HeaderTemplate = tabDynamic.FindResource("NewAnnotateTab") as DataTemplate;



            // insert tab item right before the last (+) tab item
            SidebarTabItems.Insert(count - 1, tab);
            return tab;
        }

        private TabItem AddStudyTabItem()
        {
            int count = SidebarTabItems.Count;

            // create new tab item
            TabItem tab = new TabItem();
            tab.Name = string.Format("tab{0}", count);
            tab.HeaderTemplate = tabDynamic.FindResource("NewStudyTab") as DataTemplate;



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

    }
}