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

namespace DigitalFauvel
{

    /**
     * This class contains all major methods used in the SearchTab.
     * It includes search capabilities, taking into consideration the preferences marked (case sensitive, whole word, exact phrase, language).
     * Accessing the appropriate XML file, the search methods then return a list of search results.
     * This class also includes all helper methods - i.e. getting various info (folio, page) by other info (line number, object tag).
     * Other methods include specialized forms of search, trimming of extraneous text from lyrics, etc.
     * 
     * This class contains the back end methods for SearchTab functionality.
     * For front end (UI) methods and properties, see SearchTab.cs.
     * Primary Coder: Alison Y. Chang
     **/
    public static class Search
    {
        public static CompareInfo myComp = CultureInfo.InvariantCulture.CompareInfo; // Used in the Contains method
        public static int veryLastLine = 5986; // The maximum line number of the Fauvel poem





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
            if (wordSensitive == 1) 
            {
                bool isMatch = Regex.IsMatch(toSearchIn, string.Format(@"\b{0}\b", Regex.Escape(toFind))); 
                // Issue: Does not work when string includes punctuation, bc \b marks boundary between word character and non word character. Hence the below...

                bool contains = toSearchIn.Contains(" " + toFind + " ");
                bool atEnd = (toSearchIn.Trim().EndsWith(" " + toFind));
                bool atStart = toSearchIn.Trim().StartsWith(toFind + " ");

                // These two are for multi word search, when several lines are combined for toSearchIn
                bool b1 = toSearchIn.Trim().Contains("\r\n" + toFind + " ");
                bool b2 = toSearchIn.Trim().Contains(" " + toFind + "\r\n");

                return (isMatch || contains || atEnd || atStart || b1 || b2);

            }
            else
                return true;
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
                    if (tag.EndsWith("_t"))
                        tag = tag.Substring(0, tag.Length - 2);

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
         * Takes in 1 as top left and 2 as lower right.
         * */
        public static Point getLinePoint(int lineNum, int whichPt)
        {
            Point pt = new Point();

            String lineNumStr = Convert.ToString(lineNum);

            try
            {

                XmlNode xn = SurfaceWindow1.layoutXml.DocumentElement.SelectSingleNode("//l[@n='" + lineNumStr + "']");
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

                if (str.Contains("Im")) // Image
                {
                    foundNode = whichXml.DocumentElement.SelectSingleNode("//figure[@id='" + str + "']");
                    toDisplay += foundNode.InnerText.Trim();
                }
                else if (str.StartsWith("Te")) // Poetry
                {
                    foundNode = whichXml.DocumentElement.SelectSingleNode("//lg[@id='" + str + "']");
                    XmlNodeList lineByLine = foundNode.SelectNodes("l");
                    foreach (XmlNode x in lineByLine)
                    {
                        XmlNode newX = x.RemoveChild(x.LastChild); // Removes the drop cap inner text
                        toDisplay += newX.InnerText.Trim() + "\r\n";
                    }
                }
                else if (str.StartsWith("Fo")) // Entire page
                {
                    String page = str.Substring(2);
                    foundNode = whichXml.DocumentElement.SelectSingleNode("//pb[@facs='#" + page + "']");
                    toDisplay += foundNode.InnerXml;
                }
                else // Music
                {
                    // Note: To select voices that don't have <dc>, add second level and select ("//v[not(dc)]")
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
                XmlNodeList xnl = thisXml.DocumentElement.SelectNodes("//lg/l"); // Selects poetry nodes 

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

                        newResult.tag = getTagByLineNum(newResult.lineNum);
                        newResult.text1 = xn.InnerText.Trim();
                        newResult.topL = getLinePoint(newResult.lineNum, 1);
                        newResult.bottomR = getLinePoint(newResult.lineNum, 2);

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
                    if (xn.InnerText.Contains(searchStr)) /// temp!!
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
                                if (startLine != endLine)
                                    newResult.lineRange = "-" + endLine;
                                newResult.resultType = 1;
                                newResult.text1 = getPoetry(lineNum, lineNum, thisXml);
                                newResult.tag = getTagByLineNum(startLine);
                                newResult.topL = getLinePoint(newResult.lineNum, 1);
                                newResult.bottomR = getLinePoint(newResult.lineNum, 2);

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

                }/// temp
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
                        String tempTitle = xn.SelectSingleNode("title").InnerText;
                        XmlNode node = lyricsOnly(xn);
                        String[] allLyrics = node.InnerText.Trim().Split(new String[] { "\r\n", "\n" }, StringSplitOptions.None); // Splits lyrics line by line

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
                            newResult.text1 = tempTitle; 
                            if (lyricLineNum != 0) // If the search result isn't in the title
                                newResult.text2 = allLyrics[lyricLineNum].Trim();
                            newResult.tag = xn.Attributes["id"].Value;
                            newResult.topL = getPoint(newResult.tag, 1);
                            newResult.bottomR = getPoint(newResult.tag, 2);
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
        public static List<SearchResult> searchExactLyrics(String search, int caseSensitive, int wordSensitive, XmlDocument whichXml)
        {
            List<SearchResult> results = new List<SearchResult>();

            try
            {
                XmlNodeList xnl = whichXml.DocumentElement.SelectNodes("//p");

                foreach (XmlNode node in xnl)
                {
                    if (foundBySpecifiedCase(search, node.InnerText, caseSensitive) && foundBySpecifiedWord(search, node.InnerText, wordSensitive))
                    {
                        String tempTitle = node.SelectSingleNode("title").InnerText;
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

                        newResult.text1 = tempTitle; 
                        if(lyricLineNum != 0) // If the search result isn't in the title
                            newResult.text2 = allLyrics[lyricLineNum].Trim();
                        newResult.tag = xn.Attributes["id"].Value;
                        newResult.topL = getPoint(newResult.tag, 1);
                        newResult.bottomR = getPoint(newResult.tag, 2);
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
         * NOTE: This leaves the title in! But the title node can be removed otherwise quite easily.
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
        public static List<SearchResult> searchExactPicCaptions(String search, int caseSensitive, int wordSensitive, XmlDocument whichXml)
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
                        newResult.topL = getPoint(newResult.tag, 1);
                        newResult.bottomR = getPoint(newResult.tag, 2);
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

        public static List<SearchResult> searchMultipleWordsPicCaptions(String searchStr, int caseSensitive, int wordSensitive, XmlDocument whichXml)
        {
            List<SearchResult> results = new List<SearchResult>();
            List<SpecialString> searchStrings = new List<SpecialString>();

            // Picks a certain language XML based on language int
            XmlDocument thisXml = whichXml; // Allow change based on language once multiple language captions available

            String[] search = searchStr.Trim().Split(new String[] { " " }, StringSplitOptions.None); // To search for each word independently

            foreach (String s in search)
                searchStrings.Add(new SpecialString(s, false)); // See SpecialString class


            try
            {
                XmlNodeList xnl = whichXml.DocumentElement.SelectNodes("//figure"); // Selects images

                foreach (XmlNode xn in xnl)
                {
                    // If the first word has been found
                    if (foundBySpecifiedCase(searchStrings[0].str, xn.InnerText, caseSensitive) && foundBySpecifiedWord(searchStrings[0].str, xn.InnerText, wordSensitive))
                    {
                        String resultText = xn.InnerText.Trim();

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
                            newResult.resultType = 3;
                            int index = resultText.IndexOf("(");
                            newResult.text1 = resultText.Substring(0, index).Trim(); // "Miniature __"
                            newResult.text2 = resultText.Substring(index).Trim(); // "(Image description)"
                            newResult.tag = xn.Attributes["id"].Value;
                            newResult.topL = getPoint(newResult.tag, 1);
                            newResult.bottomR = getPoint(newResult.tag, 2);
                            newResult.folio = getPageByTag(newResult.tag, 3);

                            searchStrings.Sort();

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



    }
}
