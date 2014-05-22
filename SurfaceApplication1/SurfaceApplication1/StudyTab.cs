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
     * TO-DO
     * For each music object in Fauvel, we need:
     * - .png images of each page of the scores (for polyphonic, note that we need scores for each voice as well as full score)
     * - .wma files for each audio (again, for polyponic, one overall and one for each voice too)
     * - Jamie included some .wav files too but they don't seem to be called for in the current code...
     * Add in "part" attribute info to OriginalTextXML
     * 
     * Primary Coders: Jamie Chong (original hardcode) & Alison Y. Chang (conversion to programmatic)
     * */
    class StudyTab : SideBarTab
    {
        public enum TranslationLanguage { oldFrench = 0, modernFrench = 1, English = 2 };
        public TranslationLanguage _currentTranslationLanguage, _lastTranslationLanguage;
        public Button playpause, stop, selectAudioButton, SelectLanguageButton;
        public Canvas notesCanvas, notesTabCanvas;
        public Image monoImg;
        public List<ListBoxItem> audioOptions, languageOptions;
        public List<MusicPartExpander> allVoices;
        public ListBox selectAudioListBox, SelectLanguageListBox;
        public ListBoxItem pickLanguage, modernFrench, English;
        public ListBoxItem selectAudio, MIDI, liveRecording, lastAudioChoice;
        public MediaPlayer monoPlayer;
        public MusicExpander fullExp;
        private SideBar mySideBar;
        public StackPanel motetParts, motetScore;
        public String sampleMono, samplePoly;
        public SurfaceScrollViewer noteScroll;
        public TabControl display;
        public TabItem notesTab, tabOriginalText, tabTranslation;
        public TextBlock studyTabHeader, studyPrompt, OriginalText, TranslationText, musicTitle;
        private Canvas TranslationBox;
        private Pieces _pieces; //music objects on the current opening
        private Piece _currentPiece; //piece of music currently being viewed
        private const int ButtonHeight = 50;
        private const int ButtonWidth = 200;
        private const int ButtonFontSize = 20;



        public StudyTab(SideBar mySideBar, SurfaceWindow1 surfaceWindow, Pieces pieces)
            : base(mySideBar)
        {
            this.mySideBar = mySideBar;

            // Opening page.
            studyPrompt = new TextBlock();
            studyTabHeader = new TextBlock();

            _pieces = pieces;

            headerImage.Source = new BitmapImage(new Uri(@"..\..\icons\music.png", UriKind.Relative));
            studyTabHeader.HorizontalAlignment = HorizontalAlignment.Center;
            studyTabHeader.VerticalAlignment = VerticalAlignment.Center;
            studyTabHeader.FontSize = 21;
            headerGrid.Children.Add(studyTabHeader);

            
            studyPrompt.FontSize = 30;
            studyPrompt.Text = "Please select a piece of music.";
            Canvas.SetLeft(studyPrompt, 32);
            Canvas.SetTop(studyPrompt, 45);
            studyPrompt.Visibility = System.Windows.Visibility.Visible;
            canvas.Children.Add(studyPrompt);

            TranslationBox = new Canvas();

            int offsetY = 200; 
            int offsetX = 100;
            foreach (Piece p in pieces.Values)
            {
                Button button = new Button();
                //button.Click += delegate(object sender, RoutedEventArgs e) { DisplayMusicItem(sender, e, p); };
                button.Click += new RoutedEventHandler(DisplayMusicItem);
                button.Name = '_' + p.ID; 
                button.Height = ButtonHeight;
                button.Width = ButtonWidth;
                button.Content = p.Title;
                button.FontSize = ButtonFontSize;
                Canvas.SetLeft(button, offsetX);
                Canvas.SetTop(button, offsetY);
                offsetY += 100;
                canvas.Children.Add(button);
            }

        }

        private void DisplayMusicItem(object sender, RoutedEventArgs e) //, Piece piece)
        {
            Button btn = sender as Button;
            var piece = _pieces.GetPieceById(btn.Name.Replace("_",""));
            _currentPiece = piece;
            if (piece.Voices > 1)
                StudyPoly(sender, e, piece);
            else
                StudyMono(sender, e, piece);
        }


        private void MusicControls(Piece piece)
        {
            canvas.Children.Remove(studyPrompt);
          
            // Title of the piece.
            var title = piece.Title;
            musicTitle = new TextBlock();
            Canvas.SetLeft(musicTitle, 15);
            Canvas.SetTop(musicTitle, 45);
            musicTitle.Text = title;
            musicTitle.FontSize = 30;
            studyTabHeader.Text = Study.firstWord(title); // Sets tab header to the first word of the title, i.e. the music genre (Motet or Conductus)

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
            notesTab.Header = "Edition";
            notesTab.Height = 50;
            notesTab.Width = 200;
            notesTab.FontSize = 20;



            tabOriginalText = new TabItem();
            OriginalText = new TextBlock();
            tabOriginalText.Header = "Original Text";
            tabOriginalText.Height = 50;
            tabOriginalText.Width = 175;
            tabOriginalText.FontSize = 20;
            tabOriginalText.Content = OriginalText;

            tabTranslation = new TabItem();
            TranslationText = new TextBlock();
            TranslationText.Width = canvas.Width;
            TranslationText.Height = canvas.Width - 50;
            TranslationText.Text = Search.getByTag(piece.ID, SurfaceWindow1.modFrXml);
            
            tabTranslation.Header = "Translation";
            tabTranslation.Height = 50;
            tabTranslation.Width = 175;
            tabTranslation.FontSize = 20;

            SelectLanguageListBox = new ListBox();
            SelectLanguageListBox.Background = Brushes.White;
            SelectLanguageListBox.Visibility = Visibility.Hidden;
            SelectLanguageListBox.Width = 175;

            SelectLanguageButton = new Button();
            SelectLanguageButton.Visibility = Visibility.Visible;

            SelectLanguageButton.Width = 175;
            SelectLanguageButton.Height = 50;
            SelectLanguageButton.Content = (String)"Select Language";
            SelectLanguageButton.FontSize = 21;
            SelectLanguageListBox.FontSize = 21;
            SelectLanguageListBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            SelectLanguageListBox.SelectedIndex = 0;

            //sets the position of language selection button and translated text relative to sidetab
            int n = 10;
            Canvas.SetLeft(SelectLanguageButton, n);
            Canvas.SetLeft(SelectLanguageListBox, n);
            Canvas.SetTop(SelectLanguageListBox, n);
            Canvas.SetTop(SelectLanguageButton, n);
            Canvas.SetTop(TranslationText, 50 + n + 5);

            pickLanguage = new ListBoxItem();
            modernFrench = new ListBoxItem();
            English = new ListBoxItem();

            pickLanguage.Content = "Select Language:";

            modernFrench.Content = "Modern French";
            English.Content = "English";

            languageOptions = new List<ListBoxItem>();
            languageOptions.Add(pickLanguage);

            languageOptions.Add(modernFrench);
            languageOptions.Add(English);

            foreach (ListBoxItem lbi in languageOptions)
            {
                lbi.Height = 50;
                lbi.FontSize = 21;
                SelectLanguageListBox.Items.Add(lbi);
                lbi.HorizontalAlignment = HorizontalAlignment.Center;
            }
           
            SelectLanguageButton.TouchDown += new EventHandler<TouchEventArgs>(displaySearchLanguages);
            SelectLanguageButton.Click += new RoutedEventHandler(displaySearchLanguages);
            pickLanguage.Selected += new RoutedEventHandler(TranslationLanguageChanged);
            modernFrench.Selected += new RoutedEventHandler(TranslationLanguageChanged);
            English.Selected += new RoutedEventHandler(TranslationLanguageChanged);

            tabTranslation.Content = TranslationBox;
            TranslationBox.Children.Add(TranslationText);
            TranslationBox.Children.Add(SelectLanguageListBox);
            TranslationBox.Children.Add(SelectLanguageButton);

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
             * Organization of containers for notesTab (just bc these can be confusing)
             * 1. notesTab
             * 2. notesTab.Content = notesTabCanvas;
             * 3. notesTabCanvas.Children.Add(noteScroll);
             * 4. noteScroll.Content = notesCanvas;
             * */

            notesTab.Content = notesTabCanvas;
            notesTabCanvas.Children.Add(noteScroll);


            display.Items.Add(notesTab);
            display.Items.Add(tabOriginalText);
            display.Items.Add(tabTranslation);

            canvas.Children.Add(musicTitle);
            canvas.Children.Add(playpause);
            canvas.Children.Add(stop);
            canvas.Children.Add(display);
            canvas.Children.Add(selectAudioButton);
            canvas.Children.Add(selectAudioListBox);



        }

        private void displaySearchLanguages(object sender, RoutedEventArgs e)
        {
            if (SelectLanguageListBox.Visibility == Visibility.Collapsed | SelectLanguageListBox.Visibility == Visibility.Hidden)
            {
                SelectLanguageListBox.Visibility = Visibility.Visible;
                SelectLanguageButton.Visibility = Visibility.Collapsed;
            }
            else
                SelectLanguageListBox.Visibility = Visibility.Collapsed;

        }

        private void TranslationLanguageChanged(object sender, RoutedEventArgs e)
        {
            ListBoxItem box = (ListBoxItem)sender;
            SelectLanguageButton.Content = box.Content;
            SelectLanguageListBox.Visibility = Visibility.Hidden;
            SelectLanguageButton.Visibility = Visibility.Visible;
            
            if (box == modernFrench)
            {
                _currentTranslationLanguage = TranslationLanguage.modernFrench;
                TranslationText.Text = Search.getByTag(_currentPiece.ID, SurfaceWindow1.modFrXml);
            }
            else if (box == English)
            {
                _currentTranslationLanguage = TranslationLanguage.English;
                TranslationText.Text = "No translated lyrics for this piece yet";
            }
            else if (box == pickLanguage)
            {
                _currentTranslationLanguage = _lastTranslationLanguage;
            }

            _lastTranslationLanguage = _currentTranslationLanguage;
        }

        private void StudyMono(object sender, RoutedEventArgs e, Piece piece)
        {

            //musicControls(tag); // Sets up the top part of the StudyTab: title, play/stop buttons, audio selection, etc.
            var tag = piece.ID;
            MusicControls(piece);
            playpause.Click += delegate(object s, RoutedEventArgs r) { playpause_click(s, r, "mono"); };
            stop.Click += delegate(object s, RoutedEventArgs ev) { stop_Music(s, ev, "mono"); };
            OriginalText.Text = Search.getByTag(tag, SurfaceWindow1.xmlOldFr);
            // This one is hardcoded in because we don't have any English lyrics in an XML file yet.
            //TranslationText.Text = Search.getByTag(tag, SurfaceWindow1.engXml);

            // modern music notation image file
            monoImg = new Image();
            monoImg.Source = new BitmapImage(new Uri(@"..\..\music\" + "Favellandi vicium" + ".png", UriKind.Relative)); ///
            monoImg.Width = 580;
            monoImg.Height = 860;

            notesCanvas.Width = 580;
            notesCanvas.Height = 860;
            notesCanvas.Children.Add(monoImg);

            // Create mediaplayer for audio playback.
            monoPlayer = new MediaPlayer();
            monoPlayer.Open(new Uri(@"..\..\music\" + tag + ".wma", UriKind.Relative));
            monoPlayer.MediaEnded += delegate(object s, EventArgs r) { MediaEnded(s, r, "mono"); };


        }

        private void StudyPoly(object sender, RoutedEventArgs e, Piece piece)
        {
            var tag = piece.ID;
            MusicControls(piece); // Sets up the top part of the StudyTab: title, play/stop buttons, audio selection, etc.

            playpause.Click += delegate(object s, RoutedEventArgs r) { playpause_click(s, r, "poly"); };
            stop.Click += delegate(object s, RoutedEventArgs ev) { stop_Music(s, ev, "poly"); };

            OriginalText.Text = Search.getByTag(tag, SurfaceWindow1.xmlOldFr);
            switch (_currentTranslationLanguage)
            {
                case TranslationLanguage.English:
                    TranslationText.Text = "No English translated lyrics for this piece YET!";
                    break;
                case TranslationLanguage.modernFrench:
                    TranslationText.Text = piece.ModernFrench;
                    break;
            }


            motetParts = new StackPanel(); // StackPanel for the expandable parts to stack on top of eachother.
            motetParts.Orientation = Orientation.Vertical;

            fullExp = new MusicExpander("Full Score");
            


            // Motet score
            motetScore = new StackPanel();
            motetScore.Orientation = Orientation.Vertical;
            fullExp.Content = motetScore;
            fullExp.Header = "Full Score";
            fullExp.IsExpanded = true;

            // Score has several pages.
            // Create an object here called MotetPage and make one of these for each

            /**
             * GOAL FOR THIS SECTION:
             * 1. Get names of each .png file (page) for this piece of music
             * 2. Create a ScorePage object for each page - ScorePage implements Grid.
             * 3. Add each ScorePage to motetScore.
             * */

            ScorePage page = new ScorePage(tag);
            motetScore.Children.Add(page);
            
            /*
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
            */

            /**
             * GOAL FOR THIS SECTION:
             * 1. Get names of each voice for this piece of music, using the tag.
             *    Audio files must be named exactly the same way as indicated in OriginalTextXML!
             *    NOTE: Very few motets have voice parts added in as of now, so "getVoiceParts" will not work!
             * 2. Create an expander for each voice.
             * 3. Add each expander to the motetParts StackPanel.
             * */

            allVoices = new List<MusicPartExpander>();
            motetParts.Children.Add(fullExp);

            foreach (String voicePartName in Study.getVoiceParts(tag))
            {
                MusicPartExpander newMPE = new MusicPartExpander(voicePartName, tag);
                newMPE.muteTB.Checked += delegate(object s, RoutedEventArgs r) { mute_Checked(s, r, newMPE); };
                newMPE.muteTB.Unchecked += delegate(object s, RoutedEventArgs r) { mute_Unchecked(s, r, newMPE); };
                newMPE.soloTB.Checked += delegate(object s, RoutedEventArgs r) { solo_Checked(s, r, newMPE); };
                newMPE.soloTB.Unchecked += delegate(object s, RoutedEventArgs r) { solo_Unchecked(s, r, newMPE); };
                newMPE.player.MediaEnded += delegate(object s, EventArgs r) { MediaEnded(s, r, "poly"); };
                allVoices.Add(newMPE);
                motetParts.Children.Add(newMPE);
            }

            foreach (MusicPartExpander mpe in allVoices)
                mpe.otherVoices = findOtherVoices(mpe, allVoices);



            notesCanvas.Width = 560;
            notesCanvas.Height = 4000;
            notesCanvas.Children.Add(motetParts);

        }

        
        
 




        /**
         * Creates a list of the other voices for a particular voice in polyphonic music.
         * Used in many action listeners; i.e., soloing one voice means you have to mute all the other ones.
         * Sets the List<MusicPartExpander> otherVoices value from the MusicPartExpander object.
         * */
        private List<MusicPartExpander> findOtherVoices(MusicPartExpander thisVoice, List<MusicPartExpander> allVoices)
        {
            List<MusicPartExpander> otherVoices = new List<MusicPartExpander>();
            foreach (MusicPartExpander mpe in allVoices)
            {
                if (mpe != thisVoice)
                    otherVoices.Add(mpe);
            }

            return otherVoices;
        }


        /**
         * Displays the audio options list: MIDI, liveRecording.
         * */
        void showAudioOptions(object sender, RoutedEventArgs e)
        {
            StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

            selectedTab.selectAudioButton.Visibility = System.Windows.Visibility.Hidden;
            selectedTab.selectAudioListBox.Visibility = System.Windows.Visibility.Visible;
        }


        /**
         * Hides the audio options list and sets the current audio-type choice.
         * Note: This gets weird if you select "Select audio:" twice in a row
         * */
        void audioOptionSelected(object sender, RoutedEventArgs e)
        {
            StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

            ListBoxItem lbi = sender as ListBoxItem;

            if (lbi != selectAudio)
            {
                lastAudioChoice = lbi;
                selectedTab.selectAudioButton.Content = lbi.Content;
            }
            else
                selectedTab.selectAudioListBox.SelectedItem = lastAudioChoice;

            selectedTab.selectAudioButton.Visibility = System.Windows.Visibility.Visible;
            selectedTab.selectAudioListBox.Visibility = System.Windows.Visibility.Hidden;
        }

        /**
         * Manages the play/pause toggle button by switching between the two states.
         * Takes into account the differences between mono and polyphonic music (i.e., number of MusicPlayers to play/pause).
         * */
        void playpause_click(object sender, RoutedEventArgs e, String musicType)
        {
            StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

            if ((string)playpause.Content == "||")
            {
                playpause.Content = "►";
                playpause.FontSize = 35;
                if (musicType == "mono")
                {
                    if (selectedTab.monoPlayer.CanPause)
                        selectedTab.monoPlayer.Pause();
                }
                else if (musicType == "poly")
                {
                    foreach (MusicPartExpander mpe in selectedTab.allVoices)
                        mpe.player.Pause();
                }
            }
            else if ((string)playpause.Content == "►")
            {
                playpause.Content = "||";
                playpause.FontSize = 25;
                if(musicType == "mono")
                    selectedTab.monoPlayer.Play();
                else if (musicType == "poly")
                {
                    foreach (MusicPartExpander mpe in selectedTab.allVoices)
                        mpe.player.Play();
                }
            }

        }

        /**
         * Mutes a particular voice of a polyphonic piece of music.
         * */
        void mute_Checked(object sender, RoutedEventArgs e, MusicPartExpander thisExp)
        {
            StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

            thisExp.Opacity = 0.7;
            thisExp.soloTB.IsEnabled = false;

            Boolean allOtherExpsMuted = true;
            foreach (MusicPartExpander mpe in thisExp.otherVoices)
            {
                if (mpe.muteTB.IsChecked == false) // If any other voices are still audible
                    allOtherExpsMuted = false;
            }

            if (allOtherExpsMuted == true)
            {
                thisExp.player.Stop();
                foreach (MusicPartExpander mpe in thisExp.otherVoices)
                {
                    mpe.player.Stop();
                }
                selectedTab.playpause.IsEnabled = false;
                selectedTab.stop.IsEnabled = false;
                selectedTab.playpause.FontSize = 35;
                selectedTab.playpause.Content = "►";
            }
            
            thisExp.player.IsMuted = true;
        }

        /**
         * Unmutes a particular voice of a polyphonic music object.
         * */
        void mute_Unchecked(object sender, RoutedEventArgs e, MusicPartExpander thisExp)
        {
            StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

            thisExp.Opacity = 1.00;

            // If none of the other voices are soloing, then this one can enable soloing
            Boolean otherVoiceSoloing = false;
            foreach (MusicPartExpander mpe in thisExp.otherVoices)
            {
                if (mpe.soloTB.IsChecked == true)
                    otherVoiceSoloing = true;
            }

            if (otherVoiceSoloing == false)
                thisExp.soloTB.IsEnabled = true;


            selectedTab.playpause.IsEnabled = true;
            selectedTab.stop.IsEnabled = true;
            thisExp.player.IsMuted = false;

        }

        /**
         * Solos a particular voice of a polyphonic music object (i.e., mutes/disables all the other voices).
         * */
        void solo_Checked(object sender, RoutedEventArgs e, MusicPartExpander thisExp)
        {
            thisExp.BorderBrush = Brushes.YellowGreen;
            foreach (MusicPartExpander mpe in thisExp.otherVoices)
            {
                mpe.soloTB.IsEnabled = false;
                mpe.player.IsMuted = true;
            }


        }

        /**
         * Turns off solo mode for a voice in a polyphonic music object (i.e., enables/unmutes the other voices).
         * */
        void solo_Unchecked(object sender, RoutedEventArgs e, MusicPartExpander thisExp)
        {
            thisExp.BorderBrush = Brushes.Black;

            foreach (MusicPartExpander mpe in thisExp.otherVoices)
            {
                mpe.soloTB.IsEnabled = true;
                mpe.player.IsMuted = false;
            }
        }

       
        /**
         * Stops all music players, taking into account mono vs. polyphonic (bc that affects # of MusicPlayers to stop).
         * */
        void stop_Music(object sender, RoutedEventArgs e, String musicType)
        {
            StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

            selectedTab.playpause.Content = "►";
            selectedTab.playpause.FontSize = 35;

            if (musicType == "poly") // polyphonic
            {
                foreach (MusicPartExpander mpe in selectedTab.allVoices)
                    mpe.player.Stop();
            }
            else if (musicType == "mono")
                selectedTab.monoPlayer.Stop();
        }



        /**
         * Stops all MusicPlayers based on one of them ending.
         * Originally called by one voice only in a polyphonic work (Jamie set it to tenor); hopefully calling it with all voices doesn't mess anything up.
         * */
        void MediaEnded(object sender, EventArgs e, String musicType)
        {
            StudyTab selectedTab = mySideBar.tabBar.SelectedItem as StudyTab;

            selectedTab.playpause.Content = "►";
            selectedTab.playpause.FontSize = 35;
            if (musicType == "poly")
            {
                foreach (MusicPartExpander mpe in selectedTab.allVoices)
                    mpe.player.Stop();
            }
            else if (musicType == "mono")
                selectedTab.monoPlayer.Stop();
        }
    }
}
