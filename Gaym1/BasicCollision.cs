using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaym1
{
    public class BasicCollision
    {
        public Vector2 pos { get; set; }
        public Vector2 velocity { get; set; }
        public Vector2 HitboxSize { get; set; }
        
        public Vector2 center
        {
            get { return new Vector2(HitboxSize.X / 2 + pos.X, HitboxSize.Y / 2 + pos.Y); }
        }
        public bool IsTouchingLeft(Entity sprite)
        {
            return (int)pos.X + HitboxSize.X + velocity.X > (int)sprite.pos.X &&
                   (int)pos.X < (int)sprite.pos.X &&
                   (int)pos.Y + HitboxSize.Y > (int)sprite.pos.Y &&
                   (int)pos.Y < (int)sprite.pos.Y + (int)sprite.HitboxSize.Y;
        }

        public bool IsTouchingRight(Entity sprite)
        {
            return (int)pos.X + velocity.X < (int)sprite.pos.X + (int)sprite.HitboxSize.X &&
                   (int)pos.X + HitboxSize.X > (int)sprite.pos.X + (int)sprite.HitboxSize.X &&
                   (int)pos.Y + HitboxSize.Y > (int)sprite.pos.Y &&
                   (int)pos.Y < (int)sprite.pos.Y + (int)sprite.HitboxSize.Y;
        }

        public bool IsTouchingTop(Entity sprite)
        {
            return (int)pos.Y + HitboxSize.Y + velocity.Y > (int)sprite.pos.Y &&
                   (int)pos.Y < (int)sprite.pos.Y &&
                   (int)pos.X + HitboxSize.X > (int)sprite.pos.X &&
                   (int)pos.X < (int)sprite.pos.X + (int)sprite.HitboxSize.X;
        }

        public bool IsTouchingBottom(Entity sprite)
        {
            return (int)pos.Y + velocity.Y < (int)sprite.pos.Y + (int)sprite.HitboxSize.Y &&
                   (int)pos.Y + HitboxSize.Y > (int)sprite.pos.Y + (int)sprite.HitboxSize.Y &&
                   (int)pos.X + HitboxSize.X > (int)sprite.pos.X &&
                   (int)pos.X < (int)sprite.pos.X + (int)sprite.HitboxSize.X;
        }
    }
}
