using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TTT.Game;

namespace TTT
{
    public class MCTSSP
    {
        readonly Random random = new Random(DateTime.Now.Millisecond);
        public GameMaxBag GameMaxBag = new GameMaxBag();

        public Node DoSimulation(Node root, int epochNum)
        {
            //Select a node
            for (var epoch = 0; epoch < epochNum; epoch++)
            {
                Node leaf = SelectLeafNode(root);

                if (leaf == null)
                {
                    //All moves investigated
                    break;
                }

                Node newLeaf = ExpandLeafNodeWithMoves(leaf);
                int score = Simulate(newLeaf);
                Backpropagation(newLeaf, score);
            }

            return root;
        }

        //Select a leaf node which we can simulate
        public Node SelectLeafNode(Node root)
        {
            Node current = root;

            while (current.Children.Count > 0)
            {
                Node? bestChild = null;
                double bestUCB = 0.0;

                List<Node> childrenWithMoves = new List<Node>();

                //Select another child based on UCB if it's possible (not NaN(
                foreach (var child in current.Children)
                {
                    if (child.NoFurtherMovesPossible)
                    {
                        continue;
                    }

                    var UTC = EvaluateAndSetUTC(child, root);

                    if (UTC > bestUCB)
                    {
                        bestChild = child;
                        bestUCB = UTC;
                    }

                    childrenWithMoves.Add(child);
                }

                if (bestChild != null)
                {
                    //Decide based on UTC
                    current = bestChild;
                }
                else if (bestChild == null && childrenWithMoves.Count > 0)
                {
                    //Cannot decide based on UTC, take one at random
                    current = childrenWithMoves[random.Next(childrenWithMoves.Count)];
                }
                else
                {
                    //no children with possible moves, this should only happen when all moves are investigated
                    if (current.Parent == null)
                    {
                        return null;
                    }

                    throw new Exception("KK");
                }
            }

            return current;
        }

        public Node ExpandLeafNodeWithMoves(Node leaf)
        {
            var moves = GameMaxBag.GetPossibleMovesFromState(leaf.State);

            foreach (var move in moves)
            {
                Node newLeaf = new Node()
                {
                    State = move,
                };

                newLeaf.Parent = leaf;
                leaf.Children.Add(newLeaf);
            }

            //Return leaf, not children, because no more moves are possible
            if (leaf.Children.Count == 0)
            {
                leaf.NoFurtherMovesPossible = true;
                return leaf;
            }

            return leaf.Children[random.Next(leaf.Children.Count)];
        }

        public int Simulate(Node leaf)
        {
            var moves = GameMaxBag.GetPossibleMovesFromState(leaf.State);
            int[] currentMove = leaf.State;

            if (moves.Count == 0 && leaf.Parent != null)
            {
                leaf.NoFurtherMovesPossible = true;

                //Go back and check if that part of the tree has valid moves
                var parent = leaf.Parent;
                var parentChildrenWithPossibleMoves = parent.Children.Any(c => !c.NoFurtherMovesPossible);

                while (parent != null && !parentChildrenWithPossibleMoves)
                {
                    parent.NoFurtherMovesPossible = true;
                    parent = parent.Parent;

                    if (parent == null)
                    {
                        break;
                    }

                    parentChildrenWithPossibleMoves = parent.Children.Any(c => !c.NoFurtherMovesPossible);
                }
            }
            else
            {
                while (moves.Count > 0)
                {
                    //Random moves - unless it wins the game for the player, then take that
                    currentMove = moves[random.Next(moves.Count)];
                    moves = GameMaxBag.GetPossibleMovesFromState(currentMove);
                }
            }
           
            var score = GameMaxBag.GetWinnerForState(currentMove);
            return score;
        }

        //Backpropagate from the leaf
        public void Backpropagation(Node leaf, int score)
        {
            Node? current = leaf;

            while (current != null)
            {
                current.Visits++;

                //Only update score, if it improves - this means that the root will always have the largest score, and a node's score fraction
                //can be found by Node.Score / Root.Score. E.g. 40 / 70 = 0.57
                if (score > current.Score)
                {
                    current.Score = score;
                }

                current = current.Parent;
            }
        }

        public double EvaluateAndSetUTC(Node node, Node root)
        {
            if (root.Parent != null)
            {
                throw new Exception("Fail-safe");
            }

            double c = 1.444;
            double w = node.Score;
            double n = node.Visits;
            double t;

            if (node.Parent == null)
            {
                t = node.Visits;
            }
            else
            {
                t = node.Parent.Visits;
            }

            //Two fractions. One is how much the current score match the maximum found (score / root.score), and the other is the exploration. 
            //As the node is visited, the exploration will go down, and the fraction will mean more, meaning the best way is explored
            var scoreFraction = node.Score / root.Score;
            var scoreExploration = c * Math.Sqrt(Math.Log(t) / (n+1));

            //return UTC + modification;
            //node.UTC = UTC + modification;
            return scoreFraction + scoreExploration;
        }
    }
}
