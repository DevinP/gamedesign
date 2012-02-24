using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace NyanTron
{
    class ModelHelper
    {

        //These are the world matrixes, for models to be drawn onto
        //Basically, you modify worldmatrices as your object moves/rotates/embigens, and then use the matrix to draw a model

        //SCALE TRANSLATE ROTATE THESE!
        public static Matrix modelOne_WorldMatrix;
        public static Matrix modelTwo_WorldMatrix;


        //These are model placeholders for whatever models we may be loading
        //Basically, load them and NEVER TOUCH THEM AGAIN

        //LOAD THESE!
        //public static Model modelOne;
        public static Model modelTwo;


        /// <summary>
        /// Load all the models
        /// </summary>
        public static void LoadModels(ContentManager cm)
        {
            //TODO: Replace/add other models

            //loading goblet.x and nyan.fbx (blender is your friend)
            //modelOne = cm.Load<Model>("goblet");
            modelTwo = cm.Load<Model>("nyan");
        }

        /// <summary>
        /// Set/Reset all the model's world matrices
        /// </summary>
        public static void resetWorldMatrices()
        {
            //TODO: Replace/add other models

            //set them to the identity matrix. that oughta do it.
            modelOne_WorldMatrix = Matrix.Identity;
            modelTwo_WorldMatrix = Matrix.Identity;
        }

        /// <summary>
        /// Draw all the models
        /// </summary>
        public static void drawModels(Camera camera)
        {
            //TODO: Replace/add other models

            //draw onto the camera with each model and each model worldmatrix
            //DrawModel(camera, modelOne, modelOne_WorldMatrix);
            DrawModel(camera, modelTwo, modelTwo_WorldMatrix);
        }

        /// <summary>
        /// Draws a model using a worldmatrix (I swear this is all just black magic) into a camera
        /// </summary>
        public static void DrawModel(Camera camera, Model model, Matrix worldMatrix)
        {
            //Here is a list of model transforms
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];

            //Here we take the model, and grab its "bones" and put them in that matrix up there
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            //For all the meshes in the model..
            foreach (ModelMesh mesh in model.Meshes)
            {
                //For all the effects (texture, lighting)...
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //Just give the default lighting
                    effect.EnableDefaultLighting();

                    //Translate the model by the worldmatrix (move/rotate the model...)
                    effect.World = modelTransforms[mesh.ParentBone.Index] * worldMatrix;

                    //Transpose the transforms into the view?
                    effect.View = camera.viewMatrix;

                    //I guess now they go onto the screen? I really dont know
                    effect.Projection = camera.projectionMatrix;
                }
                //We did all the effects, now draw it!
                mesh.Draw();
            }

        }

        /// <summary>
        /// This returns a scaled matrix scaled in the dimensions specified
        /// </summary>
        public static Matrix ScaleMatrix(Matrix matrix, float scaleX, float scaleY, float scaleZ)
        {
            //Easy peasy. Ask the matrix class to do it!
            return matrix * Matrix.CreateScale(scaleX, scaleY, scaleZ);
        }

        /// <summary>
        /// This returns a rotated matrix rotated by the amount specified
        /// In DEGREES, mind you.
        /// </summary>
        public static Matrix RotateMatrix(Matrix matrix, float degX, float degY, float degZ)
        {
            //Derp, just change these back to radians real quick...
            float radX = MathHelper.ToRadians(degX);
            float radY = MathHelper.ToRadians(degY);
            float radZ = MathHelper.ToRadians(degZ);

            //Create a temporary matrix
            Matrix result = matrix;

            //Twist and turn the matrix
            result = result * Matrix.CreateRotationX(radX);
            result = result * Matrix.CreateRotationY(radY);
            result = result * Matrix.CreateRotationZ(radZ);

            Console.WriteLine("degX: " + degX + " degY: " + degY + " degZ: " + degZ);

            //Return the result of the rotation
            return result;
        }

        /// <summary>
        /// This returns a translated matrix translated by the amount specified
        /// </summary>
        public static Matrix TranslateMatrix(Matrix matrix, float x, float y, float z)
        {
            //Simply retun a translated version
            return matrix * Matrix.CreateTranslation(x, y, z);
        }



    }
}
