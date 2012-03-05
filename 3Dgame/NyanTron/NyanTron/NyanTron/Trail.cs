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

        BoundingOrientedBox bOBox;

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

        public BoundingOrientedBox BoundingBox
        {
            get { return bOBox; }
            set { bOBox = value; }
        }

        public Matrix WorldMatrix
        {
            get { return trail_WorldMatrix; }
            set { trail_WorldMatrix = value; }
        }



        public Trail(Vector3 playerPos, Quaternion playerRot)
        {
            position = playerPos + Vector3.Transform(new Vector3(0, -0.8f, -1f), playerRot); 
            rotation = playerRot;

            trail_WorldMatrix = Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateFromQuaternion(rotation)
                * Matrix.CreateTranslation(position);

            //Vector3 bBox_Min = new Vector3(position.X - 0, position.Y - 0.8f, position.Z - 0);
            //Vector3 bBox_Max = new Vector3(position.X - 0.1f, position.Y + 0.8f, position.Z + 1);
            //Vector3 bBox_Min = new Vector3(0, -0.8f, 0);
            //Vector3 bBox_Max = new Vector3(-0.2f, 0.8f, 1f);

            Vector3 bBox_Min = new Vector3(0.1f, -0.8f, 0);
            Vector3 bBox_Max = new Vector3(-0.1f, 0.8f, 1f);

            bBox = new BoundingBox(bBox_Min, bBox_Max);

            bOBox = BoundingOrientedBox.CreateFromBoundingBox(bBox);

            bOBox = bOBox.Transform(rotation, position);
        }
    }
}
