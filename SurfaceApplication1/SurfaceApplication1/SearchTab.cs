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
using Microsoft.Surface.Presentation.Input;
using System.Xml;
using System.Windows.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media.Animation;

namespace DigitalFauvel
{
    /**
     * This class defines a Search Tab, opened from the side bar using the "Search" app button. 
     * It creates a default tab with a variety of search options, and it also sets up a hidden result section that will appear once a newSearch (see SideBar.cs) is conducted.
     * The methods in this class support the front end (visual) aspects of the SearchTab and Search app.
     * For the back end methods, see Search.cs.
     * Primary Coder: Alison Y. Chang
     * */
    public class SearchTab : SideBarTab
    {

        public bool? exactPhr;
        public Boolean defaultOptionsChanged, optionsShown;
        public Border poetryBorder, imagesBorder, lyricsBorder;
        public Button moreOptions, fewerOptions, goSearch, selectLanguageButton, closeLanguageList;
        public Canvas poetryCanvas, lyricsCanvas, imagesCanvas;
        public CheckBox caseSensitive, wholeWordOnly, exactPhraseOnly;
        public Grid fewerOptGrid, optionsGrid;
        public Image downArrow, upArrow;
        private int unreturnedResults;
        public Line topLine, bottomLine;
        private List<SearchResult> poetryResults, lyricResults, imageResults;
        public ResultBoxItem lastCloseupRBI;
        public ScaleTransform st;
        public enum searchLanguage { oldFrench = 0, modernFrench = 1, English = 2 };
        public searchLanguage currentSearchLanguage = searchLanguage.oldFrench, lastSearchLanguage;
        private SideBar sideBar;
        public StackPanel poetryPanel, lyricsPanel, imagesPanel;
        public String newPageToOpen, lastPageToOpen;
        public SurfaceListBox selectLanguage;
        public SurfaceListBoxItem pickLanguage, oldFrench, modernFrench, English;
        public SurfaceScrollViewer poetryScroll, lyricsScroll, imagesScroll;
        private SurfaceWindow1 surfaceWindow;
        public TabControl searchResults;
        public TabItem poetryTab, lyricsTab, imagesTab;
        public TextBlock searchPrompt, searchTabHeader, moreOptText, fewerOptText;
        public TextBox searchQueryBox;
        private Image loadImage;



        public SearchTab(SideBar mySideBar, SurfaceWindow1 surfaceWindow) : base(mySideBar)
        {
            unreturnedResults = 0;
            sideBar = mySideBar;
            this.surfaceWindow = surfaceWindow;
            searchPrompt = new TextBlock();
            searchTabHeader = new TextBlock();
            searchQueryBox = new TextBox();
            goSearch = new Button();
            moreOptions = new Button();
            topLine = new Line();

            closeLanguageList = new Button();
            fewerOptions = new Button();
            upArrow = new Image();
            caseSensitive = new CheckBox();
            wholeWordOnly = new CheckBox();
            exactPhraseOnly = new CheckBox();
            st = new ScaleTransform();
            bottomLine = new Line();

            selectLanguage = new SurfaceListBox();
            pickLanguage = new SurfaceListBoxItem();
            oldFrench = new SurfaceListBoxItem();
            modernFrench = new SurfaceListBoxItem();
            English = new SurfaceListBoxItem();
            selectLanguageButton = new Button();

            searchResults = new TabControl();
            poetryTab = new TabItem();
            lyricsTab = new TabItem();
            imagesTab = new TabItem();
            poetryCanvas = new Canvas();
            poetryScroll = new SurfaceScrollViewer(); 
            poetryPanel = new StackPanel();
            lyricsCanvas = new Canvas();
            lyricsScroll = new SurfaceScrollViewer(); 
            lyricsPanel = new StackPanel();
            imagesCanvas = new Canvas();
            imagesScroll = new SurfaceScrollViewer(); 
            imagesPanel = new StackPanel();

            headerImage.Source = new BitmapImage(new Uri(@"..\..\icons\search.png", UriKind.Relative));

            loadImage = new Image();
            loadImage.Source = new BitmapImage(new Uri(@"..\..\icons\magnifyingglass.png", UriKind.Relative));
            canvas.Children.Add(loadImage);

            headerImage.Source = new BitmapImage(new Uri(@"..\..\icons\magnifyingglass.png", UriKind.Relative));

            searchTabHeader.HorizontalAlignment = HorizontalAlignment.Center;
            searchTabHeader.VerticalAlignment = VerticalAlignment.Center;
            searchTabHeader.FontSize = 21;

            searchPrompt.FontSize = 30;
            searchPrompt.Text = "Search:";
            Canvas.SetLeft(searchPrompt, 32);
            Canvas.SetTop(searchPrompt, 26);

            searchQueryBox.Height = 40;
            searchQueryBox.Width = 380; //315
            searchQueryBox.Foreground = Brushes.Gray;
            searchQueryBox.FontSize = 21;
            searchQueryBox.Text = "Enter text";
            Canvas.SetLeft(searchQueryBox, 40);
            Canvas.SetTop(searchQueryBox, 90);

            goSearch.Height = 40;
            goSearch.Width = 95;
            goSearch.FontSize = 21;
            goSearch.Content = "Go!";
            goSearch.IsEnabled = false;
            Canvas.SetLeft(goSearch, 450); // 378
            Canvas.SetTop(goSearch, 90);

           
            downArrow = new Image();
            downArrow.Source = new BitmapImage(new Uri(@"/downArrow.png", UriKind.Relative));
            downArrow.Opacity = 0.3;
            downArrow.HorizontalAlignment = HorizontalAlignment.Center;
            moreOptText = new TextBlock();
            moreOptText.Text = "More Options";
            moreOptText.FontSize = 18; /// Might need to adjust height 
            optionsGrid = new Grid();
            moreOptions.Content = optionsGrid;
            moreOptions.Width = 135; // 100
            moreOptions.Height = 28; // 20
            moreOptions.HorizontalContentAlignment = HorizontalAlignment.Center;
            optionsGrid.Children.Add(downArrow);
            optionsGrid.Children.Add(moreOptText);
            Canvas.SetLeft(moreOptions, 230); //210
            Canvas.SetTop(moreOptions, 145);
            
            
            topLine.X1 = 40;
            topLine.Y1 = 160;
            topLine.X2 = 540; // 500
            topLine.Y2 = 160;
            topLine.Stroke = Brushes.Black;
            topLine.StrokeThickness = 2;

            closeLanguageList.Width = 600; // 550
            closeLanguageList.Height = 1000; //900 
            closeLanguageList.Style = sideBar.tabBar.FindResource("InvisibleButton") as Style;



            /// The objects for extended search options
            st.ScaleX = 2;
            st.ScaleY = 2;

            caseSensitive.FontSize = 10;
            caseSensitive.LayoutTransform = st;
            caseSensitive.Content = (string)"Case sensitive";
            Canvas.SetLeft(caseSensitive, 55); //40
            Canvas.SetTop(caseSensitive, 170);
            

            wholeWordOnly.FontSize = 10;
            wholeWordOnly.LayoutTransform = st;
            wholeWordOnly.Content = (string)"Match whole word only";
            Canvas.SetLeft(wholeWordOnly, 300); //243
            Canvas.SetTop(wholeWordOnly, 170);

            exactPhraseOnly.FontSize = 10;
            exactPhraseOnly.LayoutTransform = st;
            exactPhraseOnly.Content = (string)"Match exact phrase only";
            Canvas.SetLeft(exactPhraseOnly, 300); // 243
            Canvas.SetTop(exactPhraseOnly, 227);

            selectLanguage.Background = Brushes.LightGray;
            selectLanguage.Visibility = Visibility.Collapsed;
            selectLanguage.Width = 175;
            selectLanguage.FontSize = 21;
            selectLanguage.HorizontalContentAlignment = HorizontalAlignment.Center;
            selectLanguage.SelectedIndex = 0;


          

            Canvas.SetLeft(selectLanguage, 50); //34
            Canvas.SetTop(selectLanguage, 220); 
            Canvas.SetLeft(selectLanguageButton, 50); //34
            Canvas.SetTop(selectLanguageButton, 220);
            selectLanguageButton.Width = 175;
            selectLanguageButton.Height = 40;
            selectLanguageButton.Visibility = Visibility.Hidden;
            selectLanguageButton.Content = (string)"Old French";
            selectLanguageButton.FontSize = 21;

            pickLanguage.Content = (string)"Pick a language:";
            oldFrench.Content = (string)"Old French";
            modernFrench.Content = (string)"Modern French";
            English.Content = (string)"English";

            selectLanguage.Items.Add(pickLanguage);
            selectLanguage.Items.Add(oldFrench);
            selectLanguage.Items.Add(modernFrench);
            selectLanguage.Items.Add(English);

            foreach (SurfaceListBoxItem s in selectLanguage.Items)
            {
                s.FontFamily = new FontFamily("Cambria");
                s.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                s.FontSize = 21;
                s.Height = 40;
            }

            bottomLine.X1 = 40;
            bottomLine.Y1 = 183;
            bottomLine.X2 = 540; //500
            bottomLine.Y2 = 183;
            bottomLine.Stroke = Brushes.Black;
            bottomLine.StrokeThickness = 2;
            Canvas.SetTop(bottomLine, 113);

            upArrow.Source = new BitmapImage(new Uri(@"/upArrow.png", UriKind.Relative));
            upArrow.Opacity = 0.3;
            upArrow.HorizontalAlignment = HorizontalAlignment.Center;
            fewerOptText = new TextBlock();
            fewerOptText.Text = "Fewer Options";
            fewerOptText.FontSize = 18; /// Might need to adjust height 
            fewerOptGrid = new Grid();
            fewerOptions.Content = fewerOptGrid;
            fewerOptions.Width = 135;
            fewerOptions.Height = 28;
            fewerOptions.HorizontalContentAlignment = HorizontalAlignment.Center;
            fewerOptGrid.Children.Add(upArrow);
            fewerOptGrid.Children.Add(fewerOptText);
            Canvas.SetLeft(fewerOptions, 230); // 210
            Canvas.SetTop(fewerOptions, 280); // 285



            /// The objects on the search results section
            searchResults.Visibility = Visibility.Hidden;
            searchResults.Height = 800; //677
            searchResults.Width = 525; //482
            searchResults.FontSize = 21;
            Canvas.SetLeft(searchResults, 35); //30
            Canvas.SetTop(searchResults, 180);
            searchResults.Items.Add(poetryTab);
            searchResults.Items.Add(lyricsTab);
            searchResults.Items.Add(imagesTab);

            poetryBorder = new Border();
            poetryBorder.Child = poetryPanel;
            poetryBorder.Style = sideBar.tabBar.FindResource("ResultBorder") as Style;

            poetryTab.Header = "Poetry";
            poetryTab.Height = 40;
            poetryTab.Width = 170; 
            poetryTab.Content = poetryCanvas;

            poetryCanvas.Height = 750; 
            poetryCanvas.Children.Add(poetryScroll);
            poetryCanvas.Children.Add(poetryBorder);

            poetryScroll.Height = 410; 
            poetryScroll.Width = 513; 
            poetryScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            poetryScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            poetryScroll.PanningMode = PanningMode.VerticalOnly;
            poetryPanel.Orientation = Orientation.Horizontal;

            lyricsBorder = new Border();
            lyricsBorder.Child = lyricsPanel;
            lyricsBorder.Style = sideBar.tabBar.FindResource("ResultBorder") as Style;


            lyricsTab.Header = "Lyrics";
            lyricsTab.Height = 40;
            lyricsTab.Width = 170;
            lyricsTab.Content = lyricsCanvas;
            lyricsCanvas.Height = 750; 
            lyricsCanvas.Children.Add(lyricsScroll);
            lyricsCanvas.Children.Add(lyricsBorder);
            lyricsScroll.Height = 410; 
            lyricsScroll.Width = 513;
            lyricsScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            lyricsScroll.PanningMode = PanningMode.VerticalOnly;
            lyricsPanel.Orientation = Orientation.Horizontal;

            
            imagesBorder = new Border();
            imagesBorder.Child = imagesPanel;
            imagesBorder.Style = sideBar.tabBar.FindResource("ResultBorder") as Style;


            imagesTab.Header = "Images";
            imagesTab.Height = 40;
            imagesTab.Width = 170; 
            imagesTab.Content = imagesCanvas;
            imagesCanvas.Height = 750; 
            imagesCanvas.Children.Add(imagesScroll);
            imagesCanvas.Children.Add(imagesBorder);
            imagesScroll.Height = 410; 
            imagesScroll.Width = 513;

            imagesScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            imagesScroll.PanningMode = PanningMode.VerticalOnly;
            imagesPanel.Orientation = Orientation.Horizontal;




            /// Adding everything

            headerGrid.Children.Add(searchTabHeader);

            canvas.Children.Add(closeLanguageList); // Should add to the very back...
            canvas.Children.Add(searchPrompt);
            canvas.Children.Add(searchQueryBox);
            canvas.Children.Add(goSearch);
            canvas.Children.Add(topLine);
            canvas.Children.Add(moreOptions);

            canvas.Children.Add(searchResults);
            canvas.Children.Add(caseSensitive);
            canvas.Children.Add(bottomLine);
            canvas.Children.Add(selectLanguage);
            canvas.Children.Add(selectLanguageButton);
            canvas.Children.Add(fewerOptions);
            canvas.Children.Add(wholeWordOnly);
            canvas.Children.Add(exactPhraseOnly);
            caseSensitive.Visibility = Visibility.Hidden;
            selectLanguage.Visibility = Visibility.Hidden;
            bottomLine.Visibility = Visibility.Hidden;
            fewerOptions.Visibility = Visibility.Hidden;
            wholeWordOnly.Visibility = Visibility.Hidden;
            exactPhraseOnly.Visibility = Visibility.Hidden;

            closeLanguageList.TouchEnter += new EventHandler<TouchEventArgs>(closeLanguageList_TouchEnter);
            closeLanguageList.MouseLeave += new MouseEventHandler(closeLanguageList_MouseLeave);
            closeLanguageList.Click += new RoutedEventHandler(closeLanguageList_Click);
            moreOptions.Click += new RoutedEventHandler(Show_Options);
            moreOptions.TouchDown += new EventHandler<TouchEventArgs>(Show_Options);
            fewerOptions.Click += new RoutedEventHandler(Hide_Options);
            fewerOptions.TouchDown += new EventHandler<TouchEventArgs>(Hide_Options);
            searchQueryBox.GotFocus += new RoutedEventHandler(Focus_SearchBox);
            searchQueryBox.TouchDown += new EventHandler<TouchEventArgs>(Focus_SearchBox);
            goSearch.Click += new RoutedEventHandler(newSearch);
            goSearch.TouchDown += new EventHandler<TouchEventArgs>(newSearch);
            searchQueryBox.PreviewKeyDown += new KeyEventHandler(Enter_Clicked);
            caseSensitive.TouchDown += new EventHandler<TouchEventArgs>(changeCheck);
            exactPhraseOnly.TouchDown += new EventHandler<TouchEventArgs>(changeCheck);
            wholeWordOnly.TouchDown += new EventHandler<TouchEventArgs>(changeCheck);

            //selectLanguage.TouchDown += new EventHandler<TouchEventArgs>(displaySearchLanguages);
            //selectLanguage.SelectionChanged += new SelectionChangedEventHandler(searchLanguageChanged);
            selectLanguage.Visibility = Visibility.Collapsed;
            selectLanguageButton.TouchDown += new EventHandler<TouchEventArgs>(displaySearchLanguages);
            selectLanguageButton.Click += new RoutedEventHandler(displaySearchLanguages);
            pickLanguage.Selected += new RoutedEventHandler(searchLanguageChanged);
            oldFrench.Selected += new RoutedEventHandler(searchLanguageChanged);
            modernFrench.Selected += new RoutedEventHandler(searchLanguageChanged);
            English.Selected += new RoutedEventHandler(searchLanguageChanged);
        }


        private void closeLanguageList_Click(object sender, RoutedEventArgs e)
        {

        }

        /**
         * Displays the language choices for search.
         * */
        private void displaySearchLanguages(object sender, RoutedEventArgs e)
        {
            if (selectLanguage.Visibility == Visibility.Collapsed | selectLanguage.Visibility == Visibility.Hidden)
            {
                selectLanguage.Visibility = Visibility.Visible;
                selectLanguageButton.Visibility = Visibility.Collapsed;
            }
            else
                selectLanguage.Visibility = Visibility.Collapsed;

        }

        /**
         * Sets a new current search language.
         * Hides the search language options.
         * */
        private void searchLanguageChanged(object sender, RoutedEventArgs e)
        {
            SurfaceListBoxItem box = (SurfaceListBoxItem)sender;
            selectLanguageButton.Content = box.Content;
            selectLanguage.Visibility = Visibility.Hidden;
            selectLanguageButton.Visibility = Visibility.Visible;

            if (box == oldFrench)
                currentSearchLanguage = searchLanguage.oldFrench;
            else if (box == modernFrench)
                currentSearchLanguage = searchLanguage.modernFrench;
            else if (box == English)
                currentSearchLanguage = searchLanguage.English;
            else if (box == pickLanguage)
                currentSearchLanguage = lastSearchLanguage;

            lastSearchLanguage = currentSearchLanguage;
        }
        
        /**
         * Shows extended search options.
         * */
        private void Show_Options(object sender, RoutedEventArgs e)
        {
            optionsShown = true;
            topLine.Visibility = Visibility.Hidden;
            caseSensitive.Visibility = Visibility.Visible;
            bottomLine.Visibility = Visibility.Visible;
            fewerOptions.Visibility = Visibility.Visible;
            wholeWordOnly.Visibility = Visibility.Visible;
            exactPhraseOnly.Visibility = Visibility.Visible;
            moreOptions.Visibility = Visibility.Hidden;
            selectLanguageButton.Visibility = Visibility.Visible;
            if (searchResults.IsVisible)
                compressResults();

        }

        /**
         * Hides advanced search settings.
         * */
        private void Hide_Options(object sender, RoutedEventArgs e)
        {
            topLine.Visibility = Visibility.Visible;
            moreOptions.Visibility = Visibility.Visible;
            caseSensitive.Visibility = Visibility.Hidden;
            selectLanguage.Visibility = Visibility.Hidden;
            bottomLine.Visibility = Visibility.Hidden;
            fewerOptions.Visibility = Visibility.Hidden;
            wholeWordOnly.Visibility = Visibility.Hidden;
            exactPhraseOnly.Visibility = Visibility.Hidden;
            selectLanguageButton.Visibility = Visibility.Hidden;

            checkForChanges();

            if (defaultOptionsChanged == true)
                moreOptions.Background = SurfaceWindow1.glowColor; // A color used to draw attention to change

            else
                moreOptions.ClearValue(Control.BackgroundProperty);

            if (searchResults.IsVisible)
                expandResults();

            optionsShown = false;
        }


        private void closeLanguageList_TouchEnter(object sender, EventArgs e)
        {
            closeLanguageList.Background = Brushes.GhostWhite;
        }
        private void closeLanguageList_MouseLeave(object sender, EventArgs e)
        {
            closeLanguageList.Background = Brushes.GhostWhite;
        }



        /**
         * Changes a check box from checked to unchecked and vice versa.
         * */
        private void changeCheck(object sender, TouchEventArgs e)
        {
            CheckBox thisbox = sender as CheckBox;

            if (thisbox.IsChecked == true)
                thisbox.IsChecked = false;
            else if (thisbox.IsChecked == false)
                thisbox.IsChecked = true;
        }

        private Grid getLoadingImage()
        {
            Grid grid = new Grid();
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(@"..\..\icons\loading.png", UriKind.Relative));
            grid.Children.Add(image);

            DoubleAnimationUsingKeyFrames dakf = new DoubleAnimationUsingKeyFrames();
            dakf.KeyFrames = new DoubleKeyFrameCollection();
            dakf.KeyFrames.Add(new DiscreteDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(1))));
            dakf.KeyFrames.Add(new DiscreteDoubleKeyFrame(45, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(50))));
            dakf.KeyFrames.Add(new DiscreteDoubleKeyFrame(90, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100))));
            dakf.KeyFrames.Add(new DiscreteDoubleKeyFrame(135, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(150))));
            dakf.KeyFrames.Add(new DiscreteDoubleKeyFrame(180, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200))));
            dakf.KeyFrames.Add(new DiscreteDoubleKeyFrame(225, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(250))));
            dakf.KeyFrames.Add(new DiscreteDoubleKeyFrame(270, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(300))));
            dakf.KeyFrames.Add(new DiscreteDoubleKeyFrame(315, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(350))));

            dakf.Duration = new Duration(TimeSpan.FromMilliseconds(400));
            dakf.RepeatBehavior = RepeatBehavior.Forever;
            RotateTransform rt = new RotateTransform();
            image.RenderTransform = rt;
            image.RenderTransformOrigin = new Point(0.5, 0.5);
            rt.BeginAnimation(RotateTransform.AngleProperty, dakf);

            return grid;
        }

        /**
         * Runs a text search in Fauvel, using all specified search settings.
         * Each type of search (poetry, music lyrics, and images) is in its own thread.
         * */
        private void runSearch()
        {
            String searchQuery = searchQueryBox.Text;
            SurfaceKeyboard.IsVisible = false;

            searchTabHeader.Text = searchQueryBox.Text;
            searchResults.Visibility = Visibility.Visible;

            if (optionsShown)
            {
                selectLanguage.Visibility = Visibility.Hidden;
                selectLanguageButton.Visibility = Visibility.Visible;
            }

            int caseType = 0;
            int wordType = 0;
            if (caseSensitive.IsChecked == true)
                caseType = 1;
            if (wholeWordOnly.IsChecked == true)
                wordType = 1;
                
	        exactPhr = exactPhraseOnly.IsChecked;

            if (optionsShown == true)
                compressResults();

            poetryTab.Content = getLoadingImage();
            lyricsTab.Content = getLoadingImage();
            imagesTab.Content = getLoadingImage();
            poetryTab.Header = "Poetry (...)";
            lyricsTab.Header = "Lyrics (...)";
            imagesTab.Header = "Images (...)";
            searchQueryBox.IsEnabled = false;
            caseSensitive.IsEnabled = false;
            wholeWordOnly.IsEnabled = false;
            exactPhraseOnly.IsEnabled = false;
            moreOptions.IsEnabled = false;
            fewerOptions.IsEnabled = false;
            selectLanguageButton.IsEnabled = false;
            goSearch.Content = "searching";
            goSearch.IsEnabled = false;
            unreturnedResults = 3;

            poetryScroll.ScrollToTop();
            lyricsScroll.ScrollToTop();
            imagesScroll.ScrollToTop();
            poetryPanel.Children.Clear();
            lyricsPanel.Children.Clear();
            imagesPanel.Children.Clear();



            // Poetry results //
            poetryResults = new List<SearchResult>();

            Action poetryResultAction = delegate
            {
                BackgroundWorker worker = new BackgroundWorker();

                worker.DoWork += delegate 
                {
                    if (exactPhr == false)
                        poetryResults = Search.searchMultipleWordsPoetry(searchQuery, caseType, wordType, (int)currentSearchLanguage);
                    else
                        poetryResults = Search.searchExactPoetry(searchQuery, caseType, wordType, (int)currentSearchLanguage);
                };
                worker.RunWorkerCompleted += delegate
                {
                    ListBox poetryLB = new ListBox();
                    poetryLB.Style = sideBar.tabBar.FindResource("SearchResultSurfaceListBox") as Style;
                    poetryScroll.Content = poetryLB; // NB: the scroll bar comes from the poetryScroll, not poetryLB
                    
                    foreach (SearchResult result in poetryResults)
                    {
                        ResultBoxItem resultRBI = new ResultBoxItem();
                        convertSearchResultToResultBoxItem(result, resultRBI);
                        resultRBI.resultThumbnail.Source = Thumbnailer.getThumbnail(result.tag);
                        if (((optionsShown == false) && poetryResults.Count < 4) || ((optionsShown == true) && poetryResults.Count < 2))
                            resultRBI.Width = 480;
                        poetryLB.Items.Add(resultRBI);
                    }

                    poetryTab.Header = "Poetry (" + poetryResults.Count + ")";

                    if (poetryResults.Count == 0)
                    {
                        TextBlock noResults = new TextBlock();
                        noResults.Text = "Sorry, your search returned no poetry results.";
                        poetryTab.Content = noResults;
                    }
                    else
                        poetryTab.Content = poetryCanvas;

                    poetryScroll.Background = Brushes.LightGray;
                    poetryBorder.BorderBrush = Brushes.DarkGray;
                    returnAResult();
                };
                worker.RunWorkerAsync();
            };
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, poetryResultAction);


            // Lyric results //
            lyricResults = new List<SearchResult>();

            Action lyricResultAction = delegate
            {
                BackgroundWorker worker = new BackgroundWorker();

                worker.DoWork += delegate 
                {
                    if (exactPhr == false)
                        lyricResults = Search.searchMultipleWordsLyrics(searchQuery, caseType, wordType, (int) currentSearchLanguage);
                    else
                        lyricResults = Search.searchExactLyrics(searchQuery, caseType, wordType, Search.whichXml((int)currentSearchLanguage));
                };


                worker.RunWorkerCompleted += delegate
                {
                    ListBox lyricsLB = new ListBox();

                    foreach (SearchResult result in lyricResults)
                    {
                        ResultBoxItem resultRBI = new ResultBoxItem();
                        convertSearchResultToResultBoxItem(result, resultRBI);
                        resultRBI.resultThumbnail.Source = Thumbnailer.getThumbnail(result.tag);
                        lyricsLB.Items.Add(resultRBI);
                    }

                    lyricsLB.Style = sideBar.tabBar.FindResource("SearchResultSurfaceListBox") as Style;

                    lyricsScroll.Content = lyricsLB;

                    lyricsTab.Header = "Lyrics (" + lyricResults.Count + ")";

                    if (lyricResults.Count == 0)
                    {
                        TextBlock noResults = new TextBlock();
                        noResults.Text = "Sorry, your search returned no music lyric results.\r\n\r\nNB: Lyrics don't exist in English yet!";
                        lyricsTab.Content = noResults;
                    }
                    else
                        lyricsTab.Content = lyricsCanvas;

                    lyricsScroll.Background = Brushes.LightGray;
                    lyricsBorder.BorderBrush = Brushes.DarkGray;
                    returnAResult();
                };
                worker.RunWorkerAsync();
            };
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, lyricResultAction);
            

            // Image results //
            imageResults = new List<SearchResult>();

            Action imageResultAction = delegate
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += delegate
                {
                    
                    if (exactPhr == false)
                        imageResults = Search.searchMultipleWordsPicCaptions(searchQuery, caseType, wordType, Search.whichXml((int) currentSearchLanguage));
                    else
                        imageResults = Search.searchExactPicCaptions(searchQuery, caseType, wordType, Search.whichXml((int)currentSearchLanguage));
                };
                worker.RunWorkerCompleted += delegate
                {
                    ListBox imagesLB = new ListBox();
                    imagesLB.Style = sideBar.tabBar.FindResource("SearchResultSurfaceListBox") as Style;
                    imagesScroll.Content = imagesLB;

                    foreach (SearchResult result in imageResults)
                    {
                        ResultBoxItem resultRBI = new ResultBoxItem();
                        convertSearchResultToResultBoxItem(result, resultRBI);
                        resultRBI.miniThumbnail.Source = new BitmapImage(new Uri(@"..\..\minithumbnails\" + result.tag + ".jpg", UriKind.Relative));
                        resultRBI.miniThumbnail.Width = 50;
                        resultRBI.miniThumbnail.Height = 50;
                        resultRBI.resultThumbnail.Source = Thumbnailer.getThumbnail(result.tag);
                        imagesLB.Items.Add(resultRBI);
                    }

                    imagesTab.Header = "Images (" + imageResults.Count + ")";

                    if (imageResults.Count == 0)
                    {
                        TextBlock noResults = new TextBlock();
                        noResults.Text = "Sorry, your search returned no image results.\r\n\r\nNB: Image captions don't exist in English yet!";
                        imagesTab.Content = noResults;
                    }
                    else
                        imagesTab.Content = imagesCanvas;

                    imagesScroll.Background = Brushes.LightGray;
                    imagesBorder.BorderBrush = Brushes.DarkGray;
                    returnAResult();
                };
                worker.RunWorkerAsync();
            };
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, imageResultAction);
            
        }

        /**
         * Checks whether the search has been completed.
         * Flips to a tab with results if the current one has none.
         * */
        private void returnAResult()
        {
            unreturnedResults--;
            if (unreturnedResults == 0)
            {
                searchQueryBox.IsEnabled = true;
                goSearch.Content = "Go!";
                goSearch.IsEnabled = true;
                caseSensitive.IsEnabled = true;
                wholeWordOnly.IsEnabled = true;
                exactPhraseOnly.IsEnabled = true;
                moreOptions.IsEnabled = true;
                fewerOptions.IsEnabled = true;
                selectLanguageButton.IsEnabled = true;

                //Auto flip to a tab with results if the current one has none
                if (searchResults.SelectedItem == poetryTab && poetryResults.Count == 0)
                {
                    if (lyricResults.Count != 0)
                        searchResults.SelectedItem = lyricsTab;
                    else if (imageResults.Count != 0)
                        searchResults.SelectedItem = imagesTab;
                }
                else if (searchResults.SelectedItem == lyricsTab && lyricResults.Count == 0)
                {
                    if (poetryResults.Count != 0)
                        searchResults.SelectedItem = poetryTab;
                    else if (imageResults.Count != 0)
                        searchResults.SelectedItem = imagesTab;
                }
                else if (searchResults.SelectedItem == imagesTab && imageResults.Count == 0)
                {
                    if (poetryResults.Count != 0)
                        searchResults.SelectedItem = poetryTab;
                    else if (lyricResults.Count != 0)
                        searchResults.SelectedItem = lyricsTab;
                }
            }
        }

        /**
         * Runs a new search.
         * */
        private void newSearch(object sender, RoutedEventArgs e)
        {
            String searchQuery = searchQueryBox.Text;
            if(searchQuery.Trim()!="")
                runSearch();

        }

        /**
         * Transfers information about a search result into a ResultBoxItem for ListBox display.
         * */
        private void convertSearchResultToResultBoxItem(SearchResult sr, ResultBoxItem rbi)
        {
            rbi.folioInfo.Text = sr.folio;
            rbi.topL = sr.topL;
            rbi.bottomR = sr.bottomR;
            rbi.matchStrength = sr.matchStrength;
            rbi.resultType = sr.resultType;
            if (rbi.resultType == 1)
                rbi.lineInfo.Text = Convert.ToString(sr.lineNum) + sr.lineRange; // Assuming only one will be filled out
            rbi.excerpts = sr.excerpts;
            rbi.Height = 80; // temp taller than desired, so scrollbar shows
            rbi.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            rbi.Width = 475; //430
            rbi.Style = sideBar.tabBar.FindResource("SearchResultSurfaceListBoxItem") as Style; // Not sure if this works..

            if (rbi.resultType == 2 || rbi.resultType == 3) // For lyrics or images
                rbi.resultText.Text = sr.text1 + "\r\n" + sr.text2;
            else // For poetry
            {
                rbi.resultText.Inlines.Add(new Run { FontFamily = new FontFamily("Cambria"), Text = sr.text1 + "\r\n", FontStyle = FontStyles.Normal });
                rbi.resultText.Inlines.Add(new Run { FontFamily = new FontFamily("Cambria"), Text = sr.text2, FontStyle = FontStyles.Italic });
            }

            rbi.BorderBrush = Brushes.LightGray;
            rbi.BorderThickness = new Thickness(1.0);
            rbi.Selected += new RoutedEventHandler(Result_Closeup);
        }

        /**
         * Compresses the results section if extended search options are shown.
         * */
        private void compressResults()
        {
            // height 800, set at 180
            searchResults.Height = 660; //537
            Canvas.SetTop(searchResults, 320);
            Canvas.SetTop(poetryBorder, 275);
            poetryScroll.Height = 270; //230
            Canvas.SetTop(lyricsBorder, 275);
            lyricsScroll.Height = 270;
            Canvas.SetTop(imagesBorder, 275);
            imagesScroll.Height = 270;
        }

        /**
         * Expands the results section if extended search options are now hidden.
         * */
        private void expandResults()
        {
            searchResults.Height = 800; //677
            Canvas.SetTop(searchResults, 180);
            Canvas.SetTop(poetryBorder, 415);
            poetryScroll.Height = 410;
            Canvas.SetTop(lyricsBorder, 415);
            lyricsScroll.Height = 410;
            Canvas.SetTop(imagesBorder, 415);
            imagesScroll.Height = 410;
        }

        /**
         * Launches a new search if the "Enter" or "Return" key is hit on the virtual keyboard.
         * */
        private void Enter_Clicked(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                newSearch(sender, e);
                e.Handled = true;
            }
        }

        /**
         * Selects a search result for closeup (bottom rectangle of search panel).
         * Shows a thumbnail (image) and a text excerpt.
         * */
        private void Result_Closeup(object sender, RoutedEventArgs e)
        {
            ResultBoxItem selectedResult = e.Source as ResultBoxItem;
            lastCloseupRBI = selectedResult;  // For later reference in goToFolio

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
                poetryPanel.Children.Clear();
                poetryPanel.Children.Add(closeupImage);
                poetryPanel.Children.Add(closeupText);
                poetryPanel.TouchDown += new EventHandler<TouchEventArgs>(goToFolio);
                poetryPanel.MouseLeftButtonDown += new MouseButtonEventHandler(goToFolio);
            }

            else if (selectedResult.resultType == 2)
            {
                lyricsPanel.Children.Clear();
                lyricsPanel.Children.Add(closeupImage);
                lyricsPanel.Children.Add(closeupText);
                lyricsPanel.TouchDown += new EventHandler<TouchEventArgs>(goToFolio);
                lyricsPanel.MouseLeftButtonDown += new MouseButtonEventHandler(goToFolio);
            }

            else if (selectedResult.resultType == 3)
            {
                imagesPanel.Children.Clear();
                imagesPanel.Children.Add(closeupImage);
                imagesPanel.Children.Add(closeupText);
                imagesPanel.TouchDown += new EventHandler<TouchEventArgs>(goToFolio);
                imagesPanel.MouseLeftButtonDown += new MouseButtonEventHandler(goToFolio);
            }

            newPageToOpen = selectedResult.folioInfo.Text;

            // Bolds all search terms
            foreach (SpecialString ss in selectedResult.excerpts)
            {
                if (ss.isStyled == 1)
                    closeupText.Inlines.Add(new Run { FontFamily = new FontFamily("Cambria"), Text = ss.str, FontWeight = FontWeights.Bold });
                else
                    closeupText.Inlines.Add(new Run { FontFamily = new FontFamily("Cambria"), Text = ss.str, FontWeight = FontWeights.Normal });
            }
        }


        /**
         * Fetches the name of the image corresponding to a page of the folio.
         * */
        public static String getImageName(String folio)
        {
            String imageName = "";
            XmlNode node = SurfaceWindow1.layoutXml.DocumentElement.SelectSingleNode("//surface[@id='" + folio + "']");
            imageName = node.FirstChild.SelectSingleNode("graphic").Attributes["url"].Value;

            return imageName;
        }


        /**
         * Navigates from a search result closeup to a new tab open to that result's page.
         * Refers to the last ResultBoxItem selected for closeup.
         * */
        private void goToFolio(object sender, RoutedEventArgs e)
        {
            if(newPageToOpen != lastPageToOpen) 
            {
                String folioStr = lastCloseupRBI.folioInfo.Text;
                if (folioStr.StartsWith("Fo"))
                    folioStr = folioStr.Substring(2);
                String imageName = getImageName(folioStr);
                int pageNum = Convert.ToInt32(imageName.Substring(0, imageName.IndexOf(".jpg")));
                if (pageNum % 2 == 1) // If odd, meaning it's a Fo_r, we want to aim for the previous page.
                    pageNum--;
                editCoordinates(lastCloseupRBI); // Adds width of a page to coordinates from a recto (right side) of an opening


                // Get coordinates from lastCloseupRBI.topL and lastCloseupRBI.bottomR
                surfaceWindow.createTab(pageNum - 10);
                double x, y, w, h;
                x = lastCloseupRBI.topL.X;
                y = lastCloseupRBI.topL.Y;
                w = lastCloseupRBI.bottomR.X - lastCloseupRBI.topL.X;
                h = lastCloseupRBI.bottomR.Y - lastCloseupRBI.topL.Y;
                surfaceWindow.resizePageToRect(new Rect(x, y, w, h));
                surfaceWindow.changeLanguage((int)currentSearchLanguage + 1);
                surfaceWindow.testText.Text = x.ToString() + " " + y.ToString();// +" " + w.ToString() + " " + h.ToString();

                lastPageToOpen = newPageToOpen;
            }
        }
        

        /**
         * This method checks if the target page in goToFolio is a "recto", on the right side.
         * If it is, since each opening of 2 pages is stored as a single combined image, each of the X coordinates must be adjusted accordingly by adding 5250 (width of a page).
         * */
        private void editCoordinates(ResultBoxItem rbi)
        {
            if (rbi.folioInfo.Text.EndsWith("r"))
            {
                rbi.topL.X += 5250;
                rbi.bottomR.X += 5250;
            }

        }

        /**
         * Checks whether the default search settings 
         * */
        private Boolean checkForChanges()
        {
            if (caseSensitive.IsChecked == true | wholeWordOnly.IsChecked == true |
                exactPhraseOnly.IsChecked == true | (selectLanguage.SelectedIndex != 0 && selectLanguage.SelectedIndex != 1))
                defaultOptionsChanged = true;

            else
                defaultOptionsChanged = false;


            return defaultOptionsChanged;
        }

        /**
         * Puts the searchQueryBox into focus. 
         * Displays virtual keyboard and selects all text in the box.
         * */
        private void Focus_SearchBox(object sender, RoutedEventArgs e)
        {
            SurfaceKeyboard.IsVisible = true;
            if (searchQueryBox.Text == "Enter text")
            {
                searchQueryBox.Foreground = Brushes.Black;
                searchQueryBox.Text = "";
            }
            else
                searchQueryBox.SelectAll();


            goSearch.IsEnabled = true;
            searchQueryBox.Focus();
        }

    }
}
