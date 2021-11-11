using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
  private GameManager gameManager;
  private Rigidbody2D rb;
  public float rateOfRotation = 20f;

  void Start() {
    gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    rb = gameObject.GetComponent<Rigidbody2D>();
  }

  void Update() {
    rb.velocity = Vector3.down * gameManager.treeMoveSpeed;
    transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y + (rateOfRotation * Time.deltaTime), 0f);

    if (transform.position.y <= -10f) {
      Destroy(gameObject);
    }
  }
}
