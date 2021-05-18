using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaym1
{
    class ParticleEmitter : BasicCollision
    {
        Random r = new Random();
        public float Cooldown;
        public float fadeOut;
        float currentTime = 0f;
        public int amountPerTick;
        public int emitCount;
        public List<Particle> particles = new List<Particle>();
        public Color col;

        public bool finished = false;
        public void Update(GameTime gameTime)
        {
            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (currentTime >= Cooldown && emitCount > 0)
            {
                for (int i = 0; i < amountPerTick; i++)
                {
                    particles.Add(new Particle(center, new Vector2(r.Next(-5, 6), r.Next(-5, 6)), col));
                }
                currentTime = 0;
                emitCount--;
            }
            var deadcount = 0;
            foreach (var item in particles.ToList())
            {
                if (item.isDead)
                {
                    deadcount++;
                }
                item.Update();
            }
            if (deadcount == particles.Count)
            {
                finished = true;
            }
        }
        /*public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var item in particles)
            {
                spriteBatch.Draw(item.texture, item.Rectangle, item.color);
            }
        }*/
        public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            foreach (var item in particles)
            {
                spriteBatch.Draw(pixel, item.Rectangle, item.color * item.transparency);
            }
        }
    }
}
