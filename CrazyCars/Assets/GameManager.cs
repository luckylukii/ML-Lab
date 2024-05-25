using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private CarControllerAgent agent;
    private const float SPAWN_DELAY = 1f;

    [SerializeField] private int numLanes = 5;
    [Space, SerializeField] private RectTransform player;
    [SerializeField] private TMP_Text scoreText;
    public static float[] LanePositions;

    private int _currentPosition;

    private bool _initialized = false;

    private float _spawnTimer = 0f;
    public int score = 0;

    private async void Start() {
        RoadManager.Instance.agent = agent;
        
        LanePositions = await RoadManager.Instance.RenderRoad(numLanes);
        Debug.Log(string.Join(", ", LanePositions));
        _currentPosition = LanePositions.Length / 2;

        _initialized = true;
    }

    private async void Update() {
        while (!_initialized) {
            await Task.Yield();
        }

        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0 && RoadManager.Instance != null) {
            SpawnCars(40f, score + 400);
            agent.Reward(score);
            _spawnTimer = SPAWN_DELAY;
            score += 10;
            scoreText.text = $"Score: {score}";
        }

        int input = 0;
        if (Input.GetKeyDown(KeyCode.W)) input = -1;
        if (Input.GetKeyDown(KeyCode.S)) input = 1;
        MoveObject(player, input, ref _currentPosition);
    }

    public static void MoveObject(Transform obj, int input, ref int startingYPos) {
        startingYPos = Mathf.Clamp(startingYPos + input, 0, LanePositions.Length - 1);
        obj.position = new Vector2(obj.position.x, LanePositions[startingYPos]);
    }

    // PercentPerLane: The probability (%) that a car spawns on each lane
    private void SpawnCars(float percentPerLane, float speed) {
        int numSpawned = 0;
        foreach (float pos in LanePositions) {
            bool spawns = Random.Range(0, 100) <= percentPerLane;
            
            if (!spawns || numSpawned >= LanePositions.Length - 1) continue;
            
            RoadManager.Instance.SpawnCar(pos, speed);
            numSpawned++;
        }
    }

    public void ResetScore() => score = 0;
}