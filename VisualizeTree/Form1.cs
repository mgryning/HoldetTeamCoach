using TTT;
using static TTT.MCTS;

namespace VisualizeTree
{
    public partial class Form1 : Form
    {
        MCTS MCTS = new MCTS();

        MCTSStepResult stepResult = new MCTSStepResult()
        {
            Root = new Node()
        };

        int currentStep = 0;

        PictureBox pictureBox1;

        public Form1()
        {
            InitializeComponent();

            Panel panel = new Panel();
            panel.Size = new Size(1000, 1000);
            panel.Location = new Point(0, 0);
            panel.AutoScroll = true;

            PictureBox pictureBox = new PictureBox();
            pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1 = pictureBox;

            panel.Controls.Add(pictureBox);
            this.Controls.Add(panel);
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            stepResult = MCTS.DoStep(stepResult, currentStep);

            currentStep = currentStep + 1;

            if (currentStep > 3)
            {
                currentStep = 0;
            }

            pictureBox1.Image = stepResult.Root.NodeToBitmap(1000, 1000, stepResult);
        }
    }
}