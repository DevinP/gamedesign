using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DreadWyrm2
{
    class newTank : Prey
    {
        const int BULLETDAMAGE = 50;
        const int BULLETSPEED = 12;

        const float TIMETOCHANGEVEL = 11;
        const float SHOOTDELAY = 1000;        //Number of milliseconds in between shots

        bool isShooting = false;
        bool playShootingAnimation = false;
        bool startedShootingAnimation = false;

        float elapsedTimeVel = 0;
        float elapsedTimeShoot = 900;

        //Spritesheet constants
        const int NUM_FRAMES = 3;
        const int SPRITE_HEIGHT = 60;        //The height of the tank sprite
        const int SPRITE_WIDTH = 75;         //The width the tank sprite
        const int PREY_HEIGHT = 51;          //The height of the tank for grounding purposes
        const float BOUNDING_RADIUS = 20;    //The bounding radius of the tank
        const int MEAT_REWARD = 0;          //The meat given to the Wyrm when the tank is eaten
        const int FACING_LEFT_DOWN = 0;
        const int FACING_LEFT_MIDDLE = 60;
        const int FACING_LEFT_UP = 120;
        const int FACING_RIGHT_DOWN = 300;
        const int FACING_RIGHT_MIDDLE = 240;
        const int FACING_RIGHT_UP = 180;
        const int FACING_LEFT_DOWN_SHOOTING = 360;
        const int FACING_LEFT_MIDDLE_SHOOTING = 420;
        const int FACING_LEFT_UP_SHOOTING = 480;
        const int FACING_RIGHT_DOWN_SHOOTING = 660;
        const int FACING_RIGHT_MIDDLE_SHOOTING = 600;
        const int FACING_RIGHT_UP_SHOOTING = 540;


        public newTank(int initialX, int initialY, Wyrm predator)
            : base(initialX, initialY, predator)
        {
            preyheight = PREY_HEIGHT;
            boundingRadius = BOUNDING_RADIUS;
            spriteHeight = SPRITE_HEIGHT;
            spriteWidth = SPRITE_WIDTH;
            meatReward = MEAT_REWARD;

            asSprite = new AnimatedSprite(preyTextures[NEW_TANK], 0, 0, SPRITE_WIDTH, SPRITE_HEIGHT, NUM_FRAMES);
            asSprite.IsAnimating = true;

            if (Game1.m_random.NextDouble() < 0.5)
                xVel = -1;
            else
                xVel = 1;
        }
        public override void Update(GameTime gametime)
        {
            //Use the regular animations when not shooting
            if (xVel > 0)
                asSprite.frameOffsetY = FACING_RIGHT_DOWN;
            else
                asSprite.frameOffsetY = FACING_LEFT_DOWN;

            if (!theWyrm.b_wyrmGrounded)
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
                elapsedTimeShoot = 900;

                //Change direction occassionally
                if (elapsedTimeVel > TIMETOCHANGEVEL)
                {
                    xVel = -1 * xVel;
                    elapsedTimeVel = 0;
                }
            }

            if (!isShooting)
            {
                //Update the counter to change direction sometimes
                elapsedTimeVel += (float)gametime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                //Accumulate time for shooting
                elapsedTimeShoot += (float)gametime.ElapsedGameTime.Milliseconds;

                //Make a unit vector to determine the velocity we need to shoot the bullet

                //First, find the difference in x and y positions of the wyrm head and tank
                float diffX = theWyrm.l_segments[0].X - xPos;
                float diffY = theWyrm.l_segments[0].Y - yPos;

                Vector2 velVec = new Vector2(diffX, diffY);
                Vector2 unitVector = new Vector2();

                Vector2.Normalize(ref velVec, out unitVector);

                double angleToWyrm = Math.Atan2(-1 * (double)unitVector.Y, (double)unitVector.X);

                angleToWyrm = MathHelper.ToDegrees((float)angleToWyrm);

                int sector = getSector(angleToWyrm);

                bool shootABullet = false;
                float angleToShoot = (float)angleToWyrm;

                Vector2 bulletOriginOffset = new Vector2();

                switch (sector)
                {
                    case -1:
                        shootABullet = false;
                        break;
                    case 0:
                        bulletOriginOffset = new Vector2(0, 0);

                        if (xVel > 0)
                        {
                            shootABullet = true;

                            if (playShootingAnimation)
                            {
                                if (!startedShootingAnimation)
                                {
                                    asSprite.Frame = 0;
                                    startedShootingAnimation = true;
                                }

                                asSprite.frameOffsetY = FACING_RIGHT_DOWN_SHOOTING;

                                if (asSprite.Frame == 2)
                                {
                                    playShootingAnimation = false;
                                }
                            }
                            else
                            {
                                asSprite.frameOffsetY = FACING_RIGHT_DOWN;
                            }
                        }
                        else
                        {
                            asSprite.frameOffsetY = FACING_LEFT_DOWN;
                        }

                        break;
                    case 1:
                        bulletOriginOffset = new Vector2(0, 0);

                        if (xVel > 0)
                        {
                            shootABullet = true;

                            if (playShootingAnimation)
                            {
                                if (!startedShootingAnimation)
                                {
                                    asSprite.Frame = 0;
                                    startedShootingAnimation = true;
                                }

                                asSprite.frameOffsetY = FACING_RIGHT_MIDDLE_SHOOTING;

                                if (asSprite.Frame == 2)
                                {
                                    playShootingAnimation = false;
                                }
                            }
                            else
                            {
                                asSprite.frameOffsetY = FACING_RIGHT_MIDDLE;
                            }
                        }
                        else
                        {
                            asSprite.frameOffsetY = FACING_LEFT_DOWN;
                        }

                        break;
                    case 2:
                        bulletOriginOffset = new Vector2(0, 0);

                        if (xVel > 0)
                        {
                            shootABullet = true;

                            if (playShootingAnimation)
                            {
                                if (!startedShootingAnimation)
                                {
                                    asSprite.Frame = 0;
                                    startedShootingAnimation = true;
                                }

                                asSprite.frameOffsetY = FACING_RIGHT_UP_SHOOTING;

                                if (asSprite.Frame == 2)
                                {
                                    playShootingAnimation = false;
                                }
                            }
                            else
                            {
                                asSprite.frameOffsetY = FACING_RIGHT_UP;
                            }
                        }
                        else
                        {
                            asSprite.frameOffsetY = FACING_LEFT_DOWN;
                        }

                        break;
                    case 3:
                        shootABullet = false;
                        break;
                    case 4:
                        bulletOriginOffset = new Vector2(0, 0);

                        if (xVel < 0)
                        {
                            shootABullet = true;

                            if (playShootingAnimation)
                            {
                                if (!startedShootingAnimation)
                                {
                                    asSprite.Frame = 0;
                                    startedShootingAnimation = true;
                                }

                                asSprite.frameOffsetY = FACING_LEFT_UP_SHOOTING;

                                if (asSprite.Frame == 2)
                                {
                                    playShootingAnimation = false;
                                }
                            }
                            else
                            {
                                asSprite.frameOffsetY = FACING_LEFT_UP;
                            }
                        }
                        else
                        {
                            asSprite.frameOffsetY = FACING_RIGHT_DOWN;
                        }

                        break;
                    case 5:
                        bulletOriginOffset = new Vector2(0, 0);

                        if (xVel < 0)
                        {
                            shootABullet = true;

                            if (playShootingAnimation)
                            {
                                if (!startedShootingAnimation)
                                {
                                    asSprite.Frame = 0;
                                    startedShootingAnimation = true;
                                }

                                asSprite.frameOffsetY = FACING_LEFT_MIDDLE_SHOOTING;

                                if (asSprite.Frame == 2)
                                {
                                    playShootingAnimation = false;
                                }
                            }
                            else
                            {
                                asSprite.frameOffsetY = FACING_LEFT_MIDDLE;
                            }
                        }
                        else
                        {
                            asSprite.frameOffsetY = FACING_RIGHT_DOWN;
                        }

                        break;
                    case 6:
                        bulletOriginOffset = new Vector2(0, 0);

                        if (xVel < 0)
                        {
                            shootABullet = true;

                            if (playShootingAnimation)
                            {
                                if (!startedShootingAnimation)
                                {
                                    asSprite.Frame = 0;
                                    startedShootingAnimation = true;
                                }

                                asSprite.frameOffsetY = FACING_LEFT_DOWN_SHOOTING;

                                if (asSprite.Frame == 2)
                                {
                                    playShootingAnimation = false;
                                }
                            }
                            else
                            {
                                asSprite.frameOffsetY = FACING_LEFT_DOWN;
                            }
                        }
                        else
                        {
                            asSprite.frameOffsetY = FACING_RIGHT_DOWN;
                        }

                        break;
                    default:
                        break;
                }

                if (elapsedTimeShoot >= SHOOTDELAY)
                {
                    if (shootABullet)
                    {

                        velVec = new Vector2((float)Math.Cos((double)MathHelper.ToRadians(angleToShoot)), -1 * (float)Math.Sin((double)MathHelper.ToRadians(angleToShoot)));

                        //Create a bullet using that unit vector
                        Game1.bullets.Add(new Bullet(xPos + (int)bulletOriginOffset.X, yPos + (int)bulletOriginOffset.Y,
                            velVec * BULLETSPEED, Game1.cannonballTexture, 0, 19, 18, 7, BULLETDAMAGE));
                        Game1.bullets[Game1.bullets.Count - 1].asprite.IsAnimating = false;

                        tankShot.Play();

                        playShootingAnimation = true;
                        startedShootingAnimation = false;
                    }

                    elapsedTimeShoot = 0;
                }
            }

            //if (elapsedTimeShoot >= SHOOTDELAY)
             //   mayShoot = true;

            xPos = (int)(xPos + xVel);

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

         /// <summary>
        /// Determines the sector in which the wyrm resides in relation to this turret
        /// //Sectors: -1 is below the y-axis (turret will not fire)
        ///             0 is 0 to 30 degrees
        ///             1 is 30 to 60 degrees
        ///             2 is 60 to 80 degrees
        ///             3 is 80 to 100 degrees (this is a dead zone in which the turret does not fire)
        ///             4 is 100 to 120 degrees
        ///             5 is 120 to 150 degrees
        ///             6 is 150 to 180 degrees
        /// </summary>
        /// <param name="wyrmAngle">The angle of the wyrm relative to this turret, in Degrees</param>
        /// <returns>The sector the wyrm is in relative to this turret</returns>
        private int getSector(double wyrmAngle)
        {
            int sector = -1;     

            if (wyrmAngle < 0 || wyrmAngle > 180)
            {
                sector = -1;
            }
            else if (wyrmAngle >= 0 && wyrmAngle < 30)
            {
                sector = 0;
            }
            else if (wyrmAngle >= 30 && wyrmAngle < 60)
            {
                sector = 1;
            }
            else if (wyrmAngle >= 60 && wyrmAngle < 80)
            {
                sector = 2;
            }
            else if (wyrmAngle >= 80 && wyrmAngle <= 100)
            {
                sector = 3;
            }
            else if (wyrmAngle > 100 && wyrmAngle <= 120)
            {
                sector = 4;
            }
            else if (wyrmAngle > 120 && wyrmAngle <= 150)
            {
                sector = 5;
            }
            else if (wyrmAngle > 150 && wyrmAngle <= 180)
            {
                sector = 6;
            }

            return sector;
        }

        public override void Draw(SpriteBatch sb)
        {
            asSprite.Draw(sb, (int)xPos - spriteWidth / 2, (int)yPos - spriteHeight / 2, false);
        }

        public override void getEaten(WyrmPlayer thePlayer)
        {
            Explosion theExplosion = new Explosion(xPos, yPos, Game1.explosionTexture, false);
            Game1.explosions.Add(theExplosion);

            Game1.explosion.Play();
        }
    }
}
