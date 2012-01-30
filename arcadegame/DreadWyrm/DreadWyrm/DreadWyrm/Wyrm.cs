using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreadWyrm
{
    class Wyrm
    {
        //The minimum speed a Wyrm may travel at
        const float HEADSPEEDMIN = 0.0f;

        //The location in the segment array representing the head
        const int HEAD = 0;

        //The width of the Wyrm sprites
        const int SPRITEWIDTH_HEAD = 160; //The head is larger so it can have mandibles
        const int SPRITEWIDTH = 100;

        //The height of the Wyrm sprites
        const int SPRITEHEIGHT_HEAD = 160;
        const int SPRITEHEIGHT = 100;

        //The number of frames in the head animation
        const int HEADFRAMES = 0;

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

        //All segments are designed to simply "follow" the segment in front of it

        //The head has a velocity, etc.

        float f_HeadSpeed;
        float f_HeadSpeedMax;
        float f_HeadSpeedMin;

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
            f_HeadSpeedMax = 8;
            f_HeadSpeedMin = 2;
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
            for (int i = 1; i <= numSegments - 1; i++)
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
                //Update the velocity of the head (including the angle and magnitude)
                if (f_HeadSpeed + f_HeadAcceleration > f_HeadSpeedMax)
                    f_HeadSpeed = f_HeadSpeedMax;
                else if (f_HeadSpeed + f_HeadAcceleration < f_HeadSpeedMin)
                    f_HeadSpeed = f_HeadSpeedMin;
                else
                    f_HeadSpeed += f_HeadAcceleration;

                f_HeadDirection = (f_HeadDirection + f_HeadRotationSpeed) % (float) (2*Math.PI);

                if (f_HeadDirection < 0)
                {
                    f_HeadDirection = (float) (2*Math.PI);
                }

                l_segments[HEAD].Direction = f_HeadDirection;

                f_WyrmMoveCount = 0f;

                //update all segments
                for (int i = 0; i < numSegments; i++)
                {
                    //are we on the head?
                    if (i == HEAD)
                    {
                        //Update the X and Y positions of the head based on the magnitude and direction of the
                        //velocity of the head
                       // l_f_SegmentXPos[HEAD] += f_HeadSpeed * (float) Math.Cos(f_HeadDirection);
                       // l_f_SegmentYPos[HEAD] += f_HeadSpeed * (float) Math.Sin(f_HeadDirection);

                        l_segments[HEAD].X = l_segments[HEAD].X + (f_HeadSpeed * (float)Math.Cos(f_HeadDirection));
                        l_segments[HEAD].Y = l_segments[HEAD].Y + (f_HeadSpeed * (float)Math.Sin(f_HeadDirection));
                    }
                    else
                    {
                        //Update each Wyrm segment
                        l_segments[i].Update();
                    }
                    //now copy the segment data into the location lists
                    //l_f_SegmentXPos[i] = l_segments[i].X;
                    //l_f_SegmentYPos[i] = l_segments[i].Y;

                    //Update the Wyrm sprites
                    asSprites[i].Update(gametime);
                }
            }

            

            

        }

        public void Draw(SpriteBatch sb)
        {
            //Draw the rest of the segments
            for (int i = 1; i <= numSegments; i++)
            {
                //Do a special case for drawing the head since it's a bigger sprite
                //RIGHT NOW: it's not actually different
                if (i == numSegments)
                    asSprites[numSegments - i].Draw(sb, (int)l_segments[numSegments - i].X, (int)l_segments[numSegments - i].Y + (SPRITEHEIGHT / 2),
                                                l_segments[numSegments - i].Direction, false);
                else
                    asSprites[numSegments - i].Draw(sb, (int)l_segments[numSegments - i].X, (int)l_segments[numSegments - i].Y + (SPRITEHEIGHT / 2), 
                                                l_segments[numSegments - i].Direction, false);
            }
        }

    }
}
