using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreadWyrm
{
    class Player
    {
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
            theWyrm = new Wyrm(0, 0, wyrmTextures, 6);
        }

        public void Update(GameTime gametime)
        {

        }


    }
}
