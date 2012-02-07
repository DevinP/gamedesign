using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreadWyrm
{
    public class WyrmSegment
    {
        //The texture of this Wyrm segment
        Texture2D t2d_segmentSprite;

        //The segment in front of this Wyrm segment
        //A null value indicates it is the head
        WyrmSegment frontSeg;

        //The global magic number dimensions of the wyrm segments
        static float f_width = 50f;
        static float f_height = 50f;

        //a magic number float of the percentage from the center to the "chain" node.
        static float f_DISTORIGIN = 0.65f;
        
        //a static list of the x and y offsets for each degree of rotation
        static float[] f_XOFFS = new float[360];
        static float[] f_YOFFS = new float[360];

        bool isHead;

        bool isGrounded;

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

        public bool isFrontSegment
        {
            get { return isHead; }
            set { isHead = value; }
        }

        public float height
        {
            get { return f_height; }
            set { f_height = value; }
        }
        public float width
        {
            get { return f_width; }
            set { f_width = value; }
        }

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

            //Now is an appropriate time to follow the node ahead of us
            Follow();


        }


        public void Follow()
        {
            #region Find front offset (frontX and frontY are new offsetted points)

            //now we figure out where exactly its offset point is
            //the front's angle in degrees...
            int frontAngle = (int)(frontSeg.Direction * (180 / Math.PI));

            //now get the "other" side
            frontAngle = (frontAngle + 180) % 360;

            float frontX;
            float frontY;

            frontX = frontSeg.X + f_XOFFS[frontAngle];
            frontY = frontSeg.Y + f_YOFFS[frontAngle];

            #endregion

            #region Self point at front (change Direction to point at front node)

            //the change in Y..
            float yDiff = frontY - Y;
            float xDiff = frontX - X;

            //Change the calculation based on which quadrant we're in
            if (xDiff > 0 && yDiff > 0) //Quadrant 1 (0 degrees to 90 degrees)
            {
                Direction = (float)Math.Atan(yDiff / xDiff);
            }
            else if (xDiff < 0 && yDiff > 0) //Quadrant 2 (90 degrees to 180 degrees)
            {
                Direction = (float)(Math.PI + Math.Atan(yDiff / xDiff));
            }
            else if (xDiff < 0 && yDiff < 0) //Quadrant 3 (180 degrees to 270 degrees)
            {
                Direction = (float)(Math.PI + Math.Atan(yDiff / xDiff));
            }
            else if (xDiff > 0 && yDiff < 0) //Quadrant 4 (270 degrees to 360 aka 0 degrees)
            {
                Direction = (float)Math.Atan(yDiff / xDiff);
            }

            int dirDeg = (int)(Direction * ((180 / Math.PI)) + 360) % 360;

            #endregion

            #region Find self offset(selfX and selfY are new offsetted points)

            //simply take the current position and add the offsets
            int index = dirDeg;
            float selfX = X + f_XOFFS[index];
            float selfY = Y + f_YOFFS[index];

            #endregion

            #region Move self to appropriate location (X and Y are modified)

            X = X + frontX - selfX;
            Y = Y + frontY - selfY;

            #endregion

            //In theory, everything is done
        }

        public static void calcOffsets()
        {
            //here we fill up the offset lists
            //we do it 360 times, once for each angle degree

            //SIN(ANG) == YOFF/HYP
            //YOFF == SIN(ANG)*HYP

            //COS(ANG) == XOFF/HYP
            //XOFF == COS(ANG)*HYP

            for (int i = 0; i < 360; i++)
            {
                float iRad = ((float)Math.PI/180) * i;
                f_YOFFS[i] = (float)Math.Sin(iRad) * ((f_height * f_DISTORIGIN) / 2);
                f_XOFFS[i] = (float)Math.Cos(iRad) * ((f_width * f_DISTORIGIN) / 2);
            }
        }

        public void checkGrounded()
        {

        }
    }
}
