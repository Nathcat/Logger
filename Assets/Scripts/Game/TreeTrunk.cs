using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Game/TreeTrunk.cs
 *
 * @author Nathan "Nathcat" Baines
 */

public class TreeTrunk : MonoBehaviour
{
    private GameManager gameManager;
    private MultiplayerGameManager multiplayerGameManager;
    private Rigidbody2D rb;

    void Start() {
      gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
      multiplayerGameManager = GameObject.Find("GameManager").GetComponent<MultiplayerGameManager>();
      rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update() {
      try {
        rb.velocity = Vector3.down * gameManager.treeMoveSpeed;

        if (transform.position.y <= -10f) {
          gameManager.treesPassed++;
          Destroy(gameObject);
        }

      } catch (System.Exception e) {
        rb.velocity = Vector3.down * multiplayerGameManager.treeMoveSpeed;

        if (transform.position.y <= -10f) {
          multiplayerGameManager.treesPassed++;
          Destroy(gameObject);
        }
      }
    }
}
