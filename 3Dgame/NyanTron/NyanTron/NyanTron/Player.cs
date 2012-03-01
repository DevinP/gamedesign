using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace NyanTron
{
    class Player
    {
        Quaternion rotation = Quaternion.Identity;
        Vector3 position = new Vector3(0, 0, 0);

        const float SPEED = 1f;

        BoundingBox bBox;
        Vector3 B_BOX_MIN_DEFAULT = new Vector3(0, 0, 0);
        Vector3 B_BOX_MAX_DEFAULT = new Vector3(2, 2, 2);

        public Quaternion Rotation 
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public BoundingBox BoundingBox
        {
            get { return bBox; }
            set { bBox = value; }
        }

        public Player()
        {
           
            ModelHelper.modelTwo_WorldMatrix = (Matrix.CreateScale(0.0005f, 0.0005f, 0.0005f) * 
                Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateFromQuaternion(rotation) * 
                Matrix.CreateTranslation(position));

            Vector3 minCorner = B_BOX_MIN_DEFAULT;
            Vector3 maxCorner = B_BOX_MAX_DEFAULT;

            bBox = new BoundingBox(minCorner, maxCorner);
        }

        public void Update()
        {
            MoveForward();

            ModelHelper.resetWorldMatrices();

            ModelHelper.modelTwo_WorldMatrix = (Matrix.CreateScale(0.0005f, 0.0005f, 0.0005f) *
                Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateFromQuaternion(rotation) * 
                Matrix.CreateTranslation(position));

            UpdateBoundingBox();
        }

        private void UpdateBoundingBox()
        {
            Vector3 minCorner = B_BOX_MIN_DEFAULT;
            Vector3 maxCorner = B_BOX_MAX_DEFAULT;

            Vector3.Transform(minCorner, rotation);
            Vector3.Transform(maxCorner, rotation);

            minCorner += position;
            maxCorner += position;

            bBox = new BoundingBox(minCorner, maxCorner);
        }

        public void MoveForward()
        {
            Vector3 addVector = Vector3.Transform(new Vector3(0, 1, 0), rotation);
            position += addVector * SPEED;
        }
    }
}
