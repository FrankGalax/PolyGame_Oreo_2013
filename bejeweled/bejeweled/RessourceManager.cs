using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;
using Microsoft.Xna.Framework.Media;

namespace bejeweled
{
    public class RessourceManager
    {
        private static RessourceManager instance;
        private Dictionary<String, Texture2D> textures;
        private Dictionary<String, SoundEffect> soundEffects;
        private Dictionary<String, SpriteFont> fonts;
        private Dictionary<String, Song> songs;

        private RessourceManager() 
        {
            textures = new Dictionary<string,Texture2D>();
            soundEffects = new Dictionary<string, SoundEffect>();
            fonts = new Dictionary<string, SpriteFont>();
            songs = new Dictionary<string, Song>();
        }

        public static RessourceManager Instance
        {
            get 
            {
                if (instance == null)
                    instance = new RessourceManager();
                return instance;
            }
        }

        public void LoadContent(Game1 game)
        {
            textures.Add("logo", game.Content.Load<Texture2D>("logo"));

            textures.Add("easy", game.Content.Load<Texture2D>("easy"));
            textures.Add("medium", game.Content.Load<Texture2D>("medium"));
            textures.Add("hard", game.Content.Load<Texture2D>("hard"));

            textures.Add("blueJewel", game.Content.Load<Texture2D>("blueJewel"));
            textures.Add("redJewel", game.Content.Load<Texture2D>("redJewel"));
            textures.Add("greenJewel", game.Content.Load<Texture2D>("greenJewel"));
            textures.Add("yellowJewel", game.Content.Load<Texture2D>("yellowJewel"));
            textures.Add("orangeJewel", game.Content.Load<Texture2D>("orangeJewel"));
            textures.Add("purpleJewel", game.Content.Load<Texture2D>("purpleJewel"));
            textures.Add("select", game.Content.Load<Texture2D>("select"));

            soundEffects.Add("SwapingJewel01",game.Content.Load<SoundEffect>("SwapingJewel01"));
            soundEffects.Add("SwapingJewel02", game.Content.Load<SoundEffect>("SwapingJewel02"));
            soundEffects.Add("jewelExplosion01", game.Content.Load<SoundEffect>("jewelExplosion01"));

            fonts.Add("default", game.Content.Load<SpriteFont>("defaultFont"));
            fonts.Add("scorePopup", game.Content.Load<SpriteFont>("scorePopupFont"));

            songs.Add("gameMusic", game.Content.Load<Song>("JewelSong1"));
        }


        public Texture2D GetTexture(String key)
        {
            Debug.Assert(textures.ContainsKey(key));
            return textures[key];
        }

        public SoundEffect GetSound(String key)
        {
            Debug.Assert(soundEffects.ContainsKey(key));
            return soundEffects[key];
        }

        public SpriteFont GetFont(String key)
        {
            Debug.Assert(fonts.ContainsKey(key));
            return fonts[key];
        }

        public Song GetSong(String key)
        {
            Debug.Assert(songs.ContainsKey(key));
            return songs[key];
        }
    }
}
