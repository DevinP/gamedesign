using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreadWyrm2
{
    class Mine : Prey
    {

        Texture2D explosionTexture;

        const int NUM_FRAMES = 3;       //The number of frames in the mine animation
        const int SPRITE_HEIGHT = 9;    //The height (in pixels) of the sprite for the mine
        const int SPRITE_WIDTH = 16;    //The width (in pixels) of the sprite for the mine
        const int PREY_HEIGHT = 7;      //The height (in pixels) of the mine for grounding
        const int BOUNDING_RADIUS = 3;  //The bounding radius of the mine
        const float DAMAGE_DONE = 20;   //The damage dealt to the wyrm when it eats this mine

        /// <summary>
        /// A mine that the Wyrm will be punished for eating
        /// </summary>
        /// <param name="initialX">The initial X position of the mine</param>
        /// <param name="initialY">The initial Y position of the mine</param>
        /// <param name="predator">Not used in this class, but needed to inherit from Prey</param>
        public Mine(float initialX, float initialY, Wyrm predator, Texture2D explosionTex)
            : base(initialX, initialY, predator)
        {
            preyheight = PREY_HEIGHT;
            boundingRadius = BOUNDING_RADIUS;
            spriteHeight = SPRITE_HEIGHT;
            spriteWidth = SPRITE_WIDTH;
            meatReward = 0;

            asSprite = new AnimatedSprite(preyTextures[MINE], 0, 0, SPRITE_WIDTH, SPRITE_HEIGHT, NUM_FRAMES);
            asSprite.IsAnimating = true;

            isMine = true;

            explosionTexture = explosionTex;
        }

        public override void Draw(SpriteBatch sb)
        {
            asSprite.Draw(sb, (int)xPos - SPRITE_WIDTH / 2, (int)yPos - SPRITE_HEIGHT / 2, false);
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

        public override void getEaten(WyrmPlayer thePlayer)
        {
            thePlayer.Health -= DAMAGE_DONE;

            Explosion theExplosion = new Explosion(xPos, yPos, explosionTexture, true);
            Game1.explosions.Add(theExplosion);

            Game1.explosion.Play();
        }
    }
}
