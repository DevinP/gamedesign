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
        //The 0-indexed number of segments in this player's Wyrm
        const int WYRMSEGMENTS = 1;

        //game variables
        int i_playerID;
        Wyrm theWyrm;

        //health
        int i_Health = 0;
        int i_HealthMax = 0;

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


        //Constructor
        public Player(int ID, List<Texture2D> wyrmTextures)
        {
            i_playerID = ID;

            //Create a Wyrm
            theWyrm = new Wyrm(0, 0, wyrmTextures, WYRMSEGMENTS);
        }

        public void Update(GameTime gametime, KeyboardState keystate)
        {

            if (keystate.IsKeyDown(Keys.Up))
                theWyrm.HeadAcceleration = 1;
            else if (keystate.IsKeyUp(Keys.Up))
                theWyrm.HeadAcceleration = -0.1f;

            if (keystate.IsKeyDown(Keys.Right))
            {
                theWyrm.HeadRotationSpeed = -0.05f;
            }
            else if (keystate.IsKeyDown(Keys.Left))
            {
                theWyrm.HeadRotationSpeed = 0.05f;
            }
            else if (keystate.IsKeyUp(Keys.Right) && keystate.IsKeyUp(Keys.Left))
            {
                theWyrm.HeadRotationSpeed = 0;
            }

            theWyrm.Update(gametime);
        }

        public void Draw(SpriteBatch sb)
        {
            theWyrm.Draw(sb);
        }


    }
}
