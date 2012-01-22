using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreadWyrm
{
    class WyrmSegment
    {
        //The texture of this Wyrm segment
        Texture2D t2d_segmentSprite;

        //The segment in front of this Wyrm segment
        //A null value indicates it is the head
        WyrmSegment frontSeg;

        //The physical parameters of this Wyrm segment
        float f_xPos;
        float f_yPos;

        float f_Speed;
        float f_SpeedMin;
        float f_SpeedMax;

        float f_Direction;

        float f_Acceleration;

        float f_RotationSpeed;
        float f_RotationSpeedMin;
        float f_RotationSpeedMax;

        public float X
        {
            get { return f_xPos; }
            set { f_xPos = value; }
        }

        public float Y
        {
            get { return f_yPos; }
            set { f_yPos = value; }
        }

        public float Speed
        {
            get { return f_Speed; }
            set { f_Speed = MathHelper.Clamp(value, f_SpeedMin, f_SpeedMax); }
        }

        public float SpeedMin
        {
            get { return f_SpeedMin; }
            set { f_SpeedMin = value; }
        }

        public float SpeedMax
        {
            get { return f_SpeedMax; }
            set { f_SpeedMax = value; }
        }

        public float Direction
        {
            get { return f_Direction; }
            set { f_Direction = value % (float) (2 * Math.PI); }
        }

        public float Acceleration
        {
            get { return f_Acceleration; }
            set { f_Acceleration = value; }
        }

        public float RotationSpeed
        {
            get { return f_RotationSpeed; }
            set { f_RotationSpeed = MathHelper.Clamp(value, f_RotationSpeedMin, f_RotationSpeedMax); }
        }

        public float RotationSpeedMin
        {
            get { return f_RotationSpeedMin; }
            set { f_RotationSpeedMin = value; }
        }

        public float RotiationSpeedMax
        {
            get { return f_RotationSpeedMax; }
            set { f_RotationSpeedMax = value; }
        }

        public WyrmSegment FrontSegment
        {
            get { return frontSeg; }
            set { frontSeg = value; }
        }

        public WyrmSegment(Texture2D texture, float X, float Y, WyrmSegment front)
        {
            t2d_segmentSprite = texture;
            f_xPos = X;
            f_yPos = Y;
            frontSeg = front;
        }

        public void Update()
        {

        }
    }
}
