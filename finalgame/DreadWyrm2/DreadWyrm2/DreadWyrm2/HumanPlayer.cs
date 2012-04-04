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
        int totalMoney = 100;

        static SpriteFont humanFont;

        int mouseX;
        int mouseY;

        bool canClick;

        Wyrm theWyrm;

        static Texture2D mouseTex;

        List<HUDElement> buttons;

        /////////////////////////////////////////
        //UI element locations, sizes, and textures

        //Turret button
        static Texture2D turretButtonTex;
        const int TURRET_X = 10;
        const int TURRET_Y = 10;
        const int TURRETBUTTON_WIDTH = 75;
        const int TURRETBUTTON_HEIGHT = 75;

        //Oil derrick button
        static Texture2D oilDerrickButtonTex;
        const int OILDERRICK_X = 115;
        const int OILDERRICK_Y = 10;
        const int OILDERRICK_WIDTH = 75;
        const int OILDERRICK_HEIGHT = 75;

        //Barracks button
        static Texture2D barracksButtonTex;
        const int BARRACKS_X = 220;
        const int BARRACKS_Y = 10;
        const int BARRACKS_WIDTH = 75;
        const int BARRACKS_HEIGHT = 75;

        //The button to recruit soldiers from a barracks
        static Texture2D soldierRecruitTex;
        const int ADDTROOPS_X = 225;
        const int ADDTROOPS_Y = 90;
        const int ADDTROOPS_WIDTH = 25;
        const int ADDTROOPS_HEIGHT = 25;

        //The button to recruit engineers from a barracks
        static Texture2D engineerRecruitTex;
        const int ADDENGINEER_X = 265;
        const int ADDENGINEER_Y = 90;
        const int ADDENGINEER_WIDTH = 25;
        const int ADDENGINEER_HEIGHT = 25;

        //Factory button
        static Texture2D factoryButtonTex;
        const int FACTORY_X = 325;
        const int FACTORY_Y = 10;
        const int FACTORY_WIDTH = 75;
        const int FACTORY_HEIGHT = 75;

        //The button to recruit tanks from the factory
        static Texture2D tankRecruitTex;
        const int ADDTANK_X = 350;
        const int ADDTANK_Y = 90;
        const int ADDTANK_WIDTH = 25;
        const int ADDTANK_HEIGHT = 25;

        //Indices for the HUD elements
        const int TURRET = 0;
        const int OILDERRICK = 1;
        const int BARRACKS = 2;
        const int RECRUIT_SOLDIER = 3;
        const int RECRUIT_ENGINEER = 4;
        const int FACTORY = 5;
        const int RECRUIT_TANK = 6;
        /////////////////////////////////////////

        //Turret purchasing and placement variables
        static Texture2D ghostTurret;
        bool drawGhostTurret = false;
        const int TURRET_WIDTH = 60;
        const int TURRET_HEIGHT = 34;

        //Mouse cursor size
        const int MOUSECURSOR_WIDTH = 32;
        const int MOUSECURSOR_HEIGHT = 32;

        //Building and unit costs
        const int TURRET_COST = 25;
        const int OILDERRICK_COST = 15;
        const int BARRACKS_COST = 30;
        const int SOLDIER_COST = 5;
        const int ENGINEER_COST = 8;
        const int FACTORY_COST = 60;
        const int TANK_COST = 20;

        public int money
        {
            get { return totalMoney; }
            set { totalMoney = value; }
        }


        //Constructor
        public HumanPlayer(WyrmPlayer otherPlayer)
        {
            theWyrm = otherPlayer.theWyrm;

            buttons = new List<HUDElement>();

            //Turret button
            buttons.Add(new HUDElement(turretButtonTex, new Rectangle(TURRET_X, TURRET_Y, TURRETBUTTON_WIDTH, TURRETBUTTON_HEIGHT)));

            //Oil derrick button
            buttons.Add(new HUDElement(oilDerrickButtonTex, new Rectangle(OILDERRICK_X, OILDERRICK_Y, OILDERRICK_WIDTH, OILDERRICK_HEIGHT)));

            //Barracks button
            buttons.Add(new HUDElement(barracksButtonTex, new Rectangle(BARRACKS_X, BARRACKS_Y, BARRACKS_WIDTH, BARRACKS_HEIGHT)));

            //Soldier recruitment button
            buttons.Add(new HUDElement(soldierRecruitTex, new Rectangle(ADDTROOPS_X, ADDTROOPS_Y, ADDTROOPS_WIDTH, ADDTROOPS_HEIGHT)));

            buttons.Add(new HUDElement(engineerRecruitTex, new Rectangle(ADDENGINEER_X, ADDENGINEER_Y, ADDENGINEER_WIDTH, ADDENGINEER_HEIGHT)));

            //Factory button
            buttons.Add(new HUDElement(factoryButtonTex, new Rectangle(FACTORY_X, FACTORY_Y, FACTORY_WIDTH, FACTORY_HEIGHT)));

            //Tank recruitment button
            buttons.Add(new HUDElement(tankRecruitTex, new Rectangle(ADDTANK_X, ADDTANK_Y, ADDTANK_WIDTH, ADDTANK_HEIGHT)));

            canClick = true;
        }

        /// <summary>
        /// Loads assets needed by this class
        /// </summary>
        /// <param name="Content">A reference to the game's Content Manager</param>
        public static void LoadContent(ContentManager Content)
        {

            humanFont = Content.Load<SpriteFont>(@"Fonts\scoreFont");

            mouseTex = Content.Load<Texture2D>(@"Textures\cursor");

            turretButtonTex = Content.Load<Texture2D>(@"Textures\TurretIcon");
            oilDerrickButtonTex = Content.Load<Texture2D>(@"Textures\OilDerrickIcon");
            barracksButtonTex = Content.Load<Texture2D>(@"Textures\BarracksIcon");
            soldierRecruitTex = Content.Load<Texture2D>(@"Textures\soldierIcon");
            engineerRecruitTex = Content.Load<Texture2D>(@"Textures\engineerIcon");
            factoryButtonTex = Content.Load<Texture2D>(@"Textures\FactoryIcon");
            tankRecruitTex = Content.Load<Texture2D>(@"Textures\addUnit");

            ghostTurret = Content.Load<Texture2D>(@"Textures\ghostTurret");
        }

        public void Update(GameTime gameTime)
        {
            //Poll the mouse
            MouseState mState = Mouse.GetState();

            mouseX = mState.X;
            mouseY = mState.Y;

            bool clickedSoldier = false;
            bool clickedEngineer = false;
            bool clickedTank = false;
            bool clickedTurret = false;

            if (mState.LeftButton == ButtonState.Pressed && canClick)
            {
                canClick = false;

                clickedSoldier = buttons[RECRUIT_SOLDIER].isWithin(new Vector2(mouseX, mouseY));
                clickedEngineer = buttons[RECRUIT_ENGINEER].isWithin(new Vector2(mouseX, mouseY));
                clickedTank = buttons[RECRUIT_TANK].isWithin(new Vector2(mouseX, mouseY));
                clickedTurret = buttons[TURRET].isWithin(new Vector2(mouseX, mouseY));

                if (drawGhostTurret)
                {
                    //Place the turret at this location (mouseX, 419)
                    Building.buildings.Add(new Turret(mouseX, 419, theWyrm));
                    drawGhostTurret = false;

                }
            }
            else if (mState.LeftButton == ButtonState.Released && !canClick)
                canClick = true;

            if (clickedSoldier && (totalMoney - SOLDIER_COST >= 0))
            {
                Prey.prey.Add(new SoldierHuman(Game1.m_random.Next(20, 1050), 100, theWyrm));

                totalMoney -= SOLDIER_COST;
            }
            else if (clickedTank && (totalMoney - TANK_COST >= 0))
            {
                Prey.prey.Add(new Tank(Game1.m_random.Next(20, 1050), 100, theWyrm));

                totalMoney -= TANK_COST;
            }

            else if (clickedEngineer && (totalMoney - ENGINEER_COST >= 0))
            {
                Prey.prey.Add(new Engineer(Game1.m_random.Next(20, 1050), 100, theWyrm));

                totalMoney -= ENGINEER_COST;
            }
            else if (clickedTurret && !drawGhostTurret && (totalMoney - TURRET_COST >= 0))
            {
                drawGhostTurret = true;
                totalMoney -= TURRET_COST;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            //Draw a ghost turret if we need to
            if (drawGhostTurret)
            {
                sb.Draw(ghostTurret, new Rectangle(mouseX, 419, TURRET_WIDTH, TURRET_HEIGHT), Color.White);
            }

            //Draw all of the HUDElements
            for (int i = 0; i < buttons.Count; i++)
                buttons[i].Draw(sb);

            //Draw the total money of the human player
            sb.DrawString(humanFont, "Funds: $" + totalMoney, new Vector2(1050, 10), Color.Black);

            //Finally, draw the mouse cursor
            sb.Draw(mouseTex, new Rectangle(mouseX, mouseY, MOUSECURSOR_WIDTH, MOUSECURSOR_HEIGHT), Color.White);
        }
    }
}
