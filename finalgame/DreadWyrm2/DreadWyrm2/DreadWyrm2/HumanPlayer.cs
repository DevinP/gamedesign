using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DreadWyrm2
{
    class HumanPlayer
    {
        int totalMoney;

        int mouseX;
        int mouseY;

        bool canClick;

        Wyrm theWyrm;

        static Texture2D mouseTex;

        const int ADDTROOPS_X = 10;
        const int ADDTROOPS_Y = 10;
        const int ADDTROOPS_WIDTH = 15;
        const int ADDTROOPS_HEIGHT = 15;

        public int money
        {
            get { return totalMoney; }
            set { totalMoney = value; }
        }


        //Constructor
        public HumanPlayer(WyrmPlayer otherPlayer)
        {
            theWyrm = otherPlayer.theWyrm;

            canClick = true;
            money = 0;
        }

        public static void LoadContent(ContentManager Content)
        {
            mouseTex = Content.Load<Texture2D>(@"Textures\cursor");
        }

        public void Update(GameTime gameTime)
        {
            //Poll the mouse
            MouseState mState = Mouse.GetState();

            mouseX = mState.X;
            mouseY = mState.Y;

            bool deployATroop = false;

            if (mState.LeftButton == ButtonState.Pressed && canClick)
            {
                canClick = false;
                deployATroop = locationClicked(new Rectangle(ADDTROOPS_X, ADDTROOPS_Y, ADDTROOPS_WIDTH, ADDTROOPS_HEIGHT));
            }
            else if (mState.LeftButton == ButtonState.Released && !canClick)
                canClick = true;

            if (deployATroop)
            {
                //Prey.prey.Add(new SoldierHuman(50, 50, theWyrm));
                Prey.prey.Add(new Animal(Game1.m_random.Next(20, 1050), 100, Prey.preyTextures[Prey.ELEPHANT], 6, 71, 93, 70, 29, theWyrm, 4990, 73));
            }

        }

        public void Draw(SpriteBatch sb)
        {
            //Finally, draw the mouse cursor
            sb.Draw(mouseTex, new Rectangle(mouseX, mouseY, 32, 32), Color.White);
        }


        /// <summary>
        /// This is used to check if a mouse click position is inside of a button.
        /// </summary>
        /// <param name="button">Rectangle representing the target button.</param>
        bool locationClicked(Rectangle button)
        {
            return (mouseX < button.X + button.Width && mouseX > button.X && mouseY > button.Y && mouseY < button.Y + button.Height);
        }


    }
}
