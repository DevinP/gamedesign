using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreadWyrm2
{
    public class Bullet
    {
        float xPos;   // x position of the bullet
        float yPos;   // y position of the bullet

        int spriteWidth;
        int spriteHeight;

        int damage; //The damage this bullet deals to the player

        float boundingRad;  // bounding radius of the bullet

        Vector2 vel;    // velocity of the bullet (in Cartesian coords)

        Texture2D bulletTexture;
        public AnimatedSprite asprite;

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

        public int DamageDealt
        {
            get { return damage; }
            set { damage = value; }
        }

        public Bullet(float initialX, float initialY, Vector2 velocity, Texture2D texture, int frames, int spriteheight, int spritewidth,
            float boundingradius, int dam)
        {
            xPos = initialX;
            yPos = initialY;
            vel = velocity;
            bulletTexture = texture;
            damage = dam;

            spriteWidth = spritewidth;
            spriteHeight = spriteheight;

            asprite = new AnimatedSprite(bulletTexture, 0, 0, spriteWidth, spriteHeight, frames);
            asSprite.IsAnimating = true;
            asSprite.fFrameRate = 0.05f;
        }

        public void Update(GameTime gametime)
        {
            xPos = xPos + (int)vel.X;
            yPos = yPos + (int)vel.Y;

            asSprite.Update(gametime);
        }

        public void Draw(SpriteBatch sb)
        {
            asprite.Draw(sb, (int)(xPos - spriteWidth / 2), (int)(yPos - spriteWidth / 2), false);
        }

    }
}
