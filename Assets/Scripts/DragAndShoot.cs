using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class DragAndShoot : MonoBehaviour
{
    private Vector3 touchStartPos;
    private Vector3 touchEndPos;
    private Rigidbody rb;
    private bool isShoot;
    private Vector3 initialPosition;
    private GameObject basketballInstance;

    [SerializeField] private float forceMultiplier = 3f;
    [SerializeField] private Transform hoopTransform; // Reference to the hoop's Transform
    [SerializeField] private float respawnDelay = 1f; // Delay before respawning the ball
    [SerializeField] private GameObject basketballPrefab; // Reference to the basketball prefab
    [SerializeField] private TMP_Text scoreText; // UI Text to display the score
    [SerializeField] private TMP_Text timerText; // UI Text to display the timer

    private int score;
    private float timer = 60f; // 60 seconds game timer

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        UpdateScore(0);
        UpdateTimerText();
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTimerText();
        }
        else
        {
            timer = 0;
            EndGame();
        }

        if (transform.position.y < -10f && !isShoot)
        {
            RespawnBall();
        }

        // Handle touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                touchEndPos = touch.position;
                Shoot(touchEndPos - touchStartPos);
            }
        }
    }

    void Shoot(Vector3 force)
    {
        if (isShoot)
            return;

        // Get the Rigidbody component if it's null (e.g., first shot)
        if (rb == null)
        {
            rb = basketballInstance.GetComponent<Rigidbody>();
        }

        // Make the Rigidbody non-kinematic so it can respond to forces
        rb.isKinematic = false;

        // Add force to the ball
        rb.AddForce(new Vector3(force.x, force.y, force.y) * forceMultiplier);

        // Mark as shot
        isShoot = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Hoop"))
        {
            UpdateScore(1);
            Invoke("RespawnBall", respawnDelay);
        }
    }

    void UpdateScore(int points)
    {
        score += points;
        scoreText.text = "Score: " + score.ToString();
    }

    void UpdateTimerText()
    {
        timerText.text = "Time: " + Mathf.Round(timer).ToString();
    }

    void RespawnBall()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = initialPosition;
        isShoot = false;
    }

    void EndGame()
    {
        Debug.Log("Game Over! Final Score: " + score);
        // You can add more actions here like displaying a game over UI or resetting the game.
    }
}
