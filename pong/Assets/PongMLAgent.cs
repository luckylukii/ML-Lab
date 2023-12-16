using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PongMLAgent : Agent
{
    public bool isBlue = true;
    [Space]
    public GameManager manager;
    [Space]
    public float speed;
    [Space]
    public Transform ball;

    public override void OnEpisodeBegin()
    {
        manager.ResetSceneMLAgent();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector2)transform.position);
        sensor.AddObservation((Vector2)ball.position);
        sensor.AddObservation((Vector2)ball.GetComponent<Rigidbody2D>().velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int upDown = actions.DiscreteActions[0];
        int leftRight = actions.DiscreteActions[1];

        if (upDown == 2) upDown = -1;
        if (leftRight == 2) leftRight = -1;

        transform.position += new Vector3(leftRight * Time.deltaTime * speed / 2, upDown * Time.deltaTime * speed, 0f);

        //Clamping
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, isBlue ? -8 : 6, isBlue ? -6 : 8), 
            Mathf.Clamp(transform.position.y, -4f, 4f), transform.position.z);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;

        discreteActions[0] = Mathf.RoundToInt(Input.GetAxisRaw("Vertical") * speed);
        discreteActions[1] = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal") * speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ball"))
            AddReward(100f);
    }
    void FixedUpdate()
    {
        if (ball.position.magnitude > 100f) EndEpisode(); //failsafe if ball breaks out

        var rewardByDistance = 10 - Mathf.Abs(transform.position.y - ball.position.y);
        AddReward(rewardByDistance);
    }

    public void Reward(int ammt)
    {
        AddReward(ammt);
        EndEpisode();
    }
}