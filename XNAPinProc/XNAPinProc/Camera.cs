using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNAPinProc
{
    public class Camera
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Zoom { get; set; }

        private bool screenFlipped = false;
        public bool ScreenFlipped
        {
            get
            {
                return screenFlipped;
            }
            set
            {
                screenFlipped = value;
                if (screenFlipped)
                {
                    Rotation = MathHelper.ToRadians(180);
                    Position = new Vector2(800, 600);
                }
                else
                {
                    Rotation = 0;
                    Position = new Vector2(0, 0);
                }
            }
        }

        public Camera()
        {
            Position = Vector2.Zero;
            Zoom = 1f;
        }

        public Matrix TransformMatrix
        {
            get
            {
                if (ScreenFlipped)
                {
                    return Matrix.CreateScale(-1, 1, 1) * Matrix.CreateTranslation(400, 0, 0);
                    
                }
                else
                {
                    return Matrix.CreateRotationZ(Rotation) * Matrix.CreateScale(Zoom) *
                            Matrix.CreateTranslation(Position.X, Position.Y, 0);
                }
            }
        }
    }
}
