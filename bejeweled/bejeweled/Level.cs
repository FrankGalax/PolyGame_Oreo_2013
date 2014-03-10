using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bejeweled
{
    public class Level
    {
        private int maxTimeBetweenMoves;
        private int pointsNeededToCompleteLevel;

        /// <summary>
        /// Génère le prochain niveau à partir des attributs du niveau courant;
        /// </summary>
        /// <returns></returns>
        public Level NextLevel()
        {
            return null;
        }

        public int MaxTimeBetweenMoves { get { return maxTimeBetweenMoves; } }

        public int PointsNeededToCompleteLevel { get { return pointsNeededToCompleteLevel; } }
    }
}
