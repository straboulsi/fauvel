﻿using System;
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

namespace SurfaceApplication1
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
        public Canvas poetryCanvas, lyricsCanvas, imagesCanvas;
        public Button goSearch, selectLanguageButton;
        public TextBlock searchPrompt, searchTabHeader;
        public TextBox searchQueryBox;
        public Line topLine, bottomLine;
        public CheckBox caseSensitive, wholeWordOnly, exactPhraseOnly;
        public ScaleTransform st;
        public SurfaceListBox selectLanguage;
        public SurfaceListBoxItem pickLanguage, oldFrench, modernFrench, English;
        public TabControl searchResults;
        public TabItem poetryTab, lyricsTab, imagesTab;
        public StackPanel poetryPanel, lyricsPanel, imagesPanel;
        public Button moreOptions, fewerOptions;
        public Image downArrow, upArrow;
        public SurfaceScrollViewer poetryScroll, lyricsScroll, imagesScroll;
        public enum searchLanguage { oldFrench = 0, modernFrench = 1, English = 2 };
        public searchLanguage currentSearchLanguage = searchLanguage.oldFrench;
        public Border poetryBorder, imagesBorder, lyricsBorder;
        private bool optionsShown = false;
        public String pageToFind;
        private SurfaceWindow1 surfaceWindow;
        private Boolean defaultOptionsChanged;
        private SideBar sideBar;
        private List<SearchResult> poetryResults, lyricResults, imageResults;
        public delegate void UpdateTextCallback(string message);
        private int unreturnedResults;
        public bool? exactPhr;
        public ResultBoxItem lastCloseupRBI;

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
            poetryScroll = new SurfaceScrollViewer(); //
            poetryPanel = new StackPanel();
            lyricsCanvas = new Canvas();
            lyricsScroll = new SurfaceScrollViewer(); 
            lyricsPanel = new StackPanel();
            imagesCanvas = new Canvas();
            imagesScroll = new SurfaceScrollViewer(); 
            imagesPanel = new StackPanel();

            headerImage.Source = new BitmapImage(new Uri(@"..\..\icons\magnifyingglass.png", UriKind.Relative));
            searchTabHeader.HorizontalAlignment = HorizontalAlignment.Center;
            searchTabHeader.VerticalAlignment = VerticalAlignment.Center;
            searchTabHeader.FontSize = 21;

            searchPrompt.FontSize = 30;
            searchPrompt.Text = "Search:";
            Canvas.SetLeft(searchPrompt, 32);
            Canvas.SetTop(searchPrompt, 26);

            searchQueryBox.Height = 40;
            searchQueryBox.Width = 315;
            searchQueryBox.Foreground = Brushes.Gray;
            searchQueryBox.FontSize = 21;
            searchQueryBox.Text = "Enter text";
            Canvas.SetLeft(searchQueryBox, 40);
            Canvas.SetTop(searchQueryBox, 90);

            goSearch.Height = 40;
            goSearch.Width = 95;
            goSearch.FontSize = 21;
            goSearch.Content = "Go!";
            Canvas.SetLeft(goSearch, 378);
            Canvas.SetTop(goSearch, 90);

           
            downArrow = new Image();
            downArrow.Source = new BitmapImage(new Uri(@"/downArrow.png", UriKind.Relative));
            downArrow.Opacity = 0.3;
            downArrow.HorizontalAlignment = HorizontalAlignment.Center;
            TextBlock moreOptText = new TextBlock();
            moreOptText.Text = "More Options";
            Grid optionsGrid = new Grid();
            moreOptions.Content = optionsGrid;
            moreOptions.Width = 100;
            moreOptions.Height = 20;
            optionsGrid.Children.Add(downArrow);
            optionsGrid.Children.Add(moreOptText);
            Canvas.SetLeft(moreOptions, 225);
            Canvas.SetTop(moreOptions, 153);
            
            
            topLine.X1 = 40;
            topLine.Y1 = 163;
            topLine.X2 = 500;
            topLine.Y2 = 163;
            topLine.Stroke = Brushes.Black;
            topLine.StrokeThickness = 2;
            


            /// The objects for extended search options
            st.ScaleX = 2;
            st.ScaleY = 2;

            caseSensitive.FontSize = 10;
            caseSensitive.LayoutTransform = st;
            caseSensitive.Content = (string)"Case sensitive";
            Canvas.SetLeft(caseSensitive, 40);
            Canvas.SetTop(caseSensitive, 170);
            

            wholeWordOnly.FontSize = 10;
            wholeWordOnly.LayoutTransform = st;
            wholeWordOnly.Content = (string)"Match whole word only";
            Canvas.SetLeft(wholeWordOnly, 243);
            Canvas.SetTop(wholeWordOnly, 170);

            exactPhraseOnly.FontSize = 10;
            exactPhraseOnly.LayoutTransform = st;
            exactPhraseOnly.Content = (string)"Match exact phrase only";
            Canvas.SetLeft(exactPhraseOnly, 243);
            Canvas.SetTop(exactPhraseOnly, 227);

            selectLanguage.Background = Brushes.LightGray;
            selectLanguage.Visibility = Visibility.Collapsed;
            selectLanguage.Width = 175;
            selectLanguage.FontSize = 21;
            selectLanguage.HorizontalContentAlignment = HorizontalAlignment.Center;
            selectLanguage.SelectedIndex = 0;


          

            Canvas.SetLeft(selectLanguage, 34);
            Canvas.SetTop(selectLanguage, 220); 

            Canvas.SetLeft(selectLanguageButton, 34);
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
            bottomLine.X2 = 500;
            bottomLine.Y2 = 183;
            bottomLine.Stroke = Brushes.Black;
            bottomLine.StrokeThickness = 2;
            Canvas.SetTop(bottomLine, 113);

            upArrow.Source = new BitmapImage(new Uri(@"/upArrow.png", UriKind.Relative));
            upArrow.Opacity = 0.3;
            upArrow.HorizontalAlignment = HorizontalAlignment.Center;
            TextBlock lessOptText = new TextBlock();
            lessOptText.Text = "Fewer Options";
            Grid lessOptGrid = new Grid();
            fewerOptions.Content = lessOptGrid;
            fewerOptions.Width = 100;
            fewerOptions.Height = 20;
            lessOptGrid.Children.Add(upArrow);
            lessOptGrid.Children.Add(lessOptText);
            Canvas.SetLeft(fewerOptions, 225);
            Canvas.SetTop(fewerOptions, 285);



            /// The objects on the search results section
            searchResults.Visibility = Visibility.Hidden;
            searchResults.Height = 677;
            searchResults.Width = 482;
            searchResults.FontSize = 21;
            Canvas.SetLeft(searchResults, 30);
            Canvas.SetTop(searchResults, 180);
            searchResults.Items.Add(poetryTab);
            searchResults.Items.Add(lyricsTab);
            searchResults.Items.Add(imagesTab);

            poetryBorder = new Border();
            poetryBorder.BorderBrush = Brushes.DarkGray;
            poetryBorder.BorderThickness = new Thickness(1);
            poetryBorder.Child = poetryPanel;
            poetryBorder.Height = 294;
            poetryBorder.Width = 472;

            poetryTab.Header = "Poetry";
            poetryTab.Height = 40;
            poetryTab.Width = 159;
            poetryTab.Content = poetryCanvas;

            poetryCanvas.Height = 629;
            poetryCanvas.Children.Add(poetryScroll);
            poetryCanvas.Children.Add(poetryBorder);

            poetryScroll.Height = 325;
            poetryScroll.Width = 470;
            //poetryScroll.Background = Brushes.LightGray;
            //poetryScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            poetryScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            poetryScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            poetryScroll.PanningMode = PanningMode.VerticalOnly;
            poetryPanel.Orientation = Orientation.Horizontal;

            Canvas.SetTop(poetryBorder, 331);

            lyricsBorder = new Border();
            lyricsBorder.BorderBrush = Brushes.DarkGray;
            lyricsBorder.BorderThickness = new Thickness(1);
            lyricsBorder.Child = lyricsPanel;
            lyricsBorder.Height = 294;
            lyricsBorder.Width = 472;


            lyricsTab.Header = "Lyrics";
            lyricsTab.Height = 40;
            lyricsTab.Width = 159;
            lyricsTab.Content = lyricsCanvas;
            lyricsCanvas.Height = 629;
            lyricsCanvas.Children.Add(lyricsScroll);
            lyricsCanvas.Children.Add(lyricsBorder);
            lyricsScroll.Height = 325;
            lyricsScroll.Width = 470;
            //lyricsScroll.Background = Brushes.LightGray;
            //lyricsScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            lyricsScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            lyricsScroll.PanningMode = PanningMode.VerticalOnly;
            lyricsPanel.Orientation = Orientation.Horizontal;

            Canvas.SetTop(lyricsBorder, 331);

            imagesBorder = new Border();
            imagesBorder.BorderBrush = Brushes.DarkGray;
            imagesBorder.BorderThickness = new Thickness(1);
            imagesBorder.Child = imagesPanel;
            imagesBorder.Height = 294;
            imagesBorder.Width = 472;

            imagesTab.Header = "Images";
            imagesTab.Height = 40;
            imagesTab.Width = 159;
            imagesTab.Content = imagesCanvas;
            imagesCanvas.Height = 629;
            imagesCanvas.Children.Add(imagesScroll);
            imagesCanvas.Children.Add(imagesBorder); 
            imagesScroll.Height = 325;
            imagesScroll.Width = 470;
            //imagesScroll.Background = Brushes.LightGray;
            imagesScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            //imagesScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            imagesScroll.PanningMode = PanningMode.VerticalOnly;
            imagesPanel.Orientation = Orientation.Horizontal;

            Canvas.SetTop(imagesBorder, 331);

            /// Adding everything

            headerGrid.Children.Add(searchTabHeader);

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

            moreOptions.Click += new RoutedEventHandler(Show_Options);
            moreOptions.TouchDown += new EventHandler<TouchEventArgs>(Show_Options);
            fewerOptions.Click += new RoutedEventHandler(Hide_Options);
            fewerOptions.TouchDown += new EventHandler<TouchEventArgs>(Hide_Options);
            searchQueryBox.GotFocus += new RoutedEventHandler(Clear_SearchBox);
            searchQueryBox.TouchDown += new EventHandler<TouchEventArgs>(Clear_SearchBox);
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
            oldFrench.Selected += new RoutedEventHandler(searchLanguageChanged);
            modernFrench.Selected += new RoutedEventHandler(searchLanguageChanged);
            English.Selected += new RoutedEventHandler(searchLanguageChanged);
        }

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
        }

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
                moreOptions.Background = Brushes.MediumTurquoise;

            else
                moreOptions.ClearValue(Control.BackgroundProperty);

            if (searchResults.IsVisible)
                expandResults();

            optionsShown = false;
        }

        private void changeCheck(object sender, TouchEventArgs e)
        {
            CheckBox thisbox = sender as CheckBox;

            if (thisbox.IsChecked == true)
                thisbox.IsChecked = false;
            else if (thisbox.IsChecked == false)
                thisbox.IsChecked = true;
        }

        private void runSearch()
        {
            String searchQuery = searchQueryBox.Text;

            searchTabHeader.Text = searchQueryBox.Text;
            searchResults.Visibility = Visibility.Visible;
            poetryTab.Content = poetryCanvas;

            int caseType = 0;
            int wordType = 0;
            if (caseSensitive.IsChecked == true)
                caseType = 1;
            if (wholeWordOnly.IsChecked == true)
                wordType = 1;
                
	        exactPhr = exactPhraseOnly.IsChecked;

            if (optionsShown == true)
                compressResults();

            poetryTab.Header = "Poetry (...)";
            lyricsTab.Header = "Lyrics (...)";
            imagesTab.Header = "Images (...)";
            searchQueryBox.IsEnabled = false;
            goSearch.Content = "...";
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
                    
                    SurfaceListBox poetryLB = new SurfaceListBox();
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

                    returnAResult();
                };
                worker.RunWorkerAsync();
            };
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, poetryResultAction);


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
                        noResults.Text = "Sorry, your search returned no music lyric results.\r\n\r\nNB: Lyrics only exist in original text or Modern French - no English.";
                        lyricsTab.Content = noResults;
                    }
                    else
                        lyricsTab.Content = lyricsCanvas;

                    lyricsScroll.Background = Brushes.LightGray;

                    returnAResult();
                };
                worker.RunWorkerAsync();
            };
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, lyricResultAction);
            

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
                    
                    returnAResult();
                };
                worker.RunWorkerAsync();
            };
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, imageResultAction);
            
        }

        private void returnAResult()
        {
            unreturnedResults--;
            if (unreturnedResults == 0)
            {
                searchQueryBox.IsEnabled = true;
                goSearch.Content = "Go!";
                goSearch.IsEnabled = true;

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

        private void newSearch(object sender, RoutedEventArgs e)
        {
            String searchQuery = searchQueryBox.Text;

            if (searchQuery == "Enter text" || searchQuery == "")
                MessageBox.Show(string.Format("Please enter words to search for!"),
                    "Search", MessageBoxButton.OK);

            else
                runSearch();

        }

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
            rbi.Width = 430;
            rbi.Style = sideBar.tabBar.FindResource("SearchResultSurfaceListBoxItem") as Style; // Not sure if this works..
            rbi.resultText.Text = sr.text1 + "\r\n" + sr.text2;
            rbi.BorderBrush = Brushes.LightGray;
            rbi.BorderThickness = new Thickness(1.0);
            rbi.Selected += new RoutedEventHandler(Result_Closeup);
        }

        private void compressResults()
        {
            searchResults.Height = 537;
            Canvas.SetTop(searchResults, 320);
            poetryBorder.Height = 245;
            Canvas.SetTop(poetryBorder, 240);
            poetryScroll.Height = 230;
            lyricsBorder.Height = 245;
            Canvas.SetTop(lyricsBorder, 240);
            lyricsScroll.Height = 230;
            imagesBorder.Height = 245;
            Canvas.SetTop(imagesBorder, 240);
            imagesScroll.Height = 230;
        }

        private void expandResults()
        {
            searchResults.Height = 677;
            Canvas.SetTop(searchResults, 180);
            poetryBorder.Height = 294;
            Canvas.SetTop(poetryBorder, 331);
            poetryScroll.Height = 325;
            lyricsBorder.Height = 294;
            Canvas.SetTop(lyricsBorder, 331);
            lyricsScroll.Height = 325;
            imagesBorder.Height = 294;
            Canvas.SetTop(imagesBorder, 331);
            imagesScroll.Height = 325;
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
            }

            else if (selectedResult.resultType == 2)
            {
                lyricsPanel.Children.Clear();
                lyricsPanel.Children.Add(closeupImage);
                lyricsPanel.Children.Add(closeupText);
                lyricsPanel.TouchDown += new EventHandler<TouchEventArgs>(goToFolio);
            }

            else if (selectedResult.resultType == 3)
            {
                imagesPanel.Children.Clear();
                imagesPanel.Children.Add(closeupImage);
                imagesPanel.Children.Add(closeupText);
                imagesPanel.TouchDown += new EventHandler<TouchEventArgs>(goToFolio);
            }

            pageToFind = selectedResult.folioInfo.Text;

            foreach (SpecialString ss in selectedResult.excerpts)
            {
                if (ss.isStyled == 1)
                    closeupText.Inlines.Add(new Run { FontFamily = new FontFamily("Cambria"), Text = ss.str, FontWeight = FontWeights.Bold });
                else
                    closeupText.Inlines.Add(new Run { FontFamily = new FontFamily("Cambria"), Text = ss.str, FontWeight = FontWeights.Normal });
            }
        }


        public static String getImageName(String folio, XmlDocument layoutXml)
        {
            String imageName = "";
            XmlNode node = layoutXml.DocumentElement.SelectSingleNode("//surface[@id='" + folio + "']");
            imageName = node.FirstChild.SelectSingleNode("graphic").Attributes["url"].Value;

            return imageName;
        }


        /**
         * Navigates from a search result closeup to a new tab open to that result's page.
         * Refers to the last ResultBoxItem selected for closeup.
         * */
        private void goToFolio(object sender, TouchEventArgs e)
        {
            String folioStr = lastCloseupRBI.folioInfo.Text;
            if (folioStr.StartsWith("Fo"))
                folioStr = folioStr.Substring(2);
            String imageName = getImageName(folioStr, SurfaceWindow1.layoutXml);
            int pageNum = Convert.ToInt32(imageName.Substring(0, imageName.IndexOf(".jpg")));
            if (pageNum % 2 == 1) // If odd, meaning it's a Fo_r, we want to aim for the previous page.
                pageNum--;

            editCoordinates(lastCloseupRBI); // Adds width of a page to coordinates from a recto (right side) of an opening

            // Get coordinates from lastCloseupRBI.topL and lastCloseupRBI.bottomR

            surfaceWindow.createTab(pageNum - 10);
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

        private void Clear_SearchBox(object sender, RoutedEventArgs e)
        {
            if (searchQueryBox.Text == "Enter text")
            {
                searchQueryBox.Foreground = Brushes.Black;
                searchQueryBox.Text = "";
            }
            else
                searchQueryBox.SelectAll();

            searchQueryBox.Focus();
        }

    }
}
