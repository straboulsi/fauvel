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
    class StudyTab : SideBarTab
    {
        public TextBlock studyTabHeader, studyPrompt;

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
            Canvas.SetLeft(studyPrompt, 32);
            Canvas.SetTop(studyPrompt, 26);

            headerGrid.Children.Add(studyTabHeader);

            canvas.Children.Add(studyPrompt);
        }
    }
}
