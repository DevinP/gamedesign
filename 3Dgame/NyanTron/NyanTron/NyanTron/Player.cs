using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NyanTron
{
    class Player
    {
        //For now, how much are we rotating in each direction?
        //ROTATE THESE TO ROTATE NYAN!!
        public float xRot = 270f;
        public float yRot = 270f;
        public float zRot = 0f;

        //I guess nyancat can translate?
        //MOVE THESE TO MOVE NYAN!
        float xTrans = 0;
        float yTrans = 0;
        float zTrans = 0f;

        //Nyan cat needs some scaling: too fat
        float xSca = 0.001f;
        float ySca = 0.001f;
        float zSca = 0.001f;

        public float XRotation
        {
            get { return xRot; }
            set { xRot = (value + 360) % 360; }
        }

        public float YRotation
        {
            get { return yRot; }
            set { yRot = (value + 360) % 360; }
        }

        public float ZRotation
        {
            get { return zRot; }
            set { zRot = (value + 360) % 360; }
        }

        public float XTranslation
        {
            get { return xTrans; }
            set { xTrans = value; }
        }

        public float YTranslation
        {
            get { return xTrans; }
            set { xTrans = value; }
        }

        public float ZTranslation
        {
            get { return zTrans; }
            set { zTrans = value; }
        }

        public Player()
        {
            //The models have been initialized for us, but we need to set their scaling, position, and rotation
            
            //the Nyan cat model is HUGE! Lets scale the model down a ton (100 times smaller) so it fits on screen
            ModelHelper.modelTwo_WorldMatrix =      //the result matrix
                ModelHelper.ScaleMatrix(            //the function call
                ModelHelper.modelTwo_WorldMatrix,   //the source matrix
                xSca, ySca, zSca                    //the x, y, and z scaling
                );

            //Now lets rotate nyan cat depending on how much we calculated. Also, lets get it horizontal
            ModelHelper.modelTwo_WorldMatrix =      //the result matrix
                ModelHelper.RotateMatrix(           //the functuion call
                ModelHelper.modelTwo_WorldMatrix,   //the source matrix
                xRot, yRot, zRot                    //the x, y, and z rotation
                );

            //Nyan can can get even more exciting! Lets make him move!
            ModelHelper.modelTwo_WorldMatrix =      //the result matrix
                ModelHelper.TranslateMatrix(        //the function call
                ModelHelper.modelTwo_WorldMatrix,   //the source matrix
                xTrans, yTrans, zTrans              //the x, y, and z translation
                );
        }

        public void Update()
        {
            ModelHelper.resetWorldMatrices();

            //Scale the NyanCat model to an appropriate size
            ModelHelper.modelTwo_WorldMatrix =      //the result matrix
                ModelHelper.ScaleMatrix(            //the function call
                ModelHelper.modelTwo_WorldMatrix,   //the source matrix
                xSca, ySca, zSca                    //the x, y, and z scaling
                );

            //Now lets rotate nyan cat depending on how much we calculated.
            ModelHelper.modelTwo_WorldMatrix =      //the result matrix
                ModelHelper.RotateMatrix(           //the functuion call
                ModelHelper.modelTwo_WorldMatrix,   //the source matrix
                xRot, yRot, zRot                    //the x, y, and z rotation
                );

            //Translate the NyanCat to the appropriate position
            ModelHelper.modelTwo_WorldMatrix =      //the result matrix
                ModelHelper.TranslateMatrix(        //the function call
                ModelHelper.modelTwo_WorldMatrix,   //the source matrix
                xTrans, yTrans, zTrans              //the x, y, and z translation
                );
        }

        public void MoveForward()
        {
            xTrans += 0.1f;
        }
    }
}
