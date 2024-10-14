using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using System;

/*
 * UI/MainMenuController.cs
 *
 * @author Nathan "Nathcat" Baines
 */

public class MainMenuController : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject gemsText;
    public GameObject leaderboardText;
    public GameObject shopMenu;
    public GameObject gameModeMenu;

    public static string leaderboardURL = "https://logger-leaderboard.nathcat.net";

    void Start() {
      Screen.orientation = ScreenOrientation.Portrait;
      shopMenu.SetActive(false);
      gameModeMenu.SetActive(false);

      StartCoroutine(getLeaderboard());

      try {
        read(Path.Combine(Application.persistentDataPath, "noads.txt"));

      } catch (System.Exception e) {
        Debug.LogError(e.ToString());
        write(Path.Combine(Application.persistentDataPath, "noads.txt"), "false");
      }

      try {
        gemsText.GetComponent<Text>().text = read(Path.Combine(Application.persistentDataPath, "number_of_gems.txt"));

      } catch (System.Exception e) {
        Debug.LogError(e.ToString());
        write(Path.Combine(Application.persistentDataPath, "number_of_gems.txt"), "0");
        gemsText.GetComponent<Text>().text = read(Path.Combine(Application.persistentDataPath, "number_of_gems.txt"));
      }
    }

    public void onSinglePlayerButtonClicked() {
      PlayerPrefs.SetString("NextScene", "Game");
      SceneManager.LoadScene("Game");
    }

    public void onShopButtonClicked() {
      startMenu.SetActive(false);
      shopMenu.SetActive(true);
    }

    public void onBackToMenuButton(GameObject openMenu) {
      startMenu.SetActive(true);
      openMenu.SetActive(false);
    }

    public void gameModeMenuButtonClicked() {
      gameModeMenu.SetActive(true);
      startMenu.SetActive(false);
    }

    public void onMultiplayerButtonClicked() {
      PlayerPrefs.SetString("NextScene", "Matchmaking");
      SceneManager.LoadScene("Ads");
    }

    IEnumerator getLeaderboard() {
      string url = leaderboardURL + "/getTop5";
      
      UnityWebRequest www = UnityWebRequest.Get(url);
      yield return www.SendWebRequest();

      string leaderboard = "";
      switch (www.result) {
        case UnityWebRequest.Result.Success:
          leaderboard = www.downloadHandler.text;
          break;
        default:
          Debug.LogError(www.error);
          Debug.LogError(www.downloadHandler.text);
          leaderboard = "An error occurred.";
          break;
      }

      www.Dispose();
      leaderboardText.GetComponent<Text>().text = "-----Leaderboard-----\n" + leaderboard;
      yield break;
    }

    private string GetLeaderboardFromLocalFile() {
      string path = Path.Combine(Application.persistentDataPath, "local-leaderboard.txt");

      if (!File.Exists(path)) {
        write(path, "");
      }

      string[] content = read(path).Split("\n");
      if (content[0] == "") return "";
      
      PlayerScoreRecord[] record = new PlayerScoreRecord[content.Length-1];
      for (int i = 0; i < content.Length-1; i++) {
        string[] row = content[i].Split("\t");

        if (row.Length != 2) continue;

        PlayerScoreRecord r = new PlayerScoreRecord();
        r.name = row[0];
        r.score = Int32.Parse(row[1]);
        record[i] = r;
      }

      bool emptyPass = false;
      while (!emptyPass) {
        emptyPass = true;
        for (int i = 0; i < record.Length-1; i++) {
          if (record[i+1].score > record[i].score) {
            PlayerScoreRecord tmp = record[i];
            record[i] = record[i+1];
            record[i+1] = tmp;
            emptyPass = false;
          }
        }
      }

      string leaderboard = "";
      for (int i = 0; i < record.Length; i++) {
        leaderboard += record[i].name + " - " + record[i].score + "\n";
      }

      return leaderboard;
    }

    public static string read(string path) {
      StreamReader reader = new StreamReader(path);
      string content = reader.ReadToEnd();
      reader.Close();
      return content;
    }

    public static void write(string path, string content) {
      StreamWriter writer = new StreamWriter(path);
      writer.Write(content);
      writer.Close();
    }
}

public class Leaderboard {
  public string message;
}
