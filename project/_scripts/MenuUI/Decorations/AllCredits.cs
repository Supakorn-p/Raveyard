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
        public SpriteObject cred;
        public SpriteObject cred_tokyo;
        public SpriteObject cred_karn;
        public SpriteObject cred_p;
        public SpriteObject cred_o;
        public SpriteObject cred_sebas;

        public List<SpriteObject> all_creds = new List<SpriteObject> { };

        public Vector2 center_position;

        public AllCredits()
        {
            all_creds.Add(cred);
            all_creds.Add(cred_tokyo);
            all_creds.Add(cred_karn);
            all_creds.Add(cred_p);
            all_creds.Add(cred_o);
            all_creds.Add(cred_sebas);
        }

        public void InitAllCreds(ContentManager Content)
        {

            cred_tokyo = new SpriteObject("tokyo_cred",
            Content.Load<Texture2D>("tokyo_credits"), new Vector2(660, 332),
            new Vector2(0, 0));

            cred_sebas = new SpriteObject("sebas_cred",
            Content.Load<Texture2D>("sebas_credits"), new Vector2(660, 332),
            new Vector2(0, 0));

            cred_karn = new SpriteObject("karn_cred",
            Content.Load<Texture2D>("karn_credits"), new Vector2(660, 332),
            new Vector2(0, 0));

            cred_o = new SpriteObject("opec_cred",
            Content.Load<Texture2D>("opec_credits"), new Vector2(660, 332),
            new Vector2(0, 0));

            cred_p = new SpriteObject("p_cred",
            Content.Load<Texture2D>("p_credits"), new Vector2(660, 332),
            new Vector2(0, 0));

            cred = new SpriteObject("cred",
            Content.Load<Texture2D>("credits"), new Vector2(660, 332),
            new Vector2(0, 0));



            //foreach (SpriteObject _sprite in all_creds)
            //{
            //    _sprite.position = 
            //}
        }

    }
}
