using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bejeweled
{
    public class Score
    {
        public int ScoreTotal { get; set; }
        private List<ScorePopup> scorePopups;

        public Score()
        {
            ScoreTotal = 0;
            this.scorePopups = new List<ScorePopup>();
        }

        public void AddScorePopup(int score, Position position)
        {
            scorePopups.Add(new ScorePopup(score,
                new Position(position.X * Jewel.SIZE + 16 , position.Y * Jewel.SIZE + 16)
            ));
        }

        public void Update(GameTime gameTime)
        {
            foreach (ScorePopup scorePopup in scorePopups)
                scorePopup.Update(gameTime);

            scorePopups.RemoveAll(scorePopup => scorePopup.OutOfRange);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.DrawString(
                RessourceManager.Instance.GetFont("default"), 
                "Score: " + ScoreTotal, 
                position, 
                Color.Black
            );

            foreach (ScorePopup scorePopup in scorePopups)
                scorePopup.Draw(spriteBatch);
        }
    }
}
