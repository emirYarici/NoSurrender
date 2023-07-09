using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI peopleLeftText;
    public GameObject startScreen;
    public GameObject InGameScreen;
    public TextMeshProUGUI startTimerText;
    public float startTimer;
    public bool startCount = false;
    public GameObject resumeButton;
    public GameObject stopButton;
    public GameObject winScreen;
    public GameObject loseScreen;

    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
   
    }

    public void OnStartButtonClicked()
    {
        startScreen.SetActive(false);
        InGameScreen.SetActive(true);
        GameManager.Instance.gameStarted= true;
    }

    public void OnStopButtonClicked()
    {
        Time.timeScale = 0;
        resumeButton.SetActive(true);
        stopButton.SetActive(false);
    }

    public void OnResumeButtonClicked()
    {
        Time.timeScale = 1;
        resumeButton.SetActive(false);
        stopButton.SetActive(true);
    }

    public void OnNextButtonClicked()
    {
        SceneManager.LoadScene(0);
    }

}
