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

namespace SurfaceApplication1
{

    class SearchTab : TabItem
    {
        public Canvas searchCanvas, poetryCanvas, lyricsCanvas, imagesCanvas;
        public Grid tabHeaderGrid;
        public Button deleteTabButton, goSearch, selectLanguageButton;
        public TextBlock searchPrompt, searchTabHeader;
        public TextBox searchQueryBox;
        public Image tabHeaderImage;
        public Line topLine, bottomLine;
        public CheckBox caseSensitive, wholeWordOnly, wholePhraseOnly;
        public ScaleTransform st;
        public SurfaceListBox selectLanguage;
        public SurfaceListBoxItem pickLanguage, oldFrench, modernFrench, English;
        public TabControl searchResults;
        public TabItem poetryTab, lyricsTab, imagesTab;
        //public ScrollViewer poetryScroll, lyricsScroll, imagesScroll;
        public StackPanel poetryPanel, lyricsPanel, imagesPanel;
        public Button moreOptions, fewerOptions;
        public Image downArrow, upArrow;
        public SurfaceScrollViewer poetryScroll, lyricsScroll, imagesScroll;

        public SearchTab()
        {
            

            searchCanvas = new Canvas();
            tabHeaderGrid = new Grid();
            Content = searchCanvas;
            Header = tabHeaderGrid;

            deleteTabButton = new Button();
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
            wholePhraseOnly = new CheckBox();
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




            tabHeaderGrid.Width = 100;
            tabHeaderGrid.Height = 40;
            tabHeaderImage = new Image();
            tabHeaderImage.Source = new BitmapImage(new Uri(@"/magnifyingglass.png", UriKind.Relative));
            tabHeaderImage.Opacity = 0.3;
            searchTabHeader.HorizontalAlignment = HorizontalAlignment.Center;
            searchTabHeader.VerticalAlignment = VerticalAlignment.Center;
            searchTabHeader.FontSize = 21;


            deleteTabButton.Content = (string)"X";
            deleteTabButton.FontFamily = new FontFamily("Arial");
            deleteTabButton.FontSize = 35;
            deleteTabButton.Width = 70;
            deleteTabButton.Height = 40;
            deleteTabButton.Opacity = 0.7;
            Canvas.SetLeft(deleteTabButton, 476);
            Canvas.SetTop(deleteTabButton, 1);

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
            goSearch.Content = (string)"Go!";
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

            wholePhraseOnly.FontSize = 10;
            wholePhraseOnly.LayoutTransform = st;
            wholePhraseOnly.Content = (string)"Match whole phrase only";
            wholePhraseOnly.IsChecked = true; // This is default
            Canvas.SetLeft(wholePhraseOnly, 243);
            Canvas.SetTop(wholePhraseOnly, 227);

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

            poetryTab.Header = "Poetry";
            poetryTab.Height = 40;
            poetryTab.Width = 159;
            poetryTab.Content = poetryCanvas;

            poetryCanvas.Height = 629;
            poetryCanvas.Children.Add(poetryScroll);
            poetryCanvas.Children.Add(poetryPanel);

            poetryScroll.Height = 325;
            poetryScroll.Width = 470; 
            poetryScroll.Background = Brushes.LightGray;
            //poetryScroll.Foreground = Brushes.Tomato;
            poetryScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            poetryScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;

            poetryPanel.Orientation = Orientation.Horizontal;
            poetryPanel.Height = 300;
            poetryPanel.Width = 478;
            Canvas.SetTop(poetryPanel, 331);


            lyricsTab.Header = "Lyrics";
            lyricsTab.Height = 40;
            lyricsTab.Width = 159;
            lyricsTab.Content = lyricsCanvas;
            lyricsCanvas.Height = 629;
            lyricsCanvas.Children.Add(lyricsScroll);
            lyricsCanvas.Children.Add(lyricsPanel);
            lyricsScroll.Height = 325;
            lyricsScroll.Width = 470;
            lyricsScroll.Background = Brushes.LightGray;
            lyricsScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            lyricsScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            lyricsPanel.Orientation = Orientation.Horizontal;
            lyricsPanel.Height = 300;
            lyricsPanel.Width = 478;
            Canvas.SetTop(lyricsPanel, 331);

            imagesTab.Header = "Images";
            imagesTab.Height = 40;
            imagesTab.Width = 159;
            imagesTab.Content = imagesCanvas;
            imagesCanvas.Height = 629;
            imagesCanvas.Children.Add(imagesScroll);
            imagesCanvas.Children.Add(imagesPanel);
            imagesScroll.Height = 325;
            imagesScroll.Width = 470;
            imagesScroll.Background = Brushes.LightGray;
            imagesScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            imagesScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            imagesPanel.Orientation = Orientation.Horizontal;
            imagesPanel.Height = 300;
            imagesPanel.Width = 478;
            Canvas.SetTop(imagesPanel, 331);








            /// Adding everything

            tabHeaderGrid.Children.Add(tabHeaderImage);
            tabHeaderGrid.Children.Add(searchTabHeader);

            searchCanvas.Children.Add(searchPrompt);
            searchCanvas.Children.Add(searchQueryBox);
            searchCanvas.Children.Add(goSearch);
            searchCanvas.Children.Add(topLine);
            searchCanvas.Children.Add(moreOptions);
            searchCanvas.Children.Add(deleteTabButton);

            ///searchCanvas.Children.Add(optionsCanvas);
            //optionsCanvas.Children.Add(bottomLine);
            //optionsCanvas.Children.Add(fewerOptions);
            ///optionsCanvas.Children.Add(caseSensitive);
            //optionsCanvas.Children.Add(wholeWordOnly);
            //optionsCanvas.Children.Add(wholePhraseOnly);
            ///optionsCanvas.Children.Add(selectLanguage);

            searchCanvas.Children.Add(searchResults);
            searchCanvas.Children.Add(caseSensitive);
            searchCanvas.Children.Add(bottomLine);
            searchCanvas.Children.Add(selectLanguage);
            searchCanvas.Children.Add(selectLanguageButton);
            searchCanvas.Children.Add(fewerOptions);
            searchCanvas.Children.Add(wholeWordOnly);
            searchCanvas.Children.Add(wholePhraseOnly);
            caseSensitive.Visibility = Visibility.Hidden;
            selectLanguage.Visibility = Visibility.Hidden;
            bottomLine.Visibility = Visibility.Hidden;
            fewerOptions.Visibility = Visibility.Hidden;
            wholeWordOnly.Visibility = Visibility.Hidden;
            wholePhraseOnly.Visibility = Visibility.Hidden;



        }

    }
}
