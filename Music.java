/**
 * This class defines a "Music" object that could be found in Fauvel.
 * Each object has one name, if not more, as well as a nickname used for reference (i.e. 1rMo1).
 * @author alisonychang
 */
public class Music {

	public String name;
	public String altName = "defaultAltName";
	public String nickname; // Used for reference, i.e. 1rMo1 - "Mo" is nickname
	public int numOnPage = 0; // Resets at the start of every folio page

	
	public Music(String aName, String aNickname){
		name = aName;
		nickname = aNickname;
	}
	
	public Music(String aName, String anAltName, String aNickname){
		name = aName;
		altName = anAltName;
		nickname = aNickname;
	}
	

}
