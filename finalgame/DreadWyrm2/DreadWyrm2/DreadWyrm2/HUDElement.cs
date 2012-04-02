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
    class HUDElement
    {
        Texture2D elementTexture;   //The texture of this HUD element

        Rectangle elementLocation;  //A rectangle to represent the location and size of the HUD element

        public HUDElement(Texture2D texture, Rectangle buttonData)
        {
            elementTexture = texture;
            elementLocation = buttonData;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(elementTexture, elementLocation, Color.White);
        }

        /// <summary>
        /// Determines if a given point is within this HUDElement. This is meant to check if an element is being clicked
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True if point is within the bounds of this HUDElement</returns>
        public bool isWithin(Vector2 point)
        {
            return (point.X < elementLocation.X + elementLocation.Width && point.X > elementLocation.X
                && point.Y > elementLocation.Y && point.Y < elementLocation.Y + elementLocation.Height);
        }
    }
}
