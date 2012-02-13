using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DreadWyrm
{
    class Engineer : Prey
    {
        const float TIMETOCHANGE = 11;     //When to change velocity (in seconds)
        const float MAXTIMETOMINE = 8;     //The maximum amount of time that passes before laying a mine
        const float MINTIMETOMINE = 3;     //The minimum amount of time that passes before laying a mine

        float mineLayTarget = 0;

        bool framesFast = false;
        float elapsedTimeVel = 0;
        float elapsedTimeMine = 0;

        bool isLayingMine = false;

        int mineRightFacingY;
        int mineLeftFacingY;
        int numMines = 2;

        Texture2D mineTexture;

        public Engineer(int initialX, int initialY, Texture2D texture, int frames, int spriteHeight, int spriteWidth, int preyHeight,
                    float boundingRadius, Wyrm predator, int meat, int facingY, int mineLeftY, int mineRightY, Texture2D mineTex)
            : base(initialX, initialY, texture, frames, spriteHeight, spriteWidth, preyHeight, boundingRadius, predator, meat, facingY)
        {
            asSprite = new AnimatedSprite(texture, 0, 0, spriteWidth, spriteHeight, frames);
            asSprite.IsAnimating = true;

            otherFacing = facingY;

            if (Game1.m_random.NextDouble() < 0.5)
                xVel = -1;
            else
                xVel = 1;

            mineRightFacingY = mineRightY;
            mineLeftFacingY = mineLeftY;

            mineTexture = mineTex;

            mineLayTarget = MathHelper.Clamp((float)(Game1.m_random.NextDouble() * MAXTIMETOMINE), MINTIMETOMINE, MAXTIMETOMINE);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gametime)
        {
            elapsedTimeMine += (float)gametime.ElapsedGameTime.TotalSeconds;

            if (!theWyrm.b_wyrmGrounded)
            {
                //Oh no it's the wyrm!
                if (xPos > theWyrm.l_segments[0].X)
                {
                    xVel = 2; //It's to the left; run to the right!
                }
                else
                {
                    xVel = -2; //It's to the right; run to the left!
                }

                framesFast = true;
                elapsedTimeVel = 0;
            }
            else
            {
                //We don't see the wyrm, so don't panic
                framesFast = false;

                //If we were running very fast, slow down to normal speed
                if (xVel < -1 || xVel > 1)
                    xVel = xVel * 0.5f;

                //Change direction every once in a while
                if (elapsedTimeVel > TIMETOCHANGE)
                {
                    xVel = -1 * xVel;
                    elapsedTimeVel = 0;
                }
            }

            if (elapsedTimeMine > mineLayTarget && numMines > 0)
            {
                //Lay a mine at the current position
                isLayingMine = true;
            }

            if (!isLayingMine)
            {
                elapsedTimeVel += (float)gametime.ElapsedGameTime.TotalSeconds;

                //We only move if we aren't laying a mine
                xPos = (int)(xPos + xVel);

                //Use the walking animations when not laying a mine
                if (xVel > 0)
                    asSprite.frameOffsetY = otherFacing;
                else
                    asSprite.frameOffsetY = 0;

                //Animate the engineer running faster if it is moving faster
                if (!framesFast)
                    asSprite.FrameLength = 0.2f;
                else
                    asSprite.FrameLength = 0.1f;
            }
            else
            {
                //Switch to the mine laying animations
                if (xVel > 0)
                    asSprite.frameOffsetY = mineRightFacingY;
                else
                    asSprite.frameOffsetY = mineLeftFacingY;

                if (asSprite.Frame == 5)
                {
                    
                    //Spawn a mine trap at this position
                    Game1.prey.Add(new Trap(xPos, yPos, mineTexture, 3, 9, 16, 7, 3, theWyrm, 20, Game1.explosionTexture));

                    numMines--;

                    elapsedTimeMine = 0;
                    isLayingMine = false;

                }
            }

            //Keep the engineer on screen
            if (xPos < 50)
            {
                xPos = 50;
                xVel = -1 * xVel;
            }
            else if (xPos > Background.SCREENWIDTH - 50)
            {
                xPos = Background.SCREENWIDTH - 50;
                xVel = -1 * xVel;
            }

            //Keep the engineer on the surface of the ground
            recalcPositions();
            footToGround();


            asSprite.Update(gametime);
        }

        public override void Draw(SpriteBatch sb)
        {
            asSprite.Draw(sb, (int)xPos - spritewidth / 2, (int)yPos - spriteheight / 2, false);
        }

        public override void getEaten(Player thePlayer)
        {
            thePlayer.Meat += meatReward;

            Game1.chomp.Play();
        }
    }
}
