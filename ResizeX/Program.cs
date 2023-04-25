using System.Drawing;
using CommandLine;
using CommandLine.Text;
using Spectre.Console;
using System.Reflection;
using Scaler = Feuster.Imaging.Resizing.Scaler;
using System.Drawing.Imaging;

namespace Feuster.Imaging.Resizing
{
    class ResizeX
    {
        public static string Figlet => @"                                             ____           _        __  __
                                            |  _ \ ___  ___(_)_______\ \/ /
                                            | |_) / _ \/ __| |_  / _ \\  / 
                                            |  _ <  __/\__ \ |/ /  __//  \ 
                                            |_| \_\___||___/_/___\___/_/\_\";

        //GitVersion will be only be actualized/overwritten when using Cake build!
        public const string GitVersion = "git-55273cc";
        public static string Version = Assembly.GetEntryAssembly().GetName().Version.Major.ToString() + "." + Assembly.GetEntryAssembly().GetName().Version.Build.ToString();
        public static string InputFile = string.Empty;
        public static string OutputFile = string.Empty;
        public static string ScalerAlgorithm = string.Empty;
        public static ImageFormat OutputImageFormat = ImageFormat.Bmp;
        public static bool KeepConsoleWindowOpen;
        public static bool PerformanceTest;
        public static bool PerformanceTestNoOutputFiles;
        public static bool FastResize;
        public static bool Resize;
        public static bool SharpResize;
        public static int NewWidth;
        public static int NewHeight;

        //Keep console open by asking for a key press and exit on demand
        static void PressAnyKey(bool ExitProgram = false)
        {
            if (KeepConsoleWindowOpen)
            {
                AnsiConsole.WriteLine("");
                AnsiConsole.Write(new Rule("[blue][teal]<Press any Key>[/][/]"));
                AnsiConsole.WriteLine("");
                Console.ReadKey();
            }
            else
                AnsiConsole.WriteLine("");
            if (ExitProgram)
                Environment.Exit(0);
        }

        //Shorten a string to a fixed length
        public static string ShortString(string Text, int MaxLength)
        {
            if (Text == null)
                return string.Empty;
            if (Text.Length < 6 || MaxLength > Text.Length)
                return Text;
            string Part1 = String.Empty;
            string Part2 = String.Empty;
            int PartLength = (MaxLength - 3) / 2;
            if ((MaxLength - 3) % 2 == 0)
                Part1 = Text.Substring(0, PartLength);
            else
                Part1 = Text.Substring(0, PartLength + 1);
            Part2 = Text.Substring(Text.Length - PartLength);
            return Part1 + "..." + Part2;
        }

        //Convert scaler name to enum value
        public static int ScalerNameToScalerEnum(string ScalerName)
        {
            int result = -1;
            if (ScalerName == null || ScalerName == string.Empty)
                return result;

            int Count = Enum.GetValues(typeof(Scaler.Scalers)).Length;
            for (int i = 0; i < Count + 1; i++)
            {
                result = i;
                if (ScalerName.ToLowerInvariant() == ((Enum.GetName(typeof(Scaler.Scalers), (Scaler.Scalers)i) == null) ? string.Empty : Enum.GetName(typeof(Scaler.Scalers), (Scaler.Scalers)i).ToLowerInvariant()))
                    break;
                else
                    result = -1;
            }
            return result;
        }

        //Imageformat from extension
        public static ImageFormat ImageFormatFromExt(string Extension)
        {
            if (Extension == string.Empty)
            {
                AnsiConsole.MarkupLine("[red]Info: no output imageformat available therefore setting Bitmap as default[/]");
                return ImageFormat.Bmp;
            }
            if (Extension.Contains("."))
                Extension = Path.GetExtension(Extension).Replace(".", "").ToUpperInvariant();

            switch (Extension)
            {
                default:
                    AnsiConsole.MarkupLine("[red]Info: unsupported output imageformat " + Extension + " therefore setting Bitmap as default[/]");
                    return ImageFormat.Bmp;
                case "PNG": return ImageFormat.Png;
                case "JPG": return ImageFormat.Jpeg;
                case "JPEG": return ImageFormat.Jpeg;
                case "BMP": return ImageFormat.Bmp;
                case "WMF": return ImageFormat.Wmf;
                case "EMF": return ImageFormat.Emf;
                case "ICO": return ImageFormat.Icon;
                case "ICON": return ImageFormat.Icon;
                case "GIF": return ImageFormat.Gif;
                case "TIF": return ImageFormat.Tiff;
                case "TIFF": return ImageFormat.Tiff;
#pragma warning disable CA1416
                //HEIF & WEBP formats need Windows version 7.0 or 10.0.17763.0 and higher
                case "HEIF": return ImageFormat.Heif;
                case "WEBP": return ImageFormat.Webp;
#pragma warning restore CA1416
            }
        }

        //Commandline options
        public class Options
        {
            [Option('d', "descriptionlist", Required = false, HelpText = "Show the list with scaler descriptions.")]
            public bool DescriptionList { get; set; } = false;
            [Option('f', "fastresize", Required = false, HelpText = "Resize with FastResize algorithm. Needs --width and --height arguments!")]
            public bool FastResize { get; set; } = false;
            [Option('h', "help", Required = false, HelpText = "Show the help.", Hidden = true)]
            public bool Help { get; set; } = false;
            [Option('i', "input", Required = false, HelpText = "Set path to inputfile.")]
            public string Input { get; set; } = string.Empty;
            [Option('k', "keepopen", Required = false, HelpText = "Keep the console window open.")]
            public bool Keepopen { get; set; } = false;
            [Option('l', "list", Required = false, HelpText = "Show the list of supported scalers.")]
            public bool List { get; set; } = false;
            [Option('n', "performance-no-output", Required = false, HelpText = "Performance test which uses all scalers without creating output files.")]
            public bool PerformanceNoOutput { get; set; } = false;
            [Option('o', "output", Required = false, HelpText = "Set path to outputfile.")]
            public string Output { get; set; } = string.Empty;
            [Option('p', "performance", Required = false, HelpText = "Performance test which creates outputfiles for all scalers.")]
            public bool Performance { get; set; } = false;
            [Option('r', "resize", Required = false, HelpText = "Resize with default .net algorithm. Needs --width and --height arguments!")]
            public bool Resize { get; set; } = false;
            [Option('s', "scaler", Required = false, HelpText = "Set fixed scaler algorithm for resizing.")]
            public string Scaler { get; set; } = string.Empty;
            [Option('v', "version", Required = false, HelpText = "Show the version.", Hidden = true)]
            public bool Version { get; set; } = false;
            [Option('x', "width", Required = false, HelpText = "Set the new image width (not for fixed size scalers)")]
            public int Width { get; set; } = 0;
            [Option('y', "height", Required = false, HelpText = "Set the new image height (not for fixed size scalers)")]
            public int Height { get; set; } = 0;
            [Option('z', "sharpresize", Required = false, HelpText = "Resize with SharpResize algorithm. Needs --width and --height arguments!")]
            public bool SharpResize { get; set; } = false;

        }

        //Program start
        static void Main(string[] args)
        {
            AnsiConsole.Clear();
            Console.BufferHeight = 1000;
            AnsiConsole.MarkupLine($"[blue]{Figlet}[/]\n");
            AnsiConsole.Write(new Rule("[blue][bold]Fast Image Resizer[/][/]").Centered());
            AnsiConsole.Write("\n\n");
            AnsiConsole.MarkupLine($"[blue]Version: {Version}-{GitVersion}[/]\n\n");

            //Parse commandline arguments
            try
            {
                var parser = new Parser(with =>
                {
                    with.AutoHelp = false;
                    with.AutoVersion = false;
                    with.HelpWriter = null;
                });
                var parserresult = parser.ParseArguments<Options>(args);
                parserresult.WithParsed<Options>(o =>
                                   {
                                       //Keep console window open
                                       KeepConsoleWindowOpen = o.Keepopen;

                                       //Show help
                                       if (o.Help)
                                       {
                                           var Helptext = HelpText.AutoBuild<Options>
                                           (parserresult,
                                               h => { return HelpText.DefaultParsingErrorsHandler<Options>(parserresult, h); },
                                               e => { return e; }
                                           );
                                           AnsiConsole.Write(new Rule("[turquoise2]ResizeX Help[/]").Centered());
                                           AnsiConsole.WriteLine("");
                                           Helptext.Copyright = "(c) " + DateTime.Now.ToString("yyyy") + " Alexander Feuster";
                                           Helptext.AddPostOptionsLine("Remark: depending on the picture size and memory usage not all scalers can be aplicated. Especially big scalers on big pictures may fail.");
                                           AnsiConsole.Markup("[white]" + Helptext + "[/]");
                                           AnsiConsole.WriteLine("");
                                           PressAnyKey(true);
                                       }

                                       //Show version
                                       if (o.Version)
                                       {
                                           AnsiConsole.Write(new Rule("[turquoise2]Versions[/]").Centered());
                                           AnsiConsole.WriteLine("");
                                           Table _Table = new Table();
                                           _Table.AddColumns
                                           (
                                               new TableColumn("ResizeX").Centered().Width(30),
                                               new TableColumn("Feuster.Imaging.Resizing.Lib").Centered().Width(30)
                                           );
                                           _Table.AddRow(new Text[]
                                           {
                                               new Text($"{Version}-{GitVersion}", new Style(Spectre.Console.Color.Teal)).Centered(),
                                               new Text(Scaler.Version, new Style(Spectre.Console.Color.Teal)).Centered()
                                           }).LeftAligned();
                                           AnsiConsole.WriteLine("");
                                           _Table.Centered();
                                           AnsiConsole.Write(_Table);
                                           PressAnyKey(true);
                                       }

                                       //Parse show scaler description list
                                       if (o.DescriptionList)
                                       {
                                           AnsiConsole.Write(new Rule("[turquoise2]Descriptions of supported fixed scaler algorithms[/]").Centered());
                                           AnsiConsole.WriteLine("");
                                           int _Count = Enum.GetValues(typeof(Scaler.Scalers)).Length;
                                           var _Table = new Table();
                                           _Table.AddColumns(new TableColumn("Algorithm").LeftAligned(), new TableColumn("Description").LeftAligned());

                                           for (int i = 0; i < _Count; i++)
                                           {
                                               _Table.AddRow(new Text[]
                                               {
                                               new Text(Enum.GetName(typeof(Scaler.Scalers), (Scaler.Scalers)i).PadRight(16), new Style(Spectre.Console.Color.Teal)).RightJustified(),
                                               new Text(Scaler.ScalerDescriptions[i], new Style(Spectre.Console.Color.Teal)).RightJustified()
                                               }).LeftAligned();
                                               _Table.AddEmptyRow();
                                           }
                                           _Table.Expand();
                                           AnsiConsole.Write(_Table);
                                           AnsiConsole.WriteLine("");
                                           PressAnyKey(true);
                                       }

                                       //Parse show scaler list
                                       if (o.List)
                                       {
                                           AnsiConsole.Write(new Rule("[turquoise2]List of supported fixed scaler algorithms[/]").Centered());
                                           AnsiConsole.WriteLine("");

                                           int _Count = Enum.GetValues(typeof(Scaler.ScalerGroups)).Length;
                                           int _Count2 = Enum.GetValues(typeof(Scaler.Scalers)).Length / _Count;
                                           var _Table = new Table();
                                           var _Table2 = new Table();

                                           //Scaler description
                                           _Table.AddColumns(new TableColumn("Algorithm").LeftAligned(), new TableColumn("Description").LeftAligned());
                                           for (int i = 0; i < _Count; i++)
                                           {
                                               _Table.AddRow("[teal]" + Enum.GetName(typeof(Scaler.ScalerGroups), (Scaler.ScalerGroups)i).PadRight(16) + "[/]", "[teal]" + Scaler.ScalerGroupsDescriptions[i] + "[/]");
                                           }

                                           //Table header
                                           for (int i = 0; i < _Count; i++)
                                           {
                                               _Table2.AddColumn(new TableColumn(Enum.GetName(typeof(Scaler.ScalerGroups), (Scaler.ScalerGroups)i)).Centered());
                                           }

                                           //Table rows
                                           for (int i = 0; i < _Count2; i++)
                                           {
                                               List<Spectre.Console.Rendering.IRenderable> _Items = new List<Spectre.Console.Rendering.IRenderable>();
                                               _Items.Clear();
                                               for (int i2 = 0; i2 < _Count; i2++)
                                               {
                                                   _Items.Add(new Text(Enum.GetName(typeof(Scaler.Scalers), (Scaler.Scalers)((i2 * _Count2) + i)), new Style(Spectre.Console.Color.Teal)));
                                               }
                                               _Table2.AddRow(_Items);
                                           }
                                           _Table.Expand();
                                           _Table2.Expand();
                                           AnsiConsole.Write(_Table);
                                           AnsiConsole.Write(_Table2);
                                           AnsiConsole.WriteLine("");
                                           PressAnyKey(true);
                                       }

                                       //Start parsing the resizing parameters
                                       AnsiConsole.Write(new Rule("[turquoise2]Parsing parameters[/]").Centered());
                                       AnsiConsole.WriteLine("");

                                       //Performance test
                                       if (o.Performance)
                                       {
                                           PerformanceTest = true;
                                           PerformanceTestNoOutputFiles = false;
                                       }
                                       if (o.PerformanceNoOutput)
                                       {
                                           PerformanceTest = true;
                                           PerformanceTestNoOutputFiles = true;
                                       }

                                       //Variable resizing
                                       Resize = o.Resize;
                                       FastResize = o.FastResize;
                                       SharpResize = o.SharpResize;
                                       if (FastResize || Resize || SharpResize)
                                       {
                                           if (FastResize && !Resize && !SharpResize)
                                           {
                                               AnsiConsole.MarkupLine("[green]Info: FastResize mode selected, Scaler settings will be ignored[/]");
                                           }
                                           else if (!FastResize && Resize && !SharpResize)
                                           {
                                               AnsiConsole.MarkupLine("[green]Info: .net default Resize mode selected, Scaler settings will be ignored[/]");
                                           }
                                           else if (!FastResize && !Resize && SharpResize)
                                           {
                                               AnsiConsole.MarkupLine("[green]Info: SharpResize mode selected, Scaler settings will be ignored[/]");
                                           }
                                           else
                                           {
                                               AnsiConsole.MarkupLine("[green]Info: several resizer methods specified, using FastResize as default mode selected, Scaler settings will be ignored[/]");
                                               Resize = false;
                                               FastResize = true;
                                               SharpResize = false;
                                           }

                                           if (PerformanceTest)
                                           {
                                               AnsiConsole.MarkupLine("[green]Info: performance test disabled[/]");
                                               PerformanceTest = false;
                                           }
                                           if (o.Width == 0)
                                           {
                                               AnsiConsole.MarkupLine("[red]Info: width is not allowed to be 0[/]");
                                               PressAnyKey(true);
                                           }
                                           if (o.Height == 0)
                                           {
                                               AnsiConsole.MarkupLine("[red]Info: heigth is not allowed to be 0[/]");
                                               PressAnyKey(true);
                                           }
                                           NewWidth = o.Width;
                                           NewHeight = o.Height;
                                           AnsiConsole.MarkupLine($"[green]Info: new image size is set to {NewWidth}x{NewHeight}[/]");
                                       }

                                       //Parse input file
                                       if (o.Input == string.Empty)
                                       {
                                           if (args.Length < 1)
                                           {
                                               AnsiConsole.MarkupLine("[red]Error: no inputfile[/]");
                                               PressAnyKey(true);
                                           }
                                           else
                                           {
                                               if (File.Exists(args[0]))
                                               {
                                                   InputFile = args[0];
                                                   AnsiConsole.MarkupLine("[green]Info: inputfile " + ShortString(InputFile, 60) + "[/]");
                                               }
                                               else
                                               {
                                                   AnsiConsole.MarkupLine("[red]Error: no inputfile[/]");
                                                   PressAnyKey(true);
                                               }
                                           }
                                       }
                                       else
                                       {
                                           if (File.Exists(o.Input))
                                           {
                                               InputFile = o.Input;
                                               AnsiConsole.MarkupLine("[green]Info: inputfile " + ShortString(InputFile, 60) + "[/]");
                                           }
                                           else
                                           {
                                               AnsiConsole.MarkupLine("[red]Error: inputfile does not exist[/]");
                                               PressAnyKey(true);
                                           }
                                       }

                                       //Parse output file
                                       if (o.Output == string.Empty && !PerformanceTestNoOutputFiles)
                                       {
                                           if (InputFile != string.Empty)
                                           {
                                               OutputFile = @Path.GetDirectoryName(InputFile) + @"\" + Path.GetFileNameWithoutExtension(InputFile) + "_resized" + Path.GetExtension(InputFile);
                                               AnsiConsole.MarkupLine("[green]Info: outputfile " + ShortString(OutputFile, 60) + "[/]");
                                           }
                                           else
                                           {
                                               AnsiConsole.MarkupLine("[red]Error: no outputfile[/]");
                                               PressAnyKey(true);
                                           }
                                       }
                                       else if (!PerformanceTestNoOutputFiles)
                                       {
                                           OutputFile = o.Output;
                                           AnsiConsole.MarkupLine("[green]Info: outputfile " + ShortString(OutputFile, 60) + "[/]");
                                       }
                                       if (OutputFile == InputFile)
                                       {
                                           AnsiConsole.MarkupLine("[red]Error: outputfile and inputfile can not be the same file[/]");
                                           PressAnyKey(true);
                                       }
                                       if (!Directory.Exists(Path.GetDirectoryName(OutputFile)) && !PerformanceTestNoOutputFiles)
                                           Directory.CreateDirectory(Path.GetDirectoryName(OutputFile));

                                       //Generate ImageFormat from output file extension
                                       if (!PerformanceTestNoOutputFiles)
                                       {
                                           OutputImageFormat = ImageFormatFromExt(OutputFile);
                                           AnsiConsole.MarkupLine("[green]Info: output imageformat " + OutputImageFormat.ToString() + "[/]");
                                       }

                                       //Parse scaler algorithm
                                       if (PerformanceTest)
                                       {
                                           AnsiConsole.MarkupLine("[green]Info: performance test enabled and ignoring scaler algorithm setting[/]");
                                           if (PerformanceTestNoOutputFiles)
                                               AnsiConsole.MarkupLine("[green]Info: performance test will not create output files[/]");
                                       }
                                       else
                                       {
                                           if (o.Scaler == string.Empty && !FastResize && !SharpResize && !Resize)
                                           {
                                               ScalerAlgorithm = "Scale2x";
                                               AnsiConsole.MarkupLine("[green]Info: setting Scale2x as default scaler algorithm[/]");
                                           }
                                           else if (!FastResize && !SharpResize && !Resize)
                                           {
                                               ScalerAlgorithm = o.Scaler;
                                               if (ScalerNameToScalerEnum(ScalerAlgorithm) == -1)
                                               {
                                                   AnsiConsole.MarkupLine("[red]Info: invalid scaler " + ScalerAlgorithm + " setting therefore Scale2x is set as default scaler algorithm[/]");
                                                   ScalerAlgorithm = "Scale2x";
                                               }
                                               else
                                               {
                                                   ScalerAlgorithm = Enum.GetName(typeof(Scaler.Scalers), ScalerNameToScalerEnum(ScalerAlgorithm));
                                                   AnsiConsole.MarkupLine("[green]Info: using scaler " + ScalerAlgorithm + "[/]");
                                               }
                                           }
                                       }
                                   });
            }
            catch(Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Exception: {ex.Message}[/]");
                PressAnyKey();
            }
            //Start resizing the image
            AnsiConsole.WriteLine("");
            AnsiConsole.Write(new Rule("[turquoise2]Start resizing[/]").Centered());
            AnsiConsole.WriteLine("");
            string Result = string.Empty;
            try
            {
                AnsiConsole.MarkupLine("[skyblue1]Execute: loading inputfile " + ShortString(InputFile, 60) + "[/]");
                Bitmap Input = new Bitmap(Image.FromFile(InputFile));
                Bitmap? Output;
                DateTime start;
                TimeSpan duration;
                if (!PerformanceTest)
                {
                    if (FastResize)
                    {
                        AnsiConsole.MarkupLine($"[skyblue1]Execute: FastResize with new size {NewWidth}x{NewHeight}[/]");
                        start = DateTime.Now;
                        Output = Scaler.FastResize(Input, NewWidth, NewHeight);
                        duration = DateTime.Now - start;
                        AnsiConsole.MarkupLine("[skyblue1]Execute: saving outputfile " + ShortString(OutputFile, 60) + "[/]");
                        Output?.Save(OutputFile, OutputImageFormat);
                        if (Output == null)
                            Result = "error";
                        else
                            Result = Math.Round(duration.TotalMilliseconds).ToString() + "ms (without loading/saving)";
                    }
                    else if (SharpResize)
                    {
                        AnsiConsole.MarkupLine($"[skyblue1]Execute: SharpResize with new size {NewWidth}x{NewHeight}[/]");
                        start = DateTime.Now;
                        Output = Scaler.SharpResize(Input, NewWidth, NewHeight);
                        duration = DateTime.Now - start;
                        AnsiConsole.MarkupLine("[skyblue1]Execute: saving outputfile " + ShortString(OutputFile, 60) + "[/]");
                        Output?.Save(OutputFile, OutputImageFormat);
                        if (Output == null)
                            Result = "error";
                        else
                            Result = Math.Round(duration.TotalMilliseconds).ToString() + "ms (without loading/saving)";
                    }
                    else if (Resize)
                    {
                        AnsiConsole.MarkupLine($"[skyblue1]Execute: default .net Resize with new size {NewWidth}x{NewHeight}[/]");
                        start = DateTime.Now;
                        Output = Scaler.Resize(Input, NewWidth, NewHeight);
                        duration = DateTime.Now - start;
                        AnsiConsole.MarkupLine("[skyblue1]Execute: saving outputfile " + ShortString(OutputFile, 60) + "[/]");
                        Output?.Save(OutputFile, OutputImageFormat);
                        if (Output == null)
                            Result = "error";
                        else
                            Result = Math.Round(duration.TotalMilliseconds).ToString() + "ms (without loading/saving)";
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[skyblue1]Execute: resizing with " + ScalerAlgorithm + "[/]");
                        start = DateTime.Now;
                        Output = Scaler.Scale(Input, (Scaler.Scalers)ScalerNameToScalerEnum(ScalerAlgorithm), false);
                        duration = DateTime.Now - start;
                        AnsiConsole.MarkupLine("[skyblue1]Execute: saving outputfile " + ShortString(OutputFile, 60) + "[/]");
                        Output?.Save(OutputFile, OutputImageFormat);
                        if (Output == null)
                            Result = "error";
                        else
                            Result = Math.Round(duration.TotalMilliseconds).ToString() + "ms (without loading/saving)";
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[skyblue1]Execute: resizing with all available scalers (performance test)[/]");
                    int _Count = Enum.GetValues(typeof(Scaler.ScalerGroups)).Length;
                    int _Count2 = Enum.GetValues(typeof(Scaler.Scalers)).Length / _Count;

                    for (int group = 0; group < _Count2; group++)
                    {
                        var _Table = new Table();
                        IList<string> DurationInMS = new List<string>();
                        for (int i = group; i < Enum.GetValues(typeof(Scaler.Scalers)).Length; i += _Count2)
                        {
                            _Table.AddColumn(new TableColumn(Enum.GetName(typeof(Scaler.Scalers), i)).Centered().Width(Math.Abs(Console.WindowWidth / _Count)));
                            start = DateTime.Now;
                            Output = Scaler.Scale(Input, (Scaler.Scalers)i, true);
                            duration = DateTime.Now - start;
                            if (!PerformanceTestNoOutputFiles)
                            {
                                string NewOutputfile = @Path.GetDirectoryName(OutputFile) + @"\" + Path.GetFileNameWithoutExtension(OutputFile) + "_" + Enum.GetName(typeof(Scaler.Scalers), (Scaler.Scalers)i) + Path.GetExtension(OutputFile);
                                Output?.Save(NewOutputfile, OutputImageFormat);
                            }
                            if (Output == null)
                                DurationInMS.Add("error");
                            else
                                DurationInMS.Add(Math.Round(duration.TotalMilliseconds).ToString() + "ms");
                        }
                        List<Spectre.Console.Rendering.IRenderable> _Items = new List<Spectre.Console.Rendering.IRenderable>();
                        _Items.Clear();
                        for (int i = 0; i < DurationInMS.Count; i++)
                        {
                            _Items.Add(new Text(DurationInMS[i], new Style(Spectre.Console.Color.Teal)));
                        }
                        _Table.Rows.Add(_Items);
                        AnsiConsole.Write(_Table);
                    }
                    Result = "Performance mode finished";
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    AnsiConsole.MarkupLine("[red]Execute: error " + ex.InnerException.Message + "[/]");
                else
                    AnsiConsole.MarkupLine("[red]Execute: error " + ex.Message + "[/]");
                Result = "error " + ex.Message;
            }

            //Resizing finished
            AnsiConsole.WriteLine("");
            AnsiConsole.Write(new Rule("[turquoise2]Stop resizing after " + Result + "[/]").Centered());
            AnsiConsole.WriteLine("");
            PressAnyKey(true);
        }
    }
}