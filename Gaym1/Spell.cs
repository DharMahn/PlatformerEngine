using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gaym1
{
    class Spell : Entity
    {
        int damage = 10;
        int timer = 100;
        public bool finished = false;
        public Spell(Vector2 _pos, Vector2 dir, Color c, int _damage, Vector2 _size)
        {
            pos = _pos;
            velocity = dir;
            color = c;
            damage = _damage;
            HitboxSize = _size;
        }
        public bool isExploded = false;
        public ParticleEmitter explosion = new ParticleEmitter();
        public void Update(List<Entity> platforms, GameTime gt)
        {
            if (timer == 0)
            {
                SummonExplosion(true);
                timer = -1;
            }
            if (!isExploded)
            {
                Console.WriteLine(timer);
                Console.WriteLine(platforms.Count);
                Stopwatch sw = new Stopwatch();
                sw.Start();
                foreach (var item in platforms)
                {
                    if (!item.exploded)
                    {
                        if (IsTouchingBottom(item))
                        {
                            pos = new Vector2(pos.X, item.Bottom);
                            item.hp -= damage;
                            SummonExplosion(true);
                            break;
                        }
                        else if (IsTouchingTop(item))
                        {
                            pos = new Vector2(pos.X, item.Top - HitboxSize.Y);
                            item.hp -= damage;

                            SummonExplosion(true);
                            break;

                        }
                        if (IsTouchingLeft(item))
                        {
                            pos = new Vector2(item.Left - HitboxSize.X, pos.Y);
                            item.hp -= damage;

                            SummonExplosion(true);
                            break;

                        }
                        else if (IsTouchingRight(item))
                        {
                            pos = new Vector2(item.Right, pos.Y);
                            item.hp -= damage;

                            SummonExplosion(true);
                            break;

                        }
                    }

                }
                sw.Stop();
                Console.WriteLine("checking collisions took " + sw.ElapsedMilliseconds + " milliseconds");
                timer--;
                pos += velocity;
            }
            else
            {
                explosion.Update(gt);
                if (explosion.finished)
                {
                    finished = true;
                }
            }
        }
        public void SummonExplosion(bool explode)
        {
            isExploded = explode;
            explosion = new ParticleEmitter
            {
                pos = center,
                amountPerTick = 20,
                Cooldown = 0f,
                emitCount = 1,
                fadeOut = 0.1f,
                col = color,
            };
        }

    }
}
