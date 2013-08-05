using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using System.IO;
using System.Globalization;

namespace SurfaceApplication1
{

    /**
     * This class fetches information from the various XML files.
     * It can search for a section of poetry, music lyrics, or image description by tag id.
     * It can also return specific lines of poetry by line number, both in Old French and in English.
     * This class also makes an ArrayList of TranslationBoxes when given a page of Fauvel.
     **/
    public static class Translate
    {
        public static Brush textBrush = (Brush)(new BrushConverter().ConvertFrom("#663311"));
        public static Brush backBrush = (Brush)(new BrushConverter().ConvertFrom("#CCE0D0B0"));
        public static Brush blockBrush = (Brush)(new BrushConverter().ConvertFrom("#ccffffff"));
        public static Brush blockFillerBrush = (Brush)(new BrushConverter().ConvertFrom("#33000000"));
        public static CompareInfo myComp = CultureInfo.InvariantCulture.CompareInfo;

        public static Grid getGrid(TranslationBox tb, TextBlock t)
        {
            double width, x, y, height;
            x = tb.getTopLeft().X;
            y = tb.getTopLeft().Y;
            width = (tb.getBottomRight().X - tb.getTopLeft().X);
            height = (tb.getBottomRight().Y - tb.getTopLeft().Y);

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

            t.Foreground = textBrush;
            t.Background = backBrush;
            t.FontSize = TranslationBox.minFontSize;
            t.TextWrapping = TextWrapping.NoWrap;
            t.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
            Grid.SetRow(t, 1);
            Grid.SetColumn(t, 1);
            g.Children.Add(t);
            t.LineHeight = tb.lineHeight;
            
            return g;
        }

        public static Grid getGrid(BoundingBox bb)
        {
            double width, x, y, height;
            x = bb.topL.X;
            y = bb.topL.Y;
            width = (bb.bottomR.X - bb.topL.X);
            height = (bb.bottomR.Y - bb.topL.Y);

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

            Border B = new Border();
            B.BorderBrush = blockBrush;
            B.BorderThickness = new Thickness(2);
            Grid filla = new Grid();
            Grid.SetRow(B, 1);
            Grid.SetColumn(B, 1);
            g.Children.Add(B);
            B.Child = filla;
            filla.Background = blockFillerBrush;

            return g;
        }



        // Allows for search specifications, like ignore case vs. case sensitive
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            // Note: call method as stringToSearchIn.Contains(stringToFind, StringComparison.OrdinalIgnoreCase));
            return source.IndexOf(toCheck, comp) >= 0;
        }


        /** Used to figure out whether a Contains search will use an Ordinal or OrdinalIgnoreCase
         *  Default int of 0 is to ignore case (and return more search findings).
         **/
        public static Boolean foundBySpecifiedCase(String toFind, String toSearchIn, int caseSensitive)
        {
            if (caseSensitive == 0)
                return toSearchIn.Contains(toFind, StringComparison.OrdinalIgnoreCase);
            else
                return toSearchIn.Contains(toFind);
        }
        


        /**
         * Checks whether the search result is the exact word or the found word contains the search.
         * For example, whether a search for "corn" will return "unicorn".
         * */
        public static Boolean foundBySpecifiedWord(String toFind, String toSearchIn, int wordSensitive)
        {
            if (wordSensitive == 1) // Whole word must match  
                return Regex.IsMatch(toSearchIn, string.Format(@"\b{0}\b", Regex.Escape(toFind)));
            else
                return true;
        }





        /**Takes in start and end line numbers.
         * Returns String of Old French poetry.
         * This is good bc Surface could send the int values of the first/last lines highlighted by user
         * The overlay is then flexible and does not limit to translating entire sections of poetry
         * <param name="firstLine">First line of target poetry section</param>
         * <param name="lastLine">Last line of target poetry section</param>
        **/
        public static String getOldFrPoetry(int firstLine, int lastLine, XmlDocument xml)
        {

            String toDisplay = "";
            try
            {
                XmlNodeList foundNodes = xml.DocumentElement.SelectNodes("//lg/l[@n>=" + firstLine + "and @n<=" + lastLine + "]");
                foreach (XmlNode xn in foundNodes)
                {
                    if (xn.HasChildNodes)
                        toDisplay += xn.LastChild.InnerText.Trim() + "\r\n";
                    else
                        toDisplay += xn.InnerText.Trim() + "\r\n";
                }

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }


            toDisplay = toDisplay.TrimEnd('\r', '\n');

            return toDisplay;
        }





        /**
         * Creates an ArrayList of TranslationBox objects when given a folio page.
         * Consults the Content (Old French), Layout, and English XML files. 
         * Calls on other methods in this class to fetch English, French, or coordinates.
         * Expects folio without the "Fo" - i.e. 1v, 35r, 28tr
         **/
        public static List<TranslationBox> getTranslationOverlay(String page, XmlDocument xml, XmlDocument modFrXml, XmlDocument engXml, XmlDocument layoutXml)
        {

            List<TranslationBox> boxes = new List<TranslationBox>();

            try
            {

                XmlNodeList foundNode = layoutXml.DocumentElement.SelectNodes("//surface[@id='" + page + "']/zone");

                foreach (XmlNode xn in foundNode)
                {
                    String tag = xn.Attributes["id"].Value;
                    if (tag.StartsWith("Te"))
                    {
                        XmlNodeList boxList = xn.SelectNodes("box");
                        foreach (XmlNode node in boxList)
                        {
                            String s = node.Attributes["id"].Value; // Returns "Te_4500-4590" or something
                            int index = s.IndexOf("_");
                            int mid = s.IndexOf("-");
                            int start = Convert.ToInt32(s.Substring(index + 1, 4));
                            int end = Convert.ToInt32(s.Substring(mid + 1));

                            boxes.Add(new TranslationBox(s, getOldFrPoetry(start, end, xml), getModernFrench(start, end, modFrXml), getEnglish(start, end, engXml), getPoint(s, 1, layoutXml), getPoint(s, 2, layoutXml)));
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return boxes;
        }



        /**
         * Returns a List of BoundingBox objects for an indicated page.
         * Primarily used to check whether coordinates indicated in the LayoutXML are accurate.
         * */
        public static List<BoundingBox> getGhostBoxes(String page, XmlDocument layoutXml)
        {
            List<BoundingBox> boxes = new List<BoundingBox>();

            try
            {
                XmlNodeList foundNodes = layoutXml.DocumentElement.SelectNodes("//surface[@id='" + page + "']/zone/box");
                foreach (XmlNode node in foundNodes)
                {
                    String tag = node.ParentNode.Attributes["id"].Value;
                    
                    Point topL = new Point(Convert.ToDouble(node.Attributes["ulx"].Value), Convert.ToDouble(node.Attributes["uly"].Value));
                    Point bottomR = new Point(Convert.ToDouble(node.Attributes["lrx"].Value), Convert.ToDouble(node.Attributes["lry"].Value));
                    BoundingBox newBB = new BoundingBox(tag, topL, bottomR);
                    boxes.Add(newBB);
                }

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
        public static Point getPoint(String tag, int whichPt, XmlDocument layoutXml)
        {
            Point TL = new Point();

            try
            {
                XmlNode xn;

                if (tag.StartsWith("Te"))
                {
                    xn = layoutXml.DocumentElement.SelectSingleNode("//surface/zone/box[@id='" + tag + "']");
                    if (whichPt == 1)
                        TL = new Point(Convert.ToInt32(xn.Attributes["ulx"].Value), Convert.ToInt32(xn.Attributes["uly"].Value));
                    else if (whichPt == 2)
                        TL = new Point(Convert.ToInt32(xn.Attributes["lrx"].Value), Convert.ToInt32(xn.Attributes["lry"].Value));
                }
                else
                {
                    xn = layoutXml.DocumentElement.SelectSingleNode("//surface/zone[@id='" + tag + "']");

                    if (whichPt == 1)
                        TL = new Point(Convert.ToInt32(xn.FirstChild.Attributes["ulx"].Value), Convert.ToInt32(xn.FirstChild.Attributes["uly"].Value));
                    else if (whichPt == 2)
                        TL = new Point(Convert.ToInt32(xn.FirstChild.Attributes["lrx"].Value), Convert.ToInt32(xn.FirstChild.Attributes["lry"].Value));
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
         * Fetches Points from LayoutXML coordinates. 
         * Similar to getPoint, but specially designed to work with poetry lines.
         * For poetry, coordinates are not recorded for every line, but only for each poetry chunk.
         * */
        public static Point getLinePoint(String lineNum, int whichPt, XmlDocument layoutXml)
        {
            Point TL = new Point();

            try
            {

                XmlNode xn = layoutXml.DocumentElement.SelectSingleNode("//l[@n='" + lineNum + "']");
                XmlNode section = xn.ParentNode;

                if (whichPt == 1)
                    TL = new Point(Convert.ToInt32(section.FirstChild.Attributes["ulx"].Value), Convert.ToInt32(section.FirstChild.Attributes["uly"].Value));
                else if (whichPt == 2)
                    TL = new Point(Convert.ToInt32(section.FirstChild.Attributes["lrx"].Value), Convert.ToInt32(section.FirstChild.Attributes["lry"].Value));

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
        public static String getEnglish(int start, int end, XmlDocument engXml)
        {
            String toDisplay = "";

            try
            {
                XmlNodeList foundNodes = engXml.DocumentElement.SelectNodes("//lg/l[@n>=" + start + "and @n<=" + end + "]");
                foreach (XmlNode xn in foundNodes)
                    toDisplay += xn.InnerText.Trim() + "\r\n";

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            toDisplay = toDisplay.TrimEnd('\r', '\n');

            return toDisplay;
        }


        /**
         * Fetches Modern French translation for a section of poetry, given starting and ending line numbers.
         **/
        public static String getModernFrench(int start, int end, XmlDocument modFrXml)
        {
            String toDisplay = "";

            try
            {
                XmlNodeList foundNodes = modFrXml.DocumentElement.SelectNodes("//lg/l[@n>=" + start + "and @n<=" + end + "]");
                foreach (XmlNode xn in foundNodes)
                    toDisplay += xn.InnerText.Trim() + "\r\n";
            }
            catch (Exception e)
            {
                ///toDisplay = "Can't find the English.. Try again?";
                Console.Write(e.StackTrace);
                Console.Read();
            }

            toDisplay = toDisplay.TrimEnd('\r', '\n');

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
        public static String getByTag(String str, XmlDocument xml)
        {
            String toDisplay = "";
            try
            {
                XmlNode foundNode;

                if (str.Contains("Im"))
                {
                    foundNode = xml.DocumentElement.SelectSingleNode("//figure[@id='" + str + "']");
                    toDisplay += foundNode.InnerText.Trim();
                }
                else if (str.StartsWith("Te"))
                {
                    foundNode = xml.DocumentElement.SelectSingleNode("//lg[@id='" + str + "']");
                    XmlNodeList lineByLine = foundNode.SelectNodes("l");
                    foreach (XmlNode x in lineByLine)
                    {
                        XmlNode newX = x.RemoveChild(x.LastChild); // Removes the drop cap inner text
                        toDisplay += newX.InnerText.Trim() + "\r\n";
                    }
                }
                else if (str.StartsWith("Fo"))
                {
                    String page = str.Substring(2);
                    foundNode = xml.DocumentElement.SelectSingleNode("//pb[@facs='#" + page + "']");
                    toDisplay += foundNode.InnerXml;
                }
                else // Select music objects
                {

                    /// Note: To select voices that don't have <dc>, add second level and select ("//v[not(dc)]")
                    foundNode = xml.DocumentElement.SelectSingleNode("//p[@id='" + str + "']");
                    XmlNodeList tbRemoved;

                    tbRemoved = foundNode.SelectNodes("cp");
                    foreach (XmlNode xn in tbRemoved)
                        foundNode.RemoveChild(xn);

                    tbRemoved = foundNode.SelectNodes("nv");
                    foreach (XmlNode xn in tbRemoved)
                        foundNode.RemoveChild(xn);


                    if (str.Substring(2, 2).Equals("Mo"))
                    {
                        XmlNodeList voices = foundNode.SelectNodes("v");
                        foreach (XmlNode voice in voices)
                        {
                            tbRemoved = voice.SelectNodes("dc");
                            foreach (XmlNode xn in tbRemoved)
                                voice.RemoveChild(xn);
                        }
                    }

                    else
                    {
                        tbRemoved = foundNode.SelectNodes("dc");
                        foreach (XmlNode xn in tbRemoved)
                            foundNode.RemoveChild(xn);
                    }

                    toDisplay += foundNode.InnerText.Trim();
                }


            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            toDisplay = toDisplay.TrimEnd('\r', '\n');
            return toDisplay;

        }


        /**
         * Fetches Old French text for a section of poetry, given starting and ending line numbers.
         **/
        public static List<SearchResult> searchOldFrPoetry(String search, int caseSensitive, int wordSensitive, XmlDocument xml, XmlDocument engXml, XmlDocument layoutXml)
        {

            List<SearchResult> results = new List<SearchResult>();

            try
            {
                XmlNodeList xnl = xml.DocumentElement.SelectNodes("//lg/l");

                //int numFound = 0;


                foreach (XmlNode xn in xnl)
                {

                    if (foundBySpecifiedCase(search, xn.InnerText, caseSensitive) && foundBySpecifiedWord(search, xn.InnerText, wordSensitive))
                    {
                        SearchResult newResult = new SearchResult();

                        // Gets rid of drop caps
                        if (xn.ChildNodes.Count > 1)
                            xn.RemoveChild(xn.FirstChild);

                        //numFound++;
                        String lineNum = xn.Attributes["n"].Value;
                        newResult.lineNum = Convert.ToInt32(lineNum);
                        newResult.resultType = 1;
                        XmlNode page = xn.ParentNode.ParentNode;
                        String pageNum = page.Attributes["facs"].Value;
                        pageNum = "Fo" + pageNum.Substring(1);
                        newResult.folio = pageNum;

                        String resultLine = getOldFrPoetry(newResult.lineNum, newResult.lineNum, xml);
                        String str1 = resultLine.Substring(0, myComp.IndexOf(resultLine, search, CompareOptions.IgnoreCase));
                        String str2 = resultLine.Substring(myComp.IndexOf(resultLine, search, CompareOptions.IgnoreCase) + search.Length);
                        String lineInfo = "\r\n\r\nLines " + (newResult.lineNum - 4) + " to " + (newResult.lineNum + 4);

                        newResult.excerpt1 = getOldFrPoetry(newResult.lineNum - 4, newResult.lineNum - 1, xml) + "\r\n" + str1;
                        newResult.excerpt2 = search;
                        newResult.excerpt3 = str2 + "\r\n" + getOldFrPoetry(newResult.lineNum + 1, newResult.lineNum + 4, xml) + lineInfo;
                        newResult.text1 = xn.InnerText.Trim();
                        newResult.text2 = getEnglish(newResult.lineNum, newResult.lineNum, engXml);
                        //newResult.thumbnail = convertImage(Thumbnailer.getThumbnail(getTagByLineNum(newResult.lineNum, layoutXml)));
                        results.Add(newResult);
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return results;
        }


        /**
         * Searches for text in Modern French poetry. 
         * */
        public static List<SearchResult> searchModFrPoetry(String search, int caseSensitive, int wordSensitive, XmlDocument modFrXml, XmlDocument engXml, XmlDocument layoutXml)
        {

            List<SearchResult> results = new List<SearchResult>();

            try
            {
                XmlNodeList xnl = modFrXml.DocumentElement.SelectNodes("//lg/l");
                //int numFound = 0;


                foreach (XmlNode xn in xnl)
                {

                    if (foundBySpecifiedCase(search, xn.InnerText, caseSensitive) && foundBySpecifiedWord(search, xn.InnerText, wordSensitive))
                    {
                        SearchResult newResult = new SearchResult();

                        // Gets rid of drop caps
                        if (xn.ChildNodes.Count > 1)
                            xn.RemoveChild(xn.FirstChild);

                        //numFound++;
                        String lineNum = xn.Attributes["n"].Value;
                        newResult.lineNum = Convert.ToInt32(lineNum);
                        newResult.resultType = 1;
                        XmlNode page = xn.ParentNode.ParentNode;
                        String pageNum = page.Attributes["facs"].Value;
                        pageNum = "Fo" + pageNum.Substring(1);
                        newResult.folio = pageNum;

                        String resultLine = getOldFrPoetry(newResult.lineNum, newResult.lineNum, modFrXml);
                        String str1 = resultLine.Substring(0, myComp.IndexOf(resultLine, search, CompareOptions.IgnoreCase));
                        String str2 = resultLine.Substring(myComp.IndexOf(resultLine, search, CompareOptions.IgnoreCase) + search.Length);
                        String lineInfo = "\r\n\r\nLines " + (newResult.lineNum - 3) + " to " + (newResult.lineNum + 3);

                        newResult.excerpt1 = getOldFrPoetry(newResult.lineNum - 3, newResult.lineNum - 1, modFrXml) + "\r\n" + str1;
                        newResult.excerpt2 = search;
                        newResult.excerpt3 = str2 + "\r\n" + getOldFrPoetry(newResult.lineNum + 1, newResult.lineNum + 3, modFrXml) + lineInfo;
                        newResult.text1 = xn.InnerText.Trim();
                        newResult.text2 = getEnglish(newResult.lineNum, newResult.lineNum, engXml);
                        //newResult.thumbnail = convertImage(Thumbnailer.getThumbnail(getTagByLineNum(newResult.lineNum, layoutXml)));
                        
                        results.Add(newResult);
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return results;
        }



        /**
          * Searches for text in English translation of the poetry. 
          * */
        public static List<SearchResult> searchEngPoetry(String search, int caseSensitive, int wordSensitive, XmlDocument xml, XmlDocument engXml, XmlDocument layoutXml)
        {
            List<SearchResult> results = new List<SearchResult>();

            try
            {
                XmlNodeList xnl = engXml.DocumentElement.SelectNodes("//lg/l");

                //int numFound = 0;

                foreach (XmlNode xn in xnl)
                {

                    if (foundBySpecifiedCase(search, xn.InnerText, caseSensitive) && foundBySpecifiedWord(search, xn.InnerText, wordSensitive))
                    {
                        SearchResult newResult = new SearchResult();
                        //numFound++;
                        String lineNum = xn.Attributes["n"].Value;
                        newResult.lineNum = Convert.ToInt32(lineNum);
                        newResult.resultType = 1;
                        newResult.text1 = xn.InnerText.Trim();
                        newResult.text2 = getOldFrPoetry(newResult.lineNum, newResult.lineNum, xml);
                        newResult.folio = getPageByLineNum(newResult.lineNum, xml);
                        String resultLine = getEnglish(newResult.lineNum, newResult.lineNum, engXml);
                        String str1 = resultLine.Substring(0, myComp.IndexOf(resultLine, search, CompareOptions.IgnoreCase));
                        String str2 = resultLine.Substring(myComp.IndexOf(resultLine, search, CompareOptions.IgnoreCase) + search.Length);
                        String lineInfo = "\r\n\r\nLines " + (newResult.lineNum - 4) + " to " + (newResult.lineNum + 4);

                        newResult.excerpt1 = getEnglish(newResult.lineNum - 4, newResult.lineNum - 1, xml) + "\r\n" + str1;
                        newResult.excerpt2 = search;
                        newResult.excerpt3 = str2 + "\r\n" + getEnglish(newResult.lineNum + 1, newResult.lineNum + 4, xml) + lineInfo;
                        //newResult.thumbnail = convertImage(Thumbnailer.getThumbnail(getTagByLineNum(newResult.lineNum, layoutXml)));
                        results.Add(newResult);
                    }
                }

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return results;
        }


        /**
         * Returns the tag of an entire chunk of poetry when given a line number of poetry.
         * */
        public static String getTagByLineNum(int lineNum, XmlDocument layoutXml)
        {
            String tag = "";

            try
            {
                XmlNode xn = layoutXml.DocumentElement.SelectSingleNode("//zone/l[@n="+lineNum+"]");
                tag = xn.ParentNode.Attributes["id"].Value;
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return tag;
        }

        /**
         * Returns the name of the page that a given line of poetry is on.
         * */
        public static String getPageByLineNum(int lineNum, XmlDocument xml)
        {
            String folio = "Fo";

            try
            {
                XmlNode xnl = xml.DocumentElement.SelectSingleNode("//lg/l[@n=" + lineNum + "]");
                String folioTemp = xnl.ParentNode.ParentNode.Attributes["facs"].Value;
                folioTemp = folioTemp.Substring(1);
                folio += folioTemp;
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return folio;
        }


        /**
         * Returns the name of the page that a given object (found by tag) is on.
         * lg, p, figure = 1, 2, 3
         * */
        public static String getPageByTag(String tag, int type, XmlDocument xml)
        {
            String folio = "Fo";
            String tagType = "";
            if (type == 1)
                tagType = "lg";
            else if (type == 2)
                tagType = "p";
            else if (type == 3)
                tagType = "figure";

            try
            {
                XmlNode xnl = xml.DocumentElement.SelectSingleNode("//" + tagType + "[@id='" + tag + "']");
                String folioTemp = xnl.ParentNode.Attributes["facs"].Value;
                folioTemp = folioTemp.Substring(1);
                folio += folioTemp;
            }

            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return folio;

        }


        /**
         * Searches for text in lyrics of music.
         * */
        public static List<SearchResult> searchLyrics(String search, int caseSensitive, int wordSensitive, XmlDocument whichXml, XmlDocument layoutXml)
        {
            List<SearchResult> results = new List<SearchResult>();

            try
            {
                XmlNodeList xnl = whichXml.DocumentElement.SelectNodes("//p");

                foreach (XmlNode node in xnl)
                {
                    if (foundBySpecifiedCase(search, node.InnerText, caseSensitive) && foundBySpecifiedWord(search, node.InnerText, wordSensitive))
                    {
                        XmlNode xn = lyricsOnly(node);

                        SearchResult newResult = new SearchResult();
                        newResult.resultType = 2;
                        String str = xn.InnerText;
                        String[] allLyrics = str.Trim().Split(new String[] { "\r\n", "\n" }, StringSplitOptions.None);


                        int lyricLineNum = -5;
                        for (int i = 0; i < allLyrics.Length; i++)
                        {
                            String thisLine = allLyrics[i];
                            if (foundBySpecifiedCase(search, allLyrics[i], caseSensitive) && foundBySpecifiedWord(search, allLyrics[i], wordSensitive))
                            {
                                lyricLineNum = i;
                                break;
                            }
                        }

                        int firstLine = lyricLineNum - 3;
                        int lastLine = lyricLineNum + 3;
                        if (firstLine < 0)
                            firstLine = 0;
                        if (lastLine >= allLyrics.Length)
                            lastLine = allLyrics.Length - 1;

                        String excerpt = "";

                        if (firstLine > 0)
                            excerpt += "...\r\n";

                        for (int i = firstLine; i <= lastLine; i++)
                        {
                            if(!allLyrics[i].Trim().StartsWith("#"))
                                excerpt += allLyrics[i].Trim() + "\r\n";
                        }

                        if (lastLine != allLyrics.Length - 1)
                            excerpt += "...";

                        

                        newResult.excerpt1 = excerpt.Substring(0, myComp.IndexOf(excerpt, search, CompareOptions.IgnoreCase));
                        newResult.excerpt2 = search;
                        newResult.excerpt3 = excerpt.Substring(myComp.IndexOf(excerpt, search, CompareOptions.IgnoreCase) + search.Length);

                        newResult.text1 = allLyrics[0];
                        newResult.tag = xn.Attributes["id"].Value;
                        newResult.folio = "Fo" + (xn.ParentNode.Attributes["facs"].Value).Substring(1);
                        newResult.resultType = 2;
                        
                        results.Add(newResult);
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return results;
        }




        /**
         * Gets rid of all the extraneous text in a node of lyrics.
         * Filters out all cps, dcs, nvs, etc. 
         * */
        public static XmlNode lyricsOnly(XmlNode originalNode)
        {
            XmlNodeList cps = originalNode.SelectNodes("cp");
            if (cps.Count != 0)
            {
                foreach (XmlNode node in cps)
                    originalNode.RemoveChild(node);
            }

            XmlNode nv = originalNode.SelectSingleNode("nv");
            if (nv != null)
                originalNode.RemoveChild(nv);

            XmlNodeList vs = originalNode.SelectNodes("v");
            if (vs.Count == 0)
            {
                XmlNodeList dcs = originalNode.SelectNodes("dc");
                if (dcs != null)
                {
                    foreach (XmlNode node in dcs)
                        originalNode.RemoveChild(node);
                }

            }
            else if(vs.Count > 0)
            {
                foreach (XmlNode voice in vs)
                {
                    XmlNodeList dcs = voice.SelectNodes("dc");
                    if (dcs.Count != 0)
                    {
                        foreach (XmlNode node in dcs)
                            voice.RemoveChild(node);
                    }
                }
            }

            return originalNode;
        }


        /**
         * Searches for text in image captions.
         * */
        public static List<SearchResult> searchPicCaptions(String search, int caseSensitive, int wordSensitive, XmlDocument xml, XmlDocument layoutXml)
        {
            List<SearchResult> results = new List<SearchResult>();

            try
            {
                XmlNodeList xnl = xml.DocumentElement.SelectNodes("//figure");

                int numFound = 0;

                foreach (XmlNode xn in xnl)
                {
                    if (foundBySpecifiedCase(search, xn.InnerText, caseSensitive) && foundBySpecifiedWord(search, xn.InnerText, wordSensitive))
                    {
                        numFound++;
                        SearchResult newResult = new SearchResult();

                        newResult.resultType = 3;
                        int index = xn.InnerText.IndexOf("(");
                        newResult.text1 = xn.InnerText.Substring(0, index).Trim();
                        newResult.text2 = xn.InnerText.Substring(index).Trim();
                        String resultLine = xn.InnerText.Trim();
                        String str1 = resultLine.Substring(0, myComp.IndexOf(resultLine, search, CompareOptions.IgnoreCase));
                        String str2 = resultLine.Substring(myComp.IndexOf(resultLine, search, CompareOptions.IgnoreCase) + search.Length);
                        newResult.excerpt1 = str1;
                        newResult.excerpt2 = search;
                        newResult.excerpt3 = str2;
                        newResult.tag = xn.Attributes["id"].Value;
                        newResult.folio = "Fo" + (xn.ParentNode.Attributes["facs"].Value).Substring(1);
                        results.Add(newResult);
                    }
                }

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return results;
        }



        /**
         * The beginning of music search that allows searching for a motet by number of voice.
         * */
        public static void filterByVoice(int voiceNum, XmlDocument xml)
        {
            XmlNodeList musics = xml.DocumentElement.SelectNodes("//p[(nv)]");

            foreach (XmlNode xn in musics)
            {
                XmlNode testNode = xn.SelectSingleNode("nv");
                String str = testNode.InnerText;
                int intVoiceCount = Convert.ToInt32(str);
            }

            Console.Read();
        }


        /**
         * Converts a System.Windows.Controls.Image to System.Drawing.Image.
         * Used for thumbnailing purposes.
         * */
        public static System.Windows.Controls.Image convertImage(System.Drawing.Image gdiImg)
        {

            System.Windows.Controls.Image img = new System.Windows.Controls.Image();

            //convert System.Drawing.Image to WPF image
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(gdiImg);
            IntPtr hBitmap = bmp.GetHbitmap();
            System.Windows.Media.ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            img.Source = WpfBitmap;
            img.Width = 500;
            img.Height = 600;
            img.Stretch = System.Windows.Media.Stretch.Fill;
            return img;
        }
    }
}
