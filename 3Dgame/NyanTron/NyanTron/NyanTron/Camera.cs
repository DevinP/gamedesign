/*
 * Author: Caleb Pentecost
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



namespace NyanTron
{
    class Camera
    {
        //static Vector3 DEFAULTPOSITION = new Vector3(-10, 3, 2);
        static Vector3 DEFAULTPOSITION = new Vector3(-1, 3, 2);
        static Vector3 DEFAULTUP = new Vector3(0, 0, -1);
        static Vector3 DEFAULTTARGET = new Vector3(5, 2, 1);

        private Vector3 position;
        private Vector3 target;
        private Vector3 up;
        public Matrix viewMatrix, projectionMatrix;

        GraphicsDevice device;

        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
            set { viewMatrix = value; }
        }

        public Matrix ProjectionMatrix
        {
            get { return projectionMatrix; }
            set { projectionMatrix = value; }
        }

        /// <summary>
        /// The constructor
        /// </summary>
        public Camera(GraphicsDevice theDevice)
        {
            device = theDevice;

            //Just reset the camera
            ResetCamera();
        }

        /// <summary>
        /// Resets the camera to the default position; pointing at the identity matrix
        /// </summary>
        public void ResetCamera()
        {
            //The camera's current position is 0, 0, and 50 away from the origin
            position = DEFAULTPOSITION;
            up = DEFAULTUP;
            target = DEFAULTTARGET;

            viewMatrix = Matrix.CreateLookAt(position, target, up);

            projectionMatrix =                              //Result matrix
                Matrix.CreatePerspectiveFieldOfView(        //Function call
                MathHelper.ToRadians(45.0f),                //45 degree field of view (converted to radians)
                device.Viewport.AspectRatio,                //16/9 aspect ratio
                0.5f,                                       //Show things that are further than 0.5 away...
                Wallbox.BOXHEIGHT * 2f                      //...and less than BOXHEIGHT away
                );
        }


        /// <summary>
        /// This updates the camera. In theory you would have it follow your character, etc.
        /// </summary>
        public void Update(Player thePlayer)
        {
            position.X += 0.1f;
            target.X += 0.1f;

            //Code to rotate the camera - UNFINISHED
            double newTargetX = 0;
            double newTargetY = 0;
            double newTargetZ = 0;

            //XY plane (rotation about the Z-Axis)
            //newTargetX += (Vector3.Distance(DEFAULTPOSITION, DEFAULTTARGET) * Math.Sin(MathHelper.ToRadians(thePlayer.ZRotation)));
            newTargetY += (Vector3.Distance(DEFAULTPOSITION, DEFAULTTARGET) * Math.Cos(MathHelper.ToRadians(thePlayer.XRotation)));

            //ZX plane (rotation about the Y-Axis)
            //newTargetX += (Vector3.Distance(DEFAULTPOSITION, DEFAULTTARGET)*Math.Sin(MathHelper.ToRadians(thePlayer.YRotation)));
            newTargetZ += (Vector3.Distance(DEFAULTPOSITION, DEFAULTTARGET)*Math.Cos(MathHelper.ToRadians(thePlayer.YRotation)));

            //target.X = (float) newTargetX;
            target.Y = (float) newTargetY;
            target.Z = (float) -newTargetZ;

            //update the view matrix to point from the position to the target
            UpdateViewMatrix();
        }

        /// <summary>
        /// Tell the view matrix to be exactly where we are pointing at
        /// </summary>
        private void UpdateViewMatrix()
        {
            //Create a matrix to look at cute girls with
            viewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);
        }

        /// <summary>
        /// Moves the position of the camera by the specified amount
        /// </summary>
        public void translatePosition(float x, float y, float z)
        {
            //Just move it
            position.X = position.X + x;
            position.Y = position.Y + y;
            position.Z = position.Z + z;
        }

        /// <summary>
        /// Moves the position of the camera by the specified amount
        /// </summary>
        public void translateTarget(float x, float y, float z)
        {
            //Just move it
            target.X = target.X + x;
            target.Y = target.Y + y;
            target.Z = target.Z + z;
        }
    }
}