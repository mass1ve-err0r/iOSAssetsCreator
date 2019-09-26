/*
 * Author: Saadat Baig
 * Date: 25th Sep 2019
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace iOSAssetsCreator
{
    public partial class MainWindow : Window
    {
        ///////////////////////////////////////////////////////////////////////
        //                             GLOBAL VARS                           //
        ///////////////////////////////////////////////////////////////////////


        // <-- m4gic -->
        string gpath;

        // <-- DATA -->
        // AppIcon specifics
        int[] AIresolutions = { 20, 29, 40, 50, 57, 60, 72, 76 }; // TODO: case 83.5
        // LaunchScreen specifics
        int[,] LSIresolutions = { { 1242, 2688 }, // XS Max
                                { 1125, 2436 }, // XS, X
                                { 828, 1792 }, // XR
                                { 1125, 2208 }, // Plus (6S, 7, 8)
                                { 750, 1334 }, // Normal 6S, 7, 8
                                { 640, 1136 } // SE
        };
        // resolution suffixes
        string[] res_names = { "@2x", "@3x" };
        // iPhones IN-ORDER of LSI generation
        string[] iPhoneModels = { "XSMax", "XS", "X", "XR", "8Plus", "7Plus", "6SPlus", "8", "7", "6S", "SE" };
        // multi-purpose storage3
        List <Bitmap> img_store = new List<Bitmap>();


        ///////////////////////////////////////////////////////////////////////
        //                           MAIN FUNCTIONS                          //
        ///////////////////////////////////////////////////////////////////////
        

        // Entry
        public MainWindow()
        {
            InitializeComponent();
        }


        // DragField for AppIcons
        private void DragFieldAI_Drop(object sender, System.Windows.DragEventArgs e)
        {
            // save dragged images' path | catch user dragging multiple images
            string[] draggedImg = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop, false);
            // strip just the first images' name & save globally
            string fileName = Path.GetFileNameWithoutExtension(draggedImg[0]);
            // prepwork: create folder
            createUniqueFolder(1, fileName);
            // launch image creation
            GenerateImages(draggedImg[0], 1);
            // save images
            SaveImagesFromArray(1);
            // success message | MB_TOPMOST = 0x40000 or use the flag DefaultDesktopOnly
            System.Windows.Forms.MessageBox.Show("Successfully cerated AppIcons!",
                                                "iOSAssetsCreator - SUCCESS",
                                                MessageBoxButtons.OK, 
                                                MessageBoxIcon.Information, 
                                                MessageBoxDefaultButton.Button1, 
                                                (System.Windows.Forms.MessageBoxOptions)0x40000);
            // run  cleanup agent,else we got them overwrites lmao
            img_store.Clear();
            gpath = null;
            // fin.
        }


        // DragField for LaunchScreenImages
        private void DragFieldLS_Drop(object sender, System.Windows.DragEventArgs e)
        {
            string[] draggedImg = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop, false);
            string fileName = Path.GetFileNameWithoutExtension(draggedImg[0]);
            // pass different identifier
            createUniqueFolder(2, fileName);
            GenerateImages(draggedImg[0], 2);
            SaveImagesFromArray(2);
            System.Windows.Forms.MessageBox.Show("Successfully cerated LaunchScreenImages!",
                                                "iOSAssetsCreator - SUCCESS",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Information,
                                                MessageBoxDefaultButton.Button1,
                                                (System.Windows.Forms.MessageBoxOptions)0x40000);
            img_store.Clear();
            gpath = null;
        }


        ///////////////////////////////////////////////////////////////////////
        //                           HELPER FUNCTIONS                        //
        ///////////////////////////////////////////////////////////////////////
        

        // loop through creating images
        public void GenerateImages(string ImagePath, int type)
        {
            // check what we wanna do | 1=AI, 2=LSI
            if (type == 1)
            {
                // loop through creating the icons
                for (int i = 0; i < AIresolutions.Length; i++)
                {
                    // create image in res
                    Bitmap TempImg = new Bitmap(ResizeImage(ImagePath, AIresolutions[i], AIresolutions[i]));
                    // save image
                    img_store.Add(TempImg);
                }
            }
            if (type == 2)
            {
                // we got 6 resolutions to work through
                for (int i = 0; i < 6; i++)
                {
                    // increment the row counter, fix col-numbers 
                    // Example: { {1,1}, {2,2} } => {1,1} = [row1, 0] & [row1, 1], {2,2} = [row2, 0] & [row2, 1]
                    Bitmap TempImg = new Bitmap(ResizeImage(ImagePath, LSIresolutions[i, 0], LSIresolutions[i, 1]));
                    img_store.Add(TempImg);
                }
            }
            // fin.
        }


        // Save images from array | (assuming the public array was used)
        public void SaveImagesFromArray(int type)
        {
            // check what we wanna do | 1=AI, 2=LSI
            if (type == 1)
            {
                // create a basic for-loop
                for (int i = 0; i < img_store.Count; i++)
                {
                    img_store[i].Save(gpath + "\\AppIcon" + AIresolutions[i] + "x" + AIresolutions[i] + ".png", ImageFormat.Png);
                }
                // loop for @2x
                for (int i = 0; i < img_store.Count; i++)
                {
                    img_store[i].Save(gpath + "\\AppIcon" + AIresolutions[i] + "x" + AIresolutions[i] + res_names[0] + ".png", ImageFormat.Png);
                }
                // loop for @3x
                for (int i = 0; i < img_store.Count; i++)
                {
                    img_store[i].Save(gpath + "\\AppIcon" + AIresolutions[i] + "x" + AIresolutions[i] + res_names[1] + ".png", ImageFormat.Png);
                }
            }
            if (type == 2)
            {
                // we know the order of creation, so we CAN just call one-by-one
                // XS Max
                img_store[0].Save(gpath + "\\LaunchScreen_" + iPhoneModels[0] + ".png", ImageFormat.Png);
                // XS & X
                img_store[1].Save(gpath + "\\LaunchScreen_" + iPhoneModels[1] + ".png", ImageFormat.Png);
                img_store[1].Save(gpath + "\\LaunchScreen_" + iPhoneModels[2] + ".png", ImageFormat.Png);
                // XR
                img_store[2].Save(gpath + "\\LaunchScreen_" + iPhoneModels[3] + ".png", ImageFormat.Png);
                // Plus sizes (8, 7, 6S)
                img_store[3].Save(gpath + "\\LaunchScreen_" + iPhoneModels[4] + ".png", ImageFormat.Png);
                img_store[3].Save(gpath + "\\LaunchScreen_" + iPhoneModels[5] + ".png", ImageFormat.Png);
                img_store[3].Save(gpath + "\\LaunchScreen_" + iPhoneModels[6] + ".png", ImageFormat.Png);
                // regular sizes (8, 7, 6S)
                img_store[4].Save(gpath + "\\LaunchScreen_" + iPhoneModels[7] + ".png", ImageFormat.Png);
                img_store[4].Save(gpath + "\\LaunchScreen_" + iPhoneModels[8] + ".png", ImageFormat.Png);
                img_store[4].Save(gpath + "\\LaunchScreen_" + iPhoneModels[9] + ".png", ImageFormat.Png);
                // SE
                img_store[5].Save(gpath + "\\LaunchScreen_" + iPhoneModels[10] + ".png", ImageFormat.Png);
             }
        }


        // create folder for each type & error check
        public void createUniqueFolder(int type, string folderName)
        {
            // determine if AppIcon or LaunchScreen | 1=AppIcon, 2=LaunchScreen
            if (type == 1)
            {
                // create an output Directory according to the passed filename
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\_AppIcons_" + folderName);
                // set it as global var
                gpath = @Directory.GetCurrentDirectory() + "\\_AppIcons_" + folderName;
            }
            else if (type == 2)
            {
                // create an output Directory according to the passed filename
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\_LaunchScreens_" + folderName);
                gpath = @Directory.GetCurrentDirectory() + "\\_LaunchScreens_" + folderName;
            }
            else
            {
                // throw error
                System.Windows.Forms.MessageBox.Show("createUniqueFolder_flag_type3 triggered",
                                                    "iOSAssetsCreator - ERROR",
                                                    MessageBoxButtons.OK, 
                                                    MessageBoxIcon.Exclamation, 
                                                    MessageBoxDefaultButton.Button1, 
                                                    (System.Windows.Forms.MessageBoxOptions)0x40000);

            }
            // fin.
        }


        // actual resize logic - thanks to MSDN & SO for the tidbits
        public Bitmap ResizeImage(string imgPath, int width, int height)
        {
            // get image
            System.Drawing.Image image = System.Drawing.Image.FromFile(imgPath);
            // specify new res & holder
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);
            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (var graphics = Graphics.FromImage(destImage))
            {
                // set image configs
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                // writeback
                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            // don't forget to dispose...
            image.Dispose();
            // and return
            return destImage;
        }
        // the end.
    }
}
