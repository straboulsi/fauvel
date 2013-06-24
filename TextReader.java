import java.io.*;
import java.util.*;


/**
 * This program takes in a .txt file that has text copied/pasted from the Fauvel ed. Strubel.pdf.
 * It reads the file line by line, identifying data types (text, images, music...).
 * The program adds appropriate XML tags to each line.
 * Output is saved to a new file that has transformed the original text into an XML file.
 *    NB: The new file does not display accented characters in Eclipse, so open it in TextEdit!
 * Also see the "Copy and Paste Instructions" document.
 * @author alisonychang
 */

public class TextReader {

	private int numOfImages = 0; // Counts # of images on a folio page
	private int numOfLines = 0; // Counts number of textual lines 
	private int numOfRubriques = 0;
	private int numOfMotets = 0;

	// Types of data are indicated by the value of this int dataType
	private int dataType = 0; // Default state when dataType is unknown
	private static int poem = 1;
	private static int music = 2;
	private static int explicit = 3;
	private static int motet = 4; // Need separate case bc motets have voices
	// We do not need a state for miniature bc it is all on one line 

	// These ints are used to count line numbers for the poem
	private int endLine = 0;
	private int linePlace = 1;
	private int pageLineNum = 1; // The line number, starting over at 1 for each new page

	private String pageString = ""; // i.e. "1r" or "3v"; this will be reset as we enter a new page
	private String ID = ""; // i.e. 1rMo1

	private ArrayList<String> poemQueue; // Used to count/print lines of poetry
	File editedF = new File("FauvelEdited.txt"); // The XML-tagged transformed version of the text; program output
	private ArrayList<Music> musicGenres;




	/**
	 * Makes an array list of Music objects for every type of music we expect to encounter.
	 * Constructs a new text reader for Fauvel text.
	 * @param file The file to be read.
	 */
	public TextReader(File file){
		makeMusicGenres();
		readFile(file);
	}


	/**
	 * Reads a file line by line and interprets dataType, adding appropriate XML tags.
	 * @param f The file to be read.
	 */
	public void readFile(File f){
		try {
			Scanner in = new Scanner(f);
			PrintWriter p = new PrintWriter(editedF);
			poemQueue = new ArrayList<String>();
			p.print("<xml>\n<text>\n<body>"); // Starting XML code at top of document

			while(in.hasNext()){	

				String thisLine = lineNumChopper(in.nextLine()); // Deals with any starting numbers


				if(!thisLine.equals("")){ // Skip blank lines

					// If the line indicates a page break but is not a poem line starting w "Fo"
					if(thisLine.startsWith("Fo")&&dataType!=poem&&dataType!=music&&dataType!=motet){
						pageString = thisLine.substring(2); 
						if(Character.isDigit(thisLine.charAt(thisLine.length()-1))) // No r or v
							pageString += "r"; // Adds the default "r" to end

						newPage(); // Resets all motet/image/etc counts to 0
						p.print("\n\n\n\n\n<pb facs=\"#"+pageString+"\" />\n\n");
						dataType = 0;
					}
					
					else if(thisLine.startsWith("#")){
						p.print(thisLine+"\n");
					}
					

					// First (ONLY) line of miniature
					else if(thisLine.startsWith("Miniature")||thisLine.contains("miniature")){
						numOfImages++;
						p.print("\n<figure xml:id=\""+pageString+"Im"+numOfImages+"\">\n<figDesc>\n"
								+thisLine+"\n</figDesc>\n</figure>\n\n");
						dataType=0;
					}


					// First line of poem
					// We do not print to file immediately bc we need to wait to count lines
					else if(thisLine.startsWith("STARTPOEM"))
						dataType = poem;


					// End of poem section. Print all lines in this section and clear poemQueue.
					else if(thisLine.startsWith("ENDPOEM")){
						numOfLines = poemQueue.size();
						endLine = linePlace + numOfLines - 1;
						p.print("\n<lg xml:id=\"Te_"+String.format("%04d", linePlace)+"-"
								+String.format("%04d", endLine)+"\">\n"); // Forces format to 4 digits


						// Drop cap tagging assumes format: D- De Fauvel que tant... with D drop capped
						for(String s : poemQueue){
							String dropCap = "";
							String start = "";
							if(s.substring(1,3).equals("- ")){
								dropCap = tagDropCap(s.substring(0,1));
								s = s.substring(3);
							}

							// Imagine « M- Mais combien...
							else if((s.startsWith("«")||s.startsWith("¬´")||s.startsWith(" «"))&&s.contains("-")){
								int index1 = s.indexOf("¬´");
								int index2 = s.indexOf("-");
								start = s.substring(0,index1+2);
								dropCap = tagDropCap(s.substring(index2-1,index2));
								s = s.substring(index2+1);
							}


							p.print("<l n=\""+linePlace+"\" pn=\""+pageLineNum+"\">"+dropCap+start+s+"</l>\n");
							linePlace++;
							pageLineNum++;
						}
						p.print("</lg>\n");
						poemQueue.clear();
						linePlace = endLine + 1; // Updates which line we've reached in the poem
						dataType = 0;
					}

					else if(thisLine.startsWith("Motet")){
						numOfMotets++;
						dataType = motet;
						ID = pageString + "Mo" + numOfMotets;
						p.print("\n<notatedMusic xml:id=\""+ID+"\">\n<ptr\ntarget=\"\"" +
								"\nmimeType=\"\" />\n</notatedMusic>\n");

						p.print("\n\n<p xml:id=\""+ ID +"_t\">" + thisLine + "\n<nv>  </nv>\n");

					}


					else if(isMusicGenre(thisLine, musicGenres)&&dataType==0&&!thisLine.startsWith("Motet")){
						Music thisMusic = whichGenre(thisLine,musicGenres); // IDs music genre
						ID = pageString + thisMusic.nickname + thisMusic.numOnPage;
						p.print("\n<notatedMusic xml:id=\""+ID+"\">\n<ptr\ntarget=\"\"" +
								"\nmimeType=\"\" />\n</notatedMusic>\n");
						dataType = music;
						// Refrain 3: T- Tout le cuer... (434)
						if(thisMusic.name.equals("Refrain")&&thisLine.contains(":")&&thisLine.contains("-")){
							int index1 = thisLine.indexOf(":");
							int index2 = thisLine.indexOf("-");
							String start = thisLine.substring(0, index1+1);
							String dropCap = tagDropCap(thisLine.substring(index2-1,index2));
							thisLine = thisLine.substring(index2+1);
							p.print("\n<p xml:id=\""+ ID +"_t\">"+dropCap+start+thisLine+"\n");
						}
						else
							p.print("\n<p xml:id=\""+ ID +"_t\">" + thisLine + "\n");

					}


					// Last line of any type of music
					else if(thisLine.startsWith("ENDMUSIC")){
						p.print("</p>\n");
						dataType = 0;
					}



					else if(thisLine.startsWith("Rubrique")){
						numOfRubriques++;
						p.print("\n<text xml:id=\""+pageString+"Rub"+numOfRubriques+"\">\n<rubrique>\n"
								+thisLine+"\n</rubrique>\n</text>\n\n");
						dataType=0;
					}

					// Single case on Strubel (574)
					else if(thisLine.startsWith("Prière")||thisLine.startsWith("PrieÃÄre")){
						p.print("\n<text xml:id=\""+pageString+"Pri\">\n<priere>\n"+thisLine+"\n</priere>\n</text>\n\n");
						dataType=0;
					}

					// This is specifically for the first of the last two lines... 
					// Do we need a way to deal with it in the code, or should it be manual fix?
					else if(thisLine.startsWith("Explicit, expliceat")){
						dataType = explicit;
						p.print("\n<text xml:id=\""+pageString+"Ex\">\n<explicit>\n"
								+thisLine+"\n");
					}


					// Line of text from poem or lyrics from music
					// We don't want to accidentally print "END--" or "START--" lines though!
					// Drop cap capabilities have been added in
					else if(!thisLine.startsWith("END")&&!thisLine.startsWith("START")&&!thisLine.equals(" ")){

					
						if(dataType==music||dataType==motet){ // Lyrics
							if(thisLine.contains("mélodie")||thisLine.contains("meÃÅlodie")
									||thisLine.contains("Mélodie")||thisLine.contains("MeÃÅlodie")){
								p.print(thisLine+"\n");
							}
							else{
								String dropCap = "";
								String start = "";

								// Imagine a line (Duplum) P- Presidente... 
								if(thisLine.startsWith("(")&&thisLine.contains(")")){
									int temp = thisLine.indexOf(")");
									if(thisLine.contains("-")){
										int index1 = thisLine.indexOf(")");
										int index2 = thisLine.indexOf("-");
										dropCap = tagDropCap(thisLine.substring(index2-1,index2));
										String parentheses = thisLine.substring(0, index1+1);
										thisLine = thisLine.substring(index2+2);
										p.print(dropCap+parentheses+" "+thisLine+"\n");
									}
									else if(temp==thisLine.length()-1) // Ends in )
										p.print(thisLine+"\n");
									else if(thisLine.contains("(refrain"))
										p.print(thisLine+"\n");
									else
										p.print(thisLine+"\n");

								}

								else if(thisLine.substring(1,2).equals("-")){ 
									dropCap = tagDropCap(thisLine.substring(0,1));
									if(thisLine.length()>=3){
										thisLine = thisLine.substring(3);
										p.print(dropCap+thisLine+"\n");
									}
									else
										p.print(dropCap+"\n");

								}

								else if((thisLine.startsWith("«")||thisLine.startsWith("¬´"))&&thisLine.contains("-")){
									int index1 = thisLine.indexOf("¬´");
									int index2 = thisLine.indexOf("-");
									start = thisLine.substring(0,index1+2);
									dropCap = tagDropCap(thisLine.substring(index2-1,index2));
									thisLine = thisLine.substring(index2+1);
									p.print(dropCap+" "+start+thisLine+"\n");
								}

								else
									p.print(thisLine+"\n");

							}
						}

						// Lines of a poem are added to queue; don't print til end of poem section
						else if(dataType==poem)
							poemQueue.add(thisLine);


						// For the very last line
						else if(dataType==explicit)
							p.print(thisLine+"\n</explicit>\n</text>\n\n");

					}
				}
			}


			// XML code for end of the document
			p.print("\n\n\n</body>\n</text>\n</xml>");

			p.close();
			in.close();

		} catch (FileNotFoundException e) {
			e.printStackTrace();
		}

	}


	private String tagDropCap(String aLine){
		String tagged = "";
		tagged = "<dc>" + aLine + "</dc> ";
		return tagged;
	}

	/**
	 * 	Creating a MusicGenre object for each type we find in this manuscript
	 */
	private void makeMusicGenres(){

		musicGenres = new ArrayList<Music>();

		musicGenres.add(new Music("Alleluia", "Al"));
		musicGenres.add(new Music("Antienne", "An"));
		musicGenres.add(new Music("Ballade/Chanson", "Vi")); // Is actually a Virelai
		musicGenres.add(new Music("Ballade", "Ba")); // Requires space after "Ballade" bc of "Ballade/Chanson"
		musicGenres.add(new Music("Chanson", "Chs"));
		musicGenres.add(new Music("Chant", "Chn"));
		musicGenres.add(new Music("Composition", "Com"));
		musicGenres.add(new Music("Conductus", "Con"));
		musicGenres.add(new Music("Fatras", "Fa"));
		musicGenres.add(new Music("Lay", "Lai", "La"));
		musicGenres.add(new Music("PieÃÄce finale", "Mo")); 	
		musicGenres.add(new Music("Prosa", "Prose", "Pr"));
		musicGenres.add(new Music("Refrain", "Sotte chanson", "Ref"));
		musicGenres.add(new Music("Repons", "Respons", "Rep"));
		musicGenres.add(new Music("Rondeau", "Ro"));
		musicGenres.add(new Music("Sequence", "Se"));
		musicGenres.add(new Music("Verset", "Ve"));

	}


	/**
	 * Checks if a line from Fauvel.txt indicates a type of music genre.
	 * Matches the first word(s) of the line against each Music object's name in the musicGenre ArrayList.
	 * @return Whether the line indicates a music genre.
	 */
	private boolean isMusicGenre(String aLine, ArrayList<Music> AL){
		boolean isMusic = false;
		for(Music m : AL){
			if(aLine.startsWith(m.name)||aLine.startsWith(m.altName))
				isMusic = true;
		}

		return isMusic;
	}

	/**
	 * Tells the program which music object has been found.
	 * @param aLine The line of text.
	 * @param AL The array list of music types.
	 * @return The music object that matches the type of text.
	 */
	private Music whichGenre(String aLine, ArrayList<Music> AL){
		Music whichGenre = new Music("", "");

		for(Music m : AL){
			if(aLine.startsWith(m.name)||aLine.startsWith(m.altName)){
				m.numOnPage++;
				whichGenre = m;
				break;
			}
		}

		return whichGenre;
	}


	/**
	 * Resets image/motet/etc count to 0
	 */
	private void newPage(){
		numOfImages = 0;
		numOfRubriques = 0;
		numOfMotets = 0;
		pageLineNum = 1;

		for(Music m : musicGenres){
			m.numOnPage = 0;
		}
	}

	/**
	 * Takes a string and removes any initial line numbers.
	 * @param s The string that is examined.
	 * @return The string without initial line numbers.
	 */
	private String lineNumChopper(String s){
		String fixedLine = s;
		boolean hasText = false;
		int firstLetter = 0;

		if(!s.isEmpty()){

			// If the line starts with/only has numbers
			if(Character.isDigit(s.charAt(0))){

				// Find out if there's any text at all
				// If not, we can discard the line completely
				for(int i = 0; i < s.length(); i++){
					if(Character.isLetter(s.charAt(i))){
						hasText = true;
						firstLetter = i;
						break;
					}
				}

				// Only keeps the pure text, not the line numbers
				if(hasText)
					fixedLine = s.substring(firstLetter);		

				else // If there are only numbers
					fixedLine = "";
			}
		}
		return fixedLine;
	}

}
