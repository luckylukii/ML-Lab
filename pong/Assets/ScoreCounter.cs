using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreCounter : MonoBehaviour
{
    public TMP_Text counter;
    public int player;
    int score;
    [Space]
    public GameManager manager;

    public bool goodForAgent;
    public PongMLAgent agent;

    void OnCollisionEnter2D(Collision2D collision)
    {
        score++;
        counter.text = score.ToString();

        GameManager.Instance.ResetSceneMLAgent();

        GameManager.Instance.canPlayer1Boost = true;
        GameManager.Instance.canPlayer2Boost = true;

        
        if (goodForAgent)
        {
            agent.Reward(1000);
        }
        else
        {
            agent.Reward(-1000);
        }
        
    }
}