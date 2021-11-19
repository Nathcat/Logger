using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/*
 * Game/MultiplayerGameManager.cs
 *
 * @author Nathan "Nathcat" Baines
 */

public class MultiplayerGameManager : MonoBehaviour
{
  public GameObject[] treePrefabs;  // List of all prefabs of tree parts
  public float treeMoveSpeed = 10f;  // Defines the rate at which the tree should move
  public float originalTreeMoveSpeed;
  public int treesPassed = 0;  // The number of trees the player has passed
  public string serverHostname = "http://localhost:5000/";
  private string lobbyId;
  private int[] tree;

  void Start() {
    lobbyId = PlayerPrefs.GetString("lobbyId");

    originalTreeMoveSpeed = treeMoveSpeed;

    Screen.orientation = ScreenOrientation.Portrait;

    StartCoroutine(GetTree());
  }

  void Update() {
    if (treesPassed == 20) {
      treeMoveSpeed += 1f;
      originalTreeMoveSpeed += 1f;

      treesPassed = 0;
    }
  }

  IEnumerator GetTree() {
    UnityWebRequest www = UnityWebRequest.Get(serverHostname + lobbyId + "/tree");
    yield return www.SendWebRequest();

    if (www.error == null) {
      Tree data = JsonUtility.FromJson<Tree>(www.downloadHandler.text);

      tree = data.ParseTree();

      float y = 0f;
      for (int x = 0; x < tree.Length - 1; x++) {
        Quaternion rotation = new Quaternion();
        if (tree[x + 1] == 1) {
          rotation = new Quaternion(0f, 180f, 0f, 0f);
        }

        Instantiate(treePrefabs[tree[x]], new Vector3(0f, y, 0f), rotation);
        x++;
        y += 1.375f;
      }

    } else {
      Debug.LogError(www.error);
      SceneManager.LoadScene("Matchmaking");
    }
  }
}

public class Tree {
  public string tree;

  public int[] ParseTree() {
    string s = tree.Trim(new char[] {'[', ']'});

    string[] l = s.Split(',');

    int[] data = new int[l.Length];

    for (int x = 0; x < l.Length; x++) {
      data[x] = int.Parse(l[x]);
    }

    return data;
  }
}
