using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMaze
{
    internal class Sprite
    {
        public Texture2D texture;
        public Vector2 position;
        public int textureFrameRow = 0; 

        public int textureCounter = 0;
        public int textureFrame = 0;
        public int textureNumFrames;
        public int textureSize;
        public Sprite(Texture2D texture, Vector2 position, int textureSize)
        {
            this.texture = texture;
            this.position = position;
            this.textureSize = textureSize;
        }

        public virtual void Update(GameTime gameTime)
        {
            //update logic for the sprite
        }
        public virtual void Animate()
        {
            //animation logic for the sprite
        }
    }
}
