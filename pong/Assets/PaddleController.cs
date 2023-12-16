using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public bool isBlue;

    public string up;
    public string down;
    public KeyCode right;
    public KeyCode left;

    public float speed;

    void Update()
    {
        if (Input.GetKey(up))
        {
            transform.position += new Vector3(0f, speed * Time.deltaTime, 0f);
        }
        if (Input.GetKey(down))
        {
            transform.position -= new Vector3(0f, speed * Time.deltaTime, 0f);
        }
        if (Input.GetKey(left))
        {
            transform.position -= new Vector3((speed / 2) * Time.deltaTime, 0f, 0f);
        }
        if (Input.GetKey(right))
        {
            transform.position += new Vector3((speed / 2) * Time.deltaTime, 0f, 0f);
        }

        Transform ball = GameManager.Instance.ball.transform;
        if ((ball.position.x < 6 && ball.position.x > -6) && !GameManager.Instance.cursedMode)
        {
            Vector2 lookDir = ball.position - transform.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        if(!GameManager.Instance.cursedMode) transform.position = new Vector3(Mathf.Clamp(transform.position.x, isBlue ? -8 : 6, isBlue ? -6 : 8), Mathf.Clamp(transform.position.y, -4f, 4f), transform.position.z);
        else transform.position = new Vector3(Mathf.Clamp(transform.position.x, -9f, 9f), Mathf.Clamp(transform.position.y, -4f, 4f), transform.position.z);
    }
}
