using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Gaym1
{
    public class Particle : Entity
    {
        public Vector2 gravity = new Vector2(0, 1);
        public float transparency=1;
        public bool isDead = false;
        public Particle(Vector2 p, Vector2 dir, Color color)
        {
            pos = p;
            velocity = dir;
            base.color = color;
        }
        public Particle(Vector2 p, Vector2 s, Vector2 dir, Color color)
        {
            pos = p;
            velocity = dir;
            base.color = color;
            HitboxSize = s;
        }
        public void Update()
        {
            if (transparency > 0)
            {
                pos += velocity;
                velocity += gravity;
                transparency -= 0.05f;
            }
            else
            {
                isDead = true;
            }
        }
    }
}
