using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using System.IO;

public class MainMenuController : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject gemsText;
    public GameObject shopMenu;

    void Start() {
      Screen.orientation = ScreenOrientation.Portrait;
      shopMenu.SetActive(false);

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

    public void onBackToMenuButton() {
      startMenu.SetActive(true);
      shopMenu.SetActive(false);
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
