using System;
using System.Collections.Generic;
using System.Collections;
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
using System.Windows;



namespace Fauvel1
{
    static class Class1
    {

        /// Takes in start and end line numbers from the .xaml.cs 
        /// This is good bc Surface could send the int values of the first/last lines highlighted by user
        /// The overlay is then flexible and does not limit to translating entire sections of poetry
        /// <param name="firstLine">First line of target poetry section</param>
        /// <param name="lastLine">Last line of target poetry section</param>
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

                    foundNode = xml.DocumentElement.SelectSingleNode("//lg/l[@n='"+i+"']");
                    toDisplay += foundNode.InnerText + "\r\n";
                    ///Console.Write(foundNode.InnerText+"\r\n");
                }


            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return toDisplay;
        }

        public static ArrayList makeBoxes(String page)
        {

            ArrayList boxes = new ArrayList();

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load("XMLFinalContentFile.xml");

                XmlNodeList foundNode;
                

                page = page.Substring(2);
                foundNode = xml.DocumentElement.SelectNodes("//pb[@facs='#" + page + "']/lg");

                int i = 0;

                foreach (XmlNode x in foundNode)
                {
                   
                    String s = x.Attributes["id"].Value;
                    int index = s.IndexOf("_");
                    int mid = s.IndexOf("-");

                    int start = Convert.ToInt32(s.Substring(index+1, 4));
                    int end = Convert.ToInt32(s.Substring(mid + 1));
                    
                    boxes.Add(new TranslationBox(s, getPoetry(start, end), getEnglish(start,end), getPoint(s,1), getPoint(s,2)));
                    
                    i++;
                }  

                /**
                foreach (TranslationBox tb in boxes)
                {
                    Console.Write(tb.tag+"\r\n"+tb.pointsToString()+"\r\n");
                }  
                **/

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
         * The int whichPt = 1 if you want top left point and 2 if you want bottom right
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
                    ///Console.Write(foundNode.InnerText+"\r\n");
                }

                ///Console.Read();
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return toDisplay;
        }


        /// <summary>
        /// This method searches for an id value for images, music, and poetry sections
        /// <figure id="1rIm2"> Searching for image
        /// <p id="1rMo2"> Searching for music
        /// <lg> searching by Te_xxxx-xxxx
        /// </summary>
        /// <param name="str">The value of the id</param>
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
                else if(str.StartsWith("Te")){
                    foundNode = xml.DocumentElement.SelectSingleNode("//lg[@id='"+str+"']");
                    XmlNodeList lineByLine = foundNode.SelectNodes("l");
                    foreach (XmlNode x in lineByLine)
                    {
                        str += x.InnerText + "\r\n";
                    }
                }
                else if (str.StartsWith("Fo"))
                {
                    String page = str.Substring(2);
                    foundNode = xml.DocumentElement.SelectSingleNode("//pb[@facs='#" + page + "']");
                    str+= (foundNode.InnerXml);
                }
                else
                {
                    /// Note: To select voices that don't have <dc>, add second level and select ("//v[not(dc)]")
                    foundNode = xml.DocumentElement.SelectSingleNode("//p[@id='" + str + "']");
                    str += foundNode.InnerText;
                }
                 
                //Console.Write(foundNode.InnerText + "\r\n");
                
            }
            catch(Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return str;
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
                    Console.Write(xn.InnerText);
               
            }

            Console.Read();
        }

    }


}
