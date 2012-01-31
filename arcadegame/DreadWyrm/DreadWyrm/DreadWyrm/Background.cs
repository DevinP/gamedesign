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
        static int SCREENWIDTH = 1280;
        static int SCREENHEIGHT = 720;

        //background and foreground textures:
        Texture2D background;
        Texture2D foreground;

        bool b_wyrmGrounded;
        
        //Array of pixels to store the pixel values of the foreground texture
        //Unfortunately must be one dimensional due to the way GetData works
        Color[] pixels;


        public bool wyrmGrounded
        {
            get { return b_wyrmGrounded; }
        }

        public Background(Texture2D backtex, Texture2D foretex)
        {
            background = backtex;
            foreground = foretex;
            pixels = new Color[foreground.Width * foreground.Height]; //Create the array of pixels
            foreground.GetData<Color>(pixels);                        //Get the texture data into the pixels
        }

        public void Draw(SpriteBatch sb)
        {
            //draw the background
            sb.Draw(background, new Rectangle(0, 0, SCREENWIDTH, SCREENHEIGHT), Color.White);
            //draw the foreground
            sb.Draw(foreground, new Rectangle(0, 0, SCREENWIDTH, SCREENHEIGHT), Color.White);
        }

        public void Update(int wyrmX, int wyrmY)
        {
            b_wyrmGrounded = checkIsGrounded(wyrmX, wyrmY);
        }

        public bool checkIsGrounded(int x, int y)
        {
            //check to see if the pixel value's Alpha is one
            if (x * y >= 0) // make sure we are in a positive part of the screen
            {
                 return (pixels[y * SCREENWIDTH + x].A > 0);  //this x * SCREENWIDTH + x converts the 2D coordinates to linear
            }
            else
            {
                return false;
            }
        }




    }
}
