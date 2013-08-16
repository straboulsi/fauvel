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
using System.Windows.Controls.Primitives;

namespace DigitalFauvel
{

    /**
     * This class defines the methods used in the "Study" (Music) app from the SideBar.
     * It focuses on the front-end visual aspects, while the back-end code is in Study.cs.
     * 
     * NB: This class is a work in progress. 
     * We do not yet have score or audio files for all music objects in Fauvel.
     * Eventually, we plan to implement selecting the piece directly from the folio.
     * Tapping a piece of music will set the selectedTag method and call open_Music.
     * The two openDefault____ methods will be removed.
     * However, for now, we provide two default options.
     * 
     * Primary Coders: Jamie Chong & Alison Y. Chang
     * */
    class StudyTab : SideBarTab
    {

        public Button mono, poly, playpause, stop, selectAudioButton; 
        public Canvas notesCanvas, notesTabCanvas;
        public Grid mo_lb1, mo_lb2, mo_lb3;
        public Image musicImg, musicImg1, musicImg2, musicImg3, v1_Img, tenor_Img;
        public List<ListBoxItem> audioOptions;
        public ListBox selectAudioListBox;
        public ListBoxItem v1_name_lb, v1_mute_lb, v1_solo_lb, tenor_name_lb, tenor_mute_lb, tenor_solo_lb, space1, space2, 
            selectAudio, MIDI, liveRecording, lastAudioChoice;
        public MediaPlayer play_2rCon1, play_2vMo2_v1, play_2vMo2_tenor;
        public MusicExpander fullExp;
        public MusicPartExpander v1Exp, tenorExp;
        private SideBar mySideBar;
        public StackPanel motetParts, motetScore, v1_buttons, tenor_buttons;
        public String selectedTag;
        public SurfaceScrollViewer noteScroll;
        public TabControl display;
        public TabItem notesTab, mod_frenchTab, engTab;
        public TextBlock studyTabHeader, studyPrompt, mod_frenchText, engText, musicTitle;
        public ToggleButton v1_mute, v1_solo, tenor_mute, tenor_solo;



        public StudyTab(SideBar mySideBar, SurfaceWindow1 surfaceWindow) : base(mySideBar)
        {
            this.mySideBar = mySideBar;

            // Opening page.
            studyPrompt = new TextBlock();
            studyTabHeader = new TextBlock();

            headerImage.Source = new BitmapImage(new Uri(@"..\..\icons\musicnote.png", UriKind.Relative));
            studyTabHeader.HorizontalAlignment = HorizontalAlignment.Center;
            studyTabHeader.VerticalAlignment = VerticalAlignment.Center;
            studyTabHeader.FontSize = 21;

            studyPrompt.FontSize = 30;
            studyPrompt.Text = "Please select a piece of music.";
            Canvas.SetLeft(studyPrompt, 32);
            Canvas.SetTop(studyPrompt, 45);



            // Eventually, we plan to implement selecting the piece directly from the folio.
            // Tapping a piece of music will set the selectedTag method and call open_Music.
            // The two openDefault____ methods will be removed.
            // However, for now, we provide two default options.
            mono = new Button();
            poly = new Button();

            mono.Height = 50;
            mono.Width = 200;
            mono.Content = "study 2rCon1";
            mono.FontSize = 20;
            Canvas.SetLeft(mono, 100);
            Canvas.SetTop(mono, 200);

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
            mono.Click += new RoutedEventHandler(openDefaultMono);
            mono.TouchDown += new EventHandler<TouchEventArgs>(openDefaultMono);
            poly.Click += new RoutedEventHandler(openDefaultPoly);
            poly.TouchDown += new EventHandler<TouchEventArgs>(openDefaultPoly);
        }

        // Temporary method - replace w actual selection of music from folios
        private void openDefaultMono(object sender, RoutedEventArgs e)
        {
            selectedTag = "2rCon1";
            open_Music(sender, e);
        }

        // Temporary method - replace w actual selection of music from folios
        private void openDefaultPoly(object sender, RoutedEventArgs e)
        {
            selectedTag = "2vMo2";
            open_Music(sender, e);
        }


        // Determines whether a piece of music is monophonic or polyphonic
        // Opens a new music tab for mono or poly studying
        private void open_Music(object sender, RoutedEventArgs e)
        {
            //selectedTag = ""; // Set this by tapping on a piece of music

            if (Study.hasMultipleVoices(selectedTag) == false)
                study_Mono(sender, e);
            else if(Study.hasMultipleVoices(selectedTag) == true)
                study_Poly(sender, e);
        }


        private void musicControls(String tag)
        {
            canvas.Children.Remove(studyPrompt);
            canvas.Children.Remove(mono);
            canvas.Children.Remove(poly);


            // Title of the piece.
            musicTitle = new TextBlock();
            Canvas.SetLeft(musicTitle, 15);
            Canvas.SetTop(musicTitle, 45);
            musicTitle.Text = Study.getTitle(tag); //
            musicTitle.FontSize = 30;
            studyTabHeader.Text = tag; //

            // Play/Pause and Stop buttons.
            playpause = new Button();
            stop = new Button();
            Canvas.SetLeft(playpause, 15);
            Canvas.SetTop(playpause, 90);
            Canvas.SetLeft(stop, 65);
            Canvas.SetTop(stop, 90);
            playpause.Height = 50;
            playpause.Width = 50;
            stop.Height = 50;
            stop.Width = 50;
            playpause.Content = "►";
            stop.Content = "■";
            playpause.FontSize = 35;
            stop.FontSize = 30;

            // ListBox for audio options
            selectAudioButton = new Button();
            selectAudioListBox = new ListBox();
            selectAudio = new ListBoxItem();
            MIDI = new ListBoxItem();
            liveRecording = new ListBoxItem();
            audioOptions = new List<ListBoxItem>();

            selectAudio.Content = (String)"Select audio:";
            MIDI.Content = (String)"MIDI";
            liveRecording.Content = (String)"Live recording";

            audioOptions.Add(selectAudio);
            audioOptions.Add(MIDI);
            audioOptions.Add(liveRecording);

            foreach (ListBoxItem lbi in audioOptions)
            {
                lbi.Height = 50;
                lbi.FontSize = 25;
                selectAudioListBox.Items.Add(lbi);
                lbi.HorizontalAlignment = HorizontalAlignment.Center;
                lbi.Selected += new RoutedEventHandler(audioOptionSelected);
                lbi.TouchDown += new EventHandler<TouchEventArgs>(audioOptionSelected);
            }

            selectAudioButton.Width = 180;
            selectAudioButton.Height = 50;
            selectAudioButton.Content = (String)"Select audio:";
            selectAudioButton.FontSize = 25;
            selectAudioListBox.Width = 180;
            Canvas.SetTop(selectAudioListBox, 90);
            Canvas.SetLeft(selectAudioListBox, 140);
            Canvas.SetTop(selectAudioButton, 90);
            Canvas.SetLeft(selectAudioButton, 140);

            selectAudioButton.Visibility = Visibility.Visible;
            selectAudioListBox.Visibility = Visibility.Hidden;
            selectAudioButton.Click += new RoutedEventHandler(showAudioOptions);
            selectAudioButton.TouchDown += new EventHandler<TouchEventArgs>(showAudioOptions);

            

            display = new TabControl();
            Canvas.SetLeft(display, 15);
            Canvas.SetTop(display, 150);
            display.Height = 860;
            display.Width = 580;


            notesTab = new TabItem();
            notesTab.Header = "Modern Notation";
            notesTab.Height = 50;
            notesTab.Width = 200;
            notesTab.FontSize = 20;



            mod_frenchTab = new TabItem(); 
            mod_frenchText = new TextBlock();
            mod_frenchTab.Header = "Modern French";
            mod_frenchTab.Height = 50;
            mod_frenchTab.Width = 175;
            mod_frenchTab.FontSize = 20;
            mod_frenchTab.Content = mod_frenchText;

            engTab = new TabItem();
            engText = new TextBlock();
            engTab.Header = "English";
            engTab.Height = 50;
            engTab.Width = 175;
            engTab.FontSize = 20;
            engTab.Content = engText;

            noteScroll = new SurfaceScrollViewer();
            notesTabCanvas = new Canvas();
            notesCanvas = new Canvas();

            noteScroll.Height = 860;
            noteScroll.Width = 580;
            noteScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            noteScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            noteScroll.PanningMode = PanningMode.VerticalOnly;
            noteScroll.Content = notesCanvas;



            notesTab.Content = notesTabCanvas;
            notesTabCanvas.Children.Add(noteScroll);


            canvas.Children.Add(musicTitle);
            canvas.Children.Add(playpause);
            canvas.Children.Add(stop);
            canvas.Children.Add(display);
            canvas.Children.Add(selectAudioButton);
            canvas.Children.Add(selectAudioListBox);

            display.Items.Add(notesTab);
            display.Items.Add(mod_frenchTab);
            display.Items.Add(engTab);
        }


        // Monophonic music study page (other monophonic pieces should follow this format).
        private void study_Mono(object sender, RoutedEventArgs e)
        {
            musicControls("2rCon1");

            playpause.Click += new RoutedEventHandler(playpause_1_Click);
            stop.Click += new RoutedEventHandler(stop_1_Click);
            mod_frenchText.Text = Search.getByTag("2rCon1_t", SurfaceWindow1.modFrXml);
            // This one is hardcoded in because we don't have any English lyrics in an XML file yet.
            engText.Text = "Oh, how far transgression\nis spreading!\nVirtue is dislodged\nfrom the sanctuary.\nNow Christ is dragged\nto a new tribunal,\nwith Peter using\nthe sword of Pilate.\nRelying on the counsel\nof Fauvel,\none comes to grief;\nthe celestial legion\njustly complains.\nTherefore it begs\nthe Father and the Son\nthat for a remedy\nfor all this\nimmediately\nthe fostering Spirit provide.";


            notesCanvas.Width = 580;
            notesCanvas.Height = 860;


            // modern music notation image file
            musicImg = new Image();
            musicImg.Source = new BitmapImage(new Uri(@"..\..\musicz\2rCo1.png", UriKind.Relative));
            musicImg.Width = 580;
            musicImg.Height = 860;

            
            notesCanvas.Children.Add(musicImg);


            // Create mediaplayer for audio playback.
            play_2rCon1 = new MediaPlayer();
            play_2rCon1.Open(new Uri(@"..\..\musicz\2rCo1.wma", UriKind.Relative));
            play_2rCon1.MediaEnded += new EventHandler(play_2rCon1_MediaEnded);

            
        }



        // Polyphonic music study page (other polyphonic pieces should follow this format).
        // One big challenge for polyphonic music will be to create display of voices programmatically, bc of varying # of voices in motets
        
        void study_Poly(object sender, RoutedEventArgs e)
        {
            String tag = "2vMo2";
            musicControls(tag);

            playpause.Click += new RoutedEventHandler(playpause_2_Click);
            stop.Click += new RoutedEventHandler(stop_2_Click);
            mod_frenchText.Text = Search.getByTag(tag, SurfaceWindow1.modFrXml);
            engText.Text = "No English translated lyrics for this piece YET!";

            





            motetParts = new StackPanel(); // StackPanel for the expandable parts to stack on top of eachother.
            motetParts.Orientation = Orientation.Vertical;

            fullExp = new MusicExpander("Full Score");


            // Motet score
            motetScore = new StackPanel();
            motetScore.Orientation = Orientation.Vertical;
            fullExp.Content = motetScore;
            fullExp.Header = "Full Score";

            // Score has several pages.
            // Create an object here called MotetPage and make one of these for each

            /**
             * GOAL FOR THIS SECTION:
             * 1. Get names of each .png file (page) for this piece of music
             * 2. Create a ScorePage object for each page - ScorePage implements Grid.
             * 3. Add each ScorePage to motetScore.
             * */


            String one = "2vMo2-1";
            String two = "2vMo2-2";
            String three = "2vMo2-3";

            List<String> pages = new List<String>(); // This List should be populated programmatically, i.e. by getting all file names in a folder
            pages.Add(one);
            pages.Add(two);
            pages.Add(three);

            foreach (String s in pages)
            {
                ScorePage newPage = new ScorePage(s);
                motetScore.Children.Add(newPage);
            }


            /**
             * GOAL FOR THIS SECTION:
             * 1. Get names of each voice for this piece of music.
             *    Audio files should be named .duplum, .triplum, etc so that same word can be displayed as title (perhaps w case adjustment).
             * 2. Create an expander for each voice.
             * 3. Add each expander to the motetParts StackPanel.
             * */

            v1Exp = new MusicPartExpander("v1", tag);
            tenorExp = new MusicPartExpander("tenor", tag);

            tenorExp.player.MediaEnded += new EventHandler(play_2vMo2_tenor_MediaEnded);
            // Why is there no method for v1Exp ending?!?

            v1Exp.muteTB.Checked += new RoutedEventHandler(v1_mute_Checked);
            v1Exp.soloTB.Checked += new RoutedEventHandler(v1_solo_Checked);
            tenorExp.muteTB.Checked += new RoutedEventHandler(tenor_mute_Checked);
            tenorExp.soloTB.Checked += new RoutedEventHandler(tenor_solo_Checked);

            v1Exp.muteTB.Unchecked += new RoutedEventHandler(v1_mute_Unchecked);
            v1Exp.soloTB.Unchecked += new RoutedEventHandler(v1_solo_Unchecked);
            tenorExp.muteTB.Unchecked += new RoutedEventHandler(tenor_mute_Unchecked);
            tenorExp.soloTB.Unchecked += new RoutedEventHandler(tenor_solo_Unchecked);



            motetParts.Children.Add(fullExp);
            motetParts.Children.Add(v1Exp);
            motetParts.Children.Add(tenorExp);


            


            notesCanvas.Width = 560;
            notesCanvas.Height = 4000;
            notesCanvas.Children.Add(motetParts);

        }



        void showAudioOptions(object sender, RoutedEventArgs e)
        {
            selectAudioButton.Visibility = System.Windows.Visibility.Hidden;
            selectAudioListBox.Visibility = System.Windows.Visibility.Visible;
        }

        // Note: This gets weird if you select "Select audio:" twice in a row
        void audioOptionSelected(object sender, RoutedEventArgs e)
        {
            ListBoxItem lbi = sender as ListBoxItem;

            if (lbi != selectAudio)
            {
                lastAudioChoice = lbi;
                selectAudioButton.Content = lbi.Content;
            }
            else 
                selectAudioListBox.SelectedItem = lastAudioChoice;

            selectAudioButton.Visibility = System.Windows.Visibility.Visible;
            selectAudioListBox.Visibility = System.Windows.Visibility.Hidden;
        }

        // Unedited
        void playpause_1_Click(object sender, RoutedEventArgs e)
        {
            if ((string)playpause.Content == "||")
            {
                playpause.Content = "►";
                playpause.FontSize = 35;
                if (play_2rCon1.CanPause)
                    play_2rCon1.Pause();
            }
            else if ((string)playpause.Content == "►")
            {
                playpause.Content = "||";
                playpause.FontSize = 25;
                play_2rCon1.Play();
            }
        }

        void playpause_2_Click(object sender, RoutedEventArgs e)
        {
            StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

            if ((string)playpause.Content == "||")
            {
                playpause.Content = "►";
                playpause.FontSize = 35;
                selectedTab.tenorExp.player.Pause();
                selectedTab.v1Exp.player.Pause();
            }
            else if ((string)playpause.Content == "►")
            {
                playpause.Content = "||";
                playpause.FontSize = 25;
                selectedTab.tenorExp.player.Play();
                selectedTab.v1Exp.player.Play();
            }
        }


        void play_2rCon1_MediaEnded(object sender, EventArgs e)
        {
            playpause.Content = "►";
            playpause.FontSize = 35;
            play_2rCon1.Stop();
        }


        void v1_mute_Checked(object sender, RoutedEventArgs e)
        {
            StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

            selectedTab.v1Exp.Opacity = 0.7;
            selectedTab.v1Exp.soloTB.IsEnabled = false;

            // would you want to have it playing while all voices are muted???
            if ((bool)selectedTab.tenorExp.muteTB.IsChecked)
            {
                selectedTab.tenorExp.player.Stop();
                selectedTab.v1Exp.player.Stop();
                playpause.IsEnabled = false;
                stop.IsEnabled = false;
                playpause.FontSize = 35;
                playpause.Content = "►";
            }

            selectedTab.v1Exp.player.IsMuted = true;
        }

        void v1_solo_Checked(object sender, RoutedEventArgs e)
        {
            StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

            selectedTab.v1Exp.BorderBrush = Brushes.YellowGreen;
            selectedTab.tenorExp.soloTB.IsEnabled = false;

            selectedTab.tenorExp.player.IsMuted = true;
        }

        void tenor_mute_Checked(object sender, RoutedEventArgs e)
        {
            StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

            selectedTab.tenorExp.Opacity = 0.7;
            selectedTab.tenorExp.soloTB.IsEnabled = false;

            if ((bool)selectedTab.v1Exp.muteTB.IsChecked)
            {
                selectedTab.tenorExp.player.Stop();
                selectedTab.v1Exp.player.Stop();
                playpause.IsEnabled = false;
                stop.IsEnabled = false;
                playpause.FontSize = 35;
                playpause.Content = "►";
            }
            selectedTab.tenorExp.player.IsMuted = true;
        }

        void tenor_solo_Checked(object sender, RoutedEventArgs e)
        {
            StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

            selectedTab.tenorExp.BorderBrush = Brushes.YellowGreen;
            selectedTab.v1Exp.soloTB.IsEnabled = false;

            selectedTab.v1Exp.player.IsMuted = true;
        }

        void v1_mute_Unchecked(object sender, RoutedEventArgs e)
        {
            StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

            selectedTab.v1Exp.Opacity = 1.00;
            if (!(bool)selectedTab.tenorExp.soloTB.IsChecked)
                selectedTab.v1Exp.soloTB.IsEnabled = true;

            playpause.IsEnabled = true;
            stop.IsEnabled = true;
            selectedTab.v1Exp.player.IsMuted = false;
        }

        void v1_solo_Unchecked(object sender, RoutedEventArgs e)
        {
            StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

            selectedTab.v1Exp.BorderBrush = Brushes.Black;
            selectedTab.tenorExp.soloTB.IsEnabled = true;

            selectedTab.tenorExp.player.IsMuted = false;
        }

        void tenor_mute_Unchecked(object sender, RoutedEventArgs e)
        {
            StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

            selectedTab.tenorExp.Opacity = 1.00;
            if (!(bool)selectedTab.v1Exp.soloTB.IsChecked)
                selectedTab.tenorExp.soloTB.IsEnabled = true;

            playpause.IsEnabled = true;
            stop.IsEnabled = true;
            selectedTab.tenorExp.player.IsMuted = false;
        }

        void tenor_solo_Unchecked(object sender, RoutedEventArgs e)
        {
            StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

            selectedTab.tenorExp.BorderBrush = Brushes.Black;
            selectedTab.v1Exp.soloTB.IsEnabled = true;

            selectedTab.v1Exp.player.IsMuted = false;
        }


        void stop_1_Click(object sender, RoutedEventArgs e)
        {
            playpause.Content = "►";
            playpause.FontSize = 35;
            play_2rCon1.Stop();
        }
        void stop_2_Click(object sender, RoutedEventArgs e)
        {
            StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

            playpause.Content = "►";
            playpause.FontSize = 35;
            selectedTab.v1Exp.player.Stop();
            selectedTab.tenorExp.player.Stop();
        }

        void play_2vMo2_tenor_MediaEnded(object sender, EventArgs e)
        {
            StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

            playpause.Content = "►";
            playpause.FontSize = 35;
            selectedTab.v1Exp.player.Stop();
            selectedTab.tenorExp.player.Stop();
        }
    }
}
