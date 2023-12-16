using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class GetToTarget : Agent
{
    private const float COLLISION_DISTANCE = 1f;
    private const int PLANE_LENGTH = 15;

    [SerializeField] private Transform player;
    [SerializeField] private Transform target;

    [Space, SerializeField] private MeshRenderer groundRenderer;

    [Space, SerializeField] private Material successfulMat;
    [SerializeField] private Material failedMat;

    [Space, SerializeField] private float speed;
    public override void OnEpisodeBegin()
    {
        RandomizePositions();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;

        int action = 0;

        if (Input.GetKey("w")) action = 1;
        else if (Input.GetKey("s")) action = 2;
        else if (Input.GetKey("d")) action = 3;
        else if (Input.GetKey("a")) action = 4;

        discreteActions[0] = action;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition.x);
        sensor.AddObservation(target.localPosition.z);

        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int move = actions.DiscreteActions[0];

        Vector2 moveV = move switch
        {
            0 => Vector2.zero,
            1 => new Vector2(1, 0),
            2 => new Vector2(-1, 0),
            3 => new Vector2(0, 1),
            4 => new Vector2(0, -1),

            _ => Vector2.zero
        };
        Move(moveV);
    }

    private void FixedUpdate()
    {
        float distance = Vector3.Distance(player.localPosition, target.localPosition);

        AddReward(-distance * 0.2f);

        bool outOfBounds = transform.localPosition.x < -PLANE_LENGTH + 1 || transform.localPosition.x > PLANE_LENGTH || 
                           transform.localPosition.z < -PLANE_LENGTH + 1 || transform.localPosition.z > PLANE_LENGTH;

        if (outOfBounds)
        {
            AddReward(-100f);
            groundRenderer.material = failedMat;
            EndEpisode();
        }
        if (Vector3.Distance(transform.localPosition, target.localPosition) < COLLISION_DISTANCE)
        {
            AddReward(100f);
            groundRenderer.material = successfulMat;
            EndEpisode();
        }
    }

    private void RandomizePositions()
    {
        player.localPosition = new Vector3(Random.Range(-PLANE_LENGTH + 1, PLANE_LENGTH - 1), 8f, Random.Range(-PLANE_LENGTH + 1, PLANE_LENGTH - 1));
        target.localPosition = new Vector3(Random.Range(-PLANE_LENGTH + 1, PLANE_LENGTH - 1), 8f, Random.Range(-PLANE_LENGTH + 1, PLANE_LENGTH - 1));
    }

    private void Move(Vector2 moveV)
    {
        transform.position += speed * Time.deltaTime * new Vector3(moveV.x, 0f, moveV.y);
    }
}