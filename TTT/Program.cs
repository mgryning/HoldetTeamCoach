using System.Xml;
using TTT;
using static TTT.Game;

Console.WriteLine("Hello, TTT!");

//Construct tree
Node root = new Node()
{
    Player = Player.X
};

MCTS MCTS = new MCTS();

//Select a node
var result = MCTS.DoSimulation(root,1000000);

var a = 5;
