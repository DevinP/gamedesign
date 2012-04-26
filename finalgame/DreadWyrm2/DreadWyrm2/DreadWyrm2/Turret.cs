using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.
GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DreadWyrm2
{
    class Turret : Building
    {
        bool isShooting = false;

        const float TIMETOSHOOT = 0.08f;                         //Delay between turret shots (in seconds)
        float elapsedTimeShoot = (float)(0 - 0.5 * TIMETOSHOOT);//A counter to track time between shots, so we know when we can shoot again (starts at 0 - FRAME_LENGTH)
        float FRAME_LENGTH = (float)(0.5 * TIMETOSHOOT);        //0.5 * TIMETOSHOOT

        SoundEffectInstance turretShotInstance;

        //Turret constants
        const int BULLETSPEED = 8;
        const int BULLETDAMAGE = 3;
        const int TURRET_MAX_RANGE = 500;
        const int TURRET_MAX_HIT_POINTS = 20;
        const int TURRET_HEALTH_BAR_WIDTH = 70;    //The width of the health bar in pixels

        //Spritesheet stuff
        const int SPRITE_WIDTH = 140;
        const int SPRITE_HEIGHT = 100;
        const int TURRET_HEIGHT = 101;
        const float TURRET_SPRITE_SCALING = 0.5f;
        const int NUM_FRAMES = 4;
        const int FACING_LEFT_DOWN = 0;
        const int FACING_LEFT_MIDDLE = 100;
        const int FACING_LEFT_UP = 200;
        const int FACING_RIGHT_DOWN = 300;
        const int FACING_RIGHT_MIDDLE = 400;
        const int FACING_RIGHT_UP = 500;
        const int BOUNDING_RADIUS = 29;
        const int BOUNDING_OFFSET_X = 7;
        const int BOUNDING_OFFSET_Y = 22;

        public override int SpriteWidth
        {
            get { return (int)(spriteWidth * TURRET_SPRITE_SCALING); }
            set { }
        }

        public override int SpriteHeight
        {
            get { return (int)(spriteHeight * TURRET_SPRITE_SCALING); }
            set { }
        }


        public Turret(int x, int y, Wyrm predator)
            : base(x, y, predator)
        {
            asSprite = new AnimatedSprite(buildingTextures[TURRET], 0, 0, SPRITE_WIDTH, SPRITE_HEIGHT, NUM_FRAMES);
            asSprite.IsAnimating = false;
            asSprite.FrameLength = FRAME_LENGTH;

            hitPoints = TURRET_MAX_HIT_POINTS;

            buildingheight = TURRET_HEIGHT;
            spriteHeight = SPRITE_HEIGHT;
            spriteWidth = SPRITE_WIDTH;

            boundingRadius = BOUNDING_RADIUS * TURRET_SPRITE_SCALING;

            turretShotInstance = turretShot.CreateInstance();
        }

        public override void Update(GameTime gametime)
        {
            base.Update(gametime);

            if (!theWyrm.b_wyrmGrounded)
            {
                //Calculate the length from the turret to the wyrm head
                Vector2 wyrmPos = new Vector2(theWyrm.l_segments[0].X, theWyrm.l_segments[0].Y);
                Vector2 turretPos = new Vector2(xPos, yPos);

                Vector2 diff = new Vector2(wyrmPos.X - turretPos.X, wyrmPos.Y - turretPos.Y);

                float length = diff.Length();

                //If the wyrm is within the range of the turret, fire at it
                if (length < TURRET_MAX_RANGE)
                    isShooting = true;
                else
                    isShooting = false;
            }
            else
            {
                isShooting = false;
            }

            if (isShooting)
            {
                elapsedTimeShoot += (float)gametime.ElapsedGameTime.TotalSeconds;

                if (elapsedTimeShoot >= TIMETOSHOOT)
                {
                    //Make a unit vector to determine the direction we need to shoot the bullet

                    //First, find the difference in x and y positions of the wyrm head and turret
                    float diffX = theWyrm.l_segments[0].X - xPos;
                    float diffY = theWyrm.l_segments[0].Y - yPos;

                    //Create the unit vector
                    //float unitMultiplier = (float)(1 / (Math.Sqrt(Math.Pow(diffX, 2) + Math.Pow(diffY, 2))));
                    //Vector2 velVec = new Vector2(diffX * unitMultiplier, diffY * unitMultiplier);

                    Vector2 velVec = new Vector2(diffX, diffY);
                    Vector2 unitVector = new Vector2();

                    Vector2.Normalize(ref velVec, out unitVector);

                    double angleToWyrm = Math.Atan2(-1 * (double)unitVector.Y, (double)unitVector.X);

                    //angleToWyrm = (angleToWyrm + 2 * Math.PI) % (2 * Math.PI);

                    angleToWyrm = MathHelper.ToDegrees((float)angleToWyrm);

                    int sector = getSector(angleToWyrm);

                    bool shootABullet = false;
                    float angleToShoot = 0;

                    Vector2 bulletOriginOffset = new Vector2();

                    switch (sector)
                    {
                        case -1:
                            shootABullet = false;
                            break;
                        case 0:
                            angleToShoot = Game1.m_random.Next(0, 30);
                            asSprite.frameOffsetY = FACING_RIGHT_DOWN;
                            bulletOriginOffset = new Vector2(134, 64);
                            shootABullet = true;
                            break;
                        case 1:
                            angleToShoot = Game1.m_random.Next(30, 60);
                            asSprite.frameOffsetY = FACING_RIGHT_MIDDLE;
                            bulletOriginOffset = new Vector2(126, 16);
                            shootABullet = true;
                            break;
                        case 2:
                            angleToShoot = Game1.m_random.Next(60, 80);
                            asSprite.frameOffsetY = FACING_RIGHT_UP;
                            bulletOriginOffset = new Vector2(110,8);
                            shootABullet = true;
                            break;
                        case 3:
                            shootABullet = false;
                            break;
                        case 4:
                            angleToShoot = Game1.m_random.Next(100, 120);
                            asSprite.frameOffsetY = FACING_LEFT_UP;
                            bulletOriginOffset = new Vector2(27, 8);
                            shootABullet = true;
                            break;
                        case 5:
                            angleToShoot = Game1.m_random.Next(120, 150);
                            asSprite.frameOffsetY = FACING_LEFT_MIDDLE;
                            bulletOriginOffset = new Vector2(8, 32);
                            shootABullet = true;
                            break;
                        case 6:
                            angleToShoot = Game1.m_random.Next(150, 180);
                            asSprite.frameOffsetY = FACING_LEFT_DOWN;
                            bulletOriginOffset = new Vector2(4, 64);
                            shootABullet = true;
                            break;
                        default:
                            break;
                    }

                    bulletOriginOffset = bulletOriginOffset * TURRET_SPRITE_SCALING;


                    if (shootABullet)
                    {
                        asSprite.IsAnimating = true;

                        velVec = new Vector2((float)Math.Cos((double)MathHelper.ToRadians(angleToShoot)), -1 * (float)Math.Sin((double)MathHelper.ToRadians(angleToShoot)));

                        //Create a bullet using that unit vector
                        Game1.bullets.Add(new Bullet(xPos + (int)bulletOriginOffset.X, yPos + (int)bulletOriginOffset.Y, 
                            velVec * BULLETSPEED, Game1.bulletTexture, 4, 10, 8, 9, BULLETDAMAGE));

                        turretShotInstance.Play();
                    }
                    else
                    {
                        asSprite.IsAnimating = false;
                        asSprite.Frame = 0;
                    }

                    elapsedTimeShoot = 0;
                }
            }
            else
            {
                asSprite.Frame = 0;
                asSprite.IsAnimating = false;

                elapsedTimeShoot = (float)(0 - 0.5 * TIMETOSHOOT);

                turretShotInstance.Stop();
            }

            asSprite.Update(gametime);

            footToGround();

            yPos += (int) (SPRITE_HEIGHT * TURRET_SPRITE_SCALING + 2);
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
            asSprite.Draw(sb, xPos, yPos, TURRET_SPRITE_SCALING);

            float greenBarWidth = (hitPoints / TURRET_MAX_HIT_POINTS) * TURRET_HEALTH_BAR_WIDTH;

            sb.Draw(hb_red, new Rectangle(xPos, yPos, TURRET_HEALTH_BAR_WIDTH, 5), Color.White);
            sb.Draw(hb_green, new Rectangle(xPos, yPos, (int)greenBarWidth, 5), Color.White);
        }

        public override int getBoundingX()
        {
            return xPos + (int)(((SPRITE_WIDTH * TURRET_SPRITE_SCALING)) / 2) + (int)(BOUNDING_OFFSET_X * TURRET_SPRITE_SCALING);
        }

        public override int getBoundingY()
        {
            return yPos + (int)(((SPRITE_HEIGHT * TURRET_SPRITE_SCALING)) / 2) + (int)(BOUNDING_OFFSET_Y * TURRET_SPRITE_SCALING);
        }
    }
}
