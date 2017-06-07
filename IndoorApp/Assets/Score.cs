using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Score : MonoBehaviour {
    public Text StartText, Output;
    public TextMeshProUGUI score;
    public String teamNum;
    public AudioSource goaloso1, goaloso2, goaloso3; 
    private int pointsToWin = 3;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void increaseScore()
    {
        GameObject scoreScript = GameObject.Find("Main Camera");
        GameObject canvas = GameObject.Find("Canvas");

        Text outputText = scoreScript.GetComponent<Score>().Output;
        //if time is not stopped
        if (scoreScript.GetComponent<Score>().StartText.text == "Stop")
        {
            TextMeshProUGUI scoreText;
            scoreText = canvas.transform.Find("Background").transform.Find("Score").transform.Find("Score1").GetComponent<TextMeshProUGUI>();
            if (teamNum == "2")
            {
                scoreText = canvas.transform.Find("Background").transform.Find("Score").transform.Find("Score2").GetComponent<TextMeshProUGUI>();
            }
            // If max goals not meet then increase score
            if (Convert.ToInt32(scoreText.text) < pointsToWin)
            {
                
                scoreText.text = (Convert.ToInt32(scoreText.text) + 1).ToString();
                outputText.text = "Team " + teamNum + " scored!";
                scoreScript.GetComponent<Score>().goaloso1.Play();
                StartCoroutine(FadeTextToZeroAlpha(1f, outputText));
                //send player goal to db
                string playername = "s";
                playername = gameObject.transform.Find("Name").GetComponent<Text>().text;
                Debug.Log(playername);
                StartCoroutine(UpdatePlayerScore(playername));
            }
            // Else do nothing
        }
        else
        {
            outputText.text = "Time is stopped.";
            StartCoroutine(FadeTextToZeroAlpha(1f, outputText));
        }
    }

    public IEnumerator FadeTextToZeroAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / 5*t));
            yield return null;
        }
    }

    public IEnumerator UpdatePlayerScore(string playerName)
    {
        //create form
        Debug.Log("Update Player Goals called");
        string url = "localhost/indoor/updateScore.php"; //script that handles score update
        var form = new WWWForm();

        //vars to send to script
        form.AddField("name", playerName);

        //send form
        WWW update = new WWW(url, form);

        yield return update;
        if (update.error == null) //connection is good and string recieved from server
        {
            //Debug.Log("Connection good.");
            string text = Regex.Replace(update.text, @"\s", ""); //strip www.text of any whitespace
            //Debug.Log(text);
            if (text == "0")
            {
                //successful
                Debug.Log("Score update successful");
            }
            if (text == "1")
            {
                //unsuccessful
                Debug.Log("Score update unsuccessful");
            }
        }
        else
        {
            Debug.Log("Connection error.");
        }
    }
}
