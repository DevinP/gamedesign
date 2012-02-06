using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreadWyrm
{
    class Prey
    {
        Texture2D preyTexture;      //The texture of this prey
        AnimatedSprite asprite;    //The animated sprite belonging to this prey
        int animationFrames;        //The frames of animation in this prey

        float xPos;                 //The x position of the prey, measured in the center
        float yPos;                 //The y position of the prey, measured in the center

        Vector2 basepoint;          //The point at the bottom edge of the prey
        Vector2 footpoint;          //The point which is slightly above the bottom edge of the prey

        float xVel;                 //The x velocity of the prey

        int spriteheight;           //The height of the prey sprite
        int spritewidth;            //The width of the prey sprite

        int boundingheight;         //The height of the prey's bounding box
        int boundingwidth;          //The width of the prey's bounding box


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

        /*
        public float yVelocity
        {
            get { return yVel; }
            set { yVel = value; }
        }
        */

        int spriteHeight
        {
            get { return spriteheight; }
            set { spriteheight = value; }
        }

        int spriteWidth
        {
            get { return spritewidth; }
            set { spritewidth = value; }
        }

        public int boundingHeight
        {
            get { return boundingheight; }
            set { boundingheight = value; }
        }

        public int boundingWidth
        {
            get { return boundingwidth; }
            set { boundingwidth = value; }
        }

        public Prey(float initialX, float initialY, Texture2D texture, int frames)
        {
            xPos = initialX;
            yPos = initialY;

            preyTexture = texture;
            animationFrames = frames;

            basepoint = new Vector2(initialX, initialY + spriteheight / 2);
            footpoint = new Vector2(initialX, initialY + boundingheight / 2);
        }

        //A helper function which keeps the prey near the current ground level
        void footToGround()
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

        void recalcPositions()
        {
            basepoint.X = xPos;
            basepoint.Y = yPos + spriteheight / 2;
            footpoint.X = xPos;
            footpoint.Y = yPos + boundingheight / 2;
        }

        void Update(GameTime gametime)
        {
            xPos = xPos + xVel;
            footToGround();
        }

        void Draw(SpriteBatch sb)
        {
            asSprite.Draw(sb, (int)xPos, (int)yPos);
        }
    }
}
