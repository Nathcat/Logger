using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Networking;

/*
 * UI/GameUIController.cs
 *
 * @author Nathan "Nathcat" Baines
 */

public class GameUIController : MonoBehaviour
{
    private PlayerController playerController;
    private GameManager gameManager;
    public GameObject scoreText;
    public GameObject pauseMenu;
    public GameObject countdownText;
    public GameObject diedMenu;
    public Text usernameEntry;
    public bool paused = false;
    private bool countingDown = true;
    private int resetScore = 0;

    void Start() {
      playerController = GameObject.Find("Lumberjack").GetComponent<PlayerController>();
      gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

      StartCoroutine(doCountdown());
    }

    void Update() {
      if (countingDown) {
        gameManager.treeMoveSpeed = 0f;
        playerController.score = resetScore;

      } else if (playerController.allowMove) {
        gameManager.treeMoveSpeed = gameManager.originalTreeMoveSpeed;
      }

      scoreText.GetComponent<Text>().text = playerController.score + "";

      diedMenu.SetActive(!playerController.allowMove);

      if (Input.GetKeyDown(KeyCode.Escape) && playerController.allowMove) {
        paused = !paused;
      }

      if (paused) {
        Time.timeScale = 0f;

      } else {
        Time.timeScale = 1f;
      }

      pauseMenu.SetActive(paused);
    }

    public void resumeButtonClicked() {
      paused = false;
    }

    public void quitButtonClicked() {
      PlayerPrefs.SetString("NextScene", "MainMenu");
      SceneManager.LoadScene("Ads");
    }

    public void pauseButtonClicked() {
      if (playerController.allowMove) {
        paused = !paused;

      } else {
        paused = false;
      }
    }

    public void restartButtonClicked() {
      SceneManager.LoadScene("Game");
    }

    public void continueButtonClicked() {
      int totalGems = int.Parse(read(Path.Combine(Application.persistentDataPath, "number_of_gems.txt")));

      if (totalGems >= 20) {
        totalGems -= 20;
        write(Path.Combine(Application.persistentDataPath, "number_of_gems.txt"), totalGems.ToString());
        playerController.allowMove = true;
        resetScore = playerController.score;
        StartCoroutine(doCountdown());
        StartCoroutine(playerController.scoreTimer());
        gameManager.treeMoveSpeed = gameManager.originalTreeMoveSpeed;

      } else {
        playerController.gemsCollectedText.GetComponent<Text>().text = "You don't have enough gems!";
      }
    }

    public void SubmitScoreButtonClicked() {
      StartCoroutine(SubmitScore());
    }

    IEnumerator SubmitScore() {
      UnityWebRequest www = UnityWebRequest.Get("http://nathcat.cloudns.cl/add/" + usernameEntry.text + "/" + playerController.score);
      yield return www.SendWebRequest();

      if (www.error == null) {
        playerController.gemsCollectedText.GetComponent<Text>().text = "Score submitted!";
        
      } else {
        Debug.LogError(www.error);
        playerController.gemsCollectedText.GetComponent<Text>().text = "Something went wrong :(";
      }
    }

    IEnumerator doCountdown() {
      Time.timeScale = 0f;
      countingDown = true;
      countdownText.SetActive(true);

      countdownText.GetComponent<Text>().text = "3";
      yield return new WaitForSeconds(1f);

      countdownText.GetComponent<Text>().text = "2";
      yield return new WaitForSeconds(1f);

      countdownText.GetComponent<Text>().text = "1";
      yield return new WaitForSeconds(1f);

      countdownText.GetComponent<Text>().text = "GO!";
      Time.timeScale = 1f;
      yield return new WaitForSeconds(1f);

      countdownText.SetActive(false);
      countingDown = false;
    }

    string read(string path) {
      StreamReader reader = new StreamReader(path);
      string content = reader.ReadToEnd();
      reader.Close();
      return content;
    }

    void write(string path, string content) {
      StreamWriter writer = new StreamWriter(path);
      writer.WriteLine(content);
      writer.Close();
    }
}
