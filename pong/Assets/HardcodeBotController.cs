using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardcodeBotController : MonoBehaviour
{
    public float speed; //0 = impossible

    void Update()
    {
        Transform ball = GameManager.Instance.ball.transform;

        if (speed == 0)
        {
            transform.position = new Vector2(transform.position.x, ball.position.y/* + (0 - ball.position.y) / 4.5f*/);
        }
        else
        {
            transform.position = new Vector2(transform.position.x, Mathf.MoveTowards(transform.position.y, ball.position.y, speed));
        }

        if (ball.position.x < 6 && ball.position.x > -6)
        {
            Vector2 lookDir = ball.position - transform.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        transform.position = new Vector2(transform.position.x, Mathf.Clamp(transform.position.y, -4f, 4f));
    }
}
