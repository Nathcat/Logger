using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTrunk : MonoBehaviour
{
    private GameManager gameManager;
    private Rigidbody2D rb;

    void Start() {
      gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
      rb = gameObject.GetComponent<Rigidbody2D>();

      if (Random.Range(0, 2) == 1) {
        transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
      }
    }

    void Update() {
      rb.velocity = Vector3.down * gameManager.treeMoveSpeed;

      if (transform.position.y <= -10f) {
        gameManager.treesPassed++;
        Destroy(gameObject);
      }
    }
}
