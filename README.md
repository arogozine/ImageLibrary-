# ImageLibrary\# 
### Simple Image Manipulation Library for .NET

> This project is NO LONGER IN DEVELOPMENT. ImageLibrary\# is a .NET rewrite/conversion of a Scheme + C image editing library used in class used in Digital Image Processing class at UNM. It was inspired by another student writing a Haskell implementation (see https://github.com/jcollard/unm-hip, I think). Issue is I never got close to finishing this before the class was over... so I put it up on GitHub.

## Project Indended Goals
* Move to C#.NET to remove memory leaks present in C code and allow use of more than 4GB memory
* Use parallel processing where possible to make library faster
* Move every image function from UNM scheme C code to C#
* Move every function written in scheme lang to C# as well
* Add functionality deemed needed and/or missing
* Implement a standard set of functionality that allows easy image filtering
* Gain a broader understanding of C# in the process

## How to Use
### NameSpaces
```csharp
using ImageLibrary;
using ImageLibrary.Extensions;
```

### Color Structs
* BGRA - Blue, Green, Red, and Alpha. Each value is a byte.
* CMYK - Cyan, Magenta, Yellow, and Black Key Color. Each value is a double.
* HSL - Hue, Saturation, and Lightness / Intensity. Each value is a double.
* HSV - Hue, Saturation, and Value. Each value is a double.
* RGB - Red, Green, and Blue. Each value is a double.
* YCbCr - Y, Cb, and Cr. Each value is a byte.
* Greyscale - double
* Binary - bool
* Complex - System.Numerics.Complex

### Generating an Image
To generate an `IImage<Y>` object,
```csharp
// GrayScale Image
var greyscale = ImageFactory.Generate(/* various overloads */);
// Binary Image
var binary = ImageFactory.GenerateBinary(/* various overloads */);
// Complex Image
var complex = ImageFactory.GenerateComplex(/* various overloads */);
// RGB Image
var rgb = ImageFactory.GenerateRgb(/* various overloads */);
// BGRA Image
var bgra = ImageFactory.GenerateBgra(/* various overloads */);
// HSV Image
var hsv = ImageFactory.GenerateHsv(/* various overloads */);
// HSL Image
var hsl = ImageFactory.GenerateHsl(/* various overloads */);
// CMYK Image
var hsl = ImageFactory.GenerateCmyk(/* various overloads */);
```
ImageFactory.Generate functions have various overloads, but in general,
```csharp
// Load Image from Disk
var readFromDisk = ImageFactory.Generate(@"C:\...");

// Generate Blank of a specific width and height
var blank256 = ImageFactory.Generate(256, 256);

// Generate from an Array or Enumeration
var fromArray = ImageFactory.Generate(256, 256, doubleArray);
var fromEnumeration = ImageFactory.Generate(256, 256, doubleEnumerable);
```
### Accessing Data
```csharp
IImage<double> img = ImageFactory.Generate(fullPath);
// image data is stored as an array in the back-end
// index is from 0 to width * height - 1
double pixelValueAtIndex = img[index];
// Accessing a pixel at (X,Y) coordinates
double pixelValueAtXY = img[yLocation, xLocation];

// Lightweight helper to work with the image columns
ImageColumn<double>[] cols = img.Columns();
// Lightweight helper to work with the image rows
ImageRow<double>[] rows = image.Rows();
```
### IImage\<Y\> Functions
* `Copy()` - Creates a copy of the existing image
* `Crop()` - Creates new cropped image
* `Pad()` - Creates a new image which is padded
* `Upsample()` - Upsamples Rows and Columns
* `UpsampleCols()` - Usamples Columns Only
* `UpsampleRows()` - Usamples Rows Only
* `Downsample()` - Downsamples Rows and Columns
* `DownsampleCols()` - Downsamples Columns Only
* `DownsampleRows()` - Downsamples Rows Only
* `FlipX()` - Vertical Flip
* `FlipY()` - Horizontal Flip
* `FlipXY()` - Horizontal and Vertical Flip
* `Transpose()` - Creates a new Transposed Image
* `ToBGR()` - Returns image as RGB byte array
* `ToBGRA()` - Returns image as RGBA byte array

### Mapping Values
Map functions allow you to map new values to each index (i), location (i,j), color value, or a combination.
For example `image.MapValue(Math.Abs)` will apply `Math.Abs` function to all values.

* `img.MapIndex(generateColorValueBasedOnIndex)`
* `img.MapValue(generateColorValueBasedOnValue)`
* `img.MapLocation(generateColorValueBasedOn2DIndex)`
* `img.MapValueIndex(generateColorValueBasedOnIndexAndValue)`
* `img.MapLocationValue(generateColorValueBasedOn2DIndexAndValue)`

Optionally, you can pass `bool parallel` as the second argument. By default the map functions are run in parallel.

### Converting Between Image Types
Use the `TypeConversion` class to convert between various types.
For example,
```csharp
// BOOL -> BGRA
var bgraFromBool = TypeConversion.ToBgra(false);
// Same as TypeConversion.ToBgra(default(RGB)),
BGRA value = default(RGB).ToBgra();
```
Use the `ImageConversion` to convert an image between various types,
```csharp
IImage<double> img = ImageFactory.Generate(fullPathToImage);
IImage<Complex> complexImg = img.ToComplexImage();
```
### Splitting an Image into Parts
Use `ImageExtensions` for splitting image into parts,
```csharp
IImage<Complex> complexImg = getComplexImageFromSomewhere();
IImage<double> realPart = complexImg.RealPart();
IImage<double> imaginaryPart = complexImg.ImaginaryPart();
```
### Fast Fourier Transform (FFT)
Extensions methods for Fast Fourier Transform are in `FastFourierTransform` class.
Use `Filters` class to generate filters.

```csharp
//
IImage<double> img = getImageFormSomewhere();
 
// Run FFT
IImage<Complex> fftImg = img.Fft();
IImage<Complex> fftImg2 = img.Fft(/*shift*/true);
IImage<Complex> fftImgNoShift = img.Fft(/*shift*/false);

// Inverse FFT
IImage<double> originalImageBack = fftImg.InverseFft();

// Creating Filters for FFT
var noShiftfilter = Filters.MakeFilter(512, 512, Filters.IdealLowPass(50));
var shiftFilter = Filters.MakeFilterWithOffset(512, 512, Filters.IdealLowPass(50));
```

### Math Operations On Images
For math operations of the type,
```
define img3
For x, y in img and img2
        img3[x,y] <- img[x,y] {OP} img2[x,y]
```
use `ImageFunctions`
### Saving Images
'''Basic Image Formats'''
To save an image use one of the `WriteImage` overloads.
If you do not specify the format, by default it is PNG.
For saving as PPM, PGM, and PBM use `CustomOutput` class.

### Convolution
Convolution works on grayscale images.
* `grayScaleImage.ConvolveCols(kernel)` - Default Edge Handling Method is crop
* `grayScaleImage.ConvolveRows(kernel)` - Default Edge Handling Method is crop
* `grayScaleImage.Convolve(kernel)` - Default Edge Handling Method is crop
* `grayScaleImage.Convolve(kernel, edgeHandlingType)`

See `Convolution` class for a few predefined kernels.

### Edge Detection
See `ImageLibrary.EdgeDetection` for Canny and Bolt edge detection code.
Don't ask me how it works / what it does.

### Other Functions
* `grayScaleImage.Histogram()` - Creates a histogram of an image
* `grayScaleImage.CumulativeDistribution()` - Cumulative Distribution
* `ImageExtensions.Multiply(a, b)` - Matrix Multiply (Uses Math.NET)
* `grayScaleImage.Rotate(60)` - Rotates an image by specified degree
* `grayScaleImage.Equalize()` - Enhances Contrast based on the CDF
* `image.Normalize256()` - Normalizes Floating Point Image to [0 - 255] range for easy convesion to byte
* `image.Normalize()`
* `grayScaleImage.Erode()`
* `grayScaleImage.Dilate()`
* `grayScaleImage.Open()`
* `grayScaleImage.Close()`
* `grayScaleImage.MedianFilter(x,y)`
* `grayScaleImage.MakeHotImage()`
* `grayScaleImage.CentersOfMass()`
* `grayScaleImage.BoundingBoxes()`
* `grayScaleImage.Areas()`
* `grayScaleImage.Outline()`
* `grayScaleImage.DistanceTransform()`
* `grayScaleImage.ChiSquareDistance()`
* `grayScaleImage.HellingerDistance(anotherImage)`
* `grayScaleImage.ZeroCrossings(crossingMethod)`
