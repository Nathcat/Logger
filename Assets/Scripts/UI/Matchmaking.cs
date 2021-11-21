using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
 * UI/Matchmaking.cs
 *
 * @author Nathan "Nathcat" Baines
 */

public class Matchmaking : MonoBehaviour
{
    private string lobbyId = "";
    public string serverHostname = "http://localhost:5000/";
    public GameObject text;
    private string error = "";
    private bool inLobby = false;
    private LobbyState lobbyState;
    private bool gettingState = false;

    void Update() {
      if (inLobby && !gettingState) {
        StartCoroutine(GetLobbyState());
      }
    }

    IEnumerator GetNewLobby() {
      if (lobbyId != "") {
        Debug.LogError("ERROR: You are already in a lobby!");
        yield break;
      }

      UnityWebRequest www = UnityWebRequest.Get(serverHostname);
      yield return www.SendWebRequest();

      if (www.error == null) {
        lobbyId = www.downloadHandler.text;
        StartCoroutine(JoinLobby());

      } else {
        Debug.LogError(www.error);
        text.GetComponent<Text>().text = "Something went wrong :(";
        error = www.error.ToString();
      }
    }

    IEnumerator JoinLobby() {
      if (lobbyId == "") {
        Debug.LogError("ERROR: No lobby id.");
        yield break;
      }

      UnityWebRequest www = UnityWebRequest.Get(serverHostname + lobbyId + "/join");
      yield return www.SendWebRequest();

      if (www.error == null) {
        inLobby = true;

      } else {
        Debug.LogError(www.error);
        text.GetComponent<Text>().text = "Something went wrong :(";
        error = www.error.ToString();
      }
    }

    IEnumerator LeaveLobby() {
      if (lobbyId == "") {
        Debug.LogError("ERROR: You are not in a lobby!");
        text.GetComponent<Text>().text = "You're not in a lobby!";
        yield break;
      }

      UnityWebRequest www = UnityWebRequest.Get(serverHostname + lobbyId + "/leave");
      yield return www.SendWebRequest();

      if (www.error == null) {
        text.GetComponent<Text>().text = "You have left the lobby.";
        lobbyId = "";
        inLobby = false;

      } else {
        Debug.LogError(www.error);
        text.GetComponent<Text>().text = "Something went wrong :(";
        error = www.error.ToString();
      }
    }

    IEnumerator GetLobbyState() {
      if (lobbyId == "") {
        Debug.LogError("ERROR: You are not in a lobby!");
        yield break;
      }

      gettingState = true;
      UnityWebRequest www = UnityWebRequest.Get(serverHostname + lobbyId + "/getstate");
      yield return www.SendWebRequest();

      if (www.error == null && inLobby) {
        lobbyState = JsonUtility.FromJson<LobbyState>(www.downloadHandler.text);
        text.GetComponent<Text>().text = (3 - int.Parse(lobbyState.playersRemaining)) + " players needed to start the game.";

        if (lobbyState.playing == "yes") {
          PlayerPrefs.SetString("lobbyId", lobbyId);
          SceneManager.LoadScene("Multiplayer");
        }

      } else {
        if (inLobby) {
          Debug.LogError(www.error);
          text.GetComponent<Text>().text = "Something went wrong :(";
          error = www.error.ToString();
        }
      }

      gettingState = false;
    }

    public void JoinGame() {
      StartCoroutine(GetNewLobby());
    }

    public void LeaveGame() {
      StartCoroutine(LeaveLobby());
    }

    public void BackToMainMenu() {
      if (inLobby) {
        LeaveGame();
      }

      SceneManager.LoadScene("MainMenu");
    }
}


class LobbyState {
  public string playing;
  public string playersRemaining;
}
