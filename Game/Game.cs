using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuneWarz.Game
{
    class Game
    {
        public bool GameIsOver { get; private set; }

        GamePanel Graphics;
        Board Board;
        bool PlayerIsAllowedToMove;
        FileSystemWatcher Watcher;

        public Game(GamePanel Graphics)
        {
            this.Graphics = Graphics;
            this.Board = null;
            this.PlayerIsAllowedToMove = false;
            this.GameIsOver = true;
        }

        public void NewGame()
        {
            this.Board = new Board();
            WatchSaveFile();
            this.PlayerIsAllowedToMove = true;
            this.GameIsOver = false;
        }
        public void ResumeGame()
        {
            if (this.Board != null)
                return;
            this.Board = Board.LoadFromState();
            WatchSaveFile();
            this.PlayerIsAllowedToMove = true;
            this.GameIsOver = false;
        }

        public int HumanPlayer() { return Player.PLAYER_HUMAN;  }
        public int BoardWidth() { return this.Board == null ? 0 : this.Board.Width; }
        public int BoardHeight() { return this.Board == null ? 0 : this.Board.Height; }

        public bool AnyPlayerHasColor(int Color) {
            return Board.Players.Any(Player => Player != null && Player.Color == Color);
        }
        public int GetNumPlayers()
        {
            return Board == null ? 0 : Board.NumPlayers;
        }
        public int GetNumTilesOfPlayer(int Player)
        {
            return this.Board == null ? -1 : this.Board.Players[Player].NumTiles;
        }

        public int GetColorOfPlayer(int Player)
        {
            return this.Board == null ? -1 : this.Board.Players[Player].Color;
        }
        public int GetColorOfTile(int x, int y)
        {
            Tile tile = this.Board.GetTile(x, y);
            return tile == null ? -1 : tile.Color;
        }
        public int GetOwnerOfTile(int x, int y)
        {
            Tile tile = this.Board.GetTile(x, y);
            return tile == null ? -1 : tile.Owner;
        }

        public List<Tuple<int,int>> GetCapturableTiles(int Player, int Color)
        {
            return this.Board.GetCapturableTiles(Player, Color);
        }

        /// <summary>
        /// Take a turn by attempting to capture a color
        /// </summary>
        /// <param name="Color">Color to capture</param>
        /// <returns>True if turn was not aborted</returns>
        public bool TakeTurn(int Color)
        {
            if (!this.PlayerIsAllowedToMove || AnyPlayerHasColor(Color))
                return false;
            this.PlayerIsAllowedToMove = false;

            // Player Turn
            if (!this.Board.TryToCapture(HumanPlayer(), Color) && !PlayerIsStuck())
            {
                this.PlayerIsAllowedToMove = true;
                return false;
            }

            // AI Take Turn
            do
            {
                bool AnyAIHasMoved = TakeTurn_AI();
                if (!AnyAIHasMoved && PlayerIsStuck())
                {
                    this.GameIsOver = true;
                    return true;
                }
            } while (PlayerIsStuck());

            this.Board.Sync(true);
            this.PlayerIsAllowedToMove = true;
            return true;
        }
        bool TakeTurn_AI()
        {
            bool AnyAIHasMoved = false;
            for (int Player = HumanPlayer() + 1; Player < Board.NumPlayers; ++Player)
            {
                int BestColor = 0;
                List<Tuple<int, int>> BestList = null;
                for (int Color = 0; Color < Tile.NUM_COLORS; ++Color)
                {
                    if (AnyPlayerHasColor(Color))
                        continue;

                    List<Tuple<int, int>> List = GetCapturableTiles(Player, Color);
                    if (BestList == null || BestList.Count < List.Count)
                    {
                        BestList = List;
                        BestColor = Color;
                    }
                }
                if (BestList.Count > 0)
                {
                    this.Board.TryToCapture(Player, BestColor);
                    AnyAIHasMoved = true;
                }
            }
            return AnyAIHasMoved;
        }

        bool PlayerIsStuck()
        {
            for (int Color = 0; Color < Tile.NUM_COLORS; ++Color)
                if (AnyPlayerHasColor(Color))
                    continue;
                else if (GetCapturableTiles(HumanPlayer(), Color).Count > 0)
                    return false;
            return true;
        }

        void WatchSaveFile()
        {
            if (this.Watcher != null)
                return;

            this.Watcher = new FileSystemWatcher(".", Board.Filename);
            this.Watcher.NotifyFilter = NotifyFilters.LastWrite;
            this.Watcher.Changed += WatchSaveFile_HandleChange;
            this.Watcher.EnableRaisingEvents = true;
        }
        void WatchSaveFile_HandleChange(object sender, FileSystemEventArgs e)
        {
            if (this.Board.Sync(false) == SyncStatus.LOAD_OK)
                this.Graphics.Invalidate();
        }
    }
}
