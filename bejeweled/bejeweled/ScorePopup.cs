using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bejeweled
{
    public class ScorePopup
    {
        private static float SPEED = 0.05f;
        private static float RANGE = 30f;
        private static float SCALE_FACTOR = 0.015f;
        private static float SCALE_ORIGIN = 0.85f;

        private String score;
        private Vector2 position;
        private Vector2 startPosition;
        private Vector2 direction;
        private float speed;
        private float scale;
        private float range;
        private Color color;

        private bool outOfRange;

        public ScorePopup(int score, Position position)
        {
            this.score = score.ToString();
            this.position = new Vector2(position.X, position.Y);
            this.startPosition = new Vector2(position.X, position.Y);
            this.direction = -Vector2.UnitY;
            this.speed = SPEED;
            this.scale = Math.Min(SCALE_FACTOR * score + SCALE_ORIGIN, 1.4f);
            this.range = RANGE;
            this.color = Color.Black;
        }

        public void Update(GameTime gameTime)
        {
            // Mise à jour de la position de la particule
            position = position + speed * direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Vérifier si la particule est outOfRange
            if ((position - startPosition).Length() > range)
                outOfRange = true;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.DrawString(
                RessourceManager.Instance.GetFont("scorePopup"), 
                score, 
                position, 
                color, 
                0f, 
                Vector2.Zero, 
                scale, 
                SpriteEffects.None, 
                0f
            );
        }

        public bool OutOfRange
        {
            get { return outOfRange; }
        }
    }
}
