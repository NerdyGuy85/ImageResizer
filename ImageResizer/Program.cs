using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net;

namespace ImageResizer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Image Resizing Tool");
            Console.WriteLine("Developed by: Kevin A. Cox");
            Console.WriteLine("v1.0.0.5");
            Console.WriteLine();
            Console.WriteLine();
            BeginUserExperience();
        }

        public static void BeginUserExperience()
        {
            Console.Clear();
            var selection = makeSelection();

            if (selection.Trim() == "1")
            {
                singleImageResize();
            }
            else if (selection.Trim() == "2")
            {
                batchImageResize();
            }
            else if (selection.Trim() == "3")
            {
                urlImageResize();
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("An invalid selection was made.");
                Console.ReadLine();
            }
        }
        
        public static string makeSelection()
        {
            Console.WriteLine("Please select which reimaging process you would like to use:");
            Console.WriteLine();
            Console.WriteLine("1. Single image resize");
            Console.WriteLine("2. Folder contents resize");
            Console.WriteLine("3. Image from URL");
            Console.WriteLine();
            return Console.ReadLine();
        }

        public static string[] getDimensions()
        {
            Console.WriteLine();
            Console.WriteLine("Would you like to adjust the height (1), width (2), or autoscale to a minimum (3)?");
            Console.WriteLine();
            var dimension = Console.ReadLine();
            var questionText = "";
            if (dimension == "1")
            {
                questionText = "height";
            }
            else if (dimension == "2")
            {
                questionText = "width";
            }
            else
            {
                questionText = "minimum dimension";
            }
            Console.WriteLine();
            Console.WriteLine($"What would you like the new { questionText } to be?");
            Console.WriteLine();
            var value = Console.ReadLine();

            return new string[2] { dimension, value };
        }

        public static void singleImageResize()
        {
            Console.Clear();
            Console.WriteLine("Single Image Resize");
            Console.WriteLine();
            Console.WriteLine("Please enter the file path, including the filename:");
            Console.WriteLine();
            var filePath = Console.ReadLine().Trim();
            var fileExists = File.Exists(filePath);
            if (!fileExists)
            {
                Console.WriteLine("The file you entered can not be found.");
                Console.ReadLine();
                BeginUserExperience();
            }
            else
            {
                var original = Image.FromFile(filePath);
                var filename = filePath.Split('\\').Last();
                var width = 1;
                var height = 1;
                //Console.WriteLine("File found.  Would you like to adjust the height (1), width (2), or autoscale to a minimum (3)?");
                //Console.WriteLine();
                //var dimension = Console.ReadLine();

                //var questionText = "";
                //if (dimension == "1")
                //{
                //    questionText = "height";
                //}
                //else if (dimension == "2")
                //{
                //    questionText = "width";
                //}
                //else
                //{
                //    questionText = "minimum dimension";
                //}
                //Console.WriteLine();
                //Console.WriteLine($"What would you like the new { questionText } to be?");
                //Console.WriteLine();
                //var value = Console.ReadLine();
                var dimensionResults = getDimensions();
                var dimension = dimensionResults[0];
                var value = dimensionResults[1];

                int n;
                var isNumeric = int.TryParse(value, out n);
                if (!isNumeric)
                {
                    Console.WriteLine();
                    Console.WriteLine("An invalid selection was made.");
                    Console.ReadLine();
                    BeginUserExperience();
                }
                else
                {
                    if (dimension == "1")
                    {
                        height = n;
                    }
                    else if (dimension == "2")
                    {
                        width = n;
                    }
                    else if (dimension == "3")
                    {
                        var originalWidth = original.Width;
                        var originalHeight = original.Height;
                        if (originalWidth < originalHeight)
                        {
                            width = n;
                        }
                        else
                        {
                            height = n;
                        }
                    }
                    Image newImage = ScaleImage(original, width, height);
                    SaveImage(newImage, "", filename);
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadLine();
                }


            }
        }

        public static void batchImageResize()
        {
            Console.Clear();
            Console.WriteLine("Please enter the folder path you would like use:");
            Console.WriteLine();
            var folder = Console.ReadLine().Trim();
            Console.WriteLine();
            Console.WriteLine("What is the image extension you'd like to use? (.jpg, .png, .btm)");
            Console.WriteLine();
            var extension = Console.ReadLine().Trim();
            var myFiles = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories)
                .Where(s => extension == Path.GetExtension(s));
            Console.WriteLine();
            Console.WriteLine($"{ myFiles.Count() } files were found.");
            Console.WriteLine();
            //Console.WriteLine("Would you like to adjust the height (1), width (2), or autoscale to a minimum (3)?");
            //Console.WriteLine();
            //var dimension = Console.ReadLine();
            //var questionText = "";
            //if (dimension == "1")
            //{
            //    questionText = "height";
            //}
            //else if (dimension == "2")
            //{
            //    questionText = "width";
            //}
            //else
            //{
            //    questionText = "minimum dimension";
            //}
            //Console.WriteLine();
            //Console.WriteLine($"What would you like the new { questionText } to be?");
            //Console.WriteLine();
            //var value = Console.ReadLine();

            var dimensionResults = getDimensions();
            var dimension = dimensionResults[0];
            var value = dimensionResults[1];

            int n;
            var isNumeric = int.TryParse(value, out n);
            if (!isNumeric)
            {
                Console.WriteLine("An invalid selection was made.");
                Console.ReadLine();
                BeginUserExperience();
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Where would you like to save the new images?");
                Console.WriteLine();
                var savePath = Console.ReadLine().Trim();

                foreach (var file in myFiles)
                {
                    var original = Image.FromFile(file);
                    var filename = file.Split('\\').Last();
                    var width = 1;
                    var height = 1;
                    if (dimension == "1")
                    {
                        height = n;
                    }
                    else if (dimension == "2")
                    {
                        width = n;
                    }
                    else if (dimension == "3")
                    {
                        var originalWidth = original.Width;
                        var originalHeight = original.Height;
                        if (originalWidth < originalHeight)
                        {
                            width = n;
                        }
                        else
                        {
                            height = n;
                        }
                    }

                    Image newImage = ScaleImage(original, width, height);
                    SaveImage(newImage, savePath, filename);
                }

                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
            }
        }

        private static void urlImageResize()
        {
            Console.Clear();
            Console.WriteLine("URL Image Resize");
            Console.WriteLine();
            Console.WriteLine("Please enter the Image's URL (www.this.com/image.jpg):");
            Console.WriteLine();
            var url = "http://" + Console.ReadLine().Trim();
            try
            {
                using (WebClient wc = new WebClient())                
                {
                    byte[] imageData = wc.DownloadData(url);
                    using (MemoryStream ms = new MemoryStream(imageData))
                    {
                        Image original = Image.FromStream(ms);
                        var filename = url.Split('/').Last();
                        var width = 1;
                        var height = 1;
                        var dimensionResults = getDimensions();
                        var dimension = dimensionResults[0];
                        var value = dimensionResults[1];
                        int n;
                        var isNumeric = int.TryParse(value, out n);
                        if (!isNumeric)
                        {
                            Console.WriteLine();
                            Console.WriteLine("An invalid selection was made.");
                            Console.ReadLine();
                            BeginUserExperience();
                        }
                        else
                        {
                            if (dimension == "1")
                            {
                                height = n;
                            }
                            else if (dimension == "2")
                            {
                                width = n;
                            }
                            else if (dimension == "3")
                            {
                                var originalWidth = original.Width;
                                var originalHeight = original.Height;
                                if (originalWidth < originalHeight)
                                {
                                    width = n;
                                }
                                else
                                {
                                    height = n;
                                }
                            }
                            Image newImage = ScaleImage(original, width, height);
                            SaveImage(newImage, "", filename);
                            Console.WriteLine();
                            Console.WriteLine("Press any key to exit.");
                            Console.ReadLine();
                        }
                    }
                }
            } catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("An error occured: " + ex.Message);
                Console.WriteLine();
            }

        }

        public static Image ScaleImage(Image image, int width, int height)
        {
            var originalHeight = image.Height;
            var originalWidth = image.Width;
            var ratio = 0;
            if (height > 1)
            {
                ratio = originalHeight / height;
            }
            else
            {
                ratio = originalWidth / width;
            }
            var newWidth = originalWidth / ratio;
            var newHeight = originalHeight / ratio;

            if (newWidth > originalWidth || newHeight > originalHeight)
            {
                Console.WriteLine($"Image already meets the required formating.  Copying the original image.");
                return image;
            } else
            {
                var newImage = new Bitmap(image, newWidth, newHeight);

                return newImage;
            }
            
        }

        public static void SaveImage(Image image, string save, string fileName)
        {
            if (string.IsNullOrWhiteSpace(save))
            {
                Console.WriteLine();
                Console.WriteLine("Where would you like to save the new image?");
                Console.WriteLine();
                save = Console.ReadLine();
            }
            save = Path.Combine(save, fileName);
            try
            {
                image.Save(save);
                Console.WriteLine();
                Console.WriteLine($"Saved { fileName } successfully.");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("An error occured saving the new image:");
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

        }
    }
}
