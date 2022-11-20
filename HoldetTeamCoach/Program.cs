// See https://aka.ms/new-console-template for more information
using HoldetTeamCoach;
using OpenQA.Selenium;
using Service;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

const float countryConstant = 2.0f;
const float noOddsConstant = 300.0f;

//Load data
StreamReader sr = new StreamReader("Data\\Dataplayers.txt");
List<Player> players = JsonSerializer.Deserialize<List<Player>>(sr.ReadToEnd())!;
sr.Close();

sr = new StreamReader("Data\\DataOddsPlayers.txt");
List<Player> playersOdds = JsonSerializer.Deserialize<List<Player>>(sr.ReadToEnd())!;
sr.Close();

sr = new StreamReader("Data\\DataCountryOdds.txt");
List<Country> countryOdds = JsonSerializer.Deserialize<List<Country>>(sr.ReadToEnd())!;
sr.Close();

sr = new StreamReader("Data\\DataPlayerOddsAssist.txt");
List<Player> playersOddsAssists = JsonSerializer.Deserialize<List<Player>>(sr.ReadToEnd())!;
sr.Close();

//Test 
foreach (var player in players)
{
    var playerOddTopScorer = playersOdds.Where(p => p.Name.ToLower() == player.Name.ToLower()).FirstOrDefault();
    var playerCountryOdds = countryOdds.Where(c => c.Name.ToLower() == player.Team.ToLower()).FirstOrDefault();
    var playerOddsAssist = playersOddsAssists.Where(c => c.Name.ToLower() == player.Name.ToLower()).FirstOrDefault();

    player.fOddsCountry = Convert.ToSingle(playerCountryOdds.Odds!);

    if (playerOddTopScorer != null)
    {
        player.fOddsPlayer = Convert.ToSingle(playerOddTopScorer.Odds);
        player.Score = (1.0f / player.fOddsPlayer) + (1.0f / (player.fOddsCountry* countryConstant));
    }

    if (playerOddsAssist != null)
    {
        player.fOddsAssist = Convert.ToSingle(playerOddsAssist.Odds);

        if (player.fOddsAssist < player.fOddsPlayer || player.fOddsPlayer == 0)
        {
            player.Score = (1.0f / player.fOddsAssist) + (1.0f / (player.fOddsCountry* countryConstant));
        }
    }

    //Assign low score since player is not on assist list or topscorer list
    if (playerOddTopScorer == null && playerOddsAssist == null)
    {
        //player.Score = 0.0f;
        player.Score = (1.0f / noOddsConstant) + (1.0f / (player.fOddsCountry* countryConstant));
    }

    player.iPrice = Convert.ToInt32(player.Price);
    SetPlayerCountryAsEnum(player);
}

//Tracking
//var notExisting = playersOddsAssists.Where(p => !players.Select(p => p.Name.ToLower()).Contains(p.Name.ToLower()));
//var totalWithScore = players.Where(p => p.fOddsAssist > 0).ToList();
//var defenderswithOdds = players.Where(p => (p.fOddsAssist > 0 || p.fOddsPlayer > 0) && p.Position == "Forsvar").ToList();
//var midFielderswithOdds = players.Where(p => (p.fOddsAssist > 0 || p.fOddsPlayer > 0) && p.Position == "Midtbane" && p.iPrice <= 2000000).OrderByDescending(p => p.Score).ToList();

//Start search
QSearch qSearch= new QSearch();
qSearch.SetPlayers(players);

int epocs = 10000000;

//Read args from console
if (args.Length > 0 && args[0] != null && args[0].Length > 0)
{
    epocs = Convert.ToInt32(args[0]);
}

QSearchReturn ret = qSearch.TestSimulate(epocs, false);

Console.WriteLine($"Score: {ret.Score}. State: {String.Join(',', ret.State)}");

string playerNames = string.Empty;

foreach (int i in ret.State.SkipLast(1))
{
    if (i != -1)
    {
        playerNames += $"{qSearch.GetPlayerWithIndex(i).Name} ({qSearch.GetPlayerWithIndex(i).Team}) - Odds:{Math.Max(qSearch.GetPlayerWithIndex(i).fOddsPlayer, qSearch.GetPlayerWithIndex(i).fOddsAssist)}. Score: {qSearch.GetPlayerWithIndex(i).Score}] {Environment.NewLine}, ";
    }
}

Console.WriteLine($"Players with index: {playerNames}");

//var driver = await new SetupChrome().GetRemoveWebDriver();

//driver.Navigate().GoToUrl("https://danskespil.dk/oddset/sports/competition/15840/fodbold/international/vm-i-fodbold-2022/matches");

//await Task.Delay(15000);

//List<Player> playerAssistOdds = new List<Player>();

//var buttons = driver.FindElementsByXPath("//div[contains(text(),'Flest assists-Flest assists i turneringen')]/parent::div/parent::div//button");

//foreach (var button in buttons)
//{
//    var name = button.FindElement(By.XPath(".//span[contains(@class,'title')]"));
//    var price = button.FindElement(By.XPath(".//span[@class='button--outcome__price']"));

//    string titleText = name.Text;
//    string priceText = price.Text;

//    playerAssistOdds.Add(new Player()
//    {
//        Name = name.Text,
//        Odds = priceText
//    });
//}

//var buttons = driver.FindElementsByXPath("//div[contains(text(),'Topscorer-VM')]/parent::div/parent::div//button");

//foreach (var button in buttons)
//{
//    var name = button.FindElement(By.XPath(".//span[contains(@class,'title')]"));
//    var price = button.FindElement(By.XPath(".//span[@class='button--outcome__price']"));

//    string titleText = name.Text;
//    string priceText = price.Text;

//    players.Add(new Player()
//    {
//        Name = name.Text,
//        Odds = priceText
//    });
//}

//var data = JsonSerializer.Serialize(countryList);
//StreamWriter stream = new StreamWriter("DataPlayerOddsAssist.txt");
//stream.Write(JsonSerializer.Serialize(playerAssistOdds));
//stream.Close();

//$x("//div[contains(text(),'Topscorer-VM')]/parent::div/parent::div//button")

//ExtractPlayers(driver, players);

//while (ClickNext(driver))
//{
//    ExtractPlayers(driver, players);
//}

//var data = JsonSerializer.Serialize(players);

//StreamWriter sw = new StreamWriter("data.txt");
//sw.Write(data);
//sw.Close();

static void SetPlayerCountryAsEnum(Player player)
{
    switch (player.Team.ToLower())
    {
        case "danmark": player.TeamEnum = (int)PlayerCountry.DENMARK; break;
        case "england": player.TeamEnum = (int)PlayerCountry.ENGLAND; break;
        case "frankrig": player.TeamEnum = (int)PlayerCountry.FRANCE; break;
        case "argentina": player.TeamEnum = (int)PlayerCountry.ARGENTINA; break;
        case "belgien": player.TeamEnum = (int)PlayerCountry.BELGIUM; break;
        case "usa": player.TeamEnum = (int)PlayerCountry.USA; break;
        case "uruguay": player.TeamEnum = (int)PlayerCountry.URUGUAY; break;
        case "sydkorea": player.TeamEnum = (int)PlayerCountry.SOUTHKOREA; break;
        case "senegal": player.TeamEnum = (int)PlayerCountry.SENEGAL; break;
        case "saudi-arabien": player.TeamEnum = (int)PlayerCountry.SAUDIARABIA; break;
        case "marokko": player.TeamEnum = (int)PlayerCountry.MAROCCO; break;
        case "mexico": player.TeamEnum = (int)PlayerCountry.MEXICO; break;
        case "japan": player.TeamEnum = (int)PlayerCountry.JAPAN; break;
        case "iran": player.TeamEnum = (int)PlayerCountry.IRAN; break;
        case "ghana": player.TeamEnum = (int)PlayerCountry.GHANA; break;
        case "costa rico": player.TeamEnum = (int)PlayerCountry.COSTARICA; break;
        case "canada": player.TeamEnum = (int)PlayerCountry.CANADA; break;
        case "brasilien": player.TeamEnum = (int)PlayerCountry.BRASIL; break;
        case "cameroun": player.TeamEnum = (int)PlayerCountry.CAMEROUN; break;
        case "kroatien": player.TeamEnum = (int)PlayerCountry.CROATIA; break;
        case "ecuador": player.TeamEnum = (int)PlayerCountry.ECUADOR; break;
        case "tyskland": player.TeamEnum = (int)PlayerCountry.GERMANY; break;
        case "holland": player.TeamEnum = (int)PlayerCountry.HOLLAND; break;
        case "polen": player.TeamEnum = (int)PlayerCountry.POLAND; break;
        case "portugal": player.TeamEnum = (int)PlayerCountry.PORTUGAL; break;
        case "serbien": player.TeamEnum = (int) PlayerCountry.SERBIA; break;
        case "spanien": player.TeamEnum = (int)PlayerCountry.SPAIN; break;
        case "schweiz": player.TeamEnum = (int)PlayerCountry.SWITZERLAND; break;
        case "wales": player.TeamEnum = (int)PlayerCountry.WALES; break;
        case "qatar": player.TeamEnum = (int)PlayerCountry.QATAR; break;
        case "australien": player.TeamEnum = (int)PlayerCountry.QATAR; break;
        case "tunesien": player.TeamEnum = (int)PlayerCountry.TUNESIA; break;
        case "costa rica": player.TeamEnum = (int)PlayerCountry.COSTARICA; break;
        default: throw new Exception($"invalid country {player.Team}");
    }
}

static void ExtractPlayers(OpenQA.Selenium.Remote.RemoteWebDriver driver, List<Player> players)
{
    var playerRowNames = driver.FindElementsByXPath("//div[contains(@class,'ScrollTable__scrollTable')]/div[1]//tr");
    var playerRowInfo = driver.FindElementsByXPath("//div[contains(@class,'ScrollTable__scrollTable')]/div[2]//tr");

    for (int i = 1; i < playerRowNames.Count; i++)
    {

        var fullName = playerRowNames[i].FindElement(OpenQA.Selenium.By.XPath(".//span[contains(@class, 'PersonName__full')]"));
        var team = playerRowNames[i].FindElement(OpenQA.Selenium.By.XPath(".//span[contains(@class,'PlayerByline__full')][1]"));
        var position = playerRowNames[i].FindElement(OpenQA.Selenium.By.XPath(".//span[contains(@class,'PlayerByline__full')][2]"));

        var fullNameText = fullName.Text;
        var teamText = team.Text;
        var positionText = position.Text;

        var price = playerRowInfo[i].FindElement(OpenQA.Selenium.By.XPath(".//td[1]"));

        var priceText = price.Text;
        var dPrice = Convert.ToDouble(priceText.Replace(".", ""));

        players.Add(new Player()
        {
            Name = fullNameText,
            Position= positionText,
            Price= dPrice,
            Team = teamText
        });
    }
}

static bool ClickNext(OpenQA.Selenium.Remote.RemoteWebDriver driver)
{
    var paginationElement = driver.FindElements(OpenQA.Selenium.By.XPath("(//div[contains(@class,'StyledButton')])[last()]"));

    if (paginationElement.Count > 0)
    {
        paginationElement[0].Click();
        return true;
    }

    return false;
}