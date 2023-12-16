using UnityEngine.SceneManagement;
using UnityEngine;

public class InitBall : MonoBehaviour
{
    public float startSpeed = 70f;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Init();
    }
    public void Init()
    {
        bool goesLeft = Random.value < 0.5f;
        float horizontalVel = 0f;
        if (goesLeft) horizontalVel = -startSpeed;
        else horizontalVel = startSpeed;

        float verticalVel = Random.Range(-5f, 5f);

        rb.velocity = new Vector2(horizontalVel, verticalVel);
    }

    void Update()
    {
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -70f, 70f), Mathf.Clamp(rb.velocity.y, -70f, 70f));
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("PlayerPaddle")) GameManager.Instance.lastPlayerThatHitBall = 0;
        if (other.transform.CompareTag("BotPaddle")) GameManager.Instance.lastPlayerThatHitBall = 1;
    }
}
