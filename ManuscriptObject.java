
public class ManuscriptObject {

	public String name;
	public String type;
	Coordinate topLeft;
	Coordinate bottomRight;
	
	public ManuscriptObject(String aName, String aType){
		name = aName;
		type = aType;
	}
	
	public ManuscriptObject(String aName, String aType, Coordinate one, Coordinate two){
		name = aName;
		type = aType;
		topLeft = one;
		bottomRight = two;	
	}
	
	public String toString() {
		return name + " " + type + " " + topLeft.toString() + " " + bottomRight.toString();
	}

}
