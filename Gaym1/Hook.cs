using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace Gaym1
{
    class Hook : BasicCollision
    {
        public bool hooked = false;
        public bool inAir = false;
        public bool justHooked = true;
        public Hook(Vector2 position, Vector2 vel)
        {
            pos = position;
            velocity = vel;
            HitboxSize = Vector2.Zero;
            if (position != Vector2.Zero && velocity != Vector2.Zero)
            {
                inAir = true;
            }
        }
        public void Update(List<Entity> platforms)
        {
            if (inAir)
            {
                foreach (var item in platforms)
                {
                    if (item.hitboxEnabled)
                    {
                        if (IsTouchingBottom(item))
                        {
                            pos = new Vector2(pos.X, item.Bottom);
                            hooked = true;
                            inAir = false;
                        }
                        else if (IsTouchingTop(item))
                        {
                            pos = new Vector2(pos.X, item.Top);
                            hooked = true;
                            inAir = false;
                        }
                        else if (IsTouchingLeft(item))
                        {
                            pos = new Vector2(item.Left, pos.Y);
                            hooked = true;
                            inAir = false;
                        }
                        else if (IsTouchingRight(item))
                        {
                            pos = new Vector2(item.Right, pos.Y);
                            hooked = true;
                            inAir = false;
                        }
                        else if (pos.X > item.pos.X && pos.Y > item.pos.Y && pos.X < item.Right && pos.Y < item.Bottom)
                        {
                            hooked = true;
                            inAir = false;
                        }
                    }
                }
                pos += velocity;
            }
        }
    }
}