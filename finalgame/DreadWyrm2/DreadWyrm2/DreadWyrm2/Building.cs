using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace DreadWyrm2
{
    public abstract class Building
    {

        protected AnimatedSprite asSprite;                   //The animated sprite belonging to this prey

        protected int xPos;                                 //The x position of the prey, measured in the center
        protected int yPos;                                 //The y position of the prey, measured in the center

        protected Vector2 basepoint;                        //The point at the bottom edge of the building
        protected Vector2 footpoint;                        //The point which is slightly above the bottom edge of the building

        protected int spriteHeight;
        protected int spriteWidth;
        protected int buildingheight;                       //The height of the buildings's bounding box

        protected int hitPoints;                           //The hit points of the building

        public float boundingRadius;                        //The radius of the bounding circle for the prey

        protected Wyrm theWyrm;                             //The Wyrm player so buildings (turrets) can shoot at it

        public static List<Texture2D> buildingTextures;     //The textures used by the buildings

        public static List<Building> buildings;             //The list of all the buildings on the battlefield

        //Sound effects
        //Turret-related sound effects
        protected static SoundEffect turretShot;

        //Constant ints to access the building texture list
        public const int TURRET = 0;
        public const int BARRACKS = 1;
        public const int FACTORY = 2;
        public const int OIL_DERRICK = 3;
        public const int GENERATOR = 4;

        /// <summary>
        /// Building Constructor
        /// </summary>
        /// <param name="initialX">Initial X position of the building</param>
        /// <param name="initialY">Initial Y position of the building</param>
        /// <param name="predator">The Wyrm that attacks the building</param>
        public Building(int initialX, int initialY, Wyrm predator)
        {
            xPos = initialX;
            yPos = initialY;

            theWyrm = predator;

            basepoint = new Vector2(initialX, initialY + spriteHeight);
            footpoint = new Vector2(initialX, initialY + buildingheight);
        }

        public static void LoadContent(ContentManager Content)
        {
            turretShot = Content.Load<SoundEffect>(@"Sounds\turretShot");

            buildingTextures = new List<Texture2D>();
            buildingTextures.Add(Content.Load<Texture2D>(@"Textures\turret_sprite_sheet_140x100_4x6"));
            buildingTextures.Add(Content.Load<Texture2D>(@"Textures\Barracks142x68"));
            buildingTextures.Add(Content.Load<Texture2D>(@"Textures\Factory142x100"));
            buildingTextures.Add(Content.Load<Texture2D>(@"Textures\OilDerrickBuilding55x100"));
            buildingTextures.Add(Content.Load<Texture2D>(@"Textures\Generator251x126"));
        }

        //A helper function which keeps the building near the current ground level
        protected void footToGround()
        {
            while (!Background.checkIsGrounded((int)basepoint.X, (int)basepoint.Y))
            {
                //The base is not grounded. Move the building down until the base is grounded
                yPos++;
                recalcPositions();
            }

            while (Background.checkIsGrounded((int)footpoint.X, (int)footpoint.Y))
            {
                //The footpoint is grounded, edge the building up until the footpoint is not grounded
                yPos--;
                recalcPositions();
            }
        }

        protected void recalcPositions()
        {
            basepoint.X = xPos;
            basepoint.Y = yPos + spriteHeight;
            footpoint.X = xPos;
            footpoint.Y = yPos + buildingheight;
        }

        /// <summary>
        /// Updates all Buildings currently in the game
        /// </summary>
        /// <param name="gameTime">The GameTime object being used in the game</param>
        /// <returns>The number of buildings in the game after the update is done</returns>
        public static int UpdateAll(GameTime gameTime)
        {
            int numBuilds = buildings.Count;

            for (int i = 0; i < buildings.Count; i++)
            {
                buildings[i].Update(gameTime);                
            }

            return numBuilds;
        }

        public static void reInitializeAll()
        {
            buildings = new List<Building>();
        }

        /// <summary>
        /// Draws all the buildings in the game
        /// </summary>
        /// <param name="spriteBatch">The spritebatch object used to draw</param>
        public static void DrawAll(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < buildings.Count; i++)
            {
                buildings[i].Draw(spriteBatch);
            }

            
        }

        public abstract void Update(GameTime gametime);

        public abstract void Draw(SpriteBatch sb);

        public abstract void takeDamage(int amountDamage);

        public abstract void getDestroyed(WyrmPlayer thePlayer);

    }
}
