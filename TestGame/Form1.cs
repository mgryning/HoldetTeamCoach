using TTT;
using static TTT.Game;

namespace TestGame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        public int TTS(string text)
        {
            if (text == null || text.Length == 0)
            {
                return 0;
            }

            if (text == "X")
            {
                return (int)Player.X;
            }

            if (text == "O")
            {
                return (int)Player.O;
            }

            throw new Exception("KKK");
        }

        public string STT(int state)
        {
            if (state == 0)
            {
                return "";
            }
            
            if (state == 1)
            {
                return "X";
            }

            if (state == 2)
            {
                return "O";
            }

            throw new Exception("Emma");
        }

        public void TransferStateToTextBoxes(int[] state)
        {
            textBox1.Text = STT(state[0]);
            textBox2.Text = STT(state[1]);
            textBox3.Text = STT(state[2]);
            textBox4.Text = STT(state[3]);
            textBox5.Text = STT(state[4]);
            textBox6.Text = STT(state[5]);
            textBox7.Text = STT(state[6]);
            textBox8.Text = STT(state[7]);
            textBox9.Text = STT(state[8]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Node root = new Node()
            {
                Player = Player.O
            };

            MCTS MCTS = new MCTS();

            //make state
            root.State = new int[9] {
                TTS(textBox1.Text),
                TTS(textBox2.Text),
                TTS(textBox3.Text),
                TTS(textBox4.Text),
                TTS(textBox5.Text),
                TTS(textBox6.Text),
                TTS(textBox7.Text),
                TTS(textBox8.Text),
                TTS(textBox9.Text)
            };

            //Select a node
            var result = MCTS.DoSimulation(root, 1000000);

            //Mark AI
            double best = double.MinValue;
            var bestIndex = 0;

            for (var i=0; i < result.Children.Count; i++)
            {
                if (result.Children[i].Visits > best)
                {
                    best = result.Children[i].Visits;
                    bestIndex = i;
                }
            }

            TransferStateToTextBoxes(result.Children[bestIndex].State);
        }
    }
}