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
        private Vector3 position;
        private Vector3 target;
        public Matrix viewMatrix, projectionMatrix;

        GraphicsDevice device;

        Quaternion cameraRot = Quaternion.Identity;

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
            position = new Vector3(0, 0, Wallbox.BOXDEPTH / 2 + 50);

            target = new Vector3(0, 0, 1);

            viewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);

            projectionMatrix =                              //Result matrix
                Matrix.CreatePerspectiveFieldOfView(        //Function call
                MathHelper.ToRadians(45.0f),                //45 degree field of view (converted to radians)
                device.Viewport.AspectRatio,                //16/9 aspect ratio
                0.5f,                                       //Show things that are further than 0.5 away...
                Wallbox.BOXHEIGHT * 2f                    //...and less than BOXHEIGHT away
                );
        }


        /// <summary>
        /// This updates the camera. In theory you would have it follow your character, etc.
        /// </summary>
        public void Update()
        {
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

            Console.WriteLine("X: " + position.X + " Y: " + position.Y + " Z: " + position.Z);
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