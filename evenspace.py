# This is simpler version of findlines.py that instead of using Bonnie's
# algorithm, simply spaces out the lines evenly.

import numpy as np
import scipy.ndimage as ndimage
import scipy.spatial as spatial
import scipy.misc as misc
import matplotlib.pyplot as plt
import matplotlib.patches as patches
from pylab import imshow, gray, show, imsave
import PIL
from PIL import Image, ImageOps, ImageDraw, ImageFont
import os, string, operator, math, codecs, sys

def find_lines(ulx, uly, lrx, lry):
	global numLines

	ysize = lry - uly
	
	line_height = int(ysize/numLines)
	start_y = uly
	boxes = []
	for i in range(numLines):
		new_box = [ulx, start_y, lrx, start_y + line_height]
		start_y = new_box[1] + line_height

		boxes.append(new_box)

	return boxes

if __name__ == '__main__':
	global numLines

	lstart = int(b[0])
	lend = int(b[1])

	numLines = lend - lstart + 1

	ulx = int(b[2])
	uly = int(b[3])
	lrx = int(b[4])
	lry = int(b[5])

	# Find lines
	boxes = find_lines(ulx, uly, lrx, lry)

	# Print the line coordinates
	for i in range(len(boxes)):
		with open ("linelayout.txt", "a") as myfile:
			myfile.write("<l n=\"" + str(lstart+i) + "\" pn=\"" + str(i) + "\n")
			myfile.write("ulx=\"" + str(boxes[i][0]) + "\"\n")
			myfile.write("uly=\"" + str(boxes[i][1]) + "\"\n")
			myfile.write("lrx=\"" + str(boxes[i][2]) + "\"\n")
			myfile.write("lry=\"" + str(boxes[i][3]) + "\"></l>\n")

