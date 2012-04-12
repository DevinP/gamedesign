using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreadWyrm2
{
    class DepthCharge : Mine
    {
        Texture2D explosionTexture;

        float moveSpeed;

        const float FRICTION = 0.1f;

        float lifeTimer = 0;
        const float LIFETIME = 10000;      //The number of milliseconds the depth charge lives

        const int NUM_FRAMES = 4;       //The number of frames in the depth charge animation
        const int SPRITE_HEIGHT = 16;   //The height (in pixels) of the sprite for the depth charge
        const int SPRITE_WIDTH = 16;    //The width (in pixels) of the sprite for the depth charge
        const int PREY_HEIGHT = 17;     //The height (in pixels) of the depth charge for grounding
        const int BOUNDING_RADIUS = 16; //The bounding radius of the depth charge
        const float DAMAGE_DONE = 5;    //The damage dealt to the wyrm when it eats this depth charge

        /// <summary>
        /// A depth charge that the Wyrm will have to avoid while underground
        /// </summary>
        /// <param name="initialX">The initial X position of the depth charge</param>
        /// <param name="initialY">The initial Y position of the depth charge</param>
        /// <param name="predator">The wyrm which the depth charge wants to damage</param>
        /// <param name="explosionTex">The explosion texture which animates when the depth charge is destroyed</param>
        public DepthCharge(float initialX, float initialY, Wyrm predator, Texture2D explosionTex)
            : base(initialX, initialY, predator, explosionTex)
        {
            preyheight = PREY_HEIGHT;
            boundingRadius = BOUNDING_RADIUS;
            spriteHeight = SPRITE_HEIGHT;
            spriteWidth = SPRITE_WIDTH;
            meatReward = 0;

            asSprite = new AnimatedSprite(preyTextures[DEPTH_CHARGE], 0, 0, SPRITE_WIDTH, SPRITE_HEIGHT, NUM_FRAMES);
            asSprite.IsAnimating = true;

            isMine = true;

            explosionTexture = explosionTex;

            recalcPositions();
            footToGround();

            moveSpeed = (float) Game1.m_random.NextDouble() * 5 + 3;
        }

        public override void Update(GameTime gametime)
        {
            lifeTimer += (float)gametime.ElapsedGameTime.Milliseconds;

            if (lifeTimer >= LIFETIME)
                timeUp = true;

            if (xPos < 10)
            {
                xPos = 10;
            }
            else if (xPos > Background.SCREENWIDTH - 10)
            {
                xPos = Background.SCREENWIDTH - 10;
            }

            asSprite.Update(gametime);

            moveSpeed = moveSpeed - FRICTION;

            if (moveSpeed <= 0)
            {
                moveSpeed = 0;
            }

            yPos = yPos + moveSpeed;

            if (yPos > 700)
                yPos = 700;
        }
    }
}
