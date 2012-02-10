using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DreadWyrm
{
    class Player
    {
        //game variables
        int i_playerID;
        public Wyrm theWyrm;
        int totalMeat;
        SpriteFont scoreFont;

        //health
        float i_Health = 75;
        int i_HealthMax = 100;
        Texture2D hb_base;
        Texture2D healthBar;
        public bool regen = false;
        const int REGEN_DURATION = 5000;      //Number of milliseconds health regen lasts for
        public float REGEN_FACTOR = 0.25f;    //The percent of max health regenerated over the course of the health regen
        float elapsedTimeTotal = 0;
        float elapsedTime = 0;

        //Speed bursting
        bool canBurst = true;
        bool burst = false;
        const int STAMINA_DEPLETIONRATE = 2; //A point of stamina is two milliseconds of speed bursting
        const int STAMINA_RECHARGERATE = 5;  //One point of stamina regenerates every 5 milliseconds
        float stamina = 0;
        int staminaMax = 250;
        Texture2D staminaBar;

        const int SCOREX = 920;
        const int SCOREY = 600;

        public int playerID
        {
            get { return i_playerID; }
            set { i_playerID = value; }
        }

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
        public Player(int ID, List<Texture2D> wyrmTextures, SpriteFont font, Texture2D healthBase, Texture2D health, Texture2D stam)
        {
            i_playerID = ID;

            //Create a Wyrm
            theWyrm = new Wyrm(100, 400, wyrmTextures, Game1.WYRMSEGS);

            scoreFont = font;

            hb_base = healthBase;
            healthBar = health;
            staminaBar = stam;
            stamina = staminaMax;
        }

        public void Update(GameTime gametime, KeyboardState keystate)
        {
            if (theWyrm.b_wyrmGrounded)
            {
                #region Wyrm is grounded (player has control)

                if (keystate.IsKeyDown(Keys.Up))
                    theWyrm.HeadAcceleration = 0.3f;
                else if (keystate.IsKeyUp(Keys.Up))
                    theWyrm.HeadAcceleration = -0.1f;

                if (keystate.IsKeyDown(Keys.Right))
                {
                    theWyrm.HeadRotationSpeed = 0.05f;
                }
                else if (keystate.IsKeyDown(Keys.Left))
                {
                    theWyrm.HeadRotationSpeed = -0.05f;
                }
                else if (keystate.IsKeyUp(Keys.Right) && keystate.IsKeyUp(Keys.Left))
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
                else if(wyrmDir > Math.PI / 2 && wyrmDir < (3*Math.PI) / 2)
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

            if (regen)
            {
                //A health regen upgrade has been purchased
                //Regenerate some health every millisecond for REGEN_DURATION seconds
                //The amount of health regenerated is REGEN_FACTOR * max health

                //First, accumulate time
                elapsedTimeTotal += (float)gametime.ElapsedGameTime.TotalMilliseconds;
                elapsedTime += (float)gametime.ElapsedGameTime.TotalMilliseconds;

                //Stop regen if the duration is complete
                if (elapsedTimeTotal >= REGEN_DURATION)
                {
                    regen = false;
                }
                else
                {
                    //First, calculate the amount of health healed per millisecond
                    //This will be (Total Health Regenerated) / (Duration of Regen)
                    float healthPerMS = (REGEN_FACTOR * i_HealthMax) / (REGEN_DURATION);
                    Console.WriteLine("Health regernerated per MS: " + healthPerMS);

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
                elapsedTimeTotal = 0;

            //Reset the elapsed time
            elapsedTime = 0;

            theWyrm.Update(gametime);
        }

        public void Draw(SpriteBatch sb)
        {
            theWyrm.Draw(sb);

            sb.DrawString(scoreFont, "Total Meat: " + totalMeat + " KG", new Vector2(SCOREX, SCOREY), Color.Red);

            //Draw the health bars
            sb.Draw(hb_base, new Rectangle(50, 650, i_HealthMax, 25), Color.White);
            sb.Draw(healthBar, new Rectangle(50, 650, (int)i_Health, 25), Color.White);

            //Draw the stamina bars
            sb.Draw(hb_base, new Rectangle(50, 680, staminaMax, 25), Color.White);
            sb.Draw(staminaBar, new Rectangle(50, 680, (int)stamina, 25), Color.White);
        }

    }
}
