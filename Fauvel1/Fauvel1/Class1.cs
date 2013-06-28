using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.IO;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;


namespace Fauvel1
{
    static class Class1
    {

        /// Takes in start and end line numbers from the .xaml.cs 
        /// This is good bc Surface could send the int values of the first/last lines highlighted by user
        /// The overlay is then flexible and does not limit to translating entire sections of poetry
        /// <param name="firstLine">First line of target poetry section</param>
        /// <param name="lastLine">Last line of target poetry section</param>
        public static SurfaceTextBox go(int firstLine, int lastLine)
        {

            SurfaceTextBox stb = new SurfaceTextBox();
            try
            {
                ///Console.Write("Poetry: " + firstLine + " to " + lastLine);
                XmlDocument xml = new XmlDocument();
                xml.Load("XMLFinalContentFile.xml");

                

                String toDisplay = "";
                XmlNode foundNode;

                
                for (int i = firstLine; i <= lastLine; i++)
                {

                   
                    foundNode = xml.DocumentElement.SelectSingleNode("//lg/l[@n='"+i+"']");
                    toDisplay += foundNode.InnerText;
                    Console.Write(foundNode.InnerText+"\r\n");

                }

               
                stb.Text = toDisplay;
                Console.Write("Whatchu got in dere? "+stb.Text);
                

                Console.Write("\r\nI'm done!");
                Console.Read();
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return stb;
        }

        /// <summary>
        /// This method searches for an id value for images, music, and poetry sections
        /// <figure id="1rIm2"> Searching for image
        /// <p id="1rMo2"> Searching for music
        /// <lg> searching by Te_xxxx-xxxx
        /// </summary>
        /// <param name="str">The value of the id</param>
        public static void go(String str)
        {
            Console.Write("Input: "+ str+"\r\n");
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load("XMLFinalContentFile.xml");
                XmlNode foundNode;

                if (str.Contains("Im"))
                {
                    foundNode = xml.DocumentElement.SelectSingleNode("//figure[@id='" + str + "']");
                }
                else if(str.StartsWith("Te")){
                    foundNode = xml.DocumentElement.SelectSingleNode("//lg[@id='"+str+"']");
                }
                else if (str.StartsWith("Fo"))
                {
                    String page = str.Substring(2);
                    Console.Write("page: " + page);
                    foundNode = xml.DocumentElement.SelectSingleNode("//pb[@facs='#" + page + "']");
                    Console.Write(foundNode.InnerXml);
                }
                else
                {
                    /// Note: To select voices that don't have <dc>, add second level and select ("//v[not(dc)]")
                    foundNode = xml.DocumentElement.SelectSingleNode("//p[@id='" + str + "']");
                }
                 
                //Console.Write(foundNode.InnerText + "\r\n");
            }
            catch(Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }
        }


        /// <summary>
        ///  [@nv='"+voiceNum+"']
        /// </summary>
        /// <param name="voiceNum"></param>
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
                 
                if (intVoiceCount==voiceNum)
                {
                    Console.Write(xn.InnerText);
                }
               
            }

            
            
            Console.Read();
        }


        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    Console.WriteLine("Error: {0}", e.Message);
                    break;
                case XmlSeverityType.Warning:
                    Console.WriteLine("Warning {0}", e.Message);
                    break;
            }

        }
    }


}
