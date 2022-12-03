using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTT
{
    public class Game
    {
        //public Player CurrentPlayer { get; set; } 
        const int intSize = 4;

        public enum Player : int
        {
            X = 1,
            O = 2
        };

        public enum Winner : int
        {
            X = 1,
            O = 2,
            Draw = 3
        };

        public List<int[]> GetPossibleMoves(int[] state, Player player)
        {
            List<int[]> moves = new List<int[]>();

            if ((state[0] == state[1] && state[1] == state[2] && state[0] != 0)
                || (state[3] == state[4] && state[4] == state[5] && state[3] != 0)
                || (state[6] == state[7] && state[7] == state[8] && state[6] != 0)
                || (state[0] == state[4] && state[4] == state[8] && state[0] != 0)
                || (state[2] == state[4] && state[4] == state[6] && state[2] != 0)
                || (state[0] == state[3] && state[3] == state[6] && state[0] != 0)
                || (state[1] == state[4] && state[4] == state[7] && state[1] != 0)
                || (state[2] == state[5] && state[5] == state[8] && state[2] != 0))
            {
                return moves;
            }

            for (var i = 0; i < state.Length; i++)
            {
                if (state[i] == 0)
                {
                    int[] newState = new int[9];
                    Buffer.BlockCopy(state, 0, newState, 0, 9 * intSize);
                    newState[i] = (int)player;
                    moves.Add(newState);
                }
            }

            return moves;
        }

        public Winner DetermineWinner(int[] state)
        {
            if ((state[0] == state[1] && state[1] == state[2] && state[0] == (int)Player.X)
                || (state[3] == state[4] && state[4] == state[5] && state[3] == (int)Player.X)
                || (state[6] == state[7] && state[7] == state[8] && state[6] == (int)Player.X)
                || (state[0] == state[4] && state[4] == state[8] && state[0] == (int)Player.X)
                || (state[2] == state[4] && state[4] == state[6] && state[2] == (int)Player.X)
                || (state[0] == state[3] && state[3] == state[6] && state[0] == (int)Player.X)
                || (state[1] == state[4] && state[4] == state[7] && state[1] == (int)Player.X)
                || (state[2] == state[5] && state[5] == state[8] && state[2] == (int)Player.X))
            {
                return Winner.X;
            }

            if ((state[0] == state[1] && state[1] == state[2] && state[0] == (int)Player.O)
                || (state[3] == state[4] && state[4] == state[5] && state[3] == (int)Player.O)
                || (state[6] == state[7] && state[7] == state[8] && state[6] == (int)Player.O)
                || (state[0] == state[4] && state[4] == state[8] && state[0] == (int)Player.O)
                || (state[2] == state[4] && state[4] == state[6] && state[2] == (int)Player.O)
                || (state[0] == state[3] && state[3] == state[6] && state[0] == (int)Player.O)
                || (state[1] == state[4] && state[4] == state[7] && state[1] == (int)Player.O)
                || (state[2] == state[5] && state[5] == state[8] && state[2] == (int)Player.O))
            {
                return Winner.O;
            }

            for (int i=0; i < 8; i++)
            {
                if (state[i] == 0)
                {
                    //Not draw, called on game state that has not ended
                    throw new Exception("error error");
                }
            }

            return Winner.Draw;
        }
    }
}
