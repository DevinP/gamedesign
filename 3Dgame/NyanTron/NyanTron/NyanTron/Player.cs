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

        float speed = 1f;

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

        public Player()
        {
           
            ModelHelper.modelTwo_WorldMatrix = (Matrix.CreateScale(0.0005f, 0.0005f, 0.0005f) * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position));

        }

        public void Update()
        {
            MoveForward();

            ModelHelper.resetWorldMatrices();
            ModelHelper.modelTwo_WorldMatrix = (Matrix.CreateScale(0.0005f, 0.0005f, 0.0005f) *
                Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateFromQuaternion(rotation) * 
                Matrix.CreateTranslation(position));
        }

        public void MoveForward()
        {
            Vector3 addVector = Vector3.Transform(new Vector3(0, 1, 0), rotation);
            position += addVector * speed;
        }
    }
}
