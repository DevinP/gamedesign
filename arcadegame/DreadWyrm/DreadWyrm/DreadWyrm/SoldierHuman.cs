using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreadWyrm
{
    class SoldierHuman : Prey
    {
        float elapsedTimeVel;                 //For counting for changing the animal's velocity
        const float TIMETOCHANGEVEL = 11;     //When to change velocity (in seconds)

        float elapsedTimeShoot = 0.45f;       //For counting for when another bullet can be shot
        const float TIMETOSHOOT = 0.9f;       //How long between shots (in seconds)

        const int BULLETDAMAGE = 10;           //The damage done by this unit
        const int BULLETSPEED = 5;

        bool isShooting = false;
        bool firstShot = true;

        int shootingRightFacingY;
        int shootingLeftFacingY;

        Texture2D bulletTexture;

        public SoldierHuman(int initialX, int initialY, Texture2D texture, int frames, int spriteHeight, int spriteWidth, int preyHeight,
                    float boundingRadius, Wyrm predator, int meat, int facingY, int shootingLeftY, int shootingRightY, Texture2D bulletTex)
            : base(initialX, initialY, texture, frames, spriteHeight, spriteWidth, preyHeight, boundingRadius, predator, meat, facingY)
        {
            asSprite = new AnimatedSprite(texture, 0, 0, spriteWidth, spriteHeight, frames);
            asSprite.IsAnimating = true;

            otherFacing = facingY;

            if (Game1.m_random.NextDouble() < 0.5)
                xVel = -1;
            else
                xVel = 1;

            shootingRightFacingY = shootingRightY;
            shootingLeftFacingY = shootingLeftY;

            bulletTexture = bulletTex;
        }

        public override void Update(GameTime gametime)
        {

            if (!theWyrm.b_wyrmGrounded)
            {
                //Oh no it's the wyrm!
                isShooting = true;

                if (firstShot)
                    asSprite.Frame = 0;

                firstShot = false;


                //Is it to the left of us or the right of us?
                if (xPos > theWyrm.l_segments[0].X)
                {
                    //It's to the left of us
                    xVel = -1; //Face it so we can shoot at it
                }
                else
                {
                    //It's to the right of us
                    xVel = 1; //Face it so we can shoot at it
                }

                elapsedTimeVel = 0;
            }
            else
            {
                isShooting = false;
                firstShot = true;
                elapsedTimeShoot = 0.45f;

                //Change direction occassionally
                if (elapsedTimeVel > TIMETOCHANGEVEL)
                {
                    xVel = -1 * xVel;
                    elapsedTimeVel = 0;
                }
            }

            if (!isShooting)
            {
                elapsedTimeVel += (float)gametime.ElapsedGameTime.TotalSeconds;

                //We only move if we aren't shooting
                xPos = (int)(xPos + xVel);

                //Use the walking animations when not shooting
                if (xVel > 0)
                    asSprite.frameOffsetY = otherFacing;
                else
                    asSprite.frameOffsetY = 0;
            }
            else
            {
                elapsedTimeShoot += (float)gametime.ElapsedGameTime.TotalSeconds;

                //Switch to the shooting animations
                if (xVel > 0)
                    asSprite.frameOffsetY = shootingRightFacingY;
                else
                    asSprite.frameOffsetY = shootingLeftFacingY;

                if (asSprite.Frame == 3 && elapsedTimeShoot > TIMETOSHOOT)
                {
                    //Spawn a bullet on the shooting animation frame
                    //Make a unit vector to determine the velocity we need to shoot the bullet

                    //First, find the difference in x and y positions of the wyrm head and soldier
                    float diffX = theWyrm.l_segments[0].X - xPos;
                    float diffY = theWyrm.l_segments[0].Y - yPos;

                    //Create the unit vector
                    float unitMultiplier = (float)(1 / (Math.Sqrt(Math.Pow(diffX, 2) + Math.Pow(diffY, 2))));
                    Vector2 velVec = new Vector2(diffX * unitMultiplier, diffY * unitMultiplier);

                    //Create a bullet using that unit vector
                    Game1.bullets.Add(new Bullet(xPos, yPos, velVec * BULLETSPEED, bulletTexture, 4, 10, 8, 9, BULLETDAMAGE));

                    elapsedTimeShoot = 0;
                }
            }


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

        public override void getEaten(Player thePlayer)
        {
            thePlayer.Meat += meatReward;

            Game1.chomp.Play();
        }

    }
}
