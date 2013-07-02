using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SurfaceApplication1
{
    public static class Translate
    {
        public static Brush textBrush = (Brush)(new BrushConverter().ConvertFrom("#663311"));
        public static Brush backBrush = (Brush)(new BrushConverter().ConvertFrom("#CCE0D0B0"));


        public static String getPoetry(int firstLine, int lastLine)
        {

            String toDisplay = "";
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load("XMLFinalContentFile.xml");


                XmlNode foundNode;

                for (int i = firstLine; i <= lastLine; i++)
                {
                    foundNode = xml.DocumentElement.SelectSingleNode("//lg/l[@n='" + i + "']");
                    toDisplay += foundNode.InnerText + "\r\n";
                }


            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return toDisplay;
        }

        public static Grid getGrid(double x, double y, double width, double height, TextBlock t)
        {
            Grid g = new Grid();

            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(x, GridUnitType.Star);
            ColumnDefinition c2 = new ColumnDefinition();
            c2.Width = new GridLength(width, GridUnitType.Star);
            ColumnDefinition c3 = new ColumnDefinition();
            c3.Width = new GridLength(SurfaceWindow1.maxPageWidth - x - width, GridUnitType.Star);
            RowDefinition r1 = new RowDefinition();
            r1.Height = new GridLength(y, GridUnitType.Star);
            RowDefinition r2 = new RowDefinition();
            r2.Height = new GridLength(height, GridUnitType.Star);
            RowDefinition r3 = new RowDefinition();
            r3.Height = new GridLength(SurfaceWindow1.maxPageHeight - y - height, GridUnitType.Star);

            g.ColumnDefinitions.Add(c1);
            g.ColumnDefinitions.Add(c2);
            g.ColumnDefinitions.Add(c3);
            g.RowDefinitions.Add(r1);
            g.RowDefinitions.Add(r2);
            g.RowDefinitions.Add(r3);

            Viewbox v = new Viewbox();
            Grid.SetRow(v, 1);
            Grid.SetColumn(v, 1);
            g.Children.Add(v);
            v.Child = t;
            v.Stretch = Stretch.Uniform;

            return g;
        }

        /**
         * Creates an ArrayList of TranslationBox objects when given a folio page.
         * Consults the Content (Old French), Layout, and English XML files. 
         * Calls on other methods in this class to fetch English, French, or coordinates.
         **/
        public static List<TranslationBox> getBoxes(String page)
        {

            List<TranslationBox> boxes = new List<TranslationBox>();

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load("XMLFinalContentFile.xml");

                XmlNodeList foundNode;

                page = page.Substring(2);
                foundNode = xml.DocumentElement.SelectNodes("//pb[@facs='#" + page + "']/lg");

                foreach (XmlNode x in foundNode)
                {

                    String s = x.Attributes["id"].Value;
                    int index = s.IndexOf("_");
                    int mid = s.IndexOf("-");

                    int start = Convert.ToInt32(s.Substring(index + 1, 4));
                    int end = Convert.ToInt32(s.Substring(mid + 1));

                    boxes.Add(new TranslationBox(s, getPoetry(start, end), getEnglish(start, end), getPoint(s, 1), getPoint(s, 2)));
                }

                foreach (TranslationBox tb in boxes)
                {
                    Console.Write(tb.tag);
                }

                Console.Read();

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }
            return boxes;
        }



        /**
         * Fetches top left and bottom right coordinates from Layout XML file when given tag id of object.
         * The int whichPt should = 1 if you want top left point and 2 if you want bottom right.
         **/
        public static Point getPoint(String tag, int whichPt)
        {
            Point TL = new Point();

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load("layout1.xml");

                XmlNodeList x = xml.DocumentElement.SelectNodes("//surface/zone");
                foreach (XmlNode xn in x)
                {
                    if ((xn.Attributes["id"].Value).Equals(tag))
                    {
                        if (whichPt == 1)
                            TL = new Point(Convert.ToInt32(xn.Attributes["ulx"].Value), Convert.ToInt32(xn.Attributes["uly"].Value));
                        else if (whichPt == 2)
                            TL = new Point(Convert.ToInt32(xn.Attributes["lrx"].Value), Convert.ToInt32(xn.Attributes["lry"].Value));
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }
            return TL;
        }

        /**
         * Fetches English translation for a section of poetry, given starting and ending line numbers.
         **/
        public static String getEnglish(int start, int end)
        {
            String toDisplay = "";

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load("EnglishXML.xml");

                XmlNode foundNode;
                for (int i = start; i <= end; i++)
                {
                    foundNode = xml.DocumentElement.SelectSingleNode("//lg/l[@n='" + i + "']");
                    toDisplay += foundNode.InnerText + "\r\n";
                }
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return toDisplay;
        }

        /**
         *  <summary>
         *  This method searches for an id value for images, music, and poetry sections
         *  <figure id="1rIm2"> Searching for image
         *  <p id="1rMo2"> Searching for music
         *  <lg> searching by Te_xxxx-xxxx
         *  </summary>
         *  Searching Fo1v or some other page gives you all contents of that page.
         *  <param name="str">The value of the id</param>
        **/
        public static String go(String str)
        {
            ///Console.Write("Input: "+ str+"\r\n");
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load("XMLFinalContentFile.xml");
                XmlNode foundNode;

                if (str.Contains("Im"))
                {
                    foundNode = xml.DocumentElement.SelectSingleNode("//figure[@id='" + str + "']");
                    str += foundNode.InnerText;
                }
                else if (str.StartsWith("Te"))
                {
                    foundNode = xml.DocumentElement.SelectSingleNode("//lg[@id='" + str + "']");
                    XmlNodeList lineByLine = foundNode.SelectNodes("l");
                    foreach (XmlNode x in lineByLine)
                        str += x.InnerText + "\r\n";
                }
                else if (str.StartsWith("Fo"))
                {
                    String page = str.Substring(2);
                    foundNode = xml.DocumentElement.SelectSingleNode("//pb[@facs='#" + page + "']");
                    str += (foundNode.InnerXml);
                }
                else
                {
                    /// Note: To select voices that don't have <dc>, add second level and select ("//v[not(dc)]")
                    foundNode = xml.DocumentElement.SelectSingleNode("//p[@id='" + str + "']");
                    str += foundNode.InnerText;
                }

                //Console.Write(foundNode.InnerText + "\r\n");

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return str;
        }

        public static void filterByVoice(int voiceNum)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load("XMLFinalContentFile.xml");

            XmlNodeList musics = xml.DocumentElement.SelectNodes("//p[(nv)]");

            foreach (XmlNode xn in musics)
            {
                XmlNode testNode = xn.SelectSingleNode("nv");
                String str = testNode.InnerText;
                int intVoiceCount = Convert.ToInt32(str);

                if (intVoiceCount == voiceNum)
                    Console.Write(xn.InnerText);

            }

            Console.Read();
        }
    }
}
