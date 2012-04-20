using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DreadWyrm2
{
    class FloatingText
    {
        float xPos;
        float yPos;

        const float YVEL = -0.5f;

        String theText;
        Color theColor;

        float alphaMod = 1;
        const float ALPHA_MOD_DELTA = -0.01f;      //The amount the alpha of the text goes down each frame

        float timeElapsed_alphaDecay = 0;
        const float TIME_LIMIT_ALPHA_DECAY = 500;   //The number of milliseconds until the alpha starts to decay

        float timeElapsed;
        const float TIME_LIMIT_LIFETIME = 2000;     //The number of milliseconds the text will exist

        public bool isDone = false;

        static SpriteFont theFont;

        public FloatingText(int initialX, int initialY, String text, Color textColor)
        {
            xPos = initialX;
            yPos = initialY;
            theText = text;
            theColor = textColor;
        }

        public static void LoadContent(ContentManager Content)
        {
            theFont = Content.Load<SpriteFont>(@"Fonts\floatingFont");
        }

        public void Update(GameTime gametime)
        {
            yPos += YVEL;

            timeElapsed += (float)gametime.ElapsedGameTime.Milliseconds;
            timeElapsed_alphaDecay += (float)gametime.ElapsedGameTime.Milliseconds;

            if(timeElapsed_alphaDecay >= TIME_LIMIT_ALPHA_DECAY)
                alphaMod += ALPHA_MOD_DELTA;

            if (timeElapsed >= TIME_LIMIT_LIFETIME)
            {
                isDone = true;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.DrawString(theFont, theText, new Vector2(xPos, yPos), theColor * alphaMod);
        }
    }
}
