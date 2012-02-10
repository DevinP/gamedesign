using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreadWyrm
{
    public class Wyrm
    {
        //The minimum speed a Wyrm may travel at
        const float HEADSPEEDMIN = 0.0f;

        //The location in the segment array representing the head
        const int HEAD = 0;

        //The width of the Wyrm sprites
        const int SPRITEWIDTH_HEAD = 80; //The head is larger so it can have mandibles
        const int SPRITEWIDTH = 50;

        //The height of the Wyrm sprites
        const int SPRITEHEIGHT_HEAD = 80;
        const int SPRITEHEIGHT = 50;

        //The number of frames in the head animation
        const int HEADFRAMES = 0;

        //The acceleration caused by gravity
        const float GRAVITY = 0.075f;

        //The gravity multiplier fudge factor when going down.
        //Makes wyrm driving more fun
        const float GRAVITYMULTIPLIER = 1.7f;

        //Magic fake gravities to make the wyrm driving more fun
        const float XGRAV = 0.015f;

        //A friction that applies to the wyrm when it goes above max speed to slow it down
        const float FRICTION = 0.15f;

        //A mulitplier to adjust the amount the player can rotate their head in midair
        const float MIDAIRROTATION = 0.45f;

        //The radius in number the pixels of the wyrm head's eating radius
        public int eatRadius = 20;

        //Is the worm grounded?
        public bool b_wyrmGrounded;

        //For animating
        float f_WyrmMoveCount = 0.0f;
        float f_WyrmMoveDelay = 0.01f;

        //Body Parts
        
        //the textures are contained in a list.
        //The first texture is the head, the middle textures are segment textures, and the last segment is the tail texture
        List<Texture2D> l_t2d_SegmentTextures;

        //Similarly, a list of the segments themselves
        //The first element is the head, the middle elements are segment textures, and the last element is the tail texture
        public List<WyrmSegment> l_segments;
        
        //A list of animated sprite objects, one for each segment
        List<AnimatedSprite> asSprites;

        //How many segments the Wyrm has
        int i_numSegments;

        //All segments are designed to simply follow the segment in front of it

        //The head has a velocity, etc.

        float f_HeadSpeed;
        float f_HeadSpeedMax;
        float f_HeadSpeedMin;
        float f_HeadSpeedBoostMax;
        float f_HeadSpeedNormalMax;

        float f_HeadDirection;

        float f_HeadAcceleration;

        float f_HeadRotationSpeed;
        float f_HeadRotationSpeedMin;
        float f_HeadRotationSpeedMax;

        float f_HeadRotationAcceleration;
        float f_HeadRotationAccelerationMin;
        float f_HeadRotationAccelerationMax;


        public int numSegments
        {
            get { return i_numSegments; }
            set { i_numSegments = (int)MathHelper.Clamp(value, 0, Game1.WYRMSEGS); }
        }

        public List<Texture2D> SegmentTextures
        {
            get{return l_t2d_SegmentTextures;}
            set{l_t2d_SegmentTextures = value;}
        }

        public float HeadSpeed
        {
            get { return f_HeadSpeed; }
            set { f_HeadSpeed = MathHelper.Clamp(value, HEADSPEEDMIN, f_HeadSpeedMax); }
        }

        public float HeadSpeedMax
        {
            get { return f_HeadSpeedMax; }
            set { f_HeadSpeedMax = value; }
        }

        public float HeadSpeedBoostMax
        {
            get { return f_HeadSpeedBoostMax; }
            set { f_HeadSpeedBoostMax = value; }
        }

        public float HeadSpeedNormalMax
        {
            get { return f_HeadSpeedNormalMax; }
            set { f_HeadSpeedNormalMax = value; }
        }

        public float HeadSpeedMin
        {
            get { return f_HeadSpeedMin; }
            set { f_HeadSpeedMin = value; }
        }

        public float HeadDirection
        {
            get { return f_HeadDirection; }
            set { f_HeadDirection = value % (float)(2*Math.PI); }
        }

        public float HeadAcceleration
        {
            get { return f_HeadAcceleration; }
            set { f_HeadAcceleration = value; }
        }

        public float HeadRotationSpeed
        {
            get { return f_HeadRotationSpeed; }
            set { f_HeadRotationSpeed = MathHelper.Clamp(value, f_HeadRotationSpeedMin, f_HeadRotationSpeedMax); }
        }

        public float HeadRotationSpeedMin
        {
            get { return f_HeadRotationSpeedMin; }
            set { f_HeadRotationSpeedMin = value; }
        }

        public float HeadRotationSpeedMax
        {
            get { return f_HeadRotationSpeedMax; }
            set { f_HeadRotationSpeedMax = value; }
        }

        public float HeadRotationAcceleration
        {
            get { return f_HeadRotationAcceleration; }
            set { f_HeadRotationAcceleration = MathHelper.Clamp(value, f_HeadRotationAccelerationMin, f_HeadRotationAccelerationMax); }
        }

        public float HeadRotationAccelerationMin
        {
            get { return f_HeadRotationAccelerationMin; }
            set { f_HeadRotationAccelerationMin = value; }
        }

        public float HeadRotationAccelerationMax
        {
            get { return f_HeadRotationAccelerationMax; }
            set { f_HeadRotationAccelerationMax = value; }
        }

        public Wyrm(float initialX, float initialY, List<Texture2D> textures, int segments)
        {
            f_HeadSpeedMax = 5.2f;
            f_HeadSpeedMin = 2;
            f_HeadSpeedBoostMax = 2 * f_HeadSpeedMax;
            f_HeadSpeedNormalMax = f_HeadSpeedMax;

            f_HeadRotationSpeedMax = 6;
            f_HeadRotationSpeedMin = -6;

            //Save the wyrm textures given to us
            l_t2d_SegmentTextures = textures;

            //Start the Wyrm by creating the head
            l_segments = new List<WyrmSegment>();
            l_segments.Add(new WyrmSegment(l_t2d_SegmentTextures[HEAD], initialX, initialY, null));
            l_segments[0].isFrontSegment = true;
            asSprites = new List<AnimatedSprite>();

            //Create the head sprite, with its animation
            asSprites.Add(new AnimatedSprite(l_t2d_SegmentTextures[HEAD], (int)0, (int)0, SPRITEWIDTH_HEAD, SPRITEHEIGHT_HEAD, HEADFRAMES));
            asSprites[HEAD].IsAnimating = false;  

            numSegments = segments;

            //Add the rest of the Wyrm segments on and create sprites for them
            for (int i = 1; i <= numSegments - 2; i++)
            {
                //Create a new segment and attach it to the one in front of it
                l_segments.Add(new WyrmSegment(l_t2d_SegmentTextures[i], initialX, initialY, l_segments[i - 1]));
                l_segments[i].isFrontSegment = false;

                //Create a sprite object to correspond to the wyrm segment
                asSprites.Add(new AnimatedSprite(l_t2d_SegmentTextures[i], (int)0, (int)0, l_t2d_SegmentTextures[i].Width, l_t2d_SegmentTextures[i].Height, 0));
                asSprites[i].IsAnimating = false;
            }

            //Finish creating the wyrm by adding on the tail
            l_segments.Add(new WyrmSegment(l_t2d_SegmentTextures[l_t2d_SegmentTextures.Count - 1], initialX, initialY, l_segments[l_t2d_SegmentTextures.Count - 2]));
            asSprites.Add(new AnimatedSprite(l_t2d_SegmentTextures[l_t2d_SegmentTextures.Count - 1], 0, 0, 
                                             l_t2d_SegmentTextures[l_t2d_SegmentTextures.Count - 1].Width, 
                                             l_t2d_SegmentTextures[l_t2d_SegmentTextures.Count - 1].Height, 0));
            asSprites[asSprites.Count - 1].IsAnimating = false;

            //for other segments...
            for (int i = 0; i < numSegments; i++)
            {
                l_segments[i].X = initialX;
                l_segments[i].Y = initialY;
            }

            //lets update the offsets for the newly created segments
            WyrmSegment.calcOffsets();
        }

        public void Update(GameTime gametime)
        {
            f_WyrmMoveCount += (float)gametime.ElapsedGameTime.TotalSeconds;

            if(f_WyrmMoveCount > f_WyrmMoveDelay)
            {

                b_wyrmGrounded = Background.checkIsGrounded((int)l_segments[HEAD].X, (int)l_segments[HEAD].Y);

                #region Wyrm Movement
                if (!b_wyrmGrounded)
                {
                    #region The wyrm is in the air (gravity effects)

                    //Decompose the wyrm's polar speed into x and y velocities
                    float xVel, yVel;

                    xVel = f_HeadSpeed * (float) Math.Cos((double)f_HeadDirection);  
                    yVel = f_HeadSpeed * (float) Math.Sin((double)f_HeadDirection);

                    //Fudge the x velocity a little if going up. This make wyrm driving more fun!
                    if (yVel <= 0)
                    {
                        if (xVel >= 0)
                            xVel = xVel + XGRAV;
                        else
                            xVel = xVel - XGRAV;
                    }

                    if (yVel <= 0)
                    {
                        yVel = yVel + GRAVITY;
                    }
                    else
                    {
                        yVel = yVel + GRAVITY * GRAVITYMULTIPLIER;
                    }

                    //Recompose the x and y velocities into a head direction and speed
                    f_HeadSpeed = (float) Math.Sqrt((double) (xVel * xVel + yVel * yVel));

                    //Change the calculation based on which quadrant we're in
                    if (xVel > 0 && yVel > 0) //Quadrant 1 (0 degrees to 90 degrees)
                    {
                        f_HeadDirection = (float)Math.Atan(yVel / xVel);
                    }
                    else if (xVel < 0 && yVel > 0) //Quadrant 2 (90 degrees to 180 degrees)
                    {
                        f_HeadDirection = (float)(Math.PI + Math.Atan(yVel / xVel));
                    }
                    else if (xVel < 0 && yVel < 0) //Quadrant 3 (180 degrees to 270 degrees)
                    {
                        f_HeadDirection = (float)(Math.PI + Math.Atan(yVel / xVel));
                    }
                    else if (xVel > 0 && yVel < 0) //Quadrant 4 (270 degrees to 360 aka 0 degrees)
                    {
                        f_HeadDirection = (float)Math.Atan(yVel / xVel);
                    }

                    if (f_HeadSpeed + f_HeadAcceleration > f_HeadSpeedMax*2.5f)
                    {
                        f_HeadSpeed = f_HeadSpeedMax*2.5f;
                    }

                    f_HeadDirection = (f_HeadDirection + (f_HeadRotationSpeed * MIDAIRROTATION)) % (float)(2 * Math.PI);

                    #endregion
                }
                else
                {
                    #region The wyrm is in the ground (no gravity effects)

                    //Reset the dive boost boolean so we can dive in our next jump
                    //b_hasDiveBoosted = 0;

                    //Update the velocity of the head (including the angle and magnitude)
                    if (f_HeadSpeed + f_HeadAcceleration > f_HeadSpeedMax)
                    {
                        f_HeadSpeed = f_HeadSpeed - FRICTION;
                    }
                    else if (f_HeadSpeed + f_HeadAcceleration < f_HeadSpeedMin)
                    {
                        f_HeadSpeed = f_HeadSpeedMin;
                    }
                    else
                        f_HeadSpeed += f_HeadAcceleration;       

                    f_HeadDirection = (f_HeadDirection + f_HeadRotationSpeed) % (float)(2 * Math.PI);

                    #endregion
                }            

                if (f_HeadDirection < 0)
                {
                    f_HeadDirection = (float) (2*Math.PI + f_HeadDirection);
                }

                l_segments[HEAD].Direction = f_HeadDirection;

                f_WyrmMoveCount = 0f;
                #endregion

                #region Segment update/copy
                //update all segments
                for (int i = 0; i < numSegments; i++)
                {
                    //are we on the head?
                    if (i == HEAD)
                    {

                        l_segments[HEAD].X = l_segments[HEAD].X + (f_HeadSpeed * (float)Math.Cos(f_HeadDirection));
                        l_segments[HEAD].Y = l_segments[HEAD].Y + (f_HeadSpeed * (float)Math.Sin(f_HeadDirection));
                    }
                    else
                    {
                        //Update each Wyrm segment
                        l_segments[i].Update();
                    }

                    //Update the Wyrm sprites
                    asSprites[i].Update(gametime);



                }
                #endregion

                /*
                #region Dealings with the Background class

                //See if the head's position is grounded
                b_wyrmGrounded = Background.checkIsGrounded((int)l_segments[HEAD].X, (int)l_segments[HEAD].Y);


                #region Create dirt paths on background
                //We need to run through each pixel of the tail

                //the starting point of the tail texture
                int tailStartX = (int)l_segments[l_segments.Count-1].X - SPRITEWIDTH/2;
                int tailStartY = (int)l_segments[l_segments.Count - 1].Y;

                //for each pixel in the X direction
                for (int i = tailStartX; i < tailStartX + SPRITEWIDTH; i++)
                {
                    //for each pixel i nthe y direction
                    for (int j = tailStartY; j < tailStartY + SPRITEHEIGHT; j++)
                    {
                        //the wyrm better be on the screen
                        if (i > 0 && j > 0 && i < Background.SCREENWIDTH && j < Background.SCREENHEIGHT)
                        {
                            // is the background alpha already less than 100%?
                            if (Background.pixels[j * Background.SCREENWIDTH + i].A < 255)
                            {
                                //actually, do nothing
                            }
                            else
                            {
                                //lets create a wyrmtrail object
                                Background.createWyrmTrail(i, j);

                                //actually, make alpha 0, and murder the screen
                                //Background.pixels[j * Background.SCREENWIDTH + i].A = 0;
                            }
                        }
                    }

                }



                #endregion

                #endregion
                  
                 //*/

            }

        }

        public void Draw(SpriteBatch sb)
        {
            //Draw all the of the segments
            for (int i = 1; i <= numSegments; i++)
            {
                asSprites[numSegments - i].Draw(sb, (int)l_segments[numSegments - i].X, (int)l_segments[numSegments - i].Y + (SPRITEHEIGHT / 2), 
                                                l_segments[numSegments - i].Direction, false);
            }
        }

    }
}
