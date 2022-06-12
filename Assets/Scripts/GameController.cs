using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject prefabZombie;
    [SerializeField] GameObject prefabGunner;
    [SerializeField] GameObject prefabCivilian;

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] TextMeshProUGUI timeText;

    string playerName;

    bool scoreSaved = false;
    int score = 0;
    int wave = 0;
    TimeSpan timer;
    float startTime;
    int minute = 0;

    bool openMenu = false;
    bool gameOver = false;

    int[] spawnNumUnits = new int[2]; // Gunners, Civilians
    int numZombies;

    // Start is called before the first frame update
    void Start()
    {
        //probe = GameObject.Find("SpawnProbe").GetComponent<SpawnProbe>();

        scoreText.text = "" + score;
        waveText.text = "" + wave;
        
        // Spawn first zombie
        Vector3 pos = new Vector3(0, 15, 0);
        GameObject instantiator = Instantiate<GameObject>(prefabZombie, pos, Quaternion.identity);
        instantiator.name ="zombie_" + UnityEngine.Random.Range(0, 10000);

        numZombies = 1;

        startTime = Time.time;

        UpdateTime();
        UpdateWave();

    }

    public void Restart()
    {
        SceneManager.LoadScene("Level");
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenu");
    }


    void SetPrefs(string n, int s, string t, int i)
    {
        PlayerPrefs.SetString("name_" + i, n);
        PlayerPrefs.SetInt("score_" + i, s);
        PlayerPrefs.SetString("time_" + i, t);
    }
    public void SaveScore()
    {
        playerName = GameObject.Find("NameInputField").GetComponent<TMP_InputField>().text;

        if (!scoreSaved && !(playerName.Length == 0))
        {
            string nameCheck;
            int scoreCheck;
            string timeCheck;

            //PlayerPrefs.SetInt("score_0", score);
            //PlayerPrefs.SetString("name_0", playerName);

            for (int i = 9; i >= 0; i--)
            {
                if (PlayerPrefs.HasKey("name_" + i))
                {
                    nameCheck = PlayerPrefs.GetString("name_" + i);
                    scoreCheck = PlayerPrefs.GetInt("score_" + i);
                    timeCheck = PlayerPrefs.GetString("time_" + i);

                    if (score >= scoreCheck)
                    {
                        SetPrefs(nameCheck, scoreCheck, timeCheck, i + 1);

                        SetPrefs(playerName, score, timeText.text, i);
                    }
                    else
                    {
                        SetPrefs(playerName, score, timeText.text, i + 1);
                    }
                }
                else if (i == 0 && !PlayerPrefs.HasKey("name_" + i))
                {
                    SetPrefs(playerName, score, timeText.text, i);
                }
            }
            PlayerPrefs.Save();

            scoreSaved = true;
            GameObject.Find("SaveConfirmation").GetComponent<TextMeshProUGUI>().text = "Score Saved";
        }
        else
        {
            GameObject.Find("SaveConfirmation").GetComponent<TextMeshProUGUI>().text = "Name Can Not Empty";
        }
    }

    void Spawner(GameObject prefab, int n, string name)
    {
        for (int i = 0; i < n; i++)
        {
            Vector3 pos = new Vector3(UnityEngine.Random.Range(-39, 48), UnityEngine.Random.Range(-40, 9), 0);
            GameObject instantiator = Instantiate<GameObject>(prefab, pos, Quaternion.identity);
            instantiator.name = name + "_" + UnityEngine.Random.Range(0, 10000);
        }
    }

    // Spawns the Gunners and Civilians (Number of units spawned depends on wave number)
    void SpawnUnits()
    {
        int counter = 0;
        foreach (int num in spawnNumUnits)
        {
            if (counter == 0) // spawn Gunners
            {
                Spawner(prefabGunner, num, "gunner");
            }
            else if (counter == 1) // spawn Civilian
            {
                Spawner(prefabCivilian, num, "civilian");
            }
            counter++;
        }
    }

    public void UpdateScore(int sc, string type)
    {
        score += sc;
        scoreText.text = "" + score;
        if (type.Equals("Zombie"))
        {
            numZombies--;
        }
        else
        {
            numZombies++;
        }

        // Game end condition
        if (numZombies <= 0)
        {
            gameOver = true;

            GameObject.Find("CloseButton").SetActive(false);
            GameObject.Find("ScoreCanvas").GetComponent<Canvas>().enabled = gameOver;
            GameObject.Find("MenuText").GetComponent<TextMeshProUGUI>().text = "Game Over";

            OpenMenuInvert();
        }
    }

    void UpdateWave()
    {
        spawnNumUnits[0] = (wave * 5);
        spawnNumUnits[1] = (5 - wave);
        wave++;
        waveText.text = "" + wave;

        SpawnUnits();
    }

    void UpdateTime()
    {
        timeText.text = string.Format("{0:D2}:{1:D2}", timer.Minutes, timer.Seconds);

        if (timer.Minutes > minute)
        {
            UpdateWave();
            minute = timer.Minutes;
        }
    }

    public void OpenMenuInvert()
    {
        openMenu = !openMenu;
        GameObject.Find("InGameMenu").GetComponent<Canvas>().enabled = openMenu;
    }

    // Update is called once per frame
    void Update()
    {   
        if (!gameOver)
        {
            timer = TimeSpan.FromSeconds(Time.time - startTime);
            UpdateTime();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OpenMenuInvert();
            }
        }
    }
}
