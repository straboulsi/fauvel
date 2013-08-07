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
using Microsoft.Surface.Presentation.Input;
using System.Xml;
using System.Windows.Threading;
using System.ComponentModel;

namespace SurfaceApplication1
{
    class StudyTab : SideBarTab
    {
        public TextBlock studyTabHeader;
        public TextBlock studyPrompt;
        public Button mono, poly;

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
            studyPrompt.Text = "Conductus : Heu ! Quo progreditur (PM 6)";
            canvas.Children.Remove(mono);


        }
    }
}
