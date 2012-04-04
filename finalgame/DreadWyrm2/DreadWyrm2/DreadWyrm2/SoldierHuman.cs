using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreadWyrm2
{
    class SoldierHuman : Prey
    {
        float elapsedTimeVel;                 //For counting for changing the soldier's velocity
        const float TIMETOCHANGEVEL = 11;     //When to change velocity (in seconds)

        float elapsedTimeShoot = 0.45f;       //For counting for when another bullet can be shot
        const float TIMETOSHOOT = 0.9f;       //How long between shots (in seconds)

        const int BULLETDAMAGE = 10;           //The damage done by this unit
        const int BULLETSPEED = 5;

        bool isShooting = false;
        bool firstShot = true;

        const int NUM_FRAMES = 6;       //The number of frames in the animation for the soldier human
        const int SPRITE_HEIGHT = 25;   //The height of the soldier's sprite
        const int SPRITE_WIDTH = 20;    //The width of the soldier's sprite
        const int PREY_HEIGHT = 24;     //The height of the soldier for grounding purposes
        const float BOUNDING_RADIUS = 7;  //The bounding radius of the soldier
        const int MEAT_REWARD = 80;     //The amount of meat this unit is worth
        const int FACING_Y = 26;        //The Y position on the sprite sheet at which the sprite changes direction
        const int SHOOTING_LEFT_Y = 52; //The Y position on the sprite sheet at which the sprite shoots left
        const int SHOOTING_RIGHT_Y = 78;//The Y position on the sprite sheet at which the sprite shoots right

        /// <summary>
        /// A soldier equipped with a gun to combat the wyrm
        /// </summary>
        /// <param name="initialX">The initial X position of the soldier</param>
        /// <param name="initialY">The initial Y position of the soldier</param>
        /// <param name="predator">The wyrm for the soldier to shoot at</param>
        /// <param name="bulletTex"></param>
        public SoldierHuman(int initialX, int initialY, Wyrm predator)
            : base(initialX, initialY, predator)
        {
            preyheight = PREY_HEIGHT;
            boundingRadius = BOUNDING_RADIUS;
            spriteHeight = SPRITE_HEIGHT;
            spriteWidth = SPRITE_WIDTH;

            asSprite = new AnimatedSprite(preyTextures[SOLDIER], 0, 0, SPRITE_WIDTH, SPRITE_HEIGHT, NUM_FRAMES);
            asSprite.IsAnimating = true;

            if (Game1.m_random.NextDouble() < 0.5)
                xVel = -1;
            else
                xVel = 1;
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
                    asSprite.frameOffsetY = FACING_Y;
                else
                    asSprite.frameOffsetY = 0;
            }
            else
            {
                elapsedTimeShoot += (float)gametime.ElapsedGameTime.TotalSeconds;

                //Switch to the shooting animations
                if (xVel > 0)
                    asSprite.frameOffsetY = SHOOTING_RIGHT_Y;
                else
                    asSprite.frameOffsetY = SHOOTING_LEFT_Y;

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
                    Game1.bullets.Add(new Bullet(xPos, yPos, velVec * BULLETSPEED, Game1.bulletTexture, 4, 10, 8, 9, BULLETDAMAGE));

                    gunShot.Play();

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
            asSprite.Draw(sb, (int)xPos - spriteWidth / 2, (int)yPos - spriteHeight / 2, false);
        }

        public override void getEaten(WyrmPlayer thePlayer)
        {
            thePlayer.Meat += MEAT_REWARD;

            chomp.Play();
        }

    }
}
