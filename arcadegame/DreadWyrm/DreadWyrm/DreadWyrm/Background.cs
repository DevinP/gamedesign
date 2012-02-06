using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreadWyrm
{
    class Background
    {
        public static int SCREENWIDTH = 1280;
        public static int SCREENHEIGHT = 720;

        //background and foreground textures:
        Texture2D background;
        Texture2D foreground;
        //Array of pixels to store the pixel values of the foreground texture
        //Unfortunately must be one dimensional due to the way GetData works
        public static Color[] pixels;

        //public static int[] pixelsLife;

        //Here is a list of trails that worms or anything has made
        //public static List<WyrmTrail> trails = new List<WyrmTrail>();

        //how quickly do trails fill in?
        //const int FILLRATE = 30;

        public Background(Texture2D backtex, Texture2D foretex)
        {
            background = backtex;
            foreground = foretex;
            pixels = new Color[foreground.Width * foreground.Height]; //Create the array of pixels
           // pixelsLife = new int[foreground.Width * foreground.Height];


            foreground.GetData<Color>(pixels);                        //Get the texture data into the pixels

            //lets find which pixels are in the air
            /*for (int i = 0; i < foreground.Width * foreground.Height; i++)
            {
                if (pixels[i].A == 0)
                    pixelsLife[i] = -1;
            }*/

        }

        public void Draw(SpriteBatch sb)
        {
            /*
            int count = trails.Count;
            //Now we draw the wyrm trails
            for (int i = 0; i < trails.Count; i++)
            {
                if (trails[i].Draw())
                {
                    //count--;
                    trails.RemoveAt(i);
                }
            }
            //*/

            /*
            //for each and every pixel
            for (int i = 0; i < foreground.Width * foreground.Height; i++)
            {
                //First check the life of the pixel
                if (pixelsLife[i] == 0)
                {
                    //we add to the alpha
   
                    if (pixels[i].A >= 220)
                    {
                        pixels[i].A = 255;
                    }
                    else pixels[i].A += FILLRATE;
                }
                else
                {
                    //we reduce the life of the pixel, and set the alpha to 0
                    pixels[i].A = 0;
                    if (pixelsLife[i] <= 0)
                    {
                        //do nothing
                    }
                    else
                    pixelsLife[i] -= 1;

                }
            }
            //*/

            
           // foreground.SetData<Color>(pixels);

           // Color debug = pixels[500 * SCREENWIDTH + 500];

            //draw the background
            sb.Draw(background, new Rectangle(0, 0, SCREENWIDTH, SCREENHEIGHT), Color.White);
            //draw the foreground
            sb.Draw(foreground, new Rectangle(0, 0, SCREENWIDTH, SCREENHEIGHT), Color.White);
        }

        public void Update()
        {
           //Update wyrmtrails
           // foreach (WyrmTrail wt in trails)
           //     wt.Update();
        }

        public static bool checkIsGrounded(int x, int y)
        {
            //check to see if the pixel value's Alpha is one
            if (x * y >= 0) // make sure we are in a positive part of the screen
            {
                //pixels[y * SCREENWIDTH + x].A = 0;
                return (pixels[y * SCREENWIDTH + x].A > 0); //this x * SCREENWIDTH + x converts the 2D coordinates to linear

            }
            else
            {
                return false;
            }
        }


       /* public static void createWyrmTrail(int x, int y)
        {
            WyrmTrail wt = new WyrmTrail(x, y);
            trails.Add(wt);
        }*/
       
    }
}
