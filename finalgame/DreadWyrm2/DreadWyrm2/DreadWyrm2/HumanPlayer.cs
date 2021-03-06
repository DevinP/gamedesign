﻿using System;
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
        float incomeCounter = 0;
        const float INCOME_LIMIT = 1000;                            //The number of milliseconds between income accumulations
        const float BASE_INCOME = 100;                              //Base income per 5 seconds that can't be touched by the wyrm
        public static float incomeAdjustmentPerOilDerrick = 100;    //The number of dollars per 5 seconds an oil derrick provides
        public static float DEFAULT_INCOME_ADJUSTMENT_PER_OIL_DERRICK = 100;
        public static float INCOME_REDUCTION_PER_OIL_DERRICK = 2f;

        //Building and unit costs
        const int TURRET_COST = 2500;
        const int OILDERRICK_COST = 1500;
        const int BARRACKS_COST = 3000;
        const int SOLDIER_COST = 500;
        const int ENGINEER_COST = 800;
        const int FACTORY_COST = 6000;
        const int TANK_COST = 2000;

        float totalMoney = 6000;
        public static int numOilDerricks = 0;

        static SpriteFont humanFontFunds;
        static SpriteFont humanFontCosts;

        int mouseX;
        int mouseY;

        bool canClick;

        Wyrm theWyrm;

        static Texture2D mouseTex;

        List<HUDElement> buttons;

        List<FloatingText> incomeIndicators;

        public static bool hasBarracks = false;
        public static bool hasFactory = false;

        /////////////////////////////////////////
        //UI element locations, sizes, and textures

        //Turret button
        static Texture2D turretButtonTex;
        static Texture2D disabledTurretButtonTex;
        const int TURRET_X = 10;
        const int TURRET_Y = 10;
        const int TURRETBUTTON_WIDTH = 75;
        const int TURRETBUTTON_HEIGHT = 75;

        //Oil derrick button
        static Texture2D oilDerrickButtonTex;
        static Texture2D disabledOilDerrickButtonTex;
        const int OILDERRICK_X = 115;
        const int OILDERRICK_Y = 10;
        const int OILDERRICK_BUTTON_WIDTH = 75;
        const int OILDERRICK_BUTTON_HEIGHT = 75;

        //Barracks button
        static Texture2D barracksButtonTex;
        static Texture2D disabledBarracksButtonTex;
        const int BARRACKS_X = 220;
        const int BARRACKS_Y = 10;
        const int BARRACKS_BUTTON_WIDTH = 75;
        const int BARRACKS_BUTTON_HEIGHT = 75;

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
        static Texture2D disabledFactoryButtonTex;
        const int FACTORY_X = 325;
        const int FACTORY_Y = 10;
        const int FACTORY_BUTTON_WIDTH = 75;
        const int FACTORY_BUTTON_HEIGHT = 75;

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
        const int TURRET_DISABLED = 7;
        const int OILDERRICK_DISABLED = 8;
        const int BARRACKS_DISABLED = 9;
        const int FACTORY_DISABLED = 10;
        /////////////////////////////////////////

        //Turret purchasing and placement variables
        static Texture2D ghostTurret;
        bool drawGhostTurret = false;
        const int TURRET_WIDTH = 60;
        const int TURRET_HEIGHT = 34;

        //Oil derrick purchasing and placement variables
        static Texture2D ghostOilDerrick;
        bool drawGhostOilDerrick = false;
        const int OIL_DERRICK_WIDTH = 71;
        const int OIL_DERRICK_HEIGHT = 100;

        //Barracks purchasing and placement variables
        static Texture2D ghostBarracks;
        bool drawGhostBarracks = false;
        const int BARRACKS_WIDTH = 116;
        const int BARRACKS_HEIGHT = 70;
        const int XCOORD_BARRACKS_DOOR = 19;
        const int YCOORD_BARRACKS_DOOR = 49;
        Vector2 barracksLoc;

        //Factory purchasing and placement variables
        static Texture2D ghostFactory;
        bool drawGhostFactory = false;
        const int FACTORY_WIDTH = 121;
        const int FACTORY_HEIGHT = 100;
        const int XCOORD_FACTORY_DOOR = 65;
        const int YCOORD_FACTORY_DOOR = 76;
        Vector2 factoryLoc;

        //Mouse cursor size
        const int MOUSECURSOR_WIDTH = 32;
        const int MOUSECURSOR_HEIGHT = 32;

        public float money
        {
            get { return totalMoney; }
            set { totalMoney = value; }
        }


        //Constructor
        public HumanPlayer(WyrmPlayer otherPlayer)
        {
            theWyrm = otherPlayer.theWyrm;

            barracksLoc = new Vector2();
            factoryLoc = new Vector2();
            incomeIndicators = new List<FloatingText>();

            //Create the generator
            Building.buildings.Add(new Generator(Game1.m_random.Next(200, 1080), 330, theWyrm));

            buttons = new List<HUDElement>();

            //Turret button
            buttons.Add(new HUDElement(turretButtonTex, new Rectangle(TURRET_X, TURRET_Y, TURRETBUTTON_WIDTH, TURRETBUTTON_HEIGHT)));

            //Oil derrick button
            buttons.Add(new HUDElement(oilDerrickButtonTex, new Rectangle(OILDERRICK_X, OILDERRICK_Y, OILDERRICK_BUTTON_WIDTH, OILDERRICK_BUTTON_HEIGHT)));

            //Barracks button
            buttons.Add(new HUDElement(barracksButtonTex, new Rectangle(BARRACKS_X, BARRACKS_Y, BARRACKS_BUTTON_WIDTH, BARRACKS_BUTTON_HEIGHT)));

            //Soldier recruitment button
            buttons.Add(new HUDElement(soldierRecruitTex, new Rectangle(ADDTROOPS_X, ADDTROOPS_Y, ADDTROOPS_WIDTH, ADDTROOPS_HEIGHT)));

            //Engineer recruitment button
            buttons.Add(new HUDElement(engineerRecruitTex, new Rectangle(ADDENGINEER_X, ADDENGINEER_Y, ADDENGINEER_WIDTH, ADDENGINEER_HEIGHT)));

            //Factory button
            buttons.Add(new HUDElement(factoryButtonTex, new Rectangle(FACTORY_X, FACTORY_Y, FACTORY_BUTTON_WIDTH, FACTORY_BUTTON_HEIGHT)));

            //Tank recruitment button
            buttons.Add(new HUDElement(tankRecruitTex, new Rectangle(ADDTANK_X, ADDTANK_Y, ADDTANK_WIDTH, ADDTANK_HEIGHT)));

            //Disabled turret button
            buttons.Add(new HUDElement(disabledTurretButtonTex, new Rectangle(TURRET_X, TURRET_Y, TURRETBUTTON_WIDTH, TURRETBUTTON_HEIGHT)));

            //Disabled oil derrick button
            buttons.Add(new HUDElement(disabledOilDerrickButtonTex, new Rectangle(OILDERRICK_X, OILDERRICK_Y, OILDERRICK_BUTTON_WIDTH, OILDERRICK_BUTTON_HEIGHT)));

            //Disabled barracks button
            buttons.Add(new HUDElement(disabledBarracksButtonTex, new Rectangle(BARRACKS_X, BARRACKS_Y, BARRACKS_BUTTON_WIDTH, BARRACKS_BUTTON_HEIGHT)));

            //Disabled factory button
            buttons.Add(new HUDElement(disabledFactoryButtonTex, new Rectangle(FACTORY_X, FACTORY_Y, FACTORY_BUTTON_WIDTH, FACTORY_BUTTON_HEIGHT)));

            canClick = true;

            //Reset any static variables
            numOilDerricks = 0;
        }

        /// <summary>
        /// Loads assets needed by this class
        /// </summary>
        /// <param name="Content">A reference to the game's Content Manager</param>
        public static void LoadContent(ContentManager Content)
        {

            humanFontFunds = Content.Load<SpriteFont>(@"Fonts\scoreFont");
            humanFontCosts = Content.Load<SpriteFont>(@"Fonts\Upgrade");

            mouseTex = Content.Load<Texture2D>(@"Textures\cursor");

            turretButtonTex = Content.Load<Texture2D>(@"Textures\TurretIcon");
            disabledTurretButtonTex = Content.Load<Texture2D>(@"Textures\TurretIconDisabled");

            oilDerrickButtonTex = Content.Load<Texture2D>(@"Textures\OilDerrickIcon");
            disabledOilDerrickButtonTex = Content.Load<Texture2D>(@"Textures\OilDerrickIconDisabled");

            barracksButtonTex = Content.Load<Texture2D>(@"Textures\BarracksIcon");
            disabledBarracksButtonTex = Content.Load<Texture2D>(@"Textures\BarracksIconDisabled");

            soldierRecruitTex = Content.Load<Texture2D>(@"Textures\soldierIcon");
            engineerRecruitTex = Content.Load<Texture2D>(@"Textures\engineerIcon");

            factoryButtonTex = Content.Load<Texture2D>(@"Textures\FactoryIcon");
            disabledFactoryButtonTex = Content.Load<Texture2D>(@"Textures\FactoryIconDisabled");

            tankRecruitTex = Content.Load<Texture2D>(@"Textures\tankRecruitIcon");

            ghostTurret = Content.Load<Texture2D>(@"Textures\ghostTurret");
            ghostBarracks = Content.Load<Texture2D>(@"Textures\GhostBarracks");
            ghostFactory = Content.Load<Texture2D>(@"Textures\GhostFactory");
            ghostOilDerrick = Content.Load<Texture2D>(@"Textures\GhostOilDerrick");
        }

        public void Update(GameTime gameTime)
        {
            //Poll the mouse
            MouseState mState = Mouse.GetState();

            mouseX = mState.X;
            mouseY = mState.Y;

            if (mouseX < 0)
                mouseX = 0;
            if (mouseX > Game1.SCREENWIDTH)
                mouseX = Game1.SCREENWIDTH;
            if (mouseY < 0)
                mouseY = 0;
            if (mouseY > Game1.SCREENHEIGHT)
                mouseY = Game1.SCREENHEIGHT;

            bool drawAGhost = drawGhostBarracks || drawGhostFactory || drawGhostOilDerrick || drawGhostTurret;
            bool clickedSoldier = false;
            bool clickedEngineer = false;
            bool clickedTank = false;
            bool clickedTurret = false;
            bool clickedBarracks = false;
            bool clickedFactory = false;
            bool clickedOilDerrick = false;

            if (mState.LeftButton == ButtonState.Pressed && canClick)
            {
                canClick = false;

                clickedSoldier = buttons[RECRUIT_SOLDIER].isWithin(new Vector2(mouseX, mouseY)) && !drawAGhost;
                clickedEngineer = buttons[RECRUIT_ENGINEER].isWithin(new Vector2(mouseX, mouseY)) && !drawAGhost;
                clickedTank = buttons[RECRUIT_TANK].isWithin(new Vector2(mouseX, mouseY)) && !drawAGhost;
                clickedTurret = buttons[TURRET].isWithin(new Vector2(mouseX, mouseY)) && !drawAGhost;
                clickedBarracks = buttons[BARRACKS].isWithin(new Vector2(mouseX, mouseY)) && !drawAGhost;
                clickedFactory = buttons[FACTORY].isWithin(new Vector2(mouseX, mouseY)) && !drawAGhost;
                clickedOilDerrick = buttons[OILDERRICK].isWithin(new Vector2(mouseX, mouseY)) && !drawAGhost;

                if (drawGhostTurret)
                {
                    //Place the turret at this location
                    Building.buildings.Add(new Turret(mouseX, 350, theWyrm));
                    drawGhostTurret = false;
                    totalMoney -= TURRET_COST;
                }
                else if (drawGhostBarracks)
                {
                    //Place a barracks at this location
                    Building.buildings.Add(new Barracks(mouseX, 385, theWyrm));
                    barracksLoc = new Vector2(mouseX, 385);
                    drawGhostBarracks = false;
                    hasBarracks = true;
                    totalMoney -= BARRACKS_COST;
                }
                else if (drawGhostFactory)
                {
                    //Place a factory at this location
                    Building.buildings.Add(new Factory(mouseX, 353, theWyrm));
                    factoryLoc = new Vector2(mouseX, 353);
                    drawGhostFactory = false;
                    hasFactory = true;
                    totalMoney -= FACTORY_COST;
                }
                else if (drawGhostOilDerrick)
                {
                    //Place an oil derrick at this location
                    Building.buildings.Add(new OilDerrick(mouseX, 353, theWyrm));
                    drawGhostOilDerrick = false;
                    numOilDerricks++;
                    totalMoney -= OILDERRICK_COST;

                    if (numOilDerricks > 1)
                    {
                        //Cause diminishing returns on oil derricks
                        incomeAdjustmentPerOilDerrick = incomeAdjustmentPerOilDerrick - INCOME_REDUCTION_PER_OIL_DERRICK;
                        if (incomeAdjustmentPerOilDerrick <= 0)
                            incomeAdjustmentPerOilDerrick = 1;
                    }
                }
            }
            else if (mState.LeftButton == ButtonState.Released && !canClick)
                canClick = true;    //Allow the player to activate click effects only once per click

            if (clickedSoldier && (totalMoney - SOLDIER_COST >= 0) && hasBarracks)
            {

                PreySpawner.addImmediate(new SoldierHuman((int)(barracksLoc.X + XCOORD_BARRACKS_DOOR), (int)(barracksLoc.Y + YCOORD_BARRACKS_DOOR), theWyrm));
                //Prey.prey.Add(new SoldierHuman((int)(barracksLoc.X + XCOORD_BARRACKS_DOOR), (int)(barracksLoc.Y + YCOORD_BARRACKS_DOOR), theWyrm));

                totalMoney -= SOLDIER_COST;
            }
            else if (clickedTank && (totalMoney - TANK_COST >= 0) && hasFactory)
            {
                //Prey.prey.Add(new Tank((int)(factoryLoc.X + XCOORD_FACTORY_DOOR), (int)(factoryLoc.Y + YCOORD_FACTORY_DOOR), theWyrm));
                PreySpawner.addImmediate(new newTank((int)(factoryLoc.X + XCOORD_FACTORY_DOOR), (int)(factoryLoc.Y + YCOORD_FACTORY_DOOR), theWyrm));
                totalMoney -= TANK_COST;
            }

            else if (clickedEngineer && (totalMoney - ENGINEER_COST >= 0) && hasBarracks)
            {
                //Prey.prey.Add(new Engineer((int)(barracksLoc.X + XCOORD_BARRACKS_DOOR), (int)(barracksLoc.Y + YCOORD_BARRACKS_DOOR), theWyrm));
                PreySpawner.addImmediate(new Engineer((int)(barracksLoc.X + XCOORD_BARRACKS_DOOR), (int)(barracksLoc.Y + YCOORD_BARRACKS_DOOR), theWyrm));
                totalMoney -= ENGINEER_COST;
            }
            else if (clickedTurret && !drawGhostTurret && (totalMoney - TURRET_COST >= 0))
            {
                drawGhostTurret = true;
            }
            else if (clickedBarracks && !drawGhostBarracks && (totalMoney - BARRACKS_COST >= 0) && !hasBarracks)
            {
                drawGhostBarracks = true;
            }
            else if (clickedFactory && !drawGhostFactory && (totalMoney - FACTORY_COST >= 0) && !hasFactory)
            {
                drawGhostFactory = true;
            }
            else if (clickedOilDerrick && !drawGhostOilDerrick && (totalMoney - OILDERRICK_COST >= 0))
            {
                drawGhostOilDerrick = true;
            }

            //Cancel any impending build orders with a right click
            if (mState.RightButton == ButtonState.Pressed)
            {
                drawGhostBarracks = false;
                drawGhostFactory = false;
                drawGhostOilDerrick = false;
                drawGhostTurret = false;
            }

            //Accumulate money based on income
            incomeCounter += (float) gameTime.ElapsedGameTime.Milliseconds;
            if(incomeCounter >= INCOME_LIMIT)
            {
                totalMoney += (int)(BASE_INCOME + (incomeAdjustmentPerOilDerrick * numOilDerricks));

                incomeCounter = 0;

                //Float some text indicating that the oil derrick has provided money
                foreach (Building theBuilding in Building.buildings)
                {
                    if (theBuilding is OilDerrick)
                    {
                        incomeIndicators.Add(new FloatingText(theBuilding.xPos + 20, theBuilding.yPos, "+$" + (int)incomeAdjustmentPerOilDerrick, Color.Black));
                    }
                }
            }

            //Update all of the floating text for oil derrick incomes
            for (int i = 0; i < incomeIndicators.Count; i++)
            {
                incomeIndicators[i].Update(gameTime);

                if (incomeIndicators[i].isDone)
                    incomeIndicators.RemoveAt(i);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            //Draw a ghost turret if we need to
            if (drawGhostTurret)
            {
                sb.Draw(ghostTurret, new Rectangle(mouseX, 419, TURRET_WIDTH, TURRET_HEIGHT), Color.White);
            }
            else if (drawGhostBarracks)
            {
                sb.Draw(ghostBarracks, new Rectangle(mouseX, 385, BARRACKS_WIDTH, BARRACKS_HEIGHT), Color.White);
            }
            else if (drawGhostFactory)
            {
                sb.Draw(ghostFactory, new Rectangle(mouseX, 353, FACTORY_WIDTH, FACTORY_HEIGHT), Color.White);
            }
            else if (drawGhostOilDerrick)
            {
                sb.Draw(ghostOilDerrick, new Rectangle(mouseX, 353, OIL_DERRICK_WIDTH, OIL_DERRICK_HEIGHT), Color.White);
            }

            //Draw all of the HUDElements

            //Draw the turret icon, and make it partly transparent if it can't be purchased
            if ((totalMoney - TURRET_COST >= 0))
            {
                buttons[TURRET].Draw(sb);
            }
            else
            {
                buttons[TURRET_DISABLED].Draw(sb);
            }
            //Draw the cost of the turret under the turret icon
            sb.DrawString(humanFontCosts, "$" + TURRET_COST, new Vector2(TURRET_X + 15, TURRET_Y + TURRETBUTTON_HEIGHT), Color.Black);

            if ((totalMoney - OILDERRICK_COST >= 0))
            {
                buttons[OILDERRICK].Draw(sb);
            }
            else
            {
                buttons[OILDERRICK_DISABLED].Draw(sb);
            }
            //Draw the cost of the oil derrick under the derrick icon
            sb.DrawString(humanFontCosts, "$" + OILDERRICK_COST, new Vector2(OILDERRICK_X + 15, OILDERRICK_Y + OILDERRICK_BUTTON_HEIGHT), Color.Black);

            if (!hasBarracks && (totalMoney - BARRACKS_COST >= 0))
            {
                buttons[BARRACKS].Draw(sb);

                //Draw the cost of the barracks under the icon
                sb.DrawString(humanFontCosts, "$" + BARRACKS_COST, new Vector2(BARRACKS_X + 15, BARRACKS_Y + BARRACKS_BUTTON_HEIGHT), Color.Black);
            }
            else
            {
                buttons[BARRACKS_DISABLED].Draw(sb);
            }

            if (!hasBarracks && (totalMoney - BARRACKS_COST < 0))
            {
                //Draw the cost of the barracks even if the player can't afford it
                sb.DrawString(humanFontCosts, "$" + BARRACKS_COST, new Vector2(BARRACKS_X + 15, BARRACKS_Y + BARRACKS_BUTTON_HEIGHT), Color.Black);
            }

            if (!hasFactory && (totalMoney - FACTORY_COST >= 0))
            {
                buttons[FACTORY].Draw(sb);

                //Draw the cost of the factory under the icon
                sb.DrawString(humanFontCosts, "$" + FACTORY_COST, new Vector2(FACTORY_X + 15, FACTORY_Y + FACTORY_BUTTON_HEIGHT), Color.Black);
            }
            else
            {
                buttons[FACTORY_DISABLED].Draw(sb);
            }

            if (!hasFactory && (totalMoney - FACTORY_COST < 0))
            {
                //Draw the cost of the factory even if the player can't afford it
                sb.DrawString(humanFontCosts, "$" + FACTORY_COST, new Vector2(FACTORY_X + 15, FACTORY_Y + FACTORY_BUTTON_HEIGHT), Color.Black);
            }

            if (hasFactory)
            {
                buttons[RECRUIT_TANK].Draw(sb);

                //Draw the cost of the tank
                sb.DrawString(humanFontCosts, "$" + TANK_COST, new Vector2(ADDTANK_X - 10, ADDTANK_Y + ADDTANK_HEIGHT), Color.Black);
            }

            if (hasBarracks)
            {
                buttons[RECRUIT_SOLDIER].Draw(sb);
                buttons[RECRUIT_ENGINEER].Draw(sb);

                //Draw the costs of the units that be trained from a barracks
                sb.DrawString(humanFontCosts, "$" + SOLDIER_COST, new Vector2(ADDTROOPS_X - 7, ADDTROOPS_Y + ADDTROOPS_HEIGHT), Color.Black);
                sb.DrawString(humanFontCosts, "$" + ENGINEER_COST, new Vector2(ADDENGINEER_X - 5, ADDENGINEER_Y + ADDENGINEER_HEIGHT), Color.Black);
            }

            //Draw the total money of the human player
            sb.DrawString(humanFontFunds, "Funds: $" + (int)totalMoney, new Vector2(1050, 10), Color.Black);

            //Draw the income indicators
            foreach (FloatingText theText in incomeIndicators)
            {
                theText.Draw(sb);
            }

            //Finally, draw the mouse cursor
            sb.Draw(mouseTex, new Rectangle(mouseX, mouseY, MOUSECURSOR_WIDTH, MOUSECURSOR_HEIGHT), Color.White);
        }
    }
}
