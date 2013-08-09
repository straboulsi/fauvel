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

namespace SurfaceApplication1
{

    /**
     * This class defines the methods used in the "Study" (Music) app from the SideBar.
     * It focuses on the front-end visual aspects, while the back-end code is in Study.cs.
     * Primary Coder: Jamie Chong
     * */
    class StudyTab : SideBarTab
    {
        public Button mono, poly, playpause_1, stop_1, playpause_2, stop_2;
        public Canvas notesCanvas, notesTabCanvas;
        public Image musicImg, musicImg1, musicImg2, musicImg3;
        public MediaPlayer play_2rCon1, play_2vMo2_v1, play_2vMo2_tenor;
        public TabControl display;
        public TabItem notesTab, mod_frenchTab, engTab;
        public TextBlock studyTabHeader, studyPrompt, musicTitle, mod_frenchText, engText;
        public SurfaceScrollViewer noteScroll;



        public StudyTab(SideBar mySideBar, SurfaceWindow1 surfaceWindow) : base(mySideBar)
        {
            studyPrompt = new TextBlock();
            studyTabHeader = new TextBlock();

            headerImage.Source = new BitmapImage(new Uri(@"..\..\icons\musicnote.png", UriKind.Relative));
            studyTabHeader.HorizontalAlignment = HorizontalAlignment.Center;
            studyTabHeader.VerticalAlignment = VerticalAlignment.Center;
            studyTabHeader.FontSize = 21;

            studyPrompt.FontSize = 30;
            studyPrompt.Text = "Please select a piece of music.";
            //studyPrompt.Height = 40;
            //studyPrompt.Width = 500;
            Canvas.SetLeft(studyPrompt, 32);
            Canvas.SetTop(studyPrompt, 45);

            mono = new Button();
            poly = new Button();

            mono.Height = 50;
            mono.Width = 200;
            mono.Content = "study 2rCon1";
            mono.FontSize = 20;
            Canvas.SetLeft(mono, 100);
            Canvas.SetTop(mono, 100);

            poly.Height = 50;
            poly.Width = 200;
            poly.Content = "study 2vMo2";
            poly.FontSize = 20;
            Canvas.SetLeft(poly, 100);
            Canvas.SetTop(poly, 300);

            // Add stuff.

            headerGrid.Children.Add(studyTabHeader);

            canvas.Children.Add(studyPrompt);
            canvas.Children.Add(mono);
            canvas.Children.Add(poly);

            // Add click handlers.

            mono.Click += new RoutedEventHandler(study_Mono);
            mono.TouchDown += new EventHandler<TouchEventArgs>(study_Mono);

            poly.Click += new RoutedEventHandler(study_Poly);
        }

        private void study_Mono(object sender, RoutedEventArgs e)
        {
            canvas.Children.Remove(studyPrompt);
            canvas.Children.Remove(mono);
            canvas.Children.Remove(poly);

            musicTitle = new TextBlock();
            Canvas.SetLeft(musicTitle, 32);
            Canvas.SetTop(musicTitle, 45);
            //musicTitle.Height = 40;
            //musicTitle.Width = 500;
            musicTitle.Text = "Conductus : Heu ! Quo progreditur (PM 6)";
            musicTitle.FontSize = 30;
            studyTabHeader.Text = "2rCon1";

            playpause_1 = new Button();
            stop_1 = new Button();
            Canvas.SetLeft(playpause_1, 32);
            Canvas.SetTop(playpause_1, 90);
            Canvas.SetLeft(stop_1, 82);
            Canvas.SetTop(stop_1, 90);
            playpause_1.Height = 50;
            playpause_1.Width = 50;
            stop_1.Height = 50;
            stop_1.Width = 50;
            playpause_1.Content = "►";
            stop_1.Content = "■";
            playpause_1.FontSize = 30;
            stop_1.FontSize = 30;

            display = new TabControl();
            Canvas.SetLeft(display, 32);
            Canvas.SetTop(display, 150);
            display.Height = 860;
            display.Width = 560;

            notesTab = new TabItem();
            notesTab.Header = "Modern Notation";
            notesTab.Height = 50;
            notesTab.Width = 200;
            notesTab.FontSize = 20;

            notesTabCanvas = new Canvas();
            notesTab.Content = notesTabCanvas;
            noteScroll = new SurfaceScrollViewer();
            notesTabCanvas.Children.Add(noteScroll);
            noteScroll.Height = 800;
            noteScroll.Width = 560;
            noteScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            noteScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            noteScroll.PanningMode = PanningMode.VerticalOnly;

            musicImg = new Image();
            musicImg.Source = new BitmapImage(new Uri(@"..\..\musicz\2rCo1.png", UriKind.Relative));
            musicImg.Width = 560;

            notesCanvas = new Canvas();
            noteScroll.Content = notesCanvas;
            notesCanvas.Children.Add(musicImg);


            mod_frenchTab = new TabItem();
            mod_frenchTab.Header = "Modern French";
            mod_frenchTab.Height = 50;
            mod_frenchTab.Width = 175;
            mod_frenchTab.FontSize = 20;

            // Fetch the modern french lyrics from Alison's handy method.
            mod_frenchText = new TextBlock();
            mod_frenchTab.Content = mod_frenchText;
            mod_frenchText.Text = Search.getByTag("2rCon1_t", SurfaceWindow1.modFrXml);

            engTab = new TabItem();
            engTab.Header = "English";
            engTab.Height = 50;
            engTab.Width = 175;
            engTab.FontSize = 20;
            
            // This one is hardcoded in because we don't have any English lyrics in an XML file yet.
            engText = new TextBlock();
            engTab.Content = engText;
            engText.Text = "Oh, how far transgression\nis spreading!\nVirtue is dislodged\nfrom the sanctuary.\nNow Christ is dragged\nto a new tribunal,\nwith Peter using\nthe sword of Pilate.\nRelying on the counsel\nof Fauvel,\none comes to grief;\nthe celestial legion\njustly complains.\nTherefore it begs\nthe Father and the Son\nthat for a remedy\nfor all this\nimmediately\nthe fostering Spirit provide.";

            display.Items.Add(notesTab);
            display.Items.Add(mod_frenchTab);
            display.Items.Add(engTab);

            // Create mediaplayer for audio playback.
            play_2rCon1 = new MediaPlayer();
            play_2rCon1.Open(new Uri(@"..\..\musicz\2rCo1.wma", UriKind.Relative));
            play_2rCon1.MediaEnded += new EventHandler(play_2rCon1_MediaEnded);

            // Add to this canvas.
            canvas.Children.Add(musicTitle);
            canvas.Children.Add(playpause_1);
            canvas.Children.Add(stop_1);
            canvas.Children.Add(display);

            // Add listeners.
            playpause_1.Click += new RoutedEventHandler(playpause_1_Click);
            stop_1.Click += new RoutedEventHandler(stop_1_Click);
        }

        
        void playpause_1_Click(object sender, RoutedEventArgs e)
        {
            if ((string)playpause_1.Content == "ll")
            {
                playpause_1.Content = "►";

                if (play_2rCon1.CanPause)
                    play_2rCon1.Pause();
            }
            else if ((string)playpause_1.Content == "►")
            {
                playpause_1.Content = "ll";

                play_2rCon1.Play();
            }
        }

        void stop_1_Click(object sender, RoutedEventArgs e)
        {
            playpause_1.Content = "►";
            play_2rCon1.Stop();
        }

        void play_2rCon1_MediaEnded(object sender, EventArgs e)
        {
            playpause_1.Content = "►";
            play_2rCon1.Stop();
        }


        void study_Poly(object sender, RoutedEventArgs e)
        {
            canvas.Children.Remove(studyPrompt);
            canvas.Children.Remove(mono);
            canvas.Children.Remove(poly);

            musicTitle = new TextBlock();
            Canvas.SetLeft(musicTitle, 32);
            Canvas.SetTop(musicTitle, 45);
            //musicTitle.Height = 40;
            //musicTitle.Width = 500;
            musicTitle.Text = "Ad solitum vomitum";
            musicTitle.FontSize = 30;
            studyTabHeader.Text = "2vMo2";

            playpause_2 = new Button();
            stop_2 = new Button();
            Canvas.SetLeft(playpause_2, 32);
            Canvas.SetTop(playpause_2, 90);
            Canvas.SetLeft(stop_2, 82);
            Canvas.SetTop(stop_2, 90);
            playpause_2.Height = 50;
            playpause_2.Width = 50;
            stop_2.Height = 50;
            stop_2.Width = 50;
            playpause_2.Content = "►";
            stop_2.Content = "■";
            playpause_2.FontSize = 30;
            stop_2.FontSize = 30;

            display = new TabControl();
            Canvas.SetLeft(display, 32);
            Canvas.SetTop(display, 150);
            display.Height = 860;
            display.Width = 560;

            notesTab = new TabItem();
            notesTab.Header = "Modern Notation";
            notesTab.Height = 50;
            notesTab.Width = 200;
            notesTab.FontSize = 20;

            notesTabCanvas = new Canvas();
            notesTab.Content = notesTabCanvas;
            noteScroll = new SurfaceScrollViewer();
            notesTabCanvas.Children.Add(noteScroll);
            noteScroll.Height = 800;
            noteScroll.Width = 560;
            noteScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            noteScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            noteScroll.PanningMode = PanningMode.VerticalOnly;

            musicImg1 = new Image();
            musicImg1.Source = new BitmapImage(new Uri(@"..\..\musicz\2vMo2-1.png", UriKind.Relative));
            musicImg1.Width = 560;

            notesCanvas = new Canvas();
            noteScroll.Content = notesCanvas;
            notesCanvas.Children.Add(musicImg1);

            mod_frenchTab = new TabItem();
            mod_frenchTab.Header = "Modern French";
            mod_frenchTab.Height = 50;
            mod_frenchTab.Width = 175;
            mod_frenchTab.FontSize = 20;

            // Fetch the modern french lyrics from Alison's handy method.
            mod_frenchText = new TextBlock();
            mod_frenchTab.Content = mod_frenchText;
            mod_frenchText.Text = Search.getByTag("2vMo2_t", SurfaceWindow1.modFrXml);

            engTab = new TabItem();
            engTab.Header = "English";
            engTab.Height = 50;
            engTab.Width = 175;
            engTab.FontSize = 20;

            // This one is hardcoded in because we don't have any English lyrics in an XML file yet.
            engText = new TextBlock();
            engTab.Content = engText;
            engText.Text = "No English translated lyrics for this piece YET!";
            display.Items.Add(notesTab);
            display.Items.Add(mod_frenchTab);
            display.Items.Add(engTab);

            // Create mediaplayers for audio playback.
            play_2vMo2_v1 = new MediaPlayer();
            play_2vMo2_tenor = new MediaPlayer();
            play_2vMo2_v1.Open(new Uri(@"..\..\musicz\2vMo2_v1.wma", UriKind.Relative));
            play_2vMo2_tenor.Open(new Uri(@"..\..\musicz\2vMo2_tenor.wma", UriKind.Relative));
            play_2vMo2_tenor.MediaEnded += new EventHandler(play_2vMo2_tenor_MediaEnded);

            // Add to this canvas.
            canvas.Children.Add(musicTitle);
            canvas.Children.Add(playpause_2);
            canvas.Children.Add(stop_2);
            canvas.Children.Add(display);

            // Add listeners.
            playpause_2.Click += new RoutedEventHandler(playpause_2_Click);
            stop_2.Click += new RoutedEventHandler(stop_2_Click);
        }

        void playpause_2_Click(object sender, RoutedEventArgs e)
        {
            if ((string)playpause_2.Content == "ll")
            {
                playpause_2.Content = "►";

                play_2vMo2_v1.Pause();
                play_2vMo2_tenor.Pause();
            }
            else if ((string)playpause_2.Content == "►")
            {
                playpause_2.Content = "ll";

                play_2vMo2_v1.Play();
                play_2vMo2_tenor.Play();
            }
        }

        void stop_2_Click(object sender, RoutedEventArgs e)
        {
            playpause_2.Content = "►";
            play_2vMo2_v1.Stop();
            play_2vMo2_tenor.Stop();
        }

        void play_2vMo2_tenor_MediaEnded(object sender, EventArgs e)
        {
            playpause_2.Content = "►";
            play_2vMo2_v1.Stop();
            play_2vMo2_tenor.Stop();
        }
    }
}
