using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Gaym1
{
    class Input
    {
        public KeyboardState currentKeyboardState = Keyboard.GetState(), lastKeyboardState;

        public Keys Left { get; set; }
        public Keys Right { get; set; }
        public Keys Up { get; set; }
        public Keys Down { get; set; }
        public Keys Shoot { get; set; }
        public bool isKeyDownOnce(Keys key)
        {
            if (currentKeyboardState.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void update()
        {
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
        }
    }
}
