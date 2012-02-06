using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DreadWyrm
{
    class Animal : Prey
    {
        public Animal(float initialX, float initialY, Texture2D texture, int frames)
        {
            base(initialX, initialY, texture, frames);
        }
    }
}
