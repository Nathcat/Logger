using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Game/Coin.cs
 *
 * @author Nathan "Nathcat" Baines
 */

public class Coin : MonoBehaviour
{
  private GameManager gameManager;
  private Rigidbody2D rb;
  public float rateOfRotation = 20f;  // Defines the rate at which the coin should rotate

  void Start() {
    gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    rb = gameObject.GetComponent<Rigidbody2D>();
  }

  void Update() {
    rb.velocity = Vector3.down * gameManager.treeMoveSpeed;  // Set the velocity of the Rigidbody
    transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y + (rateOfRotation * Time.deltaTime), 0f);  // Set the rotation of the coin

    if (transform.position.y <= -10f) {  // If the coin is below y -10
      Destroy(gameObject);  // Destroy this GameObject
    }
  }
}
