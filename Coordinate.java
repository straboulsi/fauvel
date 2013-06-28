
public class Coordinate {

	public int x;
	public int y;
	
	public Coordinate(int anX, int aY){
		x = anX;
		y = aY;
	}
	
	public String toString(){
		return (int) (10.5*x) + " " + (int) (10.5*y);
	}

}
