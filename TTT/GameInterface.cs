using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTT
{
    public interface GameInterface
    {
        public List<int[]> GetPossibleMovesFromState(int[] state);
        public int GetWinnerForState(int[] state);
    }
}
