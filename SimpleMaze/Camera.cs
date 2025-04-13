using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMaze
{
    public class Camera
    {
        public Vector2 Position { get; private set; }
        private Viewport _viewport;
        private Rectangle? _limits;

        public Camera(Viewport viewport)
        {
            _viewport = viewport;
            Position = Vector2.Zero;
        }

        public void SetLimits(Rectangle limits)
        {
            _limits = limits;
        }

        public void Update(Vector2 focusPosition, int textureSize)
        {
            // Center the camera on the focus position (player)
            Position = new Vector2(
                focusPosition.X + textureSize / 2 - _viewport.Width / 2,
                focusPosition.Y + textureSize / 2 - _viewport.Height / 2);

            // Apply camera limits if they exist
            if (_limits.HasValue)
            {
                Position = new Vector2(
                    MathHelper.Clamp(Position.X, _limits.Value.Left, _limits.Value.Right - _viewport.Width),
                    MathHelper.Clamp(Position.Y, _limits.Value.Top, _limits.Value.Bottom - _viewport.Height));
            }
        }

        public Matrix GetViewMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(-Position, 0));
        }
    }
}
