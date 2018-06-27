using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class uiManager : MonoBehaviour {

    public GameObject playbtn, pauseBtn, leftBtn, rightBtn, jumpBtn, resumeBtn, quitBtn, shareBtn, replayBtn, points, bestPoints;

    public static uiManager instance;

    // Use this for initialization
    void Start() {
        if (instance = null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public void Play()
    {
        pauseBtn.SetActive(true);
        leftBtn.SetActive(true);
        rightBtn.SetActive(true);
        jumpBtn.SetActive(true);

        shareBtn.SetActive(false);
        replayBtn.SetActive(false);
        playbtn.SetActive(false);
        quitBtn.SetActive(false);
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        pauseBtn.SetActive(false);
        leftBtn.SetActive(false);
        rightBtn.SetActive(false);
        jumpBtn.SetActive(false);
        resumeBtn.SetActive(true);
        quitBtn.SetActive(true);
    }

    public void Resume()
    {
        pauseBtn.SetActive(true);
        leftBtn.SetActive(true);
        rightBtn.SetActive(true);
        jumpBtn.SetActive(true);
        Time.timeScale = 1f;
        resumeBtn.SetActive(false);
        quitBtn.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void GameOver()
    {
        points.SetActive(true);
        bestPoints.SetActive(true);

        pauseBtn.SetActive(false);
        leftBtn.SetActive(false);
        rightBtn.SetActive(false);
        jumpBtn.SetActive(false);

        shareBtn.SetActive(true);
        replayBtn.SetActive(true);
        quitBtn.SetActive(true);
    }

    public void Replay()
    {
        SceneManager.LoadScene("GameScene");
    }
}
