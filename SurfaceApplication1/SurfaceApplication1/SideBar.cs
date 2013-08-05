using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using Microsoft.Surface.Presentation.Controls;
using System.Xml;
using System.Windows.Documents;

namespace SurfaceApplication1
{
    public class SideBar
    {
        public TabControl tabBar;
        private List<SideBarTab> tabItems;
        private Boolean defaultOptionsChanged;
        private SideBarTab tabAdd;
        private bool optionsShown = false;
        public enum searchLanguage { oldFrench = 1, modernFrench = 2, English = 3 };
        public searchLanguage currentSearchLanguage = searchLanguage.oldFrench;
        public int veryFirstLine, veryLastLine;
        public String pageToFind; // for opening/navigating to a new tab from a search result closeup
        public String previousPageToFind;
        private SurfaceWindow1 SurfaceWindow;

        public SideBar(SurfaceWindow1 SurfaceWindow, TabControl tabBar)
        {
            this.SurfaceWindow = SurfaceWindow;
            this.tabBar = tabBar;
            tabItems = new List<SideBarTab>();

            veryFirstLine = 1;
            veryLastLine = 5986;

            tabAdd = new SideBarTab(this);
            tabAdd.Header = "  +  ";
            tabAdd.FontSize = 25;
            tabAdd.FontFamily = new FontFamily("Cambria");

            Canvas newTabCanvas = new Canvas();
            newTabCanvas.Height = 899;
            newTabCanvas.Width = 550;
            tabAdd.Content = newTabCanvas;


            Button searchButton = new Button();
            searchButton.Style = tabBar.FindResource("RoundButtonTemplate") as Style;
            searchButton.Click += new RoutedEventHandler(SearchButton_Selected);
            searchButton.TouchDown += new EventHandler<TouchEventArgs>(SearchButton_Selected);

            Grid searchGrid = new Grid();

            Image searchIm = new Image();
            searchIm.Source = new BitmapImage(new Uri(@"..\..\icons\magnifyingglass.png", UriKind.Relative));
            searchIm.Style = tabBar.FindResource("ButtonImageTemplate") as Style;

            TextBlock searchText = new TextBlock();
            searchText.Text = "SEARCH";
            searchText.Style = tabBar.FindResource("ButtonTextTemplate") as Style;

            searchGrid.Children.Add(searchIm);
            searchGrid.Children.Add(searchText);
            searchButton.Content = searchGrid;
            Canvas.SetLeft(searchButton, 68.0);
            Canvas.SetTop(searchButton, 350.0);
            newTabCanvas.Children.Add(searchButton);


            Button annotateButton = new Button();
            annotateButton.Style = tabBar.FindResource("RoundButtonTemplate") as Style;
            annotateButton.Click += new RoutedEventHandler(AnnotateButton_Selected);
            annotateButton.TouchDown += new EventHandler<TouchEventArgs>(AnnotateButton_Selected);

            Grid annotateGrid = new Grid();

            Image annotateIm = new Image();
            annotateIm.Source = new BitmapImage(new Uri(@"..\..\icons\pencil.jpg", UriKind.Relative));
            annotateIm.Style = tabBar.FindResource("ButtonImageTemplate") as Style;

            TextBlock annotateText = new TextBlock();
            annotateText.Style = tabBar.FindResource("ButtonTextTemplate") as Style;
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


            Button savedPagesButton = new Button();
            savedPagesButton.Style = tabBar.FindResource("RoundButtonTemplate") as Style;
            savedPagesButton.Click += new RoutedEventHandler(SavedPagesButton_Selected);
            savedPagesButton.TouchDown += new EventHandler<TouchEventArgs>(SavedPagesButton_Selected);

            Image savedPagesIm = new Image();
            savedPagesIm.Source = new BitmapImage(new Uri(@"..\..\icons\save.png", UriKind.Relative));
            savedPagesIm.Style = tabBar.FindResource("ButtonImageTemplate") as Style;

            TextBlock savedPagesText = new TextBlock();
            savedPagesText.Style = tabBar.FindResource("ButtonTextTemplate") as Style;
            savedPagesText.Text = "SAVED";

            Grid savedPagesGrid = new Grid();

            savedPagesGrid.Children.Add(savedPagesIm);
            savedPagesGrid.Children.Add(savedPagesText);
            savedPagesButton.Content = savedPagesGrid;
            Canvas.SetLeft(savedPagesButton, 370.0);
            Canvas.SetTop(savedPagesButton, 480.0);
            newTabCanvas.Children.Add(savedPagesButton);


            Button studyButton = new Button();
            studyButton.Style = tabBar.FindResource("RoundButtonTemplate") as Style;
            studyButton.Click += new RoutedEventHandler(StudyButton_Selected);
            studyButton.TouchDown += new EventHandler<TouchEventArgs>(StudyButton_Selected);

            Grid studyGrid = new Grid();

            Image studyIm = new Image();
            studyIm.Source = new BitmapImage(new Uri(@"..\..\icons\musicnote.png", UriKind.Relative));
            studyIm.Style = tabBar.FindResource("ButtonImageTemplate") as Style;

            TextBlock studyText = new TextBlock();
            studyText.Style = tabBar.FindResource("ButtonTextTemplate") as Style;
            studyText.Text = "STUDY";

            studyGrid.Children.Add(studyIm);
            studyGrid.Children.Add(studyText);
            studyButton.Content = studyGrid;
            Canvas.SetLeft(studyButton, 370.0);
            Canvas.SetTop(studyButton, 350.0);
            newTabCanvas.Children.Add(studyButton);


            tabItems.Add(tabAdd);
            tabBar.DataContext = tabItems;
            tabBar.SelectedIndex = 0;
        }

        private void SearchButton_Selected(object sender, RoutedEventArgs e)
        {
            tabBar.DataContext = null;
            SideBarTab newTab = this.AddSearchTabItem();
            tabBar.DataContext = tabItems;
            tabBar.SelectedItem = newTab;
        }

        private void AnnotateButton_Selected(object sender, RoutedEventArgs e)
        {
            tabBar.DataContext = null;
            SideBarTab newTab = this.AddAnnotateTabItem();
            tabBar.DataContext = tabItems;
            tabBar.SelectedItem = newTab;
        }

        private void StudyButton_Selected(object sender, RoutedEventArgs e)
        {
            tabBar.DataContext = null;
            SideBarTab newTab = this.AddStudyTabItem();
            tabBar.DataContext = tabItems;
            tabBar.SelectedItem = newTab;
        }

        private void SavedPagesButton_Selected(object sender, RoutedEventArgs e)
        {
            tabBar.DataContext = null;
            SideBarTab newTab = this.AddSavedPagesTabItem();
            tabBar.DataContext = tabItems;
            tabBar.SelectedItem = newTab;
        }

        private SideBarTab AddSavedPagesTabItem()
        {
            int count = tabItems.Count;
            SideBarTab tab = new SideBarTab(this);
            tab.Header = "Saved Pages";
            tab.Width = 100;



            // insert tab item right before the last (+) tab item
            tabItems.Insert(count - 1, tab);
            return tab;
        }

        private SideBarTab AddSearchTabItem()
        {
            int count = tabItems.Count;


            SearchTab tab = new SearchTab(this);

            tab.moreOptions.Click += new RoutedEventHandler(Show_Options);
            tab.moreOptions.TouchDown += new EventHandler<TouchEventArgs>(Show_Options);
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



            // insert tab item right before the last (+) tab item
            tabItems.Insert(count - 1, tab);
            return tab;
        }

        private void displaySearchLanguages(object sender, RoutedEventArgs e)
        {
            SearchTab selectedTab = tabBar.SelectedItem as SearchTab;
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
            SearchTab selectedTab = tabBar.SelectedItem as SearchTab;
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
            SearchTab selectedTab = tabBar.SelectedItem as SearchTab;
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
            SearchTab selectedTab = tabBar.SelectedItem as SearchTab;
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
            SearchTab selectedTab = tabBar.SelectedItem as SearchTab;
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
            XmlDocument xml = SurfaceWindow1.xml;
            XmlDocument engXml = SurfaceWindow1.engXml;
            XmlDocument layoutXml = SurfaceWindow1.layoutXml;
            XmlDocument modFrXml = SurfaceWindow1.modFrXml;

            SearchTab selectedTab = tabBar.SelectedItem as SearchTab;
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
                    poetryResults = Translate.searchOldFrPoetry(searchQuery, caseType, wordType,xml, engXml, layoutXml);
                else if (currentSearchLanguage == searchLanguage.modernFrench)
                    poetryResults = Translate.searchModFrPoetry(searchQuery, caseType, wordType, modFrXml, engXml, layoutXml);
                else if (currentSearchLanguage == searchLanguage.English)
                    poetryResults = Translate.searchEngPoetry(searchQuery, caseType, wordType, xml, engXml, layoutXml);

                SurfaceListBox poetryLB = new SurfaceListBox();
                poetryLB.Style = tabBar.FindResource("SearchResultSurfaceListBox") as Style;
                selectedTab.poetryScroll.Content = poetryLB; // NB: the scroll bar comes from the poetryScroll, not poetryLB

                foreach (SearchResult result in poetryResults)
                {
                    ResultBoxItem resultRBI = new ResultBoxItem();
                    convertSearchResultToResultBoxItem(result, resultRBI);
                    resultRBI.resultThumbnail = Translate.convertImage(Thumbnailer.getThumbnail(Translate.getTagByLineNum(result.lineNum, layoutXml)));
                    if (((optionsShown == false) && poetryResults.Count < 4) || ((optionsShown == true) && poetryResults.Count < 2))
                        resultRBI.Width = 480;
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

                if (poetryResults.Count <= 4)
                {

                }

                // Lyric results
                List<SearchResult> lyricResults = new List<SearchResult>();
                if (currentSearchLanguage == searchLanguage.oldFrench)
                    lyricResults = Translate.searchLyrics(searchQuery, caseType, wordType, xml, layoutXml);
                else if (currentSearchLanguage == searchLanguage.modernFrench)
                    lyricResults = Translate.searchLyrics(searchQuery, caseType, wordType, modFrXml, layoutXml);

                ListBox lyricsLB = new ListBox();
                lyricsLB.Style = tabBar.FindResource("SearchResultSurfaceListBox") as Style;
                selectedTab.lyricsScroll.Content = lyricsLB;


                foreach (SearchResult result in lyricResults)
                {
                    ResultBoxItem resultRBI = new ResultBoxItem();
                    convertSearchResultToResultBoxItem(result, resultRBI);
                    resultRBI.resultThumbnail = Translate.convertImage(Thumbnailer.getThumbnail(result.tag));
                    lyricsLB.Items.Add(resultRBI);
                }

                selectedTab.lyricsTab.Header = "Lyrics (" + lyricResults.Count + ")";

                if (lyricResults.Count == 0)
                {
                    TextBlock noResults = new TextBlock();
                    noResults.Text = "Sorry, your search returned no music lyric results.\r\n\r\nNB: Lyrics only exist in original text or Modern French - no English.";
                    selectedTab.lyricsTab.Content = noResults;
                }
                else
                    selectedTab.lyricsTab.Content = selectedTab.lyricsCanvas;



                // Image results
                List<SearchResult> imageResults = Translate.searchPicCaptions(searchQuery, caseType, wordType, xml, layoutXml);
                ListBox imagesLB = new ListBox();
                imagesLB.Style = tabBar.FindResource("SearchResultSurfaceListBox") as Style;
                selectedTab.imagesScroll.Content = imagesLB;

                foreach (SearchResult result in imageResults)
                {
                    ResultBoxItem resultRBI = new ResultBoxItem();
                    convertSearchResultToResultBoxItem(result, resultRBI);
                    resultRBI.miniThumbnail.Source = new BitmapImage(new Uri(@"..\..\minithumbnails\" + result.tag + ".jpg", UriKind.Relative));
                    resultRBI.resultThumbnail = Translate.convertImage(Thumbnailer.getThumbnail(result.tag));
                    imagesLB.Items.Add(resultRBI);
                }

                //if (selectedTab.lyricsScroll.VerticalScrollBarVisibility == ScrollBarVisibility.Visible)
                //    selectedTab.lyricsScroll.Background = Brushes.LightGray;
                selectedTab.imagesTab.Header = "Images (" + imageResults.Count + ")";

                if (imageResults.Count == 0)
                {
                    TextBlock noResults = new TextBlock();
                    noResults.Text = "Sorry, your search returned no image results.\r\n\r\nNB: Image captions exist in Modern French only; no English (yet).";
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
            if (rbi.resultType == 1)
                rbi.lineInfo.Text = Convert.ToString(sr.lineNum);
            rbi.excerpt1 = sr.excerpt1;
            rbi.excerpt2 = sr.excerpt2;
            rbi.excerpt3 = sr.excerpt3;
            rbi.Height = 80; // temp taller than desired, so scrollbar shows
            rbi.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            rbi.Width = 430;
            rbi.Style = tabBar.FindResource("SearchResultSurfaceListBoxItem") as Style; // Not sure if this works..
            rbi.resultText.Text = sr.text1 + "\r\n" + sr.text2;
            rbi.BorderBrush = Brushes.LightGray;
            rbi.BorderThickness = new Thickness(1.0);
            rbi.Selected += new RoutedEventHandler(Result_Closeup);
        }

        private void compressResults()
        {
            SearchTab selectedTab = tabBar.SelectedItem as SearchTab;
            selectedTab.searchResults.Height = 537;
            Canvas.SetTop(selectedTab.searchResults, 320);
            selectedTab.poetryBorder.Height = 245;
            Canvas.SetTop(selectedTab.poetryBorder, 240);
            selectedTab.poetryScroll.Height = 230;
            selectedTab.lyricsBorder.Height = 245;
            Canvas.SetTop(selectedTab.lyricsBorder, 240);
            selectedTab.lyricsScroll.Height = 230;
            selectedTab.imagesBorder.Height = 245;
            Canvas.SetTop(selectedTab.imagesBorder, 240);
            selectedTab.imagesScroll.Height = 230;

        }

        private void expandResults()
        {
            SearchTab selectedTab = tabBar.SelectedItem as SearchTab;
            selectedTab.searchResults.Height = 677;
            Canvas.SetTop(selectedTab.searchResults, 180);
            selectedTab.poetryBorder.Height = 294;
            Canvas.SetTop(selectedTab.poetryBorder, 331);
            selectedTab.poetryScroll.Height = 325;
            selectedTab.lyricsBorder.Height = 294;
            Canvas.SetTop(selectedTab.lyricsBorder, 331);
            selectedTab.lyricsScroll.Height = 325;
            selectedTab.imagesBorder.Height = 294;
            Canvas.SetTop(selectedTab.imagesBorder, 331);
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
            SearchTab selectedTab = tabBar.SelectedItem as SearchTab;
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


        public static String getImageName(String folio, XmlDocument layoutXml)
        {
            String imageName = "";
            XmlNode node = layoutXml.DocumentElement.SelectSingleNode("//surface[@id='" + folio + "']");
            imageName = node.FirstChild.SelectSingleNode("graphic").Attributes["url"].Value;

            return imageName;
        }

        private void goToFolio(object sender, TouchEventArgs e)
        {
            XmlDocument layoutXml = SurfaceWindow1.layoutXml;

            if (pageToFind != previousPageToFind)
            {
                if (pageToFind.StartsWith("Fo"))
                    pageToFind = pageToFind.Substring(2);
                String imageName = getImageName(pageToFind, layoutXml);
                int pageNum = Convert.ToInt32(imageName.Substring(0, imageName.IndexOf(".jpg")));
                if (pageNum % 2 == 1) // If odd, meaning it's a Fo_r, we want to aim for the previous page.
                    pageNum--;

                SurfaceWindow.createTab(pageNum - 10);
                previousPageToFind = pageToFind;
            }
        }


        private Boolean checkForChanges()
        {
            SearchTab selectedTab = tabBar.SelectedItem as SearchTab;
            if (selectedTab.caseSensitive.IsChecked == true | selectedTab.wholeWordOnly.IsChecked == true |
                selectedTab.wholePhraseOnly.IsChecked == false | (selectedTab.selectLanguage.SelectedIndex != 0 && selectedTab.selectLanguage.SelectedIndex != 1))
                defaultOptionsChanged = true;

            else
                defaultOptionsChanged = false;


            return defaultOptionsChanged;
        }

        private void Clear_SearchBox(object sender, RoutedEventArgs e)
        {
            SearchTab selectedTab = tabBar.SelectedItem as SearchTab;
            if (selectedTab.searchQueryBox.Text == "Enter text")
            {
                selectedTab.searchQueryBox.Foreground = Brushes.Black;
                selectedTab.searchQueryBox.Text = "";
            }
            else
                selectedTab.searchQueryBox.SelectAll();

            selectedTab.searchQueryBox.Focus();
        }



        private SideBarTab AddAnnotateTabItem()
        {
            int count = tabItems.Count;

            // create new tab item - eventually replace with AnnotateTab tab = new AnnotateTab();
            // Then, add all listeners here
            SideBarTab tab = new SideBarTab(this);
            tab.HeaderTemplate = tabBar.FindResource("NewAnnotateTab") as DataTemplate; // can be replaced if AnnotateTab object exists

            // insert tab item right before the last (+) tab item
            tabItems.Insert(count - 1, tab);
            return tab;
        }


        private SideBarTab AddStudyTabItem()
        {
            int count = tabItems.Count;

            // create new tab item - eventually replace with MusicTab tab = new MusicTab();
            // Then, add all listeners here
            SideBarTab tab = new SideBarTab(this);
            tab.HeaderTemplate = tabBar.FindResource("NewStudyTab") as DataTemplate; // can be replaced if StudyTab object exists

            // insert tab item right before the last (+) tab item
            tabItems.Insert(count - 1, tab);
            return tab;
        }

        public void deleteTab(object sender, RoutedEventArgs e)
        {
            SideBarTab selectedTab = tabBar.SelectedItem as SideBarTab;
            tabBar.DataContext = null;

            tabItems.Remove(selectedTab);
            tabBar.DataContext = tabItems;
            if (selectedTab == null || selectedTab.Equals(selectedTab))
            {
                selectedTab = tabItems[0];
            }
            tabBar.SelectedItem = selectedTab;
        }
        public void deleteTab(object sender, TouchEventArgs e)
        {
            deleteTab(sender, new RoutedEventArgs());
        }
    }
}
