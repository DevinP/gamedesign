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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;
        Effect effect;
        Song bgm;

        Texture2D wallTexture;

        Wallbox wallBox;

        Camera camera;

        //For now, how much are we rotating in each direction?
        //ROTATE THESE TO ROTATE NYAN!!
        float xRot = 270f;
        float yRot = 270f;
        float zRot = 0f;

        //I guess nyancat can translate?
        //MOVE THESE TO MOVE NYAN!
        float xTrans, yTrans, zTrans = 0f;
        float transDir = 1;

        //Nyan cat needs some scaling: too fat
        float xSca = 0.001f; 
        float ySca = 0.001f;
        float zSca = 0.001f;

        public Game1()
        {
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
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "Nyan Tron";

            device = graphics.GraphicsDevice;

            camera = new Camera(device);

            ModelHelper.LoadModels(Content);
            ModelHelper.resetWorldMatrices();

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
            effect = Content.Load<Effect>("effects");
            wallTexture = Content.Load<Texture2D>("wallTex");

            wallBox = new Wallbox(device, camera.ViewMatrix, camera.ProjectionMatrix, wallTexture);

            bgm = Content.Load<Song>("NyanCat");
            MediaPlayer.Play(bgm);
            MediaPlayer.IsRepeating = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }



        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            ProcessKeyboard(gameTime);

            ModelHelper.resetWorldMatrices();

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

            camera.Update();

            base.Update(gameTime);
        }

        private void ProcessKeyboard(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            // Allows the game to exit
            if (keyState.IsKeyDown(Keys.Escape))
                this.Exit();

            int xDiff = 0;
            int yDiff = 0; 
            int zDiff = 0;

            if (keyState.IsKeyDown(Keys.Down))
            {
                xDiff -= 3;
            }
            if (keyState.IsKeyDown(Keys.Up))
            {
                xDiff += 3;
            }
            if (keyState.IsKeyDown(Keys.W))
            {
                yDiff += 3;
            }
            if (keyState.IsKeyDown(Keys.S))
            {
                yDiff -= 3;
            }
            if (keyState.IsKeyDown(Keys.A))
            {
                zDiff -= 3;
            }
            if (keyState.IsKeyDown(Keys.D))
            {
                zDiff += 3;
            }

            xTrans += xDiff;
            yTrans += yDiff;
            zTrans += zDiff;

            camera.translatePosition(xDiff, yDiff, zDiff);
            camera.translateTarget(xDiff, yDiff, zDiff);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            /*RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            device.RasterizerState = rs;*/
           
            wallBox.Draw(camera.viewMatrix, camera.projectionMatrix);

            ModelHelper.drawModels(camera);

            base.Draw(gameTime);
        }

    }
}
