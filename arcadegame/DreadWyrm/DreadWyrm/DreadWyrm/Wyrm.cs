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
        const int SPRITEWIDTH = 32;

        //The height of the Wyrm sprites
        const int SPRITEHEIGHT = 32;

        //The number of frames in the head animation
        const int HEADFRAMES;

        //Body Parts
        
        //the textures are contained in a list.
        //The first texture is the head, the second is the tail, and the rest are middle segments
        List<Texture2D> l_t2d_SegmentTextures;
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
            set { i_numSegments = (int)MathHelper.Clamp(value, 2, 10); }
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

        public float HeadDirection
        {
            get { return f_HeadDirection; }
            set { f_HeadDirection = value % 360; }
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
            l_f_SegmentXPos[HEAD] = initialX;
            l_f_SegmentYPos[HEAD] = initialY;
            l_t2d_SegmentTextures = textures;

            numSegments = segments;

            //Create the head sprite, with its animation
            asSprites.Add(new AnimatedSprite(l_t2d_SegmentTextures[HEAD], (int)initialX, (int)initialY, SPRITEWIDTH, SPRITEHEIGHT, HEADFRAMES));
            asSprites[HEAD].IsAnimating = false;

            //Add the rest of the sprites, which do not animate
            for (int i = 1; i < numSegments; i++)
            {
                asSprites.Add(new AnimatedSprite(l_t2d_SegmentTextures[i], (int) initialX, (int) initialY, SPRITEWIDTH, SPRITEHEIGHT, 0));
                asSprites[i].IsAnimating = false;
            }
        }

        public void Update(GameTime gametime)
        {
            f_HeadSpeed += f_HeadRotationAcceleration;
            f_HeadDirection += f_HeadRotationSpeed;
        }
        


    }
}
