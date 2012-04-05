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
        //Spritesheet stuff
        const int NUM_FRAMES = 0;
        const int SPRITE_WIDTH = 149;
        const int SPRITE_HEIGHT = 74;
        const int GENERATOR_HEIGHT = 75;

        public Generator(int x, int y, Wyrm predator)
            : base(x, y, predator)
        {
            asSprite = new AnimatedSprite(buildingTextures[GENERATOR], 0, 0, SPRITE_WIDTH, SPRITE_HEIGHT, NUM_FRAMES);
            asSprite.IsAnimating = false;

            buildingheight = GENERATOR_HEIGHT;
            spriteHeight = SPRITE_HEIGHT;
            spriteWidth = SPRITE_WIDTH;
        }

        public override void Update(GameTime gametime)
        {
            footToGround();

            //Move the generator down to lessen the appearence of it floating/hovering
            yPos += 2;
        }

        public override void Draw(SpriteBatch sb)
        {
            asSprite.Draw(sb, xPos, yPos, false);
        }

        public override int getBoundingX()
        {
            return xPos;
        }

        public override int getBoundingY()
        {
            return yPos;
        }

        public override void takeDamage()
        {
            throw new NotImplementedException();
        }

        public override void getDestroyed(WyrmPlayer thePlayer)
        {
            throw new NotImplementedException();
        }
    }
}
