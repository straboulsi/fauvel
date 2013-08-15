README.TXT
digitalfauvel repository
Written by Alison Y. Chang, August 2013


Contains:
DigitalFauvel (inside SurfaceApplication1) – the C# code for the Microsoft Surface 2.0. Open in Visual Studio or other C# environment. Main class is SurfaceWindow1.xaml, and its code-behind is SurfaceWindow1.xaml.cs (get to it by clicking into the second SurfaceApplication1 folder, nested in the first).

Note: THIS WILL NOT RUN IN ITS CURRENT STATE. Due to copyright issues, this repository cannot include the XML, audio, or visual files that are specific to Roman de Fauvel and used in DigitalFauvel. The following folders and their contents are missing: pages, smallpages, thumbnails, icons, XML, and musicz. If you are part of the Princeton DigitalFauvel team, you may have access to our Dropbox, where most of these resources can be found.



Content XML Encoders: 
EnglishXML.java – used to create EnglishXML file. 
ModFrEncoder.java – used to create ModernFrenchXML file. 
TextReader.java – used to create OriginalTextXML file. This was the first Encoder program written for Fauvel. 
Instructions.txt – readme for TextReader.java. 

Each of the three above programs reads a .txt file and prints to another. The latter is valid XML. The actual XML documents cannot be included in this repository due to copyright.

Music.java – defines a music object for ModFrEncoder and TextReader.
TableOfContents.java – lists objects found on each folio using the XML file created by the other programs.



Layout XML Encoders:
evenspace.py – Bonnie Eisenman’s python script for line by line image recognition. Not currently used by any other code.
jdc-findlines.py – Jamie Chong’s python script for getting line by line coordinates, assuming even spacing in a column of text.
LineFinder.java – creates the LayoutXML file by allowing a user to open a folio’s image and click corners of objects. Read LineFinder_instructions for more info.
Coordinate.java – defines a coordinate object for LineFinder.java.

