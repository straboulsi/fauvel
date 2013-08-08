using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;


namespace SurfaceApplication1
{
    class Study
    {


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


    }
}
