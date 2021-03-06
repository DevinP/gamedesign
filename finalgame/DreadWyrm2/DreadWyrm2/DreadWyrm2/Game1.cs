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
using System.Runtime.InteropServices;

namespace DreadWyrm2
{

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        public static int SCREENWIDTH = 1280;
        public static int SCREENHEIGHT = 720;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D blueBoxTitleScreen;

        //Title screen booleans
        bool singlePlayerGameSelected = true;
        bool twoPlayerGameSelected = false;
        bool nuxModeSelected = false;
        bool quitToDesktop_MMSelected = false;

        Texture2D t2dTitleScreen;                          //The title screen for the game
        Texture2D t2dTitleScreenNoWords;
        Texture2D t2dtransparentBlack;                     //A partially transparent texture to draw over the game
        Texture2D t2dupgradeBox;                           //A box to put upgrade messages in
        Texture2D t2dupgradeBoxOffColor;                   //A box to indicated it is selected
        Texture2D t2dupgradeArrow;                         //Arrow to indicate which upgrade the player will select
        Song bgm;                                          //The background music for the game
        bool m_gameStarted = false;                        //Whether or not we are at the title screen
        SpriteFont titleFont;                              //The font used in the game for the title screen
        SpriteFont upgradeFont;                            //The font used in the game for the upgrade screen
        SpriteFont upgradeFontNumbers;                     //The font used for specific upgrade numbers in two player mode
        SpriteFont scoreFont;                              //The font used to dispaly the meat score
        Vector2 vStartTitleTextLoc = new Vector2(440, 440);//The location for the additional title screen text
        SoundEffect roar;
        SoundEffect missileDown;
        SoundEffectInstance missileDownInstance;
       
        Texture2D t2dbackgroundSinglePlayer;                //The background sprite
        Texture2D t2dforegroundSinglePlayer;                //The foreground sprite (part of the background)
        Texture2D t2dbackgroundTwoPlayer;                   //The background sprite for the multiplayer arena
        Texture2D t2dforegroundTwoPlayer;                   //The foreground sprite for the multiplayer arena
        public static WyrmPlayer theWyrmPlayer;             //The wyrm player of the game
        HumanPlayer theHumanPlayer;                         //The human player of the game
        List<List<int>> levelPrey;                          //The things which must be eaten to advance the wave

        //Explosion data
        public static Texture2D explosionTexture;           //Explosion animations
        public static List<Explosion> explosions;           //The current explosions in the game
        public static SoundEffect explosion;

        //Bullet data
        public static List<Bullet> bullets;                 //The bullets currently in the game
        public static Texture2D bulletTexture;              //The texture for bullets in the game
        public static Texture2D cannonballTexture;          //The texture for larger cannonballs in the game

        List<FloatingText> damageIndicators;

        Background theBackground;

        public static bool gameOver = false;                //The game has ended
        bool singlePlayerVictory = false;                   //The player has won in single player
        bool instructionMode = false;
        bool titleTransitionOk = true;
        bool titleScreenSelectionChangeOk = true;
        public static bool isTwoPlayer = false;
        public static bool p2WyrmVictory = false;
        public static bool p2HumanVictory = false;

        int currWave = 1;

        float elapsedTimeGameEnd = 0;

        //The game's random numbers
        public static Random m_random;

        bool p2CanUpgradeRegen = true;
        bool p2CanUpgradeHealth = true;
        bool p2CanUpgradeSpeed = true;
        bool p2CanUpgradeSprint = true;

        bool canRoar = true;

        //Pause mode variables
        bool gamePaused = false;
        bool gamePausedCanSwitch = true;
        bool canSwitchMenuSelection = true;
        bool returnToGameSelected = false;
        bool quitToMenuSelected = false;
        bool quitToDesktopSelected = false;

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

        int numWaves = 0;


        public static PreySpawner preySpawner = new PreySpawner();

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

        //DEBUG
        Texture2D debugCircle;

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
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            base.Initialize();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool ClipCursor(ref Rectangle rcClip);

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //DEBUG
            debugCircle = Content.Load<Texture2D>(@"Textures\debugCircleTex");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            titleFont = Content.Load<SpriteFont>(@"Fonts\Title");
            upgradeFont = Content.Load<SpriteFont>(@"Fonts\Upgrade");
            upgradeFontNumbers = Content.Load<SpriteFont>(@"Fonts\upgradeFontNumbers");
            scoreFont = Content.Load<SpriteFont>(@"Fonts\scoreFont");

            blueBoxTitleScreen = Content.Load<Texture2D>(@"Textures\blueBox");

            t2dTitleScreen = Content.Load<Texture2D>(@"Textures\titleScreen");
            t2dTitleScreenNoWords = Content.Load<Texture2D>(@"Textures\titlescreen_no_words");
            t2dtransparentBlack = Content.Load<Texture2D>(@"Textures\transparentBlack");
            t2dupgradeBox = Content.Load<Texture2D>(@"Textures\wordbubble");
            t2dupgradeBoxOffColor = Content.Load<Texture2D>(@"Textures\wordbubble_offcolor");
            t2dupgradeArrow = Content.Load<Texture2D>(@"Textures\arrow");

            HumanPlayer.LoadContent(Content);

            Prey.LoadContent(Content);

            Building.LoadContent(Content);

            FloatingText.LoadContent(Content);

            roar = Content.Load<SoundEffect>(@"Sounds\Predator Roar");
            missileDown = Content.Load<SoundEffect>(@"Sounds\Missile");
            missileDownInstance = missileDown.CreateInstance();

            explosion = Content.Load<SoundEffect>(@"Sounds\explosion");

            bgm = Content.Load<Song>(@"Sounds\Pizza");

            WyrmPlayer.LoadContent(Content);

            t2dbackgroundSinglePlayer = Content.Load<Texture2D>(@"Textures\background");
            t2dforegroundSinglePlayer = Content.Load<Texture2D>(@"Textures\foreground");

            t2dbackgroundTwoPlayer = Content.Load<Texture2D>(@"Textures\2pbackground");
            t2dforegroundTwoPlayer = Content.Load<Texture2D>(@"Textures\2pforeground");

            explosionTexture = Content.Load<Texture2D>(@"Textures\explosions");
            bulletTexture = Content.Load<Texture2D>(@"Textures\bullet");
            cannonballTexture = Content.Load<Texture2D>(@"Textures\cannonball");

            //Initialize the sprites which appear in the title/instruction screens
            tempGiraffe = new AnimatedSprite(Prey.preyTextures[Prey.GIRAFFE], 0, 0, 102, 95, 4);
            tempGiraffe.IsAnimating = true;
            tempElephant = new AnimatedSprite(Prey.preyTextures[Prey.ELEPHANT], 0, 0, 93, 71, 6);
            tempElephant.IsAnimating = true;

            tempUnarmed = new AnimatedSprite(Prey.preyTextures[Prey.UNARMEDHUMAN], 0, 0, 21, 24, 4);
            tempUnarmed.IsAnimating = true;
            tempSoldier = new AnimatedSprite(Prey.preyTextures[Prey.SOLDIER], 0, 27, 20, 25, 6);
            tempSoldier.IsAnimating = true;

            tempEngineer = new AnimatedSprite(Prey.preyTextures[Prey.MINE_LAYER], 0, 25, 25, 23, 6);
            tempEngineer.IsAnimating = true;
            tempMine = new AnimatedSprite(Prey.preyTextures[Prey.MINE], 0, 0, 16, 9, 3);

            tempTank = new AnimatedSprite(Prey.preyTextures[Prey.NEW_TANK], 0, 300, 75, 60, 3);
            tempTank.IsAnimating = true;

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(bgm);

            SoundEffect.MasterVolume = SoundEffect.MasterVolume * 0.7f;
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

            Rectangle newRec = new Rectangle(0, 0, SCREENWIDTH, SCREENHEIGHT);

           if (this.IsActive)
            {
                //Keep the cursor on the screen
                ClipCursor(ref newRec);
            }

            if (gameOver)
            {
                elapsedTimeGameEnd += (float)gameTime.ElapsedGameTime.TotalSeconds;

                theWyrmPlayer.healthAfterRegen = 0;

                if (elapsedTimeGameEnd > 3)
                {
                    if (!isTwoPlayer)
                        endSinglePlayerGame();
                    else
                        endMultiPlayerGame();
                }
                
                return;
            }

            // Get elapsed game time since last call to Update
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (m_gameStarted)
            {
                #region GamePlay Mode (m_gameStarted == true)

                int numPrey;

                //Autopause when the game loses focus
                if (!this.IsActive && gamePausedCanSwitch && !upgradeMode && !gamePaused)
                {
                    //Activate pause mode
                    gamePaused = true;
                    gamePausedCanSwitch = false;
                    returnToGameSelected = true;
                    quitToMenuSelected = false;
                    quitToDesktopSelected = false;
                    MediaPlayer.Volume = MediaPlayer.Volume * 0.25f;
                }

                //Pause if the user presses the pause button
                if ((keystate.IsKeyDown(Keys.P) || keystate.IsKeyDown(Keys.Escape)) && gamePausedCanSwitch && !upgradeMode && !gamePaused)
                {
                    //Activate pause mode
                    gamePaused = true;
                    gamePausedCanSwitch = false;
                    returnToGameSelected = true;
                    quitToMenuSelected = false;
                    quitToDesktopSelected = false;
                    MediaPlayer.Volume = MediaPlayer.Volume * 0.25f;
                }
                else if (keystate.IsKeyUp(Keys.P) && keystate.IsKeyUp(Keys.Escape))
                {
                    gamePausedCanSwitch = true;
                }

                //Tell PreySpawner to update
                preySpawner.update(gameTime);

                if (!gamePaused && !isTwoPlayer)
                {
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
                }

                if (gamePaused)
                {
                    #region Pause Mode

                    missileDownInstance.Pause();

                    if (gamePausedCanSwitch && (keystate.IsKeyDown(Keys.P) || keystate.IsKeyDown(Keys.Escape)))
                    {
                        //Return to the game
                        gamePaused = false;
                        gamePausedCanSwitch = false;

                        if(isTwoPlayer)
                            missileDownInstance.Resume();

                        MediaPlayer.Volume = MediaPlayer.Volume * 4;
                    }

                    if (returnToGameSelected)
                    {
                        if (canSwitchMenuSelection)
                        {
                            if (keystate.IsKeyDown(Keys.Down))
                            {
                                //Change the current menu selection
                                returnToGameSelected = false;
                                quitToMenuSelected = true;
                                quitToDesktopSelected = false;
                                canSwitchMenuSelection = false;
                            }
                        }

                        if (keystate.IsKeyDown(Keys.Enter))
                        {
                            //Return to the game
                            gamePaused = false;
                            gamePausedCanSwitch = false;

                            if(isTwoPlayer)
                                missileDownInstance.Resume();

                            MediaPlayer.Volume = MediaPlayer.Volume * 4;
                        }
                    }
                    else if (quitToMenuSelected)
                    {
                        if (canSwitchMenuSelection)
                        {
                            //Change the current menu selection based on input
                            if (keystate.IsKeyDown(Keys.Down))
                            {
                                returnToGameSelected = false;
                                quitToMenuSelected = false;
                                quitToDesktopSelected = true;
                                canSwitchMenuSelection = false;
                            }
                            else if (keystate.IsKeyDown(Keys.Up))
                            {
                                returnToGameSelected = true;
                                quitToMenuSelected = false;
                                quitToDesktopSelected = false;
                                canSwitchMenuSelection = false;
                            }
                        }

                        if (keystate.IsKeyDown(Keys.Enter))
                        {
                            //Quit to the main menu
                            endSinglePlayerGame();
                            endMultiPlayerGame();
                            MediaPlayer.Volume = MediaPlayer.Volume * 4;
                        }
                    }
                    else if (quitToDesktopSelected)
                    {
                        if (canSwitchMenuSelection)
                        {
                            if (keystate.IsKeyDown(Keys.Up))
                            {
                                returnToGameSelected = false;
                                quitToMenuSelected = true;
                                quitToDesktopSelected = false;
                                canSwitchMenuSelection = false;
                            }
                        }

                        if (keystate.IsKeyDown(Keys.Enter))
                        {
                            //Quit to desktop
                            this.Exit();
                        }
                    }

                    if (keystate.IsKeyUp(Keys.Up) && keystate.IsKeyUp(Keys.Down))
                        canSwitchMenuSelection = true;

                    #endregion
                }
                else if (!isTwoPlayer)
                {
                    #region onePlayerMode

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
                                if (!(theWyrmPlayer.Meat < staminaCost))
                                {
                                    if ((theWyrmPlayer.MaxStamina + STAMINA_UPGRADE_INCR) >= STAMINA_MAX)
                                    {
                                        theWyrmPlayer.MaxStamina = STAMINA_MAX;
                                    }
                                    else
                                    {
                                        theWyrmPlayer.MaxStamina += STAMINA_UPGRADE_INCR;
                                        theWyrmPlayer.Meat -= staminaCost;
                                        staminaCost += STAMINA_COST_INCR;
                                    }
                                }
                            }

                            else if (upgradeArrowDir == (float)(Math.PI / 2)) //Dig Speed
                            {
                                if (!(theWyrmPlayer.Meat < digSpeedCost))
                                {
                                    if ((theWyrmPlayer.theWyrm.HeadSpeedMax + DIGSPEED_UPGRADE_INCR) > SPEEDMAX)
                                    {
                                        theWyrmPlayer.theWyrm.HeadSpeedMax = SPEEDMAX;
                                    }
                                    else
                                    {
                                        theWyrmPlayer.theWyrm.HeadSpeedMax += DIGSPEED_UPGRADE_INCR;
                                        theWyrmPlayer.theWyrm.HeadSpeedNormalMax += DIGSPEED_UPGRADE_INCR;
                                        theWyrmPlayer.theWyrm.HeadSpeedBoostMax += DIGSPEED_UPGRADE_INCR * WYRM_BOOST_FACTOR;
                                        theWyrmPlayer.Meat -= digSpeedCost;
                                        digSpeedCost += DIGSPEED_COST_INCR;
                                    }
                                }

                            }
                            else if (upgradeArrowDir == 0) //Max Health
                            {
                                if (!(theWyrmPlayer.Meat < maxHealthCost) && (theWyrmPlayer.HealthMax + MAXHEALTH_UPGRADE_INCR <= HEALTHMAX_MAX))
                                {
                                    theWyrmPlayer.HealthMax += MAXHEALTH_UPGRADE_INCR;
                                    theWyrmPlayer.Meat -= maxHealthCost;
                                    maxHealthCost += MAXHEALTH_COST_INCR;
                                    regenCost += REGEN_COST_INCR;

                                    theWyrmPlayer.healthPerMS = (float)((theWyrmPlayer.REGEN_FACTOR * theWyrmPlayer.HealthMax) / (theWyrmPlayer.REGEN_DURATION));
                                    theWyrmPlayer.healthAfterRegen =
                                        theWyrmPlayer.Health + ((theWyrmPlayer.REGEN_DURATION - theWyrmPlayer.elapsedTimeTotalRegen) * theWyrmPlayer.healthPerMS)
                                        + (theWyrmPlayer.REGEN_DURATION * theWyrmPlayer.healthPerMS * (theWyrmPlayer.regen - 1));
                                }

                            }
                            else if (upgradeArrowDir == (float)((3 * Math.PI) / 2)) //Health Regen
                            {
                                //Make sure the player has enough meat and is missing health
                                if (!(theWyrmPlayer.Meat < regenCost) && (theWyrmPlayer.Health < theWyrmPlayer.HealthMax))
                                {
                                    theWyrmPlayer.healthPerMS = (float)((theWyrmPlayer.REGEN_FACTOR * theWyrmPlayer.HealthMax) / (theWyrmPlayer.REGEN_DURATION));
                                    theWyrmPlayer.healthAfterRegen += theWyrmPlayer.REGEN_DURATION * theWyrmPlayer.healthPerMS;

                                    if (theWyrmPlayer.healthAfterRegen < theWyrmPlayer.HealthMax * 1.24)
                                    {
                                        theWyrmPlayer.regen++;
                                        theWyrmPlayer.Meat -= regenCost;
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

                        numPrey = Prey.UpdateAll(gameTime);

                        for (int i = 0; i < bullets.Count; i++)
                        {
                            bullets[i].Update(gameTime);
                        }

                        theWyrmPlayer.Update(gameTime, keystate);

                        checkBullets();

                        for (int i = 0; i < damageIndicators.Count; i++)
                        {
                            damageIndicators[i].Update(gameTime);

                            if (damageIndicators[i].isDone)
                                damageIndicators.RemoveAt(i);
                        }

                        for (int i = 0; i < explosions.Count; i++)
                        {
                            explosions[i].Update(gameTime);

                            if (explosions[i].isDone)
                                explosions.RemoveAt(i);
                        }

                        //Make it so the player can't move off the screen
                        for (int i = 0; i < Wyrm.WYRMSEGS; i++)
                        {
                            if (theWyrmPlayer.theWyrm.l_segments[i].X < 25)
                                theWyrmPlayer.theWyrm.l_segments[i].X = 25;

                            if (theWyrmPlayer.theWyrm.l_segments[i].X > SCREENWIDTH - 25)
                                theWyrmPlayer.theWyrm.l_segments[i].X = (float)SCREENWIDTH - 25;

                            // if (thePlayer.theWyrm.l_segments[i].Y < 0)
                            //    thePlayer.theWyrm.l_segments[i].Y = 0;

                            if (theWyrmPlayer.theWyrm.l_segments[i].Y > SCREENHEIGHT - 50)
                                theWyrmPlayer.theWyrm.l_segments[i].Y = (float)SCREENHEIGHT - 50;
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
                                    singlePlayerVictory = true;
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
                else
                {
                    #region twoPlayerMode

                    #region Handling upgrades in real-time

                    //Check if the player is pressing the health regen key (1)
                    if (p2CanUpgradeRegen && keystate.IsKeyDown(Keys.D1))
                    {
                        p2CanUpgradeRegen = false;

                        //Make sure the player has enough meat and is missing health
                        if (!(theWyrmPlayer.Meat < regenCost) && (theWyrmPlayer.Health < theWyrmPlayer.HealthMax))
                        {
                            theWyrmPlayer.healthPerMS = (float)((theWyrmPlayer.REGEN_FACTOR * theWyrmPlayer.HealthMax) / (theWyrmPlayer.REGEN_DURATION));
                            theWyrmPlayer.healthAfterRegen += theWyrmPlayer.REGEN_DURATION * theWyrmPlayer.healthPerMS;

                            if (theWyrmPlayer.healthAfterRegen < theWyrmPlayer.HealthMax * 1.24)
                            {
                                theWyrmPlayer.regen++;
                                theWyrmPlayer.Meat -= regenCost;
                            }
                        }
                    }
                    else if (keystate.IsKeyUp(Keys.D1))
                    {
                        p2CanUpgradeRegen = true;
                    }

                    //Check if the player is pressing the max health key (2)
                    if (p2CanUpgradeHealth && keystate.IsKeyDown(Keys.D2))
                    {
                        p2CanUpgradeHealth = false;

                        if (!(theWyrmPlayer.Meat < maxHealthCost) && (theWyrmPlayer.HealthMax + MAXHEALTH_UPGRADE_INCR <= HEALTHMAX_MAX))
                        {
                            theWyrmPlayer.HealthMax += MAXHEALTH_UPGRADE_INCR;
                            theWyrmPlayer.Meat -= maxHealthCost;
                            maxHealthCost += MAXHEALTH_COST_INCR;
                            regenCost += REGEN_COST_INCR;

                            theWyrmPlayer.healthPerMS = (float)((theWyrmPlayer.REGEN_FACTOR * theWyrmPlayer.HealthMax) / (theWyrmPlayer.REGEN_DURATION));
                            theWyrmPlayer.healthAfterRegen =
                                theWyrmPlayer.Health + ((theWyrmPlayer.REGEN_DURATION - theWyrmPlayer.elapsedTimeTotalRegen) * theWyrmPlayer.healthPerMS)
                                + (theWyrmPlayer.REGEN_DURATION * theWyrmPlayer.healthPerMS * (theWyrmPlayer.regen - 1));
                        }
                    }
                    else if (keystate.IsKeyUp(Keys.D2))
                    {
                        p2CanUpgradeHealth = true;
                    }

                    //Check if the player is pressing the max speed key (3)
                    if (p2CanUpgradeSpeed && keystate.IsKeyDown(Keys.D3))
                    {
                        p2CanUpgradeSpeed = false;

                        if (!(theWyrmPlayer.Meat < digSpeedCost))
                        {
                            if ((theWyrmPlayer.theWyrm.HeadSpeedMax + DIGSPEED_UPGRADE_INCR) > SPEEDMAX)
                            {
                                theWyrmPlayer.theWyrm.HeadSpeedMax = SPEEDMAX;
                            }
                            else
                            {
                                theWyrmPlayer.theWyrm.HeadSpeedMax += DIGSPEED_UPGRADE_INCR;
                                theWyrmPlayer.theWyrm.HeadSpeedNormalMax += DIGSPEED_UPGRADE_INCR;
                                theWyrmPlayer.theWyrm.HeadSpeedBoostMax += DIGSPEED_UPGRADE_INCR * WYRM_BOOST_FACTOR;
                                theWyrmPlayer.Meat -= digSpeedCost;
                                digSpeedCost += DIGSPEED_COST_INCR;
                            }
                        }
                    }
                    else if (keystate.IsKeyUp(Keys.D3))
                    {
                        p2CanUpgradeSpeed = true;
                    }

                    //Check if the player is pressing the max stamina key (4)
                    if (p2CanUpgradeSprint && keystate.IsKeyDown(Keys.D4))
                    {
                        p2CanUpgradeSprint = false;

                        if (!(theWyrmPlayer.Meat < staminaCost))
                        {
                            if ((theWyrmPlayer.MaxStamina + STAMINA_UPGRADE_INCR) >= STAMINA_MAX)
                            {
                                theWyrmPlayer.MaxStamina = STAMINA_MAX;
                            }
                            else
                            {
                                theWyrmPlayer.MaxStamina += STAMINA_UPGRADE_INCR;
                                theWyrmPlayer.Meat -= staminaCost;
                                staminaCost += STAMINA_COST_INCR;
                            }
                        }
                    }
                    else if (keystate.IsKeyUp(Keys.D4))
                    {
                        p2CanUpgradeSprint = true;
                    }

                    #endregion

                    //Increase volume back to max over time if it isn't already
                   // if (MediaPlayer.Volume < 1)
                   // {
                    //    MediaPlayer.Volume = MediaPlayer.Volume + 0.0005f;
                   // }

                    theBackground.Update();

                    //Check to see if the wyrm is eating and prey
                    checkEat();

                    //Check to see if the wyrm is colliding with (and thus damaging) buildings
                    checkBuildingCollisions();

                    for (int i = 0; i < Building.buildings.Count; i++)
                    {
                        if (Building.buildings[i].isDestroyed)
                        {
                            explosions.Add(new Explosion(Building.buildings[i].xPos + Building.buildings[i].SpriteWidth / 2,
                                Building.buildings[i].yPos + Building.buildings[i].SpriteHeight / 2, explosionTexture, false));

                            explosion.Play();

                            Building.buildings.RemoveAt(i);
                        }
                    }

                    

                    //Update all the prey
                    numPrey = Prey.UpdateAll(gameTime);

                    //Update all the bullets
                    for (int i = 0; i < bullets.Count; i++)
                    {
                        bullets[i].Update(gameTime);
                    }

                    //Update the wyrm player
                    theWyrmPlayer.Update(gameTime, keystate);

                    //Reset the ability for buildings to get damaged by the wyrm if the wyrm is underground
                    if (theWyrmPlayer.theWyrm.b_wyrmGrounded)
                    {
                        foreach (Building theBuilding in Building.buildings)
                        {
                            theBuilding.DamagedThisJump = false;
                        }
                    }

                    int numBuilding;

                    //Update all the buildings
                    numBuilding = Building.UpdateAll(gameTime);

                    //Check if any bullets are hitting the wyrm
                    checkBullets();

                    for (int i = 0; i < damageIndicators.Count; i++)
                    {
                        damageIndicators[i].Update(gameTime);

                        if (damageIndicators[i].isDone)
                            damageIndicators.RemoveAt(i);
                    }

                    //Update any explosions which are happening currently
                    for (int i = 0; i < explosions.Count; i++)
                    {
                        explosions[i].Update(gameTime);

                        if (explosions[i].isDone)
                            explosions.RemoveAt(i);
                    }

                    //Update the human player
                    theHumanPlayer.Update(gameTime);

                    //Make it so the player can't move off the screen
                    for (int i = 0; i < Wyrm.WYRMSEGS; i++)
                    {
                        if (theWyrmPlayer.theWyrm.l_segments[i].X < 25)
                            theWyrmPlayer.theWyrm.l_segments[i].X = 25;

                        if (theWyrmPlayer.theWyrm.l_segments[i].X > SCREENWIDTH - 25)
                            theWyrmPlayer.theWyrm.l_segments[i].X = (float)SCREENWIDTH - 25;

                        if (theWyrmPlayer.theWyrm.l_segments[i].Y > SCREENHEIGHT - 50)
                            theWyrmPlayer.theWyrm.l_segments[i].Y = (float)SCREENHEIGHT - 50;
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
            else if (!m_gameStarted && !instructionMode)
            {
                #region Title Screen Mode (m_gameStarted == false)

                if (singlePlayerGameSelected && titleTransitionOk)
                {
                    if (titleScreenSelectionChangeOk)
                    {
                        if (keystate.IsKeyDown(Keys.Down))
                        {
                            //Change the current menu selection
                            singlePlayerGameSelected = false;
                            twoPlayerGameSelected = true;
                            nuxModeSelected = false;
                            quitToDesktop_MMSelected = false;
                            titleScreenSelectionChangeOk = false;
                        }
                    }

                    if (keystate.IsKeyDown(Keys.Enter))
                    {
                        //Start a single player game
                        nuxmode = false;
                        instructionMode = true;
                        titleTransitionOk = false;
                        isTwoPlayer = false;
                    }
                }
                else if (twoPlayerGameSelected && titleTransitionOk)
                {
                    if (titleScreenSelectionChangeOk)
                    {
                        if (keystate.IsKeyDown(Keys.Up))
                        {
                            //Change the current menu selection
                            singlePlayerGameSelected = true;
                            twoPlayerGameSelected = false;
                            nuxModeSelected = false;
                            quitToDesktop_MMSelected = false;
                            titleScreenSelectionChangeOk = false;
                        }
                        else if (keystate.IsKeyDown(Keys.Down))
                        {
                            //Change the current menu selection
                            singlePlayerGameSelected = false;
                            twoPlayerGameSelected = false;
                            nuxModeSelected = true;
                            quitToDesktop_MMSelected = false;
                            titleScreenSelectionChangeOk = false;
                        }
                    }

                    if (keystate.IsKeyDown(Keys.Enter))
                    {
                        //Start a two player game
                        nuxmode = false;
                        instructionMode = false;
                        titleTransitionOk = false;
                        startNewMultiPlayerGame();
                    }
                }
                else if (nuxModeSelected && titleTransitionOk)
                {
                    if (titleScreenSelectionChangeOk)
                    {
                        if (keystate.IsKeyDown(Keys.Up))
                        {
                            //Change the current menu selection
                            singlePlayerGameSelected = false;
                            twoPlayerGameSelected = true;
                            nuxModeSelected = false;
                            quitToDesktop_MMSelected = false;
                            titleScreenSelectionChangeOk = false;
                        }
                        else if (keystate.IsKeyDown(Keys.Down))
                        {
                            //Change the current menu selection
                            singlePlayerGameSelected = false;
                            twoPlayerGameSelected = false;
                            nuxModeSelected = false;
                            quitToDesktop_MMSelected = true;
                            titleScreenSelectionChangeOk = false;
                        }
                    }

                    if (keystate.IsKeyDown(Keys.Enter))
                    {
                        //Start a single player game with nux mode
                        nuxmode = true;
                        instructionMode = true;
                        titleTransitionOk = false;
                        isTwoPlayer = false;
                    }
                }
                else if (quitToDesktop_MMSelected && titleTransitionOk)
                {
                    if (titleScreenSelectionChangeOk)
                    {
                        if (keystate.IsKeyDown(Keys.Up))
                        {
                            //Change the current menu selection
                            singlePlayerGameSelected = false;
                            twoPlayerGameSelected = false;
                            nuxModeSelected = true;
                            quitToDesktop_MMSelected = false;
                            titleScreenSelectionChangeOk = false;
                        }
                    }

                    if (keystate.IsKeyDown(Keys.Enter))
                    {
                        //Quit to desktop
                        this.Exit();
                    }
                }

                if (keystate.IsKeyUp(Keys.Up) && keystate.IsKeyUp(Keys.Down))
                    titleScreenSelectionChangeOk = true;

                if (keystate.IsKeyUp(Keys.Enter))
                    titleTransitionOk = true;

                #endregion
            }
            else
            {
                #region Instruction Screen Mode

                if (keystate.IsKeyDown(Keys.Enter) && titleTransitionOk)
                {
                    instructionMode = false;
                    titleTransitionOk = false;
                    startNewSinglePlayerGame(nuxmode);
                }

                if (keystate.IsKeyUp(Keys.Enter))
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

                if (!isTwoPlayer)
                {
                    #region onePlayerMode
                    
                    theBackground.Draw(spriteBatch);

                    Prey.DrawAll(spriteBatch);

                    for (int i = 0; i < bullets.Count; i++)
                    {
                        bullets[i].Draw(spriteBatch);
                    }

                    theWyrmPlayer.Draw(spriteBatch);

                    for (int i = 0; i < explosions.Count; i++)
                    {
                        explosions[i].Draw(spriteBatch);
                    }

                    if (singlePlayerVictory)
                    {
                        spriteBatch.DrawString(titleFont, "YOU ATE ALL THE THINGS", new Vector2(500, 150), Color.Red);
                    }

                    if(currWave <= 10)
                        spriteBatch.DrawString(scoreFont, "Level: " + currWave, new Vector2(1120, 10), Color.Red);
                    else
                        spriteBatch.DrawString(scoreFont, "Level: 10", new Vector2(1120, 10), Color.Red);

                    if (upgradeMode)
                    {
                        #region upgradeMode
                        //Draw the partly-transparent black layer over the screen to darken it
                        spriteBatch.Draw(t2dtransparentBlack, new Rectangle(0, 0, SCREENWIDTH, SCREENHEIGHT), Color.White);

                        //Draw each upgrade box
                        spriteBatch.Draw(t2dupgradeBox, new Rectangle(515, 60, 250, 150), Color.White);
                        spriteBatch.DrawString(upgradeFont, "METABOLISM BOOST", new Vector2(570, 87), Color.Red);
                        spriteBatch.DrawString(upgradeFont, "Heal " + theWyrmPlayer.REGEN_FACTOR * 100 + "% of max health", new Vector2(545, 110), Color.Red);
                        spriteBatch.DrawString(upgradeFont, "over " + theWyrmPlayer.REGEN_DURATION / 500 + " seconds", new Vector2(545, 125), Color.Red);
                        if (theWyrmPlayer.Health >= theWyrmPlayer.HealthMax)
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
                        spriteBatch.DrawString(upgradeFont, "Current max speed: " + theWyrmPlayer.theWyrm.HeadSpeedNormalMax, new Vector2(545, 530), Color.Red);

                        if ((theWyrmPlayer.theWyrm.HeadSpeedMax + DIGSPEED_UPGRADE_INCR) > SPEEDMAX)
                            spriteBatch.DrawString(upgradeFont, "Maximum Upgrade Reached", new Vector2(540, 590), Color.Red);
                        else
                            spriteBatch.DrawString(upgradeFont, "Cost: " + digSpeedCost + " KG", new Vector2(590, 590), Color.Red);


                        spriteBatch.Draw(t2dupgradeBox, new Rectangle(755, 250, 250, 150), Color.White);
                        spriteBatch.DrawString(upgradeFont, "FAT TISSUE", new Vector2(837, 277), Color.Red);
                        spriteBatch.DrawString(upgradeFont, "Increase max health by " + MAXHEALTH_UPGRADE_INCR, new Vector2(780, 305), Color.Red);
                        spriteBatch.DrawString(upgradeFont, "Current max health: " + theWyrmPlayer.HealthMax, new Vector2(780, 330), Color.Red);

                        if (theWyrmPlayer.HealthMax >= HEALTHMAX_MAX)
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
                        spriteBatch.DrawString(upgradeFont, "Current max stamina: " + theWyrmPlayer.MaxStamina, new Vector2(310, 345), Color.Red);
                        if (theWyrmPlayer.MaxStamina >= STAMINA_MAX)
                        {
                            spriteBatch.DrawString(upgradeFont, "Maximum Upgrade Reached", new Vector2(305, 230), Color.Red);
                        }
                        else
                        {
                            spriteBatch.DrawString(upgradeFont, "Cost: " + staminaCost + " KG", new Vector2(345, 230), Color.Red);
                        }

                        //Draw the arrow which points to the currently selected box
                        spriteBatch.Draw(t2dupgradeArrow, new Rectangle(640, 325, 112, 51), null, Color.White, upgradeArrowDir, new Vector2(0, 25.5f), SpriteEffects.None, 0);

                        spriteBatch.DrawString(titleFont, "Press U to RETURN TO GAME", new Vector2(495, 680), Color.Red);
                        spriteBatch.DrawString(titleFont, "Press ENTER to DIGEST UPGRADE", new Vector2(475, 5), Color.Red);

                        #endregion
                    }

                    else
                        spriteBatch.DrawString(titleFont, "Press U for UPGRADES", new Vector2(920, 570), Color.Red);

                    if (gameOver && !singlePlayerVictory)
                        spriteBatch.DrawString(scoreFont, "G A M E   O V E R", new Vector2(500, 150), Color.Red);
                    #endregion
                }
                else
                {
                    #region twoPlayerMode

                    theBackground.Draw(spriteBatch);

                    Building.DrawAll(spriteBatch);

                    //Draw all of the bounding circles (for debugging)
                    /*foreach (Building theBuilding in Building.buildings)
                    {
                        spriteBatch.Draw(debugCircle, new Rectangle((int)(theBuilding.getBoundingX() - theBuilding.boundingRadius), (int)(theBuilding.getBoundingY() - theBuilding.boundingRadius),
                                (int)(2 * theBuilding.boundingRadius), (int)(2 * theBuilding.boundingRadius)), Color.White);
                    }*/

                    for (int i = 0; i < bullets.Count; i++)
                    {
                        bullets[i].Draw(spriteBatch);
                    }

                    Prey.DrawAll(spriteBatch);

                    theWyrmPlayer.Draw(spriteBatch);

                    for (int i = 0; i < explosions.Count; i++)
                    {
                        explosions[i].Draw(spriteBatch);
                    }

                    #region Drawing Upgrade Info

                    spriteBatch.DrawString(upgradeFontNumbers, "PRESS: ", new Vector2(760,660), Color.Red);

                    //Draw the health regen indicator
                    if(theWyrmPlayer.Health >= theWyrmPlayer.HealthMax)
                        spriteBatch.DrawString(upgradeFont, "Fully Healthy", new Vector2(840, 645), Color.Red);
                    else
                        spriteBatch.DrawString(upgradeFont, regenCost + " KG", new Vector2(865, 645), Color.Red);

                    spriteBatch.DrawString(upgradeFontNumbers, "1", new Vector2(880, 660), Color.Red);

                    spriteBatch.DrawString(upgradeFont, "REGEN", new Vector2(865, 690), Color.Red);


                    //Draw the max health indicator
                    if (theWyrmPlayer.HealthMax >= HEALTHMAX_MAX)
                    {
                        spriteBatch.DrawString(upgradeFont, "MAX", new Vector2(990,645), Color.Red);
                    }
                    else
                    {
                        spriteBatch.DrawString(upgradeFont,  maxHealthCost + " KG", new Vector2(980,645), Color.Red);
                    }

                    spriteBatch.DrawString(upgradeFontNumbers, "2", new Vector2(1000,660), Color.Red);

                    spriteBatch.DrawString(upgradeFont, "MAX HEALTH", new Vector2(965, 690), Color.Red);


                    //Draw the max speed indicator
                    if ((theWyrmPlayer.theWyrm.HeadSpeedNormalMax + DIGSPEED_UPGRADE_INCR) > SPEEDMAX)
                        spriteBatch.DrawString(upgradeFont, "MAX", new Vector2(1090,645), Color.Red);
                    else
                        spriteBatch.DrawString(upgradeFont, digSpeedCost + " KG", new Vector2(1080,645), Color.Red);

                    spriteBatch.DrawString(upgradeFontNumbers, "3", new Vector2(1100, 660), Color.Red);

                    spriteBatch.DrawString(upgradeFont, "MAX SPEED", new Vector2(1075,690), Color.Red);


                    //Draw the max stamina indicator
                    if (theWyrmPlayer.MaxStamina >= STAMINA_MAX)
                    {
                        spriteBatch.DrawString(upgradeFont, "MAX", new Vector2(1195,645), Color.Red);
                    }
                    else
                    {
                        spriteBatch.DrawString(upgradeFont, staminaCost + " KG", new Vector2(1180,645), Color.Red);
                    }

                    spriteBatch.DrawString(upgradeFontNumbers, "4", new Vector2(1200,660), Color.Red);

                    spriteBatch.DrawString(upgradeFont, "MAX STAMINA", new Vector2(1165,690), Color.Red);

                    #endregion

                    theHumanPlayer.Draw(spriteBatch);

                    if (p2HumanVictory)
                        spriteBatch.DrawString(scoreFont, "H U M A N S   W I N", new Vector2(500, 150), Color.Red);
                    else if (p2WyrmVictory)
                        spriteBatch.DrawString(scoreFont, "T H E  W Y R M  W Y N S", new Vector2(490, 150), Color.Red);

                    #endregion
                }

                for (int i = 0; i < damageIndicators.Count; i++)
                {
                    damageIndicators[i].Draw(spriteBatch);
                }

                if(gamePaused)
                {
                    #region Game Paused Mode
                    //Draw the partly-transparent black layer over the screen to darken it
                    spriteBatch.Draw(t2dtransparentBlack, new Rectangle(0, 0, SCREENWIDTH, SCREENHEIGHT), Color.White);

                    spriteBatch.DrawString(scoreFont, "Game Paused", new Vector2(525, 20), Color.Red);

                    //Draw the menu items
                    if(!returnToGameSelected)
                        spriteBatch.Draw(t2dupgradeBox, new Rectangle(505, 100, 250, 150), Color.White);
                    else
                        spriteBatch.Draw(t2dupgradeBoxOffColor, new Rectangle(505, 100, 250, 150), Color.White);

                    spriteBatch.DrawString(titleFont, "Return to game", new Vector2(550, 160), Color.Red);

                    if(!quitToMenuSelected)
                         spriteBatch.Draw(t2dupgradeBox, new Rectangle(505, 300, 250, 150), Color.White);
                    else
                        spriteBatch.Draw(t2dupgradeBoxOffColor, new Rectangle(505, 300, 250, 150), Color.White);

                    spriteBatch.DrawString(titleFont, "Quit to Main Menu", new Vector2(530, 360), Color.Red);

                    if (!quitToDesktopSelected)
                        spriteBatch.Draw(t2dupgradeBox, new Rectangle(505, 500, 250, 150), Color.White);
                    else
                        spriteBatch.Draw(t2dupgradeBoxOffColor, new Rectangle(505, 500, 250, 150), Color.White);

                    spriteBatch.DrawString(titleFont, "Quit to Desktop", new Vector2(545, 560), Color.Red);
                    #endregion
                }

                #endregion
            }
            else if (!m_gameStarted && !instructionMode)
            {
                #region Title Screen Mode (m_gameStarted == false)

                spriteBatch.Draw(t2dTitleScreen, new Rectangle(0, 0, SCREENWIDTH, SCREENHEIGHT), Color.White);

                /*if (gameTime.TotalGameTime.Milliseconds % 1000 < 700)
                {
                    spriteBatch.DrawString(titleFont, "Press Spacebar to BEGIN YOUR FEAST", vStartTitleTextLoc, Color.OrangeRed);
                    spriteBatch.DrawString(titleFont, "Press N to start the game with NUX MODE", new Vector2(410, 500), Color.OrangeRed);
                    spriteBatch.DrawString(titleFont, "Press T to ENGAGE TWO PLAYERS", new Vector2(460, 565), Color.OrangeRed);
                }*/

                if (singlePlayerGameSelected)
                    spriteBatch.Draw(blueBoxTitleScreen, new Rectangle(420, 440, 380, 25), Color.White);
                spriteBatch.DrawString(titleFont, "Begin your SINGLE PLAYER FEAST", new Vector2(440, 440), Color.OrangeRed);
                
                if(twoPlayerGameSelected)
                    spriteBatch.Draw(blueBoxTitleScreen, new Rectangle(420, 500, 380, 25), Color.White);
                spriteBatch.DrawString(titleFont, "Begin a TWO PLAYER BATTLE", new Vector2(470, 500), Color.OrangeRed);

                if(nuxModeSelected)
                    spriteBatch.Draw(blueBoxTitleScreen, new Rectangle(420, 565, 380, 25), Color.White);
                spriteBatch.DrawString(titleFont, "Experience NUX MODE", new Vector2(490, 565), Color.OrangeRed);

                if(quitToDesktop_MMSelected)
                    spriteBatch.Draw(blueBoxTitleScreen, new Rectangle(420, 620, 380, 25), Color.White);
                spriteBatch.DrawString(titleFont, "Quit to Desktop", new Vector2(520, 620), Color.OrangeRed);

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
                    spriteBatch.DrawString(titleFont, "Press ENTER to PROCEED", new Vector2(490, 650), Color.OrangeRed);
                }

                //Instruct the player on controls
                spriteBatch.DrawString(titleFont, "Press <-- to turn the Wyrm COUNTERCLOCKWISE.", new Vector2(430, 60), Color.OrangeRed);
                spriteBatch.DrawString(titleFont, "Press --> to turn the Wyrm CLOCKWISE.", new Vector2(430, 110), Color.OrangeRed);
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

        void startNewMultiPlayerGame()
        {
            //MediaPlayer.Volume = 0.25f;     // Reduce volume so we can hear the missle blast
            returnToGameSelected = false;
            quitToMenuSelected = false;
            quitToDesktopSelected = false;

            damageIndicators = new List<FloatingText>();

            preySpawner = new PreySpawner();
            PreySpawner.resetSpawnQueues();
            isTwoPlayer = true;

            singlePlayerVictory = false;
            p2WyrmVictory = false;
            p2HumanVictory = false;

            p2CanUpgradeRegen = true;
            p2CanUpgradeHealth = true;
            p2CanUpgradeSpeed = true;
            p2CanUpgradeSprint = true;

            theBackground = new Background(t2dbackgroundTwoPlayer, t2dforegroundTwoPlayer);

            m_gameStarted = true;

            //Add the wyrm head segment texture to the wyrm textures list
            List<Texture2D> wyrmTextures = new List<Texture2D>();

            WyrmPlayer.nuxMode = false;

            WyrmPlayer.LoadContent(Content);

            theWyrmPlayer = new WyrmPlayer();

            missileDownInstance.Play();

            explosions = new List<Explosion>();

            Prey.reInitializeAll();
            bullets = new List<Bullet>();
            Building.reInitializeAll();

            theHumanPlayer = new HumanPlayer(theWyrmPlayer);

            elapsedTimeGameEnd = 0;

            HumanPlayer.hasFactory = false;
            HumanPlayer.hasBarracks = false;
            HumanPlayer.incomeAdjustmentPerOilDerrick = HumanPlayer.DEFAULT_INCOME_ADJUSTMENT_PER_OIL_DERRICK;    //Reset the oil derrick to the default

            regenCost = DEFAULT_REGEN_COST;
            digSpeedCost = DEFAULT_DIGSPEED_COST;
            staminaCost = DEFAULT_STAMINA_COST;
            maxHealthCost = DEFAULT_MAXHEALTH_COST;
        }

        void startNewSinglePlayerGame(bool nuxMode)
        {
            returnToGameSelected = false;
            quitToMenuSelected = false;
            quitToDesktopSelected = false;

            damageIndicators = new List<FloatingText>();

            preySpawner = new PreySpawner();
            PreySpawner.resetSpawnQueues();
            isTwoPlayer = false;

            singlePlayerVictory = false;

            if (nuxMode)
                WyrmPlayer.nuxMode = true;
            else
                WyrmPlayer.nuxMode = false;

            WyrmPlayer.LoadContent(Content);

            theWyrmPlayer = new WyrmPlayer();

            theBackground = new Background(t2dbackgroundSinglePlayer, t2dforegroundSinglePlayer);

            Prey.reInitializeAll();
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

        void endSinglePlayerGame()
        {
            gamePaused = false;
            gamePausedCanSwitch = true;
            m_gameStarted = false;
            gameOver = false;
            singlePlayerGameSelected = true;
            twoPlayerGameSelected = false;
            nuxModeSelected = false;
            quitToDesktop_MMSelected = false;
            titleTransitionOk = false;
            missileDownInstance.Stop();
            nuxmode = false;
        }

        void endMultiPlayerGame()
        {
            gamePaused = false;
            gamePausedCanSwitch = true;
            m_gameStarted = false;
            gameOver = false;
            singlePlayerGameSelected = true;
            twoPlayerGameSelected = false;
            nuxModeSelected = false;
            quitToDesktop_MMSelected = false;
            titleTransitionOk = false;
            missileDownInstance.Stop();
            nuxmode = false;
        }

        void startNewWave(int wave)
        {
            Prey.prey = new List<Prey>();

            //For each prey type
            for (int i = 0; i < levelPrey.Count; i++)
            {
                //For each number of that type of prey this wave
                for (int j = 0; j < levelPrey[i][wave]; j++)
                {
                    if (i == Prey.GIRAFFE)
                        Prey.prey.Add(new Animal(m_random.Next(20, 1050), 100, Prey.preyTextures[Prey.GIRAFFE], 4, 95, 102, 94, 30, theWyrmPlayer.theWyrm, 1191, 97));
                    if (i == Prey.ELEPHANT)
                        Prey.prey.Add(new Animal(m_random.Next(20, 1050), 100, Prey.preyTextures[Prey.ELEPHANT], 6, 71, 93, 70, 29, theWyrmPlayer.theWyrm, 4990, 73));
                    if (i == Prey.UNARMEDHUMAN)
                        Prey.prey.Add(new Animal(m_random.Next(20, 1050), 100, Prey.preyTextures[Prey.UNARMEDHUMAN], 4, 24, 21, 23, 6, theWyrmPlayer.theWyrm, 80, 25));
                    if (i == Prey.SOLDIER)
                        Prey.prey.Add(new SoldierHuman(m_random.Next(20, 1050), 100, theWyrmPlayer.theWyrm));
                    if (i == Prey.MINE_LAYER)
                        Prey.prey.Add(new Engineer(m_random.Next(20, 1050), 100, theWyrmPlayer.theWyrm));
                    if (i == Prey.TANK)
                        Prey.prey.Add(new newTank(m_random.Next(20, 1050), 100, theWyrmPlayer.theWyrm));
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
            return (Math.Pow((r1 + r2), 2) >= Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2));
        }

        void checkEat()
        {
            for (int i = 0; i < Prey.prey.Count; i++)
            {
                //Do circular collision detection
                if (isColliding((int)(theWyrmPlayer.theWyrm.l_segments[0].X - WYRMHEAD_CENTER_NUMBER * Math.Cos(theWyrmPlayer.theWyrm.HeadDirection)),
                    (int)(theWyrmPlayer.theWyrm.l_segments[0].Y + QUARTER_OF_WYRMHEAD_SPRITEHEIGHT - WYRMHEAD_CENTER_NUMBER * Math.Sin(theWyrmPlayer.theWyrm.HeadDirection)),
                    theWyrmPlayer.theWyrm.eatRadius,
                    (int)Prey.prey[i].xPosistion, (int)Prey.prey[i].yPosition, Prey.prey[i].boundingRadius))
                {
                    Prey.prey[i].getEaten(theWyrmPlayer);
                    Prey.prey.RemoveAt(i);

                    if (theWyrmPlayer.Health <= 0 && !WyrmPlayer.nuxMode)
                    {
                        gameOver = true;
                        if (isTwoPlayer)
                            p2HumanVictory = true;
                    }
                }
            }

        }

        void checkBuildingCollisions()
        {
            for (int i = 0; i < Building.buildings.Count; i++)
            {
                //Do circular collision detection
                if (isColliding((int)(theWyrmPlayer.theWyrm.l_segments[0].X - WYRMHEAD_CENTER_NUMBER * Math.Cos(theWyrmPlayer.theWyrm.HeadDirection)),
                    (int)(theWyrmPlayer.theWyrm.l_segments[0].Y + QUARTER_OF_WYRMHEAD_SPRITEHEIGHT - WYRMHEAD_CENTER_NUMBER * Math.Sin(theWyrmPlayer.theWyrm.HeadDirection)),
                    theWyrmPlayer.theWyrm.eatRadius,
                    Building.buildings[i].getBoundingX(), Building.buildings[i].getBoundingY(), Building.buildings[i].boundingRadius))
                {
                    if(!Building.buildings[i].DamagedThisJump && Building.buildings[i].CanBeDamaged)
                        Building.buildings[i].takeDamage();

                    Building.buildings[i].DamagedThisJump = true;
                    Building.buildings[i].CanBeDamaged = false;
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

                if (Background.checkIsGrounded((int)bullets[i].xPosistion, (int)bullets[i].yPosition))
                {
                    bullets.RemoveAt(i);
                    continue;
                }

                //Check to see if the bullet is colliding with the head of the wyrm (discounting the mandibles)
                if (isColliding((int)(theWyrmPlayer.theWyrm.l_segments[0].X - WYRMHEAD_CENTER_NUMBER * Math.Cos(theWyrmPlayer.theWyrm.HeadDirection)),
                    (int)(theWyrmPlayer.theWyrm.l_segments[0].Y + QUARTER_OF_WYRMHEAD_SPRITEHEIGHT - WYRMHEAD_CENTER_NUMBER * Math.Sin(theWyrmPlayer.theWyrm.HeadDirection)),
                    theWyrmPlayer.theWyrm.eatRadius,
                    (int)bullets[i].xPosistion, (int)bullets[i].yPosition, bullets[i].boundingRadius))
                {
                    theWyrmPlayer.Health -= bullets[i].DamageDealt;

                    if (theWyrmPlayer.Health <= 0 && !WyrmPlayer.nuxMode)
                    {
                        gameOver = true;
                        if (isTwoPlayer)
                            p2HumanVictory = true;
                    }

                    damageIndicators.Add(new FloatingText((int)bullets[i].xPosistion, (int)bullets[i].yPosition, "-" + bullets[i].DamageDealt, Color.Red));

                    bullets.RemoveAt(i);

                    continue;
                }

                foreach (WyrmSegment ws in theWyrmPlayer.theWyrm.l_segments)
                {
                    if (isColliding((int)ws.X, (int)ws.Y + QUARTER_OF_WYRMHEAD_SPRITEHEIGHT, ws.boundingRadius, (int)bullets[i].xPosistion, (int)bullets[i].yPosition, bullets[i].boundingRadius))
                    {
                        theWyrmPlayer.Health -= bullets[i].DamageDealt;

                        if (theWyrmPlayer.Health <= 0 && !WyrmPlayer.nuxMode)
                        {
                            gameOver = true;
                            if (isTwoPlayer)
                                p2HumanVictory = true;
                        }

                        damageIndicators.Add(new FloatingText((int)bullets[i].xPosistion, (int)bullets[i].yPosition, "-" + bullets[i].DamageDealt, Color.Red));

                        bullets.RemoveAt(i);

                        break;
                    }
                }
            }
        }
    }
}
