using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreadWyrm2
{
    class Trap : Prey
    {
        float damageDone;

        Texture2D explosionTexture;

        public Trap(int initialX, int initialY, Texture2D texture, int frames, int spriteHeight, int spriteWidth, int preyHeight,
                    float boundingRadius, Wyrm predator, float damagedone, Texture2D explosionTex)
            : base(initialX, initialY, texture, frames, spriteHeight, spriteWidth, preyHeight, boundingRadius, predator, 0, 0)
        {
            asSprite = new AnimatedSprite(texture, 0, 0, spriteWidth, spriteHeight, frames);
            asSprite.IsAnimating = true;

            isMine = true;

            damageDone = damagedone;

            explosionTexture = explosionTex;
        }

        public override void Draw(SpriteBatch sb)
        {
            asSprite.Draw(sb, (int)xPos - spritewidth / 2, (int)yPos - spriteheight / 2, false);
        }

        public override void Update(GameTime gametime)
        {
            if (xPos < 10)
            {
                xPos = 10;
            }
            else if (xPos > Background.SCREENWIDTH - 10)
            {
                xPos = Background.SCREENWIDTH - 10;
            }

            recalcPositions();
            footToGround();

            asSprite.Update(gametime);
        }

        public override void getEaten(Player thePlayer)
        {
            thePlayer.Health -= damageDone;

            Explosion theExplosion = new Explosion(xPos, yPos, explosionTexture, true);
            Game1.explosions.Add(theExplosion);

            Game1.explosion.Play();
        }
    }
}
