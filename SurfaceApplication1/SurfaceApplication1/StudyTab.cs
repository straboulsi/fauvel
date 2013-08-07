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

            // Add stuff.

            headerGrid.Children.Add(studyTabHeader);

            canvas.Children.Add(studyPrompt);
            canvas.Children.Add(mono);

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


            canvas.Children.Add(musicTitle);
            canvas.Children.Add(playpause_1);
            canvas.Children.Add(stop_1);
        }
    }
}
