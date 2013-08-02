using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;


namespace SurfaceApplication1
{

    /**
     * This class fetches thumbnails from the thumbnails folder for search results.
     * */ 
    static class Thumbnailer
    {

        /**
         * Returns the image when given the tag of a chunk of poetry, music object, or image.
         * For music lyric results, starts by removing the _t at end of tag (which meant text).
         * */
        public static Image getThumbnail(String tag)
        {
            if (tag.EndsWith("_t"))
                tag = tag.Substring(0, tag.Length - 2);

            tag = checkForCounterparts(tag);

            Image thumbnail = Image.FromFile(@"..\..\thumbnails\" + tag + ".jpg", true);

            return thumbnail;
        }

        /**
         * Checks whether a tag is referring to a non-first counterpart of a music object.
         * Replaces the tag with the tag for the first part of that music object, if necessary.
         * */
        public static String checkForCounterparts(String tag)
        {
            if (!tag.StartsWith("Te") && !tag.Contains("Im")) // If it's music
            {
                foreach (Counterpart c in makeCounterpartList())
                {
                    foreach (String s in c.otherNames)
                    {
                        if (tag == s)
                        {
                            tag = c.name;
                            break;
                        }
                    }
                }
                
            }
            return tag;

        }

        /**
         * Sets up the Counterpart objects - one for every music object in Fauvel that has counterparts.
         * */
        public static List<Counterpart> makeCounterpartList()
        {
            List<Counterpart> counterpartList = new List<Counterpart>();
            counterpartList.Add(new Counterpart("10vMo2", new List<String>(new String[] { "11rMo1" })));
            counterpartList.Add(new Counterpart("11vMo1", new List<String>(new String[] { "12rMo1" })));
            counterpartList.Add(new Counterpart("12rPr1", new List<String>(new String[] { "12vPr1" })));
            counterpartList.Add(new Counterpart("14rPr1", new List<String>(new String[] { "14vPr1", "15rPr1" })));
            counterpartList.Add(new Counterpart("15vMo1", new List<String>(new String[] { "16rMo1" })));
            counterpartList.Add(new Counterpart("16vBa2", new List<String>(new String[] { "17rBa1" })));
            counterpartList.Add(new Counterpart("17rLa1", new List<String>(new String[] { "17vLa1", "18rLa1", "18vLa1" })));
            counterpartList.Add(new Counterpart("19rLa1", new List<String>(new String[] { "19vLa1" })));
            counterpartList.Add(new Counterpart("19vRo1", new List<String>(new String[] { "20rRo1" })));
            counterpartList.Add(new Counterpart("22rLa1", new List<String>(new String[] { "22vLa1", "23rLa1" })));
            counterpartList.Add(new Counterpart("23vBa1", new List<String>(new String[] { "24rBa1" })));
            counterpartList.Add(new Counterpart("25vRef4", new List<String>(new String[] { "26rRef1" })));
            counterpartList.Add(new Counterpart("28brLa1", new List<String>(new String[] { "28bvLa1", "28trLa1", "28tvLa1" })));
            counterpartList.Add(new Counterpart("34vLa1", new List<String>(new String[] { "35rLa1", "35vLa1", "36rLa1", "36vLa1" })));
            counterpartList.Add(new Counterpart("37rSe1", new List<String>(new String[] { "37vSe1" })));
            counterpartList.Add(new Counterpart("41vMo1", new List<String>(new String[] { "42rMo1" })));
            counterpartList.Add(new Counterpart("43rMo2", new List<String>(new String[] { "43vMo1" })));

            return counterpartList;
        }


    }
}
