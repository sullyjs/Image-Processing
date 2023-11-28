using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace INFOIBV
{
    public partial class INFOIBV : Form
    {
        private Bitmap InputImage;
        private Bitmap OutputImage;
        private byte[,] prevOutput;

        public INFOIBV()
        {
            InitializeComponent();
        }

        /*
         * loadButton_Click: process when user clicks "Load" button
         */
        private void loadImageButton_Click(object sender, EventArgs e)
        {
           if (openImageDialog.ShowDialog() == DialogResult.OK)             // open file dialog
            {
                string file = openImageDialog.FileName;                     // get the file name
                imageFileName.Text = file;                                  // show file name
                if (InputImage != null) InputImage.Dispose();               // reset image
                InputImage = new Bitmap(file);                              // create new Bitmap from file
                if (InputImage.Size.Height <= 0 || InputImage.Size.Width <= 0 ||
                    InputImage.Size.Height > 512 || InputImage.Size.Width > 512) // dimension check (may be removed or altered)
                    MessageBox.Show("Error in image dimensions (have to be > 0 and <= 512)");
                else
                    pictureBox1.Image = (Image) InputImage;                 // display input image
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            // check if and or or is selected and if there is a previous output
            if(andOrOperationDropDown.SelectedItem != null && prevOutput == null)
            {
                MessageBox.Show("create an output first, after that you can select an AND/OR operation which uses that first output and a new output");
                andOrOperationDropDown.SelectedItem = null;
                return;
            }

            if (InputImage == null) { MessageBox.Show("Load an input image"); return; }                                // get out if no input image
            if (OutputImage != null) OutputImage.Dispose();                 // reset output image
            OutputImage = new Bitmap(InputImage.Size.Width, InputImage.Size.Height); // create new output image
            Color[,] Image = new Color[InputImage.Size.Width, InputImage.Size.Height]; // create array to speed-up operations (Bitmap functions are very slow)

            // copy input Bitmap to array            
            for (int x = 0; x < InputImage.Size.Width; x++)                 // loop over columns
                for (int y = 0; y < InputImage.Size.Height; y++)            // loop over rows
                    Image[x, y] = InputImage.GetPixel(x, y);                // set pixel color in array at (x,y)

            byte[,] greyImage = adjustContrast(convertToGrayscale(Image));          // convert image to grayscale

            byte[,] workingImage = greyImage;

            sbyte[,] sobelHorizontalKernel = new sbyte[,] { // chosen kernels from: https://www.cs.auckland.ac.nz/compsci373s1c/PatricesLectures/Edge%20detection-Sobel_2up.pdf
                { -1, 0, 1 },
                { -2, 0, 2 },
                { -1, 0, 1 }
            };

            sbyte[,] sobelVerticalKernel = new sbyte[,] {
                { -1, -2, -1 },
                { 0, 0, 0 },
                { 1, 2, 1 }
            };

            // work on the corresponding pipeline that is selected
            if(pipeLineDropDown.SelectedItem == "PipeLine 1")
            {
                // create parameter for the gaussian filter and create it
                byte size = 5;
                float sigma = 0.6f;
                float[,] gaussianFilter = createGaussianFilter(size,sigma);

                // create the working image by linearly filtering the image with the created gaussian filter
                workingImage = convolveImage(greyImage, gaussianFilter);

                // get an edge detected image with the designated function
                byte[,] edgeDetectedImage = edgeMagnitude(workingImage, sobelHorizontalKernel, sobelVerticalKernel);

                // get the eventually working image by thresholding the edge detected image
                workingImage = thresholdImage(edgeDetectedImage);
            }else if(pipeLineDropDown.SelectedItem == "PipeLine 2")
            {            
                // create the working image by using the median filter with size 5x5
                workingImage = medianFilter(greyImage, 5);

                // get an edge detected image with the designated function
                byte[,] edgeDetectedImage = edgeMagnitude(workingImage, sobelHorizontalKernel, sobelVerticalKernel);

                // get the eventually working image by thresholding the edge detected image
                workingImage = thresholdImage(edgeDetectedImage);
            }
            else if(pipeLineDropDown.SelectedItem == "binary image") { workingImage = thresholdImage(greyImage); }
            else workingImage = greyImage;

            // after the pipeline do assignment 2 filters
            // first check if even a assignment 2 filter is selected, otherwise just excecute the selected pipeline and nothing else
            if(morphologicalFilterDropDown.SelectedItem != null)
            {
                // check if the sturcturing element shape is selected, if so retrieve the selected one
                bool plus;
                if(StrucElemShapeDropDown.SelectedItem == null) { MessageBox.Show("please choose a structuring element shape"); return; }
                else if(StrucElemShapeDropDown.SelectedItem == "plus") { plus = true; }
                else { plus = false; }
                // check if the structuring element size has an input and if so check if it is correct/usable
                int strucSize;
                if(!int.TryParse(StrucElemShapeSizeTxtBox.Text, out strucSize) || strucSize < 3 || strucSize > 99 || strucSize % 2 != 1) 
                { MessageBox.Show("please fill in a structuring element size which is a number, odd and between 3 and 99"); return; }
                
                // create the structuring element
                byte[,] strucElem = createStructuringElement(plus, strucSize);

                // work out the selected morphological filter
                switch (morphologicalFilterDropDown.SelectedItem)
                {
                    case "erosion":
                        workingImage = erodeImage(workingImage, strucElem);
                        break;
                    case "dilation":
                        workingImage = dilateImage(workingImage, strucElem);
                        break;
                    case "opening":
                        workingImage = openImage(workingImage, strucElem);
                        break;
                    default:
                        workingImage = closeImage(workingImage, strucElem);
                        break;
                }
                
            }

            // if the and or or is selected call the right function
            if(andOrOperationDropDown.SelectedItem != null)
            {
                if(andOrOperationDropDown.SelectedItem == "AND")
                {
                    workingImage = andImages(workingImage, prevOutput);
                }
                else
                {
                    workingImage = orImages(workingImage, prevOutput);
                }
            }

            // if the value counting is checked than show the counted values
            if (valueCountingCheckBox.Checked)
            {
                ValueCountOutput count = countValues(workingImage);
                int distinctValues = count.DistinctValues;
                MessageBox.Show("Value Count: " + distinctValues);
            }
            // if the boundary tracing is checked than create an image with all the boundaries
            if (boundaryTracingCheckBox.Checked)
            {
                List<Tuple<int, int>> tracedImage = traceBoundary(workingImage);
                for(int i = 0; i < workingImage.GetLength(0); i++)
                {
                    for(int j = 0; j < workingImage.GetLength(1); j++)
                    {
                        if(tracedImage.Contains(new Tuple<int, int>(i, j))) { workingImage[i,j] = 255;}
                        else workingImage[i,j] = 0;
                    }
                }
                /*
                foreach(var item in tracedImage)
                {
                    MessageBox.Show("x coordinate: " + item.Item1 + ". y coordinate: " + item.Item2);
                }
                */
            }

            /*
             * For counting non background values
            int nonBackgroundValues = 0;
            for(int i = 0; i < Image.GetLength(0); i++)
            {
                for(int j = 0; j < Image.GetLength(1); j++)
                {
                    if(workingImage[i, j] != 0) nonBackgroundValues++;
                }
            }
            MessageBox.Show("Non-background values: " + nonBackgroundValues);
            */

            prevOutput = workingImage;

            if (BycicleWheels.Checked)
            {
                CompleteHoughTransform.Checked = true;
            }

            if (HoughTransformImage.Checked && CompleteHoughTransform.Checked)
            {
                MessageBox.Show("Choose only 1 hough checkbox");
            }
            else if (HoughTransformImage.Checked)
            {
                workingImage = edgeMagnitude(workingImage, sobelHorizontalKernel, sobelVerticalKernel);
                if(!String.IsNullOrEmpty(EdgeThreshold.Text))
                {
                    workingImage = thresholdImageBasedOnThreshold(workingImage, Convert.ToByte(EdgeThreshold.Text));

                    //MessageBox.Show("Edge is empty");
                }
                else
                {
                    workingImage = thresholdImageBasedOnThreshold(workingImage, 250);
                }
                workingImage = HoughTransform(workingImage);
                List<Tuple<int, int>> peaks = peakFinding(workingImage, Convert.ToInt32(PeakThreshold.Text));
                OutputImage = new Bitmap(workingImage.GetLength(0), workingImage.GetLength(1));
            }
            else if (CompleteHoughTransform.Checked)
            {
                if(String.IsNullOrEmpty(PeakThreshold.Text) || String.IsNullOrEmpty(MinThreshold.Text) || String.IsNullOrEmpty(LineLength.Text) || String.IsNullOrEmpty(PixelGap.Text))
                {
                    MessageBox.Show("make sure peak threshold, minthreshold, linelength and pixelgap are all filled in.");
                }
                workingImage = edgeMagnitude(workingImage, sobelHorizontalKernel, sobelVerticalKernel);

                if(!String.IsNullOrEmpty(EdgeThreshold.Text))
                {
                    workingImage = thresholdImageBasedOnThreshold(workingImage, Convert.ToByte(EdgeThreshold.Text));

                    //MessageBox.Show("Edge is empty");
                }                
                else
                {
                    workingImage = thresholdImageBasedOnThreshold(workingImage, 250);
                }                
                

                /*
                foreach(Tuple<int, int, int> circlePeak in circlePeaks)
                {
                    MessageBox.Show("center: x=" + circlePeak.Item1 + " y=" + circlePeak.Item2 + " radius: " + circlePeak.Item3);
                }*/
                



                byte[,] houghTransformImage = HoughTransform(workingImage);
                if(!(String.IsNullOrEmpty(LowerAngle.Text)) && !(String.IsNullOrEmpty(UpperAngle.Text)))
                {
                    houghTransformImage = HoughTransformAngleLimits(workingImage, Convert.ToInt32(LowerAngle.Text), Convert.ToInt32(UpperAngle.Text) );
                }

                int rOffset = houghTransformImage.GetLength(1) / 2;
                //workingImage = houghTransformImage;
                List<Tuple<int, int>> peaks = peakFinding(houghTransformImage, Convert.ToInt32(PeakThreshold.Text));
                List<Tuple<Tuple<int, int>, Tuple<int,int>, Tuple<int,int>>> lineSegments = 
                    new List<Tuple<Tuple<int, int>, Tuple<int,int>, Tuple<int,int>>>();
                //MessageBox.Show("peaks: " + peaks.Count());
                foreach(Tuple<int, int> peak in peaks)
                {
                    //MessageBox.Show("dit duurt lang");
                    //MessageBox.Show("r: " + peak.Item1 + " theta: " + peak.Item2);
                    lineSegments.AddRange(houghLineDetection(workingImage, peak, Convert.ToInt32(MinThreshold.Text), Convert.ToInt32(LineLength.Text), Convert.ToInt32(PixelGap.Text), rOffset));
                    //List<Tuple<Tuple<int, int>, Tuple<int,int>>> lineSegments = houghLineDetection(workingImage, peak, 180, 10, 2, rOffset);
                    //workingImage = visualizeHoughLineSegments(workingImage, lineSegments);

                }
                
                if (BycicleWheels.Checked)
                {
                    int minRadius = Convert.ToInt32(CircleMinR.Text);
                    int maxRadius = Convert.ToInt32(CircleMaxR.Text);
                    int circPeakThres = Convert.ToInt32(CirclePeakThres.Text);

                    byte[,,] houghTransformImageCircle = HoughTransformCircle(workingImage, maxRadius, minRadius);
                    List<Tuple<int, int, int>> circlePeaks = peakFindingCircle(houghTransformImageCircle, circPeakThres, minRadius);
                    List<Tuple<int,int,int>> bicycleWheels = bicycleWheel(circlePeaks, lineSegments);
                    workingImage = visualizeHoughLineSegments(workingImage, lineSegments);
                    workingImage = visualizeHoughCircles(workingImage, circlePeaks);
                    foreach(Tuple<int,int,int> bicyclewheel in bicycleWheels)
                    {
                        int centerX = bicyclewheel.Item1;
                        int centerY = bicyclewheel.Item2;
                        int radius = bicyclewheel.Item3;
                        MessageBox.Show("x:" + centerX + " y:" + centerY + " radius:" + radius);
                        int imageX = workingImage.GetLength(0);
                        int imageY = workingImage.GetLength(1);
                        for(int x = centerX - radius; x < centerX + radius; x++)
                        {
                            if(x > 0 && x < imageX)
                            {
                                if(centerY - radius > 0)
                                {
                                    workingImage[x, centerY - radius] = 197;
                                }
                                if(centerY + radius < imageY)
                                {
                                    workingImage[x, centerY + radius] = 197;
                                }
                            }
                        }
                        for(int y = centerY - radius; y < centerY + radius; y++)
                        {
                            if(y > 0 && y < imageY)
                            {
                                if(centerX - radius > 0)
                                {
                                    workingImage[centerX - radius, y] = 197;
                                }
                                if(centerX + radius < imageX)
                                {
                                    workingImage[centerX + radius, y] = 197;
                                }
                            }
                        }
                    }
                }
                else
                {
                   workingImage = visualizeHoughLineSegments(workingImage, lineSegments);
                }
                
            }



            
            //workingImage = visualizeHoughLineSegments(workingImage, lineSegments);
            // --- REST OF METHOD CODE FROM TEMPLATE SITUATED AFTER THE "YOUR FUNCTION CALLS HERE" SEGMENT
            // copy array to output Bitmap
            OutputImage = new Bitmap(workingImage.GetLength(0), workingImage.GetLength(1));
            for (int x = 0; x < workingImage.GetLength(0); x++)             // loop over columns
            { 
                for (int y = 0; y < workingImage.GetLength(1); y++)         // loop over rows
                {
                    Color newColor = Color.FromArgb(workingImage[x, y], workingImage[x, y], workingImage[x, y]);
                    if(workingImage[x, y] == 199 && CompleteHoughTransform.Checked) newColor = Color.FromArgb(255, 0, 0);
                    if(workingImage[x,y] == 201 && CompleteHoughTransform.Checked) newColor = Color.FromArgb(0, 255, 0);
                    if(workingImage[x,y] == 197 && CompleteHoughTransform.Checked) newColor = Color.FromArgb(0, 204, 255);
                    OutputImage.SetPixel(x, y, newColor);                  // set the pixel color at coordinate (x,y)
                    
                }
            }
            
            pictureBox2.Image = (Image)OutputImage;                         // display output image
        }


        /*
         * saveButton_Click: process when user clicks "Save" button
         */
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (OutputImage == null) return;                                // get out if no output image
            if (saveImageDialog.ShowDialog() == DialogResult.OK)
                OutputImage.Save(saveImageDialog.FileName);                 // save the output image
        }


        /*
         * convertToGrayScale: convert a three-channel color image to a single channel grayscale image
         * input:   inputImage          three-channel (Color) image
         * output:                      single-channel (byte) image
         */
        private byte[,] convertToGrayscale(Color[,] inputImage)
        {
            // create temporary grayscale image of the same size as input, with a single channel
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            // setup progress bar
            progressBar.Visible = true;
            progressBar.Minimum = 1;
            progressBar.Maximum = InputImage.Size.Width * InputImage.Size.Height;
            progressBar.Value = 1;
            progressBar.Step = 1;

            // process all pixels in the image
            for (int x = 0; x < InputImage.Size.Width; x++)                 // loop over columns
                for (int y = 0; y < InputImage.Size.Height; y++)            // loop over rows
                {
                    Color pixelColor = inputImage[x, y];                    // get pixel color
                    byte average = (byte)((pixelColor.R + pixelColor.B + pixelColor.G) / 3); // calculate average over the three channels
                    tempImage[x, y] = average;                              // set the new pixel color at coordinate (x,y)
                    progressBar.PerformStep();                              // increment progress bar
                }

            progressBar.Visible = false;                                    // hide progress bar

            return tempImage;
        }


        // ====================================================================
        // ============= YOUR FUNCTIONS FOR ASSIGNMENT 1 GO HERE ==============
        // ====================================================================

        /*
         * invertImage: invert a single channel (grayscale) image
         * input:   inputImage          single-channel (byte) image
         * output:                      single-channel (byte) image
         */
        private byte[,] invertImage(byte[,] inputImage)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            // TODO: add your functionality and checks

            for (int i = 0; i < inputImage.GetLength(0); i++)
            {
                for (int j = 0; j < inputImage.GetLength(1); j++)
                {
                    tempImage[i, j] = (byte)(255 - inputImage[i, j]); //255 - x voor elke pixel
                }
            }

            return tempImage;
        }


        /*
         * adjustContrast: create an image with the full range of intensity values used
         * input:   inputImage          single-channel (byte) image
         * output:                      single-channel (byte) image
         */
        private byte[,] adjustContrast(byte[,] inputImage)
        {
            int rows = inputImage.GetLength(0);
            int collumns = inputImage.GetLength(1);
            // create temporary grayscale image
            byte[,] tempImage = new byte[rows, collumns];

            // initialize minimum and maximum at their worst
            byte high = (byte) 0;
            byte low = (byte) 255;

            // loop over all the pixels to determine the current image contrast max and min points
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < collumns; y++)
                {
                    if(inputImage[x, y] < low) low = inputImage[x, y];
                    if(inputImage[x, y] > high) high = inputImage[x, y];
                }
            }
            // calculate the current image contrast
            byte contrast = (byte) (high - low);

            // change all pixels so that the contrast is adjusted to the full range of intensity values
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < collumns; y++)
                {
                    tempImage[x, y] = (byte) ((inputImage[x, y] - low) * (255 / (float)contrast));
                }
            }

            return tempImage;
        }


        /*
         * createGaussianFilter: create a Gaussian filter of specific square size and with a specified sigma
         * input:   size                length and width of the Gaussian filter (only odd sizes)
         *          sigma               standard deviation of the Gaussian distribution
         * output:                      Gaussian filter
         */
        private float[,] createGaussianFilter(byte size, float sigma)
        {
            // create temporary grayscale image
            float[,] filter = new float[size, size];
            float sum = 0;
            int halfSize = size / 2;

            // TODO: add your functionality and checks

            if (size % 2 == 0) //check if its odd
            {
                return null; //no filter
            }



            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int x = i - halfSize;
                    int y = j - halfSize;
                    filter[i, j] = (float)Math.Exp(-(x * x + y * y) / (2 * sigma * sigma));
                    sum += filter[i, j];
                }
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    filter[i, j] /= sum;
                }
            }

            return filter;
        }


        /*
         * convolveImage: apply linear filtering of an input image
         * input:   inputImage          single-channel (byte) image
         *          filter              linear kernel
         * output:                      single-channel (byte) image
         */
        private byte[,] convolveImage(byte[,] inputImage, float[,] filter)
        {
            int rows = inputImage.GetLength(0);
            int collumns = inputImage.GetLength (1);
            int kernelSize = filter.GetLength(0);
            int kernelEdge = (int)Math.Floor((float)kernelSize / 2);           // Size of the edge of pixels around the center pixel
            // create temporary grayscale image
            byte[,] tempImage = new byte[rows, collumns];
            // loop over all pixels
            for(int x = 0; x<rows; x++)
            {
                for(int y = 0; y < collumns; y++)
                {
                    // check if the current pixel is a border pixel
                    bool borderPixel = false;
                    if(x < kernelEdge || y < kernelEdge || x >= rows - kernelEdge || y >= collumns - kernelEdge) borderPixel = true;

                    // if the current pixel is a border pixel, give it the maximum intensity value
                    if(borderPixel) tempImage[x,y] = 255;
                    // otherwise linearly filter the pixel
                    else
                    {
                        float filteredPixel = 0;
                        // loop over all the pixels around the current pixel that fall in the kernel
                        for(int i = 0; i < kernelSize; i++)
                        {
                            for(int j = 0; j < kernelSize; j++)    
                            {
                                int imageX = x + (i - kernelEdge);
                                int imageY = y + (j - kernelEdge);
                                // add the current pixel in the kernel multiplied by its designated filter value and add to filtered pixel value
                                filteredPixel += inputImage[imageX, imageY] * filter[i, j];
                            }
                        }
                        // give the xy pixel the new filtered value
                        tempImage[x,y] = (byte) Math.Floor(filteredPixel);
                    }
                }
            }

            return tempImage;
        }


        /*
         * medianFilter: apply median filtering on an input image with a kernel of specified size
         * input:   inputImage          single-channel (byte) image
         *          size                length/width of the median filter kernel
         * output:                      single-channel (byte) image
         */
        private byte[,] medianFilter(byte[,] inputImage, byte size)
        {
            int rows = inputImage.GetLength(0);
            int collumns = inputImage.GetLength (1);

            // create temporary grayscale image
            byte[,] tempImage = new byte[rows, collumns];

            // loop over all pixels
            for(int x = 0; x < rows; x++)
            {
                for(int y = 0; y < collumns; y++)
                {
                    // check if the current pixel is a border pixel
                    bool borderPixel = false;
                    int filterEdge = (size - 1) / 2;
                    if(x < filterEdge || y < filterEdge || x >= rows - filterEdge || y >= collumns - filterEdge) borderPixel = true;
                    // if the current pixel is a border pixel than give the pixel it's original value from the original image
                    if(borderPixel) tempImage[x,y] = inputImage[x,y];
                    // otherwise median filter over this pixel
                    else
                    {
                        // initialize an array of all pixels surrounding the center pixel
                        byte[] surroundingPixels = new byte[size * size];
                        // loop over all the pixels that fall inside the kernel
                        for(int i = 0; i < size; i++)
                        {
                            for(int j = 0; j < size; j++)
                            {
                                // add the surrounding pixel to the surroundingpixels array
                                surroundingPixels[j * size + i] = inputImage[x + (i - filterEdge),y + (j - filterEdge)];
                            }
                        }
                        // sort the array
                        Array.Sort(surroundingPixels);
                        // and write the median pixel value of the sorted surrounding pixels array to the median filtered image
                        tempImage[x,y] = surroundingPixels[(surroundingPixels.Length - 1) / 2];
                    }
                }
            }

            return tempImage;
        }


        /*
         * edgeMagnitude: calculate the image derivative of an input image and a provided edge kernel
         * input:   inputImage          single-channel (byte) image
         *          horizontalKernel    horizontal edge kernel
         *          virticalKernel      vertical edge kernel
         * output:                      single-channel (byte) image
         */
        private byte[,] edgeMagnitude(byte[,] inputImage, sbyte[,] horizontalKernel, sbyte[,] verticalKernel)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            // TODO: add your functionality and checks, think about border handling and type conversion (negative values!)

            if (horizontalKernel.GetLength(0) == verticalKernel.GetLength(0) && horizontalKernel.GetLength(1) == verticalKernel.GetLength(1))
            {

                int rx = horizontalKernel.GetLength(0) / 2;
                int ry = horizontalKernel.GetLength(1) / 2;
                


                for (int i = rx; i < inputImage.GetLength(0) - rx; i++)
                {
                    for (int j = ry; j < inputImage.GetLength(1) - ry; j++)
                    {
                        int gx = 0; //gradientsss
                        int gy = 0;
                        for (int p = -rx; p <= rx; p++)
                        {
                            for (int q = -ry; q <= ry; q++)
                            {
                                gx += inputImage[i + p, j + q] * horizontalKernel[p + rx, q + ry];
                                gy += inputImage[i + p, j + q] * verticalKernel[p + rx, q + ry];
                            }
                        }

                        int m = (int)Math.Sqrt(gx * gx + gy * gy);
                        tempImage[i, j] = (byte)Math.Min(255, Math.Max(0, m));
                    }
                }

                return tempImage;
            } else
            {
                return inputImage;
            }
        }


        /*
         * thresholdImage: threshold a grayscale image
         * input:   inputImage          single-channel (byte) image
         * output:                      single-channel (byte) image with on/off values
         */
        private byte[,] thresholdImage(byte[,] inputImage)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            // TODO: add your functionality and checks, think about how to represent the binary values

            //byte[,] threshold = new byte[0, inputImage.GetLength(1)];

            for (int i = 0; i < inputImage.GetLength(0); i++)
            {
                for (int j = 0; j < inputImage.GetLength(1); j++)
                {
                    if (inputImage[i, j] >= 128)
                    {
                        tempImage[i, j] = 255; //max
                    }
                    else
                    {
                        tempImage[i, j] = 0;
                    }
                }
            }

            return tempImage;
        }

        private byte[,] thresholdImageBasedOnThreshold(byte[,] inputImage, byte threshold)
        {
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            for (int i = 0; i < inputImage.GetLength(0); i++)
            {
                for (int j = 0; j < inputImage.GetLength(1); j++)
                {
                    if (inputImage[i, j] >= threshold)
                    {
                        tempImage[i, j] = 255; // max
                    }
                    else
                    {
                        tempImage[i, j] = 0;
                    }
                }
       
            }

            return tempImage;
        }


        private byte[,] edgeSharpen(byte[,] inputImage)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            // laplacian kernel --> used this: http://www.idlcoyote.com/ip_tips/sharpen.html
            int[,] sharpenKernel = {
                { -1, -1, -1 },
                { -1, 8, -1 },
                { -1, -1, -1 }
            };
            for (int i = 0; i < inputImage.GetLength(0); i++)
            {
                for (int j = 0; j < inputImage.GetLength(1); j++)
                {
                    int sum = 0;
                    for (int p = -1; p <= 1; p++)
                    {
                        for (int q = -1; q <= 1; q++)
                        {
                            sum += inputImage[i + p, j + q] * sharpenKernel[p + 1, q + 1];
                        }
                    }
                    tempImage[i, j] = (byte)Math.Min(255, Math.Max(0, sum));
                }
            }

            return tempImage;
        }

        private byte[,] histogramEq(byte[,] inputImage)
        {

            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            int[] histogram = new int[255]; //make histogram
            for (int x = 0; x < inputImage.GetLength(0); x++)
            {
                for (int y = 0; y < inputImage.GetLength(1); y++)
                {
                    histogram[inputImage[x, y]]++;
                }
            }

            //look up how to histogram equalize online or on slides

            return tempImage;
        }


        
        // ====================================================================
        // ============= YOUR FUNCTIONS FOR ASSIGNMENT 2 GO HERE ==============
        // ====================================================================

        private byte[,] createStructuringElement(bool plus, int size)
        {
            byte[,] output = new byte[size, size];

            // create plus structuring element
            if (plus)
            {
                // start with 3x3 plus structuring element
                byte[,] plus3x3 = { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };
                // if that is the size return just that
                if (size == 3) return plus3x3;
                else                    // if larger than continue here
                {
                    // index for the middle element
                    int middle = size / 2;
                    // loop over the whole size the structuring element needs to be
                    for (int x = 0; x < size; x++)
                    {
                        for (int y = 0; y < size; y++)
                        {
                            // in the middle 3x3 of the (larger) structuring element fill it with the basic 3x3 plus structuring element
                            if ((x >= middle - 1 && x <= middle + 1) && (y >= middle - 1 && y <= middle + 1))
                            {
                                output[x, y] = plus3x3[x - (middle - 1), y - (middle - 1)];
                            }
                            else output[x, y] = 0;                     // if it is not the middle 3x3 leave it blank
                        }
                    }
                    // now let the basic 3x3 plus structuring in the (larger) structuring element grow to the right proportions
                    for (int i = 3; i < size; i += 2)
                    {
                        output = dilateImage(output, plus3x3);
                    }
                    // return this dilated plus structuring element
                    return output;
                }
            }
            else                                // so needs to be squared
            {
                // fill the whole structuring element with 1's so every element gets used
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        output[i, j] = 1;
                    }
                }
            }
            // return this filled with 1's square
            return output;
        }

        // dillate image
        private byte[,] dilateImage(byte[,] image, byte[,] strucElem)
        {
            return dilEroImage(image, strucElem, true);
        }
        // erode image
        private byte[,] erodeImage(byte[,] image, byte[,] strucElem)
        {
            return dilEroImage(image, strucElem, false);
        }

        // dilation en erosion behave similarly except they respectively use min or max
        private byte[,] dilEroImage(byte[,] image, byte[,] strucElem, bool dilate)
        {
            // create necessary variables
            byte[,] output = new byte[image.GetLength(0), image.GetLength(1)];
            int strucSize = strucElem.GetLength(0);
            int strucThickness = strucSize / 2;         // Thickness of the border around the center element of the structuring element
            int imageX = image.GetLength(0);
            int imageY = image.GetLength(1);
            int strucX, strucY, strucXmax, strucYmax;
            for (int x = 0; x < imageX; x++)
            {
                // border handling for x
                if (x < strucThickness) { strucX = strucThickness - x; strucXmax = strucSize; }                   // left bound
                else if (imageX - x <= strucThickness) { strucX = 0; strucXmax = strucThickness + (imageX - x); } // right bound
                else { strucX = 0; strucXmax = strucSize; }                                                     // no border

                for (int y = 0; y < imageY; y++)
                {
                    // border handling for y
                    if (y < strucThickness) { strucY = strucThickness - y; strucYmax = strucSize; }                  // upper bound
                    else if (imageY - y <= strucThickness) { strucY = 0; strucYmax = strucThickness + (imageY - y); }// lower bound
                    else { strucY = 0; strucYmax = strucSize; }                                                     // no border

                    // make a list for all the values that count
                    List<byte> values = new List<byte>();
                    // loop over the structuring element with parameters that handle the borders
                    for (int i = strucX; i < strucXmax; i++)
                    {
                        for (int j = strucY; j < strucYmax; j++)
                        {
                            // add value if it is allowed by the structuring element
                            byte strucElement = strucElem[i, j];
                            if (strucElement != 0)
                            {
                                values.Add((byte)(image[x + (i - strucThickness), y + (j - strucThickness)] * strucElement));
                            }
                        }
                    }
                    // if it needs to be dilated take the maximum value
                    if (dilate) output[x, y] = values.Max();
                    else output[x, y] = values.Min();            // else it needs to be eroded, so minimum value

                }
            }
            // return the dilated or eroded image
            return output;
        }

        // open the image by first eroding and dilating afterwards
        private byte[,] openImage(byte[,] image, byte[,] strucElem)
        {
            byte[,] output = erodeImage(image, strucElem);
            return dilateImage(output, strucElem);
        }

        // close the image by first dilating and eroding afterwards
        private byte[,] closeImage(byte[,] image, byte[,] strucElem)
        {
            byte[,] output = dilateImage(image, strucElem);
            return erodeImage(output, strucElem);
        }


        //check if its a valid binary image, for and / or funcs
        private bool Checkimage(byte[,] image)
        {
            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    if (image[x, y] != 0 && image[x, y] != 1)
                    {
                        return false; //not a valid binary image
                    }
                }
            }

            return true;
        }

        private byte[,] andImages(byte[,] image1, byte[,] image2)
        {

            byte[,] output = new byte[image1.GetLength(0), image1.GetLength(1)];
            //check both images
            if (!Checkimage(image1) || !Checkimage(image2))
            {
                Console.WriteLine("Invalid");
            }

            for (int x = 0; x < image1.GetLength(0); x++)
            {
                for (int y = 0; y < image1.GetLength(1); y++)
                {
                    output[x, y] = (byte)(image1[x, y] & image2[x, y]);
                }
            }

            return output;
        }

        private byte[,] orImages(byte[,] image1, byte[,] image2)
        {
            byte[,] output = new byte[image1.GetLength(0), image1.GetLength(1)];
            //check both images
            if (!Checkimage(image1) || !Checkimage(image2))
            {
                Console.WriteLine("Invalid");
            }

            for (int x = 0; x < image1.GetLength(0); x++)
            {
                for (int y = 0; y < image1.GetLength(1); y++)
                {
                    output[x, y] = (byte)(image1[x, y] | image2[x, y]);
                }
            }

            return output;
        }

        private bool isAGrayscaleImage(byte[,] image) //check for valuecounting
        {
            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    byte pixelValue = image[x, y];
                    if (pixelValue != image[0, 0])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public struct ValueCountOutput
        {
            public int DistinctValues;
            public int[] Histogram;
        }

        public ValueCountOutput countValues(byte[,] image)
        {

            if (!isAGrayscaleImage(image)){
                Console.WriteLine("Not a grayscale image.");
            }
            int distinctValues = 0;
            int[] histogram = new int[256];
            HashSet<byte> distinctValueSet = new HashSet<byte>();

            // count distinct values
            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    byte pixelValue = image[x, y];
                    if (!distinctValueSet.Contains(pixelValue))
                    {
                        distinctValueSet.Add(pixelValue);
                        distinctValues++;
                    }
                    histogram[pixelValue]++;
                }
            }

            ValueCountOutput output = new ValueCountOutput //set in struct for output
            {
                DistinctValues = distinctValues,
                Histogram = histogram
            };

            return output;
        }

        private List<Tuple<int, int>> traceBoundary(byte[,] image) //tuple bc its an output of lists with x,y pairs
        {
            List<Tuple<int, int>> boundaryCoordinates = new List<Tuple<int, int>>(); //initialize starting points
            int startX = -1;
            int startY = -1;
            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    if (image[x, y] == 1)
                    {
                        startX = x;
                        startY = y;
                        break;
                    }
                }
                if (startX != -1) break;
            }

            if (startX != -1)
            {
                int x = startX;
                int y = startY;
                int initialDirection = 0;
                int currentDirection = initialDirection;

                while (x != startX || y != startY)  // continue until returning to the starting point
                {
                    boundaryCoordinates.Add(new Tuple<int, int>(x, y));
                    int nextX = x;
                    int nextY = y;
                    switch (currentDirection)
                    {
                        case 0: nextX++; break;
                        case 1: nextY++; break;
                        case 2: nextX--; break;
                        case 3: nextY--; break;
                    }

                    // check if the next pixel is within the boundary, then move and keep same direcvtion
                    if (nextX >= 0 && nextX < image.GetLength(0) && nextY >= 0 && nextY < image.GetLength(1) && image[nextX, nextY] == 1)
                    {
                        x = nextX;
                        y = nextY;
                    }
                    else
                    {
                        currentDirection = (currentDirection + 1) % 4; //clockwise direction
                    }

                }
            }
            return boundaryCoordinates;
        }


        // ====================================================================
        // ============= YOUR FUNCTIONS FOR ASSIGNMENT 3 GO HERE ==============
        // ====================================================================
        private byte[,] HoughTransform(byte[,] image, int thetares = 180)
        {
            int maxRadius = (int)Math.Sqrt(image.GetLength(0) * image.GetLength(0) + image.GetLength(1) * image.GetLength(1)); //breedt^2 x hoogte^2
            byte[,] hImage = new byte[thetares, 2 * maxRadius + 1]; //voor output

            //eerst alles op 0
            for (int i = 0; i < thetares; i++)
            {
                for (int j = 0; j < hImage.GetLength(1); j++) { hImage[i, j] = 0; }
            }

            for (int x = 0; x < image.GetLength(0); x++) //loop door elke pixel
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    byte binaryvalue = image[x, y];
                    if (binaryvalue > 0)
                    {
                        // loop door elke 
                        for (int theta = 0; theta < thetares; theta++)
                        {
                            double thetaRadians = ((theta * (180.0 / thetares)) * Math.PI / 180);
                            int r = (int)Math.Round(x * Math.Cos(thetaRadians) + y * Math.Sin(thetaRadians));
                            r += maxRadius;
                            hImage[theta, r]++;
                        }
                    }
                }
            }

            return hImage;
        }

        private byte[,] HoughTransformAngleLimits(byte[,] image, int minTheta, int maxTheta, int thetares = 180)
        {
            int maxRadius = ((int)Math.Sqrt(image.GetLength(0) * image.GetLength(0) + image.GetLength(1) * image.GetLength(1))) / 2;
            byte[,] hImage = new byte[thetares, maxRadius * 2];

            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    if (image[x, y] != 0)
                    {
                        for (int ti = 0; ti < thetares; ti++)
                        {
                            // hough transform equation : r = x * cos(θ) + y * sin(θ)
                            int r = (int)Math.Round(x * Math.Cos((minTheta + (ti * (maxTheta - minTheta) / (double)thetares)) * (Math.PI / 180.0)) + y * Math.Sin((minTheta + (ti * (maxTheta - minTheta) / (double)thetares)) * (Math.PI / 180.0)));
                            r += maxRadius;
                            hImage[ti, r]++;
                        }
                    }
                }
            }

            return hImage;
        }

        //using option B: non maximum suppression from the slides
        internal List<Tuple<int, int>> peakFinding(byte[,] Image, int peakThres)
        {
            List<Tuple<int, int>> peakList = new List<Tuple<int, int>>();
            for (int theta = 0; theta < Image.GetLength(0); theta++)
            {
                for (int r = 0; r < Image.GetLength(1); r++)
                {
                    if (Image[theta, r] >= peakThres)
                    {
                        if (LocalMaxima(Image, theta, r, 0.0)) //gebruik een percentage threshold 0.0
                        {
                            peakList.Add(new Tuple<int, int>(r - (Image.GetLength(1) / 2), theta));
                            //MessageBox.Show("r: " + (r - (Image.GetLength(1) / 2)) + ". theta: " + theta);
                        }
                    }
                }
            }

            return peakList;
        }

        private bool LocalMaxima(byte[,] Image, int theta, int r, double percentThreshold)
        {
            int thresholdValue = (int)(percentThreshold * Image[theta, r]);
            for (int deltaTheta = -1; deltaTheta <= 1; deltaTheta++)
            {
                for (int deltaR = -1; deltaR <= 1; deltaR++)
                {
                    int neighborTheta = (theta + deltaTheta + Image.GetLength(0)) % Image.GetLength(0);
                    int neighborR = r + deltaR;

                    if (neighborR >= 0 && neighborR < Image.GetLength(1) &&
                        Image[neighborTheta, neighborR] > Image[theta, r] + thresholdValue) //check buren
                    {
                        return false; //Geen local maxima
                    }
                }
            }
            return true; // Is een local maxima
        }


        private List<Tuple<Tuple<int, int>, Tuple<int, int>, Tuple<int,int>>> houghLineDetection(
            byte[,] image, Tuple<int, int> rTheta, int minThres, int minLen, int maxGap, int rOffset)
        {
            // create thresholded image with "on" pixels
            byte[,] thresImage = thresholdImageBasedOnThreshold(image, (byte)minThres);
            // take r and theta from tuple respectively
            int r = rTheta.Item1;
            int theta = rTheta.Item2;
            // create a list that will contain all the "on" pixels that also correspond to r and theta
            List<Tuple<int, int>> rthetaPixels = new List<Tuple<int, int>>();
            // create a list that will contain all the line segments with a start (x,y) and end (x,y)
            List<Tuple<Tuple<int, int>, Tuple<int, int>, Tuple<int, int>>> lineSegments = 
                new List<Tuple<Tuple<int, int>, Tuple<int, int>, Tuple<int,int>>>();


            int maxRadius = (int)Math.Sqrt(image.GetLength(0) * image.GetLength(0) + image.GetLength(1) * image.GetLength(1));
            double thetaRadians = (theta * Math.PI / 180);
            double cosTheta = Math.Cos(thetaRadians);
            double sinTheta = Math.Sin(thetaRadians);
            int realR = r + rOffset - maxRadius;

            // loop over thresholded image
            for(int x = 0; x < thresImage.GetLength(0); x++)
            {
                for(int y = 0; y < thresImage.GetLength(1); y++)
                {
                    // if pixel is "on" go on
                    if(thresImage[x,y] > 0)
                    {

                        // if pixel correspond to r and theta combination add it to the list
                        double rFromTheta = ((x * cosTheta) + (y * sinTheta));
                        //MessageBox.Show("x: " + x + " y: " + y + " rFromTheta " + rFromTheta);
                        if(Math.Round(rFromTheta) == realR)
                        {
                            //MessageBox.Show("theta pixel added: x " + x + " y " + y);
                            rthetaPixels.Add(new Tuple<int, int>(x,y));
                        }
                    }
                }
            }
            //MessageBox.Show("done finding pixels");

            List<Tuple<int, int>> checkedPixels = new List<Tuple<int, int>>();
            // loop over all the pixels that are "on" and correspond to r and theta
            foreach(Tuple<int, int> onPixel in rthetaPixels)// int i = 0; i < rthetaPixels.Count; i++)
            {
                
                if(checkedPixels.Contains(onPixel)) continue;
                
                List<Tuple<int, int>> connectedPixels = new List<Tuple<int, int>>();
                connectedPixels.Add(onPixel);
                Tuple<int,int> startPxl = onPixel;
                Tuple<int,int> endPxl = onPixel;
                double totalDistance = 0;
                bool newPixelConnected = false;
                foreach(Tuple<int, int> pxl in rthetaPixels)
                {
                    if(pxl != startPxl)
                    {
                        double distance = Math.Sqrt(Math.Pow(pxl.Item1 - startPxl.Item1, 2) + Math.Pow(pxl.Item2 - startPxl.Item2, 2));
                        if(distance <= maxGap)
                        {
                            connectedPixels.Add(pxl);
                            newPixelConnected = true;
                            endPxl = pxl;
                            totalDistance = distance;
                            break;
                        }
                    }
                }
                while (newPixelConnected)
                {
                    newPixelConnected = false;
                    foreach(Tuple<int,int> pxl in rthetaPixels)
                    {
                        if (!connectedPixels.Contains(pxl))
                        {
                            double distanceStart = Math.Sqrt(Math.Pow(pxl.Item1 - startPxl.Item1, 2) + Math.Pow(pxl.Item2 - startPxl.Item2, 2));
                            double distanceEnd = Math.Sqrt(Math.Pow(pxl.Item1 - endPxl.Item1, 2) + Math.Pow(pxl.Item2 - endPxl.Item2, 2));
                            if(distanceStart <= maxGap || distanceEnd <= maxGap)
                            {
                                connectedPixels.Add(pxl);
                                if(distanceStart > totalDistance)
                                {
                                    newPixelConnected=true;
                                    endPxl = pxl;
                                    totalDistance = distanceStart;
                                    continue;
                                }else if(distanceEnd > totalDistance)
                                {
                                    newPixelConnected=true;
                                    startPxl = pxl;
                                    totalDistance = distanceEnd;
                                    continue;
                                }
                            }
                            if(distanceStart < totalDistance && distanceEnd < totalDistance)
                            {
                                connectedPixels.Add(pxl);
                            }
                        }
                    }
                    /*
                    foreach(Tuple<int, int> connectedPixel in connectedPixels)
                    {        
                        foreach(Tuple<int, int> pxl in rthetaPixels)
                        {
                            if (!connectedPixels.Contains(pxl))
                            {
                                double distance = Math.Sqrt(Math.Pow(pxl.Item1 - connectedPixel.Item1, 2) + Math.Pow(pxl.Item2 - connectedPixel.Item2, 2));
                                if(distance <= maxGap)
                                {
                                    newPixelConnected=true;
                                    newlyFoundPxls.Add(pxl);
                                }
                            }
                        }
                    }
                    connectedPixels.AddRange(newlyFoundPxls);
                    */
                }

                checkedPixels.AddRange(connectedPixels);
                /*
                var linePixels = connectedPixels.OrderByDescending(x => x.Item1);
                if(linePixels.First().Item1 == linePixels.Last().Item1)
                {
                    linePixels = connectedPixels.OrderByDescending(x => x.Item2);
                }
                Tuple<int, int> startingPxl = new Tuple<int, int>(linePixels.First().Item1, linePixels.First().Item2);
                Tuple<int, int> endingPxl = new Tuple<int, int>(linePixels.Last().Item1, linePixels.Last().Item2);

                double lineDistance = Math.Sqrt(Math.Pow(endingPxl.Item1 - startingPxl.Item1, 2) + Math.Pow(endingPxl.Item2 - startingPxl.Item2, 2));
                */
                if(totalDistance >= minLen)
                {
                    lineSegments.Add(new Tuple<Tuple<int,int>, Tuple<int, int>, Tuple<int,int>>(startPxl, endPxl, rTheta));
                }
                

                /*
                // save current pixel tuple
                //Tuple<int,int> currentPxl = rthetaPixels[i];
                // create a list that will contain all the seen pixels when searching for valid pixels to create the line
                List<Tuple<int, int>> seenPxls = new List<Tuple<int, int>>();
                // add current pixel
                seenPxls.Add(onPixel);
                Tuple<int,int> lastPxl = onPixel;
                // look around current pixel to find the next pixel in the line
                Tuple<int,int> nextPxl = findNextPxl(lastPxl, maxGap, rthetaPixels, seenPxls);
                // if there is a valid pixel nearby continue
                while(nextPxl != null)
                {
                    //MessageBox.Show("next pixel found: x " + nextPxl.Item1 + " y " + nextPxl.Item2);
                    // start looking from the next pixel
                    lastPxl = nextPxl;
                    // add the newly found pixel to the seen pixels
                    seenPxls.Add(nextPxl);
                    // look for the next pixel with as starting point the previous next point
                    nextPxl = findNextPxl(lastPxl, maxGap, rthetaPixels,seenPxls);
                }
                // calculate x and y distance from the first starting pixel and the last valid found pixel
                int xDistance = onPixel.Item1 - lastPxl.Item1;
                int yDistance = onPixel.Item2 - lastPxl.Item2;
                // take the maximum to calculate the pixel distance
                double distanceLineSegment = Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2));
                // if the distance is more than the minimum length a line segment should be add it to the list
                if(distanceLineSegment >= (double)minLen)
                {
                    lineSegments.Add(new Tuple<Tuple<int,int>, Tuple<int, int>, Tuple<int,int>>(onPixel, lastPxl, rTheta));
                    //MessageBox.Show("lineSegment found: x1 " + onPixel.Item1 + " y1 " + onPixel.Item2 + " x2 " + onPixel.Item1 + " y2 " + onPixel.Item2); 
                }
                */
            }
            return lineSegments;
        }

        private Tuple<int, int> findNextPxl(Tuple<int, int> center, int maxGap, List<Tuple<int, int>> pixels, List<Tuple<int, int>> seenPxls)
        {
            //MessageBox.Show("finding next pixel");
            // loop over the pixels around the center pixel
            // NOTE: you want one more pixel than the gap, because if you then find a pixel at the edge, the gap is at it's maximum but still valid
            for(int x = (-1) - maxGap; x <= maxGap + 1; x++)
            {
                for(int y = (-1) - maxGap; y <= maxGap + 1; y++)
                {
                    // if the pixel is not the center itself continue
                    if(!(x == 0 && y == 0))
                    {
                        // check if the pixel is a valid ("on" && r/theta) pixel and has not been seen before
                        Tuple<int, int> pxl = new Tuple<int, int> (center.Item1 + x, center.Item2 + y);
                        //MessageBox.Show("x " + pxl.Item1 + " y " + pxl.Item2);
                        if(pixels.Contains(pxl) && (!seenPxls.Contains(pxl)))
                        {
                            // if so return this pixel
                            return pxl;
                        }
                    }
                }
            }
            // if no pixel around the center pixel is valid return null
            return null;
        }


        private byte[,] visualizeHoughLineSegments(byte[,] image, List<Tuple<Tuple<int, int>, Tuple<int, int>, Tuple<int,int>>> lineSegments)
        {
            byte[,] outputImage = image;
            // loop over all line segments
            foreach(Tuple<Tuple<int, int>, Tuple<int, int>, Tuple<int,int>> lineSegment in lineSegments)
            {
                Tuple<int, int> startLine = lineSegment.Item1;
                Tuple<int, int> endLine = lineSegment.Item2;
                // with bresenham algorithm get all the pixels in between the start and end point of line segment
                List<Tuple<int, int>> pixelsOnLine = bresenhamLineAlgorithm(startLine.Item1, startLine.Item2, endLine.Item1, endLine.Item2);
                // loop over all pixels on the line and color white on the output image
                foreach(Tuple<int, int> pixel in pixelsOnLine)
                {
                    outputImage[pixel.Item1, pixel.Item2] = 199;
                }
            }
            // return output image
            return outputImage;
        }

        // Bresenham's line algorithm for calculating all pixels for a line between 2 point
        // https://stackoverflow.com/questions/11678693/all-cases-covered-bresenhams-line-algorithm
        public List<Tuple<int, int>> bresenhamLineAlgorithm(int x,int y,int x2, int y2) 
        {
            List<Tuple<int, int>> pixelsOnLine = new List<Tuple<int, int>>();
            int w = x2 - x ;
            int h = y2 - y ;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0 ;
            if (w<0) dx1 = -1 ; else if (w>0) dx1 = 1 ;
            if (h<0) dy1 = -1 ; else if (h>0) dy1 = 1 ;
            if (w<0) dx2 = -1 ; else if (w>0) dx2 = 1 ;
            int longest = Math.Abs(w) ;
            int shortest = Math.Abs(h) ;
            if (!(longest>shortest)) {
                longest = Math.Abs(h) ;
                shortest = Math.Abs(w) ;
                if (h<0) dy2 = -1 ; else if (h>0) dy2 = 1 ;
                dx2 = 0 ;            
            }
            int numerator = longest >> 1 ;
            for (int i=0;i<=longest;i++) {
                pixelsOnLine.Add(new Tuple<int, int>(x, y));
                numerator += shortest ;
                if (!(numerator<longest)) {
                    numerator -= longest ;
                    x += dx1 ;
                    y += dy1 ;
                } else {
                    x += dx2 ;
                    y += dy2 ;
                }
            }
            return pixelsOnLine;
        }

        //choice task
        public List<Tuple<int,int>> findCrossings(byte[,] image, List<Tuple<int,int>> pairs)
        {
            List<Tuple<int,int>> crossings = new List<Tuple<int,int>>();
            foreach (Tuple<int, int> pair1 in pairs)//lets check each pair
            {
                for (int i = 0; i < pairs.Count; i++)
                {
                    if (i != pairs.IndexOf(pair1))
                    {
                        Tuple<int,int> pair2 = pairs[i];
                        int x1;
                        int y1;
                        if (!(Math.Cos(pair1.Item2) == 0 || Math.Cos(pair2.Item2) == 0))
                        {
                            //formulas
                            //Δr = (r₂ / sin(θ₂)) - (r₁ / sin(θ₁))
                            //Δtan = -tan(θ₁) + tan(θ₂)
                            //x1 = round(Δr / Δtan)
                            //y1 = round(-tan(θ₁) * x1 + (r₁ / sin(θ₁)))
                            x1 = (int)Math.Round(((pair2.Item1 / Math.Sin(pair2.Item2)) - (pair1.Item1 / Math.Sin(pair1.Item2))) / ((-Math.Tan(pair1.Item2)) - (-Math.Tan(pair2.Item2))));
                            y1 = (int)Math.Round(-Math.Tan(pair1.Item2) * x1 + (pair1.Item1 / Math.Sin(pair1.Item2)));

                            //if x1 is bigger than 0, but lower than the image height, + y is bigger than 0 and smaller than the width, and this point hasn't been added yet - we have a crossing
                            if (x1 >= 0 && x1 < image.GetLength(1) && y1 >= 0 && y1 < image.GetLength(0) && !crossings.Contains(new Tuple<int,int>(x1, y1)))
                            {
                                crossings.Add(new Tuple<int,int>(x1, y1));
                            }
                        }
                    }
                }
            }
            return crossings;
        }

        // so call it like showCrossings(findCrossings(input parameters), input image);
        private byte[,] showCrossings(List<Tuple<int,int>> crossings, byte[,] image)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[image.GetLength(0), image.GetLength(1)];

            // TODO: add your functionality and checks
            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    tempImage[x, y] = image[x, y];
                }
            }
            foreach (var crossing in crossings)
            {
                tempImage[crossing.Item1, crossing.Item2] = 255; //make it white
            }

            return tempImage;
        }

        // ====================================================================
        // ============= YOUR FUNCTIONS FOR ASSIGNMENT 4 GO HERE ==============
        // ====================================================================

        private byte[,,] HoughTransformCircle(byte[,] image, int maxCircleRadius, int minCircleRadius)
        {
            
            //int maxRadius = (int)Math.Sqrt(image.GetLength(0) * image.GetLength(0) + image.GetLength(1) * image.GetLength(1)); //breedt^2 x hoogte^2
            int imageX = image.GetLength(0);
            int imageY = image.GetLength(1);
            byte[,,] accArray = new byte[imageX,imageY,maxCircleRadius - minCircleRadius]; //voor output

            //eerst alles op 0
            for (int i = 0; i < imageX; i++)
            {
                for (int j = 0; j < imageY; j++) 
                { 
                    for(int k = 0; k < (maxCircleRadius - minCircleRadius); k++) { accArray[i,j,k] = 0;}
                }
            }
            double piDiv180 = Math.PI / 50;
            for (int x = 0; x < image.GetLength(0); x++) //loop door elke pixel
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    byte binaryvalue = image[x, y];
                    if (binaryvalue > 0)
                    {
                        for(int k = minCircleRadius; k < maxCircleRadius; k++)
                        {
                            List<Tuple<int,int>> seenCords = new List<Tuple<int,int>>();
                            for(int angle = 0; angle < 100; angle++)
                            {
                                int xArray = x + (int)((double)k * Math.Cos((double)angle * piDiv180));
                                int yArray = y + (int)((double)k * Math.Sin((double)angle * piDiv180));
                                if(xArray < 0 || yArray < 0 || xArray >= imageX || yArray >= imageY) continue;
                                if(!seenCords.Contains(new Tuple<int, int>(xArray, yArray)))
                                {
                                    seenCords.Add(new Tuple<int, int>(xArray, yArray));
                                    accArray[xArray,yArray,k - minCircleRadius]++;
                                }
                            }
                        }
                    }
                }
            }

            return accArray;
        }


        //using option B: non maximum suppression from the slides
        internal List<Tuple<int, int, int>> peakFindingCircle(byte[,,] accArray, int peakThres, int minCircleRadius)
        {
            List<Tuple<int, int, int>> peakList = new List<Tuple<int, int, int>>();
            for (int x = 0; x < accArray.GetLength(0); x++)
            {
                for (int y = 0; y < accArray.GetLength(1); y++)
                {
                    for(int r = 0; r < accArray.GetLength(2); r++)
                    {
                        if (accArray[x, y, r] >= peakThres)
                        {
                            if (LocalMaxima3D(accArray, x, y, r, 0.0)) //gebruik een percentage threshold 0.0
                            {
                                peakList.Add(new Tuple<int, int, int>(x, y, r + minCircleRadius));
                            }
                        }
                    }
                }
            }

            return peakList;
        }

        private bool LocalMaxima3D(byte[,,] accArray, int x, int y, int r, double percentThreshold)
        {
            int thresholdValue = accArray[x, y, r] + (int)(percentThreshold * accArray[x, y, r]);
            int maxX = accArray.GetLength(0);
            int maxY = accArray.GetLength(1);
            int maxR = accArray.GetLength(2);
            for (int deltaX = -1; deltaX <= 1; deltaX++)
            {
                if(deltaX + x< 0 || deltaX + x>= maxX) continue;

                for (int deltaY = -1; deltaY <= 1; deltaY++)
                {
                    if(deltaY + y< 0 || deltaY + y>= maxY) continue;

                    for(int deltaR = -1; deltaR <= 1; deltaR++)
                    {
                        if(deltaR + r< 0 || deltaR + r>= maxR) continue;
                        if (accArray[deltaX + x, deltaY + y, deltaR + r] > thresholdValue) //check buren
                        {
                            return false; //Geen local maxima
                        }
                    }
                }
            }
            return true; // Is een local maxima
        }

        private byte[,] visualizeHoughCircles(byte[,] image, List<Tuple<int, int, int>> circles)
        {
            byte[,] outputImage = image;
            foreach(Tuple<int, int, int> circle in circles)
            {
                int centerX = circle.Item1;
                int centerY = circle.Item2;
                int radius = circle.Item3;
                double piDiv180 = Math.PI / 180;
                for(int i = 0; i < 360; i++)
                {
                    int x = centerX + (int)((double)radius * Math.Cos((double)i * piDiv180));
                    int y = centerY + (int)((double)radius * Math.Sin((double)i * piDiv180));
                    if(x > 0 && y > 0 && x < image.GetLength(0) && y < image.GetLength(1)) outputImage[x, y] = 201;
                }
            }
            return outputImage;
        }

        private List<Tuple<int,int,int>> bicycleWheel(List<Tuple<int, int, int>> circles, 
            List<Tuple<Tuple<int,int>, Tuple<int,int>, Tuple<int,int>>> lineSegments)
        {
            List<Tuple<int,int,int>> bicycleWheels = new List<Tuple<int,int,int>>();
            foreach(Tuple<int, int, int> circle in circles)
            {
                int spokes = 0;
                int centerX = circle.Item1;
                int centerY = circle.Item2;
                int radius = circle.Item3;
                //MessageBox.Show("total lineSegments: " + lineSegments.Count());
                foreach(Tuple<Tuple<int,int>, Tuple<int,int>, Tuple<int,int>> lineSegment in lineSegments)
                {
                    Tuple<int,int> lineStart = lineSegment.Item1;
                    Tuple<int,int> lineEnd = lineSegment.Item2;
                    Tuple<int,int> rTheta = lineSegment.Item3;
                    double thetaRadians = (rTheta.Item2 * Math.PI / 180);
                    double cosTheta = Math.Cos(thetaRadians);
                    double sinTheta = Math.Sin(thetaRadians);
                    double rFromTheta = ((centerX * cosTheta) + (centerY * sinTheta));
                    if(Math.Round(rFromTheta) == rTheta.Item1)
                    {
                        double distanceEndCenter = Math.Sqrt(Math.Pow((lineEnd.Item1 - centerX), 2) + Math.Pow((lineEnd.Item2 - centerY), 2));
                        double distanceStartCenter = Math.Sqrt(Math.Pow((lineStart.Item1 - centerX), 2) + Math.Pow((lineStart.Item2 - centerY), 2));

                        if(distanceEndCenter < radius + 5 && distanceStartCenter < radius + 5)
                        {
                            spokes++;
                            //MessageBox.Show("SPOKE FOUND!");
                        }
                        /*
                        //MessageBox.Show("line goes through center");
                        int x = centerX + (int)((double)radius * cosTheta);
                        int y = centerY + (int)((double)radius * sinTheta);

                        //MessageBox.Show("x:" + x + " y:" + y + " linex:" + lineStart.Item1 + " liney:" + lineStart.Item2);
                        if(x < lineStart.Item1 + 5 && y < lineStart.Item2 + 5 && x > lineStart.Item1 - 5 && y > lineStart.Item2 - 5)
                        {
                            MessageBox.Show("line start on edge circle");
                            // line start lays on edge of circle with some margin of error
                            double distanceEndCenter = Math.Sqrt(Math.Pow((lineEnd.Item1 - centerX), 2) + Math.Pow((lineEnd.Item2 - centerY), 2));
                            if(distanceEndCenter <= radius + 2)
                            {
                                spokes++;
                                MessageBox.Show("SPOKE FOUND!");
                            }

                        }else if(x < lineEnd.Item1 + 5 && y < lineEnd.Item2 + 5 && x > lineEnd.Item1 - 5 && y > lineEnd.Item2 - 5)
                        {
                            MessageBox.Show("line end on edge circle");
                            // line end lays on edge of circle with some margin of error
                            double distanceStartCenter = Math.Sqrt(Math.Pow((lineStart.Item1 - centerX), 2) + Math.Pow((lineStart.Item2 - centerY), 2));
                            if(distanceStartCenter <= radius + 2)
                            {
                                spokes++;
                                MessageBox.Show("SPOKE FOUND!");
                            }
                        }
                        */
                    }
                }
                if(spokes > 3)
                {
                    bicycleWheels.Add(circle);
                }
            }
            return bicycleWheels;
        }
    }
}
