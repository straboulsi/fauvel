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
     * Primary Coder: Jamie Chong
     * */
    class StudyTab : SideBarTab
    {

        public Button mono, poly, playpause, stop, selectAudioButton; 
        public Canvas notesCanvas, notesTabCanvas;
        public Expander fullExp, v1Exp, tenorExp;
        public Grid mo_lb1, mo_lb2, mo_lb3;
        public Image musicImg, musicImg1, musicImg2, musicImg3, v1_Img, tenor_Img;
        public List<ListBoxItem> audioOptions;
        public ListBox selectAudioListBox;
        public ListBoxItem v1_name_lb, v1_mute_lb, v1_solo_lb, tenor_name_lb, tenor_mute_lb, tenor_solo_lb, space1, space2, 
            selectAudio, MIDI, liveRecording, lastAudioChoice;
        public MediaPlayer play_2rCon1, play_2vMo2_v1, play_2vMo2_tenor;
        public StackPanel motetParts, motetScore, v1_buttons, tenor_buttons;
        public SurfaceScrollViewer noteScroll;
        public TabControl display;
        public TabItem notesTab, mod_frenchTab, engTab;
        public TextBlock studyTabHeader, studyPrompt, mod_frenchText, engText, musicTitle;
        public ToggleButton v1_mute, v1_solo, tenor_mute, tenor_solo;



        public StudyTab(SideBar mySideBar, SurfaceWindow1 surfaceWindow) : base(mySideBar)
        {
            // Opening page.
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

            // For now, until we implement selecting the piece directly on the folio.
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
            mono.Click += new RoutedEventHandler(study_Mono);
            mono.TouchDown += new EventHandler<TouchEventArgs>(study_Mono);
            poly.Click += new RoutedEventHandler(study_Poly);
        }


        private void musicControls(String musicName, String musicAbbrv)
        {
            canvas.Children.Remove(studyPrompt);
            canvas.Children.Remove(mono);
            canvas.Children.Remove(poly);


            // Title of the piece.
            musicTitle = new TextBlock();
            Canvas.SetLeft(musicTitle, 15);
            Canvas.SetTop(musicTitle, 45);
            musicTitle.Text = musicName; //
            musicTitle.FontSize = 30;
            studyTabHeader.Text = musicAbbrv; //

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

            canvas.Children.Add(selectAudioButton);
            canvas.Children.Add(selectAudioListBox);
            canvas.Children.Add(musicTitle);
            canvas.Children.Add(playpause);
            canvas.Children.Add(stop);
            canvas.Children.Add(display);

            display.Items.Add(notesTab);
            display.Items.Add(mod_frenchTab);
            display.Items.Add(engTab);
        }


        // Monophonic music study page (other monophonic pieces should follow this format).
        private void study_Mono(object sender, RoutedEventArgs e)
        {
            musicControls("Conductus : Heu ! Quo progreditur (PM 6)", "2rCon1");

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
        void study_Poly(object sender, RoutedEventArgs e)
        {
            musicControls("Ad solitum vomitum", "2vMo2");

            playpause.Click += new RoutedEventHandler(playpause_2_Click);
            stop.Click += new RoutedEventHandler(stop_2_Click);
            mod_frenchText.Text = Search.getByTag("2vMo2_t", SurfaceWindow1.modFrXml);
            engText.Text = "No English translated lyrics for this piece YET!";

            
            // StackPanel for the expandable parts to stack on top of eachother.
            motetParts = new StackPanel();
            motetParts.Orientation = Orientation.Vertical;

            fullExp = new Expander();
            v1Exp = new Expander();
            tenorExp = new Expander();

            fullExp.IsExpanded = false;
            v1Exp.IsExpanded = false;
            tenorExp.IsExpanded = false;

            fullExp.ExpandDirection = ExpandDirection.Down;
            v1Exp.ExpandDirection = ExpandDirection.Down;
            tenorExp.ExpandDirection = ExpandDirection.Down;

            fullExp.Width = 560;
            v1Exp.Width = 560;
            tenorExp.Width = 560;

            fullExp.MinHeight = 30;
            v1Exp.MinHeight = 20;
            tenorExp.MinHeight = 20;

            fullExp.MaxHeight = 2400;
            v1Exp.MaxHeight = 860;
            tenorExp.MaxHeight = 860;

            fullExp.BorderBrush = Brushes.Black;
            v1Exp.BorderBrush = Brushes.Black;
            tenorExp.BorderBrush = Brushes.Black;

            // Score has several pages.
            musicImg1 = new Image();
            musicImg1.Source = new BitmapImage(new Uri(@"..\..\musicz\2vMo2-1.png", UriKind.Relative));
            musicImg1.Width = 560;

            musicImg2 = new Image();
            musicImg2.Source = new BitmapImage(new Uri(@"..\..\musicz\2vMo2-2.png", UriKind.Relative));
            musicImg2.Width = 560;

            musicImg3 = new Image();
            musicImg3.Source = new BitmapImage(new Uri(@"..\..\musicz\2vMo2-3.png", UriKind.Relative));
            musicImg3.Width = 560;

            motetScore = new StackPanel();
            motetScore.Orientation = Orientation.Vertical;

            mo_lb1 = new Grid();
            mo_lb2 = new Grid();
            mo_lb3 = new Grid();
            mo_lb1.Height = 860;
            mo_lb2.Height = 860;
            mo_lb3.Height = 860;

            mo_lb1.Children.Add(musicImg1);
            mo_lb2.Children.Add(musicImg2);
            mo_lb3.Children.Add(musicImg3);

            motetScore.Children.Add(mo_lb1);
            motetScore.Children.Add(mo_lb2);
            motetScore.Children.Add(mo_lb3);

            fullExp.Content = motetScore;
            fullExp.Header = "Full Score";

            // For the individual voices, we have mute and solo buttons as well.
            v1_Img = new Image();
            v1_Img.Source = new BitmapImage(new Uri(@"..\..\musicz\2vMo2_v1.png", UriKind.Relative));
            v1_Img.Width = 560;

            tenor_Img = new Image();
            tenor_Img.Source = new BitmapImage(new Uri(@"..\..\musicz\2vMo2_tenor.png", UriKind.Relative));
            tenor_Img.Width = 560;

            v1Exp.Content = v1_Img;
            tenorExp.Content = tenor_Img;

            v1_buttons = new StackPanel();
            tenor_buttons = new StackPanel();
            v1_buttons.Orientation = Orientation.Horizontal;
            tenor_buttons.Orientation = Orientation.Horizontal;

            v1_name_lb = new ListBoxItem();
            v1_mute_lb = new ListBoxItem();
            v1_solo_lb = new ListBoxItem();
            tenor_name_lb = new ListBoxItem();
            tenor_mute_lb = new ListBoxItem();
            tenor_solo_lb = new ListBoxItem();
            space1 = new ListBoxItem();
            space2 = new ListBoxItem();

            v1_mute = new ToggleButton();
            v1_solo = new ToggleButton();
            tenor_mute = new ToggleButton();
            tenor_solo = new ToggleButton();

            v1_mute.Content = "M";
            v1_solo.Content = "S";
            tenor_mute.Content = "M";
            tenor_solo.Content = "S";

            v1_mute.FontSize = 14;
            v1_solo.FontSize = 14;
            tenor_mute.FontSize = 14;
            tenor_solo.FontSize = 14;

            v1_mute.Height = 20;
            v1_solo.Height = 20;
            tenor_solo.Height = 20;
            tenor_mute.Height = 20;
            v1_mute.Width = 20;
            v1_solo.Width = 20;
            tenor_mute.Width = 20;
            tenor_solo.Width = 20;

            v1_name_lb.Content = "Voice  ";
            v1_mute_lb.Content = v1_mute;
            v1_solo_lb.Content = v1_solo;
            space1.Content = "  ";

            tenor_name_lb.Content = "Tenor  ";
            tenor_mute_lb.Content = tenor_mute;
            tenor_solo_lb.Content = tenor_solo;
            space2.Content = "  ";

            v1_buttons.Children.Add(v1_name_lb);
            v1_buttons.Children.Add(v1_mute_lb);
            v1_buttons.Children.Add(space1);
            v1_buttons.Children.Add(v1_solo_lb);

            tenor_buttons.Children.Add(tenor_name_lb);
            tenor_buttons.Children.Add(tenor_mute_lb);
            tenor_buttons.Children.Add(space2);
            tenor_buttons.Children.Add(tenor_solo_lb);

            v1Exp.Header = v1_buttons;
            tenorExp.Header = tenor_buttons;

            motetParts.Children.Add(fullExp);
            motetParts.Children.Add(v1Exp);
            motetParts.Children.Add(tenorExp);

            notesCanvas.Width = 560;
            notesCanvas.Height = 4000;
            notesCanvas.Children.Add(motetParts);



            // Create mediaplayers for each voice for audio playback.
            play_2vMo2_v1 = new MediaPlayer();
            play_2vMo2_tenor = new MediaPlayer();
            play_2vMo2_v1.Open(new Uri(@"..\..\musicz\2vMo2_v1.wma", UriKind.Relative));
            play_2vMo2_tenor.Open(new Uri(@"..\..\musicz\2vMo2_tenor.wma", UriKind.Relative));
            play_2vMo2_tenor.MediaEnded += new EventHandler(play_2vMo2_tenor_MediaEnded);



            // Add listeners.

            v1_mute.Checked += new RoutedEventHandler(v1_mute_Checked);
            v1_solo.Checked += new RoutedEventHandler(v1_solo_Checked);
            tenor_mute.Checked += new RoutedEventHandler(tenor_mute_Checked);
            tenor_solo.Checked += new RoutedEventHandler(tenor_solo_Checked);

            v1_mute.Unchecked += new RoutedEventHandler(v1_mute_Unchecked);
            v1_solo.Unchecked += new RoutedEventHandler(v1_solo_Unchecked);
            tenor_mute.Unchecked += new RoutedEventHandler(tenor_mute_Unchecked);
            tenor_solo.Unchecked += new RoutedEventHandler(tenor_solo_Unchecked);
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
            if ((string)playpause.Content == "||")
            {
                playpause.Content = "►";
                playpause.FontSize = 35;
                play_2vMo2_v1.Pause();
                play_2vMo2_tenor.Pause();
            }
            else if ((string)playpause.Content == "►")
            {
                playpause.Content = "||";
                playpause.FontSize = 25;
                play_2vMo2_v1.Play();
                play_2vMo2_tenor.Play();
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
            v1Exp.Opacity = 0.7;
            v1_solo.IsEnabled = false;

            // would you want to have it playing while all voices are muted???
            if ((bool)tenor_mute.IsChecked)
            {
                play_2vMo2_tenor.Stop();
                play_2vMo2_v1.Stop();
                playpause.IsEnabled = false;
                stop.IsEnabled = false;
                playpause.FontSize = 35;
                playpause.Content = "►";
            }

            play_2vMo2_v1.IsMuted = true;
        }

        void v1_solo_Checked(object sender, RoutedEventArgs e)
        {
            v1Exp.BorderBrush = Brushes.YellowGreen;
            tenor_solo.IsEnabled = false;

            play_2vMo2_tenor.IsMuted = true;
        }

        void tenor_mute_Checked(object sender, RoutedEventArgs e)
        {
            tenorExp.Opacity = 0.7;
            tenor_solo.IsEnabled = false;

            if ((bool)v1_mute.IsChecked)
            {
                play_2vMo2_tenor.Stop();
                play_2vMo2_v1.Stop();
                playpause.IsEnabled = false;
                stop.IsEnabled = false;
                playpause.FontSize = 35;
                playpause.Content = "►";
            }
            play_2vMo2_tenor.IsMuted = true;
        }

        void tenor_solo_Checked(object sender, RoutedEventArgs e)
        {
            tenorExp.BorderBrush = Brushes.YellowGreen;
            v1_solo.IsEnabled = false;

            play_2vMo2_v1.IsMuted = true;
        }

        void v1_mute_Unchecked(object sender, RoutedEventArgs e)
        {
            v1Exp.Opacity = 1.00;
            if (!(bool)tenor_solo.IsChecked)
                v1_solo.IsEnabled = true;

            playpause.IsEnabled = true;
            stop.IsEnabled = true;
            play_2vMo2_v1.IsMuted = false;
        }

        void v1_solo_Unchecked(object sender, RoutedEventArgs e)
        {
            v1Exp.BorderBrush = Brushes.Black;
            tenor_solo.IsEnabled = true;

            play_2vMo2_tenor.IsMuted = false;
        }

        void tenor_mute_Unchecked(object sender, RoutedEventArgs e)
        {
            tenorExp.Opacity = 1.00;
            if (!(bool)v1_solo.IsChecked)
                tenor_solo.IsEnabled = true;

            playpause.IsEnabled = true;
            stop.IsEnabled = true;
            play_2vMo2_tenor.IsMuted = false;
        }

        void tenor_solo_Unchecked(object sender, RoutedEventArgs e)
        {
            tenorExp.BorderBrush = Brushes.Black;
            v1_solo.IsEnabled = true;

            play_2vMo2_v1.IsMuted = false;
        }


        void stop_1_Click(object sender, RoutedEventArgs e)
        {
            playpause.Content = "►";
            playpause.FontSize = 35;
            play_2rCon1.Stop();
        }
        void stop_2_Click(object sender, RoutedEventArgs e)
        {
            playpause.Content = "►";
            playpause.FontSize = 35;
            play_2vMo2_v1.Stop();
            play_2vMo2_tenor.Stop();
        }

        void play_2vMo2_tenor_MediaEnded(object sender, EventArgs e)
        {
            playpause.Content = "►";
            playpause.FontSize = 35;
            play_2vMo2_v1.Stop();
            play_2vMo2_tenor.Stop();
        }
    }
}
