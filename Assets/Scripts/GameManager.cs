using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public enum GameState { GS_GAME, GS_PAUSE_MENU, GS_LEVEL_COMPLETED , GS_OPTIONS}

public class GameManager : MonoBehaviour
{
    public GameState currentGameState = GameState.GS_GAME;
    public static GameManager instance;
    public Canvas inGameCanvas;
    public TMP_Text scoreText;
    public TMP_Text finalScoreText;
    public TMP_Text timeText;
    public TMP_Text enemiesKilledText;
    public TMP_Text highScoreText;
    public TMP_Text qualityText;
    public TMP_Text gameInfoText;
    public TMP_Text framesPerSecondText;
    public TMP_Text completionTimeText;
    public TMP_Text bestCompletionTimeText;
    public GameObject gameInfoPanel;
    public Image[] keysTab;
    public Image[] livesTab;
    public Canvas pauseMenuCanvas;
    public Canvas levelCompletedCanvas;
    public Canvas optionsCanvas;
    public Grid grid;

    const string keyHighScore = "HighScore"; // add level name
    const string keyBestTime = "BestTime";   // (currentLevel.name string)

    private int lives = 3;
    private int score = 0;
    private int keysFound = 0;
    private int enemiesKilled = 0;
    private float timer = 0.0f;
    private bool canEndLevel = false;
    private int framesPerSecond = 0;
    private int framesCounter = 0;
    private float framesCounterResetTime = 0.0f;
    private Scene currentLevel;

    void Awake()
    {
        currentLevel = SceneManager.GetActiveScene();
        framesPerSecondText.text = "0 FPS";
        gameInfoPanel.SetActive(false);
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Duplicated Game Manager", gameObject);
        }

        for(int i = 0; i < keysTab.Length; i++)
        {
            keysTab[i].color = new Color(0.1f, 0.1f, 0.1f, 1f);
        }

        scoreText.text = score.ToString();

        for(int i = 0; i < livesTab.Length - lives; i++)
        {
            livesTab[livesTab.Length - i - 1].enabled = false;
        }

        if(! PlayerPrefs.HasKey(keyHighScore + currentLevel.name))
        {
            PlayerPrefs.SetInt(keyHighScore + currentLevel.name, 0);
            PlayerPrefs.Save();
        }

        if(! PlayerPrefs.HasKey(keyBestTime + currentLevel.name))
        {
            PlayerPrefs.SetFloat(keyBestTime + currentLevel.name, timer);
            PlayerPrefs.Save();
        }

        InGame();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Can end level" + canEndLevel);
        if(canEndLevel && Input.GetKeyDown(KeyCode.E))
        {
            EndLevel();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(currentGameState == GameState.GS_GAME)
            {
                PauseMenu();
            }
            else if (currentGameState == GameState.GS_PAUSE_MENU)
            {
                InGame();
            }
        }
        framesCounter++;
        framesCounterResetTime += Time.deltaTime;
        if(framesCounterResetTime >= 1)
        {
            framesPerSecond = (int)(framesCounter/framesCounterResetTime);
            framesCounter = 0;
            framesCounterResetTime = 0;
            framesPerSecondText.text = framesPerSecond.ToString() + " FPS";
        }
        timer += Time.deltaTime;
        int seconds = (int)timer % 60;
        int minutes = (int)timer / 60;

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void AddPoints(int points)
    {
        Debug.Log("Points: " + score);
        score += points;
        scoreText.text = score.ToString();
    }

    public void AddKey()
    {
        keysTab[keysFound%keysTab.Length].color = Color.white;
        keysFound++;
    }

    public int GetKeysFoundNumber()
    {
        return keysFound;
    }

    public void GiveLive()
    {
        livesTab[lives % livesTab.Length].enabled = true;
        lives++;
    }

    public void DecreaseLive()
    {
        if (lives <= livesTab.Length)
        {
            livesTab[lives-1].enabled = false;
        }
        lives--;
        if(lives == 0)
        {
            GameOver();
        }
    }

    public int GetLivesNumber()
    {
        return lives;
    }

    public void EnemiesKilledNumberInc()
    {
        enemiesKilled++;
        enemiesKilledText.text = enemiesKilled.ToString();
    }

    public void OnResumeButtonClicked()
    {
        InGame();
    }

    public void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(currentLevel.name);
    }

    public void OnReturnToMainMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnOptionsButtonClicked()
    {
        Options();
    }

    void SetGameState(GameState newGameState)
    {
        currentGameState = newGameState;

        levelCompletedCanvas.enabled = (currentGameState == GameState.GS_LEVEL_COMPLETED);
        inGameCanvas.enabled = (currentGameState == GameState.GS_GAME);
        pauseMenuCanvas.enabled = (currentGameState == GameState.GS_PAUSE_MENU);
        if (optionsCanvas.enabled = (currentGameState == GameState.GS_OPTIONS))
        {
            SetQualitySettingName();
        }

        if (currentGameState == GameState.GS_LEVEL_COMPLETED)
        {            
            int highScore = PlayerPrefs.GetInt(keyHighScore + currentLevel.name);
            float bestTime = PlayerPrefs.GetFloat(keyBestTime + currentLevel.name);

            if(highScore < score)
            {
                highScore = score;
                PlayerPrefs.SetInt(keyHighScore + currentLevel.name, highScore);
                PlayerPrefs.Save();
            }
            finalScoreText.text = "Score: " + score;
            highScoreText.text = "The best score: " + highScore;

            if(bestTime > timer || bestTime == 0)
            {
                bestTime = timer;
                PlayerPrefs.SetFloat(keyBestTime + currentLevel.name, bestTime);
                PlayerPrefs.Save();
            }
            int seconds = (int)timer % 60;
            int minutes = (int)timer / 60;
            completionTimeText.text = string.Format("Your time: {0:00}:{1:00}", minutes, seconds);

            seconds = (int)bestTime % 60;
            minutes = (int)bestTime / 60;
            bestCompletionTimeText.text = string.Format("Best time: {0:00}:{1:00}", minutes, seconds);
        }

        if (currentGameState != GameState.GS_GAME)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void PauseMenu()
    {
        SetGameState(GameState.GS_PAUSE_MENU);
    }
    public void InGame()
    {
        SetGameState(GameState.GS_GAME);
    }
    public void LevelCompleted()
    {
        SetGameState(GameState.GS_LEVEL_COMPLETED);
    }
    public void GameOver()
    {
        SceneManager.LoadScene(currentLevel.name);
    }
    public void Options()
    {
        SetGameState(GameState.GS_OPTIONS);
    }

    public void OnQualityMinusButtonClicked()
    {
        QualitySettings.DecreaseLevel();
        SetQualitySettingName();
    }

    public void OnQualityPlusButtonClicked()
    {
        QualitySettings.IncreaseLevel();
        SetQualitySettingName();
    }

    public void SetQualitySettingName()
    {
        qualityText.text = QualitySettings.names[QualitySettings.GetQualityLevel()];
    }

    public void SetVolume(float vol)
    {
        AudioListener.volume = vol;
        //Debug.Log("Volume set to: " + vol);
    }

    public void ExitInfo()
    {
        if (GetKeysFoundNumber() == keysTab.Length)
        {
            gameInfoText.text = "Press E to end level";
            gameInfoPanel.SetActive(true);
            canEndLevel = true;
        }
        else
        {
            gameInfoText.text = "You are missing " + (keysTab.Length - GetKeysFoundNumber()) + " keys!";
            gameInfoPanel.SetActive(true);
            //Debug.Log("Brakuj¹ce klucze: " + (GameManager.instance.keysTab.Length - GameManager.instance.GetKeysFoundNumber()));
        }
    }

    public void DisableExitInfo()
    {
        gameInfoPanel.SetActive(false);
    }

    public void EndLevel()
    {
        AddPoints(100 * GetLivesNumber());
        LevelCompleted();
        Debug.Log("Wygrana!");
    }

    public void DisableCanEndLevel()
    {
        canEndLevel = false;
    }
}
