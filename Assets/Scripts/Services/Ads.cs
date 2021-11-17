using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using System.IO;

/*
 * Services/Ads.cs
 *
 * @author Nathan "Nathcat" Baines
 */

public class Ads : MonoBehaviour, IUnityAdsListener
{
  private string gameId = "4434885";
  private string unitId = "Interstitial_Android";
  private bool allowAd = true;

  void Start() {
    Advertisement.AddListener(this);
    Advertisement.Initialize(gameId, false);

    if (read(Path.Combine(Application.persistentDataPath, "noads.txt")).Equals("true\n")) {
      allowAd = false;
    }
  }

  void Update() {
    if (Advertisement.IsReady()) {
      if (SceneManager.GetActiveScene().name == "Ads" && allowAd && !Advertisement.isShowing) {
        Advertisement.Show(unitId);
        allowAd = false;
      }
    }

    if (SceneManager.GetActiveScene().name == "Ads" && !allowAd) {
      SceneManager.LoadScene(PlayerPrefs.GetString("NextScene"));
    }
  }

  public void OnUnityAdsReady (string placementId) {

  }

  public void OnUnityAdsDidFinish (string placementId, ShowResult showResult) {
    SceneManager.LoadScene(PlayerPrefs.GetString("NextScene"));
    allowAd = true;
  }

  public void OnUnityAdsDidError (string message) {
    Debug.LogError(message);
  }

  public void OnUnityAdsDidStart (string placementId) {

  }

  string read(string path) {
    StreamReader reader = new StreamReader(path);
    string content = reader.ReadToEnd();
    reader.Close();
    return content;
  }
}
