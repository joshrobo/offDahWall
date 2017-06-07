using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{

    public Text TimerText, StartText, Output;
    public TextMeshProUGUI Score1, Score2;

    private float startTime, timePassed = 0;
    private int min;
    private float sec;
    private bool started = false;
    private bool stopped = false;
    private int minsPassed;
    private float secsPassed;
    private bool finished = false;
    private bool goldengoal = false;
    private string goalsToWin = "3";
    private int gametime = 10;
    private int overtime = 2;
    private int winner = 0;
    //public AudioSource StartGameSound, GoalScoreSound, EndGameSound;

    //public List<Team.PlayerCont> Team1, Team2;
    //public GameObject TeamCont1, TeamCont2;

    // Use this for initialization
    void Start()
    {
        Output.text = "";
        TimerText.text = "0:0";
        StartText.text = "Start";
        //pull all player information from db and put into set
    }

    public void startTimer()
    {
        
        if (StartText.text == "Start")
        {
            startTime = Time.time;
            started = true;
            StartText.text = "Stop";
            //StartGameSound.Play();
        }
        else if (StartText.text == "New Game")
        {
            resetGame();
            StartText.text = "Start";
        }
        else if (StartText.text == "Stop")
        {
            started = false;
            stopped = true;
            minsPassed = min;
            secsPassed = sec;
            timePassed = Time.time - startTime;
            TimerText.text = minsPassed + ":" + secsPassed.ToString("f2");
            StartText.text = "Start";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (finished)
            return;
        if (started)
        {
            float t = timePassed + (Time.time - startTime);
            min = (int)t / 60;
            string minutes = min.ToString();
            sec = t % 60;
            string seconds = sec.ToString("f0");

            TimerText.text = minutes + ":" + seconds;

            updateFinish();
        }
        if (stopped)
        {
            //stop time
            
        }

    }

    public void updateFinish()
    {
        if (Score1.text == goalsToWin) //if team 1 reaches three, end game
        {
            //team 1 wins
            Score1.color = Color.red;
            Output.color = new Color(Output.color.r, Output.color.g, Output.color.b, 1);
            Output.text = "Team 1 wins!!";
            winner = 1;
            endGame();
        }

        if (Score2.text == goalsToWin) //if team 2 reaches three, end game
        {
            //team 2 wins
            Score2.color = Color.red;
            Output.color = new Color(Output.color.r, Output.color.g, Output.color.b, 1);
            Output.text = "Team 2 wins!!";
            winner = 2;
            endGame();
        }

        //if timer reaches 10 mins and there is a team that has more goals than other, end game
        if ((min == gametime) && Score1.text != Score2.text)
        {
            if (Convert.ToInt32(Score1.text) > Convert.ToInt32(Score2.text))
            {
                //team 1 wins
                Score1.color = Color.red;
                Output.color = new Color(Output.color.r, Output.color.g, Output.color.b, 1);
                Output.text = "Team 1 wins!!!!";
                winner = 1;
            }

            if (Convert.ToInt32(Score1.text) < Convert.ToInt32(Score2.text))
            {
                //team 2 wins
                Score2.color = Color.red;
                Output.color = new Color(Output.color.r, Output.color.g, Output.color.b, 1);
                Output.text = "Team 2 wins!!!!!";
                winner = 2;
            }
            endGame();
        }

        if ((min == gametime) && Score1.text == Score2.text) // score is tied, go into two minute overtime, golden goal 
        {
            //output overtime
            Output.color = new Color(Output.color.r, Output.color.g, Output.color.b, 1);
            Output.text = overtime + " minute overtime! Golden goal!";
            goldengoal = true;
            gametime += overtime;
        }

        if (goldengoal)
        {
            //one team scores a goal that team wins
            if ((min < gametime + overtime))
            {
                if (Convert.ToInt32(Score1.text) > Convert.ToInt32(Score2.text))
                {
                    //team 1 wins
                    Score1.color = Color.red;
                    Output.color = new Color(Output.color.r, Output.color.g, Output.color.b, 1);
                    Output.text = "Team 1 wins!";
                    winner = 1;
                    endGame();
                }

                if (Convert.ToInt32(Score1.text) < Convert.ToInt32(Score2.text))
                {
                    //team 2 wins
                    Score2.color = Color.red;
                    Output.color = new Color(Output.color.r, Output.color.g, Output.color.b, 1);
                    Output.text = "Team 2 wins!";
                    winner = 2;
                    endGame();
                }

            }
            //else both teams tie
            else
            {
                Output.color = new Color(Output.color.r, Output.color.g, Output.color.b, 1);
                Output.text = "Tie";
                winner = 0;
                endGame();
            }
        }
    }
    public void endGame()
    {
        //play sound

        TimerText.color = Color.red; //change timer to red
        finished = true; //break update
        StartText.text = "New Game";
    }

    public void resetGame()
    {
        Output.text = "";
        TimerText.color = Color.black;
        timePassed = 0;
        TimerText.text = "0:0";
        Score1.text = "0";
        Score1.color = Color.black;
        Score2.text = "0";
        Score2.color = Color.black;
        finished = false;
        started = false;
        stopped = false;
        goldengoal = false;
        GameObject gameScript = GameObject.Find("GameScript");
       
        if (winner == 1) //remove team 2 from list
        {
            Debug.Log("Removing team 2");
            int i = 1;
            Debug.Log("before: " + gameScript.GetComponent<Team>().Team2.Count);
            foreach (Team.PlayerCont player in gameScript.GetComponent<Team>().Team2)
            {
                Debug.Log(i++);
                Team.PlayerCont p = new Team.PlayerCont();
                p.name = player.name;
                p.icon = player.icon;
                gameScript.GetComponent<Team>().AllPlayers.Add(p);
                //gameScript.GetComponent<Team>().Team2.Remove(player);
                
            }
            gameScript.GetComponent<Team>().Team2.Clear();
            gameScript.GetComponent<Team>().loadedTeam2 = false;
            Debug.Log("after: " + gameScript.GetComponent<Team>().Team2.Count);
        }
        if (winner == 2) //remove team 1 from list
        {
            for (var i = 0; i < gameScript.GetComponent<Team>().Team1.Count; i++)
            {
                Team.PlayerCont p = new Team.PlayerCont();
                p.name = gameScript.GetComponent<Team>().Team1[i].name;
                p.icon = gameScript.GetComponent<Team>().Team1[i].icon;
                gameScript.GetComponent<Team>().AllPlayers.Add(p);
            }
            gameScript.GetComponent<Team>().Team1.Clear();
            gameScript.GetComponent<Team>().loadedTeam1 = false;
        }
        if (winner == 0) //tie, remove both from list
        {
            //remove team 1
            for (var i = 0; i < gameScript.GetComponent<Team>().Team1.Count; i++)
            {
                Team.PlayerCont p = new Team.PlayerCont();
                p.name = gameScript.GetComponent<Team>().Team1[i].name;
                p.icon = gameScript.GetComponent<Team>().Team1[i].icon;
                gameScript.GetComponent<Team>().AllPlayers.Add(p);
            }
            gameScript.GetComponent<Team>().Team1.Clear();
            gameScript.GetComponent<Team>().loadedTeam1 = false;
            //remove team 2
            for (var i = 0; i < gameScript.GetComponent<Team>().Team2.Count; i++)
            {
                Team.PlayerCont p = new Team.PlayerCont();
                p.name = gameScript.GetComponent<Team>().Team2[i].name;
                p.icon = gameScript.GetComponent<Team>().Team2[i].icon;
                gameScript.GetComponent<Team>().AllPlayers.Add(p);
            }
            gameScript.GetComponent<Team>().Team2.Clear();
            gameScript.GetComponent<Team>().loadedTeam2 = false;
        }
        gameScript.GetComponent<Team>().loaded = false;

        StartCoroutine(gameScript.GetComponent<Team>().showTeam2());
        StartCoroutine(gameScript.GetComponent<Team>().showTeam1());
        StartCoroutine(gameScript.GetComponent<Team>().showAllPlayersList());
    }

    public IEnumerator OutputText(Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        yield return null;
    }
}
