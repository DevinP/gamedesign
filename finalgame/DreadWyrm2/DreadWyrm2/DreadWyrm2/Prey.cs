using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreadWyrm2
{
    public abstract class Prey
    {
        protected Texture2D preyTexture;                //The texture of this prey
        protected AnimatedSprite asprite;               //The animated sprite belonging to this prey
        protected int animationFrames;                  //The frames of animation in this prey

        protected int xPos;                             //The x position of the prey, measured in the center
        protected int yPos;                             //The y position of the prey, measured in the center

        protected Vector2 basepoint;                    //The point at the bottom edge of the prey
        protected Vector2 footpoint;                    //The point which is slightly above the bottom edge of the prey

        protected float xVel;                           //The x velocity of the prey

        protected int spriteheight;                     //The height of the prey sprite
        protected int spritewidth;                      //The width of the prey sprite

        protected int preyheight;                       //The height of the prey's bounding box

        public float boundingradius;                    //The radius of the bounding circle for the prey

        protected Wyrm theWyrm;

        public int meatReward;                          //The meat granted from eating this prey

        protected int otherFacing;                      //The y position in the sprite sheet of the reverse facing

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


        public int spriteHeight
        {
            get { return spriteheight; }
            set { spriteheight = value; }
        }

        public int spriteWidth
        {
            get { return spritewidth; }
            set { spritewidth = value; }
        }

        public int preyHeight
        {
            get { return preyheight; }
            set { preyheight = value; }
        }

        public Prey(int initialX, int initialY, Texture2D texture, int frames, int spriteHeight, int spriteWidth, int preyHeight,
                    float boundingRadius, Wyrm predator, int meat, int facingY)
        {
            xPos = initialX;
            yPos = initialY;

            preyTexture = texture;
            animationFrames = frames;

            spriteheight = spriteHeight;
            spritewidth = spriteWidth;
            preyheight = preyHeight;
            boundingradius = boundingRadius;

            theWyrm = predator;

            meatReward = meat;

            basepoint = new Vector2(initialX, initialY + spriteheight / 2);
            footpoint = new Vector2(initialX, initialY + preyheight / 2);
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
            basepoint.Y = yPos + spriteheight / 2;
            footpoint.X = xPos;
            footpoint.Y = yPos + preyheight / 2;
        }

        public abstract void Update(GameTime gametime);

        public abstract void Draw(SpriteBatch sb);

        public abstract void getEaten(Player thePlayer);
    }
}
