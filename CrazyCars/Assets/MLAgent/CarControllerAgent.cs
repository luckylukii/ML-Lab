using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CarControllerAgent : Agent {
    [SerializeField] private GameManager manager;
    private int position = 0;
    public override void CollectObservations(VectorSensor sensor) {
        // Observations through CameraSensor
    }

    public override void OnActionReceived(ActionBuffers actions) {
        if (!GameManager.Initialized) return;
        
        ActionSegment<int> discreteActions = actions.DiscreteActions;
        int input = discreteActions[0];
        
        position = Mathf.Clamp(position + input, 0, GameManager.LanePositions.Length - 1);
        Debug.Log($"Inp: {input}; Pos: {position}");
        transform.position = new Vector2(transform.position.x, GameManager.LanePositions[position]);
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        int input = 0;
        if (Input.GetKeyDown(KeyCode.W)) input = -1;
        if (Input.GetKeyDown(KeyCode.S)) input = 1;
        
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = input;
    }

    public override void OnEpisodeBegin() {
        RoadManager.Instance.ResetMl();
        manager.ResetScore();
    }

    public void OnLost() => EndEpisode();
    public void Reward(float r) => AddReward(r);
}
