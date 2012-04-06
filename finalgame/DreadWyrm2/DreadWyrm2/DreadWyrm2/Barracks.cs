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
    class Barracks : Building
    {
        //Barracks constants
        const int BARRACKS_MAX_HIT_POINTS = 50;
        const int BARRACKS_HEALTH_BAR_WIDTH = 100;    //The width of the health bar in pixels

        //spritesheet stuff
        const int NUM_FRAMES = 3;
        const int SPRITE_WIDTH = 116;
        const int SPRITE_HEIGHT = 70;
        const int BARRACKS_HEIGHT = 71;

        public Barracks(int initialX, int initialY, Wyrm predator)
            : base(initialX, initialY, predator)
        {
            asSprite = new AnimatedSprite(buildingTextures[BARRACKS], 0, 0, SPRITE_WIDTH, SPRITE_HEIGHT, NUM_FRAMES);
            asSprite.IsAnimating = true;

            boundingRadius = 25;

            hitPoints = BARRACKS_MAX_HIT_POINTS;

            buildingheight = BARRACKS_HEIGHT;
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

            float greenBarWidth = (hitPoints / BARRACKS_MAX_HIT_POINTS) * BARRACKS_HEALTH_BAR_WIDTH;

            sb.Draw(hb_red, new Rectangle(xPos + 7, yPos - 5, BARRACKS_HEALTH_BAR_WIDTH, 5), Color.White);
            sb.Draw(hb_green, new Rectangle(xPos + 7, yPos - 5, (int)greenBarWidth, 5), Color.White);
        }

        public override void getDestroyed()
        {
            base.getDestroyed();

            HumanPlayer.hasBarracks = false;
        }

        public override int getBoundingX()
        {
            return xPos + 55;
        }

        public override int getBoundingY()
        {
            return yPos + 52;
        }
    }
}
