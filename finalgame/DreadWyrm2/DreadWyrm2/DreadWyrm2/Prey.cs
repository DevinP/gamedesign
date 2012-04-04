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
    public abstract class Prey
    {
        protected AnimatedSprite asprite;               //The animated sprite belonging to this prey

        protected int xPos;                             //The x position of the prey, measured in the center
        protected int yPos;                             //The y position of the prey, measured in the center

        protected Vector2 basepoint;                    //The point at the bottom edge of the prey
        protected Vector2 footpoint;                    //The point which is slightly above the bottom edge of the prey

        protected float xVel;                           //The x velocity of the prey

        protected int spriteHeight;
        protected int spriteWidth;
        protected int preyheight;              //The height of the prey's bounding box

        public float boundingRadius;           //The radius of the bounding circle for the prey

        protected Wyrm theWyrm;

        public static List<Texture2D> preyTextures;     //The textures used by the prey
        //public static List<Bullet> bullets;             //The bullets currently in the game

        public static List<Prey> prey;

        //public static Texture2D bulletTexture;
        //public static Texture2D cannonballTexture;

        protected static SoundEffect chomp;
        protected static SoundEffect gunShot;
        protected static SoundEffect tankShot;

        //Constant ints to access the prey texture list
        public const int GIRAFFE = 0;
        public const int ELEPHANT = 1;
        public const int UNARMEDHUMAN = 2;
        public const int SOLDIER = 3;
        public const int MINE_LAYER = 4;
        public const int TANK = 5;
        public const int MINE = 6;

        public bool isMine = false;

        public AnimatedSprite asSprite
        {
            get { return asprite; }
            set { asprite = value; }
        }

        public int xPosistion
        {
            get { return xPos; }
            set { xPos = value; }
        }


        public int yPosition
        {
            get { return yPos; }
            set { yPos = value; }
        }

        public Vector2 basePoint
        {
            get { return basepoint; }
            set { basepoint = value; }
        }

        public Vector2 footPoint
        {
            get { return footpoint; }
            set { footPoint = value; }
        }

        public float xVelocity
        {
            get { return xVel; }
            set { xVel = value; }
        }

        public int preyHeight
        {
            get { return preyheight; }
            set { preyheight = value; }
        }

        public Prey(int initialX, int initialY, Wyrm predator)
        {
            xPos = initialX;
            yPos = initialY;


            theWyrm = predator;

            basepoint = new Vector2(initialX, initialY + spriteHeight / 2);
            footpoint = new Vector2(initialX, initialY + preyheight / 2);
        }

        public static void LoadContent(ContentManager Content)
        {
            chomp = Content.Load<SoundEffect>(@"Sounds\aud_chomp");
            gunShot = Content.Load<SoundEffect>(@"Sounds\soldierShot");
            tankShot = Content.Load<SoundEffect>(@"Sounds\tankShot");

            preyTextures = new List<Texture2D>();
            preyTextures.Add(Content.Load<Texture2D>(@"Textures\giraffe"));
            preyTextures.Add(Content.Load<Texture2D>(@"Textures\elephant"));
            preyTextures.Add(Content.Load<Texture2D>(@"Textures\unarmed"));
            preyTextures.Add(Content.Load<Texture2D>(@"Textures\soldier"));
            preyTextures.Add(Content.Load<Texture2D>(@"Textures\mine_layer"));
            preyTextures.Add(Content.Load<Texture2D>(@"Textures\tank"));
            preyTextures.Add(Content.Load<Texture2D>(@"Textures\mine"));
        }

        //A helper function which keeps the prey near the current ground level
        protected void footToGround()
        {
            while (!Background.checkIsGrounded((int)basepoint.X, (int)basepoint.Y))
            {
                //The base is not grounded. Move the prey down until the base is grounded
                yPos++;
                recalcPositions();
            }

            while (Background.checkIsGrounded((int)footpoint.X, (int)footpoint.Y))
            {
                //The footpoint is grounded, edge the prey up until the footpoint is not grounded
                yPos--;
                recalcPositions();
            }
        }

        protected void recalcPositions()
        {
            basepoint.X = xPos;
            basepoint.Y = yPos + spriteHeight / 2;
            footpoint.X = xPos;
            footpoint.Y = yPos + preyheight / 2;
        }

        
        /// <summary>
        /// Updates all Prey currently in the game, as well as the bullets associated with those prey
        /// </summary>
        /// <param name="gameTime">The GameTime object being used in the game</param>
        /// <returns>The number of Prey in the game after the update is done</returns>
        public static int UpdateAll(GameTime gameTime)
        {
            int numPrey = prey.Count;

            for (int i = 0; i < prey.Count; i++)
            {
                prey[i].Update(gameTime);

                if (prey[i].isMine)
                    numPrey--;
            }

            /*for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Update(gameTime);
            }*/

            return numPrey;
        }

        /// <summary>
        /// Draws all the prey in the game as well as all bullets associated with those prey
        /// </summary>
        /// <param name="spriteBatch">The spritebatch object used to draw</param>
        public static void DrawAll(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < prey.Count; i++)
            {
                prey[i].Draw(spriteBatch);
            }

            /*for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Draw(spriteBatch);
            }*/
        }

        public static void reInitializeAll()
        {
            //bullets = new List<Bullet>();
            prey = new List<Prey>();
        }

        public abstract void Update(GameTime gametime);

        public abstract void Draw(SpriteBatch sb);

        public abstract void getEaten(WyrmPlayer thePlayer);
    }
}
