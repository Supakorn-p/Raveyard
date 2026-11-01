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

namespace Raveyard._scripts.MenuUI.ButtonTypes
{
    internal class SwitchMenu : Button
    {
        public ContextMenu menuToSwitch;

        public SwitchMenu(bool _isActive, Vector2 _scale)
        {
            isActive = _isActive;
            scale = _scale;
        }
        public override void ButtonPressed(string _sfx = "snd_click_menu_button")
        {
            if (menuToSwitch != null)
            {
                parentMenu.switchOnExit = menuToSwitch;
            }

            buttonSprite.tweener.TweenTo(target: buttonSprite, player => player.scale,
                   toValue: scale * 0.5f, duration: 0.1f)
                   .Easing(EasingFunctions.CubicOut)
                   .OnEnd(tweener => buttonSprite.tweener.TweenTo(target: buttonSprite, player => player.scale,
                   toValue: scale, duration: 0.2f)
                   .Easing(EasingFunctions.CubicInOut));

            parentMenu.Exit();
            GameplaySoundLibrary.PlaySound(_sfx);
        }
    }
}
