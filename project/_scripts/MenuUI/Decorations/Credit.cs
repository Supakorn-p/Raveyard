using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raveyard._scripts.MenuUI.Decorations
{
    internal class Credit
    {
        public SpriteObject creditSprite;
        public TextObject roles;

        Random random;
        private Color designatedColor;

        bool isFlashing = false;

        float randomCooldown;
        float flashCooldown = 1;
        int i;

        public void InitCred(ContentManager Content, string _spriteName, Color _textColor, string _roles = "")
        {
            SpriteFont font = Content.Load<SpriteFont>("anton_sc");
            roles = new TextObject(font, new StringBuilder(_roles));

            creditSprite = new SpriteObject("cred",
            Content.Load<Texture2D>(_spriteName), new Vector2(660, 322),
            new Vector2(0, 0));

            designatedColor = _textColor;
            roles.color = _textColor;
        }

        public void FlashCredit(GameTime gameTime, float delta)
        {
            // Exquisite code that definitely is not flawed and overdone in any way
            if (isFlashing == false)
            {
                if (randomCooldown > 0)
                {
                    randomCooldown -= delta;
                }
                else
                {
                    random = new Random();
                    if (random.Next(1, 5) == 1)
                    {
                        isFlashing = true;
                    }
                    else
                    {
                        randomCooldown = random.Next(3, 6);
                    }
                }
            }
            else
            {
                if (flashCooldown > 0)
                {
                    flashCooldown -= delta;
                }
                else
                {
                    if (i < 2)
                    {
                        if (creditSprite.animatedSprite.Color == Color.White)
                        {
                            creditSprite.animatedSprite.Color = Color.Black;
                            roles.color = Color.Black;
                            flashCooldown = 0.15f;
                        }
                        else
                        {
                            GameplaySoundLibrary.PlaySound("snd_credits_flash", 1, 0, random.NextSingle() * (0.5f));
                            creditSprite.animatedSprite.Color = Color.White;
                            roles.color = designatedColor;
                            flashCooldown = 0.15f;
                            i++;
                        }
                    }
                    else
                    {
                        isFlashing = false;
                        randomCooldown = random.Next(3, 6);
                        flashCooldown = 0.15f;
                        i = 0;
                    }
                }
            }
        }
    }
}
