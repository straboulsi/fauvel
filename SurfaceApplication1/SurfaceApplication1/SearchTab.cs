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
using SSC = Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;

namespace SurfaceApplication1
{

    class SearchTab : TabItem
    {
        public Canvas searchCanvas, optionsCanvas, poetryCanvas, lyricsCanvas, imagesCanvas;
        public Grid tabHeaderGrid;
        public Button deleteTabButton, goSearch;
        ///public Button moreOptions, lessOptions;
        public TextBlock searchPrompt, searchTabHeader, poetryResults;
        public TextBox searchQueryBox;
        public Image tabHeaderImage;
        public Line topLine, bottomLine;
        public CheckBox caseSensitive, wholeWordOnly, wholePhraseOnly;
        public ScaleTransform st;
        public ComboBox selectLanguage;
        public ComboBoxItem oldFrench, modernFrench, english;
        public TabControl searchResults;
        public TabItem poetryTab, lyricsTab, imagesTab;
        public ScrollViewer poetryScroll, lyricsScroll, imagesScroll;
        public StackPanel poetryPanel, lyricsPanel, imagesPanel;
        public Button moreOptionsNew, lessOptionsNew;
        public Image downArrow, upArrow;
        

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
            ///moreOptions = new Button();
            moreOptionsNew = new Button();
            topLine = new Line();

            optionsCanvas = new Canvas();
            ///lessOptions = new Button();
            lessOptionsNew = new Button();
            upArrow = new Image();
            caseSensitive = new CheckBox();
            wholeWordOnly = new CheckBox();
            wholePhraseOnly = new CheckBox();
            st = new ScaleTransform();
            bottomLine = new Line();

            selectLanguage = new ComboBox();
            oldFrench = new ComboBoxItem();
            modernFrench = new ComboBoxItem();
            english = new ComboBoxItem();

            searchResults = new TabControl();
            poetryTab = new TabItem();
            lyricsTab = new TabItem();
            imagesTab = new TabItem();
            poetryCanvas = new Canvas();
            poetryScroll = new ScrollViewer();
            poetryPanel = new StackPanel();
            lyricsCanvas = new Canvas();
            lyricsScroll = new ScrollViewer();
            lyricsPanel = new StackPanel();
            imagesCanvas = new Canvas();
            imagesScroll = new ScrollViewer();
            imagesPanel = new StackPanel();
            poetryResults = new TextBlock();




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

            /**
            moreOptions.Height = 30;
            moreOptions.Width = 30;
            moreOptions.Content = (string)"▼";
            moreOptions.FontSize = 20;
            moreOptions.VerticalContentAlignment = VerticalAlignment.Top;
            Canvas.SetLeft(moreOptions, 40);
            Canvas.SetTop(moreOptions, 143);
            **/
            downArrow = new Image();
            downArrow.Source = new BitmapImage(new Uri(@"/downArrow.png", UriKind.Relative));
            downArrow.Opacity = 0.3;
            downArrow.HorizontalAlignment = HorizontalAlignment.Center;
            TextBlock moreOptText = new TextBlock();
            moreOptText.Text = "More Options";
            Grid optionsGrid = new Grid();
            moreOptionsNew.Content = optionsGrid;
            moreOptionsNew.Width = 100;
            moreOptionsNew.Height = 20;
            optionsGrid.Children.Add(downArrow);
            optionsGrid.Children.Add(moreOptText);
            Canvas.SetLeft(moreOptionsNew, 225);
            Canvas.SetTop(moreOptionsNew, 153);
            
            
            ///topLine.X1 = 72;
            topLine.X1 = 40;
            topLine.Y1 = 163;
            topLine.X2 = 500;
            ///topLine.X2 = 473;
            topLine.Y2 = 163;
            topLine.Stroke = Brushes.Black;
            topLine.StrokeThickness = 2;
            

            /// The objects on the optionsCanvas
            optionsCanvas.Visibility = Visibility.Hidden;
            /**
            lessOptions.Height = 30;
            lessOptions.Width = 30;
            lessOptions.FontSize = 20;
            lessOptions.Content = (string)"▲";
            lessOptions.VerticalContentAlignment = VerticalAlignment.Top;
            Canvas.SetLeft(lessOptions, 40);
            Canvas.SetTop(lessOptions, 281);
            **/

            st.ScaleX = 2;
            st.ScaleY = 2;

            caseSensitive.FontSize = 10;
            caseSensitive.LayoutTransform = st;
            caseSensitive.Content = (string)"Case sensitive";
            Canvas.SetLeft(caseSensitive, 40);
            Canvas.SetTop(caseSensitive, 170);
            caseSensitive.Visibility = System.Windows.Visibility.Visible;

            wholeWordOnly.FontSize = 10;
            wholeWordOnly.LayoutTransform = st;
            wholeWordOnly.Content = (string)"Match whole word only";
            Canvas.SetLeft(wholeWordOnly, 243);
            Canvas.SetTop(wholeWordOnly, 170);

            wholePhraseOnly.FontSize = 10;
            wholePhraseOnly.LayoutTransform = st;
            wholePhraseOnly.Content = (string)"Match whole phrase only";
            Canvas.SetLeft(wholePhraseOnly, 243);
            Canvas.SetTop(wholePhraseOnly, 227);

            selectLanguage.Background = Brushes.White;
            selectLanguage.Text = "Select language";
            selectLanguage.Width = 175;
            selectLanguage.Height = 40;
            selectLanguage.FontSize = 21;
            selectLanguage.HorizontalContentAlignment = HorizontalAlignment.Center;
            selectLanguage.SelectedIndex = 0;
            Canvas.SetLeft(selectLanguage, 34);
            Canvas.SetTop(selectLanguage, 220);

            oldFrench.Content = (string)"Old French";
            modernFrench.Content = (string)"Modern French";
            english.Content = (string)"English";

            selectLanguage.Items.Add(oldFrench);
            selectLanguage.Items.Add(modernFrench);
            selectLanguage.Items.Add(english);

            ///bottomLine.X1 = 72;
            bottomLine.X1 = 40;
            bottomLine.Y1 = 183;
            ///bottomLine.X2 = 473;
            bottomLine.X2 = 500;
            bottomLine.Y2 = 183;
            bottomLine.Stroke = Brushes.Black;
            bottomLine.StrokeThickness = 2;
            Canvas.SetTop(bottomLine, 113);

            upArrow.Source = new BitmapImage(new Uri(@"/upArrow.png", UriKind.Relative));
            upArrow.Opacity = 0.3;
            upArrow.HorizontalAlignment = HorizontalAlignment.Center;
            TextBlock lessOptText = new TextBlock();
            lessOptText.Text = "Less Options";
            Grid lessOptGrid = new Grid();
            lessOptionsNew.Content = lessOptGrid;
            lessOptionsNew.Width = 100;
            lessOptionsNew.Height = 20;
            lessOptGrid.Children.Add(upArrow);
            lessOptGrid.Children.Add(lessOptText);
            Canvas.SetLeft(lessOptionsNew, 225);
            Canvas.SetTop(lessOptionsNew, 285);



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
            ///poetryTab.Content = poetryResults;

            poetryCanvas.Height = 629;
            poetryCanvas.Children.Add(poetryScroll);
            poetryCanvas.Children.Add(poetryPanel);
            poetryScroll.Height = 325;
            poetryScroll.Width = 470;
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
            ///searchCanvas.Children.Add(moreOptions);
            searchCanvas.Children.Add(topLine);
            searchCanvas.Children.Add(moreOptionsNew);
            searchCanvas.Children.Add(deleteTabButton);

            searchCanvas.Children.Add(optionsCanvas);
            optionsCanvas.Children.Add(bottomLine);
            ///optionsCanvas.Children.Add(lessOptions);
            optionsCanvas.Children.Add(lessOptionsNew);
            optionsCanvas.Children.Add(caseSensitive);
            optionsCanvas.Children.Add(wholeWordOnly);
            optionsCanvas.Children.Add(wholePhraseOnly);
            optionsCanvas.Children.Add(selectLanguage);
            

            searchCanvas.Children.Add(searchResults);




        }

    }
}
