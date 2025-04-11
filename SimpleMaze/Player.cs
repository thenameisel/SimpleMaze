using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        private bool wPressed = false;
        private bool aPressed = false;
        private bool sPressed = false;
        private bool dPressed = false;


        public Player(Texture2D texture, Vector2 position, float speed) : base(texture, position)
        {
            this.speed = speed;
        }
        
        public override void Update(GameTime gameTime) 
        {
            base.Update(gameTime);

            if(!wPressed && Keyboard.GetState().IsKeyDown(Keys.W))
            {
                wPressed = true;
            }
            else if (wPressed && Keyboard.GetState().IsKeyUp(Keys.W))
            {
                wPressed = false;
            }

            if (!aPressed && Keyboard.GetState().IsKeyDown(Keys.A))
            {
                aPressed = true;
            }
            else if (aPressed && Keyboard.GetState().IsKeyUp(Keys.A))
            {
                aPressed = false;
            }

            if (!sPressed && Keyboard.GetState().IsKeyDown(Keys.S))
            {
                sPressed = true;
                
            }
            else if (sPressed && Keyboard.GetState().IsKeyUp(Keys.S))
            {
                sPressed = false;
            }

            if (!dPressed && Keyboard.GetState().IsKeyDown(Keys.D))
            {
                dPressed = true;
                
            }
            else if (dPressed && Keyboard.GetState().IsKeyUp(Keys.D))
            {
                dPressed = false;
            }

            if (wPressed) position.Y -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (aPressed) position.X -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (sPressed) position.Y += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (dPressed) position.X += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //position.X += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
    
}
