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
        public static int WYRMSEGS = 10 ;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D t2dTitleScreen;                          //The title screen for the game
        Texture2D t2dtransparentBlack;                     //A partially transparent texture to draw over the game
        Texture2D t2dupgradeBox;                           //A box to put upgrade messages in
        Texture2D t2dupgradeArrow;                         //Arrow to indicate which upgrade the player will select
        List<Texture2D> preyTextures;                      //The textures used by the prey
        Song bgm;                                          //The background music for the game
        Song bgm2;
        Song bgm3;
        bool m_gameStarted = false;                        //Whether or not we are at the title screen
        SpriteFont titleFont;                              //The font used in the game for the title screen
        SpriteFont upgradeFont;                            //The font used in the game for the upgrade screen
        SpriteFont scoreFont;                              //The font used to dispaly the meat score
        Vector2 vStartTitleTextLoc = new Vector2(440, 440);//The location for the additional title screen text
        SoundEffect roar;
        SoundEffect chomp;

        Texture2D t2dWyrmHead;                              //The sprite for the Wyrm head
        Texture2D t2dWyrmSeg;                               //The sprite for the Wyrm segments
        Texture2D t2dWyrmTail;                              //The sprite for the Wyrm tail
        Texture2D healthBase;                               //The sprite for the health base base
        Texture2D health;                                   //The sprite for the health bar
        Texture2D stamina;                                  //The sprite for the stamina bar

        Texture2D bulletTexture;                            //The sprite for the bullets used by enemies

        Texture2D t2dbackground;                            //The background sprite
        Texture2D t2dforeground;                            //The foreground sprite (part of the background)
        Player thePlayer;                                   //The player of the game
        List<Prey> prey;                                    //The edible animals on screen

        Background theBackground;

        //The game's random numbers
        public static Random m_random;

        //A static list of bullets being fired by enemies in-game
        public static List<Bullet> bullets;

        bool canRoar = true;
        bool canSwitchSongs = true;
        bool bgm1Playing = false;
        bool bgm2Playing = false;
        bool bgm3Playing = false;

        //Upgrade variables
        bool upgradeMode = false;
        bool upgradeModeCanSwitch = true;
        bool upgraded = false;
        float upgradeArrowDir = 0;
        int healCost = 1;
        int maxHealthCost = 1;
        int digSpeedCost = 1;
        int speedBurstCost = 1;
        const int HEALTHMAX_MAX = 1000;
        const int SPEEDMAX = 10;
        const float DIGSPEED_UPGRADE_INCR = 0.1f;
        const int MAXHEALTH_UPGRADE_INCR = 10;
        const int SPEEDBURST_UPGRADE_INCR = 1;
        const int DIGSPEED_COST_INCR = 1;
        const int MAXHEALTH_COST_INCR = 1;
        const int SPEEDBURST_COST_INCR = 1;
        
        //Implementing speed boost
        const float WYRM_BOOST_FACTOR = 2; //Multiplies the max speed of the wyrm when boosting

        //Constant ints to access the prey texture list
        const int GIRAFFE = 0;
        const int ELEPHANT = 1;
        const int UNARMEDHUMAN = 2;
        const int SOLDIER = 3;

        //Magical constants
        const int WYRMHEAD_CENTER_NUMBER = 10; //This number is a magic number
        const int QUARTER_OF_WYRMHEAD_SPRITEHEIGHT = 20;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            m_random = new Random();
            bullets = new List<Bullet>();
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
            scoreFont = Content.Load<SpriteFont>(@"Fonts\scoreFont");

            t2dTitleScreen = Content.Load<Texture2D>(@"Textures\titleScreen");
            t2dtransparentBlack = Content.Load<Texture2D>(@"Textures\transparentBlack");
            t2dupgradeBox = Content.Load<Texture2D>(@"Textures\wordbubble");
            t2dupgradeArrow = Content.Load<Texture2D>(@"Textures\arrow");

            roar = Content.Load<SoundEffect>(@"Sounds\Predator Roar");
            chomp = Content.Load<SoundEffect>(@"Sounds\aud_chomp");

            bgm = Content.Load<Song>(@"Sounds\bgm");
            bgm2 = Content.Load<Song>(@"Sounds\bgm2");
            bgm3 = Content.Load<Song>(@"Sounds\bgm3");

            t2dWyrmHead = Content.Load<Texture2D>(@"Textures\wyrmHeadRed");
            t2dWyrmSeg = Content.Load<Texture2D>(@"Textures\wyrmSegRed");
            t2dWyrmTail = Content.Load<Texture2D>(@"Textures\wyrmTailRed");

            healthBase = Content.Load<Texture2D>(@"Textures\hb_red");
            health = Content.Load<Texture2D>(@"Textures\hb_green");
            stamina = Content.Load<Texture2D>(@"Textures\hb_yellow");

            bulletTexture = Content.Load<Texture2D>(@"Textures\bullet");

            t2dbackground = Content.Load<Texture2D>(@"Textures\background");
            t2dforeground = Content.Load<Texture2D>(@"Textures\foreground");

            preyTextures = new List<Texture2D>();
            preyTextures.Add(Content.Load<Texture2D>(@"Textures\giraffe"));
            preyTextures.Add(Content.Load<Texture2D>(@"Textures\elephant"));
            preyTextures.Add(Content.Load<Texture2D>(@"Textures\unarmed"));
            preyTextures.Add(Content.Load<Texture2D>(@"Textures\soldier"));

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
            thePlayer = new Player(0, wyrmTextures, scoreFont, healthBase, health, stamina);

            prey = new List<Prey>();

            //Add some giraffes...
            for (int i = 0; i < 0; i++)
            {
                prey.Add(new Animal(m_random.Next(20, 1050), 100, preyTextures[GIRAFFE], 4, 95, 102, 94, 30, thePlayer.theWyrm, false, 1191, 97));
            }

            //...and some elephants
            for (int i = 0; i < 0; i++)
            {
                prey.Add(new Animal(m_random.Next(20, 1050), 100, preyTextures[ELEPHANT], 6, 71, 93, 70, 29, thePlayer.theWyrm, false, 4990, 73));
            }

            //...and humans!
            for (int i = 0; i < 0; i++)
            {
                prey.Add(new Animal(m_random.Next(20, 1050), 100, preyTextures[UNARMEDHUMAN], 4, 24, 21, 23, 6, thePlayer.theWyrm, true, 80, 25));
            }

            //...and MORE humans (always so many humans). These are armed
            for (int i = 0; i < 100; i++)
            {
                prey.Add(new SoldierHuman(m_random.Next(20, 1050), 100, preyTextures[SOLDIER], 6, 25, 20, 24, 7, thePlayer.theWyrm, 80, 26, 52, 78, bulletTexture));
            }

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
                        upgradeArrowDir = (float)Math.PI; //Speed burst
                    }
                    else if (keystate.IsKeyDown(Keys.Down))
                    {
                        upgradeArrowDir = (float)(Math.PI / 2); //Dig speed
                    }
                    else if (keystate.IsKeyDown(Keys.Right))
                    {
                        upgradeArrowDir = 0; //Max health
                    }
                    else if (keystate.IsKeyDown(Keys.Up))
                    {
                        upgradeArrowDir = (float)((3 * Math.PI) / 2); //Health regen
                    }

                    if (keystate.IsKeyDown(Keys.Enter) && !upgraded)
                    {
                        upgraded = true;

                        if (upgradeArrowDir == (float)Math.PI) //Speed burst
                        {
                            if (!(thePlayer.Meat < speedBurstCost))
                            {
                                thePlayer.MaxStamina += SPEEDBURST_UPGRADE_INCR;
                                thePlayer.Meat -= speedBurstCost;
                                speedBurstCost += SPEEDBURST_COST_INCR;
                            }
                        }

                        else if (upgradeArrowDir == (float)(Math.PI / 2)) //Dig Speed
                        {
                            if (!(thePlayer.Meat < digSpeedCost))
                            {
                                if ((thePlayer.theWyrm.HeadSpeedMax + DIGSPEED_UPGRADE_INCR) >= SPEEDMAX)
                                {
                                    thePlayer.theWyrm.HeadSpeedMax = SPEEDMAX;
                                }
                                else
                                {
                                    thePlayer.theWyrm.HeadSpeedMax += DIGSPEED_UPGRADE_INCR;
                                    thePlayer.theWyrm.HeadSpeedNormalMax += DIGSPEED_UPGRADE_INCR;
                                    thePlayer.theWyrm.HeadSpeedBoostMax += DIGSPEED_UPGRADE_INCR * WYRM_BOOST_FACTOR;
                                    thePlayer.Meat -= digSpeedCost;
                                    digSpeedCost += DIGSPEED_COST_INCR;
                                }
                            }

                        }
                        else if (upgradeArrowDir == 0) //Max Health
                        {
                            if(!(thePlayer.Meat < maxHealthCost) && (thePlayer.HealthMax + MAXHEALTH_UPGRADE_INCR <= HEALTHMAX_MAX))
                            {
                                thePlayer.HealthMax += MAXHEALTH_UPGRADE_INCR;
                                thePlayer.Meat -= maxHealthCost;
                                maxHealthCost += MAXHEALTH_COST_INCR;
                            }

                        }
                        else if (upgradeArrowDir == (float)((3 * Math.PI) / 2)) //Health Regen
                        {

                        }
                    }
                    else if (keystate.IsKeyUp(Keys.Enter))
                        upgraded = false;

                    #endregion
                }
                else
                {
                    #region Play Mode (upgradeMode == false)

                    theBackground.Update();

                    checkEat();

                    for (int i = 0; i < prey.Count; i++)
                    {
                        prey[i].Update(gameTime);
                    }

                    for (int i = 0; i < bullets.Count; i++)
                    {
                        bullets[i].Update(gameTime);
                    }

                    thePlayer.Update(gameTime, keystate);

                    checkBullets();

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

                for (int i = 0; i < prey.Count; i++)
                {
                    prey[i].Draw(spriteBatch);
                }

                thePlayer.Draw(spriteBatch);

                for (int i = 0; i < bullets.Count; i++)
                {
                    bullets[i].Draw(spriteBatch);
                }

                if (prey.Count == 0)
                {
                    spriteBatch.DrawString(titleFont, "YOU ATE ALL THE THINGS", new Vector2(500, 150), Color.Red);
                }

                if (upgradeMode)
                {
                    //Draw the partly-transparent black layer over the screen to darken it
                    spriteBatch.Draw(t2dtransparentBlack, new Rectangle(0, 0, SCREENWIDTH, SCREENHEIGHT), Color.White);

                    //Draw each upgrade box
                    spriteBatch.Draw(t2dupgradeBox, new Rectangle(540, 110, 200, 100), Color.White);
                    spriteBatch.DrawString(upgradeFont, "Metabolism Boost", new Vector2(560, 130), Color.Red);
                    spriteBatch.DrawString(upgradeFont, "(Heal Over Time)", new Vector2(560, 155), Color.Red);
                    spriteBatch.DrawString(upgradeFont, "Cost: " + healCost + " KG", new Vector2(590, 90), Color.Red);

                    spriteBatch.Draw(t2dupgradeBox, new Rectangle(540, 440, 200, 100), Color.White);
                    spriteBatch.DrawString(upgradeFont, "Muscle Vibration", new Vector2(560, 460), Color.Red);
                    spriteBatch.DrawString(upgradeFont, "(Dig Speed)", new Vector2(560, 485), Color.Red);
                    //spriteBatch.DrawString(upgradeFont, "Cost: " + digSpeedCost + " KG", new Vector2(590, 537), Color.Red);
                    if (thePlayer.theWyrm.HeadSpeedMax >= SPEEDMAX)
                        spriteBatch.DrawString(upgradeFont, "Maximum Upgrade Reached", new Vector2(520, 537), Color.Red);
                    else
                        spriteBatch.DrawString(upgradeFont, "Cost: " + digSpeedCost + " KG", new Vector2(590, 537), Color.Red);


                    spriteBatch.Draw(t2dupgradeBox, new Rectangle(755, 270, 200, 100), Color.White);
                    spriteBatch.DrawString(upgradeFont, "Fat Tissue", new Vector2(775, 290), Color.Red);
                    spriteBatch.DrawString(upgradeFont, "(Max Health)", new Vector2(775, 315), Color.Red);
                    if (thePlayer.HealthMax >= HEALTHMAX_MAX)
                    {
                        spriteBatch.DrawString(upgradeFont, "Maximum Upgrade Reached", new Vector2(735, 250), Color.Red);
                    }
                    else
                    {
                        spriteBatch.DrawString(upgradeFont, "Cost: " + maxHealthCost + " KG", new Vector2(805, 250), Color.Red);
                    }

                    spriteBatch.Draw(t2dupgradeBox, new Rectangle(325, 270, 200, 100), Color.White);
                    spriteBatch.DrawString(upgradeFont, "Muscle Coiling", new Vector2(345, 290), Color.Red);
                    spriteBatch.DrawString(upgradeFont, "(Speed Burst)", new Vector2(345, 315), Color.Red);
                    spriteBatch.DrawString(upgradeFont, "Cost: " + speedBurstCost + " KG", new Vector2(375, 250), Color.Red);

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

        bool isColliding(int x1, int y1, float r1, int x2, int y2, float r2)
        {
            return(Math.Pow((r1 + r2), 2) >= Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2),2));
        }

        void checkEat()
        {
            for (int i = 0; i < prey.Count; i++)
            {
                //Do circular collision detection
                if (isColliding((int)(thePlayer.theWyrm.l_segments[0].X - WYRMHEAD_CENTER_NUMBER * Math.Cos(thePlayer.theWyrm.HeadDirection)), 
                    (int)(thePlayer.theWyrm.l_segments[0].Y + QUARTER_OF_WYRMHEAD_SPRITEHEIGHT - WYRMHEAD_CENTER_NUMBER*Math.Sin(thePlayer.theWyrm.HeadDirection)),
                    thePlayer.theWyrm.eatRadius,
                    (int)prey[i].xPosistion, prey[i].yPosition, prey[i].boundingradius))
                {
                    thePlayer.Meat = thePlayer.Meat + prey[i].meatReward;
                    prey.RemoveAt(i);
                    chomp.Play();
                }
            }
            
        }

        void checkBullets()
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                if (bullets[i].xPosistion >= Background.SCREENWIDTH || bullets[i].xPosistion <= 0 || bullets[i].yPosition <= 0)
                {
                    bullets.RemoveAt(i);
                    continue;
                }

                if (Background.checkIsGrounded(bullets[i].xPosistion, bullets[i].yPosition))
                {
                    bullets.RemoveAt(i);
                    continue;
                }

                //Check to see if the bullet is colliding with the head of the wyrm (discounting the mandibles)
                if (isColliding((int)(thePlayer.theWyrm.l_segments[0].X - WYRMHEAD_CENTER_NUMBER * Math.Cos(thePlayer.theWyrm.HeadDirection)), 
                    (int)(thePlayer.theWyrm.l_segments[0].Y + QUARTER_OF_WYRMHEAD_SPRITEHEIGHT - WYRMHEAD_CENTER_NUMBER*Math.Sin(thePlayer.theWyrm.HeadDirection)),
                    thePlayer.theWyrm.eatRadius,
                    (int)bullets[i].xPosistion, bullets[i].yPosition, bullets[i].boundingRadius))
                {
                    thePlayer.Health -= bullets[i].DamageDealt;

                    if (thePlayer.Health < 0)
                        thePlayer.Health = 1;

                    bullets.RemoveAt(i);
                }
            }
        }
    }
}
