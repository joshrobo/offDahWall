using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

#if NETFX_CORE
using System.Xml.Linq;
#else
using System.Xml.Serialization;
#endif

using System.IO;
using System;
using System.Text;
using UnityEngine.SceneManagement;
using TMPro;

public class Leaderboard : MonoBehaviour {
    public GameObject playerCellPrefab;
    public class PlayerCont
    {
        public int rank { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
        public int goals { get; set; }
        public float time { get; set; }
        public override string ToString()
        {
            return rank + " NAME: " + name + "   icon: " + icon + "\ngoals: " + goals + " | time: " + time + "\n";
        }
    }
    private char currentFilter;
    private int loadmore = 0;
    private int loadamount = 25;
    private bool loaded = false;
    private bool error = false;

    //list to hold filter results
    public List<PlayerCont> playerList = new List<PlayerCont>();

    public GameObject verticalLayout;
    public GameObject scrollview;
    public Button ParagonButton, SlayerButton, HunterButton, Local_Global, Load_More, BackToOverWorld;
    XmlNodeList player;

    //For scroll view
    Vector2 scrollPosition = Vector2.zero;

    //set the URLs for the different filters....
    private string leaderboardURL = "localhost/indoor/leaderboard.php";

    void Start()
    {
        //clearCells();
        if (playerList.Count == 0) //if first time using filter, grab first 25
        {
            StartCoroutine(WaitForData()); //load by the load_more amount
        }
        
    }
    void Update()
    {
        Debug.Log(loaded);
        
    }

    //grab leaderboard infomation
    IEnumerator WaitForData()
    {
        var form = new WWWForm();
        form.AddField("location", "00000000");
        form.AddField("load_more", loadmore);
  
        //send form to leaderboard.php
        WWW www = new WWW(leaderboardURL, form);
      
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
            //loaded = true;
#if NETFX_CORE
            XDocument xmlDoc = new XDocument();
            //xmlDoc.Load(www.text.TrimStart());
            //setLeaderboardWindows(player, playerList);
#else
            XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
            xmlDoc.LoadXml(www.text.TrimStart()); // load the file.


            player = xmlDoc.GetElementsByTagName("player"); // array of the level nodes.
            setLeaderboard(player, playerList);
#endif
            loadmore++;
            StartCoroutine(showList(playerList));
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    public void setLeaderboard(XmlNodeList player, List<PlayerCont> List)
    {
        int i = 1;
        foreach (XmlNode playerInfo in player)
        {
            PlayerCont p = new PlayerCont();
#if NETFX_CORE

#else
            p.rank = i;
            p.name = playerInfo.SelectSingleNode("name").InnerText;
            p.icon = playerInfo.SelectSingleNode("icon").InnerText;
            p.goals = Convert.ToInt32(playerInfo.SelectSingleNode("goals").InnerText);
            p.time = Convert.ToInt32(playerInfo.SelectSingleNode("time").InnerText);
#endif
            Debug.Log("Here");
            Debug.Log(p.ToString()+'\n');
            List.Add(p);
            i++;
        }
    }

    public void setLeaderboardWindows(XmlNodeList player, List<PlayerCont> List)
    {
        int i = 1;
        foreach (XmlNode playerInfo in player)
        {
            PlayerCont p = new PlayerCont();
            p.rank = i;
#if NETFX_CORE

#else
            p.name = playerInfo.SelectSingleNode("name").InnerText;
            p.icon = playerInfo.SelectSingleNode("icon").InnerText;
            p.goals = Convert.ToInt32(playerInfo.SelectSingleNode("goals").InnerText);
            p.time = Convert.ToInt32(playerInfo.SelectSingleNode("time").InnerText);
#endif
            Debug.Log("Here");
            Debug.Log(p.ToString() + '\n');
            List.Add(p);
            i++;
        }
    }

    void clearCells()
    {
        //clear leaderboard player cells in vertical scroll
        foreach (Transform child in verticalLayout.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    IEnumerator showList(List<PlayerCont> List)
    {
        if (!loaded)
        {
            //visit each player in list
            foreach (PlayerCont p in List)
            {
                p.ToString();
                Debug.Log("here2");
                //create new cell and assign player info
                GameObject newCell = Instantiate(playerCellPrefab) as GameObject;
                newCell.transform.Find("Rank").GetComponent<Text>().text = p.rank.ToString();
                newCell.transform.Find("Name").GetComponent<Text>().text = p.name;
                newCell.transform.Find("Icon").GetComponent<TextMeshProUGUI>().text = p.icon;
                newCell.transform.Find("Goals").GetComponent<Text>().text = "Goals: " + p.goals.ToString();
                newCell.transform.Find("Time").GetComponent<Text>().text = p.time.ToString();

                //set child to fit box
                newCell.transform.SetParent(verticalLayout.gameObject.transform, false);
            }
        }

        loaded = true;
        yield return null;
    }

}
