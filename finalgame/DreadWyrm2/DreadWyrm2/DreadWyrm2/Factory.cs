﻿using System;
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
        //spritesheet stuff
        const int NUM_FRAMES = 0;
        const int SPRITE_WIDTH = 142;
        const int SPRITE_HEIGHT = 100;
        const int FACTORY_HEIGHT = 101;

        public Factory(int initialX, int initialY, Wyrm predator)
            : base(initialX, initialY, predator)
        {
            asSprite = new AnimatedSprite(buildingTextures[FACTORY], 0, 0, SPRITE_WIDTH, SPRITE_HEIGHT, NUM_FRAMES);
            asSprite.IsAnimating = false;

            buildingheight = FACTORY_HEIGHT;
            spriteHeight = SPRITE_HEIGHT;
            spriteWidth = SPRITE_WIDTH;
        }

        public override void Update(GameTime gametime)
        {
            footToGround();
        }

        public override void Draw(SpriteBatch sb)
        {
            asSprite.Draw(sb, xPos, yPos + 3, false);
        }

        public override int getBoundingX()
        {
            throw new NotImplementedException();
        }

        public override int getBoundingY()
        {
            throw new NotImplementedException();
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
