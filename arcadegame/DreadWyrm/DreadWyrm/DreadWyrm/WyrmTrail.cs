using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreadWyrm
{
    class WyrmTrail
    {

        bool isFilling = false;

        int xPos, yPos;

        byte alpha;

        const int FILLRATE = 20;
        double frScale;

        const int FILLDELAY = 100;
        double fdScale;

        int delayCount;
        public WyrmTrail(int x, int y)
        {
            xPos = x;
            yPos = y;

            alpha = 0;

            frScale = (double) MathHelper.Clamp((float)Game1.m_random.NextDouble(), 0.3f, 1);

            fdScale = (double)MathHelper.Clamp((float)Game1.m_random.NextDouble(), 0.3f, 1);


            delayCount = 0;
        }


        public void Update()
        {
            if (isFilling)
            {
                #region We are filling, start adding to the alpha value each frame


                alpha = (byte)(alpha + FILLRATE*frScale);

                #endregion
            }
            else
            {
                #region We arent filling yet, just wait it out and change the boolean when appropriate

                if (delayCount >= FILLDELAY*fdScale)
                {
                    isFilling = true;
                }
                else
                {
                    delayCount++;
                }
                
                #endregion
            }


        }

        public bool Draw()
        {
            #region Here we copy my data to the pixels array in background



            //Now I return, based on if I am done existing or not
            if (alpha > 200)
            {
                
                Background.pixels[Background.SCREENWIDTH * yPos + xPos].A = 255;
                return true;
            }
            else
            {
                //I'm going to now alter the array inside of background
                Background.pixels[Background.SCREENWIDTH * yPos + xPos].A = alpha;
                return false;
            }

            #endregion
        }


    }
}
