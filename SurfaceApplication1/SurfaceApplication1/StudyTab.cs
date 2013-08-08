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
     * Primary Coder: Jamie Chong
     * */
    class StudyTab : SideBarTab
    {
        public TextBlock studyTabHeader;
        public TextBlock studyPrompt;
        public Button mono, poly;
        public TextBlock musicTitle;
        public Button playpause_1, stop_1, playpause_2, stop_2;
        public TabControl display;
        public TabItem notesTab, mod_frenchTab, engTab;
        public Canvas notesCanvas, notesTabCanvas;
        public Image musicImg;
        public TextBlock mod_frenchText, engText;
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
        }

        private void study_Mono(object sender, RoutedEventArgs e)
        {
            canvas.Children.Remove(studyPrompt);
            canvas.Children.Remove(mono);

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

            musicImg = new Image();
            musicImg.Source = new BitmapImage(new Uri(@"..\..\musicz\2rCo1.png", UriKind.Relative));

            mod_frenchTab = new TabItem();
            mod_frenchTab.Header = "Modern French";
            mod_frenchTab.Height = 50;
            mod_frenchTab.Width = 175;
            mod_frenchTab.FontSize = 20;

            mod_frenchText = new TextBlock();
            mod_frenchTab.Content = mod_frenchText;
            mod_frenchText.Text = Search.getByTag("2rCon1_t", SurfaceWindow1.modFrXml);

            engTab = new TabItem();
            engTab.Header = "English";
            engTab.Height = 50;
            engTab.Width = 175;
            engTab.FontSize = 20;

            engText = new TextBlock();
            engTab.Content = engText;
            engText.Text = "Oh, how far transgression\nis spreading!\nVirtue is dislodged\nfrom the sanctuary.\nNow Christ is dragged\nto a new tribunal,\nwith Peter using\nthe sword of Pilate.\nRelying on the counsel\nof Fauvel,\none comes to grief;\nthe celestial legion\njustly complains.\nTherefore it begs\nthe Father and the Son\nthat for a remedy\nfor all this\nimmediately\nthe fostering Spirit provide.";

            display.Items.Add(notesTab);
            display.Items.Add(mod_frenchTab);
            display.Items.Add(engTab);

            canvas.Children.Add(musicTitle);
            canvas.Children.Add(playpause_1);
            canvas.Children.Add(stop_1);
            canvas.Children.Add(display);
        }
    }
}
