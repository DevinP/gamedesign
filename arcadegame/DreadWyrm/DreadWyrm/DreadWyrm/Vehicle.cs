using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreadWyrm
{
    class Vehicle : Prey
    {
        const int BULLETDAMAGE = 25;
        const int BULLETSPEED = 10;

        const float TIMETOCHANGEVEL = 11;
        const float SHOOTDELAY = 1;

        Texture2D bulletTexture;

        bool isShooting = false;
        bool mayShoot = false;

        float elapsedTimeVel = 0;
        float elapsedTimeShoot = 0;

        public Vehicle(int initialX, int initialY, Texture2D texture, int frames, int spriteHeight, int spriteWidth, int preyHeight,
                    float boundingRadius, Wyrm predator, int meat, int facingY, Texture2D bulletTex)
            : base(initialX, initialY, texture, frames, spriteHeight, spriteWidth, preyHeight, boundingRadius, predator, meat, facingY)
        {
            asSprite = new AnimatedSprite(texture, 0, 0, 145, 50, 0);
            asSprite.IsAnimating = false;

            otherFacing = facingY;

            if (Game1.m_random.NextDouble() < 0.5)
                xVel = -1;
            else
                xVel = 1;

            bulletTexture = bulletTex;
        }

        public override void Update(GameTime gametime)
        {
            elapsedTimeShoot += (float)gametime.ElapsedGameTime.TotalSeconds;

            //Use the walking animations when not shooting
            if (xVel > 0)
                asSprite.frameOffsetY = otherFacing;
            else
                asSprite.frameOffsetY = 0;

            if (!theWyrm.b_wyrmGrounded && mayShoot)
            {
                //Oh no it's the wyrm!
                

                //Is it to the left of us or the right of us?
                if (xPos > theWyrm.l_segments[0].X && xVel < 0)
                {
                    //It's to the left of us and we are facing it
                    isShooting = true;
                }
                else if (xPos <= theWyrm.l_segments[0].X && xVel > 0)
                {
                    //It's to the right of us and we are facing it
                    isShooting = true;
                }

                elapsedTimeVel = 0;
            }
            else
            {
                isShooting = false;

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
            }
            else
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
                Game1.bullets[Game1.bullets.Count - 1].asprite.IsAnimating = false;

                isShooting = false;
                mayShoot = false;
                elapsedTimeShoot = 0;
            }

            if (elapsedTimeShoot >= SHOOTDELAY)
                mayShoot = true;

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
            Explosion theExplosion = new Explosion(xPos, yPos, Game1.explosionTexture, false);
            Game1.explosions.Add(theExplosion);

            Game1.explosion.Play();
        }
    }
}
