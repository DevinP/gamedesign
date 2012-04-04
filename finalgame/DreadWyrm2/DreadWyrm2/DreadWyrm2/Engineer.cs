using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DreadWyrm2
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

        int numMines = 2;

        //Things specific to the Engineer unit
        const int NUM_FRAMES = 6;      //The number of frames in this unit's animation
        const int SPRITEHEIGHT = 23;   //The height (in pixels) of this unit's sprite
        const int SPRITEWIDTH = 25;    //The width (in pixels) of this unit's sprite
        const int PREY_HEIGHT = 22;    //The height of this unit for hte purposes of keeping it on the ground
        const int BOUNDING_RADIUS = 7; //The radius of this unit's bounding circle
        const int MEAT_AMOUNT = 80;    //The amount of meat that this unit awards
        const int FACING_Y = 25;       //The Y-position on the sprite sheet where the mirror image of the sprite begins
        const int MINE_LEFT_Y = 51;    //The Y-position on the sprite sheet for the animation of laying a mine to the left
        const int MINE_RIGHT_Y = 76;   //The Y-position on the sprite sheet for the animation of laying a mine to the right

        /// <summary>
        /// An engineer which lays mines to trick the Wyrm
        /// </summary>
        /// <param name="initialX">The initial X location of the Engineer</param>
        /// <param name="initialY">The initial Y location of the Engineer</param>
        /// <param name="predator">The Wyrm which is looking to eat the Engineer</param>
        public Engineer(int initialX, int initialY, Wyrm predator)
            : base(initialX, initialY, predator)
        {
            preyheight = PREY_HEIGHT;         //Assign a value to the height of this prey
            boundingRadius = BOUNDING_RADIUS; //Assign a value to the bounding radius of this prey
            spriteHeight = SPRITEHEIGHT;
            spriteWidth = SPRITEWIDTH;

            asSprite = new AnimatedSprite(preyTextures[MINE_LAYER], 0, 0, SPRITEWIDTH,  SPRITEHEIGHT, NUM_FRAMES);
            asSprite.IsAnimating = true;

            if (Game1.m_random.NextDouble() < 0.5)
                xVel = -1;
            else
                xVel = 1;

            mineLayTarget = MathHelper.Clamp((float)(Game1.m_random.NextDouble() * MAXTIMETOMINE), MINTIMETOMINE, MAXTIMETOMINE);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gametime)
        {
            elapsedTimeMine += (float)gametime.ElapsedGameTime.TotalSeconds;

            if (!theWyrm.b_wyrmGrounded && numMines <= 0)
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
                    asSprite.frameOffsetY = FACING_Y;
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
                    asSprite.frameOffsetY = MINE_RIGHT_Y;
                else
                    asSprite.frameOffsetY = MINE_LEFT_Y;

                if (asSprite.Frame == 5)
                {

                    //Spawn a mine trap at this position
                    //Note that we do not need any information about the Wyrm, so we will just pass in null
                    prey.Add(new Mine(xPos, yPos, null, Game1.explosionTexture));

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
            asSprite.Draw(sb, (int)xPos - spriteWidth / 2, (int)yPos - spriteHeight / 2, false);
        }

        public override void getEaten(WyrmPlayer thePlayer)
        {
            thePlayer.Meat += MEAT_AMOUNT;

            chomp.Play();
        }
    }
}
