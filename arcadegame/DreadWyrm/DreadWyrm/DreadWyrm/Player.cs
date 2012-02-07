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
        int i_Health = 0;
        int i_HealthMax = 0;

        const int SCOREX = 950;
        const int SCOREY = 650;

        public int playerID
        {
            get { return i_playerID; }
            set { i_playerID = value; }
        }

        public int Health
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


        //Constructor
        public Player(int ID, List<Texture2D> wyrmTextures, SpriteFont font)
        {
            i_playerID = ID;

            //Create a Wyrm
            theWyrm = new Wyrm(100, 400/*-250*/, wyrmTextures, Game1.WYRMSEGS);

            scoreFont = font;
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

                #endregion
            }
            else
            {
                #region The wyrm is in the air (the player has limited control)

                //Stop the player from communicating with the wyrm while it is affected by gravity, save for turning at a reduced rate
                theWyrm.HeadAcceleration = 0;

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

            theWyrm.Update(gametime);
        }

        public void Draw(SpriteBatch sb)
        {
            theWyrm.Draw(sb);

            sb.DrawString(scoreFont, "Total Meat: " + totalMeat + " kg", new Vector2(SCOREX, SCOREY), Color.Red);
        }

    }
}
