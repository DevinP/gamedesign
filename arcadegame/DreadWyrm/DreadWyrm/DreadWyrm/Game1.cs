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
        public static int WYRMSEGS = 8 ;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D t2dTitleScreen;                          //The title screen for the game
        Texture2D t2dTitleScreenNoWords;
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
        public static SoundEffect gunShot;
        public static SoundEffect chomp;
        public static SoundEffect explosion;
        public static SoundEffect tankShot;

        Texture2D t2dWyrmHead;                              //The sprite for the Wyrm head
        Texture2D t2dWyrmSeg;                               //The sprite for the Wyrm segments
        Texture2D t2dWyrmTail;                              //The sprite for the Wyrm tail
        Texture2D healthBase;                               //The sprite for the health base base
        Texture2D health;                                   //The sprite for the health bar
        Texture2D stamina;                                  //The sprite for the stamina bar
        Texture2D regenBar;                                 //The sprite to indicate the amount of health being regened

        Texture2D bulletTexture;                            //The sprite for the bullets used by enemies
        Texture2D cannonballTexture;                        //The sprite for tank shells

        Texture2D t2dbackground;                            //The background sprite
        Texture2D t2dforeground;                            //The foreground sprite (part of the background)
        Player thePlayer;                                   //The player of the game
        public static List<Prey> prey;                      //The edible things on screen
        List<List<int>> levelPrey;                          //The things which must be eaten to advance the wave
        public static Texture2D explosionTexture;           //Explosion animations
        public static List<Explosion> explosions;           //The current explosions in the game

        Background theBackground;

        bool gameOver = false;                              //The game has ended
        bool victory = false;                               //The player has won
        bool instructionMode = false;
        bool titleTransitionOk = true;

        int currWave = 1;

        float elapsedTimeGameEnd = 0;

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
        int maxHealthCost = 0;
        int digSpeedCost = 0;
        int staminaCost = 0;
        int regenCost = 0;
        const int HEALTHMAX_MAX = 500;
        const int SPEEDMAX = 7;
        const int STAMINA_MAX = 500;
        const float DIGSPEED_UPGRADE_INCR = 0.5f;
        const int MAXHEALTH_UPGRADE_INCR = 25;
        const int STAMINA_UPGRADE_INCR = 50;
        const int DIGSPEED_COST_INCR = 500;
        const int MAXHEALTH_COST_INCR = 200;
        const int STAMINA_COST_INCR = 500;
        const int REGEN_COST_INCR = 20;

        const int DEFAULT_MAXHEALTH_COST = 1000;
        const int DEFAULT_DIGSPEED_COST = 2000;
        const int DEFAULT_STAMINA_COST = 1000;
        const int DEFAULT_REGEN_COST = 50;
        
        //Implementing speed boost
        const float WYRM_BOOST_FACTOR = 1.5f; //Multiplies the max speed of the wyrm when boosting

        //Constant ints to access the prey texture list
        const int GIRAFFE = 0;
        const int ELEPHANT = 1;
        const int UNARMEDHUMAN = 2;
        const int SOLDIER = 3;
        const int MINE_LAYER = 4;
        const int TANK = 5;
        const int MINE = 6;

        int numWaves = 0;

        //Magical constants
        const int WYRMHEAD_CENTER_NUMBER = 8; //This number is a magic number
        const int QUARTER_OF_WYRMHEAD_SPRITEHEIGHT = 15;
        const int QUARTER_OF_WYRMSEG_SPRITEHEIGHT = 12;

        AnimatedSprite tempGiraffe;
        AnimatedSprite tempElephant;
        AnimatedSprite tempUnarmed;
        AnimatedSprite tempSoldier;
        AnimatedSprite tempEngineer;
        AnimatedSprite tempMine;
        AnimatedSprite tempTank;

        bool nuxmode = false;

        float waveSpawnCounter = 0;

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
            scoreFont = Content.Load<SpriteFont>(@"Fonts\scoreFont");

            t2dTitleScreen = Content.Load<Texture2D>(@"Textures\titleScreen");
            t2dTitleScreenNoWords = Content.Load<Texture2D>(@"Textures\titlescreen_no_words");
            t2dtransparentBlack = Content.Load<Texture2D>(@"Textures\transparentBlack");
            t2dupgradeBox = Content.Load<Texture2D>(@"Textures\wordbubble");
            t2dupgradeArrow = Content.Load<Texture2D>(@"Textures\arrow");

            roar = Content.Load<SoundEffect>(@"Sounds\Predator Roar");
            chomp = Content.Load<SoundEffect>(@"Sounds\aud_chomp");
            explosion = Content.Load<SoundEffect>(@"Sounds\explosion");
            gunShot = Content.Load<SoundEffect>(@"Sounds\soldierShot");
            tankShot = Content.Load<SoundEffect>(@"Sounds\tankShot");

            bgm = Content.Load<Song>(@"Sounds\bgm");
            bgm2 = Content.Load<Song>(@"Sounds\bgm2");
            bgm3 = Content.Load<Song>(@"Sounds\bgm3");

            t2dWyrmHead = Content.Load<Texture2D>(@"Textures\wyrmHeadRed");
            t2dWyrmSeg = Content.Load<Texture2D>(@"Textures\wyrmSegRed");
            t2dWyrmTail = Content.Load<Texture2D>(@"Textures\wyrmTailRed");

            healthBase = Content.Load<Texture2D>(@"Textures\hb_red");
            health = Content.Load<Texture2D>(@"Textures\hb_green");
            stamina = Content.Load<Texture2D>(@"Textures\hb_yellow");
            regenBar = Content.Load<Texture2D>(@"Textures\hb_orange");

            bulletTexture = Content.Load<Texture2D>(@"Textures\bullet");
            cannonballTexture = Content.Load<Texture2D>(@"Textures\cannonball");

            t2dbackground = Content.Load<Texture2D>(@"Textures\background");
            t2dforeground = Content.Load<Texture2D>(@"Textures\foreground");

            preyTextures = new List<Texture2D>();
            preyTextures.Add(Content.Load<Texture2D>(@"Textures\giraffe"));
            preyTextures.Add(Content.Load<Texture2D>(@"Textures\elephant"));
            preyTextures.Add(Content.Load<Texture2D>(@"Textures\unarmed"));
            preyTextures.Add(Content.Load<Texture2D>(@"Textures\soldier"));
            preyTextures.Add(Content.Load<Texture2D>(@"Textures\mine_layer"));
            preyTextures.Add(Content.Load<Texture2D>(@"Textures\tank"));
            preyTextures.Add(Content.Load<Texture2D>(@"Textures\mine"));

            explosionTexture = Content.Load<Texture2D>(@"Textures\explosions");

            //Initialize the sprites which appear in the title/instruction screens
            tempGiraffe = new AnimatedSprite(preyTextures[GIRAFFE], 0, 0, 102, 95, 4);
            tempGiraffe.IsAnimating = true;
            tempElephant = new AnimatedSprite(preyTextures[ELEPHANT], 0, 0, 93, 71, 6);
            tempElephant.IsAnimating = true;

            tempUnarmed = new AnimatedSprite(preyTextures[UNARMEDHUMAN], 0, 0, 21, 24, 4);
            tempUnarmed.IsAnimating = true;
            tempSoldier = new AnimatedSprite(preyTextures[SOLDIER], 0, 27, 20, 25, 6);
            tempSoldier.IsAnimating = true;

            tempEngineer = new AnimatedSprite(preyTextures[MINE_LAYER], 0, 25, 25, 23, 6);
            tempEngineer.IsAnimating = true;
            tempMine = new AnimatedSprite(preyTextures[MINE], 0, 0, 16, 9, 3);

            tempTank = new AnimatedSprite(preyTextures[TANK], 0, 50, 145, 50, 0);
            tempTank.IsAnimating = false;

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(bgm);
            bgm1Playing = true;
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

            if (gameOver)
            {
                elapsedTimeGameEnd += (float)gameTime.ElapsedGameTime.TotalSeconds;

                thePlayer.healthAfterRegen = 0;

                if(elapsedTimeGameEnd > 3)
                    endGame();

                return;
            }

            #region SongSwitching (Code to handle the switching of the song with right shift)

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

                        if (upgradeArrowDir == (float)Math.PI) //Stamina
                        {
                            if (!(thePlayer.Meat < staminaCost))
                            {
                                if ((thePlayer.MaxStamina + STAMINA_UPGRADE_INCR) >= STAMINA_MAX)
                                {
                                    thePlayer.MaxStamina = STAMINA_MAX;
                                }
                                else
                                {
                                    thePlayer.MaxStamina += STAMINA_UPGRADE_INCR;
                                    thePlayer.Meat -= staminaCost;
                                    staminaCost += STAMINA_COST_INCR;
                                }
                            }
                        }

                        else if (upgradeArrowDir == (float)(Math.PI / 2)) //Dig Speed
                        {
                            if (!(thePlayer.Meat < digSpeedCost))
                            {
                                if ((thePlayer.theWyrm.HeadSpeedMax + DIGSPEED_UPGRADE_INCR) > SPEEDMAX)
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
                                regenCost += REGEN_COST_INCR;

                                thePlayer.healthPerMS = (float)((thePlayer.REGEN_FACTOR * thePlayer.HealthMax) / (thePlayer.REGEN_DURATION));
                                thePlayer.healthAfterRegen =
                                    thePlayer.Health + ((thePlayer.REGEN_DURATION - thePlayer.elapsedTimeTotalRegen) * thePlayer.healthPerMS) 
                                    + (thePlayer.REGEN_DURATION * thePlayer.healthPerMS * (thePlayer.regen - 1));
                            }

                        }
                        else if (upgradeArrowDir == (float)((3 * Math.PI) / 2)) //Health Regen
                        {
                            //Make sure the player has enough meat and is missing health
                            if(!(thePlayer.Meat < regenCost) && (thePlayer.Health < thePlayer.HealthMax))
                            {
                                thePlayer.healthPerMS = (float)((thePlayer.REGEN_FACTOR * thePlayer.HealthMax) / (thePlayer.REGEN_DURATION));
                                thePlayer.healthAfterRegen += thePlayer.REGEN_DURATION * thePlayer.healthPerMS;

                                if (thePlayer.healthAfterRegen < thePlayer.HealthMax * 1.24)
                                {
                                    thePlayer.regen++;
                                    thePlayer.Meat -= regenCost;
                                }
                            }
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

                    int numPrey = prey.Count;

                    for (int i = 0; i < prey.Count; i++)
                    {
                        prey[i].Update(gameTime);

                        if (prey[i].isMine)
                            numPrey--;
                    }

                    for (int i = 0; i < bullets.Count; i++)
                    {
                        bullets[i].Update(gameTime);
                    }

                    thePlayer.Update(gameTime, keystate);

                    checkBullets();

                    for (int i = 0; i < explosions.Count; i++)
                    {
                        explosions[i].Update(gameTime);

                        if (explosions[i].isDone)
                            explosions.RemoveAt(i);
                    }

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

                    if (numPrey <= 0)
                    {
                        waveSpawnCounter += (float)gameTime.ElapsedGameTime.Milliseconds;

                        if (waveSpawnCounter > 1500)
                        {
                            currWave++;

                            if (currWave > numWaves)
                            {
                                gameOver = true;
                                victory = true;
                            }
                            else
                                startNewWave(currWave - 1);

                            waveSpawnCounter = 0;
                        }

                    }
                    else
                        waveSpawnCounter = 0;

                    #endregion
                }

                #endregion

            }
            else if (!m_gameStarted && !instructionMode)
            {
                #region Title Screen Mode (m_gameStarted == false)

                if (keystate.IsKeyDown(Keys.Space))
                {
                    nuxmode = false;
                    instructionMode = true;
                    titleTransitionOk = false;
                }
                else if (keystate.IsKeyDown(Keys.N))
                {
                    nuxmode = true;
                    instructionMode = true;
                    titleTransitionOk = false;
                }

                if (keystate.IsKeyUp(Keys.Space))
                    titleTransitionOk = true;

                #endregion
            }
            else
            {
                #region Instruction Screen Mode

                if (keystate.IsKeyDown(Keys.Space) && titleTransitionOk)
                {
                    instructionMode = false;
                    titleTransitionOk = false;
                    startNewGame(nuxmode);
                }

                if (keystate.IsKeyUp(Keys.Space))
                    titleTransitionOk = true;

                tempGiraffe.Update(gameTime);
                tempElephant.Update(gameTime);

                tempUnarmed.Update(gameTime);
                tempSoldier.Update(gameTime);

                tempEngineer.Update(gameTime);
                tempMine.Update(gameTime);

                tempTank.Update(gameTime);

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

                for (int i = 0; i < explosions.Count; i++)
                {
                    explosions[i].Draw(spriteBatch);
                }

                if (victory)
                {
                    spriteBatch.DrawString(titleFont, "YOU ATE ALL THE THINGS", new Vector2(500, 150), Color.Red);
                }

                spriteBatch.DrawString(scoreFont, "Level: " + currWave, new Vector2(1120, 10), Color.Red);

                if (upgradeMode)
                {
                    //Draw the partly-transparent black layer over the screen to darken it
                    spriteBatch.Draw(t2dtransparentBlack, new Rectangle(0, 0, SCREENWIDTH, SCREENHEIGHT), Color.White);

                    //Draw each upgrade box
                    spriteBatch.Draw(t2dupgradeBox, new Rectangle(515, 60, 250, 150), Color.White);
                    spriteBatch.DrawString(upgradeFont, "METABOLISM BOOST", new Vector2(570, 87), Color.Red);
                    spriteBatch.DrawString(upgradeFont, "Heal " + thePlayer.REGEN_FACTOR*100 + "% of max health", new Vector2(545, 110), Color.Red);
                    spriteBatch.DrawString(upgradeFont, "over " + thePlayer.REGEN_DURATION / 500 + " seconds", new Vector2(545, 125), Color.Red);
                    if (thePlayer.Health >= thePlayer.HealthMax)
                    {
                        spriteBatch.DrawString(upgradeFont, "Already At Max Health", new Vector2(550, 40), Color.Red);
                    }
                    else
                    {
                        spriteBatch.DrawString(upgradeFont, "Cost: " + regenCost + " KG", new Vector2(585, 40), Color.Red);
                    }

                    spriteBatch.Draw(t2dupgradeBox, new Rectangle(515, 440, 250, 150), Color.White);
                    spriteBatch.DrawString(upgradeFont, "MUSCLE VIBRATION", new Vector2(575, 467), Color.Red);
                    spriteBatch.DrawString(upgradeFont, "Increase max dig speed", new Vector2(545, 490), Color.Red);
                    spriteBatch.DrawString(upgradeFont, "by " + DIGSPEED_UPGRADE_INCR, new Vector2(545, 505), Color.Red);
                    spriteBatch.DrawString(upgradeFont, "Current max speed: " + thePlayer.theWyrm.HeadSpeedNormalMax, new Vector2(545, 530), Color.Red);
                    
                    if ((thePlayer.theWyrm.HeadSpeedMax + DIGSPEED_UPGRADE_INCR) > SPEEDMAX)
                        spriteBatch.DrawString(upgradeFont, "Maximum Upgrade Reached", new Vector2(540, 590), Color.Red);
                    else
                        spriteBatch.DrawString(upgradeFont, "Cost: " + digSpeedCost + " KG", new Vector2(590, 590), Color.Red);


                    spriteBatch.Draw(t2dupgradeBox, new Rectangle(755, 250, 250, 150), Color.White);
                    spriteBatch.DrawString(upgradeFont, "FAT TISSUE", new Vector2(837, 277), Color.Red);
                    spriteBatch.DrawString(upgradeFont, "Increase max health by " + MAXHEALTH_UPGRADE_INCR, new Vector2(780, 305), Color.Red);
                    spriteBatch.DrawString(upgradeFont, "Current max health: " + thePlayer.HealthMax, new Vector2(780, 330), Color.Red);

                    if (thePlayer.HealthMax >= HEALTHMAX_MAX)
                    {
                        spriteBatch.DrawString(upgradeFont, "Maximum Upgrade Reached", new Vector2(785, 230), Color.Red);
                    }
                    else
                    {
                        spriteBatch.DrawString(upgradeFont, "Cost: " + maxHealthCost + " KG", new Vector2(817, 230), Color.Red);
                    }

                    spriteBatch.Draw(t2dupgradeBox, new Rectangle(275, 250, 250, 150), Color.White);
                    spriteBatch.DrawString(upgradeFont, "MUSCLE COILING", new Vector2(338, 278), Color.Red);
                    spriteBatch.DrawString(upgradeFont, "Increase max stamina", new Vector2(310, 300), Color.Red);
                    spriteBatch.DrawString(upgradeFont, "by " + STAMINA_UPGRADE_INCR, new Vector2(310, 315), Color.Red);
                    spriteBatch.DrawString(upgradeFont, "Current max stamina: " + thePlayer.MaxStamina, new Vector2(310, 345), Color.Red);
                    if (thePlayer.MaxStamina >= STAMINA_MAX)
                    {
                        spriteBatch.DrawString(upgradeFont, "Maximum Upgrade Reached", new Vector2(305, 230), Color.Red);
                    }
                    else
                    {
                        spriteBatch.DrawString(upgradeFont, "Cost: " + staminaCost + " KG", new Vector2(345, 230), Color.Red);
                    }

                    //Draw the arrow which points to the currently selected box
                    spriteBatch.Draw(t2dupgradeArrow, new Rectangle(640, 325, 112, 51), null, Color.White, upgradeArrowDir, new Vector2(0, 25.5f), SpriteEffects.None, 0);

                    spriteBatch.DrawString(titleFont, "Press U to RETURN TO GAME", new Vector2(475, 680), Color.Red);
                    spriteBatch.DrawString(titleFont, "Press ENTER to DIGEST UPGRADE", new Vector2(475, 5), Color.Red);
                }
                else
                    spriteBatch.DrawString(titleFont, "Press U for UPGRADES", new Vector2(920, 570), Color.Red);

                if (gameOver && !victory)
                    spriteBatch.DrawString(scoreFont, "G A M E   O V E R", new Vector2(500, 150), Color.Red);

                #endregion
            }
            else if (!m_gameStarted && !instructionMode)
            {
                #region Title Screen Mode (m_gameStarted == false)

                spriteBatch.Draw(t2dTitleScreen, new Rectangle(0, 0, SCREENWIDTH, SCREENHEIGHT), Color.White);

                if (gameTime.TotalGameTime.Milliseconds % 1000 < 700)
                {
                    spriteBatch.DrawString(titleFont, "Press Spacebar to BEGIN YOUR FEAST", vStartTitleTextLoc, Color.OrangeRed);
                    spriteBatch.DrawString(titleFont, "Press N to start the game with NUX MODE", new Vector2(410, 500), Color.OrangeRed);
                    //spriteBatch.DrawString(titleFont, "Press I for INSTRUCTIONS", new Vector2(495, 650), Color.OrangeRed);
                }

                #endregion
            }
            else
            {
                #region Instruction Screen Mode

                //Draw the instruction screen
                spriteBatch.Draw(t2dTitleScreenNoWords, new Rectangle(0, 0, SCREENWIDTH, SCREENHEIGHT), Color.White);

                spriteBatch.DrawString(scoreFont, "INSTRUCTIONS", new Vector2(500, 25), Color.OrangeRed);

                if (gameTime.TotalGameTime.Milliseconds % 1000 < 700)
                {
                    spriteBatch.DrawString(titleFont, "Press SPACE to PROCEED", new Vector2(490, 650), Color.OrangeRed);
                }

                //Instruct the player on controls
                spriteBatch.DrawString(titleFont, "Press --> to turn the Wyrm COUNTERCLOCKWISE.", new Vector2(430, 60), Color.OrangeRed);
                spriteBatch.DrawString(titleFont, "Press <-- to turn the Wyrm CLOCKWISE.", new Vector2(430, 110), Color.OrangeRed);
                spriteBatch.DrawString(titleFont, "Press ^ to ACCELERATE the Wyrm in your CURRENT DIRECTION.", new Vector2(430, 160), Color.OrangeRed);
                spriteBatch.DrawString(titleFont, "|", new Vector2(487, 170), Color.OrangeRed);
                spriteBatch.DrawString(titleFont, "Press left shift to SPRINT. This uses STAMINA.", new Vector2(430, 210), Color.OrangeRed);

                spriteBatch.DrawString(titleFont, "Get MEAT by EATING prey on the surface. Use this MEAT to UPGRADE.", new Vector2(430, 280), Color.OrangeRed);

                //Inform the player about prey they will see
                tempGiraffe.Draw(spriteBatch, 315, 310, false);
                tempElephant.Draw(spriteBatch, 215, 330, false);
                spriteBatch.DrawString(titleFont, "ANIMALS are worth a lot of meat. Eat them quick!", new Vector2(430, 335), Color.OrangeRed);

                spriteBatch.DrawString(titleFont, "BASIC HUMANS are small but may FIGHT BACK. Eat them before they cause too much damage!", new Vector2(295, 440), Color.OrangeRed);

                tempUnarmed.Draw(spriteBatch, 245, 440, false);
                tempSoldier.Draw(spriteBatch, 265, 440, false);

                spriteBatch.DrawString(titleFont, "ENGINEERS lay MINES on the ground in an effort to fool you. Get them before they can!", new Vector2(295, 490), Color.OrangeRed);
                tempEngineer.Draw(spriteBatch, 250, 490, false);
                tempMine.Draw(spriteBatch, 273, 504, false);

                spriteBatch.DrawString(titleFont, "TANKS shoot fast, powerful shots, but only IN FRONT of them. Attack them from behind to avoid taking damage!", new Vector2(50, 540), Color.OrangeRed);
                tempTank.Draw(spriteBatch, 50, 570, false);
                

                #endregion
            }

            //Close the SpriteBatch
            spriteBatch.End();

            base.Draw(gameTime);
        }

        void startNewGame(bool nuxMode)
        {
            victory = false;

            //Add the wyrm head segment texture to the wyrm textures list
            List<Texture2D> wyrmTextures = new List<Texture2D>();

            if (!nuxMode)
                wyrmTextures.Add(t2dWyrmHead);
            else
                wyrmTextures.Add(Content.Load<Texture2D>(@"Textures\nux_head"));

            //Add on the wyrm segment textures
            //We want to subtract two from the total segments since the head and tail are not this texture
            //derp
            for (int i = 0; i < WYRMSEGS - 2; i++)
            {
                wyrmTextures.Add(t2dWyrmSeg);
            }

            //Lastly, add the wyrm tail texture
            wyrmTextures.Add(t2dWyrmTail);
            thePlayer = new Player(0, wyrmTextures, scoreFont, healthBase, health, stamina, regenBar);

            if (nuxMode)
                thePlayer.nuxMode = true;

            theBackground = new Background(t2dbackground, t2dforeground);

            prey = new List<Prey>();

            bullets = new List<Bullet>();
            explosions = new List<Explosion>();

            m_gameStarted = true;
            currWave = 1;

            levelPrey = new List<List<int>>();
            numWaves = 0;

            makeLevels();

            elapsedTimeGameEnd = 0;

            regenCost = DEFAULT_REGEN_COST;
            digSpeedCost = DEFAULT_DIGSPEED_COST;
            staminaCost = DEFAULT_STAMINA_COST;
            maxHealthCost = DEFAULT_MAXHEALTH_COST;

            startNewWave(currWave - 1);
        }

        void endGame()
        {
            m_gameStarted = false;
            gameOver = false;
        }

        void startNewWave(int wave)
        {
            prey = new List<Prey>();

            //For each prey type
            for (int i = 0; i < levelPrey.Count; i++)
            {
                //For each number of that type of prey this wave
                for (int j = 0; j < levelPrey[i][wave]; j++)
                {
                    if (i == GIRAFFE)
                        prey.Add(new Animal(m_random.Next(20, 1050), 100, preyTextures[GIRAFFE], 4, 95, 102, 94, 30, thePlayer.theWyrm, false, 1191, 97));
                    if (i == ELEPHANT)
                        prey.Add(new Animal(m_random.Next(20, 1050), 100, preyTextures[ELEPHANT], 6, 71, 93, 70, 29, thePlayer.theWyrm, false, 4990, 73));
                    if (i == UNARMEDHUMAN)
                        prey.Add(new Animal(m_random.Next(20, 1050), 100, preyTextures[UNARMEDHUMAN], 4, 24, 21, 23, 6, thePlayer.theWyrm, true, 80, 25));
                    if (i == SOLDIER)
                        prey.Add(new SoldierHuman(m_random.Next(20, 1050), 100, preyTextures[SOLDIER], 6, 25, 20, 24, 7, thePlayer.theWyrm, 80, 26, 52, 78, bulletTexture));
                    if (i == MINE_LAYER)
                        prey.Add(new Engineer(m_random.Next(20, 1050), 100, preyTextures[MINE_LAYER], 6, 23, 25, 22, 7, thePlayer.theWyrm, 80, 25, 51, 76, preyTextures[MINE]));
                    if (i == TANK)
                        prey.Add(new Vehicle(m_random.Next(20, 1050), 100, preyTextures[TANK], 0, 50, 145, 49, 25, thePlayer.theWyrm, 0, 50, cannonballTexture));
                }
            }
        }

        void makeLevels()
        {
            /*GIRAFFE = 0;
           ELEPHANT = 1;
           UNARMEDHUMAN = 2;
           SOLDIER = 3;
           MINE_LAYER = 4;
           TANK = 5;*/

            List<int> numGiraffes = new List<int>();
            List<int> numElephants = new List<int>();
            List<int> numUnarmed = new List<int>();
            List<int> numSoldier = new List<int>();
            List<int> numEngie = new List<int>();
            List<int> numTank = new List<int>();

            //Build level 1
            numGiraffes.Add(0);  //0 giraffe on level 1
            numElephants.Add(0); //0 elephants on level 1
            numUnarmed.Add(10);   //10 unarmed humans on level 1
            numSoldier.Add(0);   //etc
            numEngie.Add(0);
            numTank.Add(0);
            numWaves++;

            //Build level 2
            numGiraffes.Add(1);
            numElephants.Add(0);
            numUnarmed.Add(6);
            numSoldier.Add(3);
            numEngie.Add(0);
            numTank.Add(0);
            numWaves++;

            //Build level 3
            numGiraffes.Add(2);
            numElephants.Add(0);
            numUnarmed.Add(2);
            numSoldier.Add(5);
            numEngie.Add(2);
            numTank.Add(0);
            numWaves++;

            //Build level 4
            numGiraffes.Add(0);
            numElephants.Add(0);
            numUnarmed.Add(10);
            numSoldier.Add(7);
            numEngie.Add(3);
            numTank.Add(0);
            numWaves++;

            //Build level 5
            numGiraffes.Add(1);
            numElephants.Add(1);
            numUnarmed.Add(5);
            numSoldier.Add(9);
            numEngie.Add(2);
            numTank.Add(1);
            numWaves++;

            //Build level 6
            numGiraffes.Add(2);
            numElephants.Add(1);
            numUnarmed.Add(5);
            numSoldier.Add(6);
            numEngie.Add(3);
            numTank.Add(4);
            numWaves++;

            //Build level 7
            numGiraffes.Add(1);
            numElephants.Add(1);
            numUnarmed.Add(3);
            numSoldier.Add(12);
            numEngie.Add(6);
            numTank.Add(2);
            numWaves++;

            //Build level 8
            numGiraffes.Add(2);
            numElephants.Add(0);
            numUnarmed.Add(5);
            numSoldier.Add(20);
            numEngie.Add(2);
            numTank.Add(0);
            numWaves++;

            //Build level 9
            numGiraffes.Add(2);
            numElephants.Add(2);
            numUnarmed.Add(4);
            numSoldier.Add(15);
            numEngie.Add(10);
            numTank.Add(5);
            numWaves++;
            
            //Build level 10
            numGiraffes.Add(2);
            numElephants.Add(1);
            numUnarmed.Add(2);
            numSoldier.Add(15);
            numEngie.Add(10);
            numTank.Add(8);
            numWaves++;

            levelPrey = new List<List<int>>();

            levelPrey.Add(numGiraffes);
            levelPrey.Add(numElephants);
            levelPrey.Add(numUnarmed);
            levelPrey.Add(numSoldier);
            levelPrey.Add(numEngie);
            levelPrey.Add(numTank);
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
                    prey[i].getEaten(thePlayer);
                    prey.RemoveAt(i);

                    if (thePlayer.Health <= 0 && !thePlayer.nuxMode)
                        gameOver = true;
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

                    if (thePlayer.Health <= 0 && !thePlayer.nuxMode)
                        gameOver = true;

                    bullets.RemoveAt(i);

                    continue;
                }

                foreach (WyrmSegment ws in thePlayer.theWyrm.l_segments)
                {
                    if (isColliding((int)ws.X, (int)ws.Y + QUARTER_OF_WYRMHEAD_SPRITEHEIGHT, ws.boundingRadius, bullets[i].xPosistion, bullets[i].yPosition, bullets[i].boundingRadius))
                    {
                        thePlayer.Health -= bullets[i].DamageDealt;

                        if (thePlayer.Health <= 0 && !thePlayer.nuxMode)
                            gameOver = true;

                        bullets.RemoveAt(i);

                        break;
                    }
                }
            }
        }
    }
}
