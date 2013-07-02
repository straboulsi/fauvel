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

namespace Fauvel1
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// 
        public SurfaceWindow1()
        {
            InitializeComponent();

            Console.SetBufferSize(80, 1000);

            
            //Class1.getPoint("Te_0035-0048", 2);

            /// Console.Write("Width: " + Console.BufferWidth);
            /// Console.Write("Height: "+Console.BufferHeight);

            /// Issue: We currently have a limit of approx 300 lines displayed in console
            /// That's why we need to reset!
            ///Class1.getPoetry(1,3);
            ///Class1.go("1rIm2"); 
            ///Class1.go("5rCon1_t"); 
            ///Class1.go("Fo3v"); /// Works if you add closing </pb> tag
            ///Class1.go("Te_0035-0048");
            
            
            ///Class1.filterByVoice(2);
            ///Class1.searchEngPoetry("beast");
            ///Class1.searchLyrics("Dictus");
            ///Class1.searchPicCaptions("Fauvel");


            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();

            

        }



        private void Canvas_TouchDown(object sender, TouchEventArgs e)
        {
            Console.Write("got a touch!");
            Point touchPosition = e.TouchDevice.GetPosition(this);
            
            e.Handled = true;
        }


        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int sLine = Convert.ToInt32(startLine.Text);
            int eLine = Convert.ToInt32(endLine.Text);
            String sSelectedText = Class1.getPoetry(sLine, eLine);
            stb.Text = sSelectedText;
            stb.Opacity = 100;
            
        }

        private void Search_French(object sender, RoutedEventArgs e)
        {
            stb.Text = Class1.searchFrPoetry(SearchText.Text);
            stb.Opacity = 100;
        }

        private void Search_English(object sender, RoutedEventArgs e)
        {
            stb.Text = Class1.searchEngPoetry(SearchText.Text);
            stb.Opacity = 100;
        }

        private void Search_Lyrics(object sender, RoutedEventArgs e)
        {
            stb.Text = Class1.searchLyrics(SearchText.Text);
            stb.Opacity = 100;
        }

        private void Search_Image(object sender, RoutedEventArgs e)
        {
            stb.Text = Class1.searchPicCaptions(SearchText.Text);
            stb.Opacity = 100;
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            String sSelectedText = Class1.go(Tag.Text);
            stb.Text = sSelectedText;
            stb.Opacity = 100;
        }

        private void Fetch_English(object sender, RoutedEventArgs e)
        {
            int sLine = Convert.ToInt32(startLine.Text);
            int eLine = Convert.ToInt32(endLine.Text);
            String sSelectedText = Class1.getEnglish(sLine,eLine);
            stb.Text = sSelectedText;
            stb.Opacity = 100;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
            tb.Foreground = Brushes.Black;
        }
    }
}