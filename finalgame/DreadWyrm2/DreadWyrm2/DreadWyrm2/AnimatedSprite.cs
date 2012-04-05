using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreadWyrm2
{
    public class AnimatedSprite
    {
        Texture2D t2dTexture;

        public float fFrameRate = 0.15f;
        float fElapsed = 0.0f;

        int iFrameOffsetX = 0;
        int iFrameOffsetY = 0;
        int iFrameWidth = 32;
        int iFrameHeight = 32;

        int iFrameCount = 1;
        int iCurrentFrame = 0;
        int iScreenX = 0;
        int iScreenY = 0;

        bool bAnimating = true;

        Color cTinting = Color.White;

        public int X
        {
            get { return iScreenX; }
            set { iScreenX = value; }
        }

        public int Y
        {
            get { return iScreenY; }
            set { iScreenY = value; }
        }

        public int Frame
        {
            get { return iCurrentFrame; }
            set { iCurrentFrame = (int)MathHelper.Clamp(value, 0, iFrameCount); }
        }

        public float FrameLength
        {
            get { return fFrameRate; }
            set { fFrameRate = (float)Math.Max(value, 0f); }
        }

        public bool IsAnimating
        {
            get { return bAnimating; }
            set { bAnimating = value; }
        }

        public int frameOffsetY
        {
            get { return iFrameOffsetY; }
            set { iFrameOffsetY = value; }
        }

        public AnimatedSprite(
              Texture2D texture,
              int FrameOffsetX,
              int FrameOffsetY,
              int FrameWidth,
              int FrameHeight,
              int FrameCount)
        {
            t2dTexture = texture;
            iFrameOffsetX = FrameOffsetX;
            iFrameOffsetY = FrameOffsetY;
            iFrameWidth = FrameWidth;
            iFrameHeight = FrameHeight;
            iFrameCount = FrameCount;
        }

        public Rectangle GetSourceRect()
        {
            return new Rectangle(
            iFrameOffsetX + (iFrameWidth * iCurrentFrame),
            iFrameOffsetY,
            iFrameWidth,
            iFrameHeight);
        }

        public Color Tint
        {
            get { return cTinting; }
            set { cTinting = value; }
        }

        public void Update(GameTime gametime)
        {
            if (bAnimating)
            {
                // Accumulate elapsed time...
                fElapsed += (float)gametime.ElapsedGameTime.TotalSeconds;

                // Until it passes our frame length
                if (fElapsed > fFrameRate)
                {
                    // Increment the current frame, wrapping back to 0 at iFrameCount
                    iCurrentFrame = (iCurrentFrame + 1) % iFrameCount;

                    // Reset the elapsed frame time.
                    fElapsed = 0.0f;
                }
            }
        }

        /* Draw()
         * 
         * This method is a version of Draw which is best used in sprites that do not rotate
         * 
         * @params spriteBatch - the SpriteBatch to use in drawing
         *         XOffset - the X location on the screen to begin drawing the sprite
         *         YOffset - the Y location on the screen to begin drawing the sprite
         *         NeedBeginEnd - Should be true if the spritebatch needs a begin and end statement
         */
        public void Draw(
        SpriteBatch spriteBatch,
        int XOffset,
        int YOffset,
        bool NeedBeginEnd)
        {
            if (NeedBeginEnd)
                spriteBatch.Begin();

            spriteBatch.Draw(
                t2dTexture,
                new Rectangle(
                  iScreenX + XOffset,
                  iScreenY + YOffset,
                  iFrameWidth,
                  iFrameHeight),
                GetSourceRect(),
                cTinting);

            if (NeedBeginEnd)
                spriteBatch.End();
        }

        public void Draw(
        SpriteBatch spriteBatch,
        int XOffset,
        int YOffset,
        float scale)
        {
            spriteBatch.Draw(t2dTexture, new Vector2(iScreenX + XOffset, iScreenY + YOffset), GetSourceRect(), Color.White,
                0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
        }

        public void Draw(
        SpriteBatch spriteBatch,
        int XOffset,
        int YOffset,
        float scale,
        float rotationAngle)
        {
            spriteBatch.Draw(t2dTexture, new Vector2(iScreenX + XOffset, iScreenY + YOffset), GetSourceRect(), Color.White, rotationAngle,
                new Vector2(0, 0), scale, SpriteEffects.None, 0);
        }

        /* Draw()
         * 
         * This method is a version of Draw which is best used in sprites that will be rotating around their center
         * 
         * @params spriteBatch - the SpriteBatch to use in drawing
         *         XOffset - the X location on the screen to begin drawing the sprite
         *         YOffset - the Y location on the screen to begin drawing the sprite
         *         rotationAngle - the angle of rotation the sprite will be drawn at, in radians
         *         NeedBeginEnd - Should be true if the spritebatch needs a begin and end statement
         */
        public void Draw(
        SpriteBatch spriteBatch,
        int XOffset,
        int YOffset,
        float rotationAngle,
        bool NeedBeginEnd)
        {
            if (NeedBeginEnd)
                spriteBatch.Begin();

            spriteBatch.Draw(
                t2dTexture,
                new Rectangle(
                    iScreenX + XOffset,
                    iScreenY + YOffset,
                    iFrameWidth,
                    iFrameHeight),
                    GetSourceRect(),
                    cTinting,
                    rotationAngle,
                    new Vector2(iFrameWidth / 2, iFrameHeight / 2), //Provide the sprite's center as the rotation origin
                    SpriteEffects.None,
                    0f);

            if (NeedBeginEnd)
                spriteBatch.End();
        }

        public void Draw(SpriteBatch spriteBatch, int XOffset, int YOffset)
        {
            Draw(spriteBatch, XOffset, YOffset, true);
        }
    }
}
