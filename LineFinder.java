
import java.awt.event.*;
import java.awt.image.BufferedImage;
import java.awt.*;
import java.io.*;

import javax.imageio.ImageIO;
import javax.swing.*;
import java.util.*;

public class LineFinder extends JFrame implements MouseListener, ActionListener, WindowListener {
	private static final long serialVersionUID = 1L;
	private JFrame frame;
	private ImageIcon myPic;
	private JPanel buttons;
	private JLabel myPicLabel;
	private JPanel myPicPanel;
	private JFileChooser fileChooser;
	private JMenuBar menuBar;
	private JMenu file;
	private JMenuItem open,save,exit;
	private JButton text, music, image, saveObject, done;
	private boolean left = true;
	private ManuscriptObject newObject;
	private ArrayList<ManuscriptObject> objects = new ArrayList<ManuscriptObject>();
	private int x1, y1, width, height;
	private int y1a, frameOffset; // Accounts for the pixels that make up the top of the frame
	Graphics2D g2d;
	Scanner in;
	File format = new File("layout.txt");
	PrintWriter p;
	FileWriter f;
	BufferedWriter b;
	BufferedImage img;
	BufferedImage smallimg;
	TableOfContents table = new TableOfContents();
	int foNum;
	String side;

	public LineFinder() throws IOException {
		frame = new JFrame("Object Recorder");
		buttons = new JPanel();
		text = new JButton("text");
		music = new JButton("music");
		image = new JButton("image");
		saveObject = new JButton("save object");
		done = new JButton("done w/ this folio");
		menuBar=new JMenuBar();
		file = new JMenu("File");
		open = new JMenuItem("Open");
		save = new JMenuItem("Save");
		exit = new JMenuItem("Exit");
		fileChooser = new JFileChooser();
		in = new Scanner(System.in);
		myPicPanel = new JPanel();
		frameOffset = 50;

		frame.setLayout(new BorderLayout());
		frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		frame.addMouseListener(this);

		OpenFileListener openListener = new OpenFileListener();
		open.addActionListener(openListener);
		text.addActionListener(this);
		music.addActionListener(this);
		image.addActionListener(this);
		saveObject.addActionListener(this);
		done.addActionListener(this);
		file.add(open);
		file.add(save);
		file.add(exit);
		menuBar.add(file);
		frame.setJMenuBar(menuBar);

		buttons.setLayout(new GridLayout(1,5));
		buttons.add(text);
		buttons.add(music);
		buttons.add(image);
		buttons.add(saveObject);
		buttons.add(done);
		frame.add(buttons, BorderLayout.SOUTH);
		frame.setSize(500, 775);
		frame.setVisible(true);

		System.out.println("Please start by opening a folio image from File.");

		p = new PrintWriter(format);
		p.print("<xml>\n<facsimile>\n");
		p.close();
	}


	@Override
	public void mouseClicked(MouseEvent e) { // need to scale recorded coordinates BACK
		if (left) {
			if (newObject == null) {
				System.out.println("Please click type first.");
			}
			else {
				newObject.topLeft = new Coordinate(e.getX(), e.getY()-frameOffset);
				System.out.println(newObject.topLeft);
				left = !left;
			}
		}
		else {
			newObject.bottomRight = new Coordinate(e.getX(), e.getY()-frameOffset);
			System.out.println(newObject.bottomRight);

			x1 = newObject.topLeft.x;
			y1 = newObject.topLeft.y;
			width = newObject.bottomRight.x - x1;
			height = newObject.bottomRight.y - y1;

			g2d = smallimg.createGraphics();
			g2d.setColor(Color.white);
			g2d.drawRect(x1, y1, width, height);

			frame.validate();
			frame.repaint();

			myPic = new ImageIcon(smallimg);
			myPicLabel = new JLabel("", myPic, JLabel.CENTER);
			myPicPanel.add(myPicLabel);
			myPicPanel.repaint();
			myPicPanel.revalidate();
		}	
	}


	private class OpenFileListener implements ActionListener {

		@Override
		public void actionPerformed(ActionEvent arg0) {
			// TODO Auto-generated method stub
			if(JFileChooser.APPROVE_OPTION == fileChooser.showOpenDialog(frame)) {
				System.out.println("folio/page number: (Enter as \"3 r\", \"21 v\", etc)");
				foNum = in.nextInt();
				side = in.next();
				System.out.println("On "+foNum+side+", you should find the following objects:");
				System.out.println(table.contents(foNum, side));

				if ((side.equals("r")) || (side.equals("v"))) {
					try {
						f = new FileWriter("layout.txt", true);
						b = new BufferedWriter(f);
						b.write("<surface id=\"" + foNum + side + "\">\n");
						b.write("<zone\n" + "id=\"" + foNum + side + "_p\"\n");
						b.write("ulx=\"0\"\n" + "uly=\"0\"\n" + "lrx=\"5250\"\n" + "lry=\"7350\">\n");

					} catch (IOException e) {
						// TODO Auto-generated catch block
						e.printStackTrace();
					}
					File file = fileChooser.getSelectedFile();
					try { // original file will be w: 5250 and h: 7350. scaled by 10.5
						b.write("<graphic url=\"" + file.getName() + "\" />\n");
						b.write("</zone>\n");
						img = ImageIO.read(file);
						smallimg = createResizedCopy(img, 500, 700, false);
						myPic = new ImageIcon(smallimg);
						myPicLabel = new JLabel("", myPic, JLabel.CENTER);
						myPicPanel = new JPanel();
						myPicPanel.add(myPicLabel);
						frame.add(myPicPanel, BorderLayout.CENTER);
						frame.validate();
					} catch (IOException e1) {
						// TODO Auto-generated catch block
						e1.printStackTrace();
					}

					try {
						b.close();
						f.close();
					} catch (IOException e) {
						// TODO Auto-generated catch block
						e.printStackTrace();
					}
				}
				else
					System.out.println("Invalid folio. Please try loading image again.");
			}	
		}
	}

	BufferedImage createResizedCopy(Image originalImage, 
			int scaledWidth, int scaledHeight, 
			boolean preserveAlpha)
	{
		int imageType = preserveAlpha ? BufferedImage.TYPE_INT_RGB : BufferedImage.TYPE_INT_ARGB;
		BufferedImage scaledBI = new BufferedImage(scaledWidth, scaledHeight, imageType);
		Graphics2D g = scaledBI.createGraphics();
		if (preserveAlpha) {
			g.setComposite(AlphaComposite.Src);
		}
		g.drawImage(originalImage, 0, 0, scaledWidth, scaledHeight, null); 
		g.dispose();
		return scaledBI;
	}

	@Override
	public void mouseEntered(MouseEvent e) {
		// TODO Auto-generated method stub

	}

	@Override
	public void mouseExited(MouseEvent e) {
		// TODO Auto-generated method stub

	}

	@Override
	public void mousePressed(MouseEvent e) {

	}

	@Override
	public void mouseReleased(MouseEvent e) {
		// TODO Auto-generated method stub

	}

	private boolean contdObj(ManuscriptObject find) {
		if (objects.size() < 2)
			return false;
		if (objects.get(objects.size()-2).name.equals(find.name))
			return true;
		return false;
	}

	@Override
	public void actionPerformed(ActionEvent arg0) {
		if(arg0.getSource().equals(text)) {
			if (newObject != null)
				System.out.println("You have already specified a type for this object.");
			else {
				System.out.println("text object ID:");
				String tName = in.next();
				newObject = new ManuscriptObject(tName, "text");
			}
		}
		else if(arg0.getSource().equals(music)) {
			if (newObject != null)
				System.out.println("You have already specified a type for this object.");
			else {
				System.out.println("music object ID:");
				String mName = in.next();
				newObject = new ManuscriptObject(mName, "music");
			}
		}
		else if(arg0.getSource().equals(image)) {
			if (newObject != null)
				System.out.println("You have already specified a type for this object.");
			else {
				System.out.println("image object ID:");
				String iName = in.next();
				newObject = new ManuscriptObject(iName, "image");
			}
		}
		else if(arg0.getSource().equals(saveObject)){
			System.out.println("add this object? enter \"y\" if yes and anything else if no");
			if (in.next().equals("y")) {
				objects.add(newObject);
				try {
					f = new FileWriter("layout.txt", true);
					b = new BufferedWriter(f);
					if (contdObj(newObject)) {
						if (newObject.type.equals("text")) { // script????
							b.write("<box\n");
							b.write("ulx=\"" + ((int) (10.5*newObject.topLeft.x)) + "\"\n" 
									+ "uly=\"" + ((int) (10.5*newObject.topLeft.y)) + "\"\n" 
									+ "lrx=\"" + ((int) (10.5*newObject.bottomRight.x)) + "\"\n" 
									+ "lry=\"" + ((int) (10.5*newObject.bottomRight.y)) + "\">\n" + "</box>\n");
						}
						else {
							b.write("<box\n");
							b.write("ulx=\"" + ((int) (10.5*newObject.topLeft.x)) + "\"\n" 
									+ "uly=\"" + ((int) (10.5*newObject.topLeft.y)) + "\"\n" 
									+ "lrx=\"" + ((int) (10.5*newObject.bottomRight.x)) + "\"\n" 
									+ "lry=\"" + ((int) (10.5*newObject.bottomRight.y)) + "\">\n" + "</box>\n");
						}
					}
					else {
						if (objects.size() != 1)
							b.write("</zone>\n");
						if (newObject.type.equals("text")) { // script????
							b.write("<zone\n" + "id=\"" + newObject.name + "\">\n");
							b.write("<box\n");
							b.write("ulx=\"" + ((int) (10.5*newObject.topLeft.x)) + "\"\n" 
									+ "uly=\"" + ((int) (10.5*newObject.topLeft.y)) + "\"\n" 
									+ "lrx=\"" + ((int) (10.5*newObject.bottomRight.x)) + "\"\n" 
									+ "lry=\"" + ((int) (10.5*newObject.bottomRight.y)) + "\">\n" + "</box>\n");
						}
						else {
							b.write("<zone\n" + "id=\"" + newObject.name + "\">\n");
							b.write("<box\n");
							b.write("ulx=\"" + ((int) (10.5*newObject.topLeft.x)) + "\"\n" 
									+ "uly=\"" + ((int) (10.5*newObject.topLeft.y)) + "\"\n" 
									+ "lrx=\"" + ((int) (10.5*newObject.bottomRight.x)) + "\"\n" 
									+ "lry=\"" + ((int) (10.5*newObject.bottomRight.y)) + "\">\n" + "</box>\n");
						}
					}
					b.close();
					f.close();
				}
				catch (IOException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				System.out.println(newObject.name + " added");
			}
			else
				System.out.println(newObject.name + " not added");
			left = !left;
			g2d.dispose();
			newObject = null;
		}
		else if(arg0.getSource().equals(done)){
			System.out.println("Are you sure you are done with this folio? Enter \"y\" if yes and anything else if no");
			if (in.next().equals("y")) {
				for (ManuscriptObject mo : objects) {
					System.out.println(mo);
				}
				try {
					f = new FileWriter("layout.txt", true);
					b = new BufferedWriter(f);
					b.write("</zone>\n");
					b.write("</surface>\n");
					b.close();
					f.close();
				}
				catch (IOException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				frame.remove(myPicPanel);
				frame.validate();
				frame.repaint();
				objects.clear();
				p.close();
			}
			else
				System.out.println("Continuing with this folio.");
		}
	}


	@Override
	public void windowActivated(WindowEvent arg0) {
		// TODO Auto-generated method stub

	}


	@Override
	public void windowClosed(WindowEvent arg0) {
		// TODO Auto-generated method stub
	}


	@Override
	public void windowClosing(WindowEvent arg0) {
	}


	@Override
	public void windowDeactivated(WindowEvent arg0) {
		// TODO Auto-generated method stub
	}


	@Override
	public void windowDeiconified(WindowEvent arg0) {
		// TODO Auto-generated method stub

	}


	@Override
	public void windowIconified(WindowEvent arg0) {
		// TODO Auto-generated method stub

	}


	@Override
	public void windowOpened(WindowEvent arg0) {
		// TODO Auto-generated method stub

	}

	public static void main(String[] args) throws IOException {
		LineFinder t = new LineFinder();
	}
}
