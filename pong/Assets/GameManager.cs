using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton Reference

    public static GameManager Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        if (Instance != this)
        {
            Destroy(gameObject);
        }

        Cursor.lockState = CursorLockMode.Confined;
    }

    #endregion

    [SerializeField] private GameObject paddle;
    [SerializeField] private GameObject playerPaddle;
    [SerializeField] private GameObject mlPaddle;
    public GameObject ball;

    [SerializeField] private TMP_Text controls;

    public bool cursedMode = false;

    public enum PlayingAgainst
    {
        Player,
        HardcodeEasy,
        HardcodeNormal,
        HardcodeImpossible,
        MLBot
    }
    public PlayingAgainst redMode;
    public PlayingAgainst blueMode;

    void Start()
    {
        UpdateEnemy(paddle);
        UpdateEnemy(playerPaddle);
    }

    [HideInInspector] public bool canPlayer1Boost = true;
    [HideInInspector] public bool canPlayer2Boost = true;

    [HideInInspector] public int lastPlayerThatHitBall = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            blueMode = PlayingAgainst.Player;
            UpdateEnemy(playerPaddle);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            blueMode = PlayingAgainst.HardcodeEasy;
            UpdateEnemy(playerPaddle);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            blueMode = PlayingAgainst.HardcodeNormal;
            UpdateEnemy(playerPaddle);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            blueMode = PlayingAgainst.HardcodeImpossible;
            UpdateEnemy(playerPaddle);
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            redMode = PlayingAgainst.Player;
            UpdateEnemy(paddle);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            redMode = PlayingAgainst.HardcodeEasy;
            UpdateEnemy(paddle);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            redMode = PlayingAgainst.HardcodeNormal;
            UpdateEnemy(paddle);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            redMode = PlayingAgainst.HardcodeImpossible;
            UpdateEnemy(paddle);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetScene();
            UpdateEnemy(paddle);
            UpdateEnemy(playerPaddle);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canPlayer1Boost && lastPlayerThatHitBall == 0 && blueMode == PlayingAgainst.Player)
        {
            canPlayer1Boost = false;
            ball.GetComponent<Rigidbody2D>().velocity += ball.GetComponent<Rigidbody2D>().velocity.normalized * 3f;
            ball.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        }
        if (Input.GetKeyDown(KeyCode.RightShift) && canPlayer2Boost && lastPlayerThatHitBall == 1 && redMode == PlayingAgainst.Player)
        {
            canPlayer2Boost = false;
            ball.GetComponent<Rigidbody2D>().velocity += ball.GetComponent<Rigidbody2D>().velocity.normalized * 3f;
            ball.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            playerPaddle.GetComponent<PaddleController>().speed += 0.5f;
            paddle.GetComponent<PaddleController>().speed += 0.5f;
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            playerPaddle.GetComponent<PaddleController>().speed -= 0.5f;
            paddle.GetComponent<PaddleController>().speed -= 0.5f;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            cursedMode = !cursedMode;
            
            if(cursedMode)
            {
                paddle.transform.rotation = Quaternion.identity;
                playerPaddle.transform.rotation = Quaternion.identity;
            }
        }
    }

    private void UpdateEnemy(GameObject paddle)
    {
        if (paddle == this.paddle)
        {
            Debug.Log("Now playing against " + redMode.ToString());
            switch (redMode)
            {
                case PlayingAgainst.Player:
                    paddle.GetComponent<HardcodeBotController>().enabled = false;
                    paddle.GetComponent<PaddleController>().enabled = true;

                    controls.text = "Controls\nPlayer1   Player2\n    w/s       up/down";
                    break;


                case PlayingAgainst.HardcodeEasy:
                    paddle.GetComponent<HardcodeBotController>().enabled = true;
                    paddle.GetComponent<HardcodeBotController>().speed = 0.0085f;

                    paddle.GetComponent<PaddleController>().enabled = false;

                    controls.text = "Controls\nw/s";
                    break;


                case PlayingAgainst.HardcodeNormal:
                    paddle.GetComponent<HardcodeBotController>().enabled = true;
                    paddle.GetComponent<HardcodeBotController>().speed = 0.02f;

                    paddle.GetComponent<PaddleController>().enabled = false;

                    controls.text = "Controls\nw/s";
                    break;


                case PlayingAgainst.HardcodeImpossible:
                    paddle.GetComponent<HardcodeBotController>().enabled = true;
                    paddle.GetComponent<HardcodeBotController>().speed = 0f; //0 = impossibleMode

                    paddle.GetComponent<PaddleController>().enabled = false;

                    controls.text = "Controls\nw/s";
                    break;


                case PlayingAgainst.MLBot:
                    Debug.LogError("MLBot not available");

                    controls.text = "Controls\nw/s";
                    break;
            }
        }
        else
        {
            Debug.Log("Now playing against " + blueMode.ToString());
            switch (blueMode)
            {
                case PlayingAgainst.Player:
                    paddle.GetComponent<HardcodeBotController>().enabled = false;
                    paddle.GetComponent<PaddleController>().enabled = true;

                    controls.text = "Controls\nPlayer1   Player2\n    w/s       up/down";
                    break;


                case PlayingAgainst.HardcodeEasy:
                    paddle.GetComponent<HardcodeBotController>().enabled = true;
                    paddle.GetComponent<HardcodeBotController>().speed = 0.0085f;

                    paddle.GetComponent<PaddleController>().enabled = false;

                    controls.text = "Controls\nw/s";
                    break;


                case PlayingAgainst.HardcodeNormal:
                    paddle.GetComponent<HardcodeBotController>().enabled = true;
                    paddle.GetComponent<HardcodeBotController>().speed = 0.02f;

                    paddle.GetComponent<PaddleController>().enabled = false;

                    controls.text = "Controls\nw/s";
                    break;


                case PlayingAgainst.HardcodeImpossible:
                    paddle.GetComponent<HardcodeBotController>().enabled = true;
                    paddle.GetComponent<HardcodeBotController>().speed = 0f; //0 = impossibleMode

                    paddle.GetComponent<PaddleController>().enabled = false;

                    controls.text = "Controls\nw/s";
                    break;


                case PlayingAgainst.MLBot:
                    Debug.LogError("MLBot not available");

                    controls.text = "Controls\nw/s";
                    break;
            }
        }
        
    }

    public void ResetScene()
    {
        playerPaddle.transform.position = new Vector2(-7f, 0f);
        paddle.transform.position = new Vector2(7f, 0f);

        ball.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();

        canPlayer1Boost = true;
        canPlayer2Boost = true;

        ball.transform.position = Vector3.zero;
        ball.GetComponent<InitBall>().Init();
    }
    public void ResetSceneMLAgent()
    {
        mlPaddle.transform.position = new Vector2(-7, Random.Range(-4f, 4f));
        paddle.transform.position = new Vector2(7, 0);

        ball.transform.position = Vector3.zero;
        ball.GetComponent<InitBall>().Init();
    }
}