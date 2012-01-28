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

        static int SCREENWIDTH = 1280;
        static int SCREENHEIGHT = 720;
        public static int WYRMSEGS = 10;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D t2dTitleScreen;                          //The title screen for the game
        Texture2D t2dmainBackground;                       //The main background for the game
        Song bgm;                                          //The background music for the game
        Song bgm2;
        bool m_gameStarted = false;                        //Whether or not we are at the title screen
        SpriteFont titleFont;                              //The font used in the game for the title screen
        Vector2 vStartTitleTextLoc = new Vector2(440, 440);//The location for the additional title screen text
        SoundEffect roar;

        Texture2D t2dWyrmHead;                              //The sprite for the Wyrm head
        Texture2D t2dWyrmSeg;                               //The sprite for the Wyrm segments
        Texture2D t2dWyrmTail;                              //The sprite for the Wyrm tail

        Player thePlayer;                                   //The player of the game


        bool canRoar = true;
        bool canSwitchSongs = true;
        bool bgm1Playing = false;
        bool bgm2Playing = false;

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
            graphics.PreferredBackBufferHeight = SCREENHEIGHT;
            graphics.PreferredBackBufferWidth = SCREENWIDTH;
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

            t2dTitleScreen = Content.Load<Texture2D>(@"Textures\titleScreen");

            t2dmainBackground = Content.Load<Texture2D>(@"Textures\background");

            roar = Content.Load<SoundEffect>(@"Sounds\Predator Roar");

            bgm = Content.Load<Song>(@"Sounds\bgm");
            bgm2 = Content.Load<Song>(@"Sounds\bgm2");

            t2dWyrmHead = Content.Load<Texture2D>(@"Textures\wyrmHeadRed");
            t2dWyrmSeg = Content.Load<Texture2D>(@"Textures\wyrmSegRed");
            t2dWyrmTail = Content.Load<Texture2D>(@"Textures\wyrmTailRed");

            //Add the wyrm head segment texture to the wyrm textures list
            List<Texture2D> wyrmTextures = new List<Texture2D>();
            wyrmTextures.Add(t2dWyrmHead);
  
            //Add on the wyrm segment textures
            //We want to subtract three from the total segments since the head and tail are not this texture
            for (int i = 0; i < WYRMSEGS - 2; i++)
            {
                wyrmTextures.Add(t2dWyrmSeg);
            }

            //Lastly, add the wyrm tail texture
            wyrmTextures.Add(t2dWyrmTail);

            thePlayer = new Player(0, wyrmTextures);

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(bgm2);
            bgm2Playing = true;
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

            if (keystate.IsKeyDown(Keys.LeftShift) && canSwitchSongs)
            {
                if (bgm1Playing)
                {
                    MediaPlayer.Play(bgm2);
                    bgm2Playing = true;
                    bgm1Playing = false;
                }
                else if (bgm2Playing)
                {
                    MediaPlayer.Play(bgm);
                    bgm1Playing = true;
                    bgm2Playing = false;
                }

                canSwitchSongs = false;
            }
            else if (keystate.IsKeyUp(Keys.LeftShift) && !canSwitchSongs)
            {
                canSwitchSongs = true;
            }

            // TODO: Add your update logic here

            // Get elapsed game time since last call to Update
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (m_gameStarted)
            {
                #region GamePlay Mode (m_gameStarted == true)

                thePlayer.Update(gameTime, keystate);

                //Make it so the player can't move off the screen
                for (int i = 0; i < WYRMSEGS; i++)
                {
                    if (thePlayer.theWyrm.l_segments[i].X < 50)
                        thePlayer.theWyrm.l_segments[i].X = 50;

                    if (thePlayer.theWyrm.l_segments[i].X > SCREENWIDTH - 50)
                        thePlayer.theWyrm.l_segments[i].X = (float)SCREENWIDTH - 50;

                    if(thePlayer.theWyrm.l_segments[i].Y < 0)
                        thePlayer.theWyrm.l_segments[i].Y = 0;

                    if (thePlayer.theWyrm.l_segments[i].Y > SCREENHEIGHT - 100)
                        thePlayer.theWyrm.l_segments[i].Y = (float)SCREENHEIGHT - 100;
                }

                if (keystate.IsKeyDown(Keys.LeftControl) && canRoar)
                {
                    roar.Play();
                    canRoar = false;
                }
                else if (keystate.IsKeyUp(Keys.LeftControl) && !canRoar)
                {
                    canRoar = true;
                }

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

                spriteBatch.Draw(t2dmainBackground, new Rectangle(0, 0, SCREENWIDTH, SCREENHEIGHT), Color.White);

                thePlayer.Draw(spriteBatch);

                #endregion
            }
            else
            {
                #region Title Screen Mode (m_gameStarted == false)

                spriteBatch.Draw(t2dTitleScreen, new Rectangle(0, 0, SCREENWIDTH, SCREENHEIGHT), Color.White);

                if (gameTime.TotalGameTime.Milliseconds % 1000 < 700)
                {
                    spriteBatch.DrawString(titleFont, "Press Spacebar to BEGIN YOUR FEAST", vStartTitleTextLoc, Color.OrangeRed);
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
        }
    }
}
