using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

/*
 * UI/MultiplayerGameUIController.cs
 *
 * @author Nathan "Nathcat" Baines
 */

public class MultiplayerGameUIController : MonoBehaviour
{
  public GameObject pauseMenu;
  public GameObject diedMenu;
  private PlayerController playerController;
  private MultiplayerGameManager gameManager;
  private int resetScore = 0;
  private bool countingDown;
  private bool lastAllowMove = true;
  private string lobbyId;
  private bool inLobby = true;
  public GameObject countdownText;
  private LobbyState lobbyState;
  public bool paused = false;

  void Start() {
    lobbyId = PlayerPrefs.GetString("lobbyId");
    playerController = GameObject.Find("Lumberjack").GetComponent<PlayerController>();
    gameManager = GameObject.Find("GameManager").GetComponent<MultiplayerGameManager>();

    pauseMenu.SetActive(false);
    diedMenu.SetActive(false);

    StartCoroutine(doCountdown());
  }

  void Update() {
    playerController.paused = paused;

    if (countingDown) {
      gameManager.treeMoveSpeed = 0f;
      playerController.score = resetScore;

    } else if (playerController.allowMove) {
      gameManager.treeMoveSpeed = gameManager.originalTreeMoveSpeed;
    }

    if (!playerController.allowMove) {
      gameManager.treeMoveSpeed = 0f;
      diedMenu.SetActive(true);

      if (lastAllowMove == true) {
        StartCoroutine(LeaveLobby());
      }

      StartCoroutine(GetLobbyState());
    }

    lastAllowMove = playerController.allowMove;
  }

  public void OpenPauseMenu() {
    pauseMenu.SetActive(true);
  }

  public void ClosePauseMenu() {
    pauseMenu.SetActive(false);
  }

  public void LeaveGame() {
    if (inLobby) {
      StartCoroutine(LeaveLobby());
    }

    SceneManager.LoadScene("Matchmaking");
  }

  void ShowPosition() {
    try {
      int.Parse(lobbyState.playersRemaining);

      string suffix = "";
      lobbyState.playersRemaining = (int.Parse(lobbyState.playersRemaining) + 1).ToString();
      char[] charArray = lobbyState.playersRemaining.ToCharArray();

      if (charArray[charArray.Length - 1] == '1') {
        suffix = "st";

      } else if (charArray[charArray.Length - 1] == '2') {
        suffix = "nd";

      } else if (charArray[charArray.Length - 1] == '3') {
        suffix = "rd";

      } else {
        suffix = "th";
      }

      playerController.gemsCollectedText.GetComponent<Text>().text = "You came " + lobbyState.playersRemaining + suffix;

    } catch (System.Exception e) {
      Debug.LogError(e.ToString());
      playerController.gemsCollectedText.GetComponent<Text>().text = "Something went wrong :(";
    }
  }

  IEnumerator LeaveLobby() {
    UnityWebRequest www = UnityWebRequest.Get(gameManager.serverHostname + lobbyId + "/leave");
    yield return www.SendWebRequest();

    if (www.error == null) {
      inLobby = false;

    } else {
      Debug.LogError(www.error);
    }
  }

  IEnumerator doCountdown() {
    countingDown = true;
    countdownText.SetActive(true);

    countdownText.GetComponent<Text>().text = "3";
    yield return new WaitForSeconds(1f);

    countdownText.GetComponent<Text>().text = "2";
    yield return new WaitForSeconds(1f);

    countdownText.GetComponent<Text>().text = "1";
    yield return new WaitForSeconds(1f);

    countdownText.GetComponent<Text>().text = "GO!";
    yield return new WaitForSeconds(1f);

    countdownText.SetActive(false);
    countingDown = false;
  }

  IEnumerator GetLobbyState() {
    lobbyState = null;

    UnityWebRequest www = UnityWebRequest.Get(gameManager.serverHostname + lobbyId + "/getstate");
    yield return www.SendWebRequest();

    if (www.error == null) {
      lobbyState = JsonUtility.FromJson<LobbyState>(www.downloadHandler.text);
      ShowPosition();

    } else {
      Debug.LogError(www.error);
      lobbyState = new LobbyState();
      lobbyState.playersRemaining = "Something went wrong :(";
    }
  }
}
