import java.io.*;
import java.util.*;


public class EnglishXML {

	File file1 = new File("EnglishXML.txt");

	public EnglishXML(File file){
		readFile(file);
	}

	public void readFile(File aFile){
		try {
			Scanner in = new Scanner(aFile);
			PrintWriter p = new PrintWriter(file1);
			int i = 1;

			p.print("<xml>\n<text>\n<body>\n<lg id=\"Te_0001-5986\">\n");

			while(in.hasNext()){
				String str = in.nextLine();
				if(str!=""){
					if(str.contains("[")&&str.contains("]")&&!str.contains("dead")){

						// Get rid of the [editorial comments]
						int index1 = str.indexOf("[");
						int index2 = str.indexOf("]");
						if(index2==str.length()-1){
							str = str.substring(0, index1);
						}

						// Get rid of trailing whitespace if there's more than 2

					}

					// Get rid of trailing whitespace
					if(str.contains("   ")){
						//System.out.println("SPACE: "+str);
						int lastLetter = 0;
						int numOfSpaces = 0;
						for(int k = 0; k<str.length(); k++){
							if(!Character.isWhitespace(str.charAt(k))){
								lastLetter = k;
							}
							else if(Character.isWhitespace(str.charAt(k))){
								numOfSpaces++;
							}
						}
						if(numOfSpaces > 2)
							str = str.substring(0, lastLetter+1);

					}
					
					// Get rid of trailing tabs
					if(str.contains("	")){
						//System.out.println("TAB: "+str);
						int index = str.indexOf("	");
						str = str.substring(0, index);
					}

					p.print("<l n=\""+i+"\"> "+str+"</l>\n");
					i++;
				}
			}

			p.print("</lg>\n</body>\n</text>\n</xml>");

			in.close();
			p.close();
		} catch (Exception e){
			e.printStackTrace();
		}

	}



	public static void main(String[] args) {
		EnglishXML eXML = new EnglishXML(new File("EnglishTrans.txt"));

	}

}
