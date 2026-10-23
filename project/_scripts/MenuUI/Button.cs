using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Tweening;
using Raveyard._scripts.Visuals;
using System.Diagnostics;


namespace Raveyard._scripts.MenuUI
{
    internal class Button
    {
        public SpriteObject buttonSprite;
        public Vector2 position;
        public Vector2 scale;
        public RectangleF buttonBounds;

        public ContextMenu parentMenu;

        public bool isActive = false;
        private bool isHovered = false;

        public void ButtonInit()
        {
            buttonSprite.rectangleBounds = buttonBounds;
            buttonSprite.scale = scale;
        }

        public void Update(GameTime gameTime)
        {
            if (isActive == true)
            {
                if (buttonSprite.rectangleBounds.Intersects(new RectangleF(Mouse.GetState().Position.X, Mouse.GetState().Position.Y, 20, 20)))
                {
                    ButtonHover();
                }
                else
                {
                    ExitHover();
                }

                if (isHovered == true)
                {
                    MouseExtended.Update();
                    MouseStateExtended _mouseState = MouseExtended.GetState();
                    if (_mouseState.WasButtonPressed(MouseButton.Left))
                    {
                        ButtonPressed();
                    }
                }
            }
        }

        private void ButtonHover()
        {
            if (isHovered == false)
            {
                buttonSprite.tweener.TweenTo(target: buttonSprite, player => player.scale,
                    toValue: scale * 1.25f, duration: 0.3f)
                    .Easing(EasingFunctions.CubicInOut);
                isHovered = true;
            }
        }

        private void ExitHover()
        {
            if (isHovered == true)
            {
                buttonSprite.tweener.TweenTo(target: buttonSprite, player => player.scale,
                    toValue: scale, duration: 0.5f)
                    .Easing(EasingFunctions.CubicInOut);
                isHovered = false;
            }
        }

        private void ButtonMove()
        {

        }

        public virtual void ButtonPressed() { }
    }
}
