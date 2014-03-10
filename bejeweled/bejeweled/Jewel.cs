using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bejeweled
{
    public class Jewel
    {
        private static float SWAP_SPEED = 0.005f;

        public static int SIZE = 36;

        private static Random randomNumber = new Random();

        // Couleur du jewel
        private Color1 color;

        // Différence en X entre le coin supérieur gauche de la case du jewel et où on doit l'afficher
        private float dx;

        // Différence en Y entre le coin supérieur gauche de las case du jewel et où on doit l'afficher
        private float dy;

        // Texture du jewel
        private Texture2D texture;

        /// <summary>
        /// Constructeur par défaut, attribue une couleur aléatoire au jewel
        /// </summary>
        public Jewel()
        {
            this.color = (Color1)randomNumber.Next(Enum.GetNames(typeof(Color1)).Length);
            switch (this.color)
            {
            case Color1.BLUE:
                this.texture = RessourceManager.Instance.GetTexture("blueJewel");
                break;
            case Color1.GREEN:
                this.texture = RessourceManager.Instance.GetTexture("greenJewel");
                break;
            case Color1.ORANGE:
                this.texture = RessourceManager.Instance.GetTexture("orangeJewel");
                break;
            case Color1.PURPLE:
                this.texture = RessourceManager.Instance.GetTexture("purpleJewel");
                break;
            case Color1.RED:
                this.texture = RessourceManager.Instance.GetTexture("redJewel");
                break;
            case Color1.YELLOW:
                this.texture = RessourceManager.Instance.GetTexture("yellowJewel");
                break;
            }
            this.dx = 0;
            this.dy = 0;
        }
        public Jewel(Jewel copy)
        {
            this.color = copy.Color;
            this.dx = copy.Dx;
            this.dy = copy.Dy;
            this.texture = copy.Texture;

        }

        /// <summary>
        /// Constructeur par paramètre, attribue la couleur passé en paramètre au jewel
        /// </summary>
        /// <param name="color"></param>
        public Jewel(Color1 color)
        {
            this.color = color;
            this.dx = 0;
            this.dy = 0;
            switch (color)
            {
            case Color1.BLUE:
                this.texture = RessourceManager.Instance.GetTexture("blueJewel");
                break;
            case Color1.GREEN:
                this.texture = RessourceManager.Instance.GetTexture("greenJewel");
                break;
            case Color1.ORANGE:
                this.texture = RessourceManager.Instance.GetTexture("orangeJewel");
                break;
            case Color1.PURPLE:
                this.texture = RessourceManager.Instance.GetTexture("purpleJewel");
                break;
            case Color1.RED:
                this.texture = RessourceManager.Instance.GetTexture("redJewel");
                break;
            case Color1.YELLOW:
                this.texture = RessourceManager.Instance.GetTexture("yellowJewel");
                break;
            }
        }

        /// <summary>
        /// Lorsqu'on déplace un jewel vers le haut, dy doit valoir 1
        /// </summary>
        public void MoveUp()
        {
            dy = 1;
            dx = 0;
        }

        /// <summary>
        /// Lorsqu'on déplace un jewel vers la droite, dx doit valoir -1
        /// </summary>
        public void MoveRight()
        {
            dx = -1;
            dy = 0;
        }

        /// <summary>
        /// Lorsqu'on déplace un jewel vers le bas, dy doit valoir -1
        /// </summary>
        public void MoveDown()
        {
            dy = -1;
            dx = 0;
        }

        public void MoveDown(int n)
        {
            dy = -n;
            dx = 0;
        }

        /// <summary>
        /// Lorsqu'on déplace un jewel vers la gauche, dx doit valoir 1
        /// </summary>
        public void MoveLeft()
        {
            dx = 1;
            dy = 0;
        }

        /// <summary>
        /// Remettre dx et dy tranquillement à 0 s'ils n'y sont pas. À l'affichage, ceci aura pour effet
        /// de faire glisser le jewel vers sa nouvelle case
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (dx > 0)
                dx = Math.Max((float)(dx - SWAP_SPEED * gameTime.ElapsedGameTime.TotalMilliseconds), 0);
            else if (dx < 0)
                dx = Math.Min((float)(dx + SWAP_SPEED * gameTime.ElapsedGameTime.TotalMilliseconds), 0);
            if (dy > 0)
                dy = Math.Max((float)(dy - SWAP_SPEED * gameTime.ElapsedGameTime.TotalMilliseconds), 0);
            else if (dy < 0)
                dy = Math.Min((float)(dy + SWAP_SPEED * gameTime.ElapsedGameTime.TotalMilliseconds), 0);
        }

        public bool IsMoving()
        {
            return dx != 0 || dy != 0;
        }

        public float Dx { get { return dx; } }

        public float Dy { get { return dy; } }

        public Color1 Color { get { return color; } }

        public Texture2D Texture { get { return texture; } }
    }
}
