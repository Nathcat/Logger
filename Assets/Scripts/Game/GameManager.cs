using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] treePrefabs;
    public GameObject coin;
    public GameObject gem;
    public float treeMoveSpeed = 10f;
    public int numberOfTrees = 100;
    public float originalTreeMoveSpeed;
    public int treesPassed = 0;
    private int lastIndex;

    void Start() {
      originalTreeMoveSpeed = treeMoveSpeed;

      Screen.orientation = ScreenOrientation.Portrait;

      generateTree();
    }

    void Update() {
      if (Physics2D.OverlapBox(new Vector2(0f, 7f), new Vector2(0.5f, 0.5f), 0f) == null) {
        generateTree();
      }

      if (treesPassed == 20) {
        treeMoveSpeed += 1f;
        originalTreeMoveSpeed += 1f;

        treesPassed = 0;
      }
    }

    GameObject randomTree(Vector3 position) {
      int index = Random.Range(0, treePrefabs.Length);

      if (lastIndex != null) {
        if (lastIndex == 1) {
          index = 0;
        }
      }

      if (index != 1 && Random.Range(0, 101) >= 75) {
        float x = Random.Range(-1, 2) * 1.5f;
        if (x == 0) {
          x = 1.5f;
        }

        Instantiate(coin, position + new Vector3(x, 0f, 0f), new Quaternion());

      } else if (index != 1 && Random.Range(0, 101) >= 99) {
        float x = Random.Range(-1, 2) * 1.5f;
        if (x == 0) {
          x = 1.5f;
        }

        Instantiate(gem, position + new Vector3(x, 0f, 0f), new Quaternion());
      }

      lastIndex = index;

      return treePrefabs[index];
    }

    void generateTree() {
      int treesSpawned = 0;
      float y = 7;

      while (treesSpawned <= numberOfTrees) {
        Instantiate(randomTree(new Vector3(0f, y, 0f)), new Vector3(0f, y, 0f), new Quaternion());
        y += 1.375f;
        treesSpawned++;
      }
    }
}
