using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HoldetTeamCoach
{
    public class QSearch
    {
        const int priceIndex = 11;
        const int intSize = 4;
        Random random = new Random(DateTime.Now.Microsecond);

        Player[] PlayersIndexed = new Player[919];
        Player[] Goalies;
        Player[] Defenders;
        Player[] Midfielders;
        Player[] Attackers;

        public Player GetPlayerWithIndex(int index)
        {
            return PlayersIndexed[index];
        }

        public void SetPlayers(List<Player> players)
        {
            int goalieCount = players.Where(p => p.Position == "Mål").Count();
            int defenderCount = players.Where(p => p.Position == "Forsvar").Count();
            int midfieldCount = players.Where(p => p.Position == "Midtbane").Count();
            int attackerCount = players.Where(p => p.Position == "Angreb").Count();

            Goalies = new Player[goalieCount];
            Defenders = new Player[defenderCount];
            Midfielders = new Player[midfieldCount];
            Attackers = new Player[attackerCount];

            int addedGoalies = 0;
            int addedDefenders = 0;
            int addedMidfielders = 0;
            int addedAttackers = 0;

            for (var i = 0; i < players.Count; i++)
            {
                PlayersIndexed[i] = players[i];
                PlayersIndexed[i].Id = i;
                PlayersIndexed[i].iPrice = Convert.ToInt32(players[i].Price);

                if (PlayersIndexed[i].Position == "Mål")
                {
                    Goalies[addedGoalies++] = PlayersIndexed[i];
                }
                else if (PlayersIndexed[i].Position == "Forsvar")
                {
                    Defenders[addedDefenders++] = PlayersIndexed[i];
                }
                else if (PlayersIndexed[i].Position == "Midtbane")
                {
                    Midfielders[addedMidfielders++] = PlayersIndexed[i];
                }
                else if (PlayersIndexed[i].Position == "Angreb")
                {
                    Attackers[addedAttackers++] = PlayersIndexed[i];
                }
            }
        }

        public float CalculateScoreForState(int[] state)
        {
            float totalScore = 0.0f;

            for (var i = 0; i < 11; i++)
            {
                if (state[i] == -1)
                {
                    totalScore -= 1.0f;
                }
                else
                {
                    totalScore += PlayersIndexed[state[i]].Score;
                }
            }

            return totalScore;
        }

        public QSearchReturn TestSimulate(int numEpocs, bool writeProgress)
        {
            int[] bestStateFound = new int[12];
            float bestScoreFound = 0.0f;

            for (var u=0; u < numEpocs; u++)
            {
                if (u != 0 && u % 5000 == 0)
                {
                    Console.WriteLine($"Ran 5000 epocs, best score: {bestScoreFound}. Progress: {u}/{numEpocs}");
                }

                if (writeProgress && u != 0 && u % 50000 == 0)
                {
                    StreamWriter sw = new StreamWriter("progress.txt",false);
                    sw.WriteLine($"{string.Join(',', bestStateFound)}");
                    sw.WriteLine($"{bestScoreFound}");
                    sw.Close();
                    Console.WriteLine("Wrote to file");
                }

                var currentState = new int[12] { 241,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,50000000-5500000 };
                //var currentState = new int[12] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 50000000 - 5500000 };

                while (true)
                {
                    var moves = GetPossibleMovesFromState(currentState);

                    if (moves.Count == 0)
                    {
                        break;
                    }

                    //If we random explore, then select random move, if we exploit, then select the best possible move
                    int action = random.Next(0, 4);

                    if (action <= 2)
                    {
                        //Explore
                        var selectedMove = random.Next(moves.Count);
                        currentState = moves[selectedMove];
                    }
                    else
                    {
                        //Exploit
                        float bestMoveScore = 0.0f;
                        int bestMoveIndex = 0;

                        for (var j=0; j < moves.Count; j++)
                        {
                            float moveScore = CalculateScoreForState(moves[j]);

                            if (moveScore > bestMoveScore)
                            {
                                bestMoveScore = moveScore;
                                bestMoveIndex = j;
                            }
                        }

                        currentState = moves[bestMoveIndex];
                    }
                }

                //Get score
                float totalScore = CalculateScoreForState(currentState);

                if (totalScore > bestScoreFound)
                {
                    bestScoreFound = totalScore;
                    bestStateFound = currentState;
                }
            }

            return new QSearchReturn()
            {
                Score = bestScoreFound,
                State = bestStateFound
            };

        }

        public int[]? GetRandomNextState(int[] currentState)
        {
            var moves = GetPossibleMovesFromState(currentState);

            if (moves.Count == 0)
            {
                return null;
            }

            return moves[random.Next(moves.Count)];
        }

        //Get all possible moves from the state with business logic implemented
        //we moves from left to right to reduce complexity
        public List<int[]> GetPossibleMovesFromState(int[] state)
        {
            List<int[]> ret = new List<int[]>();

            int currIndex = state.Length;

            //11 players, 1 goalie, 3 defenders, 4 midfield, 3 attackers. Find next possible state index where we can add
            for (var j = 0; j < state.Length; j++)
            {
                if (state[j] == -1)
                {
                    currIndex = j;
                    break;
                }
            }

            //We cannot go any further
            if (currIndex == state.Length)
            {
                return ret;
            }

            //Determine which player to add
            if (currIndex == 0)
            {
                //Goalie - first player to add
                for (var i = 0; i < Goalies.Length; i++)
                {
                    int[] newState = new int[12];
                    Buffer.BlockCopy(state, 0, newState, 0, 12 * intSize);
                    newState[currIndex] = Goalies[i].Id;
                    newState[priceIndex] = state[priceIndex] - Goalies[i].iPrice;
                    ret.Add(newState);
                }
            }
            else if (currIndex == 1 || currIndex == 2 || currIndex == 3)
            {
                for (var i = 0; i < Defenders.Length; i++)
                {
                    bool duplicate = false;

                    int[] newState = new int[12];
                    var id = Defenders[i].Id;

                    for (var u = 0; u < currIndex; u++)
                    {
                        if (state[u] == id)
                        {
                            duplicate = true;
                            break;
                        }
                    }

                    if (duplicate)
                    {
                        continue;
                    }

                    Buffer.BlockCopy(state, 0, newState, 0, 12 * intSize);

                    newState[currIndex] = id;
                    newState[priceIndex] = state[priceIndex] - Defenders[i].iPrice;
                    ret.Add(newState);
                }
            }
            else if (currIndex == 4 || currIndex == 5 || currIndex == 6 || currIndex == 7)
            {
                int[] stateTeams = new int[(int)PlayerCountry.MAX];

                for (var u = 0; u < currIndex; u++)
                {
                    stateTeams[PlayersIndexed[state[u]].TeamEnum]++;
                }

                for (var i = 0; i < Midfielders.Length; i++)
                {
                    bool duplicate = false;
                    int[] newState = new int[12];
                  
                    var id = Midfielders[i].Id;
                    var team = Midfielders[i].TeamEnum;

                    //We have already added 3 of this team
                    if (stateTeams[team] >= 4)
                    {
                        continue;
                    }

                    //Check that player is not already added
                    for (var u = 0; u < currIndex; u++)
                    {
                        if (state[u] == id)
                        {
                            duplicate = true;
                            break;
                        }
                    }

                    if (duplicate)
                    {
                        continue;
                    }

                    Buffer.BlockCopy(state, 0, newState, 0, 12 * intSize);

                    //Don't add players we can't afford
                    if (state[priceIndex] - Midfielders[i].iPrice < 0)
                    {
                        continue;
                    }

                    newState[currIndex] = id;
                    newState[priceIndex] = state[priceIndex] - Midfielders[i].iPrice;
                    ret.Add(newState);
                }
            }
            else if (currIndex == 8 || currIndex == 9 || currIndex == 10)
            {
                int[] stateTeams = new int[(int)PlayerCountry.MAX];

                for (var u = 0; u < currIndex; u++)
                {
                    stateTeams[PlayersIndexed[state[u]].TeamEnum]++;
                }

                for (var i = 0; i < Attackers.Length; i++)
                {
                    bool duplicate = false;
                    int[] newState = new int[12];
                    var id = Attackers[i].Id;
                    var team = Attackers[i].TeamEnum;

                    //We have already added 3 of this team
                    if (stateTeams[team] >= 4)
                    {
                        continue;
                    }

                    //Check that player is not already added
                    for (var u = 0; u < currIndex; u++)
                    {
                        if (state[u] == id)
                        {
                            duplicate = true;
                            break;
                        }
                    }

                    if (duplicate)
                    {
                        continue;
                    }

                    Buffer.BlockCopy(state, 0, newState, 0, 12 * intSize);

                    //Don't add players we can't afford
                    if (state[priceIndex] - Attackers[i].iPrice < 0)
                    {
                        continue;
                    }

                    //Check nationality (only 4 from same team allowed)
                    newState[currIndex] = id;
                    newState[priceIndex] = state[priceIndex] - Attackers[i].iPrice;
                    ret.Add(newState);
                }
            }

            return ret;
        }

    }
}
