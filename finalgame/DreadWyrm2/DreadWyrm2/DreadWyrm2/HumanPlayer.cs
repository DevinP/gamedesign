using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DreadWyrm2
{
    class HumanPlayer
    {
        int totalMoney;

        int mouseX;
        int mouseY;

        Texture2D mouseTex;

        public int money
        {
            get { return totalMoney; }
            set { totalMoney = value; }
        }


        //Constructor
        public HumanPlayer(Texture2D mouseTexture)
        {
            mouseTex = mouseTexture;

            money = 0;
        }

        public void Update(GameTime gameTime)
        {
            //Poll the mouse
            MouseState mState = Mouse.GetState();

            mouseX = mState.X;
            mouseY = mState.Y;
        }

        public void Draw(SpriteBatch sb)
        {
            //Finally, draw the mouse cursor
            sb.Draw(mouseTex, new Rectangle(mouseX, mouseY, 32, 32), Color.White);
        }

    }
}
