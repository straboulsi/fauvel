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
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;


namespace SearchSidebar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<TabItem> SidebarTabItems;
        private TabItem tabAdd;
        private Boolean defaultOptionsChanged;
        public XmlDocument xml, engXml, layoutXml;
        public int veryFirstLine, veryLastLine;
        ///public Dictionary<String, Image> tableOfContents;

        public MainWindow()
        {

            try
            {
                InitializeComponent();

                veryFirstLine = 1;
                veryLastLine = 5986;


                // initialize tabItem array
                SidebarTabItems = new List<TabItem>();

                // add a tabItem with + in header 
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



                xml = new XmlDocument();
                xml.Load("XMLFinalContentFile.xml");
                engXml = new XmlDocument();
                engXml.Load("EnglishXML.xml");
                layoutXml = new XmlDocument();
                layoutXml.Load("layout_full.xml");


                Thumbnailer.getRect("1rIm1", layoutXml);
                ///Class1.getPoint("1rIm1", 1, layoutXml);
                ///getImage("Fo1r", layoutXml);

                SidebarTabItems.Add(tabAdd);
                tabDynamic.DataContext = SidebarTabItems;
                tabDynamic.SelectedIndex = 0;


                

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

        }

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

            
            

            // create new tab item
            SearchTab tab = new SearchTab();
            tab.Name = string.Format("tab{0}", count);
            
            tab.moreOptions.Click += new RoutedEventHandler(Show_Options);
            tab.deleteTabButton.Click += new RoutedEventHandler(btnDelete_Click);
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
            SearchTab selectedTab = tabDynamic.SelectedItem as SearchTab;
            String searchQuery = selectedTab.searchQueryBox.Text;

            if(searchQuery == "Enter text" || searchQuery == "")
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
                    poetryResults = Class1.searchFrPoetry(searchQuery, caseType, wordType, xml, engXml);
                else if (selectedTab.selectLanguage.SelectedIndex == 2)
                    poetryResults = Class1.searchEngPoetry(searchQuery, caseType, wordType, xml, engXml);

                ListBox poetryLB = new ListBox();
                poetryLB.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
                selectedTab.poetryScroll.Content = poetryLB;

                foreach (SearchResult result in poetryResults)
                {
                    ResultBoxItem resultRBI = new ResultBoxItem();
                    resultRBI.folioInfo.Text = result.folio;
                    resultRBI.lineInfo.Text = Convert.ToString(result.lineNum);
                    resultRBI.resultType = result.resultType;
                    ///resultRBI.resultThumbnail = result.thumbnail; /// Add this in once we have a method to get thumbnails
                    resultRBI.resultThumbnail.Source = new BitmapImage(new Uri(@"/poetry.jpg", UriKind.Relative));
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



                // Lyric results
                List<SearchResult> lyricResults = Class1.searchLyrics(searchQuery, caseType, wordType, xml);
                ListBox lyricsLB = new ListBox();
                lyricsLB.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
                selectedTab.lyricsScroll.Content = lyricsLB;

                foreach (SearchResult result in lyricResults)
                {
                    ResultBoxItem resultRBI = new ResultBoxItem();
                    resultRBI.folioInfo.Text = result.folio;
                    resultRBI.resultType = result.resultType;
                    ///resultRBI.resultThumbnail = result.thumbnail; /// Add this in once we have a method to get thumbnails
                    resultRBI.resultThumbnail.Source = new BitmapImage(new Uri(@"/music.jpg", UriKind.Relative));
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



                // Image results
                List<SearchResult> imagesResults = Class1.searchPicCaptions(searchQuery, caseType, wordType, xml);
                ListBox imagesLB = new ListBox();
                imagesLB.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
                selectedTab.imagesScroll.Content = imagesLB;

                foreach (SearchResult result in imagesResults)
                {
                    ResultBoxItem resultRBI = new ResultBoxItem();
                    resultRBI.folioInfo.Text = result.folio;
                    resultRBI.resultType = result.resultType;
                    ///resultRBI.resultThumbnail = result.thumbnail; /// Add this in once we have a method to get thumbnails
                    resultRBI.resultThumbnail.Source = new BitmapImage(new Uri(@"/Medieval.jpg", UriKind.Relative));
                    resultRBI.excerpt = result.excerpt;
                    resultRBI.resultText.Text = result.text1 +"\r\n" + result.text2;
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



                if (selectedTab.optionsCanvas.IsVisible)
                    compressResults();

            }
            
        }

        // Assuming input of Fo1r, Fo1v, etc.
        // Remember other types: Fo23v, Fo28br, Fo28tv
        private Image getImage(String folio, XmlDocument layoutXml)
        {
            /** Recall: Fo1r is 13.jpg, Fo1v is 14.jpg
             * If r, multiply by 2 and add 11
             * If v, multiply by 2 and add 12
             **/
            folio = folio.Substring(2);
            Image image = new Image();
            try
            {
                XmlNode node = layoutXml.DocumentElement.SelectSingleNode("//surface[@id='" + folio + "']");
                ///Console.Write(node.FirstChild.InnerXml);
                String imageName = node.FirstChild.SelectSingleNode("graphic").Attributes["url"].Value;
                ///Console.Write(imageName);

                image.Source = new BitmapImage(new Uri(@"/" + imageName, UriKind.Relative));

                Console.Write(image.Source);
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }
            
            return image;
        }



        private void compressResults()
        {
            SearchTab selectedTab = tabDynamic.SelectedItem as SearchTab;
            selectedTab.searchResults.Height = 563;
            Canvas.SetTop(selectedTab.searchResults, 320);

        }

        private void expandResults()
        {
            SearchTab selectedTab = tabDynamic.SelectedItem as SearchTab;
            selectedTab.searchResults.Height = 677;
            Canvas.SetTop(selectedTab.searchResults, 180);
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
                if (tab.Header.Equals("+")&&SidebarTabItems.Count > 2)
                {
                    
                }
                else
                {

                }
            }
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e)
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
