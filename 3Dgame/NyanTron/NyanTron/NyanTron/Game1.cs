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

        Texture2D titleScreen;

        bool gameStarted = false;

        Wallbox wallBox;        //The wallbox which will contain the player

        Camera camera;          //The camera object, used to determine what the player sees

        Player thePlayer;       //The player. Specifically, the player's NyanCat avatar

        SpriteFont scoreFont;
        SpriteFont endGameFont;

        public static List<Trail> trails;

        float trailDrop_Timer = 0f;
        const float TRAILDROP_LIMIT = 68f;

        float gamePause_Timer = 0f;
        const float GAMEPAUSE_LIMIT = 1500f;
        bool gamePaused = false;

        int playerScore = 0;

        const bool DRAWDEBUGBOXES = false;

        const int MAXTRAILS = 800;

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

            trails = new List<Trail>();
         



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

            scoreFont = Content.Load<SpriteFont>("scoreFont");
            endGameFont = Content.Load<SpriteFont>("endGameFont");

            titleScreen = Content.Load<Texture2D>("tron-nyan");

            DebugShapeRenderer.Initialize(device);


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

            if (!gameStarted)
            {

                #region Title Screen Mode

                // Allows the game to exit
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    this.Exit();

                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    gameStarted = true;

                #endregion

            }
            else
            {
                #region Gameplay Mode

                if (!gamePaused)
                {

                    //Process the input to the game
                    ProcessKeyboard(gameTime);

                    //Update the player's position and rotation
                    thePlayer.Update();

                    camera.Update(thePlayer);

                    trailDrop_Timer += (float)gameTime.ElapsedGameTime.Milliseconds;

                    if (trailDrop_Timer >= TRAILDROP_LIMIT)
                    {
                        dropTrail();
                        trailDrop_Timer = 0;
                        playerScore++;
                    }


                    if (!isPlayerWallCollision(thePlayer, wallBox) || isPlayerTrailCollision(thePlayer, trails))
                    {
                        thePlayer.Position = new Vector3(0, 0, 0);
                        thePlayer.Rotation = Quaternion.Identity;
                        
                        gamePaused = true;
                    }
                }
                else
                {
                    // Allows the game to exit
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        this.Exit();

                    gamePause_Timer += (float)gameTime.ElapsedGameTime.Milliseconds;

                    if (gamePause_Timer >= GAMEPAUSE_LIMIT)
                    {
                        gamePaused = false;
                        gamePause_Timer = 0;
                        trails = new List<Trail>();
                        playerScore = 0;
                    }
                }

                #endregion
            }

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

            if (keyState.IsKeyDown(Keys.D))
                leftRightRot -= TURNINGSPEED;
            if (keyState.IsKeyDown(Keys.A))
                leftRightRot += TURNINGSPEED;

            if (keyState.IsKeyDown(Keys.S))
                upDownRot -= TURNINGSPEED;
            if (keyState.IsKeyDown(Keys.W))
                upDownRot += TURNINGSPEED;

            if (keyState.IsKeyDown(Keys.Q))
                rollRot -= TURNINGSPEED;
            if (keyState.IsKeyDown(Keys.E))
                rollRot += TURNINGSPEED;

            Quaternion additionalRot = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, -1), leftRightRot) * Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), upDownRot) * Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), rollRot);

            thePlayer.Rotation *= additionalRot;
        }

        void dropTrail()
        {
            trails.Add(new Trail(thePlayer.Position, thePlayer.Rotation));

            if (trails.Count >= 800)
                trails.RemoveAt(0);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            /*RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            device.RasterizerState = rs;*/

            if (!gameStarted)
            {
                #region Title Screen Mode

                spriteBatch.Begin();

                spriteBatch.Draw(titleScreen, new Rectangle(0, 0, 1280, 720), Color.White);

                spriteBatch.DrawString(scoreFont, "W and S - Pitch Control", new Vector2(450, 10), Color.Honeydew);
                spriteBatch.DrawString(scoreFont, "A and D - Yaw Control", new Vector2(450, 40), Color.Honeydew);
                spriteBatch.DrawString(scoreFont, "Q and E - Roll Control", new Vector2(450, 70), Color.Honeydew);

                if (gameTime.TotalGameTime.Milliseconds % 1000 < 700)
                {
                    spriteBatch.DrawString(scoreFont, "Press space to begin", new Vector2(500, 500), Color.HotPink);
                }

                spriteBatch.End();

                #endregion
            }
            else
            {
                #region Gameplay mode
                wallBox.Draw(camera.viewMatrix, camera.projectionMatrix);

                ModelHelper.drawModels(camera);

                if (DRAWDEBUGBOXES)
                {
                    DebugShapeRenderer.AddBoundingBox(thePlayer.BoundingBox, Color.Aqua, 0f);

                    foreach (Trail currentTrail in trails)
                    {
                        DebugShapeRenderer.AddBoundingBox(currentTrail.BoundingBox, Color.Beige, 0f);
                    }

                    DebugShapeRenderer.Draw(gameTime, camera.viewMatrix, camera.projectionMatrix);
                }

                spriteBatch.Begin();

                spriteBatch.DrawString(scoreFont, "Esc - Exit", new Vector2(10, 10), Color.HotPink);
                spriteBatch.DrawString(scoreFont, "Score: " + playerScore, new Vector2(1090, 10), Color.HotPink);

                if (gamePaused)
                    spriteBatch.DrawString(endGameFont, "Y O U  C R A S H E D", new Vector2(350, 250), Color.Orange);

                spriteBatch.End();


                #endregion
            }

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime); 
        }

        private bool isPlayerWallCollision(Player thePlayer, Wallbox theBox)
        {
            //return theBox.BoundingBox.Intersects(thePlayer.BoundingBox);
            BoundingBox wallBoxBox = theBox.BoundingBox;
            return thePlayer.BoundingBox.Intersects(ref wallBoxBox);
        }

        private bool isPlayerTrailCollision(Player thePlayer, List<Trail> theTrails)
        {
            if(theTrails.Count <= 1)
                return false;

            for (int i = 0; i < theTrails.Count - 2; i++)
            {
                BoundingOrientedBox trailBox = theTrails[i].BoundingBox;
                if (thePlayer.BoundingBox.Intersects(ref trailBox))
                    return true;
            }

            return false;
        }
    }
}
