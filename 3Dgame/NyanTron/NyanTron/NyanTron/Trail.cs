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
    public class Trail
    {
        Vector3 position;
        Quaternion rotation;
        BoundingBox bBox;

        Matrix trail_WorldMatrix;



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

        public Matrix WorldMatrix
        {
            get { return trail_WorldMatrix; }
            set { trail_WorldMatrix = value; }
        }



        public Trail(Vector3 playerPos, Quaternion playerRot)
        {
            position = playerPos;
            rotation = playerRot;


            /*trail_WorldMatrix = (Matrix.CreateScale(0.0005f, 0.0005f, 0.0005f) *
                Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateFromQuaternion(rotation) *
                Matrix.CreateTranslation(position));*/

            trail_WorldMatrix = Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateFromQuaternion(rotation)
                * Matrix.CreateTranslation(position);

            Vector3 bBox_Min = new Vector3(position.X - 1, position.Y - 1, position.Z - 1);
            Vector3 bBox_Max = new Vector3(position.X + 1, position.Y + 1, position.Z + 1);
            Vector3.Transform(bBox_Min, rotation);
            Vector3.Transform(bBox_Max, rotation);

            bBox = new BoundingBox(bBox_Min, bBox_Max);
        }
    }
}
