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
        Texture2D t2dtransparentBlack;                     //A partially transparent texture to draw over the game
        Texture2D t2dupgradeBox;                           //A box to put upgrade messages in
        Texture2D t2dupgradeArrow;                         //Arrow to indicate which upgrade the player will select
        Song bgm;                                          //The background music for the game
        Song bgm2;
        Song bgm3;
        bool m_gameStarted = false;                        //Whether or not we are at the title screen
        SpriteFont titleFont;                              //The font used in the game for the title screen
        SpriteFont upgradeFont;                            //The font used in the game for the upgrade screen
        Vector2 vStartTitleTextLoc = new Vector2(440, 440);//The location for the additional title screen text
        SoundEffect roar;

        Texture2D t2dWyrmHead;                              //The sprite for the Wyrm head
        Texture2D t2dWyrmSeg;                               //The sprite for the Wyrm segments
        Texture2D t2dWyrmTail;                              //The sprite for the Wyrm tail
        
        Texture2D t2dbackground;                            //The background sprite
        Texture2D t2dforeground;                            //The foreground sprite (part of the background)
        Player thePlayer;                            //The player of the game

        Background theBackground;

        //The game's random numbers
        public static Random m_random;


        bool canRoar = true;
        bool canSwitchSongs = true;
        bool bgm1Playing = false;
        bool bgm2Playing = false;
        bool bgm3Playing = false;

        bool upgradeMode = false;
        bool upgradeModeCanSwitch = true;
        float upgradeArrowDir = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            m_random = new Random();
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

            titleFont = Content.Load<SpriteFont>(@"Fonts\Title");
            upgradeFont = Content.Load<SpriteFont>(@"Fonts\Upgrade");

            t2dTitleScreen = Content.Load<Texture2D>(@"Textures\titleScreen");
            t2dtransparentBlack = Content.Load<Texture2D>(@"Textures\transparentBlack");
            t2dupgradeBox = Content.Load<Texture2D>(@"Textures\wordbubble");
            t2dupgradeArrow = Content.Load<Texture2D>(@"Textures\arrow");

            roar = Content.Load<SoundEffect>(@"Sounds\Predator Roar");

            bgm = Content.Load<Song>(@"Sounds\bgm");
            bgm2 = Content.Load<Song>(@"Sounds\bgm2");
            bgm3 = Content.Load<Song>(@"Sounds\bgm3");

            t2dWyrmHead = Content.Load<Texture2D>(@"Textures\wyrmHeadRed");
            t2dWyrmSeg = Content.Load<Texture2D>(@"Textures\wyrmSegRed");
            t2dWyrmTail = Content.Load<Texture2D>(@"Textures\wyrmTailRed");

            t2dbackground = Content.Load<Texture2D>(@"Textures\background");
            t2dforeground = Content.Load<Texture2D>(@"Textures\foreground");

            //Add the wyrm head segment texture to the wyrm textures list
            List<Texture2D> wyrmTextures = new List<Texture2D>();
            wyrmTextures.Add(t2dWyrmHead);
  
            //Add on the wyrm segment textures
            //We want to subtract two from the total segments since the head and tail are not this texture
            //derp
            for (int i = 0; i < WYRMSEGS - 2; i++)
            {
                wyrmTextures.Add(t2dWyrmSeg);
            }

            //Lastly, add the wyrm tail texture
            wyrmTextures.Add(t2dWyrmTail);

            theBackground = new Background(t2dbackground, t2dforeground);
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

            #region SongSwitching (Code to handle the switching of the song with left shift)

            if (keystate.IsKeyDown(Keys.RightShift) && canSwitchSongs)
            {
                if (bgm1Playing)
                {
                    MediaPlayer.Play(bgm2);
                    bgm1Playing = false;
                    bgm2Playing = true;
                    bgm3Playing = false;
                }
                else if (bgm2Playing)
                {
                    MediaPlayer.Play(bgm3);
                    bgm1Playing = false;
                    bgm2Playing = false;
                    bgm3Playing = true;
                }
                else if (bgm3Playing)
                {
                    MediaPlayer.Play(bgm);
                    bgm1Playing = true;
                    bgm2Playing = false;
                    bgm3Playing = false;
                }

                canSwitchSongs = false;
            }
            else if (keystate.IsKeyUp(Keys.RightShift) && !canSwitchSongs)
            {
                canSwitchSongs = true;
            }

            #endregion

            // Get elapsed game time since last call to Update
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (m_gameStarted)
            {
                #region GamePlay Mode (m_gameStarted == true)

                if (keystate.IsKeyDown(Keys.U) && upgradeModeCanSwitch)
                {
                    //Toggle the upgrade mode
                    if (upgradeMode)
                        upgradeMode = false;
                    else if (!upgradeMode)
                        upgradeMode = true;

                    upgradeModeCanSwitch = false;
                }
                else if (keystate.IsKeyUp(Keys.U) && !upgradeModeCanSwitch)
                {
                    upgradeModeCanSwitch = true;
                }

                if (upgradeMode)
                {
                    #region Upgrade Mode (upgradeMode == true)

                    if (keystate.IsKeyDown(Keys.Left))
                    {
                        upgradeArrowDir = (float)Math.PI;
                    }
                    else if (keystate.IsKeyDown(Keys.Down))
                    {
                        upgradeArrowDir = (float)(Math.PI / 2);
                    }
                    else if (keystate.IsKeyDown(Keys.Right))
                    {
                        upgradeArrowDir = 0;
                    }
                    else if (keystate.IsKeyDown(Keys.Up))
                    {
                        upgradeArrowDir = (float)((3 * Math.PI) / 2);
                    }

                    #endregion
                }
                else
                {
                    #region Play Mode (upgradeMode == false)

                    thePlayer.Update(gameTime, keystate);

                    theBackground.Update();
                   // Console.WriteLine(Background.trails.Count);


                     //For debugging whether the wyrm is in the ground or the air.
                    /*if (theBackground.wyrmGrounded)
                        System.Diagnostics.Debug.WriteLine("grounded");
                    else
                        System.Diagnostics.Debug.WriteLine("airborne");*/
                    


                    //Make it so the player can't move off the screen
                    for (int i = 0; i < WYRMSEGS; i++)
                    {
                        if (thePlayer.theWyrm.l_segments[i].X < 25)
                            thePlayer.theWyrm.l_segments[i].X = 25;

                        if (thePlayer.theWyrm.l_segments[i].X > SCREENWIDTH - 25)
                            thePlayer.theWyrm.l_segments[i].X = (float)SCREENWIDTH - 25;

                       // if (thePlayer.theWyrm.l_segments[i].Y < 0)
                        //    thePlayer.theWyrm.l_segments[i].Y = 0;

                        if (thePlayer.theWyrm.l_segments[i].Y > SCREENHEIGHT - 50)
                            thePlayer.theWyrm.l_segments[i].Y = (float)SCREENHEIGHT - 50;
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

                //spriteBatch.Draw(t2dmainBackground, new Rectangle(0, 0, SCREENWIDTH, SCREENHEIGHT), Color.White);
                theBackground.Draw(spriteBatch);
                thePlayer.Draw(spriteBatch);

                if (upgradeMode)
                {
                    //Draw the partly-transparent black layer over the screen to darken it
                    spriteBatch.Draw(t2dtransparentBlack, new Rectangle(0, 0, SCREENWIDTH, SCREENHEIGHT), Color.White);

                    //Draw each upgrade box
                    spriteBatch.Draw(t2dupgradeBox, new Rectangle(540, 110, 200, 100), Color.White);
                    spriteBatch.DrawString(upgradeFont, "Metabolism Boost", new Vector2(560, 130), Color.Red);
                    spriteBatch.DrawString(upgradeFont, "(Heal Over Time)", new Vector2(560, 155), Color.Red);

                    spriteBatch.Draw(t2dupgradeBox, new Rectangle(540, 440, 200, 100), Color.White);
                    spriteBatch.DrawString(upgradeFont, "Muscle Vibration", new Vector2(560, 460), Color.Red);
                    spriteBatch.DrawString(upgradeFont, "(Dig Speed)", new Vector2(560, 485), Color.Red);

                    spriteBatch.Draw(t2dupgradeBox, new Rectangle(755, 270, 200, 100), Color.White);
                    spriteBatch.DrawString(upgradeFont, "Fat Tissue", new Vector2(775, 290), Color.Red);
                    spriteBatch.DrawString(upgradeFont, "(Max Health)", new Vector2(775, 315), Color.Red);

                    spriteBatch.Draw(t2dupgradeBox, new Rectangle(325, 270, 200, 100), Color.White);
                    spriteBatch.DrawString(upgradeFont, "Muscle Coiling", new Vector2(345, 290), Color.Red);
                    spriteBatch.DrawString(upgradeFont, "(Speed Burst)", new Vector2(345, 315), Color.Red);

                    //Draw the arrow which points to the currently selected box
                    spriteBatch.Draw(t2dupgradeArrow, new Rectangle(640, 325, 112, 51), null, Color.White, upgradeArrowDir, new Vector2(0, 25.5f), SpriteEffects.None, 0);
                }

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
