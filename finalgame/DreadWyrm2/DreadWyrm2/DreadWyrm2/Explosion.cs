using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace DreadWyrm2
{
    public class Explosion
    {
        float xPos;
        float yPos;

        AnimatedSprite asSprite;

        bool done = false;
        int yOff;

        const int SPRITEHEIGHT = 65;
        const int SPRITEWIDTH = 65;

        public bool isDone
        {
            get { return done; }
            set { done = value; }
        }

        public Explosion(float x, float y, Texture2D texture, bool mineExplosion)
        {
            xPos = x;
            yPos = y;

            if (mineExplosion)
            {
                yOff = 390;
            }
            else
            {
                yOff = 0;
            }

            asSprite = new AnimatedSprite(texture, 0, yOff, SPRITEWIDTH, SPRITEHEIGHT, 16);

            asSprite.FrameLength = 0.05f;
        }

        public void Update(GameTime gametime)
        {
            if (!done)
                asSprite.Update(gametime);

            if (asSprite.Frame >= 15)
            {
                done = true;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (!done)
                asSprite.Draw(sb, (int)xPos - SPRITEWIDTH / 2, (int)yPos - SPRITEHEIGHT / 2, false);
        }
    }
}
