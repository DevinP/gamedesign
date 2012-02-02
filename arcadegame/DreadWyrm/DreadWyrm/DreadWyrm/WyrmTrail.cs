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

        int fillRate;

        int fillDelay;

        int delayCount;

        public WyrmTrail(int x, int y, int fr, int fd)
        {
            xPos = x;
            yPos = y;

            alpha = 0;

            fillRate = fr;

            fillDelay = fd;
            delayCount = 0;
        }


        public void Update()
        {
            if (isFilling)
            {
                #region We are filling, start adding to the alpha value each frame


                alpha = (byte)(alpha + fillRate);

                #endregion
            }
            else
            {
                #region We arent filling yet, just wait it out and change the boolean when appropriate

                if (delayCount >= fillDelay)
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

        public void Draw()
        {
            #region Here we copy my data to the pixels array in background

            //I'm going to now alter the array inside of background
            Background.pixels[Background.SCREENWIDTH * yPos + xPos].A = alpha;

            #endregion
        }


    }
}
