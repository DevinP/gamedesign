using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace DreadWyrm2
{
    public class WyrmPlayer
    {
        //game variables
        public Wyrm theWyrm;
        int totalMeat = 0;
        static SpriteFont scoreFont;

        //health
        public float i_Health = 100;
        public int i_HealthMax = 100;
        public static Texture2D hb_base;
        public static Texture2D healthBar;
        public static Texture2D regenBar;
        public int regen = 0;
        public int REGEN_DURATION = 5000;      //Number of milliseconds health regen lasts for
        public float REGEN_FACTOR = 0.25f;     //The percent of max health regenerated over the course of the health regen
        public float elapsedTimeTotalRegen = 0;
        public float elapsedTime = 0;
        public float healthPerMS;
        public float healthAfterRegen;

        //Speed bursting
        bool canBurst = true;
        bool burst = false;
        const int STAMINA_DEPLETIONRATE = 2; //A point of stamina is two milliseconds of speed bursting
        const int STAMINA_RECHARGERATE = 10;  //One point of stamina regenerates every 5 milliseconds
        public float stamina = 0;
        public int staminaMax = 200;
        public static Texture2D staminaBar;

        const int SCOREX = 920;
        const int SCOREY = 600;

        public static bool nuxMode = false;

        public float Health
        {
            get { return i_Health; }
            set { i_Health = (int)MathHelper.Clamp(value, 0, i_HealthMax); }
        }

        public int HealthMax
        {
            get { return i_HealthMax; }
            set { i_HealthMax = value; }
        }

        public int Meat
        {
            get { return totalMeat; }
            set { totalMeat = value; }
        }

        public int MaxStamina
        {
            get { return staminaMax; }
            set { staminaMax = value; }
        }


        //Constructor
        public WyrmPlayer()
        {
            //Create a Wyrm
            theWyrm = new Wyrm(100, -3650);

            stamina = staminaMax;
        }

        public static void LoadContent(ContentManager Content)
        {
            Wyrm.LoadContent(Content);

            scoreFont = Content.Load<SpriteFont>(@"Fonts\scoreFont");

            hb_base = Content.Load<Texture2D>(@"Textures\hb_red");
            healthBar = Content.Load<Texture2D>(@"Textures\hb_green");
            staminaBar = Content.Load<Texture2D>(@"Textures\hb_yellow");
            regenBar = Content.Load<Texture2D>(@"Textures\hb_orange");
        }

        public void Update(GameTime gametime, KeyboardState keystate)
        {
            if (theWyrm.b_wyrmGrounded)
            {
                #region Wyrm is grounded (player has control)

                if (keystate.IsKeyDown(Keys.Up) || keystate.IsKeyDown(Keys.W))
                    theWyrm.HeadAcceleration = 0.3f;
                else if (keystate.IsKeyUp(Keys.Up))
                    theWyrm.HeadAcceleration = -0.1f;

                if (keystate.IsKeyDown(Keys.Right) || keystate.IsKeyDown(Keys.D))
                {
                    theWyrm.HeadRotationSpeed = 0.05f;
                }
                else if (keystate.IsKeyDown(Keys.Left) || keystate.IsKeyDown(Keys.A))
                {
                    theWyrm.HeadRotationSpeed = -0.05f;
                }
                else if (keystate.IsKeyUp(Keys.Right) && keystate.IsKeyUp(Keys.Left) && 
                    keystate.IsKeyUp(Keys.A) && keystate.IsKeyUp(Keys.D))
                {
                    theWyrm.HeadRotationSpeed = 0;
                }

                // Begin bursting
                if (keystate.IsKeyDown(Keys.LeftShift) && canBurst)
                    burst = true;

                //Stop bursting
                if (keystate.IsKeyUp(Keys.LeftShift) && canBurst)
                    burst = false;

                if (burst)
                {
                    //Set the max speed to be the boost speed
                    theWyrm.HeadSpeedMax = theWyrm.HeadSpeedBoostMax;

                    //Deplete some stamina
                    stamina -= (float)gametime.ElapsedGameTime.TotalMilliseconds / STAMINA_DEPLETIONRATE;
                    if (stamina <= 0)
                    {
                        //We ran out of stamina. Stop bursting
                        stamina = 0;
                        burst = false;
                        canBurst = false;
                    }
                }


                #endregion
            }
            else
            {
                #region The wyrm is in the air (the player has limited control)

                //Stop the player from communicating with the wyrm while it is affected by gravity, save for turning at a reduced rate
                theWyrm.HeadAcceleration = 0;

                //If the wyrm is bursting, stop bursting
                burst = false;

                float wyrmDir = theWyrm.HeadDirection;

                if (wyrmDir < Math.PI / 2 || wyrmDir > (3 * Math.PI) / 2)
                {
                    if (keystate.IsKeyDown(Keys.Right))
                    {
                        theWyrm.HeadRotationSpeed = 0.05f;
                    }
                    else
                    {
                        theWyrm.HeadRotationSpeed = 0;
                    }
                }
                else if (wyrmDir > Math.PI / 2 && wyrmDir < (3 * Math.PI) / 2)
                {
                    if (keystate.IsKeyDown(Keys.Left))
                    {
                        theWyrm.HeadRotationSpeed = -0.05f;
                    }
                    else
                    {
                        theWyrm.HeadRotationSpeed = 0;
                    }
                }

                if (keystate.IsKeyUp(Keys.Right) && keystate.IsKeyUp(Keys.Left))
                {
                    theWyrm.HeadRotationSpeed = 0;
                }

                #endregion
            }


            if (!burst)
            {
                burst = false;
                canBurst = true;

                //Reset the max speed
                theWyrm.HeadSpeedMax = theWyrm.HeadSpeedNormalMax;

                //Recharge stamina
                stamina += (float)gametime.ElapsedGameTime.TotalMilliseconds / STAMINA_RECHARGERATE;

                if (stamina >= staminaMax)
                    stamina = staminaMax;

            }

            //If at any time we reach max health, we will stop regen
            if (i_Health >= i_HealthMax)
                regen = 0;

            if (regen > 0)
            {
                //A health regen upgrade has been purchased
                //Regenerate some health every millisecond for REGEN_DURATION seconds
                //The amount of health regenerated is REGEN_FACTOR * max health

                //First, accumulate time
                elapsedTimeTotalRegen += (float)gametime.ElapsedGameTime.TotalMilliseconds;
                elapsedTime = (float)gametime.ElapsedGameTime.TotalMilliseconds;

                //First, calculate the amount of health healed per millisecond
                //This will be (Total Health Regenerated) / (Duration of Regen)
                healthPerMS = (REGEN_FACTOR * i_HealthMax) / (REGEN_DURATION);

                //Stop regen if the duration is complete
                if (elapsedTimeTotalRegen >= REGEN_DURATION)
                {
                    regen--;
                    if (regen <= 0)
                        regen = 0;

                    elapsedTimeTotalRegen = 0;
                }
                else
                {
                    //For every millisecond that passes, add on one health per millisecond
                    for (int i = 0; i < elapsedTime; i++)
                    {
                        i_Health += healthPerMS;
                        if (i_Health >= i_HealthMax)
                            i_Health = i_HealthMax;
                    }
                }
            }
            else //We are not regenerating, so the total time should be reset
                elapsedTimeTotalRegen = 0;

            healthAfterRegen = i_Health + ((REGEN_DURATION - elapsedTimeTotalRegen) * healthPerMS) + (REGEN_DURATION * healthPerMS * (regen - 1));

            theWyrm.Update(gametime);

            if (nuxMode && i_Health <= 0)
                i_Health = 1;
        }

        public void Draw(SpriteBatch sb)
        {
            theWyrm.Draw(sb);

            sb.DrawString(scoreFont, "Total Meat: " + totalMeat + " KG", new Vector2(SCOREX, SCOREY), Color.Red);

            if (healthAfterRegen >= i_HealthMax)
                healthAfterRegen = i_HealthMax;

            //Draw the health bars
            sb.Draw(hb_base, new Rectangle(50, 650, i_HealthMax, 25), Color.White);
            sb.Draw(regenBar, new Rectangle(50, 650, (int)healthAfterRegen, 25), Color.White);
            sb.Draw(healthBar, new Rectangle(50, 650, (int)i_Health, 25), Color.White);

            //Draw the stamina bars
            sb.Draw(hb_base, new Rectangle(50, 680, staminaMax, 25), Color.White);
            sb.Draw(staminaBar, new Rectangle(50, 680, (int)stamina, 25), Color.White);
        }
    }
}
