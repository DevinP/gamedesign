using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreadWyrm
{
    class Bullet
    {
        int xPos;   // x position of the bullet
        int yPos;   // y position of the bullet

        float boundingRad;  // bounding radius of the bullet

        Vector2 vel;    // velocity of the bullet (in Cartesian coords)

        Texture2D bulletTexture;
        AnimatedSprite asprite;

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

        public Vector2 velocity
        {
            get { return vel; }
            set { vel = value; }
        }

        public float boundingRadius
        {
            get { return boundingRad; }
            set { boundingRad = value; }
        }

        public AnimatedSprite asSprite
        {
            get { return asprite; }
            set { asprite = value; }
        }

        public Bullet(int initialX, int initialY, Vector2 velocity, Texture2D texture, int frames, int spriteHeight, int spriteWidth, float boundingradius)
        {
            xPos = initialX;
            yPos = initialY;
            vel = velocity;
            bulletTexture = texture;
            asprite = new AnimatedSprite(bulletTexture, 0, 0, spriteWidth, spriteHeight, frames);
        }

        public void Update(GameTime gametime)
        {
            xPos = xPos + (int) vel.X;
            yPos = yPos + (int) vel.Y;
        }

        public void Draw(SpriteBatch sb)
        {
            asprite.Draw(sb, xPos, yPos);
        }

    }
}
