using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMaze
{
    internal class Player : Sprite
    {
        private float speed;

        public Player(Texture2D texture, Vector2 position, float speed) : base(texture, position)
        {
            this.speed = speed;
        }
        
        public override void Update(GameTime gameTime) 
        {
            position.X += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
    
}
