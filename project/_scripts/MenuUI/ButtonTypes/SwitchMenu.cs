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
        public override void ButtonPressed()
        {
            parentMenu.switchOnExit = menuToSwitch;
            parentMenu.Exit();
        }
    }
}
