using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bejeweled
{
    public class Button
    {
        public static int DEFAULT_WIDTH = 202;
        public static int DEFAULT_HEIGHT = 52;
        private static int DEFAULT_HOVER_OFFSET_X = 0;
        private static int DEFAULT_HOVER_OFFSET_Y = 52;

        private Vector2 position;
        private Texture2D texture;
        private int width;
        private int height;
        private int hoverOffsetX;
        private int hoverOffsetY;

        private bool isClicked;
        private bool isHover;

        private MouseState lastMouseState;
        private MouseState mouseState;

        public Button(Vector2 position, Texture2D texture)
        {
            this.position = position;
            this.texture = texture;
            this.width = DEFAULT_WIDTH;
            this.height = DEFAULT_HEIGHT;
            this.hoverOffsetX = DEFAULT_HOVER_OFFSET_X;
            this.hoverOffsetY = DEFAULT_HOVER_OFFSET_Y;

            this.isClicked = false;
            this.isHover = false;

            this.mouseState = Mouse.GetState();
            this.lastMouseState = mouseState;
        }

        public void Update()
        {
            lastMouseState = mouseState;
            mouseState = Mouse.GetState();
            int x = mouseState.X;
            int y = mouseState.Y;

            if (x >= position.X && x < position.X + width &&
                y >= position.Y && y < position.Y + height)
            {
                isHover = true;

                if (lastMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed)
                {
                    isClicked = true;
                }
                else
                    isClicked = false;
            }
            else
                isHover = false;
        }

        public void Draw(SpriteBatch batch)
        {
            if (isHover)
            {
                batch.Draw(
                    texture,
                    new Rectangle((int)position.X, (int)position.Y, width, height),
                    new Rectangle(hoverOffsetX, hoverOffsetY, width, height),
                    Color.White
                );
            }
            else
            {
                batch.Draw(
                    texture,
                    new Rectangle((int)position.X, (int)position.Y, width, height),
                    new Rectangle(0, 0, width, height),
                    Color.White
                );
            }
        }

        public bool IsClicked
        {
            get { return isClicked; }
        }
    }
}
