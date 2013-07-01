# This is Jamie's edit on Bonnie's script.
# should eventually take in a full folio image with box coordinates of text and identify lines
# in that way

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

#I know it's a bit janky, but this is how the folder layout is defined.
#Clone from git and it should work.
dir_folios = 'folios/'
dir_bin_folios = dir_folios + 'bin/'
dir_columns = 'columns/'
dir_bin_columns = dir_columns + 'bin/'
dir_line_imgs = 'lines/'
dir_word_imgs = 'words/'

#Helper methods for doing math on pictures
def get_row_value(y):
	return math.fsum([pix[x,y] for x in range(ulx, lrx)])
def get_col_value(x):
	return math.fsum([pix[x,y] for y in range(uly, lry)])

def get_box_val(y1, y2):	
	if y2 > lry:
		y2 = lry
	row_sums = [get_row_value(y) for y in range(y1, y2)]
	box_val = math.fsum(row_sums)
	return box_val

def get_box_val_xy(x1, x2, y1, y2, im):
	pixels = im.load()
	if y2 > lry:
		y2 = lry
	if x2 > lrx:
		x2 = lrx
	k = sum([pixels[x,y] for x in range(x1, x2) for y in range(y1, y2)])
	return k

# Guesses where a line might be. Fudge factor limits its search field.
def line_in_range(ymin, line_height, fudge):
	boxes = [get_box_val(k+ymin, k+ymin+line_height) for k in range(fudge)]
	best_box = max(boxes)
	best_y = boxes.index(best_box) + ymin
	return (best_y, best_box)

def weighted_box_val(y1, y2):
	# Returns a weighted value for a box, favoring middle pixels.
	if y2 > ysize:
		y2 = ysize
	col_sums = [get_col_value(x) for x in range(0, xsize)]
	mid = len(col_sums)/2.0

	col_weights = [1.0 / round((abs(mid-x)) + 1) for x in range(len(col_sums))]
	weighted = [i * j for i, j in zip(col_sums, col_weights)]
	w = math.fsum(weighted)
	return w

# Binarizes an image, taking only pixels above the mean pixel value.
def preprocess_thresh(infile, outfile):
	im = misc.imread(infile, True)	
	im = (im > im.mean())
	misc.imsave(outfile, im)

# Finds the lines! Wooo.
def find_lines(inpath, ulx, uly, lrx, lry, save_file=False, show_file=False):
	# These shouldn't really be global; it could be cleaned up.
	global xsize
	global ysize
	global pix

	# Load into PIL
	im = ImageOps.invert(ImageOps.grayscale(Image.open(inpath)))
	pix = im.load()
	xsize = lrx - ulx
	ysize = lry - uly
	
	line_height = 70
	fudge = 40
	start_y = uly
	boxes = []
	for i in range(100):
		new_box = line_in_range(start_y, line_height, fudge)
		start_y = new_box[0] + line_height

		if get_box_val(new_box[0], new_box[0] + line_height) == 0:
			break
		boxes.append(new_box[0])

	box_vals = [get_box_val(y, y+line_height) for y in boxes]
	med = np.median(box_vals)

	filtered_boxes = filter(
		lambda y: get_box_val(y,y+line_height) > med/2.0
					and get_box_val(y,y+line_height) < med*2,
					boxes)

	# left, upper, right, and lower
	final_boxes = [(ulx, y, lrx, y+line_height) for y in filtered_boxes]
	return final_boxes

# Performs particle condensation (see report)
def condense_particles(words):
	i = 0
	while (i < len(words)):
		if len(words[i]) == 2 and i + 1 < len(words):
			w = words[i]
			words[i+1] = w + words[i+1]
			del(words[i])
		i += 1
	return words

# Crops & saves images for output.
def save_lines(boxes, orig, savedir):
	for i in range(len(boxes)):
		b = boxes[i]
		line_crop = orig.crop(b)
		new_name = savedir + 'line' + str(i) + '.png'
		line_crop.save(new_name)
	return

# Boxes: a list of tuples (x1, y1, x2, y2)
# Words: a list of words
def overlay_boxes(boxes, img_name, save_path='', excludes=[], words=None):
	# Load the image for display
	scimg = misc.imread(img_name)
	fig = plt.figure()
	ax = fig.add_subplot(111)
	ax.imshow(scimg, cmap=plt.cm.gray)
	color = 'red'
	for i in range(len(boxes)):
		(x1, y1, x2, y2) = boxes[i]
		if (i % 2 == 0):
			color = 'blue'
		else:
			color = 'red'
		p = patches.Rectangle((x1, y1), x2-x1, y2-y1,
							fc=color, ec=color, alpha=0.2);
		ax.add_patch(p)

		# Overlay text?
		if words:
			word_str = ''
			try:
				word_str = unicode(words[i])
				#This doesn't totally work, so it's
				# commented out to avoid breakage.

				"""
				plt.text(x1, y1, word_str,
				        horizontalalignment='left',
				        verticalalignment='top',
				        size=13
				        )
				"""

			except:
				print 'except'


	for i in range(len(excludes)):
		(x1, y1, x2, y2) = excludes[i]
		p = patches.Rectangle((x1, y1), x2-x1, y2-y1, fc='green',
			ec='green', alpha=0.2)
		ax.add_patch(p)
	if save_path:
		print "Saving to: ", save_path
		plt.savefig(save_path, dpi=300, facecolor='0.75', format='png')
	else:
		plt.show()

if __name__ == '__main__':
	global xsize
	global ysize
	global lrx
	global lry
	global ulx
	global uly
	global pix

	# Load column image
	b = raw_input().split(' ')
	filestem = b[0]
	filename = filestem + '.jpg'
	orig_loc = dir_folios + filename
	orig_im = ImageOps.invert(ImageOps.grayscale(Image.open(orig_loc)))

	ulx = int(b[1])
	uly = int(b[2])
	lrx = int(b[3])
	lry = int(b[4])

	# Preprocess
	bin_img_path = dir_bin_folios + filename
	preprocess_thresh(orig_loc, bin_img_path)

	# Find lines
	boxes = find_lines(bin_img_path, ulx, uly, lrx, lry)
	bin_im = ImageOps.invert(ImageOps.grayscale(Image.open(bin_img_path)))

	overlay_boxes(boxes, orig_loc, save_path=dir_line_imgs + filename)

	# Print the line coordinates
	for i in range(len(boxes)):
		print boxes[i]

