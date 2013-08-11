import java.util.*;
import java.io.*;

/**
 * Creates a hash map of the name of each page and its contents
 */
public class TableOfContents {

	public String folioContents = "";
	String folioName, nextPage;
	HashMap TOC = new HashMap();
	
	public TableOfContents(){


		String page = "";
		String contents = "";
		
		try{

			Scanner in = new Scanner(new File("FolioObjects.txt"));

			while(in.hasNext()){
				String thisLine = in.nextLine();

				if(thisLine.startsWith("<pb facs")){
					contents = "";
					int index1 = thisLine.indexOf("#");
					int index2 = thisLine.indexOf("\" /");
					page = thisLine.substring(index1+1,index2);
				}
				else if(!thisLine.isEmpty()&&Character.isDigit(thisLine.charAt(0)))
					contents += thisLine+"\n";
					
				else if(thisLine.isEmpty()&&!contents.isEmpty())
					TOC.put(page, contents);
				
			}

		} catch (Exception e){
			e.printStackTrace();
		}


	}
	
	public String contents(int aPage, String aSide){
		String searchPage = String.valueOf(aPage)+aSide;
		String contents = TOC.get(searchPage).toString();
		
		
		return contents;
	}

}
