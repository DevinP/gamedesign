using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DreadWyrm
{
    public class Animal : Prey
    {

        float elapsedTime;                 //For counting for changing the animal's velocity
        const float TIMETOCHANGE = 8;      //When to change velocity (in seconds)

        int otherFacing;

        bool human;
        bool framesFast;

        public Animal(int initialX, int initialY, Texture2D texture, int frames, int spriteHeight, int spriteWidth, int preyHeight, 
                      float boundingRadius, Wyrm predator, bool isHuman, int meat, int facingY) 
            : base(initialX, initialY, texture, frames, spriteHeight, spriteWidth, preyHeight, boundingRadius, predator, meat)
        {
            asSprite = new AnimatedSprite(preyTexture, 0, 0, spritewidth, spriteheight, animationFrames);
            asSprite.IsAnimating = true;

            human = isHuman;

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
                framesFast = false;

                if (xVel < -1 || xVel > 1)
                    xVel = xVel * 0.5f;

                if (elapsedTime > TIMETOCHANGE)
                {
                    xVel = -1 * xVel;
                    elapsedTime = 0;
                }
            }

            if (!framesFast)
                asSprite.FrameLength = 0.2f;
            else
                asSprite.FrameLength = 0.1f;

            xPos = (int) (xPos + xVel);

            if (xVel < 0)
                asSprite.frameOffsetY = otherFacing;
            else
                asSprite.frameOffsetY = 0;

            if (xPos < 10)
            {
                xPos = 10;
                xVel = -1 * xVel;
            }
            else if (xPos > Background.SCREENWIDTH - 10)
            {
                xPos = Background.SCREENWIDTH - 10;
                xVel = -1 * xVel;
            }

            recalcPositions();
            footToGround();


            asSprite.Update(gametime);

        }

        public override void Draw(SpriteBatch sb)
        {
            asSprite.Draw(sb, (int)xPos - spritewidth / 2, (int)yPos - spriteheight / 2, false);
        }
    }
}
