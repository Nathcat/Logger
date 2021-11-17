using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;

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

    void Start() {
      Screen.orientation = ScreenOrientation.Portrait;
      shopMenu.SetActive(false);

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

    public void onPlayButtonClicked() {
      PlayerPrefs.SetString("NextScene", "Game");
      SceneManager.LoadScene("Ads");
    }

    public void onShopButtonClicked() {
      startMenu.SetActive(false);
      shopMenu.SetActive(true);
    }

    public void onBackToMenuButton(GameObject openMenu) {
      startMenu.SetActive(true);
      openMenu.SetActive(false);
    }

    IEnumerator getLeaderboard() {
      UnityWebRequest www = UnityWebRequest.Get("http://nathcat.cloudns.cl/get");
      yield return www.SendWebRequest();

      if (www.error == null) {
        string leaderboard = JsonUtility.FromJson<Leaderboard>(www.downloadHandler.text).message;
        leaderboardText.GetComponent<Text>().text = "-----Leaderboard-----\n" + leaderboard;

      } else {
        Debug.LogError(www.error);
        leaderboardText.GetComponent<Text>().text = "-----Leaderboard-----\nFailed to get the leaderboard :(";
      }
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

public class Leaderboard {
  public string message;
}
