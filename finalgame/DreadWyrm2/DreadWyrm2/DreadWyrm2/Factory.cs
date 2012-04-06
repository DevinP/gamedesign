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
    class Factory : Building
    {
        //Factory constants
        const float FRAME_LENGTH = 0.2f;
        const int FACTORY_MAX_HIT_POINTS = 60;
        const int FACTORY_HEALTH_BAR_WIDTH = 100;    //The width of the health bar in pixels

        //spritesheet stuff
        const int NUM_FRAMES = 10;
        const int SPRITE_WIDTH = 121;
        const int SPRITE_HEIGHT = 100;
        const int FACTORY_HEIGHT = 101;

        public Factory(int initialX, int initialY, Wyrm predator)
            : base(initialX, initialY, predator)
        {
            asSprite = new AnimatedSprite(buildingTextures[FACTORY], 0, 0, SPRITE_WIDTH, SPRITE_HEIGHT, NUM_FRAMES);
            asSprite.IsAnimating = true;

            asSprite.FrameLength = FRAME_LENGTH;

            boundingRadius = 35;

            hitPoints = FACTORY_MAX_HIT_POINTS;

            buildingheight = FACTORY_HEIGHT;
            spriteHeight = SPRITE_HEIGHT;
            spriteWidth = SPRITE_WIDTH;
        }

        public override void Update(GameTime gametime)
        {
            base.Update(gametime);

            footToGround();

            asSprite.Update(gametime);
        }

        public override void Draw(SpriteBatch sb)
        {
            asSprite.Draw(sb, xPos, yPos + 3, false);

            float greenBarWidth = (hitPoints / FACTORY_MAX_HIT_POINTS) * FACTORY_HEALTH_BAR_WIDTH;

            sb.Draw(hb_red, new Rectangle(xPos + 10, yPos - 5, FACTORY_HEALTH_BAR_WIDTH, 5), Color.White);
            sb.Draw(hb_green, new Rectangle(xPos + 10, yPos - 5, (int)greenBarWidth, 5), Color.White);
        }

        public override int getBoundingX()
        {
            return xPos + 61;
        }

        public override int getBoundingY()
        {
            return yPos + 75;
        }
    }
}
