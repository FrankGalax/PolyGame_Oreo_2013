using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace bejeweled
{
    public class ParticulesManager
    {
        private static int DEFAULT_PARTICULE_SIZE = 16;
        private static Random random = new Random();

        // Textures selon les couleurs, Dictionary == Map
        private Dictionary<Color1, Texture2D> colorTextures;

        // Liste de particules à dessiner
        private List<Particule> particules;

        public ParticulesManager()
        {
            this.particules = new List<Particule>();
            this.colorTextures = new Dictionary<Color1,Texture2D>();

            colorTextures.Add(Color1.BLUE, RessourceManager.Instance.GetTexture("blueJewel"));
            colorTextures.Add(Color1.GREEN, RessourceManager.Instance.GetTexture("greenJewel"));
            colorTextures.Add(Color1.ORANGE, RessourceManager.Instance.GetTexture("orangeJewel"));
            colorTextures.Add(Color1.PURPLE, RessourceManager.Instance.GetTexture("purpleJewel"));
            colorTextures.Add(Color1.RED, RessourceManager.Instance.GetTexture("redJewel"));
            colorTextures.Add(Color1.YELLOW, RessourceManager.Instance.GetTexture("yellowJewel"));
        }

        public void addParticules(Color1 color, Position position) 
        {
            // Nombre de particules à afficher entre 1 et 4
            int nb = random.Next(10) + 1;

            for (int i = 0; i < nb; ++i)
                particules.Add(new Particule(colorTextures[color],                  // Couleur de la particule
                    random.Next(2) * DEFAULT_PARTICULE_SIZE,                        // Coordonnée x pour le découpage de la texture
                    random.Next(2) * DEFAULT_PARTICULE_SIZE,                        // Coordonnée y pour le découpage de la texture
                    DEFAULT_PARTICULE_SIZE,                                         // sizeX de la texture à découper
                    DEFAULT_PARTICULE_SIZE,                                         // sizeY de la texture à découper
                    new Vector2(position.X * Jewel.SIZE, position.Y * Jewel.SIZE))  // Position initialiale de la particule
                );
        }

        public void Update(GameTime gameTime) 
        {
            foreach (Particule particule in particules) 
                particule.Update(gameTime);

            particules.RemoveAll(particule => particule.OutOfRange);
        }

        public void Draw(SpriteBatch batch)
        {
            foreach (Particule particule in particules)
                particule.Draw(batch);
        }
    }
}
