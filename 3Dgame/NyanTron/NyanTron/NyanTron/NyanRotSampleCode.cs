/*using System;
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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //Here, we make easy references to the devices and managers
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;

        //Make a camera object that we can control in the future
        Camera camera;



        //For now, how much are we rotating in each direction?
        //ROTATE THESE TO ROTATE NYAN!!
        float xRot, yRot, zRot = 0f;

        //I guess nyancat can translate?
        //MOVE THESE TO MOVE NYAN!
        float xTrans, yTrans, zTrans = 0f;
        float transDir = 1;

        //Nyan cat needs some scaling: too fat
        float xSca, ySca, zSca = 0f;


        public Game1()
        {
            //This stuff looks dangerous. Lets leave it alone...
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Refer to the correct device
            device = graphics.GraphicsDevice;

            //Lets make the model's world matrices the default values
            ModelHelper.resetWorldMatrices();

            camera = new Camera(device);

            //Leave this alone...
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Tell the ModelHelper to load its models
            ModelHelper.LoadModels(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            //OH GOODNESS PLEASE DONT TOUCH THIS
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Instruct the camera to update itself
            camera.Update();

            #region Keyboard nonsense
            KeyboardState keyBoardState = Keyboard.GetState();

            //Rotate Cube along its Up Vector
            if (keyBoardState.IsKeyDown(Keys.Up))
            {
                camera.translatePosition(0, 0, 1);
            }
            if (keyBoardState.IsKeyDown(Keys.Down))
            {
                camera.translatePosition(0, 0, -1);
            }

            #endregion


            //Lets figure out how much to change nyan
            updateNyan(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //This sets the drawing view up
            setupDraw();

            //Ask modelhelper to reset our world matrices
            ModelHelper.resetWorldMatrices();

            //Now we draw all the objects we want to draw
            DrawObjects();


        }

        /// <summary>
        /// This prepares/instructs objects to draw themselves
        /// </summary>
        private void DrawObjects()
        {

            #region Perhaps these should be in their own helper method?

            //the Nyan cat model is HUGE! Lets scale the model down a ton (100 times smaller) so it fits on screen
            ModelHelper.modelTwo_WorldMatrix =      //the result matrix
                ModelHelper.ScaleMatrix(            //the function call
                ModelHelper.modelTwo_WorldMatrix,   //the source matrix
                xSca, ySca, zSca                    //the x, y, and z scaling
                );

            //Now lets rotate nyan cat depending on how much we calculated. Also, lets get it horizontal
            ModelHelper.modelTwo_WorldMatrix =      //the resutl matrix
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

            #endregion

            //Draw the models onto the camera object
            ModelHelper.drawModels(camera);
        }

        /// <summary>
        /// This is like the biggest waste of space imaginable. Just clears the screen...
        /// </summary>
        private void setupDraw()
        {
            //Cornflower blue is such an ugly color
            GraphicsDevice.Clear(Color.CornflowerBlue);
        }



        /// <summary>
        /// Nyancat is dynamic! (loosely)
        /// </summary>
        private void updateNyan(GameTime gametime)
        {
            //Rotate the rotation
            updateRotation(gametime);
            updateTranslation(gametime);
            updateScaling(gametime);
        }

        /// <summary>
        /// This updates the rotation arbitrarily assigned by me
        /// </summary>
        private void updateRotation(GameTime gametime)
        {
            //All that we are going to do is rotate in the y direction by one degree....
            yRot = (yRot + 1f) % 360;

            //Make the cat stand up
            xRot = 270f;
            zRot = 0;
        }

        /// <summary>
        /// This updates the translation arbitrarily assigned by me
        /// </summary>
        private void updateTranslation(GameTime gametime)
        {
            //X and Z dont change...
            xTrans = 0f;
            zTrans = 0f;

            //Moves left.... then right. Very intense stuff here
            if (yTrans > 25)
            {
                transDir = -1;
            }
            if (yTrans < -25)
            {
                transDir = 1;
            }

            yTrans += transDir / 2;
        }

        /// <summary>
        /// This updates the rotation arbitrarily assigned by me
        /// </summary>
        private void updateScaling(GameTime gametime)
        {
            //Nyan cat just needs to get smaller
            xSca = 0.01f;
            ySca = 0.01f;
            zSca = 0.01f;
        }

    }
}*/
