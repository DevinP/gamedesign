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

namespace DreadWyrm
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D t2dTitleScreen;                          //The title screen for the game
        Song bgm;                                          //The background music for the game
        bool m_gameStarted = false;                        //Whether or not we are at the title screen
        SpriteFont titleFont;                              //The font used in the game for the title screen
        Vector2 vStartTitleTextLoc = new Vector2(250, 50); //The location for the title screen text
        SpriteFont title2Font;                             //Additional font for the title screen
        Vector2 vStartTitle2TextLoc = new Vector2(440, 450);//The location for the additional title screen text
        SoundEffect roar;

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
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.ApplyChanges();

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

            // TODO: use this.Content to load your game content here
            titleFont = Content.Load<SpriteFont>(@"Fonts\Title");
            title2Font = Content.Load<SpriteFont>(@"Fonts\Title2");

            t2dTitleScreen = Content.Load<Texture2D>(@"Textures\titlescreen");

            roar = Content.Load<SoundEffect>(@"Sounds\Predator Roar");

            bgm = Content.Load<Song>(@"Sounds\bgm");

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(bgm);
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
            // Store values for the Keyboard so we aren't
            // Querying them multiple times per Update
            KeyboardState keystate = Keyboard.GetState();

            // If the Escape Key is pressed, exit the game.
            if (keystate.IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here

            // Get elapsed game time since last call to Update
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (m_gameStarted)
            {
                #region GamePlay Mode (m_gameStarted == true)
                #endregion
            }
            else
            {
                #region Title Screen Mode (m_gameStarted == false)

                if (keystate.IsKeyDown(Keys.Space))
                {
                    startNewGame();
                }

                #endregion
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            // Start a SpriteBatch.Begin which will be used
            // by all of our drawing code.
            spriteBatch.Begin();

            if (m_gameStarted)
            {
                #region Game Play Mode (m_gameStarted == true)
                #endregion
            }
            else
            {
                #region Title Screen Mode (m_gameStarted == false)

                spriteBatch.Draw(t2dTitleScreen, new Rectangle(0, 0, 1280, 720), Color.White);
                spriteBatch.DrawString(titleFont, "D R E A D   W Y R M", vStartTitleTextLoc, Color.Black);

                if (gameTime.TotalGameTime.Milliseconds % 1000 < 700)
                {
                    spriteBatch.DrawString(title2Font, "Press Spacebar to BEGIN YOUR FEAST", vStartTitle2TextLoc, Color.Black);
                }

                #endregion
            }
            //Close the SpriteBatch
            spriteBatch.End();

            base.Draw(gameTime);
        }

        void startNewGame()
        {
            m_gameStarted = true;
            roar.Play();
        }
    }
}
