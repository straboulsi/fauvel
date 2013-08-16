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
        public Image monoImg;
        public List<ListBoxItem> audioOptions;
        public ListBox selectAudioListBox;
        public ListBoxItem selectAudio, MIDI, liveRecording, lastAudioChoice;
        public MediaPlayer monoPlayer;
        public MusicExpander fullExp;
        public MusicPartExpander v1Exp, tenorExp;
        private SideBar mySideBar;
        public StackPanel motetParts, motetScore;
        public SurfaceScrollViewer noteScroll;
        public TabControl display;
        public TabItem notesTab, mod_frenchTab, engTab;
        public TextBlock studyTabHeader, studyPrompt, mod_frenchText, engText, musicTitle;



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

            
            headerGrid.Children.Add(studyTabHeader);
            canvas.Children.Add(studyPrompt);
            canvas.Children.Add(mono);
            canvas.Children.Add(poly);
            
            /** 
             * The below listeners are set temporarily to two defaults 
             * Once we can select a piece of music to study from the manuscript (shown in main UI), get its tag and call open_Music as demonstrated below:
             * */
            mono.Click += delegate(object sender, RoutedEventArgs e) { open_Music(sender, e, "2rCon1"); };
            mono.TouchDown += delegate(object sender, TouchEventArgs e) { open_Music(sender, e, "2rCon1"); };
            poly.Click += delegate(object sender, RoutedEventArgs e) { open_Music(sender, e, "2vMo2"); };
            poly.TouchDown += delegate(object sender, TouchEventArgs e) { open_Music(sender, e, "2vMo2"); };

        }



        // Determines whether a piece of music is monophonic or polyphonic
        // Opens a new music tab for mono or poly studying
        private void open_Music(object sender, RoutedEventArgs e, String tag)
        {
            if (Study.hasMultipleVoices(tag) == false)
                study_Mono(sender, e, tag);
            else if(Study.hasMultipleVoices(tag) == true)
                study_Poly(sender, e, tag);
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


            /**
             * Organization of containers for notesTab:
             * 1. notesTab
             * 2. notesTab.Content = notesTabCanvas;
             * 3. notesTabCanvas.Children.Add(noteScroll);
             * 4. noteScroll.Content = notesCanvas;
             * */

            notesTab.Content = notesTabCanvas; 
            notesTabCanvas.Children.Add(noteScroll);



            display.Items.Add(notesTab);
            display.Items.Add(mod_frenchTab);
            display.Items.Add(engTab);

            canvas.Children.Add(musicTitle);
            canvas.Children.Add(playpause);
            canvas.Children.Add(stop);
            canvas.Children.Add(display);
            canvas.Children.Add(selectAudioButton);
            canvas.Children.Add(selectAudioListBox);



        }


        // Monophonic music study page (other monophonic pieces should follow this format).
        private void study_Mono(object sender, RoutedEventArgs e, String tag)
        {

            musicControls(tag); // Sets up the top part of the StudyTab: title, play/stop buttons, audio selection, etc.

            playpause.Click += new RoutedEventHandler(playpause_1_Click);
            //stop.Click += new RoutedEventHandler(stop_1_Click);
            stop.Click += delegate(object s, RoutedEventArgs ev) { stop_Music(s, ev, "mono"); };
            mod_frenchText.Text = Search.getByTag(tag, SurfaceWindow1.modFrXml);
            // This one is hardcoded in because we don't have any English lyrics in an XML file yet.
            engText.Text = "Oh, how far transgression\nis spreading!\nVirtue is dislodged\nfrom the sanctuary.\nNow Christ is dragged\nto a new tribunal,\nwith Peter using\nthe sword of Pilate.\nRelying on the counsel\nof Fauvel,\none comes to grief;\nthe celestial legion\njustly complains.\nTherefore it begs\nthe Father and the Son\nthat for a remedy\nfor all this\nimmediately\nthe fostering Spirit provide.";


            // modern music notation image file
            monoImg = new Image();
            monoImg.Source = new BitmapImage(new Uri(@"..\..\musicz\" + tag + ".png", UriKind.Relative)); ///
            monoImg.Width = 580;
            monoImg.Height = 860;

            notesCanvas.Width = 580;
            notesCanvas.Height = 860;
            notesCanvas.Children.Add(monoImg);

            // Create mediaplayer for audio playback.
            monoPlayer = new MediaPlayer();
            monoPlayer.Open(new Uri(@"..\..\musicz\" + tag + ".wma", UriKind.Relative));
            monoPlayer.MediaEnded += new EventHandler(monoPlayer_MediaEnded);

            
        }



        // Polyphonic music study page (other polyphonic pieces should follow this format).
        // One big challenge for polyphonic music will be to create display of voices programmatically, bc of varying # of voices in motets
        
        void study_Poly(object sender, RoutedEventArgs e, String tag)
        {
            musicControls(tag); // Sets up the top part of the StudyTab: title, play/stop buttons, audio selection, etc.

            playpause.Click += new RoutedEventHandler(playpause_2_Click);
            //stop.Click += new RoutedEventHandler(stop_2_Click);
            stop.Click += delegate(object s, RoutedEventArgs ev) { stop_Music(s, ev, "poly"); };

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

            List<MusicPartExpander> otherExpsForV1 = new List<MusicPartExpander>();
            otherExpsForV1.Add(tenorExp);

            List<MusicPartExpander> otherExpsForTenor = new List<MusicPartExpander>();
            otherExpsForTenor.Add(v1Exp);

            v1Exp.muteTB.Checked += delegate(object s, RoutedEventArgs r) { mute_Checked(s, r, v1Exp, otherExpsForV1); };
            tenorExp.muteTB.Checked += delegate(object s, RoutedEventArgs r) { mute_Checked(s, r, tenorExp, otherExpsForTenor); };
            v1Exp.muteTB.Unchecked += delegate(object s, RoutedEventArgs r) { mute_Unchecked(s, r, v1Exp, otherExpsForV1); };
            tenorExp.muteTB.Unchecked += delegate(object s, RoutedEventArgs r) { mute_Unchecked(s, r, tenorExp, otherExpsForTenor); };

            v1Exp.soloTB.Checked += delegate(object s, RoutedEventArgs r) { solo_Checked(s, r, v1Exp, otherExpsForV1); };
            tenorExp.soloTB.Checked += delegate(object s, RoutedEventArgs r) { solo_Checked(s, r, tenorExp, otherExpsForTenor); };
            v1Exp.soloTB.Unchecked += delegate(object s, RoutedEventArgs r) { solo_Unchecked(s, r, v1Exp, otherExpsForV1); };
            tenorExp.soloTB.Unchecked += delegate(object s, RoutedEventArgs r) { solo_Unchecked(s, r, tenorExp, otherExpsForTenor); };


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
                if (monoPlayer.CanPause)
                    monoPlayer.Pause();
            }
            else if ((string)playpause.Content == "►")
            {
                playpause.Content = "||";
                playpause.FontSize = 25;
                monoPlayer.Play();
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


        void monoPlayer_MediaEnded(object sender, EventArgs e)
        {
            playpause.Content = "►";
            playpause.FontSize = 35;
            monoPlayer.Stop();
        }


        void mute_Checked(object sender, RoutedEventArgs e, MusicPartExpander thisExp, List<MusicPartExpander> otherExps)
        {
            thisExp.Opacity = 0.7;
            thisExp.soloTB.IsEnabled = false;

            Boolean allOtherExpsMuted = true;
            foreach (MusicPartExpander mpe in otherExps)
            {
                if (mpe.muteTB.IsChecked == false) // If any other voices are still audible
                    allOtherExpsMuted = false;
            }

            if (allOtherExpsMuted == true)
            {
                thisExp.player.Stop();
                foreach (MusicPartExpander mpe in otherExps)
                {
                    mpe.player.Stop();
                }
                playpause.IsEnabled = false;
                stop.IsEnabled = false;
                playpause.FontSize = 35;
                playpause.Content = "►";
            }
            
            thisExp.player.IsMuted = true;
        }

        void mute_Unchecked(object sender, RoutedEventArgs e, MusicPartExpander thisExp, List<MusicPartExpander> otherExps)
        {
            thisExp.Opacity = 1.00;

            // If none of the other voices are soloing, then this one can enable soloing
            Boolean otherVoiceSoloing = false;
            foreach (MusicPartExpander mpe in otherExps)
            {
                if (mpe.soloTB.IsChecked == true)
                    otherVoiceSoloing = true;
            }

            if (otherVoiceSoloing == false)
            {
                thisExp.soloTB.IsEnabled = true;
            }

            playpause.IsEnabled = true;
            stop.IsEnabled = true;
            thisExp.player.IsMuted = false;

        }


        void solo_Checked(object sender, RoutedEventArgs e, MusicPartExpander thisExp, List<MusicPartExpander> otherExps)
        {
            thisExp.BorderBrush = Brushes.YellowGreen;
            foreach (MusicPartExpander mpe in otherExps)
            {
                mpe.soloTB.IsEnabled = false;
                mpe.player.IsMuted = true;
            }


        }

        void solo_Unchecked(object sender, RoutedEventArgs e, MusicPartExpander thisExp, List<MusicPartExpander> otherExps)
        {
            thisExp.BorderBrush = Brushes.Black;

            foreach (MusicPartExpander mpe in otherExps)
            {
                mpe.soloTB.IsEnabled = true;
                mpe.player.IsMuted = false;
            }
        }

        //void v1_solo_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

        //    selectedTab.v1Exp.BorderBrush = Brushes.Black;
        //    selectedTab.tenorExp.soloTB.IsEnabled = true;

        //    selectedTab.tenorExp.player.IsMuted = false;
        //}



        //void tenor_solo_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

        //    selectedTab.tenorExp.BorderBrush = Brushes.Black;
        //    selectedTab.v1Exp.soloTB.IsEnabled = true;

        //    selectedTab.v1Exp.player.IsMuted = false;
        //}


        //void v1_solo_Checked(object sender, RoutedEventArgs e)
        //{
        //    StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

        //    selectedTab.v1Exp.BorderBrush = Brushes.YellowGreen;
        //    selectedTab.tenorExp.soloTB.IsEnabled = false;

        //    selectedTab.tenorExp.player.IsMuted = true;
        //}

        

        //void tenor_solo_Checked(object sender, RoutedEventArgs e)
        //{
        //    StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

        //    selectedTab.tenorExp.BorderBrush = Brushes.YellowGreen;
        //    selectedTab.v1Exp.soloTB.IsEnabled = false;

        //    selectedTab.v1Exp.player.IsMuted = true;
        //}





        void stop_Music(object sender, RoutedEventArgs e, String musicType)
        {
            StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

            playpause.Content = "►";
            playpause.FontSize = 35;

            if (musicType == "poly") // polyphonic
            {
                selectedTab.v1Exp.player.Stop();
                selectedTab.tenorExp.player.Stop();
            }
            else if (musicType == "mono")
                selectedTab.monoPlayer.Stop();
        }


        //void stop_1_Click(object sender, RoutedEventArgs e)
        //{
        //    playpause.Content = "►";
        //    playpause.FontSize = 35;
        //    monoPlayer.Stop();
        //}
        //void stop_2_Click(object sender, RoutedEventArgs e)
        //{
        //    StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

        //    playpause.Content = "►";
        //    playpause.FontSize = 35;
        //    selectedTab.v1Exp.player.Stop();
        //    selectedTab.tenorExp.player.Stop();
        //}

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
