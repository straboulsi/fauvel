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
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace Fauvel1
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {

        XmlDocument xml;
        XmlDocument engXml;
        XmlDocument layoutXml;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// 
        public SurfaceWindow1()
        {
            InitializeComponent();

            

            ///Class1.getBoxes("Fo1v");

            /// NB: Default limit is approx 300 lines displayed in console
            /// If you need to print more, use Console.SetBufferSize(width, height);
           /// Console.SetBufferSize(300, 1200);
            ///Class1.getPoint("Te_0035-0048", 2);
            ///Stopwatch sw = new Stopwatch();
            ///sw.Start();
           //Console.Write("Before method " +DateTime.Now+" "+DateTime.Now.Millisecond);

            xml = new XmlDocument();
            xml.Load("XMLFinalContentFile.xml");
            engXml = new XmlDocument();
            engXml.Load("EnglishXML.xml");
            layoutXml = new XmlDocument();
            layoutXml.Load("layout1.xml");


            ///Class1.getBoxes("Fo1v", xml, engXml, layoutXml);
           

            //Console.Write(Class1.getEnglish(500,1000, engXml)); 

            // Lines 4500-5500 took 10.91 sec using next sibling and 10.79 sec using line by line search and 0.14 sec using selectNodes
            // Lines 1500-2500 took 4.64 sec using next sibling and 4.56 sec using line by line search and 0.14 sec using selectNodes
            // Lines 1500-3500 took 10.02 sec using next sibling and 9.82 sec using line by line search and 0.38 sec using selectNodes
            // Lines 1500-1570 took 0.30 sec using next sibling and 0.29 sec using line by line search and 0.07 sec using selectNodes
            // Lines 500-5500 took 32.3 sec using next sibling and 31 sec using line by line search and 3.31 sec using selectNodes
            ///Console.Write(Class1.getEnglish(1000, 1500));
            /**
            sw.Stop();
            TimeSpan ts = sw.Elapsed;

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            **/
          
            ///Class1.go("1rIm2"); 
            ///Class1.go("1rMo3_t"); 
            ///Class1.go("34vLa1_t");

            ///Class1.go("Fo3v"); /// Works if you add closing </pb> tag
            ///Class1.go("Te_0035-0048");

            
            ///Class1.filterByVoice(2);
            ///Class1.searchFrPoetry("Torcher");
            ///Class1.searchEngPoetry("beast");
            ///Class1.searchLyrics("Dictus");
            ///Class1.searchPicCaptions("Fauvel");



            ///Console.Read();
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


        private void Enter_Clicked(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Button_Click2(sender, e);
                e.Handled = true;
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int sLine = Convert.ToInt32(startLine.Text);
            int eLine = Convert.ToInt32(endLine.Text);
            String sSelectedText = "";

            if (sLine > eLine)
                sSelectedText = "Please enter the lower number on the left!";
            else
                sSelectedText = Class1.getPoetry(sLine, eLine, xml);
            stb.Text = sSelectedText;
            stb.Opacity = 100;
            
        }

        private void Search_French(object sender, RoutedEventArgs e)
        {
            int caseType = 0;
            int wordType = 0;
            if (CaseSensitive.IsChecked == true)
                caseType = 1;
            if (WordSensitive.IsChecked == true)
                wordType = 1;
            stb.Text = Class1.searchFrPoetry(SearchText.Text, caseType, wordType, xml);
            stb.Opacity = 100;
        }

        private void Search_English(object sender, RoutedEventArgs e)
        {
            int caseType = 0;
            int wordType = 0;
            if (CaseSensitive.IsChecked == true)
                caseType = 1;
            if (WordSensitive.IsChecked == true)
                wordType = 1;
            
            stb.Text = Class1.searchEngPoetry(SearchText.Text, caseType, wordType, engXml);
            stb.Opacity = 100;
        }

        private void Search_Lyrics(object sender, RoutedEventArgs e)
        {
            int caseType = 0;
            int wordType = 0;
            if (CaseSensitive.IsChecked == true)
                caseType = 1;
            if (WordSensitive.IsChecked == true)
                wordType = 1;
            stb.Text = Class1.searchLyrics(SearchText.Text, caseType, wordType, xml);
            stb.Opacity = 100;
        }

        private void Search_Image(object sender, RoutedEventArgs e)
        {
            int caseType = 0;
            int wordType = 0;
            if (CaseSensitive.IsChecked == true)
                caseType = 1;
            if (WordSensitive.IsChecked == true)
                wordType = 1; 
            stb.Text = Class1.searchPicCaptions(SearchText.Text, caseType, wordType, xml);
            stb.Opacity = 100;
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            String sSelectedText = Class1.go(Tag.Text, xml);
            stb.Text = sSelectedText;
            stb.Opacity = 100;
        }

        private void Fetch_English(object sender, RoutedEventArgs e)
        {
            int sLine = Convert.ToInt32(startLine.Text);
            int eLine = Convert.ToInt32(endLine.Text);
            String sSelectedText = "";
            if(sLine > eLine)
                sSelectedText = "Please enter the lower number on the left!";
            else
                sSelectedText = Class1.getEnglish(sLine,eLine, engXml);
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