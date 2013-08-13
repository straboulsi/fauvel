/**
 * Used in LineFinder.java to record Coordinates of objects in Fauvel.
 * Primary coders: Alison Y. Chang and Jamie Chong
 */
public class Coordinate {

	public int x;
	public int y;
	
	public Coordinate(int anX, int aY){
		x = anX;
		y = aY;
	}
	
    // Used to print out and check the coordinates
	public String toString(){
		return (int) (10.5*x) + " " + (int) (10.5*y);
	}

}
