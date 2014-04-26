using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Windows;


namespace DigitalFauvel
{

    /**
     * This class contains back-end code for the music Study app.
     * Refer to StudyTab.cs for the front-end visual code.
     * NB: Many capabilities of this class are, as of now, very underdeveloped/uncalled. TBC!
     * Primary Coder: Alison Y. Chang
     * */
    public class Study
    {
        //returns a list of pieces on the current opening
        public static Pieces GetPieces(int pageNumber)
        {
            var pieces = new Pieces();
            try
            {
                //gets the xelements representing the two pages of the opening (e.g. 1r, 2v)
                XElement root = SurfaceWindow1.xOldFr.Root;
                string verso = "#" + (pageNumber-2).ToString() + "v";
                string recto = "#" + (pageNumber-1).ToString() + "r";
                IEnumerable<XElement> pages = 
                    from el in root.Descendants("pb")
                    where ((string)el.Attribute("facs") ==  verso || el.Attribute("facs").Value == recto)
                    select el;
                //adds the pieces on each page to the pieceList
                foreach (XElement page in pages)
                {
                    //var foo = page.Attribute("facs");
                    IEnumerable<XElement> pieceElements =
                    from el in page.Elements("p")
                    select el; //.Attribute("id").Value;

                    foreach (XElement p in pieceElements)
                    {
                        var piece = new Piece(p);
                        pieces.Add(piece.ID,piece);
                    }
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return pieces;
        }

        /**
         * The beginning of music search that allows searching for a motet by number of voice.
         * */
        public static void filterByVoice(int voiceNum)
        {
            try
            {
                XmlNodeList musics = SurfaceWindow1.xml.DocumentElement.SelectNodes("//p[(nv)]");
                String temp = "";

                foreach (XmlNode xn in musics)
                {
                    if (Convert.ToInt32(xn.SelectSingleNode("nv").InnerText) == voiceNum)
                        temp = Search.lyricsOnly(xn).InnerText + "\r\n\r\n";
                }
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

        }

        /**
         * Takes in the tag of a music object and returns number of voices. If > 1 voice, it is a motet. 
         * */
        public static int voiceCount(String tag)
        {
            int voiceCount = 0;
            try
            {
                // Bc the number of voices tag is within the music object with lyrics - the p tag, not the notatedMusic tag
                if (!tag.EndsWith("_t"))
                    tag += "_t";

                XmlNode foundNode = SurfaceWindow1.xml.DocumentElement.SelectSingleNode("//p[@id='" + tag + "']");
                if (foundNode.SelectSingleNode("nv") != null)
                    voiceCount = Convert.ToInt32(foundNode.SelectSingleNode("nv").InnerText);
                else
                    voiceCount = 1;
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return voiceCount;
        }


        /**
         * Takes in the tag of a music object and checks whether it has multiple voices (motet) or not (other music type).
         * */
        public static Boolean hasMultipleVoices(String tag)
        {
            if(voiceCount(tag) > 1)
                return true;
            else
                return false;
        }

        /**
         * Takes in tag of a music object and returns its title.
         * */
        public static String getTitle(String tag)
        {
            String title = "";
            try
            {
                // Bc the title tag is within the music object with lyrics - the p tag, not the notatedMusic tag
                if (!tag.EndsWith("_t"))
                    tag += "_t";

                XmlNode foundNode = SurfaceWindow1.xml.DocumentElement.SelectSingleNode("//p[@id='" + tag + "']");
                XmlNode titleNode = foundNode.SelectSingleNode("title");
                title = titleNode.InnerText.Trim();

                Console.Write(tag + "\r\n" + title + "\r\n\r\n");
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return title;
        }

        /**
         * Takes in a longer string, like the title of a piece of music, and returns the first word only.
         * */
        public static String firstWord(String title)
        {
            String firstWord = "";

            firstWord = title.Substring(0, title.IndexOf(" "));

            return firstWord;
        }


        /**
         * Takes in the ID tag for a piece of music.
         * Returns a list of the voice parts (i.e. duplum, triplum).
         * The voice part info is coming from the OriginalTextXML.
         * */
        public static List<String> getVoiceParts(String tag)
        {
            List<String> voiceParts = new List<String>();

            try
            {
                if (!tag.EndsWith("_t"))
                    tag += "_t";
                XmlNode musicObj = SurfaceWindow1.xml.DocumentElement.SelectSingleNode("//p[@id='" + tag + "']");
                XmlNodeList voices = musicObj.SelectNodes("v");

                foreach (XmlNode xn in voices)
                {
                    String s = xn.Attributes["part"].Value;
                    voiceParts.Add(s);
                }
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return voiceParts;
        }
    }
}
