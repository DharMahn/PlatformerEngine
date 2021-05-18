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
    class Player : Entity
    {
        public bool moving = false;
        public int frame = 1;
        int limit = 7;
        float countDuration = .25f;
        float currentTime = 0f;
        public Input input;
        public float maxSpeed = 15;
        public Vector2 gravity = new Vector2(0, 1);
        const int maxjump = 3;
        int jump = 3;
        public bool touchRight, touchLeft, touchTop;
        public List<Spell> Projectiles = new List<Spell>();
        MouseState prevstate;

        public Hook hook = new Hook(Vector2.Zero, Vector2.Zero);

        public Player()
        {

        }

        public Player(int _maxhp, Vector2 _pos, ContentManager content, string textureName)
        {
            maxhp = _maxhp;
            hp = _maxhp;
            pos = _pos;
            try
            {
                texture = content.Load<Texture2D>(textureName);

            }
            catch (Exception)
            {
                LoadPlaceholderTexture(texture);
            }
        }
        public Player(int _maxhp, Vector2 _pos, ContentManager content, string textureName, Input _input)
        {
            maxhp = _maxhp;
            hp = _maxhp;
            pos = _pos;
            try
            {
                texture = content.Load<Texture2D>(textureName);

            }
            catch (Exception)
            {
                LoadPlaceholderTexture(texture);
            }
            input = _input;

        }
        public Player(int _maxhp, Vector2 _pos, ContentManager content, string textureName, Input _input, Color _playerColor)
        {
            maxhp = _maxhp;
            hp = _maxhp;
            pos = _pos;
            try
            {
                texture = content.Load<Texture2D>(textureName);

            }
            catch (Exception)
            {
                LoadPlaceholderTexture(texture);
            }
            input = _input;
            color = _playerColor;
            HitboxSize = new Vector2(50, 50);

        }

        public Player(int _maxhp, Vector2 _pos, ContentManager content)
        {
            maxhp = _maxhp;
            hp = _maxhp;
            pos = _pos;
            try
            {
                texture = content.Load<Texture2D>("missing");

            }
            catch (Exception)
            {
                LoadPlaceholderTexture(texture);
            }
        }
        public Player(int _maxhp, Vector2 _pos, ContentManager content, string textureName, Input _input, Color _playerColor, Vector2 hbsize, Vector2 textsize)
        {
            maxhp = _maxhp;
            hp = _maxhp;
            pos = _pos;
            try
            {
                texture = content.Load<Texture2D>(textureName);

            }
            catch (Exception)
            {
                LoadPlaceholderTexture(texture);
            }
            input = _input;
            color = _playerColor;
            HitboxSize = hbsize;
            TextureSize = textsize;
        }

        public void Update(List<Entity> platforms, GameTime gameTime, Vector2 mousePosWithCamera)
        {
            input.update();
            Move(mousePosWithCamera);

            touchLeft = false;
            touchRight = false;
            touchTop = false;
            if ((prevstate.RightButton == ButtonState.Released) && (Mouse.GetState().RightButton == ButtonState.Pressed))
            {
                Vector2 vel = new Vector2(mousePosWithCamera.X - (pos.X + (HitboxSize.X / 2f)), mousePosWithCamera.Y - (pos.Y + (HitboxSize.Y / 2f)));
                vel.Normalize();
                vel *= 30f;
                hook = new Hook(new Vector2(pos.X + (HitboxSize.X / 2f), pos.Y + (HitboxSize.Y / 2f)), vel);
            }
            hook.Update(platforms);
            if ((prevstate.RightButton == ButtonState.Pressed) && (Mouse.GetState().RightButton == ButtonState.Released))
            {
                hook.inAir = false;
                hook.hooked = false;
            }
            if (((((hook.pos.X - pos.X) - (HitboxSize.X / 2f)) * ((hook.pos.X - pos.X) - (HitboxSize.X / 2f))) + (((hook.pos.Y - pos.Y) - (HitboxSize.Y / 2f)) * ((hook.pos.Y - pos.Y) - (HitboxSize.Y / 2f)))) > 400000f)
            {
                hook.inAir = false;
            }
            if (hook.hooked)
            {
                if (hook.justHooked)
                {
                    velocity = new Vector2(velocity.X, velocity.Y / 15f);
                    hook.justHooked = false;
                }
                Vector2 vector2 = new Vector2((hook.pos.X - pos.X) - (HitboxSize.X / 2f), (hook.pos.Y - pos.Y) - (HitboxSize.Y / 2f));
                vector2.Normalize();
                vector2 *= new Vector2(2f, 0.75f);
                velocity += vector2;
            }
            for (int i = 0; i < platforms.Count; i++)
            {
                if (platforms[i].hitboxEnabled)
                {
                    if (platforms[i].isDeadly)
                    {
                        /*if (IsTouchingBottom(platforms[i]) || (IsTouchingTop(platforms[i]) || (IsTouchingLeft(platforms[i]) || IsTouchingRight(platforms[i]))))
                        {
                            playerColor = Color.Red;
                        }*/
                    }
                    else
                    {
                        if (IsTouchingLeft(platforms[i]))
                        {
                            touchLeft = true;
                            if ((platforms[i].Top + 15) <= Bottom)
                            {
                                pos = new Vector2(platforms[i].Left - HitboxSize.X, pos.Y);
                                velocity = new Vector2(0f, velocity.Y);
                                if (velocity.Y > 2f)
                                {
                                    velocity = new Vector2(velocity.X, 2f);
                                }
                            }
                        }
                        else if (IsTouchingRight(platforms[i]))
                        {
                            touchRight = true;
                            if ((platforms[i].Top + 15) <= Bottom)
                            {
                                pos = new Vector2(platforms[i].Right, pos.Y);
                                velocity = new Vector2(0f, velocity.Y);
                                if (velocity.Y > 2f)
                                {
                                    velocity = new Vector2(velocity.X, 2f);
                                }
                            }
                        }
                        if (IsTouchingTop(platforms[i]))
                        {
                            pos = new Vector2(pos.X, platforms[i].Top - HitboxSize.Y);
                            velocity = new Vector2(velocity.X, 0f);
                            jump = maxjump;
                            touchTop = true;
                        }
                        else if (IsTouchingBottom(platforms[i]))
                        {
                            pos = new Vector2(pos.X, platforms[i].Bottom);
                            velocity = new Vector2(velocity.X, 0f);
                        }
                    }
                }
            }
            prevstate = Mouse.GetState();
            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (currentTime >= countDuration)
            {
                frame++;
                currentTime = 0;
            }
            if (frame >= limit)
            {
                frame = 0;

            }
            if (!hook.hooked)
            {
                velocity += gravity;
            }
            countDuration = 1 / Math.Abs(velocity.X);
            foreach (var item in Projectiles.ToList())
            {
                if (!item.finished)
                {
                    item.Update(platforms, gameTime);
                }
                else
                {
                    Projectiles.Remove(item);
                }
            }
            pos += velocity;
        }

        private void Jump()
        {
            velocity = new Vector2(velocity.X, -20);
            if (touchLeft && !touchTop)
            {
                velocity += new Vector2(-10, 0);
                touchLeft = false;
            }
            else if (touchRight && !touchTop)
            {
                velocity += new Vector2(10, 0);
                touchRight = false;
            }
            touchTop = false;
            jump--;
        }

        private void Move(Vector2 mousePosWithCamera)
        {
            if (input != null)
            {

                if (input.isKeyDownOnce(input.Shoot))
                {

                    Vector2 vel = new Vector2(mousePosWithCamera.X - (pos.X + (HitboxSize.X / 2f)), mousePosWithCamera.Y - (pos.Y + (HitboxSize.Y / 2f)));
                    vel.Normalize();
                    vel *= 10f;
                    if (vel.X > 0)
                    {
                        flipped = true;
                    }
                    else
                    {
                        flipped = false;
                    }
                    Projectiles.Add(new Spell(center, vel, Color.Pink, 100, new Vector2(20, 20)));
                }
                moving = false;
                if (input.currentKeyboardState.IsKeyDown(input.Left))
                {
                    flipped = false;
                    if (velocity.X > -maxSpeed)
                    {
                        velocity = !hook.hooked ? (velocity + new Vector2(-1f, 0f)) : (velocity + new Vector2(-0.25f, 0f));
                    }
                    moving = true;
                }
                else if (velocity.X < 0f)
                {
                    velocity = (velocity.X <= -1f) ? (!hook.hooked ? (velocity + new Vector2(1f, 0f)) : (velocity + new Vector2(0.25f, 0f))) : new Vector2(0f, velocity.Y);
                }
                if (input.currentKeyboardState.IsKeyDown(input.Right))
                {
                    moving = true;
                    flipped = true;
                    if (velocity.X < maxSpeed)
                    {
                        velocity = !hook.hooked ? (velocity + new Vector2(1f, 0f)) : (velocity + new Vector2(0.25f, 0f));
                    }
                }
                else if (velocity.X > 0f)
                {
                    velocity = (velocity.X >= 1f) ? (!hook.hooked ? (velocity + new Vector2(-1f, 0f)) : (velocity + new Vector2(-0.25f, 0f))) : new Vector2(0f, velocity.Y);
                }
                if (input.isKeyDownOnce(input.Up) && (jump > 0))
                {
                    Jump();
                    moving = true;

                }
                if (input.currentKeyboardState.IsKeyDown(input.Down) && (!hook.hooked && !hook.hooked))
                {
                    velocity += new Vector2(0f, 3f);
                    //moving = true;

                }
            }
            else
            {

            }
        }
    }
}
