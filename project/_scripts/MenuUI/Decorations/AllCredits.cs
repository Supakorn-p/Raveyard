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
using System.Collections.Generic;

namespace Raveyard._scripts.MenuUI.Decorations
{
    internal class AllCredits
    {
        public Credit cred;
        public Credit cred_tokyo;
        public Credit cred_karn;
        public Credit cred_p;
        public Credit cred_o;
        public Credit cred_sebas;

        public static string flash_sound = "snd_credits_flash";

        public ContextMenu parentMenu;

        public List<Credit> all_creds = new List<Credit> { };

        float flashCountdown = 2;
        bool turnedOn = true;

        public AllCredits()
        {
            cred = new Credit();
            cred_tokyo = new Credit();
            cred_karn = new Credit();
            cred_p = new Credit();
            cred_o = new Credit();
            cred_sebas = new Credit();
        }

        int i = 0;
        public void InitAllCreds(ContentManager Content)
        {

            all_creds.Add(cred);
            all_creds.Add(cred_tokyo);
            all_creds.Add(cred_karn);
            all_creds.Add(cred_p);
            all_creds.Add(cred_o);
            all_creds.Add(cred_sebas);


            new GameplaySoundLibrary(Content).LoadSound(flash_sound);

            Dictionary<int, List<string>> allSprites = new Dictionary<int, List<string>>
            {
                {0, ["credits", ""] },
                {1, ["tokyo_credits", "Lead Programmer, Music, Sound Designer, Designer"] },
                {2, ["karn_ credits", "Lead Artist, Art Director"] },
                {3, ["opec_credits", "Charting, Level Designer"] },
                {4, ["p_credits", "QA Tester"] },
                {5, ["sebas_credits", "Programmer, Director, Additional Sounds"] }
            };

            Dictionary<int, Color> textColors = new Dictionary<int, Color>()
            {
                {0,  new Color() },
                {1,  new Color(181, 225, 208) },
                {2,  new Color(225, 191, 245) },
                {3,  new Color(214, 246, 255) },
                {4,  new Color(247, 229, 192) },
                {5,  new Color(216, 242, 203) },
            };

            foreach (Credit _credit in all_creds)
            {
                _credit.InitCred(Content, allSprites[i][0], textColors[i] ,allSprites[i][1]);

                i++;

                _credit.creditSprite.layer = 5;
                _credit.creditSprite.active = true;
                if (_credit != cred) { _credit.creditSprite.scale = new Vector2(0.75f, 0.75f); }


                if (i <= 3)
                {
                    _credit.creditSprite.position = new Vector2(parentMenu.cameraPosition.X - 400, parentMenu.cameraPosition.Y + ((i * 170) - 475));
                }
                else if (i > 3)
                {
                    _credit.creditSprite.position = new Vector2(parentMenu.cameraPosition.X + 300, parentMenu.cameraPosition.Y + ((i - 3) * 170) - 465);
                }

                _credit.roles.position = new Vector2(_credit.creditSprite.position.X, _credit.creditSprite.position.Y + 75);
                _credit.roles.layer = 5;
                _credit.roles.scale = new Vector2(0.25f, 0.25f);
                _credit.roles.active = true;
            }

            i = 0;
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (Credit _credit in all_creds)
            {
                _credit.FlashCredit(gameTime, deltaTime);
            }
        }
    }
}
