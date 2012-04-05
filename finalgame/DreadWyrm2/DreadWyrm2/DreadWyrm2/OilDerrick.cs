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
        
        //Spritesheet stuff
        const int NUM_FRAMES = 0;
        const int SPRITE_WIDTH = 55;
        const int SPRITE_HEIGHT = 100;
        const int OIL_DERRICK_HEIGHT = 56;

        public OilDerrick(int x, int y, Wyrm predator)
            : base(x, y, predator)
        {
            asSprite = new AnimatedSprite(buildingTextures[OIL_DERRICK], 0, 0, SPRITE_WIDTH, SPRITE_HEIGHT, NUM_FRAMES);
            asSprite.IsAnimating = false;

            buildingheight = OIL_DERRICK_HEIGHT;
            spriteHeight = SPRITE_HEIGHT;
            spriteWidth = SPRITE_WIDTH;
        }

        public override void Update(GameTime gametime)
        {
            footToGround();
        }

        public override void Draw(SpriteBatch sb)
        {
            asSprite.Draw(sb, xPos, yPos, false);
        }

        public override void takeDamage(int amountDamage)
        {
            throw new NotImplementedException();
        }

        public override void getDestroyed(WyrmPlayer thePlayer)
        {
            throw new NotImplementedException();
        }
    }
}
