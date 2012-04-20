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
    class Generator : Building
    {
        //Generator constants
        const float FRAME_LENGTH = 0.15f;
        const int GENERATOR_MAX_HIT_POINTS = 100;
        const int GENERATOR_HEALTH_BAR_WIDTH = 100;    //The width of the health bar in pixels

        //Spritesheet stuff
        const int NUM_FRAMES = 4;
        const int SPRITE_WIDTH = 125;
        const int SPRITE_HEIGHT = 56;
        const int GENERATOR_HEIGHT = 57;
        const int BOUNDING_RADIUS = 30;

        public Generator(int x, int y, Wyrm predator)
            : base(x, y, predator)
        {
            asSprite = new AnimatedSprite(buildingTextures[GENERATOR], 0, 0, SPRITE_WIDTH, SPRITE_HEIGHT, NUM_FRAMES);
            asSprite.IsAnimating = true;
            asSprite.FrameLength = FRAME_LENGTH;

            hitPoints = GENERATOR_MAX_HIT_POINTS;

            boundingRadius = BOUNDING_RADIUS;

            buildingheight = GENERATOR_HEIGHT;
            spriteHeight = SPRITE_HEIGHT;
            spriteWidth = SPRITE_WIDTH;
        }

        public override void Update(GameTime gametime)
        {
            base.Update(gametime);

            footToGround();

            //Move the generator down to lessen the appearence of it floating/hovering
            yPos += 2;

            asSprite.Update(gametime);
        }

        public override void Draw(SpriteBatch sb)
        {
            asSprite.Draw(sb, xPos, yPos, false);

            float greenBarWidth = (hitPoints / GENERATOR_MAX_HIT_POINTS) * GENERATOR_HEALTH_BAR_WIDTH;

            sb.Draw(hb_red, new Rectangle(xPos + 10, yPos - 13, GENERATOR_HEALTH_BAR_WIDTH, 8), Color.White);
            sb.Draw(hb_green, new Rectangle(xPos + 10, yPos - 13, (int)greenBarWidth, 8), Color.White);
        }

        public override int getBoundingX()
        {
            return xPos + SPRITE_WIDTH / 2;
        }

        public override int getBoundingY()
        {
            return yPos + SPRITE_HEIGHT / 2;
        }

        public override void getDestroyed()
        {
            base.getDestroyed();

            Game1.p2WyrmVictory = true;
            Game1.gameOver = true;
        }
    }
}
