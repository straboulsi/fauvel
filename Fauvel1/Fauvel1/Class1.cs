using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.IO;

namespace Fauvel1
{
    static class Class1
    {

        /// Takes in start and end line numbers from the .xaml.cs 
        /// This is good bc Surface could send the int values of the first/last lines highlighted by user
        /// The overlay is then flexible and does not limit to translating entire sections of poetry
        /// <param name="firstLine">First line of target poetry section</param>
        /// <param name="lastLine">Last line of target poetry section</param>
        public static void go(int firstLine, int lastLine)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load("XMLFinalContentFile.xml");



                
                /*
                Note: These validation handlers are meant to be used with lots of schemas in the heading
                ValidationEventHandler eventHandler = new ValidationEventHandler(ValidationEventHandler);
               
                xml.Validate(eventHandler);
              
                XmlElement elem = xml.GetElementById("Te_0035-0048");
                if (elem != null)
                {
                    Console.Write(elem.InnerText);
                }
                 * */



                
                for (int i = firstLine; i <= lastLine; i++)
                {

                    ///XmlNode foundNode = xml.DocumentElement.SelectSingleNode("//lg"); /// THIS WORKS!
                    ///XmlNode foundNode = xml.DocumentElement.SelectSingleNode("//lg[@xml:id='Te_0035-0048']"); /// This doesn't
                    XmlNode foundNode = xml.DocumentElement.SelectSingleNode("//lg/l[@n='"+i+"']");
                
                    Console.Write(foundNode.InnerText+"\r\n");

                }
                

                /*
                /// The following works to output all poetry in Fauvel
                ///XmlNodeList nodeList = xml.DocumentElement.SelectNodes("//lg");
            
                /// This line does not work though:
                /// XmlNodeList nodeList = xml.DocumentElement.SelectNodes("//lg[@xml:id='Te_0035-0048']");
                /// But if you change "xml:id" to "class" in .xml...
                ///XmlNodeList nodeList = xml.DocumentElement.SelectNodes("//lg[@class='Te_0035-0048']");
          
                foreach (XmlNode poetry in nodeList)
                {
                    Console.Write(poetry.InnerText);
                }
                */


                Console.Write("\r\nI'm done!");
                Console.Read();
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }
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
