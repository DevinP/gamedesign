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

        Texture2D wallTexture;  //The texture to be applied to the wallbox

        Wallbox wallBox;        //The wallbox which will contain the player

        Camera camera;          //The camera object, used to determine what the player sees

        Player thePlayer;       //The player. Specifically, the player's NyanCat avatar

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

            //Alias the graphics card to be an easily accessible name
            device = graphics.GraphicsDevice;

            camera = new Camera(device);

            //Initialize the models
            ModelHelper.LoadModels(Content);
            ModelHelper.resetWorldMatrices();

            //Make a new player
            thePlayer = new Player();
            
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

            //Make a new wallbox
            wallBox = new Wallbox(device, camera.ViewMatrix, camera.ProjectionMatrix, wallTexture);

            //Play the iconic Nyan Cat music!
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
            //Process the input to the game
            ProcessKeyboard(gameTime);

            //Update the player's position and rotation
            thePlayer.Update();

            camera.Update(thePlayer);

            base.Update(gameTime);
        }

        private void ProcessKeyboard(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            float leftRightRot = 0;
            float upDownRot = 0;
            float rollRot = 0;

            const float TURNINGSPEED = 0.03f;

            // Allows the game to exit
            if (keyState.IsKeyDown(Keys.Escape))
                this.Exit();

            if (keyState.IsKeyDown(Keys.Right))
                leftRightRot -= TURNINGSPEED;
            if (keyState.IsKeyDown(Keys.Left))
                leftRightRot += TURNINGSPEED;

            if (keyState.IsKeyDown(Keys.Down))
                upDownRot -= TURNINGSPEED;
            if (keyState.IsKeyDown(Keys.Up))
                upDownRot += TURNINGSPEED;

            if (keyState.IsKeyDown(Keys.A))
                rollRot -= TURNINGSPEED;
            if (keyState.IsKeyDown(Keys.D))
                rollRot += TURNINGSPEED;

            Quaternion additionalRot = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, -1), leftRightRot) * Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), upDownRot) * Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), rollRot);

            thePlayer.Rotation *= additionalRot;
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

        //Source: http://www.toymaker.info/Games/XNA/html/xna_model_collisions.html
        //The first argument as an axis-aligned bounding box
        //The second argument is the wallbox we use in-game
        private bool CheckWallCollision(BoundingBox AABB, Wallbox theBox)
        {
            // firstly extract the corners of the AABB into 8 vectors - 1 for each corner
            Vector3[] obb=new Vector3[8];
            AABB.GetCorners(obb);

            // Transform the vectors by the model's world matrix
            Vector3.Transform(obb, ref ModelHelper.modelTwo_WorldMatrix, obb);

            // create an AABB in world space form the OBB in world space
            BoundingBox worldAABBModel = BoundingBox.CreateFromPoints(obb);

            BoundingBox wallBoxAABB = theBox.BoundingBox;

            return worldAABBModel.Intersects(wallBoxAABB);
        }
    }
}
