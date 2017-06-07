using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class goToManager : MonoBehaviour {
    public GameObject AllPlayersCont, teamCreateBG, exitButton;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void goToPlayerCreate()
    {
        SceneManager.LoadScene("PlayerCreate", LoadSceneMode.Additive);
    }

    public void goToTeamCreate()
    {
        SceneManager.LoadScene("TeamCreate", LoadSceneMode.Additive);
    }

    public void goToLeaderboard()
    {
        SceneManager.LoadScene("Leaderboard", LoadSceneMode.Additive);
    }

    public void closeLeaderboard()
    {
        SceneManager.UnloadSceneAsync("Leaderboard");
    }

    public void closePlayerCreate()
    {
        SceneManager.UnloadSceneAsync("PlayerCreate");
    }

    public void closeTeamCreate()
    {
        AllPlayersCont.SetActive(false);
        teamCreateBG.SetActive(false);
        exitButton.SetActive(false);
    }

    // SceneManager.LoadScene("Timer", LoadSceneMode.Additive);
}
