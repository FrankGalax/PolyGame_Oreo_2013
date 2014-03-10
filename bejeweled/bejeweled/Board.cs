using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;

namespace bejeweled
{
    public class Board
    {
        private static Random randomNumber = new Random();

        // Nombre de tuiles en X
        private int sizeX;

        // Nombre de tuiles en Y
        private int sizeY;

        // Matrice des jewels
        private Jewel[][] jewels;

        // Position du jewel sélectionné
        private Position selected;

        private Texture2D selectTexture;

        private GameState currentState;

        // Score 
        private Score score;
        private int combo;

        private ParticulesManager particulesManager;

        //Timer
        private Timer timer;

        /// <summary>
        /// Constructeur du board. Remplit le board de jewels aux couleurs aléatoires
        /// </summary>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        public Board(int newSizeX, int newSizeY)
        {
            sizeX = newSizeX;
            sizeY = newSizeY;
            score = new Score();
            timer = new Timer(120);
            
            jewels = new Jewel[sizeX][];
            for (int i = 0; i < sizeX; i++)
            {
                jewels[i] = new Jewel[sizeY];
                for (int j = 0; j < sizeY; j++)
                {
                    int colorsLength = Enum.GetNames(typeof(Color1)).Length;
                    int r = randomNumber.Next(colorsLength);
                    Color1 color = (Color1)r;
                    if (i >= 2)
                    {
                        while (color == jewels[i - 2][j].Color && color == jewels[i - 1][j].Color)
                        {
                            color = (Color1)(++r % colorsLength);
                        }
                        if (j >= 2)
                        {
                            while (color == jewels[i][j - 2].Color && color == jewels[i][j - 1].Color)
                            {
                                color = (Color1)(++r % colorsLength);
                            }
                        }
                    }
                    else if (j >= 2)
                    {
                        while (color == jewels[i][j - 2].Color && color == jewels[i][j - 1].Color)
                        {
                            color = (Color1)(++r % colorsLength);
                        }
                        if (i >= 2)
                        {
                            while (color == jewels[i - 2][j].Color && color == jewels[i - 1][j].Color)
                            {
                                color = (Color1)(++r % colorsLength);
                            }
                        }
                    }
                    jewels[i][j] = new Jewel(color);
                }

            }

            selected = new Position(-1, -1);
            this.selectTexture = RessourceManager.Instance.GetTexture("select");

            // Vérification si le board a générer des explosions possibles aléatoirement
            if (!CheckExplosion()) {
                if (!PossibleNextMovement())
                    Console.WriteLine("No possible next movement");
                currentState = new PlayingState(this);
            }

            this.particulesManager = new ParticulesManager();
        }

        // Classe de base de la machine à état
        private abstract class GameState
        {
            protected Board board;

            public GameState(Board board)
            {
                this.board = board;
            }

            public abstract void Update(GameTime gameTime);

            public abstract void Draw(SpriteBatch spriteBatch);
        }

        // État principal du jeu
        //   Aucuns paramètres
        // Le joueur peut cliquer sur des jewels et les swapper
        // Transitions :
        //   -Si le joueur swap un jewel, le prochain état sera SwappingState
        private class PlayingState : GameState
        {
            private MouseState mouseState;
            private MouseState lastMouseState;

            // Constructeur, garde une référence vers le board, initialise le mouseState
            public PlayingState(Board board)
                : base(board)
            {
                mouseState = Mouse.GetState();
            }

            // Logique générale de l'état
            public override void Update(GameTime gameTime)
            {
                // Remet le multiplicateur de combo à 1
                board.combo = 0;

                // Débouncer pour la souris
                lastMouseState = mouseState;
                mouseState = Mouse.GetState();

                // Si l'utilisateur a cliqué
                if (lastMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed)
                {
                    int x = mouseState.X;
                    int y = mouseState.Y;

                    // Si l'utilisateur a cliqué dans la grille
                    if (x >= 0 && x < board.sizeX * Jewel.SIZE && y >= 0 && y < board.sizeY * Jewel.SIZE)
                    {
                        // On récupère les coordonnées de la grille ou on a cliqué
                        Position jewelPosition = new Position(x / Jewel.SIZE, y / Jewel.SIZE);

                        if (!board.IsSelected())//si aucun jewel n'etait selectionne, on stocke la selection
                            board.selected = jewelPosition;
                        else
                        {
                            // Si on reclick sur le même jewel, on déselectionne
                            if (jewelPosition.X == board.selected.X && jewelPosition.Y == board.selected.Y)
                                board.Unselect();
                            //si le jewel est voisin au jewel selectionné, on les swap sinon on les desélectionne
                            else if (Math.Abs(jewelPosition.X - board.selected.X) + Math.Abs(jewelPosition.Y - board.selected.Y) == 1)
                            {
                                // On swap les jewels
                                board.Swap(board.selected, jewelPosition);

                                // On swap les références de sélection
                                Position temp = board.selected;
                                board.selected = jewelPosition;
                                jewelPosition = temp;

                                // Son pour le swap
                                RessourceManager.Instance.GetSound("SwapingJewel01").Play();

                                // Transition vers l'état SwappingState
                                board.currentState = new SwappingState(board, new Position(board.selected.X, board.selected.Y), jewelPosition);
                                board.Unselect();
                            }
                            // Sinon, on sélectionne le nouveau jewel
                            else
                            {
                                board.selected = jewelPosition;
                            }
                        }
                    }
                }
            }

            public override void Draw(SpriteBatch spriteBatch)
            {
                // On déssine le sélecteur si quelque chose est sélectionné
                if (board.IsSelected())
                    spriteBatch.Draw(board.selectTexture, new Vector2(board.selected.X * Jewel.SIZE, board.selected.Y * Jewel.SIZE), Color.White);
            }

        }

        // État du board pendant un Swap
        //   firstPosition : La position du premier jewel à swapper (le jewel sélectionné en premier)
        //   secondPosition : La position du deuxième jewel à swapper
        // Les inputs sont désactivés
        // Transitions : 
        //   Lorsque l'animation est terminé,
        //     Si une explosion est possible, on passe à l'état ExplodingState
        //     Sinon, on passe à l'état UnSwappingState
        private class SwappingState : GameState
        {
            private Position firstPosition;
            private Position secondPosition;
            private Jewel firstJewel;
            private Jewel secondJewel;

            public SwappingState(Board board, Position firstPosition, Position secondPosition)
                : base(board)
            {
                this.firstPosition = firstPosition;
                this.secondPosition = secondPosition;
                firstJewel = board.jewels[firstPosition.X][firstPosition.Y];
                secondJewel = board.jewels[secondPosition.X][secondPosition.Y];
            }

            public override void Update(GameTime gameTime)
            {
                // Animation du jewel pendant le swap
                if (firstJewel.IsMoving() || secondJewel.IsMoving())
                {
                    // Update modifie le dx et dy petit à petit jusqu'à temps qu'ils tombent à zéro
                    firstJewel.Update(gameTime);
                    secondJewel.Update(gameTime);
                }
                // Fin de l'animation
                else
                {
                    // Si aucune explosion ext possible après l'animation
                    if (!board.PossibleExplosionAfterSwap(firstPosition) &&
                        !board.PossibleExplosionAfterSwap(secondPosition))
                    {
                        // On reswap les jewels pour qu'ils retournent à leur place
                        board.Swap(firstPosition, secondPosition);

                        // son
                        RessourceManager.Instance.GetSound("SwapingJewel02").Play(); 

                        // Transition vers l'état UnSwappingState
                        board.currentState = new UnSwappingState(board, firstPosition, secondPosition);
                    }
					// l'explosion est possible
                    else
                    {
                        List<Position> explodingPositions = new List<Position>();
                        explodingPositions.AddRange(board.getAllExplodingPosition(firstPosition));
                        explodingPositions.AddRange(board.getAllExplodingPosition(secondPosition));

                        board.currentState = new ExplodingState(board, explodingPositions);
                    }
                }
            }

            // Rien de spécial à déssiner pour cet état
            public override void Draw(SpriteBatch spriteBatch)
            {
            }

        }

        // État du board pendant la remise à sa place d'un swap qui n'a pas causé d'explosion
        //   firstPosition : position du premier jewel
        //   secondPosition : position du deuxième jewel
        // Transitions: 
        //   Après l'animation, retourne à PlayingState
        private class UnSwappingState : GameState
        {
            private Jewel firstJewel;
            private Jewel secondJewel;
            
            public UnSwappingState(Board board, Position firstPosition, Position secondPosition)
                : base(board)
            {
                firstJewel = board.jewels[firstPosition.X][firstPosition.Y];
                secondJewel = board.jewels[secondPosition.X][secondPosition.Y];

                // On doit remettre le sélecteur à la position du jewel d'orgine
                board.selected = secondPosition;
            }

            public override void Update(GameTime gameTime)
            {
                
                // Animation
                if (firstJewel.IsMoving() || secondJewel.IsMoving())
                {
                    firstJewel.Update(gameTime);
                    secondJewel.Update(gameTime);
                }
                // Fin de l'animation
                else
                {
                    // Retour à PlayingState
                    board.currentState = new PlayingState(board);
                }
            }

            // Rien de spécial à déssiner pour cet état
            public override void Draw(SpriteBatch spriteBatch)
            {
            }

        }

        // État du board pendant l'explosion de jewels
        //   explodingPositions : positions à faire exploser
        // Transitions: 
        //   Après l'animation, on passe à FallingState
        private class ExplodingState : GameState
        {
            private List<Position> explodingPositions;      // Liste positions à faire exploser

            public ExplodingState(Board board, List<Position> explodingPositions)
                : base(board)
            {
                this.explodingPositions = explodingPositions;
            }

            public override void Update(GameTime gameTime)
            {
                // Son de l'explosion
                RessourceManager.Instance.GetSound("jewelExplosion01").Play(); 

                // Explosion des positions
                foreach (Position position in explodingPositions)
                {
                    board.particulesManager.addParticules(board.jewels[position.X][position.Y].Color, position);

                    board.Delete(position);
                }

                /* Construction des positions à faire "faller"
                 * Il ne peut y avoir qu'une seule position à faire "faller" par colonne et la position qui est dans une colonne
                 *   doit avoir le Y maximal.
                 * Exemple : Si les positions à faire exploser sont
                 * --------
                 * --------
                 * --------
                 * -XXX----
                 * --XXX---
                 * --------
                 * Les positions à faire faller seront
                 * --------
                 * --------
                 * --------
                 * -X------
                 * --XXX---
                 * --------
                 * On enlève les position qui sont en double dans les colonnes mais on garde les plus basses.
                 */
                List<Position> fallingPositions = new List<Position>();

                foreach (Position explodingPosition in explodingPositions) 
                {
                    // Recherche d'une position déjà ajouté qui est dans la même colonne
                    // position => position.X == explodingPosition.X est un foncteur en C# (comme les foncteurs STL en C++)
                    // ça veut dire trouve la première position dans la liste qui satisfait la condition position.X == explodingPosition.X
                    Position positionAlreadyInColumn = fallingPositions.Find(position => position.X == explodingPosition.X);

                    // S'il n'y a pas de position dans cette colonne, on l'ajoute
                    if (positionAlreadyInColumn == null)
                    {
                        fallingPositions.Add(explodingPosition);
                    }
                    // Sinon, on la remplace si son Y est plus grand que celui déjà dans la colonne
                    else if (positionAlreadyInColumn.Y < explodingPosition.Y)
                    {
                        fallingPositions.Remove(positionAlreadyInColumn);
                        fallingPositions.Add(explodingPosition);
                    }
                }

                // Gestion du score
                int nbExplodingJewels = explodingPositions.Count;

                // Test pour différents score, on devra faire plus de conditions pour des scores différents
                int scorePerJewel = 10;
                if (nbExplodingJewels > 6)
                    scorePerJewel = 50;
                else if (nbExplodingJewels > 5)
                    scorePerJewel = 40;
                else if (nbExplodingJewels > 4)
                    scorePerJewel = 30;
                else if (nbExplodingJewels > 3)
                    scorePerJewel = 20;

                // Multiplie le ScorePerJewel par le nombre de combo
                board.combo++;
                scorePerJewel *= board.combo;

                foreach (Position explodingPosition in explodingPositions)
                {
                    board.score.ScoreTotal += scorePerJewel;
                    board.score.AddScorePopup(scorePerJewel, explodingPosition);
                }

                // Changement d'état
                board.currentState = new FallingState(board, fallingPositions);
            }

            // Rien de spécial à déssiner pour cet état
            public override void Draw(SpriteBatch spriteBatch)
            {
            }

        }

        // État du board pendant le fall des jewels
        //   fallingPositions : positions à faire "faller"
        // Transitions: 
        //   Après l'animation, 
        //     Si une explosion est possible dans une des positions affectés, on passe à ExplodingState,
        //     Sinon, on retourne à PlayingState
        private class FallingState : GameState
        {
            private List<Position> fallingPositions;    // positions à faire faller
            private bool newJewelsCreated;              // bool qui vérifie si les nouveaux jewels sont crées et que l'animation peut commencer
            private List<Jewel> movingJewels;           // jewels à animer

            public FallingState(Board board, List<Position> fallingPositions)
                : base(board)
            {
                this.fallingPositions = fallingPositions;
                this.newJewelsCreated = false;
                this.movingJewels = new List<Jewel>();
            }

            public override void Update(GameTime gameTime)
            {
                // Création et mise à jour des jewels
                if (!newJewelsCreated)
                {
                    // Pour chaque colonne
                    foreach (Position fallingPosition in fallingPositions)
                    {
                        int column = fallingPosition.X;

                        // Compte du nombre de position vide dans la colonne. Exemple,
                        // une explosion standard vertical de 3 jewel cause un "trou" de 3.
                        // nbNullJewelsInColumn vaudra alors 3
                        int nbNullJewelsInColumn = 0;
                        for (int i = fallingPosition.Y; i >= 0; i--)
                        {
                            if (board.jewels[column][i] == null)
                                nbNullJewelsInColumn++;
                            else
                                break;
                        }

                        // Déplacement des jewels EXISTANTS au dessus de la position à faller du nombre de trous vers le bas
                        for (int i = 0; i < fallingPosition.Y - nbNullJewelsInColumn + 1; ++i)
                        {
                            // Déplacement des jewels
                            board.jewels[column][fallingPosition.Y - i] = board.jewels[column][fallingPosition.Y - i - nbNullJewelsInColumn];

                            // Initialisation de l'animation
                            board.jewels[column][fallingPosition.Y - i].MoveDown(nbNullJewelsInColumn);

                            // Ajout du jewel aux jewels à animer
                            movingJewels.Add(board.jewels[column][fallingPosition.Y - i]);

                            // Suppression du jewel à l'ancienne position
                            board.Delete(new Position(column, fallingPosition.Y - i - nbNullJewelsInColumn));
                        }

                        // Création de nouveaux jewels dans les trous qui sont maintenant rendus en haut de colonne complétement
                        for (int i = 0; i < nbNullJewelsInColumn; ++i)
                        {
                            // Création du jewel
                            board.jewels[column][i] = new Jewel();

                            // Initialisation de l'animation
                            board.jewels[column][i].MoveDown(nbNullJewelsInColumn);

                            // Ajout du nouveau jewel aux jewels à animer
                            movingJewels.Add(board.jewels[column][i]);
                        }
                    }
                    // Initialisation et création complétée
                    newJewelsCreated = true;
                }
                // Animation
                else
                {
                    // Recherche d'au moins un jewel qui bouge
                    Jewel firstMovingJewel = movingJewels.Find(jewel => jewel.IsMoving());

                    // Si au moins un jewel bouge
                    if (firstMovingJewel != null)
                    {
                        // Déplacement des jewels
                        foreach (Jewel jewel in movingJewels)
                            jewel.Update(gameTime);
                    }
                    // L'animation est terminé
                    else
                    {
                        // Recherche d'explosions causées par le Fall
                        // S'il n'y en a pas, on retourne au PlayingState
                        // S'il y en a, la méthode CheckExplosion fera une transition vers ExplodingState
                        if (!board.CheckExplosion())
                        {
                            if (!board.PossibleNextMovement())
                                Console.WriteLine("No possible next movement");
                            board.currentState = new PlayingState(board);
                        }
                    }
                }
            }

            // Rien de spécial à déssiner pour cet état
            public override void Draw(SpriteBatch spriteBatch)
            {
            }

        }
        /// <summary>
        /// Logique générale du jeu
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Dispatch de la logique à la machine à état
            currentState.Update(gameTime);

            particulesManager.Update(gameTime);
            score.Update(gameTime);

            timer.Update(gameTime);
            if (timer.IsDone())
            {
                Console.WriteLine("Game over, no more time left");
            }
        }

        /// <summary>
        /// Dessine le board à l'écran
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    Jewel jewel = jewels[i][j]; 
                    if (jewel != null)
                        spriteBatch.Draw(jewel.Texture, new Vector2((i+jewel.Dx) * Jewel.SIZE, (j+jewel.Dy) * Jewel.SIZE), Color.White);
                }
            }

            // Dessins particuliers de l'état courant
            currentState.Draw(spriteBatch);

            particulesManager.Draw(spriteBatch);

            score.Draw(spriteBatch, new Vector2(sizeX * Jewel.SIZE, 0));
            timer.Draw(spriteBatch, new Vector2 (sizeX * Jewel.SIZE, 50));
        }

        
        ///
        /// <summary>
        /// Sélectionne un jewel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void Select(int x, int y)
        {
            selected.X = x;
            selected.Y = y;
        }

        /// <summary>
        /// Désélectionner
        /// </summary>
        private void Unselect()
        {
            selected.X = -1;
            selected.Y = -1;
        }

        /// <summary>
        /// Vérifier si un jewel est sélectionné
        /// </summary>
        /// <returns></returns>
        private bool IsSelected()
        {
            return selected.X != -1 && selected.Y != -1;
        }

        /// <summary>
        /// Échange les jewels aux deux positions indiquées
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void Swap(Position firstJewelPosition , Position secondJewelPosition)
        {
            Jewel first = jewels[firstJewelPosition.X][firstJewelPosition.Y];
            Jewel second = jewels[secondJewelPosition.X][secondJewelPosition.Y];

            if (firstJewelPosition.Y != secondJewelPosition.Y && firstJewelPosition.X != secondJewelPosition.X)
                return;

            if (firstJewelPosition.Y == secondJewelPosition.Y)
            {
                if (firstJewelPosition.X > secondJewelPosition.X)
                {
                    first.MoveLeft();
                    second.MoveRight();
                }
                else if (firstJewelPosition.X < secondJewelPosition.X)
                {
                    first.MoveRight();
                    second.MoveLeft();
                }
            }
            else if (firstJewelPosition.X == secondJewelPosition.X)
            {
                if (firstJewelPosition.Y > secondJewelPosition.Y)
                {
                    first.MoveUp();
                    second.MoveDown();
                }
                else if (firstJewelPosition.Y < secondJewelPosition.Y)
                {
                    first.MoveDown();
                    second.MoveUp();
                }
            }

            Jewel temp = jewels[secondJewelPosition.X][secondJewelPosition.Y];
            jewels[secondJewelPosition.X][secondJewelPosition.Y] = jewels[firstJewelPosition.X][firstJewelPosition.Y];
            jewels[firstJewelPosition.X][firstJewelPosition.Y] = temp;
        }


        /// <summary>
        /// Vérifie s'il y a une explosion possible sur le Board, Si oui, l'état change pour ExplodingState
        /// </summary>
        /// <returns>true s'il y a une explosion possible, false sinon</returns>
        private bool CheckExplosion()
        {
            for (int i = 0; i < sizeX; ++i)
            {
                for (int j = 0; j < sizeY; ++j)
                {
                    Position position = new Position(i, j);
                    if (PossibleExplosionAfterSwap(position))
                    {
                        currentState = new ExplodingState(this, getAllExplodingPosition(position));
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Vérifie s'il y a des explosions possible avec un jewel après qu'il ait bougé
        /// vrai si il a au moins deux voisins de meme couleur soit horizontalement ou verticalement
        /// </summary>
        private bool PossibleExplosionAfterSwap(Position position)
        {
            return PossibleHorizontalExplosion(position) || PossibleVerticalExplosion(position);
        }

        /// <summary>
        /// Construit une liste de tous les jewels qui doivent exploser à partir d'une position
        /// </summary>
        /// <param name="position">position à vérifier</param>
        /// <returns>La liste des jewels à faire exploser</returns>
        private List<Position> getAllExplodingPosition(Position position)
        {
            List<Position> explodingPositions = new List<Position>();

            int x = position.X;
            int y = position.Y;

            // Si une explosion horizontale est possible
            if (PossibleHorizontalExplosion(position))
            {
                // Ajout des jewels voisins à l'horizontal
                for (int i = 1; i <= NbSameColorJewelsLeft(position); ++i)
                    explodingPositions.Add(new Position(x - i, y));
                for (int i = 1; i <= NbSameColorJewelsRight(position); ++i)
                    explodingPositions.Add(new Position(x + i, y));
            }
            // Si une explosion verticale est possible
            if (PossibleVerticalExplosion(position))
            {
                // Ajout des jewels voisins à la verticale
                for (int i = 1; i <= NbSameColorJewelsUp(position); ++i)
                    explodingPositions.Add(new Position(x, y - i));
                for (int i = 1; i <= NbSameColorJewelsDown(position); ++i)
                    explodingPositions.Add(new Position(x, y + i));
            }

            // Si la liste n'est pas vide, ça veut dire qu'une explosion est possible, donc il
            // faut ajout la position elle-même
            if (explodingPositions.Count > 0)
                explodingPositions.Add(position);

            return explodingPositions;
        }

        /// <summary>
        /// Vérifie s'il y a des explosions possible avec un jewel horizontalement
        /// </summary>
        private bool PossibleHorizontalExplosion(Position position)
        {
            return NbSameColorJewelsRight(position) + NbSameColorJewelsLeft(position) >= 2;
        }

        /// <summary>
        /// Vérifie s'il y a des explosions possible avec un jewel verticalement
        /// </summary>
        private bool PossibleVerticalExplosion(Position position)
        {
            return NbSameColorJewelsDown(position) + NbSameColorJewelsUp(position) >= 2;
        }

        /// <summary>
        /// Retourne le nombre de Jewels de meme couleur au jewel selectionne à sa droite
        /// </summary>
        private int NbSameColorJewelsRight(Position position)
        {
            Color1 currentColor = jewels[position.X][position.Y].Color;
            int nbSameColorJewel = 0;

            //on compare les voisins de droite du jewel selectionné
            for (int i = 1; position.X + i < sizeX; i++)
                if (currentColor == jewels[position.X + i][position.Y].Color)
                    ++nbSameColorJewel;
                else
                    break;
            return nbSameColorJewel;
 
        }

        /// <summary>
        /// Retourne le nombre de Jewels de meme couleur au jewel selectionne à sa gauche
        /// </summary>
        private int NbSameColorJewelsLeft(Position position)
        {
            Color1 currentColor = jewels[position.X][position.Y].Color;
            int nbSameColorJewel = 0;

            //on compare les voisins de gauche du jewel selectionné
            for (int i = 1; position.X - i >= 0; i++)
                if (currentColor == jewels[position.X - i][position.Y].Color)
                    ++nbSameColorJewel;
                else
                    break;
            return nbSameColorJewel;

        }
        /// <summary>
        /// Retourne le nombre de Jewels de meme couleur au jewel selectionne au dessus de lui
        /// </summary>
        private int NbSameColorJewelsDown(Position position)
        {
            Color1 currentColor = jewels[position.X][position.Y].Color;
            int nbSameColorJewel = 0;
            //on compare les voisins du dessus du jewel selectionné
            for (int j = 1; position.Y + j < sizeY; j++)
                if (currentColor == jewels[position.X][position.Y + j].Color)
                    ++nbSameColorJewel;
                else
                    break;
            return nbSameColorJewel;
        }

        /// <summary>
        /// Retourne le nombre de Jewels de meme couleur au jewel selectionne au dessous de lui
        /// </summary>
        private int NbSameColorJewelsUp(Position position)
        {
            Color1 currentColor = jewels[position.X][position.Y].Color;
            int nbSameColorJewel = 0;
            //on compare les voisins du dessous du jewel selectionné
            for (int j = 1; position.Y - j >= 0; j++)
                if (currentColor == jewels[position.X][position.Y - j].Color)
                    ++nbSameColorJewel;
                else
                    break;
            return nbSameColorJewel;
        }
                
        /// <summary>
        /// Supprime un jewel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void Delete(Position position)
        {
            jewels[position.X][position.Y] = null;
        }

        /// <summary>
        /// Vérifie s'il y a un mouvement possible dans le board
        /// </summary>
        /// <returns></returns>
        private bool PossibleNextMovement()
        {
            bool movementPossible = false;
            for (int i = 0; i < sizeX; ++i)
            {
                for (int j = 0; j < sizeY; ++j)
                {
                    if (i != sizeX - 1)
                    {
                        Jewel tmp = jewels[i][j];
                        jewels[i][j] = jewels[i + 1][j];
                        jewels[i + 1][j] = tmp;


                        movementPossible = (PossibleExplosionAfterSwap(new Position(i, j)) || PossibleExplosionAfterSwap(new Position(i + 1, j)));

                        tmp = jewels[i][j];
                        jewels[i][j] = jewels[i + 1][j];
                        jewels[i + 1][j] = tmp;

                        if (movementPossible)
                            return true;
                    }

                    if (j != sizeY - 1)
                    {
                        Jewel tmp = jewels[i][j];
                        jewels[i][j] = jewels[i][j + 1];
                        jewels[i][j + 1] = tmp;

                        movementPossible = (PossibleExplosionAfterSwap(new Position(i, j)) || PossibleExplosionAfterSwap(new Position(i, j + 1)));

                        tmp = jewels[i][j];
                        jewels[i][j] = jewels[i][j + 1];
                        jewels[i][j + 1] = tmp;

                        if (movementPossible)
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Génère un nouveau board quand il n'y a plus de mouvements possibles, mais fait perdre des points au joueur
        /// </summary>
        private void Refresh()
        {
        }
    }
}
