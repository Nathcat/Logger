using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/*
 * Game/PlayerControllers.cs
 *
 * @author Nathan "Nathcat" Baines
 */

public class PlayerController : MonoBehaviour
{
    public Vector3 rightSide;  // Right side of the tree
    public Vector3 leftSide;  // Left side of the tree
    private SpriteRenderer renderer;
    private GameManager gameManager;
    private GameUIController uiController;
    public bool allowMove = true;  // Should the player be allowed to move
    public int score = 0;
    public ParticleSystem treeCutParticles;  // Particle system which is played when a tree piece is cut
    public ParticleSystem gemParticles;  // Particle system which is played when a gem is collected
    private int gemsCollected = 0;  // The number of gems collected by the player
    public GameObject gemsCollectedText;
    public AudioSource cutAudio;  // The AudioSource which is played when the player cuts a part of the tree

    void Start() {
      renderer = gameObject.GetComponent<SpriteRenderer>();
      gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
      uiController = GameObject.Find("Canvas").GetComponent<GameUIController>();
      cutAudio = GameObject.Find("CutAudioClip").GetComponent<AudioSource>();
      StartCoroutine(scoreTimer());
    }

    void Update() {
      // PC/Mac/Editor controls
      if (Input.GetKeyDown(KeyCode.RightArrow) && transform.position != rightSide && allowMove && !uiController.paused) {
        transform.position = rightSide;
        renderer.flipX = true;
        cutTree();

      } else if (Input.GetKeyDown(KeyCode.LeftArrow) && transform.position != leftSide && allowMove && !uiController.paused) {
        transform.position = leftSide;
        renderer.flipX = false;
        cutTree();
      }

      // Android controls
      if (Input.touchCount > 0) {
        Touch touch = Input.GetTouch(0);

        if (touch.position.x > Screen.width / 2 && transform.position != rightSide && allowMove && !uiController.paused) {
          transform.position = rightSide;
          renderer.flipX = true;
          cutTree();

        } else if (touch.position.x < Screen.width / 2 && transform.position != leftSide && allowMove && !uiController.paused) {
          transform.position = leftSide;
          renderer.flipX = false;
          cutTree();
        }
      }
    }

    void OnTriggerEnter2D(Collider2D other) {
      if (other.transform.gameObject.CompareTag("Coin")) {
        score += 100;
        Destroy(other.transform.gameObject);

      } else if (other.transform.gameObject.CompareTag("Gem")) {
        gemsCollected++;
        gemParticles.transform.position = other.transform.position;
        gemParticles.Play();
        Destroy(other.transform.gameObject);

      } else {
        gameManager.treeMoveSpeed = 0f;
        allowMove = false;

        gemsCollectedText.GetComponent<Text>().text = $"You collected {gemsCollected} gems, you now have {int.Parse(read(Path.Combine(Application.persistentDataPath, "number_of_gems.txt"))) + gemsCollected}";
        write(Path.Combine(Application.persistentDataPath, "number_of_gems.txt"), (int.Parse(read(Path.Combine(Application.persistentDataPath, "number_of_gems.txt"))) + gemsCollected).ToString());
      }
    }

    public IEnumerator scoreTimer() {
      while (allowMove) {
        yield return new WaitForSeconds(0.1f);
        score++;
      }
    }

    void cutTree() {
      Collider2D treeA = Physics2D.OverlapBox(new Vector2(0f, transform.position.y), new Vector2(0.5f, 0.5f), 0f);
      Collider2D treeB = Physics2D.OverlapBox(new Vector2(0f, transform.position.y - 0.75f), new Vector2(0.5f, 0.5f), 0f);

      if (treeA != null) {
        Destroy(treeA.transform.gameObject);
        treeCutParticles.Play();
      }

      if (treeB != null) {
        Destroy(treeB.transform.gameObject);
        treeCutParticles.Play();
      }

      if (treeA != null || treeB != null) {
        cutAudio.Play();
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
