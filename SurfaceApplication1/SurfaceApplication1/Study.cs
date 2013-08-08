using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;


namespace SurfaceApplication1
{

    /**
     * This class contains back-end code for the music Study app.
     * Refer to StudyTab.cs for the front-end visual code.
     * NB: Many capabilities of this class are, as of now, very underdeveloped/uncalled. TBC!
     * Primary Coder: Alison Y. Chang
     * */
    public class Study
    {


        /**
         * The beginning of music search that allows searching for a motet by number of voice.
         * This method is not fully developed, nor has it been called.
         * 
         * */
        public static void filterByVoice(int voiceNum)
        {
            XmlNodeList musics = SurfaceWindow1.xml.DocumentElement.SelectNodes("//p[(nv)]");
            String temp = "";

            foreach (XmlNode xn in musics)
            {
                if (Convert.ToInt32(xn.SelectSingleNode("nv").InnerText) == voiceNum)
                    temp = Search.lyricsOnly(xn).InnerText+"\r\n\r\n";
            }

        }

        // Temporary method to check if I got all the titles
        public static void checkTitles()
        {
            XmlNodeList xnl = SurfaceWindow1.xml.DocumentElement.SelectNodes("//p[not(title)]");

            foreach (XmlNode xn in xnl)
            {
                XmlNode titleNode = xn.FirstChild;
                Console.Write(titleNode.InnerText);
            }

            XmlNodeList xnl2 = SurfaceWindow1.xml.DocumentElement.SelectNodes("//p[(title)]");

            foreach (XmlNode xn in xnl2)
            {
                XmlNode titleNode = xn.FirstChild;
                String[] allLyrics = Search.lyricsOnly(xn).InnerText.Trim().Split(new String[] { "\r\n", "\n" }, StringSplitOptions.None); // Splits lyrics line by line


                Console.Write("\r\n\r\n" + titleNode.InnerText + "\r\n" + allLyrics[0]);
            }
        }


    }
}
