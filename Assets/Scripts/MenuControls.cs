using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;


public class MenuControls : MonoBehaviour
{
    public void ChangeScene(string name)
    {
        // Change scene argument recieved
        SceneManager.LoadScene(name);
    }

    public void ChangeMenuScreen(string to)
    {
        // Change scene argument recieved

        string from = EventSystem.current.currentSelectedGameObject.name;

        //Debug.Log("to " + GameObject.Find(to).GetComponent<Canvas>().enabled);
        GameObject.Find(to).GetComponent<Canvas>().enabled = true;

        //Debug.Log(GameObject.Find(from).transform.parent.gameObject.name + " closed");
        GameObject.Find(from).transform.parent.gameObject.GetComponent<Canvas>().enabled = false;
    }

    public void LoadScores()
    {
        string[] names = new string[10];
        int[] scores = new int[10];
        string[] times = new string[10];


        for (int i = 0; i < 10; i++)
        {
            if (PlayerPrefs.HasKey("name_" + i))
            {
                names[i] = PlayerPrefs.GetString("name_" + i);
                scores[i] = PlayerPrefs.GetInt("score_" + i);
                times[i] = PlayerPrefs.GetString("time_" + i);

                GameObject.Find("Score_" + i).GetComponent<TextMeshProUGUI>().text = names[i] + ", " + scores[i] + ", " + times[i];
            }
        }
    }
}
