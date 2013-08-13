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

    }
}
