using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTT
{
    public struct BagItem
    {
        public int id;
        public int weight;
        public int value;
    }
    //Simple game. Each item has a weight and a value. Maximize the value when we can carry 50kg
    public class GameMaxBag : GameInterface
    {
        public List<BagItem> PossibleItems = new List<BagItem>();
        const int MAXWEIGHT = 50;

        public GameMaxBag()
        {
            PossibleItems.Add(new BagItem() { id = 0, value = 5, weight = 10 });
            PossibleItems.Add(new BagItem() { id = 1, value = 10, weight = 20 });
            PossibleItems.Add(new BagItem() { id = 2, value = 2, weight = 10 });
            PossibleItems.Add(new BagItem() { id = 3, value = 50, weight = 50 });
            PossibleItems.Add(new BagItem() { id = 4, value = 10, weight = 20 });
            PossibleItems.Add(new BagItem() { id = 5, value = 20, weight = 15 });
            PossibleItems.Add(new BagItem() { id = 6, value = 5, weight = 5 });
            PossibleItems.Add(new BagItem() { id = 7, value = 15, weight = 35 });
            PossibleItems.Add(new BagItem() { id = 8, value = 20, weight = 10 });
            PossibleItems.Add(new BagItem() { id = 9, value = 30, weight = 25 });
            PossibleItems.Add(new BagItem() { id = 10, value = 40, weight = 40 });
            PossibleItems.Add(new BagItem() { id = 11, value = 10, weight = 25 });
            PossibleItems.Add(new BagItem() { id = 12, value = 10, weight = 5 });
            PossibleItems.Add(new BagItem() { id = 13, value = 300, weight = 55 });
            PossibleItems.Add(new BagItem() { id = 14, value = 5, weight = 20 });
            //Best = 8+9 = 50 / 35 . + 5 = 8+9+5 . sum=20+30+20, weight=10+25+15 = 50
        }

        public List<int[]> GetPossibleMovesFromState(int[] state)
        {
            List<int[]> ret = new List<int[]>();

            int weight = 0;
            List<int> addedItemsIndexes = new List<int>();
            int firstZeroIndex = -1;

            for (var i=0; i < state.Length; i++)
            {
                if (state[i] != -1)
                {
                    addedItemsIndexes.Add(state[i]);
                    weight = weight + PossibleItems.ElementAt(state[i]).weight;
                }

                if (state[i] == -1 && firstZeroIndex == -1)
                {
                    firstZeroIndex = i;
                }
            }

            //No more items can be carried
            if (firstZeroIndex == -1)
            {
                return ret;
            }

            //Return new possible state
            for (var i=0; i < PossibleItems.Count; i++) 
            {
                //We can carry the item, and we dont have it already
                if (weight + PossibleItems[i].weight <= MAXWEIGHT && !addedItemsIndexes.Contains(i))
                {
                    int[] newState = new int[9];
                    Buffer.BlockCopy(state, 0, newState, 0, 9 * 4);
                    newState[firstZeroIndex] = i;
                    ret.Add(newState);
                }
            }

            return ret;
        }

        //Return a score based on the state
        public int GetWinnerForState(int[] state)
        {
            var score = 0;

            foreach (var index in state)
            {
                if (index != -1)
                {
                    score = score + PossibleItems[index].value;
                }
            }

            return score;
        }
    }
}
