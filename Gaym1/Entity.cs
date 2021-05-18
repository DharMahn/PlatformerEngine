using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gaym1
{
    public class Entity : BasicCollision
    {
        Random r = new Random();
        int explosionParticleSize = 10;
        public int maxhp { get; set; }
        public int hp { get; set; }

        public Vector2 TextureSize;
        public bool flipped = false;

        public Texture2D texture;

        public bool isDeadly;

        public Color color;

        public bool exploded = false;
        public bool hitboxEnabled = true;

        public List<Particle> explosionParticles = new List<Particle>();
        public Entity()
        {
            pos = new Vector2(0, 0);
            velocity = new Vector2(0, 0);
            maxhp = 1;
            hp = 100;
            HitboxSize = new Vector2(20, 20);
            exploded = false;
        }
        public Entity(int _maxhp, Vector2 _pos, Vector2 _size, ContentManager content, string textureName)
        {
            maxhp = _maxhp;
            hp = _maxhp;
            pos = _pos;
            texture = content.Load<Texture2D>(textureName);
            HitboxSize = _size;
        }
        public Entity(int _maxhp, Vector2 _pos, Vector2 _size, ContentManager content)
        {
            HitboxSize = _size;
            maxhp = _maxhp;
            hp = _maxhp;
            pos = _pos;
            texture = content.Load<Texture2D>("missing");
            LoadPlaceholderTexture(texture);
        }

        public Rectangle Rectangle
        {
            get { return new Rectangle((int)pos.X, (int)pos.Y, (int)HitboxSize.X, (int)HitboxSize.Y); }
        }

        public int Left
        {
            get { return (int)pos.X; }
            set { pos = new Vector2(value, pos.Y); }
        }
        public int Top
        {
            get { return (int)pos.Y; }
            set { pos = new Vector2(pos.X, value); }
        }
        public int Right
        {
            get { return (int)pos.X + (int)HitboxSize.X; }
        }
        public int Bottom
        {
            get { return (int)pos.Y + (int)HitboxSize.Y; }
        }

        public void LoadPlaceholderTexture(Texture2D texture)
        {
            Color[] data = new Color[20 * 20];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Color.Chocolate;
            }
            texture.SetData(data);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (exploded)
            {
                foreach (var item in explosionParticles.ToList())
                {
                    item.Update();
                    if (item.transparency <= 0)
                    {
                        explosionParticles.Remove(item);
                    }
                    spriteBatch.Draw(item.texture, item.Rectangle, item.color * item.transparency);
                }
            }
            else
            {
                spriteBatch.Draw(texture, Rectangle, color);
            }
        }
        public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            if (exploded)
            {
                foreach (var item in explosionParticles.ToList())
                {
                    item.Update();
                    if (item.transparency <= 0)
                    {
                        explosionParticles.Remove(item);
                    }
                    spriteBatch.Draw(pixel, item.Rectangle, item.color * item.transparency);
                }
            }
            else
            {
                spriteBatch.Draw(pixel, Rectangle, color);
            }
        }

        public void Explode()
        {
            hitboxEnabled = false;
            for (int y = 0; y < HitboxSize.Y; y += explosionParticleSize)
            {
                for (int x = 0; x < HitboxSize.X; x += explosionParticleSize)
                {
                    int particleX = 0;
                    int particleY = 0;
                    if (x + explosionParticleSize >= HitboxSize.X)
                    {
                        particleX = (int)HitboxSize.X - x;
                    }
                    else
                    {
                        particleX = x + explosionParticleSize;
                    }

                    if (y + explosionParticleSize >= HitboxSize.Y)
                    {
                        particleY = (int)HitboxSize.Y - y;
                    }
                    else
                    {
                        particleY = y + explosionParticleSize;
                    }

                    explosionParticles.Add(new Particle(new Vector2(particleX+pos.X, particleY+pos.Y),new Vector2(explosionParticleSize), new Vector2(r.Next(-2, 3), r.Next(-1, 2)), color));
                }
            }
        }
    }
}
