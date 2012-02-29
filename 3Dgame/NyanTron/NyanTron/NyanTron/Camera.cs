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
        static Vector3 DEFAULTPOSITION = new Vector3(-1f, 0f, -1.5f);
        static Vector3 DEFAULTUP = new Vector3(0, 0, -1);
        static Vector3 DEFAULTTARGET = new Vector3(1, 4, 0);

        private Vector3 position;
        private Vector3 target;
        private Vector3 up;
        public Matrix viewMatrix, projectionMatrix;


        Quaternion rotation = Quaternion.Identity;

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
          
            Vector3 campos = DEFAULTPOSITION;
            campos = Vector3.Transform(campos, Matrix.CreateFromQuaternion(rotation));
            campos += thePlayer.Position;

            Vector3 camup = DEFAULTUP;
            camup = Vector3.Transform(camup, Matrix.CreateFromQuaternion(rotation));

            Vector3 addVector = Vector3.Transform(DEFAULTTARGET, thePlayer.Rotation);
            Vector3 target = thePlayer.Position + addVector;


            viewMatrix = Matrix.CreateLookAt(campos, target, camup);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 0.2f, 1000.0f);

            position = campos;
            up = camup;
            rotation = Quaternion.Lerp(rotation, thePlayer.Rotation, 0.2f);


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