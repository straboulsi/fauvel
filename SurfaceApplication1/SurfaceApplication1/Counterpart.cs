using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigitalFauvel
{
    /**
     * This class helps to organize all the music objects in Fauvel that have multiple parts.
     * The first string "name" is the first tag for that object.
     * The other strings in "otherNames" are tags for all other counterparts.
     * A list of Counterparts is created for all applicable objects in Fauvel.
     * This class also includes a method to check whether the tag being used to refer to an object should be replaced with another tag.
     * Primary Coder: Alison Y. Chang
     **/
    class Counterpart
    {
        public static List<Counterpart> counterpartList;
        public List<String> otherNames;
        public String name;



        public Counterpart(String aName, List<String> someOtherNames)
        {
            name = aName;
            otherNames = someOtherNames;
        }



        /**
         * Sets up the Counterpart objects - one for every music object in Fauvel that has counterparts.
         * */
        public static void makeCounterpartList()
        {
            counterpartList = new List<Counterpart>();
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
        }


        /**
         * Checks whether a tag is referring to a non-first counterpart of a music object.
         * Replaces the tag with the tag for the first part of that music object, if necessary.
         * */
        public static String checkForCounterparts(String tag)
        {
            if (!tag.StartsWith("Te") && !tag.Contains("Im")) // If it's music
            {
                foreach (Counterpart c in counterpartList)
                    foreach (String s in c.otherNames)
                        if (tag == s)
                            return c.name;

            }
            return tag;
        }


    }
}
