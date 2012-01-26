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

        //The location in the segment array representing the tail
        const int TAIL = 1;

        //The width of the Wyrm sprites
        const int SPRITEWIDTH = 154;

        //The height of the Wyrm sprites
        const int SPRITEHEIGHT = 154;

        //The number of frames in the head animation
        const int HEADFRAMES = 0;

        float f_WyrmMoveCount = 0.0f;
        float f_WyrmMoveDelay = 0.01f;

        //Body Parts
        
        //the textures are contained in a list.
        //The first texture is the head, the second is the tail, and the rest are middle segments
        List<Texture2D> l_t2d_SegmentTextures;

        //Similarly, a list of the segments themselves
        //The first element is the head, the second is the tail, and the rest are middle segments
        List<WyrmSegment> l_segments;
        
        //A list of animated sprite objects, one for each segment
        List<AnimatedSprite> asSprites;

        //How many segments the Wyrm has
        int i_numSegments;


        //The wyrm contains the positions of each and every segment
        //The first is the head, the second is the tail, and the rest are middle segments
        List<float> l_f_SegmentXPos;
        List<float> l_f_SegmentYPos;

        //All segments are designed to simply "follow" the head

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
            set { i_numSegments = (int)MathHelper.Clamp(value, 0, 10); }
        }

        public List<Texture2D> SegmentTextures
        {
            get{return l_t2d_SegmentTextures;}
            set{l_t2d_SegmentTextures = value;}
        }

        public List<float> SegmentXPos
        {
            get { return l_f_SegmentXPos; }
            set { l_f_SegmentXPos = value; }
        }

        public List<float> SegmentYPos
        {
            get { return l_f_SegmentYPos; }
            set { l_f_SegmentYPos = value; }
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
            f_HeadSpeedMin = 1f;
            f_HeadRotationSpeedMax = 6;
            f_HeadRotationSpeedMin = -6;

            l_f_SegmentXPos = new List<float>();
            l_f_SegmentYPos = new List<float>();

            l_f_SegmentXPos.Add(initialX);
            l_f_SegmentYPos.Add(initialY);

            //for 8 other segments...
            for (int i = 0; i < 8; i++)
            {
                l_f_SegmentXPos.Add(initialX);
                l_f_SegmentYPos.Add(initialY);
            }
            l_t2d_SegmentTextures = textures;

            numSegments = segments;

            l_segments = new List<WyrmSegment>();

            //Create the Wyrm head
            l_segments.Add(new WyrmSegment(l_t2d_SegmentTextures[HEAD], initialX, initialY, null));

            asSprites = new List<AnimatedSprite>();
            //Create the head sprite, with its animation
            asSprites.Add(new AnimatedSprite(l_t2d_SegmentTextures[HEAD], (int)0, (int)0, SPRITEWIDTH, SPRITEHEIGHT, HEADFRAMES));
            asSprites[HEAD].IsAnimating = false;

            if (numSegments > 1)
            {
                //Add the rest of the sprites, which do not animate
                for (int i = 1; i < numSegments; i++)
                {
                    //Create a new segment and attach it to the one in front of it
                    l_segments.Add(new WyrmSegment(l_t2d_SegmentTextures[i], initialX, initialY, l_segments[i - 1]));

                    //Create a sprite object to correspond to the wyrm segment
                    asSprites.Add(new AnimatedSprite(l_t2d_SegmentTextures[i], (int)0, (int)0, l_t2d_SegmentTextures[i].Width, l_t2d_SegmentTextures[i].Height, 0));
                    asSprites[i].IsAnimating = false;
                }
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

                f_HeadDirection += f_HeadRotationSpeed;

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
                    l_f_SegmentXPos[i] = l_segments[i].X;
                    l_f_SegmentYPos[i] = l_segments[i].Y;

                    //Update the Wyrm sprites
                    asSprites[i].Update(gametime);
                }
            }

            

            

        }

        public void Draw(SpriteBatch sb)
        {
            //Draw the head
            asSprites[HEAD].Draw(sb, (int)l_f_SegmentXPos[HEAD] + (SPRITEWIDTH/2), (int)l_f_SegmentYPos[HEAD] + (SPRITEHEIGHT/2), f_HeadDirection, false);

            //Draw the rest of the segments
            for (int i = 1; i < numSegments; i++)
            {
                asSprites[i].Draw(sb, (int)l_f_SegmentXPos[i] + (SPRITEWIDTH / 2), (int)l_f_SegmentYPos[i] + (SPRITEHEIGHT / 2), l_segments[i].Direction, false);
            }
        }

    }
}
