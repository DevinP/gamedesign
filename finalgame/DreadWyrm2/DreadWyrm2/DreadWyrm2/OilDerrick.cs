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
    class OilDerrick : Building
    {
        //Oil derrick constants
        const float FRAME_LENGTH = 0.8f;
        const int OILDERRICK_MAX_HIT_POINTS = 15;
        const int OILDERRICK_HEALTH_BAR_WIDTH = 50;    //The width of the health bar in pixels
        
        //Spritesheet stuff
        const int NUM_FRAMES = 4;
        const int SPRITE_WIDTH = 71;
        const int SPRITE_HEIGHT = 100;
        const int OIL_DERRICK_HEIGHT = 72;

        public OilDerrick(int x, int y, Wyrm predator)
            : base(x, y, predator)
        {
            asSprite = new AnimatedSprite(buildingTextures[OIL_DERRICK], 0, 0, SPRITE_WIDTH, SPRITE_HEIGHT, NUM_FRAMES);
            asSprite.IsAnimating = true;
            asSprite.FrameLength = FRAME_LENGTH;

            boundingRadius = 25;

            hitPoints = OILDERRICK_MAX_HIT_POINTS;

            buildingheight = OIL_DERRICK_HEIGHT;
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
            asSprite.Draw(sb, xPos, yPos, false);

            float greenBarWidth = (hitPoints / OILDERRICK_MAX_HIT_POINTS) * OILDERRICK_HEALTH_BAR_WIDTH;

            sb.Draw(hb_red, new Rectangle(xPos + 15, yPos - 10, OILDERRICK_HEALTH_BAR_WIDTH, 5), Color.White);
            sb.Draw(hb_green, new Rectangle(xPos + 15, yPos - 10, (int)greenBarWidth, 5), Color.White);
        }

        public override int getBoundingX()
        {
            return xPos + 38;
        }

        public override int getBoundingY()
        {
            return yPos + 75;
        }

        public override void getDestroyed()
        {
            base.getDestroyed();

            HumanPlayer.numOilDerricks--;
        }
    }
}
