using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Feuster.Imaging.Resizing
{
    public class Scaler
    {
        //GitVersion will be only be actualized/overwritten when using Cake build of ResizeX!
        public const string GitVersion = "git-2273bbc";

        //Version string will be only be actualized/overwritten when using Cake build of ResizeX as fallback Assembly is read out but this works not if lib is compiled into exe!
        public static string Version = "1.0.1.0";

        /// <summary>
        /// Call to library version
        /// </summary>
        /// <returns>Version string</returns>
        internal static string GetVersion()
        {
            if (Version == string.Empty)
                Version = Assembly.GetEntryAssembly().GetName().Version.Major.ToString() + "." + Assembly.GetEntryAssembly().GetName().Version.Minor.ToString() + "." + Assembly.GetEntryAssembly().GetName().Version.Revision.ToString() + "." + Assembly.GetEntryAssembly().GetName().Version.Build.ToString();
            if (GitVersion != string.Empty)
                return $"{Version}-{GitVersion}";
            else
                return Version;
        }
        public static string LibVersion => GetVersion();


        //List of the available Scaler algorithm groups
        public enum ScalerGroups
        {
            ResizeX,
            ScaleX,
            FastResize,
            ResizeNET,
            SharpResize
        }

        //Description of the available Scaler algorithm groups
        public static readonly string[] ScalerGroupsDescriptions =
        {
            "Very fast picture resize with given factor by just cloning the pixels",
            "Fast quality picture resize with given factor and a ScaleX algorithm adding details",
            "Resizes picture faster but probably with less details as the default .net Bitmap class scaler",
            "Resizes picture with the default .net Bitmap class scaler (Reference)",
            "Resizes picture with more sharpness but slower as the default .net Bitmap class scaler"
        };

        //List of the available Scaler algorithms
        public enum Scalers {
            Resize2x,
            Resize3x,
            Resize4x,
            Resize6x,
            Resize8x,
            Resize9x,
            Resize12x,
            Resize16x,
            Scale2x,
            Scale3x,
            Scale4x,
            Scale6x,
            Scale8x,
            Scale9x,
            Scale12x,
            Scale16x,
            FastResize200,
            FastResize300,
            FastResize400,
            FastResize600,
            FastResize800,
            FastResize900,
            FastResize1200,
            FastResize1600,
            Resize200,
            Resize300,
            Resize400,
            Resize600,
            Resize800,
            Resize900,
            Resize1200,
            Resize1600,
            SharpResize200,
            SharpResize300,
            SharpResize400,
            SharpResize600,
            SharpResize800,
            SharpResize900,
            SharpResize1200,
            SharpResize1600
        };

        //Description of the available Scaler algorithms
        public static readonly string[] ScalerDescriptions =
        {
            "Very fast picture resize with given factor by just cloning the pixels to the double size",
            "Very fast picture resize with given factor by just cloning the pixels to three times the size",
            "Very fast picture resize with given factor by just cloning the pixels to four times the size",
            "Very fast picture resize with given factor by just cloning the pixels to six times the size",
            "Very fast picture resize with given factor by just cloning the pixels to eight times the size",
            "Very fast picture resize with given factor by just cloning the pixels to nine times the size",
            "Very fast picture resize with given factor by just cloning the pixels to twelve times the size",
            "Very fast picture resize with given factor by just cloning the pixels to sixteen times the size",
            "Fast quality picture resize with given factor and a Scale2X algorithm adding details to the double size",
            "Fast quality picture resize with given factor and a Scale3X algorithm adding details to three times the size",
            "Fast quality picture resize with given factor and a Scale2X algorithm adding details to four times the size",
            "Fast quality picture resize with given factor and a Scale2X/Scale3X algorithm adding details to six times the size",
            "Fast quality picture resize with given factor and a Scale2X algorithm adding details to eight times the size",
            "Fast quality picture resize with given factor and a Scale3X algorithm adding details to nine times the size",
            "Fast quality picture resize with given factor and a Scale2X/Scale3X algorithm adding details to twelve times the size",
            "Fast quality picture resize with given factor and a Scale2X algorithm adding details to sixteen times the size",
            "Resizes picture faster but probably with less details as the default .net Bitmap class scaler to the double size",
            "Resizes picture faster but probably with less details as the default .net Bitmap class scaler to three times the size",
            "Resizes picture faster but probably with less details as the default .net Bitmap class scaler to four times the size",
            "Resizes picture faster but probably with less details as the default .net Bitmap class scaler to six times the size",
            "Resizes picture faster but probably with less details as the default .net Bitmap class scaler to eight times the size",
            "Resizes picture faster but probably with less details as the default .net Bitmap class scaler to nine times the size",
            "Resizes picture faster but probably with less details as the default .net Bitmap class scaler to twelve times the size",
            "Resizes picture faster but probably with less details as the default .net Bitmap class scaler to sixteen times the size",
            "Resizes picture with the default .net Bitmap class scaler to the double size (can be used as performance reference)",
            "Resizes picture with the default .net Bitmap class scaler to three times the size (can be used as performance reference)",
            "Resizes picture with the default .net Bitmap class scaler to four times the size (can be used as performance reference)",
            "Resizes picture with the default .net Bitmap class scaler to six times the size (can be used as performance reference)",
            "Resizes picture with the default .net Bitmap class scaler to eight times the size (can be used as performance reference)",
            "Resizes picture with the default .net Bitmap class scaler to nine times the size (can be used as performance reference)",
            "Resizes picture with the default .net Bitmap class scaler to twelve times the size (can be used as performance reference)",
            "Resizes picture with the default .net Bitmap class scaler to sixteen times the size (can be used as performance reference)",
            "Resizes picture with more sharpness but slower as the default .net Bitmap class scaler to the double size",
            "Resizes picture with more sharpness but slower as the default .net Bitmap class scaler to three times the size",
            "Resizes picture with more sharpness but slower as the default .net Bitmap class scaler to four times the size",
            "Resizes picture with more sharpness but slower as the default .net Bitmap class scaler to six times the size",
            "Resizes picture with more sharpness but slower as the default .net Bitmap class scaler to eight times the size",
            "Resizes picture with more sharpness but slower as the default .net Bitmap class scaler to nine times the size",
            "Resizes picture with more sharpness but slower as the default .net Bitmap class scaler to twelve times the size",
            "Resizes picture with more sharpness but slower as the default .net Bitmap class scaler to sixteen times the size"
        };

        /// <summary>
        /// Resize a Bitmap by the selected scaler algorithm
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <param name="ScalerAlgorithm">Scaler algorithm for resizing</param>
        /// <param name="EvenSizes">Create Bitmap with even width and height</param>
        /// <returns>Bitmap with new even sizes</returns>
        public static Bitmap? Scale(Bitmap? Input, Scalers ScalerAlgorithm, bool EvenSizes = false)
        {
            //invalid image
            if (Input == null || Input.Width == 0 || Input.Height == 0)
                return null;

            //check Scaler
            if (!Enum.IsDefined(typeof(Scalers), ScalerAlgorithm))
                return Input;

            //scale image
            if (EvenSizes)
                Input = Scaler.ResizeToEvenSize(Input);
            
            Bitmap? Output = null;
            if (Input != null)
            {
                try
                {
                    switch (ScalerAlgorithm)
                    {
                        default:
                            Output = Input;
                            break;
                        case Scalers.Resize200:
                            Output = Scaler.Resize(Input, Input.Width * 2, Input.Height * 2);
                            break;
                        case Scalers.Resize300:
                            Output = Scaler.Resize(Input, Input.Width * 3, Input.Height * 3);
                            break;
                        case Scalers.Resize400:
                            Output = Scaler.Resize(Input, Input.Width * 4, Input.Height * 4);
                            break;
                        case Scalers.Resize600:
                            Output = Scaler.Resize(Input, Input.Width * 6, Input.Height * 6);
                            break;
                        case Scalers.Resize800:
                            Output = Scaler.Resize(Input, Input.Width * 8, Input.Height * 8);
                            break;
                        case Scalers.Resize900:
                            Output = Scaler.Resize(Input, Input.Width * 9, Input.Height * 9);
                            break;
                        case Scalers.Resize1200:
                            Output = Scaler.Resize(Input, Input.Width * 12, Input.Height * 12);
                            break;
                        case Scalers.Resize1600:
                            Output = Scaler.Resize(Input, Input.Width * 16, Input.Height * 16);
                            break;
                        case Scalers.FastResize200:
                            Output = Scaler.FastResize(Input, Input.Width * 2, Input.Height * 2);
                            break;
                        case Scalers.FastResize300:
                            Output = Scaler.FastResize(Input, Input.Width * 3, Input.Height * 3);
                            break;
                        case Scalers.FastResize400:
                            Output = Scaler.FastResize(Input, Input.Width * 4, Input.Height * 4);
                            break;
                        case Scalers.FastResize600:
                            Output = Scaler.FastResize(Input, Input.Width * 6, Input.Height * 6);
                            break;
                        case Scalers.FastResize800:
                            Output = Scaler.FastResize(Input, Input.Width * 8, Input.Height * 8);
                            break;
                        case Scalers.FastResize900:
                            Output = Scaler.FastResize(Input, Input.Width * 9, Input.Height * 9);
                            break;
                        case Scalers.FastResize1200:
                            Output = Scaler.FastResize(Input, Input.Width * 12, Input.Height * 12);
                            break;
                        case Scalers.FastResize1600:
                            Output = Scaler.FastResize(Input, Input.Width * 16, Input.Height * 16);
                            break;
                        case Scalers.SharpResize200:
                            Output = Scaler.SharpResize(Input, Input.Width * 2, Input.Height * 2);
                            break;
                        case Scalers.SharpResize300:
                            Output = Scaler.SharpResize(Input, Input.Width * 3, Input.Height * 3);
                            break;
                        case Scalers.SharpResize400:
                            Output = Scaler.SharpResize(Input, Input.Width * 4, Input.Height * 4);
                            break;
                        case Scalers.SharpResize600:
                            Output = Scaler.SharpResize(Input, Input.Width * 6, Input.Height * 6);
                            break;
                        case Scalers.SharpResize800:
                            Output = Scaler.SharpResize(Input, Input.Width * 8, Input.Height * 8);
                            break;
                        case Scalers.SharpResize900:
                            Output = Scaler.SharpResize(Input, Input.Width * 9, Input.Height * 9);
                            break;
                        case Scalers.SharpResize1200:
                            Output = Scaler.SharpResize(Input, Input.Width * 12, Input.Height * 12);
                            break;
                        case Scalers.SharpResize1600:
                            Output = Scaler.SharpResize(Input, Input.Width * 16, Input.Height * 16);
                            break;
                        case Scalers.Resize2x:
                            Output = Scaler.Resize2x(Input);
                            break;
                        case Scalers.Resize3x:
                            Output = Scaler.Resize3x(Input);
                            break;
                        case Scalers.Resize4x:
                            Output = Scaler.Resize4x(Input);
                            break;
                        case Scalers.Resize6x:
                            Output = Scaler.Resize6x(Input);
                            break;
                        case Scalers.Resize8x:
                            Output = Scaler.Resize8x(Input);
                            break;
                        case Scalers.Resize9x:
                            Output = Scaler.Resize9x(Input);
                            break;
                        case Scalers.Resize12x:
                            Output = Scaler.Resize12x(Input);
                            break;
                        case Scalers.Resize16x:
                            Output = Scaler.Resize16x(Input);
                            break;
                        case Scalers.Scale2x:
                            Output = Scaler.Scale2x(Input);
                            break;
                        case Scalers.Scale3x:
                            Output = Scaler.Scale3x(Input);
                            break;
                        case Scalers.Scale4x:
                            Output = Scaler.Scale4x(Input);
                            break;
                        case Scalers.Scale6x:
                            Output = Scaler.Scale6x(Input);
                            break;
                        case Scalers.Scale8x:
                            Output = Scaler.Scale8x(Input);
                            break;
                        case Scalers.Scale9x:
                            Output = Scaler.Scale9x(Input);
                            break;
                        case Scalers.Scale12x:
                            Output = Scaler.Scale12x(Input);
                            break;
                        case Scalers.Scale16x:
                            Output = Scaler.Scale16x(Input);
                            break;
                    }
                }
                catch
                {
                    Output = null;
                }
                return Output;
            }
            else
                return Output;
        }

        /// <summary>
        /// Resize a Bitmap to even sizes for width and height
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <returns>Bitmap with new even sizes</returns>
        public static Bitmap? ResizeToEvenSize(Bitmap Input)
        {
            //invalid image
            if (Input == null || Input.Width == 0 || Input.Height == 0)
                return null;

            //no resize needed
            if (Input.Width % 2 == 0 && Input.Height % 2 == 0) 
                return Input;

            //make sizes even
            int Width;
            int Height;
            
            if (Input.Width % 2 == 0)
                Width = Input.Width;
            else
                Width = Input.Width + 1;
            if (Input.Height % 2 == 0)
                Height = Input.Height;
            else
                Height = Input.Height + 1;

            //resize image to even sizes
            Bitmap? Output = Resize(Input, Width, Height);
            return Output;
        }

        /// <summary>
        /// Resize a Bitmap with default .net Bitmap class function
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <param name="Width">New width</param>
        /// <param name="Height">New height</param>
        /// <returns>Bitmap with new size</returns>
        public static Bitmap? Resize(Bitmap Input, int Width, int Height)
        {
            //invalid image
            if (Input == null || Input.Width == 0 || Input.Height == 0)
                return null;
            try
            {
                return new Bitmap(Input, Width, Height);
            }
            catch
            {
                return new Bitmap(Width, Height); ;
            }
        }

        /// <summary>
        /// Resize a Bitmap with more sharpness but slower than with the default .net Bitmap class function
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <param name="Width">New width</param>
        /// <param name="Height">New height</param>
        /// <returns>Bitmap with new size</returns>
        public static Bitmap? SharpResize(Bitmap Input, int Width, int Height)
        {
            //invalid image
            if (Input == null || Input.Width == 0 || Input.Height == 0)
                return null;

            //base settings
            int InputWidth = Input.Width;
            int InputHeight = Input.Height;
            byte ColorBytesCount;
            if (Input.PixelFormat == PixelFormat.Format32bppArgb)
                ColorBytesCount = 4;
            else if (Input.PixelFormat == PixelFormat.Format24bppRgb)
                ColorBytesCount = 3;
            else
                ColorBytesCount = 3;

            //copy input Bitmap data into array and create output Bitmap data array
            Bitmap Output = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            BitmapData InputData = Input.LockBits(new Rectangle(0, 0, Input.Width, Input.Height), ImageLockMode.ReadOnly, Input.PixelFormat);
            BitmapData OutputData = Output.LockBits(new Rectangle(0, 0, Output.Width, Output.Height), ImageLockMode.ReadWrite, Output.PixelFormat);
            int BytesIn = Math.Abs(InputData.Stride) * Input.Height;
            int BytesOut = Math.Abs(OutputData.Stride) * Output.Height;
            byte[] RGBValuesIn = new byte[BytesIn];
            byte[] RGBValuesOut = new byte[BytesOut];
            Marshal.Copy(InputData.Scan0, RGBValuesIn, 0, BytesIn);
            Input.UnlockBits(InputData);

            //Resizing
            int Pos = 0;
            int Xin = 0;
            int Yin = 0;
            int Xinold = -1;
            int Yinold = -1;
            Color E = Color.Black;
            decimal FactorX = (decimal)Width / (decimal)InputWidth;
            decimal FactorY = (decimal)Height / (decimal)InputHeight;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    //read Pixel from input image
                    Color GetPixel(int x, int y)
                    {
                        if (x >= InputWidth)
                            x = InputWidth - 1;
                        if (y >= InputHeight)
                            y = InputHeight - 1;
                        if (ColorBytesCount == 4)
                        {
                            int Position = (x * ColorBytesCount) + (y * InputWidth * ColorBytesCount);
                            return Color.FromArgb(RGBValuesIn[Position + 3], RGBValuesIn[Position + 2], RGBValuesIn[Position + 1], RGBValuesIn[Position]);
                        }
                        else
                        {
                            int Position = (x * ColorBytesCount) + (y * InputWidth * ColorBytesCount);
                            return Color.FromArgb(255, RGBValuesIn[Position + 2], RGBValuesIn[Position + 1], RGBValuesIn[Position]);
                        }
                    }

                    //default input image Pixel depending on factor between input and output Bitmap
                    Xin = (int) (x / FactorX);
                    Yin = (int) (y / FactorY);   
                    
                    //only read default Pixel if Pixel position is different
                    if (Xin != Xinold || Yin != Yinold)
                        E = GetPixel(Xin, Yin);
                    Xinold = Xin;
                    Yinold = Yin;

                    //paint last default Pixel to output image data (again)
                    Pos = x * 4 + y * Math.Abs(OutputData.Stride);
                    if (Pos <= RGBValuesOut.Length - 4)
                    {
                        RGBValuesOut[Pos] = E.B;
                        RGBValuesOut[Pos + 1] = E.G;
                        RGBValuesOut[Pos + 2] = E.R;
                        RGBValuesOut[Pos + 3] = E.A;
                    }
                }
            }

            //copy output image data to Bitmap
            Marshal.Copy(RGBValuesOut, 0, OutputData.Scan0, RGBValuesOut.Length);
            Output.UnlockBits(OutputData);
            return Output;
        }

        /// <summary>
        /// Resize a Bitmap faster but with lower quality than with the default .net Bitmap class function
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <param name="Width">New width</param>
        /// <param name="Height">New height</param>
        /// <returns>Bitmap with new size</returns>
        public static Bitmap? FastResize(Bitmap Input, int Width, int Height)
        {
            //invalid image
            if (Input == null || Input.Width == 0 || Input.Height == 0)
                return null;

            //base settings
            int InputWidth = Input.Width;
            int InputHeight = Input.Height;
            byte ColorBytesCount;
            if (Input.PixelFormat == PixelFormat.Format32bppArgb)
                ColorBytesCount = 4;
            else if (Input.PixelFormat == PixelFormat.Format24bppRgb)
                ColorBytesCount = 3;
            else
                ColorBytesCount = 3;

            //copy input Bitmap data into array and create output Bitmap data array
            Bitmap Output = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            BitmapData InputData = Input.LockBits(new Rectangle(0, 0, Input.Width, Input.Height), ImageLockMode.ReadOnly, Input.PixelFormat);
            BitmapData OutputData = Output.LockBits(new Rectangle(0, 0, Output.Width, Output.Height), ImageLockMode.ReadWrite, Output.PixelFormat);
            int BytesIn = Math.Abs(InputData.Stride) * Input.Height;
            int BytesOut = Math.Abs(OutputData.Stride) * Output.Height;
            byte[] RGBValuesIn = new byte[BytesIn];
            byte[] RGBValuesOut = new byte[BytesOut];
            Marshal.Copy(InputData.Scan0, RGBValuesIn, 0, BytesIn);
            Input.UnlockBits(InputData);

            //Resizing
            float FactorX = (float)InputWidth / Width;
            float FactorY = (float)InputHeight / Height;

            //int Pos = 0;
            for (int y = 0; y <= Height; y+=1)
            {
                for (int x = 0; x <= Width; x+=1)
                {
                    //read Pixel from input image
                    Color GetPixel(int x, int y)
                    {
                        if (FactorX > 0)
                            x = (int)Math.Round(x * FactorX, 0, MidpointRounding.ToZero);
                        if (FactorY > 0)
                            y = (int)Math.Round(y * FactorY, 0, MidpointRounding.ToZero);
                        if (x >= InputWidth)
                            x = InputWidth - 1;
                        if (y >= InputHeight)
                            y = InputHeight - 1;
                        if (ColorBytesCount == 4)
                        {
                            int Position = (x * ColorBytesCount) + (y * InputWidth * ColorBytesCount);
                            return Color.FromArgb(RGBValuesIn[Position + 3], RGBValuesIn[Position + 2], RGBValuesIn[Position + 1], RGBValuesIn[Position]);
                        }
                        else
                        {
                            int Position = (x * ColorBytesCount) + (y * InputWidth * ColorBytesCount);
                            return Color.FromArgb(255, RGBValuesIn[Position + 2], RGBValuesIn[Position + 1], RGBValuesIn[Position]);
                        }
                    }

                    //default input image Pixel
                    Color E = GetPixel(x, y);

                    //paint default Pixel to output image data
                    int Pos = x * 4 + y * Math.Abs(OutputData.Stride);
                    if (Pos <= RGBValuesOut.Length - 4)
                    {
                        RGBValuesOut[Pos] = E.B;
                        RGBValuesOut[Pos + 1] = E.G;
                        RGBValuesOut[Pos + 2] = E.R;
                        RGBValuesOut[Pos + 3] = E.A;
                    }
                }
            }

            //copy output image data to Bitmap
            Marshal.Copy(RGBValuesOut, 0, OutputData.Scan0, RGBValuesOut.Length);
            Output.UnlockBits(OutputData);
            return Output;
        }

        /// <summary>
        /// Resize a Bitmap to 200% of the original size
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <returns>Bitmap with new size</returns>
        public static Bitmap? Resize2x(Bitmap Input)
        {
            //invalid image
            if (Input == null || Input.Width == 0 || Input.Height == 0)
                return null;

            //base settings
            int Height = Input.Height;
            int Width = Input.Width;
            byte ColorBytesCount;
            if (Input.PixelFormat == PixelFormat.Format32bppArgb)
                ColorBytesCount = 4;
            else if (Input.PixelFormat == PixelFormat.Format24bppRgb)
                ColorBytesCount = 3;
            else
                ColorBytesCount = 3;

            //copy input Bitmap data into array and create output Bitmap data array
            Bitmap Output = new Bitmap(Width * 2, Height * 2, PixelFormat.Format32bppArgb);
            BitmapData InputData = Input.LockBits(new Rectangle(0, 0, Input.Width, Input.Height), ImageLockMode.ReadOnly, Input.PixelFormat);
            BitmapData OutputData = Output.LockBits(new Rectangle(0, 0, Output.Width, Output.Height), ImageLockMode.ReadWrite, Output.PixelFormat);
            int BytesIn = Math.Abs(InputData.Stride) * Input.Height;
            int BytesOut = Math.Abs(OutputData.Stride) * Output.Height;
            byte[] RGBValuesIn = new byte[BytesIn];
            byte[] RGBValuesOut = new byte[BytesOut];
            Marshal.Copy(InputData.Scan0, RGBValuesIn, 0, BytesIn);
            Input.UnlockBits(InputData);

            //Upscaling
            int Pos = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    //read Pixel from input image
                    Color GetPixel(int x, int y)
                    {
                        if (ColorBytesCount == 4)
                        {
                            int Position = (x * ColorBytesCount) + (y * Width * ColorBytesCount);
                            return Color.FromArgb(RGBValuesIn[Position + 3], RGBValuesIn[Position + 2], RGBValuesIn[Position + 1], RGBValuesIn[Position]);
                        }
                        else
                        {
                            int Position = (x * ColorBytesCount) + (y * Width * ColorBytesCount);
                            return Color.FromArgb(255, RGBValuesIn[Position + 2], RGBValuesIn[Position + 1], RGBValuesIn[Position]);
                        }
                    }

                    //default input image Pixel
                    Color E = GetPixel(x, y);

                    //paint default Pixel as scaled 2x2 Pixels to output image data
                    Pos = x * 8 + y * 2 * Math.Abs(OutputData.Stride);
                    RGBValuesOut[Pos] = E.B;
                    RGBValuesOut[Pos + 1] = E.G;
                    RGBValuesOut[Pos + 2] = E.R;
                    RGBValuesOut[Pos + 3] = E.A;
                    RGBValuesOut[Pos + 4] = E.B;
                    RGBValuesOut[Pos + 5] = E.G;
                    RGBValuesOut[Pos + 6] = E.R;
                    RGBValuesOut[Pos + 7] = E.A;
                    Pos += Math.Abs(OutputData.Stride);
                    if (Pos > RGBValuesOut.Length - 4)
                        break;
                    RGBValuesOut[Pos] = E.B;
                    RGBValuesOut[Pos + 1] = E.G;
                    RGBValuesOut[Pos + 2] = E.R;
                    RGBValuesOut[Pos + 3] = E.A;
                    RGBValuesOut[Pos + 4] = E.B;
                    RGBValuesOut[Pos + 5] = E.G;
                    RGBValuesOut[Pos + 6] = E.R;
                    RGBValuesOut[Pos + 7] = E.A;
                }
            }

            //copy output image data to Bitmap
            Marshal.Copy(RGBValuesOut, 0, OutputData.Scan0, RGBValuesOut.Length);
            Output.UnlockBits(OutputData);
            return Output;
        }

        /// <summary>
        /// Resize a Bitmap to 300% of the original size
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <returns>Bitmap with new size</returns>
        public static Bitmap? Resize3x(Bitmap Input)
        {
            //invalid image
            if (Input == null || Input.Width == 0 || Input.Height == 0)
                return null;

            //base settings
            int Height = Input.Height;
            int Width = Input.Width;
            byte ColorBytesCount;
            if (Input.PixelFormat == PixelFormat.Format32bppArgb)
                ColorBytesCount = 4;
            else if (Input.PixelFormat == PixelFormat.Format24bppRgb)
                ColorBytesCount = 3;
            else
                ColorBytesCount = 3;

            //copy input Bitmap data into array and create output Bitmap data array
            Bitmap Output = new Bitmap(Width * 3, Height * 3, PixelFormat.Format32bppArgb);
            BitmapData InputData = Input.LockBits(new Rectangle(0, 0, Input.Width, Input.Height), ImageLockMode.ReadOnly, Input.PixelFormat);
            BitmapData OutputData = Output.LockBits(new Rectangle(0, 0, Output.Width, Output.Height), ImageLockMode.ReadWrite, Output.PixelFormat);
            int BytesIn = Math.Abs(InputData.Stride) * Input.Height;
            int BytesOut = Math.Abs(OutputData.Stride) * Output.Height;
            byte[] RGBValuesIn = new byte[BytesIn];
            byte[] RGBValuesOut = new byte[BytesOut];
            Marshal.Copy(InputData.Scan0, RGBValuesIn, 0, BytesIn);
            Input.UnlockBits(InputData);

            //Upscaling
            int Pos = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    //read Pixel from input image
                    Color GetPixel(int x, int y)
                    {
                        if (ColorBytesCount == 4)
                        {
                            int Position = (x * ColorBytesCount) + (y * Width * ColorBytesCount);
                            return Color.FromArgb(RGBValuesIn[Position + 3], RGBValuesIn[Position + 2], RGBValuesIn[Position + 1], RGBValuesIn[Position]);
                        }
                        else
                        {
                            int Position = (x * ColorBytesCount) + (y * Width * ColorBytesCount);
                            return Color.FromArgb(255, RGBValuesIn[Position + 2], RGBValuesIn[Position + 1], RGBValuesIn[Position]);
                        }
                    }

                    //default input image Pixel
                    Color E = GetPixel(x, y);
                    
                    //paint default Pixel as scaled 3x3 Pixels to output image data
                    Pos = x * 12 + y * 3 * Math.Abs(OutputData.Stride);
                    RGBValuesOut[Pos] = E.B;
                    RGBValuesOut[Pos + 1] = E.G;
                    RGBValuesOut[Pos + 2] = E.R;
                    RGBValuesOut[Pos + 3] = E.A;
                    RGBValuesOut[Pos + 4] = E.B;
                    RGBValuesOut[Pos + 5] = E.G;
                    RGBValuesOut[Pos + 6] = E.R;
                    RGBValuesOut[Pos + 7] = E.A;
                    RGBValuesOut[Pos + 8] = E.B;
                    RGBValuesOut[Pos + 9] = E.G;
                    RGBValuesOut[Pos + 10] = E.R;
                    RGBValuesOut[Pos + 11] = E.A;
                    Pos += Math.Abs(OutputData.Stride);
                    if (Pos > RGBValuesOut.Length - 9)
                        break;
                    RGBValuesOut[Pos] = E.B;
                    RGBValuesOut[Pos + 1] = E.G;
                    RGBValuesOut[Pos + 2] = E.R;
                    RGBValuesOut[Pos + 3] = E.A;
                    RGBValuesOut[Pos + 4] = E.B;
                    RGBValuesOut[Pos + 5] = E.G;
                    RGBValuesOut[Pos + 6] = E.R;
                    RGBValuesOut[Pos + 7] = E.A;
                    RGBValuesOut[Pos + 8] = E.B;
                    RGBValuesOut[Pos + 9] = E.G;
                    RGBValuesOut[Pos + 10] = E.R;
                    RGBValuesOut[Pos + 11] = E.A;
                    Pos += Math.Abs(OutputData.Stride);
                    if (Pos > RGBValuesOut.Length - 9)
                        break;
                    RGBValuesOut[Pos] = E.B;
                    RGBValuesOut[Pos + 1] = E.G;
                    RGBValuesOut[Pos + 2] = E.R;
                    RGBValuesOut[Pos + 3] = E.A;
                    RGBValuesOut[Pos + 4] = E.B;
                    RGBValuesOut[Pos + 5] = E.G;
                    RGBValuesOut[Pos + 6] = E.R;
                    RGBValuesOut[Pos + 7] = E.A;
                    RGBValuesOut[Pos + 8] = E.B;
                    RGBValuesOut[Pos + 9] = E.G;
                    RGBValuesOut[Pos + 10] = E.R;
                    RGBValuesOut[Pos + 11] = E.A;
                }
            }

            //copy output image data to Bitmap
            Marshal.Copy(RGBValuesOut, 0, OutputData.Scan0, RGBValuesOut.Length);
            Output.UnlockBits(OutputData);
            return Output;
        }

        /// <summary>
        /// Resize a Bitmap to 400% of the original size
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <returns>Bitmap with new size</returns>
        public static Bitmap? Resize4x(Bitmap Input)
        {
            return Resize2x(Resize2x(Input));
        }

        /// <summary>
        /// Resize a Bitmap to 600% of the original size
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <returns>Bitmap with new size</returns>
        public static Bitmap? Resize6x(Bitmap Input)
        {
            return Resize3x(Resize2x(Input));
        }

        /// <summary>
        /// Resize a Bitmap to 800% of the original size
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <returns>Bitmap with new size</returns>
        public static Bitmap? Resize8x(Bitmap Input)
        {
            return Resize4x(Resize2x(Input));
        }

        /// <summary>
        /// Resize a Bitmap to 900% of the original size
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <returns>Bitmap with new size</returns>
        public static Bitmap? Resize9x(Bitmap Input)
        {
            return Resize3x(Resize3x(Input));
        }

        /// <summary>
        /// Resize a Bitmap to 1600% of the original size
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <returns>Bitmap with new size</returns>
        public static Bitmap? Resize12x(Bitmap Input)
        {
            return Resize3x(Resize4x(Input));
        }

        /// <summary>
        /// Resize a Bitmap to 1600% of the original size
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <returns>Bitmap with new size</returns>
        public static Bitmap? Resize16x(Bitmap Input)
        {
            return Resize4x(Resize4x(Input));
        }

        /*
        * The Scale2x and Scale3x effects are developed by Andrea Mazzoleni in the year 2001 for the project AdvanceMAME.
        * Contact: https://www.scale2x.it/authors
        */
        /// <summary>
        /// Resize a Bitmap using the Scale2x algorithm to 200% of the original size
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <returns>Bitmap with new size</returns>
        public static Bitmap? Scale2x(Bitmap Input)
        {
            //invalid image
            if (Input == null || Input.Width == 0 || Input.Height == 0)
                return null;

            //base settings
            int Height = Input.Height;
            int Width = Input.Width;
            byte ColorBytesCount;
            if (Input.PixelFormat == PixelFormat.Format32bppArgb)
                ColorBytesCount = 4;
            else if (Input.PixelFormat == PixelFormat.Format24bppRgb)
                ColorBytesCount = 3;
            else
                ColorBytesCount = 3;

            //copy input Bitmap data into array and create output Bitmap data array
            Bitmap Output = new Bitmap(Width * 2, Height * 2, PixelFormat.Format32bppArgb);
            BitmapData InputData = Input.LockBits(new Rectangle(0, 0, Input.Width, Input.Height), ImageLockMode.ReadOnly, Input.PixelFormat);
            BitmapData OutputData = Output.LockBits(new Rectangle(0, 0, Output.Width, Output.Height), ImageLockMode.ReadWrite, Output.PixelFormat);
            int BytesIn = Math.Abs(InputData.Stride) * Input.Height;
            int BytesOut = Math.Abs(OutputData.Stride) * Output.Height;
            byte[] RGBValuesIn = new byte[BytesIn];
            byte[] RGBValuesOut = new byte[BytesOut];
            Marshal.Copy(InputData.Scan0, RGBValuesIn, 0, BytesIn);
            Input.UnlockBits(InputData);

            //Upscaling
            int Pos = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    //read Pixel from input image
                    Color GetPixel(int x, int y)
                    {
                        if (ColorBytesCount == 4)
                        {
                            int Position = (x * ColorBytesCount) + (y * Width * ColorBytesCount);
                            return Color.FromArgb(RGBValuesIn[Position + 3], RGBValuesIn[Position + 2], RGBValuesIn[Position + 1], RGBValuesIn[Position]);
                        }
                        else
                        {
                            int Position = (x * ColorBytesCount) + (y * Width * ColorBytesCount);
                            return Color.FromArgb(255, RGBValuesIn[Position + 2], RGBValuesIn[Position + 1], RGBValuesIn[Position]);
                        }
                    }

                    //default input image Pixel
                    Color E = GetPixel(x, y);

                    //surrounding Pixel which are needed for later upscale calculation
                    Color B, D, H, F, E0, E1, E2, E3;
                    if (y == 0)
                        B = E;
                    else
                        B = GetPixel(x, y - 1);
                    if (y == Height - 1)
                        H = E;
                    else
                        H = GetPixel(x, y + 1);
                    if (x == 0)
                        D = E;
                    else
                        D = GetPixel(x - 1, y);
                    if (x == Width-1)
                        F = E;
                    else
                        F = GetPixel(x + 1, y);

                    //calculate new scaled Pixels
                    if ((B != H) && (D != F))
                    {
                        E0 = D == B ? D : E;
                        E1 = B == F ? F : E;
                        E2 = D == H ? D : E;
                        E3 = H == F ? F : E;
                    }
                    else
                    {
                        E0 = E;
                        E1 = E;
                        E2 = E;
                        E3 = E;
                    }

                    //paint default Pixel as scaled 2x2 Pixels to output image data
                    Pos = x * 8 + y * 2 * Math.Abs(OutputData.Stride);
                    RGBValuesOut[Pos] = E0.B;
                    RGBValuesOut[Pos + 1] = E0.G;
                    RGBValuesOut[Pos + 2] = E0.R;
                    RGBValuesOut[Pos + 3] = E0.A;
                    RGBValuesOut[Pos + 4] = E1.B;
                    RGBValuesOut[Pos + 5] = E1.G;
                    RGBValuesOut[Pos + 6] = E1.R;
                    RGBValuesOut[Pos + 7] = E1.A;
                    Pos += Math.Abs(OutputData.Stride);
                    if (Pos > RGBValuesOut.Length - 4)
                        break;
                    RGBValuesOut[Pos] = E2.B;
                    RGBValuesOut[Pos + 1] = E2.G;
                    RGBValuesOut[Pos + 2] = E2.R;
                    RGBValuesOut[Pos + 3] = E2.A;
                    RGBValuesOut[Pos + 4] = E3.B;
                    RGBValuesOut[Pos + 5] = E3.G;
                    RGBValuesOut[Pos + 6] = E3.R;
                    RGBValuesOut[Pos + 7] = E3.A;
                }
            }

            //copy output image data to Bitmap
            Marshal.Copy(RGBValuesOut, 0, OutputData.Scan0, RGBValuesOut.Length);
            Output.UnlockBits(OutputData);
            return Output;
        }

        /*
        * The Scale2x and Scale3x effects are developed by Andrea Mazzoleni in the year 2001 for the project AdvanceMAME.
        * Contact: https://www.scale2x.it/authors
        */
        /// <summary>
        /// Resize a Bitmap using the Scale3x algorithm to 300% of the original size
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <returns>Bitmap with new size</returns>
        public static Bitmap? Scale3x(Bitmap Input)
        {
            //invalid image
            if (Input == null || Input.Width == 0 || Input.Height == 0)
                return null;

            //base settings
            int Height = Input.Height;
            int Width = Input.Width;
            byte ColorBytesCount;
            if (Input.PixelFormat == PixelFormat.Format32bppArgb)
                ColorBytesCount = 4;
            else if (Input.PixelFormat == PixelFormat.Format24bppRgb)
                ColorBytesCount = 3;
            else
                ColorBytesCount = 3;

            //copy input Bitmap data into array and create output Bitmap data array
            Bitmap Output = new Bitmap(Width * 3, Height * 3, PixelFormat.Format32bppArgb);
            BitmapData InputData = Input.LockBits(new Rectangle(0, 0, Input.Width, Input.Height), ImageLockMode.ReadOnly, Input.PixelFormat);
            BitmapData OutputData = Output.LockBits(new Rectangle(0, 0, Output.Width, Output.Height), ImageLockMode.ReadWrite, Output.PixelFormat);
            int BytesIn = Math.Abs(InputData.Stride) * Input.Height;
            int BytesOut = Math.Abs(OutputData.Stride) * Output.Height;
            byte[] RGBValuesIn = new byte[BytesIn];
            byte[] RGBValuesOut = new byte[BytesOut];
            Marshal.Copy(InputData.Scan0, RGBValuesIn, 0, BytesIn);
            Input.UnlockBits(InputData);

            //Upscaling
            int Pos = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    //read Pixel from input image
                    Color GetPixel(int x, int y)
                    {
                        if (ColorBytesCount == 4)
                        {
                            int Position = (x * ColorBytesCount) + (y * Width * ColorBytesCount);
                            return Color.FromArgb(RGBValuesIn[Position + 3], RGBValuesIn[Position + 2], RGBValuesIn[Position + 1], RGBValuesIn[Position]);
                        }
                        else
                        {
                            int Position = (x * ColorBytesCount) + (y * Width * ColorBytesCount);
                            return Color.FromArgb(255, RGBValuesIn[Position + 2], RGBValuesIn[Position + 1], RGBValuesIn[Position]);
                        }
                    }

                    //default input image Pixel
                    Color E = GetPixel(x, y);

                    //surrounding Pixel which are needed for later upscale calculation
                    Color A, B, C, D, E0, E1, E2, E3, E4, E5, E6, E7, E8, F, G, H, I;
                    if (x == 0 || y == 0)
                        A = E;
                    else
                        A = GetPixel(x - 1, y - 1);
                    if (y == 0)
                        B = E;
                    else
                        B = GetPixel(x, y - 1);
                    if (x == Width - 1 || y == 0)
                        C = E;
                    else
                        C = GetPixel(x + 1, y - 1);
                    if (x == 0)
                        D = E;
                    else
                        D = GetPixel(x - 1, y);
                    if (x == Width - 1)
                        F = E;
                    else
                        F = GetPixel(x + 1, y);
                    if (x == 0 || y == Height - 1)
                        G = E;
                    else
                        G = GetPixel(x - 1, y + 1);
                    if (y == Height - 1)
                        H = E;
                    else
                        H = GetPixel(x, y + 1);
                    if (x == Width - 1 || y == Height - 1)
                        I = E;
                    else
                        I = GetPixel(x + 1, y + 1);

                    //calculate new scaled Pixels
                    if (B != H && D != F)
                    {
                        E0 = D == B ? D : E;
                        E1 = (D == B && E != C) || (B == F && E != A) ? B : E;
                        E2 = B == F ? F : E;
                        E3 = (D == B && E != G) || (D == H && E != A) ? D : E;
                        E4 = E;
                        E5 = (B == F && E != I) || (H == F && E != C) ? F : E;
                        E6 = D == H ? D : E;
                        E7 = (D == H && E != I) || (H == F && E != G) ? H : E;
                        E8 = H == F ? F : E;
                    }
                    else
                    {
                        E0 = E;
                        E1 = E;
                        E2 = E;
                        E3 = E;
                        E4 = E;
                        E5 = E;
                        E6 = E;
                        E7 = E;
                        E8 = E;
                    }

                    //paint default Pixel as scaled 3x3 Pixels to output image data
                    Pos = x * 12 + y * 3 * Math.Abs(OutputData.Stride);
                    RGBValuesOut[Pos] = E0.B;
                    RGBValuesOut[Pos + 1] = E0.G;
                    RGBValuesOut[Pos + 2] = E0.R;
                    RGBValuesOut[Pos + 3] = E0.A;
                    RGBValuesOut[Pos + 4] = E1.B;
                    RGBValuesOut[Pos + 5] = E1.G;
                    RGBValuesOut[Pos + 6] = E1.R;
                    RGBValuesOut[Pos + 7] = E1.A;
                    RGBValuesOut[Pos + 8] = E2.B;
                    RGBValuesOut[Pos + 9] = E2.G;
                    RGBValuesOut[Pos + 10] = E2.R;
                    RGBValuesOut[Pos + 11] = E2.A;
                    Pos += Math.Abs(OutputData.Stride);
                    if (Pos > RGBValuesOut.Length - 9)
                        break;
                    RGBValuesOut[Pos] = E3.B;
                    RGBValuesOut[Pos + 1] = E3.G;
                    RGBValuesOut[Pos + 2] = E3.R;
                    RGBValuesOut[Pos + 3] = E3.A;
                    RGBValuesOut[Pos + 4] = E4.B;
                    RGBValuesOut[Pos + 5] = E4.G;
                    RGBValuesOut[Pos + 6] = E4.R;
                    RGBValuesOut[Pos + 7] = E4.A;
                    RGBValuesOut[Pos + 8] = E5.B;
                    RGBValuesOut[Pos + 9] = E5.G;
                    RGBValuesOut[Pos + 10] = E5.R;
                    RGBValuesOut[Pos + 11] = E5.A;
                    Pos += Math.Abs(OutputData.Stride);
                    if (Pos > RGBValuesOut.Length - 9)
                        break;
                    RGBValuesOut[Pos] = E6.B;
                    RGBValuesOut[Pos + 1] = E6.G;
                    RGBValuesOut[Pos + 2] = E6.R;
                    RGBValuesOut[Pos + 3] = E6.A;
                    RGBValuesOut[Pos + 4] = E7.B;
                    RGBValuesOut[Pos + 5] = E7.G;
                    RGBValuesOut[Pos + 6] = E7.R;
                    RGBValuesOut[Pos + 7] = E7.A;
                    RGBValuesOut[Pos + 8] = E8.B;
                    RGBValuesOut[Pos + 9] = E8.G;
                    RGBValuesOut[Pos + 10] = E8.R;
                    RGBValuesOut[Pos + 11] = E8.A;

                }
            }

            //copy output image data to Bitmap
            Marshal.Copy(RGBValuesOut, 0, OutputData.Scan0, RGBValuesOut.Length);
            Output.UnlockBits(OutputData);
            return Output;
        }

        /// <summary>
        /// Resize a Bitmap using the Scale4x algorithm to 400% of the original size
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <returns>Bitmap with new size</returns>
        public static Bitmap? Scale4x(Bitmap Input)
        {
            return Scale2x(Scale2x(Input));
        }

        /// <summary>
        /// Resize a Bitmap using the Scale6x algorithm to 600% of the original size
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <returns>Bitmap with new size</returns>
        public static Bitmap? Scale6x(Bitmap Input)
        {
            return Scale3x(Scale2x(Input));
        }

        /// <summary>
        /// Resize a Bitmap using the Scale8x algorithm to 800% of the original size
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <returns>Bitmap with new size</returns>
        public static Bitmap? Scale8x(Bitmap Input)
        {
            return Scale4x(Scale2x(Input));
        }

        /// <summary>
        /// Resize a Bitmap using the Scale6x algorithm to 900% of the original size
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <returns>Bitmap with new size</returns>
        public static Bitmap? Scale9x(Bitmap Input)
        {
            return Scale3x(Scale3x(Input));
        }

        /// <summary>
        /// Resize a Bitmap using the Scale12x algorithm to 1200% of the original size
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <returns>Bitmap with new size</returns>
        public static Bitmap? Scale12x(Bitmap Input)
        {
            return Scale3x(Scale4x(Input));
        }

        /// <summary>
        /// Resize a Bitmap using the Scale16x algorithm to 1600% of the original size
        /// </summary>
        /// <param name="Input">Bitmap to resize</param>
        /// <returns>Bitmap with new size</returns>
        public static Bitmap? Scale16x(Bitmap Input)
        {
            return Scale4x(Scale4x(Input));
        }
    }
}