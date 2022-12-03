using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TTT.Game;
using static TTT.MCTS;

namespace TTT
{
    [DebuggerDisplay("Children: {Children.Count}, Sim: {TimesSimulated}, Won: {TimesWon}")]
    public class Node
    {
        public List<Node> Children { get; set; } = new List<Node>();
        public Node? Parent { get; set; } = null;
        public int[] State { get; set; } = new int[9];
        public Player Player { get; set; }
        public double TimesSimulated { get; set; } = 0;
        public double TimesWon { get; set; } = 0;
        public bool Simulated { get; set; } = false;
        public bool EndState { get; set; } = false;
        public bool NoFurtherMovesPossible { get; set; } = false;
        public int Depth { get; set; } = 0;

        /// <summary>
        /// Converts the node to a bitmap
        /// </summary>
        public Bitmap NodeToBitmap(int width, int height, MCTSStepResult? r = null)
        {
            var tree = GetFullTree();
            int maxDepth = this.GetMaxNodeDepth(tree);

            Bitmap ret = new Bitmap(width, 50*tree.Count+50);

            using (Graphics g = Graphics.FromImage(ret))
            {
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, width, 50 * tree.Count + 50);

                for (var i = 0; i < tree.Count; i++)
                {
                    

                    if (r != null && r.Leaf != null && tree[i] == r.Leaf)
                    {
                        g.FillRectangle(new SolidBrush(Color.LightCyan), new Rectangle(tree[i].Depth * 50, i * 50, 50, 50));
                    }

                    if (r != null && r.NewLeaf != null && tree[i] == r.NewLeaf)
                    {
                        g.FillRectangle(new SolidBrush(Color.LightCoral), new Rectangle(tree[i].Depth * 50, i * 50, 50, 50));
                    }

                    g.DrawRectangle(new Pen(new SolidBrush(Color.Black)), new Rectangle(tree[i].Depth * 50, i * 50, 50, 50));

                    g.DrawString($"{tree[i].TimesSimulated}/{tree[i].TimesWon} - {(tree[i].NoFurtherMovesPossible ? "S" : "")}", new Font("Verdana",10.0f), new SolidBrush(Color.Black), new PointF(tree[i].Depth * 50, i * 50));
                }
            }

            return ret;
        }

        public List<Node> GetFullTree()
        {
            List<Node> visited = new List<Node>();
            Stack<Node> stack = new Stack<Node>();

            visited.Add(this);

            foreach (var child in this.Children)
            {
                stack.Push(child);
            }

            while (stack.Count > 0)
            {
                var pop = stack.Pop();

                foreach (var child in pop.Children)
                {
                    if (!visited.Contains(child))
                    {
                        stack.Push(child);
                    }
                }

                visited.Add(pop);
            }

            return visited;
        }

        /// <summary>
        /// Get maximum depth from node, using DFS
        /// </summary>
        /// <returns></returns>
        public int GetMaxNodeDepth(List<Node> treeNodes)
        {
            if (treeNodes.Count == 0)
            {
                return 0;
            }

            return treeNodes.Max(t => t.Depth);
        }
    }


}
