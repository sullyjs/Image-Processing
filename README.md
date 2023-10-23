# Image-Processing
An image processing app, for a university assignment.

This image processing app is made with C# and Windows.Forms. 

It includes the following functions to manipulate photos:
- Inversion: a function (invertImage) that inverts the values of a single channel grayscale image.
- Contrast adjustment: a function (adjustContrast) that takes a grayscale image and outputs the image with the full range of intensity values used (0-255).
= Gaussian filter: a function (createGaussianFilter) that takes a (square) kernel size and a standard deviation and returns a square Gaussian kernel of the requested size (sum of all elements is 1). 
- Linear filtering: a function (convolveImage) that takes an input image and a kernel (e.g., from createGaussianFilter) and outputs the result after linear filtering with this kernel. 
- Nonlinear filtering: a function (medianFilter) that takes an input image and a median filter size and outputs the result after applying the filter. 
- Edge detection:  a function (edgeMagnitude) that takes an input image and a horizontal and vertical edge kernel and outputs an image with the edge strength per pixel. 
Thresholding
- -Histogram Equalization: a function for histogram equalization.
- Edge sharpening: a function for edge sharpening
- Structuring element: a function (createStructuringElement) that takes as input the structure element shape (plus or square) and size and outputs the corresponding structure element. For the plus-shaped element, the result should be the same as an iterative dilation of a 3x3 plus-shaped structuring element with a 3x3 plus-shaped structuring element.
- Erosion/dilation: two functions (erodeImage and dilateImage) that take a grayscale image and a structuring element as input and output the eroded/dilated image, respectively.
- Opening/closing: two functions (openImage and closeImage) that take a grayscale image and a structuring element as input and output the image after morphological opening and closing, respectively.
- AND/OR: implement two functions (andImages and orImages) that take two binary images as input and output the pixel-wise AND and OR of both images, respectively.
- Value counting: a function (countValues) that takes a grayscale image as input and outputs (1) the number of distinct values and (2) a histogram how often each value occurs.
- Boundary trace: a function (traceBoundary) that, given a binary image, traces the outer boundary of a foreground shape in that image. The output of the function is a list of (x,y)-pairs, each corresponding to a boundary pixel.
- Hough transform: a function (houghTransform) with an image as input, output is a Hough transform (r-theta image) of the image.
- Hough peak finding: a function (peakFinding) to provide (r, theta)-pairs whose Hough transform values are above a certain peak threshold (either in pixels or %), given an image and this threshold.
- Hough line detection: a function (houghLineDetection) that takes an image and a (r, theta)-pair (from the Hough peak finding function), a minimum intensity threshold (for grayscale images), a minimum length parameter and a maximum gap parameter, and outputs a list of line segments.
- Hough visualization: a function (visualizeHoughLineSegments) that, based on an image and a list of line segment descriptions (from the Hough line detector), outputs an image with the corresponding line segments superimposed.
- Hough angle limits: a Hough transform function (houghTransformAngleLimits) that takes additional lower and upper limits for the angle.
