using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
    private string playerName, icon, location;
    float time;
    int goals;
    public GameObject nameInput;
    public TextMeshProUGUI iconInput;
    public GameObject hint1, hint2;
    public Text output;
    
	// Use this for initialization
	void Start () {
        playerName = "";
        output.text = "Enter a user name and Emoji";
    }
	
	// Update is called once per frame
	void Update () {
        //If the input field is focused, change its color to green.
        if (nameInput.GetComponent<InputField>().isFocused == true)
        {
           hint1.SetActive(true);
           output.text = "Letters, Numbers, & Underscores (11 max)";
        }
        else
        {
           hint1.SetActive(false);
            output.text = "Enter a user name and Emoji";
        }

        //if (iconInput.GetComponent<InputField>().isFocused == true)
        //{
        //    hint1.SetActive(true);
        //    output.text = "Emoji here";
        //}
        //else
        //{
        //    hint1.SetActive(false);
        //    output.text = "Enter a user name and Emoji";
        //}

    }

    public void createPlayer()
    {
        playerName = nameInput.GetComponentInChildren<Text>().text;
        icon = iconInput.text;
        location = "00000000";

        Debug.Log("icon test: " + icon);

        string url = "localhost/indoor/createPlayer.php"; // player creation script
        //string url = "192.168.200.0/indoor/createPlayer.php"; // player creation script
        // need to validate input
        // send input to script
        if (isValid())
        {
            var form = new WWWForm();
            form.AddField("name", playerName);
            form.AddField("icon", icon);
            form.AddField("location", location);
            //send form to register.php
            WWW send = new WWW(url, form);
            StartCoroutine(WaitForRequest(send));
        }
    }

    bool isValid()
    {
        //if player name is valid
        return true;
            //if icon is valid

        //else
       // return false;
    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            string text = www.text.Substring(www.text.Length - 1, 1);
            //createPlayer.php returns 0,1,2 based on whether create successful 
            Debug.Log(text);
            if (text == "0")
            {
                Debug.Log("Creation successful");
                SceneManager.UnloadSceneAsync("PlayerCreate");
            }

            if (text == "1")
            {
                Debug.Log("Emoji already taken for location.");
            }
            Debug.Log("Connection good.");
        }
        else
        {
            Debug.Log("Connection error.");
        }
    }
}
