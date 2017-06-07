using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml;
using System;

#if NETFX_CORE
using System.Xml.Linq;
#else
using System.Xml.Serialization;
#endif


public class Team : MonoBehaviour
{
    public class PlayerCont
    {
        public string name { get; set; }
        public string icon { get; set; }
        public override string ToString()
        {
            return " NAME: " + name + "   icon: " + icon + "\n";
        }
    }

    // These prefabs will hold and display player infomation as they are waiting to be added and are added
    public GameObject playerCellPrefab, addedCellPrefab;

    // Variable that knows if player info is grabbed from database; 
    // Need to known if a new player has been added, then need to pull again... or just add to set when created
    public bool loaded = false, loadedTeam1 = false, loadedTeam2 = false;

    // These list will hold the players that are waiting, and the one currently on a team.
    public List<PlayerCont> Team1, Team2, AllPlayers;

    // The UI containers that will display the above lists
    public GameObject TeamCont1, TeamCont2, AllPlayersCont, AllPlayerPanel, teamCreateBG, exitButton;

    // This script will grab all players that have made an account for a certain location
    private string playerURL = "localhost/indoor/allPlayers.php";

    // Holds the info coming from script
    XmlNodeList player;

    // Use this for initialization
    void Start()
    {
        AllPlayers = new List<PlayerCont>();
        Team1 = new List<PlayerCont>();
        Team2 = new List<PlayerCont>();
        StartCoroutine(GrabAllPlayerInfo());
    }

    // Update is called once per frame
    void Update()
    {
        //StartCoroutine(showTeam1());
        //StartCoroutine(showTeam2());
        //StartCoroutine(showAllPlayersList(AllPlayers, AllPlayersCont));
    }

    public void showAllPlayers()
    {
        AllPlayerPanel.SetActive(true);
        teamCreateBG.SetActive(true);
        exitButton.SetActive(true);
    }

    //grab leaderboard infomation
    IEnumerator GrabAllPlayerInfo()
    {
        var form = new WWWForm();
        form.AddField("location", "00000000");

        //send form to leaderboard.php
        WWW www = new WWW(playerURL, form);

        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
            //loaded = true;
#if NETFX_CORE
            XDocument xmlDoc = new XDocument();
            //xmlDoc.Load(www.text.TrimStart());
            //loadPlayerSetWindows(player, AllPlayers);
#else
            XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
            xmlDoc.LoadXml(www.text.TrimStart()); // load the file.
            player = xmlDoc.GetElementsByTagName("player"); // array of the level nodes.
            loadPlayerSet(player, AllPlayers);
#endif
            StartCoroutine(showAllPlayersList());
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    public void loadPlayerSet(XmlNodeList player, List<PlayerCont> List)
    {
        foreach (XmlNode playerInfo in player)
        {
            PlayerCont p = new PlayerCont();
#if NETFX_CORE

#else
            p.name = playerInfo.SelectSingleNode("name").InnerText;
            p.icon = playerInfo.SelectSingleNode("icon").InnerText;
            Debug.Log(p.ToString() + '\n');
#endif
            List.Add(p);
        }
    }

    public void loadPlayerSetWindows(XmlNodeList player, List<PlayerCont> List)
    {

    }

    public IEnumerator showAllPlayersList()
    {
        if (!loaded)
        {
            //remove old player cells
            clearCells(AllPlayersCont);
            //visit each player in list
            foreach (PlayerCont p in AllPlayers)
            {
                //p.ToString();
                //create new cell and assign player info
                GameObject newCell = Instantiate(playerCellPrefab) as GameObject;
                newCell.transform.Find("Name").GetComponent<Text>().text = p.name;
                newCell.transform.Find("Icon").GetComponent<TextMeshProUGUI>().text = p.icon;
                //newCell.GetComponent<Player>().name = p.name;
                //newCell.GetComponent<Player>().icon = p.icon;
                //set child to fit box
                newCell.transform.SetParent(AllPlayersCont.gameObject.transform, false);
            }
        }

        loaded = true;
        yield return null;
    }

    public IEnumerator showTeam1()
    {
        GameObject gameScript = GameObject.Find("GameScript");
        if (!loadedTeam1)
        {
            clearCells(gameScript.GetComponent<Team>().TeamCont1);
            Debug.Log("show Team 1");
            //visit each player in list
            foreach (PlayerCont p in gameScript.GetComponent<Team>().Team1)
            {
                p.ToString();
                //create new cell and assign player info
                GameObject newCell = Instantiate(addedCellPrefab) as GameObject;
                newCell.transform.Find("Name").GetComponent<Text>().text = p.name;
                newCell.transform.Find("Icon").GetComponent<TextMeshProUGUI>().text = p.icon;
                newCell.GetComponent<Score>().teamNum = "1";

                //set child to fit box
                newCell.transform.SetParent(gameScript.GetComponent<Team>().TeamCont1.gameObject.transform, false);
            }
            
        }
        loadedTeam1 = true;
        yield return null;
    }

    public IEnumerator showTeam2()
    {
        GameObject gameScript = GameObject.Find("GameScript");
        if (!loadedTeam2)
        {
            clearCells(gameScript.GetComponent<Team>().TeamCont2);
            Debug.Log("show Team 2");
            //visit each player in list
            foreach (PlayerCont p in gameScript.GetComponent<Team>().Team2)
            {
                p.ToString();
                //create new cell and assign player info
                GameObject newCell = Instantiate(addedCellPrefab) as GameObject;
                newCell.transform.Find("Name").GetComponent<Text>().text = p.name;
                newCell.transform.Find("Icon").GetComponent<TextMeshProUGUI>().text = p.icon;
                newCell.GetComponent<Score>().teamNum = "2";

                //set child to fit box
                newCell.transform.SetParent(gameScript.GetComponent<Team>().TeamCont2.gameObject.transform, false);
            }

        }
        loadedTeam2 = true;
        yield return null;
    }

    public void moveToTeam1()
    {
        //GameObject teamCreate = GameObject.Find("Main Camera");
        GameObject gameScript = GameObject.Find("GameScript");
        if (gameScript.GetComponent<Team>().Team1.Count < 5)
        {
            string playerName = gameObject.GetComponentInChildren<Text>().text;
            string playerIcon = gameObject.GetComponentInChildren<TextMeshProUGUI>().text;
            Debug.Log("Assigning player to team 1");
            //take player from all player set and assign them to team 1
            PlayerCont p = new PlayerCont();
            p.icon = playerIcon;
            p.name = playerName;


            //remove player from all player list
            for (var i = 0; i < gameScript.GetComponent<Team>().AllPlayers.Count; i++)
            {
                if (gameScript.GetComponent<Team>().AllPlayers[i].name == p.name)
                {
                    gameScript.GetComponent<Team>().AllPlayers.RemoveAt(i);
                    break;
                }
            }
            //add player to respective team
            gameScript.GetComponent<Team>().Team1.Add(p);
            //reset loaded booleans
            gameScript.GetComponent<Team>().loaded = false;
            gameScript.GetComponent<Team>().loadedTeam1 = false;

            StartCoroutine(gameScript.GetComponent<Team>().showTeam1());
            StartCoroutine(gameScript.GetComponent<Team>().showAllPlayersList());
        }
    }

    public void moveToTeam2()
    {
        //GameObject teamCreate = GameObject.Find("Main Camera");
        GameObject gameScript = GameObject.Find("GameScript");
        if (gameScript.GetComponent<Team>().Team2.Count < 5)
        {
            string playerName = gameObject.GetComponentInChildren<Text>().text;
            string playerIcon = gameObject.GetComponentInChildren<TextMeshProUGUI>().text;
            Debug.Log("Assigning player to team 2");
            //take player from all player set and assign them to team 2
            PlayerCont p = new PlayerCont();
            p.icon = playerIcon;
            p.name = playerName;

            //remove player from all player list
            for (var i = 0; i < gameScript.GetComponent<Team>().AllPlayers.Count; i++)
            {
                if (gameScript.GetComponent<Team>().AllPlayers[i].name == p.name)
                {
                    gameScript.GetComponent<Team>().AllPlayers.RemoveAt(i);
                    break;
                }
            }

            //add player to respective team
            gameScript.GetComponent<Team>().Team2.Add(p);
            //reset loaded booleans
            gameScript.GetComponent<Team>().loaded = false;
            gameScript.GetComponent<Team>().loadedTeam2 = false;

            StartCoroutine(gameScript.GetComponent<Team>().showTeam2());
            StartCoroutine(gameScript.GetComponent<Team>().showAllPlayersList());
        }
    }

    public void removeFromTeam()
    {
        GameObject gameScript = GameObject.Find("GameScript");
        string teamNum = gameObject.GetComponentInParent<Score>().teamNum;
        string playerName = gameObject.GetComponentInChildren<Text>().text;
        string playerIcon = gameObject.GetComponentInChildren<TextMeshProUGUI>().text;
        Debug.Log("Removing player from team 1");
        //take player from all player set and assign them to team 2
        PlayerCont p = new PlayerCont();
        p.icon = playerIcon;
        p.name = playerName;

        //remove player from all player list
        if (teamNum == "1")
        {
            for (var i = 0; i < gameScript.GetComponent<Team>().Team1.Count; i++)
            {
                if (gameScript.GetComponent<Team>().Team1[i].name == p.name)
                {
                    gameScript.GetComponent<Team>().Team1.RemoveAt(i);
                    break;
                }
            }
            gameScript.GetComponent<Team>().loadedTeam1 = false;
            StartCoroutine(gameScript.GetComponent<Team>().showTeam1());
        }
        if (teamNum == "2")
        {
            for (var i = 0; i < gameScript.GetComponent<Team>().Team2.Count; i++)
            {
                if (gameScript.GetComponent<Team>().Team2[i].name == p.name)
                {
                    gameScript.GetComponent<Team>().Team2.RemoveAt(i);
                    break;
                }
            }
            gameScript.GetComponent<Team>().loadedTeam2 = false;
            StartCoroutine(gameScript.GetComponent<Team>().showTeam2());
        }
        //add player back to all players list
        gameScript.GetComponent<Team>().AllPlayers.Add(p);
        //reset loaded booleans
        gameScript.GetComponent<Team>().loaded = false;
        StartCoroutine(gameScript.GetComponent<Team>().showAllPlayersList());
    }
    
    void clearCells(GameObject TeamCont)
    {
        //clear leaderboard player cells in vertical scroll
        foreach (Transform child in TeamCont.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void playerScored()
    {
        //find out what team the player is on to increase score
        //find out player name to update db

        GameObject scoreScript = GameObject.Find("Main Camera");

        scoreScript.GetComponent<Score>().increaseScore();

    }
}
