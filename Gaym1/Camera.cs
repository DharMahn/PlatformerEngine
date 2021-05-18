using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gaym1
{
    class Camera
    {
        public Matrix Transform { get; private set; }

        public void Follow(Entity target)
        {
            var position = Matrix.CreateTranslation(-target.pos.X - (target.HitboxSize.X / 2), -target.pos.Y - (target.HitboxSize.Y / 2), 0);
            var offset = Matrix.CreateTranslation(Game1.ScreenWidth / 2, Game1.ScreenHeight-(Game1.ScreenHeight / 2.5f), 0);
            Transform = position * offset;
        }
    }
}
