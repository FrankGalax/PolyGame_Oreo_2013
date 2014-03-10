using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bejeweled
{
    public class Timer
    {
        // Temps total que le timer va compter
        private double totalSecs;

        // Compteur
        private double currentSecs;

        private Color color;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="totalMillis"></param>
        public Timer(double totalSecs)
        {
            this.totalSecs = totalSecs;
            this.currentSecs = 0;
        }

        /// <summary>
        /// Mise à jour du compteur
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (IsDone()) 
                return;
            currentSecs += gameTime.ElapsedGameTime.TotalSeconds;

            if (currentSecs > totalSecs * 0.75)
                color = Color.Red;
            else if (currentSecs > totalSecs * 0.50)
                color = Color.Orange;
            else
                color = Color.Black;
        }

        /// <summary>
        /// Vérifie si le temps total est atteint
        /// </summary>
        /// <returns></returns>
        public bool IsDone()
        {
            return currentSecs >= totalSecs;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            double remainingSecs = totalSecs - currentSecs;
            int minutes = (int)(remainingSecs / 60);
            int secondes = (int)(remainingSecs % 60);
            String timerString = "Time: " + minutes + ":" + (secondes < 10 ? "0": "") + secondes;
            spriteBatch.DrawString(
                RessourceManager.Instance.GetFont("default"), 
                timerString, 
                position, 
                color
            );
        }
        /// <summary>
        /// Réinitialize le timer
        /// </summary>
        public void Reset()
        {
        }
    }
}
