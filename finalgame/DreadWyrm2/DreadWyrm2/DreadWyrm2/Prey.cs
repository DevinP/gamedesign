﻿using System;
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

        protected float xPos;                             //The x position of the prey, measured in the center
        protected float yPos;                             //The y position of the prey, measured in the center

        protected Vector2 basepoint;                    //The point at the bottom edge of the prey
        protected Vector2 footpoint;                    //The point which is slightly above the bottom edge of the prey

        protected float xVel;                           //The x velocity of the prey

        protected int spriteHeight;
        protected int spriteWidth;
        protected int preyheight;              //The height of the prey's bounding box

        public float boundingRadius;           //The radius of the bounding circle for the prey

        public int meatReward;

        public bool timeUp = false;

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
        public const int DEPTH_CHARGE = 7;
        public const int NEW_TANK = 8;

        public bool isMine = false;

        public AnimatedSprite asSprite
        {
            get { return asprite; }
            set { asprite = value; }
        }

        public float xPosistion
        {
            get { return xPos; }
            set { xPos = value; }
        }


        public float yPosition
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

        public Prey(float initialX, float initialY, Wyrm predator)
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
            preyTextures.Add(Content.Load<Texture2D>(@"Textures\charge_16x16x4"));
            preyTextures.Add(Content.Load<Texture2D>(@"Textures\tank_75x60x3x12"));
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

            //Update all of the prey
            foreach (Prey p in prey)
            {
                p.Update(gameTime);
                if (p.isMine)
                    numPrey--;
            }

            //Allow depth charges to expire
            for (int i = 0; i < prey.Count; i++)
            {
                if (prey[i].timeUp)
                {
                    Game1.explosions.Add(new Explosion(prey[i].xPos, prey[i].yPos, Game1.explosionTexture, true));

                    Game1.explosion.Play();

                    prey.RemoveAt(i);
                }
            }

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

        protected bool withinRange(int maxRange)
        {
            //Calculate the length from the turret to the wyrm head
            Vector2 wyrmPos = new Vector2(theWyrm.l_segments[0].X, theWyrm.l_segments[0].Y);
            Vector2 position = new Vector2(xPos, yPos);

            Vector2 diff = new Vector2(wyrmPos.X - position.X, wyrmPos.Y - position.Y);

            float length = diff.Length();

            //If the wyrm is within the range of the turret, fire at it
            if (length < maxRange)
                return true;
            else
                return false;
        }

        protected void determineVelocityUnscared()
        {
            xVel = xVel * 0.5f;
            float velScale = (float)(Game1.m_random.Next(7, 17)) / 10f;
            xVel = xVel * velScale;
        }


        public abstract void Update(GameTime gametime);

        public abstract void Draw(SpriteBatch sb);

        public abstract void getEaten(WyrmPlayer thePlayer);
    }
}
