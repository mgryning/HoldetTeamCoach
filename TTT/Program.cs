using System.Xml;
using TTT;
using static TTT.Game;

Console.WriteLine("Hello, TTT!");

//Construct tree
Node root = new Node()
{
    State = new int[9] {-1,-1,-1,-1,-1,-1,-1,-1,-1},
    Visits = 1
};

MCTSSP MCTS = new MCTSSP();

//Select a node
var result = MCTS.DoSimulation(root,2500);

var node = result;

//Find way to best score
while (node.Children.Count > 0)
{
    double best = 0;
    int bestIndex = -1;

    for (var i = 0; i < node.Children.Count; i++)
    {
        if (node.Children[i].Score >= best)
        {
            best = node.Children[i].Score;
            bestIndex = i;
        }
    }

    node = node.Children[bestIndex];
}

//get sum
int sum = 0;
int weight = 0;

for (var i=0; i < node.State.Length;i++)
{
    if (node.State[i] != -1)
    {
        sum = sum + MCTS.GameMaxBag.PossibleItems[node.State[i]].value;
        weight = weight + MCTS.GameMaxBag.PossibleItems[node.State[i]].weight;
    }
}

var a = 5;
