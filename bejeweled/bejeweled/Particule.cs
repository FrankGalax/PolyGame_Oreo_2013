using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace bejeweled
{
    public class Particule
    {
        // Générateur random
        private static Random random = new Random();

        // Paramètres des particules
        private static float MIN_SPEED = 0.05f;
        private static float MAX_SPEED = 0.20f;
        private static float MAX_ROTATION = 0.5f;
        private static float MIN_RANGE = 100f;
        private static float MAX_RANGE = 180f;

        // Texture et coordonnées sur le spriteSheet
        private Texture2D texture;
        private int textureOffsetX;
        private int textureOffsetY;
        private int sizeX;
        private int sizeY;

        // Attributs de la particule courant
        private Vector2 startPosition;
        private Vector2 position;
        private float speed;
        private Vector2 direction;
        private float rotation;
        private float angle;
        private float range;
        private bool clockWiseRotation;

        // Vérifie si on doit détruire la particule
        private bool outOfRange;

        public Particule(Texture2D texture, int textureOffsetX, int textureOffsetY, int sizeX, int sizeY, Vector2 startPosition)
        {
            this.texture = texture;
            this.textureOffsetX = textureOffsetX;
            this.textureOffsetY = textureOffsetY;
            this.sizeX = sizeX;
            this.sizeY = sizeY;

            this.startPosition = startPosition;
            this.position = startPosition;

            // Vitesse aléatoire entre les deux paramètre spécifié
            this.speed = (float)(random.NextDouble() * (MAX_SPEED - MIN_SPEED) + MIN_SPEED);

            // Direction aléatoire sur 360 degré
            double angle = random.Next(360) / 360f * 2 * Math.PI;
            this.direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            // Rotation aléatoire entre 0 et le paramètre spécifié
            this.rotation = (float)(random.NextDouble()*MAX_ROTATION / 360f * 2 * Math.PI);
            this.angle = 0;

            // Range aléatoire entre les paramètres spécifiés
            this.range = (float)(random.NextDouble() * (MAX_RANGE - MIN_RANGE) + MIN_RANGE);

            // Sens de la rotation 
            this.clockWiseRotation = random.Next(2) == 1;

            this.outOfRange = false;
        }

        public void Update(GameTime gameTime)
        {
            // Mise à jour de la position de la particule
            position = position + speed * direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Mise à jour de l'angle
            if (clockWiseRotation)
            {
                angle += rotation * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (angle >= 2 * Math.PI)
                    angle -= (float)(2 * Math.PI);
            }
            else
            {
                angle -= rotation * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (angle < 0)
                    angle += (float)(2 * Math.PI);
            }

            // Vérifier si la particule est outOfRange
            if ((position - startPosition).Length() > range)
                outOfRange = true;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(texture,                                                 // texture
                new Rectangle((int)position.X, (int)position.Y, sizeX, sizeY),  // Position à dessiner
                new Rectangle(textureOffsetX, textureOffsetY, sizeX, sizeY),    // Rectangle pour "découper" la partie de la texture à dessiner
                Color.White,                                                    // Background, invisible de toute façon
                angle,                                                          // Angle de rotation pendant le dessin
                new Vector2(sizeX / 2, sizeY / 2),                              // origine de la rotation, centre de la particule
                SpriteEffects.None,                                             // Aucun effet
                0f                                                              // Aucun scale
            );
        }

        public bool OutOfRange
        {
            get { return outOfRange; }
        }
    }
}
