using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TTT.Game;

namespace TTT
{
    public class MCTS
    {
        readonly Random random = new Random(DateTime.Now.Millisecond);
        public Game game = new Game();

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
                Winner winner = Simulate(newLeaf);
                Backpropagation(newLeaf, winner);
            }

            return root;
        }

        public class MCTSStepResult
        {
            public Node Root { get; set; }
            public Node Leaf { get; set; }
            public Node NewLeaf { get; set; }
            public Winner Winner { get; set; }
        }

        public MCTSStepResult DoStep(MCTSStepResult result, int step)
        {
            if (step == 0)
            {
                Node leaf = SelectLeafNode(result.Root);

                return new MCTSStepResult()
                {
                    Root = result.Root,
                    Leaf = leaf
                };
            }
            else if (step == 1)
            {
                Node newLeaf = ExpandLeafNodeWithMoves(result.Leaf);

                return new MCTSStepResult()
                {
                    Root = result.Root,
                    Leaf = result.Leaf,
                    NewLeaf = newLeaf
                };
            }
            else if (step == 2)
            {
                Winner winner = Simulate(result.NewLeaf);

                return new MCTSStepResult()
                {
                    Root = result.Root,
                    Leaf = result.Leaf,
                    NewLeaf = result.NewLeaf,
                    Winner = winner
                };
            }
            else if (step == 3)
            {
                Backpropagation(result.NewLeaf, result.Winner);

                return new MCTSStepResult()
                {
                    Root = result.Root,
                    Leaf = result.Leaf,
                    NewLeaf = result.NewLeaf,
                    Winner = result.Winner
                };
            }

            throw new Exception("Invalid Step");

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

                    childrenWithMoves.Add(child);

                    var UCB = (child.TimesWon / child.TimesSimulated) + (1.444 * Math.Sqrt(Math.Log(current.TimesSimulated) / child.TimesSimulated));

                    if (UCB > bestUCB)
                    {
                        bestChild = child;
                        bestUCB = UCB;
                    }
                }

                //Cannot determine child based on UCB, select one at random?
                if (bestChild != null)
                {
                    current = bestChild;
                }
                else if (bestChild == null && childrenWithMoves.Count > 0)
                {
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
            var moves = game.GetPossibleMoves(leaf.State, leaf.Player);

            foreach (var move in moves)
            {
                Node newLeaf = new Node()
                {
                    Player = (leaf.Player == Player.X) ? Player.O : Player.X,
                    State = move,
                    Depth = leaf.Depth + 1
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

        public Winner Simulate(Node leaf)
        {
            var player = leaf.Player;
            var moves = game.GetPossibleMoves(leaf.State, leaf.Player);
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
            else while (moves.Count > 0)
            {
                //Random moves
                currentMove = moves[random.Next(moves.Count)];

                if (player == Player.O)
                {
                    player = Player.X;
                }
                else
                {
                    player = Player.O;
                }

                moves = game.GetPossibleMoves(currentMove, player);
            }

            var winner = game.DetermineWinner(currentMove);
            leaf.Simulated = true;

            return winner;
        }

        //Backpropagate from the leaf
        public void Backpropagation(Node leaf, Winner winner)
        {
            Node? current = leaf;

            while (current != null)
            {
                current.TimesSimulated++;

                if (winner == 0)
                {
                    current.TimesWon += 0.5;
                }
                else if ((int)winner == (int)current.Player)
                {
                    current.TimesWon += 1.0;
                }

                current = current.Parent;
            }
        }
    }
}
