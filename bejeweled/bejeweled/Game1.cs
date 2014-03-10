using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace bejeweled
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private const int SIZE_X = 12;
        private const int SIZE_Y = 12;

        private float musicVolume = 0.4f;
        private GamePadState currentGamePadState;
        private KeyboardState currentKeyBoardState;

        private GlobalState currentState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = SIZE_X * Jewel.SIZE + 200;
            graphics.PreferredBackBufferHeight = SIZE_Y * Jewel.SIZE;
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            RessourceManager.Instance.LoadContent(this);

            this.currentState = new MainMenuState(this);

            MediaPlayer.Play(RessourceManager.Instance.GetSong("gameMusic"));
            MediaPlayer.IsRepeating = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            currentKeyBoardState = Keyboard.GetState();

            // Pour modifier le volume de la music
            if (currentGamePadState.DPad.Up == ButtonState.Pressed || currentKeyBoardState.IsKeyDown(Keys.Up))
            {
                musicVolume += 0.01f;
            }
            if (currentGamePadState.DPad.Down == ButtonState.Pressed || currentKeyBoardState.IsKeyDown(Keys.Down))
            {
                musicVolume -= 0.01f; 
            }
            MediaPlayer.Volume = musicVolume;
            
            currentState.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            currentState.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private abstract class GlobalState
        {
            protected Game1 game;

            public GlobalState(Game1 game)
            {
                this.game = game;
            }

            public abstract void Update(GameTime gameTime);

            public abstract void Draw(SpriteBatch spriteBatch);
        }

        private class MainMenuState : GlobalState
        {
            private Button easyButton;
            private Button mediumButton;
            private Button hardButton;
            private Texture2D logo;
            private Vector2 logoPosition;

            public MainMenuState(Game1 game) :
                base(game)
            {
                int buttonX = game.graphics.PreferredBackBufferWidth / 2 - Button.DEFAULT_WIDTH / 2;

                this.easyButton = new Button(new Vector2(buttonX, 150), RessourceManager.Instance.GetTexture("easy"));
                this.mediumButton = new Button(new Vector2(buttonX, 225), RessourceManager.Instance.GetTexture("medium"));
                this.hardButton = new Button(new Vector2(buttonX, 300),  RessourceManager.Instance.GetTexture("hard"));

                this.logo = RessourceManager.Instance.GetTexture("logo");
                this.logoPosition = new Vector2(game.graphics.PreferredBackBufferWidth / 2 - logo.Width / 2, 20);
            }

            public override void Update(GameTime gameTime)
            {
                easyButton.Update();
                mediumButton.Update();
                hardButton.Update();

                if (easyButton.IsClicked)
                {
                    game.currentState = new PlayState(game, Difficulty.EASY);
                }
                else if (mediumButton.IsClicked)
                {
                    game.currentState = new PlayState(game, Difficulty.MEDIUM);
                }
                else if (hardButton.IsClicked)
                {
                    game.currentState = new PlayState(game, Difficulty.HARD);
                }
            }

            public override void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(logo, logoPosition, Color.White);

                easyButton.Draw(spriteBatch);
                mediumButton.Draw(spriteBatch);
                hardButton.Draw(spriteBatch);
            }
        }

        private class PlayState : GlobalState
        {
            private Board board;

            public PlayState(Game1 game, Difficulty difficulty) :
                base(game)
            {
                this.board = new Board(SIZE_X, SIZE_Y);
            }

            public override void Update(GameTime gameTime)
            {
                board.Update(gameTime);
            }

            public override void Draw(SpriteBatch spriteBatch)
            {
                board.Draw(spriteBatch);
            }
        }
    }
}
