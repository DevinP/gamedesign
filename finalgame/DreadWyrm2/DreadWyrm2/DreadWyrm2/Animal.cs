using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DreadWyrm2
{
    public class Animal : Prey
    {

        float elapsedTime;                 //For counting for changing the animal's velocity
        const float TIMETOCHANGE = 11;     //When to change velocity (in seconds)
        
        bool framesFast;

        int meatReward;
        int otherFacing;

        /// <summary>
        /// An animal that the Wyrm can eat
        /// </summary>
        /// <param name="initialX">The initial X position of the animal</param>
        /// <param name="initialY">The initial Y position of the animal</param>
        /// <param name="texture">The texture this animal uses</param>
        /// <param name="frames">The number of frames in the animal's animation</param>
        /// <param name="spriteHeight">The pixel height of the animal's sprite</param>
        /// <param name="spriteWidth">The pixel width of the animal's sprite</param>
        /// <param name="prHeight">The height of the animal in-game</param>
        /// <param name="boundingRadius">The bounding radius that the wyrm can eat the animal with</param>
        /// <param name="predator">The wyrm that the animal reacts to</param>
        /// <param name="meat">The amount of meat that this animal is worth</param>
        /// <param name="facingY">The Y position on the spritesheet which mirrors the sprite's facing</param>
        public Animal(int initialX, int initialY, Texture2D texture, int frames, int spHeight, int spWidth, int prHeight,
                      float bndRadius, Wyrm predator, int meat, int facingY)
            : base(initialX, initialY, predator)
        {
            spriteWidth = spWidth;
            spriteHeight = spHeight;
            preyheight = prHeight;
            boundingRadius = bndRadius;

            meatReward = meat;
            otherFacing = facingY;

            asSprite = new AnimatedSprite(texture, 0, 0, spriteWidth, spriteHeight, frames);
            asSprite.IsAnimating = true;

            otherFacing = facingY;

            if (Game1.m_random.NextDouble() < 0.5)
                xVel = -1;
            else
                xVel = 1;
        }

        public override void Update(GameTime gametime)
        {
            elapsedTime += (float)gametime.ElapsedGameTime.TotalSeconds;

            if (!theWyrm.b_wyrmGrounded)
            {
                //Oh no it's the wyrm!

                //Is it to the left of us or the right of us?
                if (xPos > theWyrm.l_segments[0].X)
                {
                    xVel = 2; //It's to the left; run to the right!
                    framesFast = true;
                }
                else
                {
                    xVel = -2; //It's to the right; run to the left!
                    framesFast = true;
                }

                elapsedTime = 0;
            }
            else
            {
                //We don't see the wyrm, so don't panic
                framesFast = false;

                //If we were running very fast, slow down to normal speed
                if (xVel < -1 || xVel > 1)
                    xVel = xVel * 0.5f;

                //Change direction every once in a while
                if (elapsedTime > TIMETOCHANGE)
                {
                    xVel = -1 * xVel;
                    elapsedTime = 0;
                }
            }

            //Animated the running faster if it is moving faster
            if (!framesFast)
                asSprite.FrameLength = 0.2f;
            else
                asSprite.FrameLength = 0.1f;

            xPos = (int)(xPos + xVel);

            //Change the direction the sprite is facing to match the direction of movement
            if (xVel < 0)
                asSprite.frameOffsetY = otherFacing;
            else
                asSprite.frameOffsetY = 0;

            //Keep the animal on screen
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

            //Keep the animal on the surface of the ground
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
            thePlayer.Meat += meatReward;

            chomp.Play();
        }
    }
}
