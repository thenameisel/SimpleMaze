using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMaze
{
    internal class Player : Sprite
    {
        private float speed;
        private Func<Rectangle, bool> _collisionCheck;

        public Player(Texture2D texture, Vector2 position, float speed, int textureSize, Func<Rectangle, bool> collisionCheck) : base(texture, position, textureSize)
        {
            this.speed = speed;
            _collisionCheck = collisionCheck;
            base.textureNumFrames = 4; // I know this is four, but could be made dynamic by math of atlas width / sprite width
        }
        
        public override void Update(GameTime gameTime) 
        {
            base.Update(gameTime);
            Vector2 movement = Vector2.Zero;
            bool isMoving = false;

            // Handle input and movement
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                movement.Y -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                isMoving = true;
                textureFrameRow = 4;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                movement.Y += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                isMoving = true;
                textureFrameRow = 6;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                movement.X -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                isMoving = true;
                textureFrameRow = 2; 
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                movement.X += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                isMoving = true;
                textureFrameRow = 0; 
            }

            if (!isMoving && textureFrameRow % 2 == 0)
            {
                textureFrameRow += 1;
            }
            Animate();
            

            Rectangle proposedBounds = new Rectangle(
                (int)(position.X + movement.X),
                (int)(position.Y + movement.Y),
                textureSize, textureSize);

            if (!_collisionCheck(proposedBounds))
            {
                position += movement;
            }

        }


        public override void Animate()
        {
            base.Animate();
            //animation logic for the player

            textureCounter++;
            if (textureCounter >= 15)
            {
                textureCounter = 0;
                textureFrame++;
                if (textureFrame >= textureNumFrames)
                {
                    textureFrame = 0;
                }
            }

        }
    }
    
}
