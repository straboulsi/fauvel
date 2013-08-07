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
     * Primary Coder: Alison Y. Chang
     **/
    public static class Translate
    {
        public static Brush textBrush = (Brush)(new BrushConverter().ConvertFrom("#663311"));
        public static Brush backBrush = (Brush)(new BrushConverter().ConvertFrom("#CCE0D0B0"));
        public static Brush blockBrush = (Brush)(new BrushConverter().ConvertFrom("#ccffffff"));
        public static Brush blockFillerBrush = (Brush)(new BrushConverter().ConvertFrom("#33000000"));
        public static CompareInfo myComp = CultureInfo.InvariantCulture.CompareInfo; // Used in the Contains method
        public static int veryLastLine = 5986; // The maximum line number of the Fauvel poem



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




        /**
         * Creates an ArrayList of TranslationBox objects when given a folio page.
         * Consults the Content (Old French), Layout, and English XML files. 
         * Calls on other methods in this class to fetch English, French, or coordinates.
         * Expects folio without the "Fo" - i.e. 1v, 35r, 28tr
         **/
        public static List<TranslationBox> getTranslationOverlay(String page)
        {

            List<TranslationBox> boxes = new List<TranslationBox>();

            try
            {

                // Looks for each poetry object indicated in the layoutXML on a given page
                XmlNodeList foundNode = SurfaceWindow1.layoutXml.DocumentElement.SelectNodes("//surface[@id='" + page + "']/zone");

                foreach (XmlNode xn in foundNode)
                {
                    String tag = xn.Attributes["id"].Value;
                    if (tag.StartsWith("Te")) // Poetry objects only
                    {
                        XmlNodeList boxList = xn.SelectNodes("box"); // Gets each set of coordinates
                        foreach (XmlNode node in boxList)
                        {
                            String s = node.Attributes["id"].Value; // Returns line range - i.e. "Te_4500-4590"
                            int index = s.IndexOf("_"); 
                            int mid = s.IndexOf("-");
                            int start = Convert.ToInt32(s.Substring(index + 1, 4)); // First line number in the range
                            int end = Convert.ToInt32(s.Substring(mid + 1)); // Last line number in the range

                            // Gets translations in every available language
                            boxes.Add(new TranslationBox(s, getPoetry(start, end, SurfaceWindow1.xml), getPoetry(start, end, SurfaceWindow1.modFrXml),
                                getPoetry(start, end, SurfaceWindow1.engXml), getPoint(s, 1), getPoint(s, 2)));
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
        public static List<BoundingBox> getGhostBoxes(String page)
        {
            List<BoundingBox> boxes = new List<BoundingBox>();

            try
            {
                // Looks at every box in layoutXml 
                XmlNodeList foundNodes = SurfaceWindow1.layoutXml.DocumentElement.SelectNodes("//surface[@id='" + page + "']/zone/box");
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
        public static Point getPoint(String tag, int whichPt)
        {
            Point pt = new Point();

            try
            {
                XmlNode xn;

                if (tag.StartsWith("Te")) // Poetry object
                {
                    xn = SurfaceWindow1.layoutXml.DocumentElement.SelectSingleNode("//surface/zone/box[@id='" + tag + "']");
                    if (whichPt == 1) // Top left
                        pt = new Point(Convert.ToInt32(xn.Attributes["ulx"].Value), Convert.ToInt32(xn.Attributes["uly"].Value));
                    else if (whichPt == 2) // Lower right
                        pt = new Point(Convert.ToInt32(xn.Attributes["lrx"].Value), Convert.ToInt32(xn.Attributes["lry"].Value));
                }
                else // Music or image
                {
                    xn = SurfaceWindow1.layoutXml.DocumentElement.SelectSingleNode("//surface/zone[@id='" + tag + "']");

                    if (whichPt == 1) //  Top left
                        pt = new Point(Convert.ToInt32(xn.FirstChild.Attributes["ulx"].Value), Convert.ToInt32(xn.FirstChild.Attributes["uly"].Value));
                    else if (whichPt == 2) // Lower right
                        pt = new Point(Convert.ToInt32(xn.FirstChild.Attributes["lrx"].Value), Convert.ToInt32(xn.FirstChild.Attributes["lry"].Value));
                }
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return pt;
        }


        /**
         * Fetches Points from LayoutXML coordinates. 
         * Similar to getPoint, but specially designed to work with poetry lines.
         * For poetry, coordinates are not recorded for every line, but only for each poetry chunk.
         * */
        public static Point getLinePoint(String lineNum, int whichPt)
        {
            Point pt = new Point();

            try
            {

                XmlNode xn = SurfaceWindow1.layoutXml.DocumentElement.SelectSingleNode("//l[@n='" + lineNum + "']");
                XmlNode section = xn.ParentNode; // Finds which section of poetry has this line

                if (whichPt == 1) // Top left
                    pt = new Point(Convert.ToInt32(section.FirstChild.Attributes["ulx"].Value), Convert.ToInt32(section.FirstChild.Attributes["uly"].Value));
                else if (whichPt == 2) // Lower right
                    pt = new Point(Convert.ToInt32(section.FirstChild.Attributes["lrx"].Value), Convert.ToInt32(section.FirstChild.Attributes["lry"].Value));

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return pt;
        }






        /**
         * Takes in start and end line numbers.
         * Returns String of Old French, Modern French, or English poetry, depending on the Xml document.
         **/
        public static String getPoetry(int firstLine, int lastLine, XmlDocument whichXml)
        {

            String toDisplay = "";
            try
            {
                XmlNodeList foundNodes = whichXml.DocumentElement.SelectNodes("//lg/l[@n>=" + firstLine + "and @n<=" + lastLine + "]");
                foreach (XmlNode xn in foundNodes)
                {
                    if (xn.HasChildNodes) // Skips over <dc>
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
         *  This method searches for an id value for images, music, and poetry sections, using a tag.
         *  <figure id="1rIm2"> Searching for image
         *  <p id="1rMo2"> Searching for music
         *  <lg> searching by Te_xxxx-xxxx
         *  Searching Fo1v or some other page gives you all contents of that page.
         *  <param name="str">The value of the id</param>
        **/
        public static String getByTag(String str, XmlDocument whichXml)
        {
            String toDisplay = "";
            try
            {
                XmlNode foundNode;

                if (str.Contains("Im"))
                {
                    foundNode = whichXml.DocumentElement.SelectSingleNode("//figure[@id='" + str + "']");
                    toDisplay += foundNode.InnerText.Trim();
                }
                else if (str.StartsWith("Te"))
                {
                    foundNode = whichXml.DocumentElement.SelectSingleNode("//lg[@id='" + str + "']");
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
                    foundNode = whichXml.DocumentElement.SelectSingleNode("//pb[@facs='#" + page + "']");
                    toDisplay += foundNode.InnerXml;
                }
                else // Select music objects
                {

                    /// Note: To select voices that don't have <dc>, add second level and select ("//v[not(dc)]")
                    foundNode = whichXml.DocumentElement.SelectSingleNode("//p[@id='" + str + "']");

                    toDisplay += lyricsOnly(foundNode).InnerText.Trim();
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
         * Searches for text in Fauvel poetry.
         * Last two parameters are language-specific - 0 is Old Fr, 1 is Mod Fr, 2 is Eng
         **/
        public static List<SearchResult> searchExactPoetry(String search, int caseSensitive, int wordSensitive, int language)
        {

            List<SearchResult> results = new List<SearchResult>();

            XmlDocument thisXml = whichXml(language);

            try
            {
                XmlNodeList xnl = thisXml.DocumentElement.SelectNodes("//lg/l");

                foreach (XmlNode xn in xnl)
                {

                    if (foundBySpecifiedCase(search, xn.InnerText, caseSensitive) && foundBySpecifiedWord(search, xn.InnerText, wordSensitive))
                    {
                        SearchResult newResult = new SearchResult();

                        // Gets rid of drop caps
                        if (xn.ChildNodes.Count > 1) // Originally written for OldFr; make sure this doesn't mess up Mod Fr or Eng!
                            xn.RemoveChild(xn.FirstChild);
                        
                        newResult.lineNum = Convert.ToInt32(xn.Attributes["n"].Value);
                        newResult.resultType = 1;
                        newResult.folio = getPageByLineNum(newResult.lineNum);

                        int startLine = newResult.lineNum - 3; 
                        if (startLine < 1)
                            startLine = 1;
                        int endLine = newResult.lineNum + 3;
                        if (endLine > veryLastLine)
                            endLine = veryLastLine;


                        String resultLine = getPoetry(newResult.lineNum, newResult.lineNum, thisXml); 
                        String str1 = resultLine.Substring(0, myComp.IndexOf(resultLine, search, CompareOptions.IgnoreCase));
                        String str2 = resultLine.Substring(myComp.IndexOf(resultLine, search, CompareOptions.IgnoreCase) + search.Length);
                        String lineInfo = "\r\n\r\nLines " + startLine + " to " + endLine;

                        newResult.excerpts.Add(new SpecialString((getPoetry(startLine, newResult.lineNum - 1, thisXml) + "\r\n" + str1), 0)); // Same xml as above
                        newResult.excerpts.Add(new SpecialString(search, 1));
                        newResult.excerpts.Add(new SpecialString((str2 + "\r\n" + getPoetry(newResult.lineNum + 1, endLine, thisXml) + lineInfo), 0));

                        newResult.text1 = xn.InnerText.Trim();

                        if(language == 2) // For English, the secondary text is original text
                            newResult.text2 = getPoetry(newResult.lineNum, newResult.lineNum, SurfaceWindow1.xml);
                        else // For all other languages, the secondary text is the English
                            newResult.text2 = getPoetry(newResult.lineNum, newResult.lineNum, SurfaceWindow1.engXml); 

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

        public static XmlDocument whichXml(int language)
        {
            if (language == 1)
                return SurfaceWindow1.modFrXml;
            else if (language == 2)
                return SurfaceWindow1.engXml;
            else // i.e. if 0
                return SurfaceWindow1.xml; // This can be our default
        }
    


        /**
         * Takes in 0 for Old Fr, 1 for Mod Fr, and 2 for Eng. 
         * This method is specifically for poetry.
         * */
        public static List<SearchResult> searchMultipleWordsPoetry(String searchStr, int caseSensitive, int wordSensitive, int language)
        {
            List<SearchResult> results = new List<SearchResult>();
            List<SpecialString> searchStrings = new List<SpecialString>();

            // Picks a certain language XML based on language int
            XmlDocument thisXml = whichXml(language);

            String[] search = searchStr.Trim().Split(new String[] { " " }, StringSplitOptions.None); // To search for each word independently

            foreach (String s in search)
                searchStrings.Add(new SpecialString(s, false)); // See SpecialString class

            try
            {
                XmlNodeList xnl = thisXml.DocumentElement.SelectNodes("//lg/l"); // Poetry lines only
                

                foreach (XmlNode xn in xnl)
                {
                    // Start by finding the first word
                    if (foundBySpecifiedCase(searchStrings[0].str, xn.InnerText, caseSensitive) && foundBySpecifiedWord(searchStrings[0].str, xn.InnerText, wordSensitive))
                    {
                        // Get five lines to search in
                        int lineNum = Convert.ToInt32(xn.Attributes["n"].Value); 
                        int startLine = lineNum - 2;
                        if (startLine < 1)
                            startLine = 1;
                        int endLine = lineNum + 2;
                        if (endLine > veryLastLine)
                            endLine = veryLastLine;
                        String resultText = getPoetry(startLine, endLine, thisXml);


                        // Makes sure every word shows up in this excerpt
                        // This criteria can be tweaked if we decide to be more lenient - i.e. include half or more
                        foreach (SpecialString ss in searchStrings)
                        {
                            if (foundBySpecifiedCase(ss.str, resultText, caseSensitive) && foundBySpecifiedWord(ss.str, resultText, wordSensitive))
                            {
                                ss.isFound = true; // Keep in mind that this will stay true until set again otherwise!
                                ss.spotInResult = myComp.IndexOf(resultText, ss.str, CompareOptions.IgnoreCase);
                            }
                            else
                                ss.isFound = false; // Make sure to set this for each new result
                        }

                        Boolean foundAll = true;
                        foreach (SpecialString ss in searchStrings)
                        {
                            if (ss.isFound == false)
                                foundAll = false;
                        }


                        // If all criteria met, create a new SearchResult
                        if (foundAll == true)
                        {
                            SearchResult newResult = new SearchResult();
                            newResult.lineNum = startLine;
                            if(startLine != endLine)
                                newResult.lineRange = "-" + endLine;
                            newResult.resultType = 1;
                            newResult.text1 = getPoetry(lineNum, lineNum, thisXml);

                            if (language == 2) // For English, the secondary text is original text
                                newResult.text2 = getPoetry(lineNum, lineNum, SurfaceWindow1.xml);
                            else // For all other languages, the secondary text is the English
                                newResult.text2 = getPoetry(lineNum, lineNum, SurfaceWindow1.engXml); 

                            newResult.folio = getPageByLineNum(lineNum);


                            searchStrings.Sort(); // Sorts the search words by the order they show up in these few lines

                            // Now, check format of each piece of the text - search words bold.
                            // See SpecialString class for more info.
                            // resultText is substringed on each pass bc otherwise it will reenter all of the text from the very beginning.
                            foreach (SpecialString ss in searchStrings)
                            {
                                newResult.excerpts.Add(new SpecialString(resultText.Substring(0, myComp.IndexOf(resultText, ss.str, CompareOptions.IgnoreCase)), 0));
                                newResult.excerpts.Add(new SpecialString(resultText.Substring(myComp.IndexOf(resultText, ss.str, CompareOptions.IgnoreCase), ss.str.Length), 1));
                                resultText = resultText.Substring(myComp.IndexOf(resultText, ss.str, CompareOptions.IgnoreCase) + ss.str.Length);
                            }


                            newResult.excerpts.Add(new SpecialString(resultText, 0)); // Adding the rest of the resultText, from after the end of the last search word
                            newResult.excerpts.Add(new SpecialString("\r\n\r\nLines " + startLine + " to " + endLine, 0));

                            results.Add(newResult);
                        }

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
         * Searches for multiple words (not necessarily adjacent) in lyrics.
         * Takes in 0 for old french and 1 for modern french.
         * */
        public static List<SearchResult> searchMultipleWordsLyrics(String searchStr, int caseSensitive, int wordSensitive, int language)
        {
            List<SearchResult> results = new List<SearchResult>();
            List<SpecialString> searchStrings = new List<SpecialString>();

            // Picks a certain language XML based on language int
            XmlDocument thisXml = whichXml(language);

            String[] search = searchStr.Trim().Split(new String[] { " " }, StringSplitOptions.None); // To search for each word independently

            foreach (String s in search)
                searchStrings.Add(new SpecialString(s, false)); // See SpecialString class

            try
            {
                XmlNodeList xnl = thisXml.DocumentElement.SelectNodes("//p"); // Music lines only


                foreach (XmlNode xn in xnl)
                {
                    // Start by finding the first word
                    if (foundBySpecifiedCase(searchStrings[0].str, xn.InnerText, caseSensitive) && foundBySpecifiedWord(searchStrings[0].str, xn.InnerText, wordSensitive))
                    {
                        XmlNode node = lyricsOnly(xn);
                        String[] allLyrics = node.InnerText.Trim().Split(new String[] { "\r\n", "\n" }, StringSplitOptions.None);

                        int lyricLineNum = -5; // a random negative number to distinguish from actual result
                        for (int i = 0; i < allLyrics.Length; i++)
                        {
                            if (foundBySpecifiedCase(searchStrings[0].str, allLyrics[i], caseSensitive) && foundBySpecifiedWord(searchStrings[0].str, allLyrics[i], wordSensitive))
                            {
                                lyricLineNum = i; // Finds the lyric line that the word appears in
                                break;
                            }
                        }

                        String resultText = ""; // Creating a range of text to find the other words in
                        int startLine = lyricLineNum - 3;
                        int endLine = lyricLineNum + 3;
                        if (startLine < 0)
                            startLine = 0;
                        if (endLine >= allLyrics.Length)
                            endLine = allLyrics.Length - 1;

                        for (int i = startLine; i <= endLine; i++)
                        {
                            if (!allLyrics[i].Trim().StartsWith("#")) // Ignoring comments
                                resultText += allLyrics[i].Trim() + "\r\n";
                        }


                        // Makes sure every word shows up in this excerpt
                        // This criteria can be tweaked if we decide to be more lenient - i.e. include half or more
                        foreach (SpecialString ss in searchStrings)
                        {
                            if (foundBySpecifiedCase(ss.str, resultText, caseSensitive) && foundBySpecifiedWord(ss.str, resultText, wordSensitive))
                            {
                                ss.isFound = true; // Keep in mind that this will stay true until set again otherwise!
                                ss.spotInResult = myComp.IndexOf(resultText, ss.str, CompareOptions.IgnoreCase);
                            }
                            else
                                ss.isFound = false; // Make sure to set this for each new result
                        }

                        Boolean foundAll = true;
                        foreach (SpecialString ss in searchStrings)
                        {
                            if (ss.isFound == false)
                                foundAll = false;
                        }


                        // If all criteria met, create a new SearchResult
                        if (foundAll == true)
                        {
                            SearchResult newResult = new SearchResult();
                            newResult.resultType = 2;
                            newResult.text1 = allLyrics[0];
                            newResult.tag = xn.Attributes["id"].Value;
                            newResult.folio = getPageByTag(newResult.tag, 2);

                            if (startLine > 1)
                                resultText = "...\r\n" + resultText;
                            if (endLine < allLyrics.Count())
                                resultText += "...";


                            searchStrings.Sort(); // Sorts the search words by the order they show up in these few lines

                            // Now, check format of each piece of the text - search words bold.
                            // See SpecialString class for more info.
                            // resultText is substringed on each pass bc otherwise it will reenter all of the text from the very beginning.
                            foreach (SpecialString ss in searchStrings)
                            {
                                newResult.excerpts.Add(new SpecialString(resultText.Substring(0, myComp.IndexOf(resultText, ss.str, CompareOptions.IgnoreCase)), 0));
                                newResult.excerpts.Add(new SpecialString(resultText.Substring(myComp.IndexOf(resultText, ss.str, CompareOptions.IgnoreCase), ss.str.Length), 1));
                                resultText = resultText.Substring(myComp.IndexOf(resultText, ss.str, CompareOptions.IgnoreCase) + ss.str.Length);
                            }


                            newResult.excerpts.Add(new SpecialString(resultText, 0)); // Adding the rest of the resultText, from after the end of the last search word

                            results.Add(newResult);
                        }

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
        public static String getTagByLineNum(int lineNum)
        {
            String tag = "";

            try
            {
                XmlNode xn = SurfaceWindow1.layoutXml.DocumentElement.SelectSingleNode("//zone/l[@n=" + lineNum + "]");
                tag = xn.ParentNode.Attributes["id"].Value; // Gets tag one level up
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return tag;
        }

        /**
         * Returns the name of the page that a given line of poetry is on, using the original text Xml.
         * Folio is returned as "Fo__"
         * */
        public static String getPageByLineNum(int lineNum)
        {
            String folio = "Fo";

            try
            {
                XmlNode xnl = SurfaceWindow1.xml.DocumentElement.SelectSingleNode("//lg/l[@n=" + lineNum + "]");
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
        public static String getPageByTag(String tag, int type)
        {
            String folio = "Fo";
            String tagType = "";
            if (type == 1)
                tagType = "lg"; // Poetry
            else if (type == 2)
                tagType = "p"; // Music
            else if (type == 3)
                tagType = "figure"; // Image

            try
            {
                XmlNode xnl = SurfaceWindow1.xml.DocumentElement.SelectSingleNode("//" + tagType + "[@id='" + tag + "']");
                String folioTemp = xnl.ParentNode.Attributes["facs"].Value;
                folioTemp = folioTemp.Substring(1); // Removes the #
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
        public static List<SearchResult> searchLyrics(String search, int caseSensitive, int wordSensitive, XmlDocument whichXml)
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
                        newResult.resultType = 2; // resultType for music
                        String str = xn.InnerText;
                        String[] allLyrics = str.Trim().Split(new String[] { "\r\n", "\n" }, StringSplitOptions.None); // Splits line by line


                        int lyricLineNum = -5; // a random negative number to distinguish from actual result
                        for (int i = 0; i < allLyrics.Length; i++)
                        {
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
                            excerpt += "...\r\n"; // To indicate there is more
                        
                        for (int i = firstLine; i <= lastLine; i++)
                        {
                            if(!allLyrics[i].Trim().StartsWith("#"))
                                excerpt += allLyrics[i].Trim() + "\r\n";
                        }

                        if (lastLine != allLyrics.Length - 1)
                            excerpt += "..."; // To indicate there is more 

                        

                        newResult.excerpts.Add(new SpecialString(excerpt.Substring(0, myComp.IndexOf(excerpt, search, CompareOptions.IgnoreCase)), 0));
                        newResult.excerpts.Add(new SpecialString(search, 1));
                        newResult.excerpts.Add(new SpecialString(excerpt.Substring(myComp.IndexOf(excerpt, search, CompareOptions.IgnoreCase) + search.Length), 0));

                        newResult.text1 = allLyrics[0]; // Should this be allLyrics[lyricLineNum]? 
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
        public static List<SearchResult> searchPicCaptions(String search, int caseSensitive, int wordSensitive, XmlDocument whichXml)
        {
            List<SearchResult> results = new List<SearchResult>();

            try
            {
                XmlNodeList xnl = whichXml.DocumentElement.SelectNodes("//figure"); // Selects images

                foreach (XmlNode xn in xnl)
                {
                    if (foundBySpecifiedCase(search, xn.InnerText, caseSensitive) && foundBySpecifiedWord(search, xn.InnerText, wordSensitive))
                    {
                        SearchResult newResult = new SearchResult();

                        newResult.resultType = 3;
                        int index = xn.InnerText.IndexOf("(");
                        newResult.text1 = xn.InnerText.Substring(0, index).Trim();
                        newResult.text2 = xn.InnerText.Substring(index).Trim();
                        
                        String resultLine = xn.InnerText.Trim();
                        String str1 = resultLine.Substring(0, myComp.IndexOf(resultLine, search, CompareOptions.IgnoreCase));
                        String str2 = resultLine.Substring(myComp.IndexOf(resultLine, search, CompareOptions.IgnoreCase) + search.Length);
                        newResult.excerpts.Add(new SpecialString(str1, 0));
                        newResult.excerpts.Add(new SpecialString(search, 1));
                        newResult.excerpts.Add(new SpecialString(str2, 0));
                        newResult.tag = xn.Attributes["id"].Value;
                        newResult.folio = "Fo" + (xn.ParentNode.Attributes["facs"].Value).Substring(1); // Gets rid of # 
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
         * This method is not fully developed, nor has it been called.
         * */
        public static void filterByVoice(int voiceNum)
        {
            XmlNodeList musics = SurfaceWindow1.xml.DocumentElement.SelectNodes("//p[(nv)]");

            foreach (XmlNode xn in musics)
            {
                XmlNode testNode = xn.SelectSingleNode("nv"); 
                String str = testNode.InnerText;
                int intVoiceCount = Convert.ToInt32(str);
            }

        }


        /**
         * Converts a System.Windows.Controls.Image to System.Drawing.Image.
         * Used for thumbnailing purposes.
         * This method was adopted from somewhere on the internet.
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
