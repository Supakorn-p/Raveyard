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
using System.Text;


namespace Raveyard._scripts.MenuUI
{
    internal class Button
    {
        public SpriteObject buttonSprite;
        public Vector2 position;
        public Vector2 scale;
        public RectangleF buttonBounds;

        public ContextMenu parentMenu;

        public TextObject buttonLabel;

        public string click = "snd_click_menu_button";
        public string hover = "snd_hover_menu_button";

        public bool isActive = false;
        private bool isHovered = false;

        public void ButtonInit(ContentManager Content, bool _soundLoaded = true)
        {
            buttonSprite.rectangleBounds = buttonBounds;
            buttonSprite.scale = scale;

            SpriteFont font = Content.Load<SpriteFont>("anton_sc");
            buttonLabel = new TextObject(font, new StringBuilder(""));

            // Ngl I don't think I can come up with a better idea
            if (_soundLoaded == false)
            {
                new GameplaySoundLibrary(Content).LoadSound(click);
                new GameplaySoundLibrary(Content).LoadSound(hover);
            }

            buttonLabel.position = new Vector2(position.X, position.Y - 100);
            buttonLabel.scale = new Vector2(0.5f, 0.5f);
            buttonLabel.layer = 11;
            buttonLabel.color = Color.White;
            buttonLabel.active = true;
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
                GameplaySoundLibrary.PlaySound(hover);
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
