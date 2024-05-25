using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class RoadManager : MonoBehaviour {
    [HideInInspector] public CarControllerAgent agent;
    public static RoadManager Instance;
    private List<EnemyCar> cars = new();

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    [SerializeField] private Transform laneParent;
    [SerializeField] private GameObject lanePrefab;
    [Space, SerializeField] private EnemyCar carPrefab;
    [SerializeField] private Transform carParent;

    /// <summary>
    /// Renders a Road
    /// </summary>
    /// <param name="lanes">The number of lanes the road should have</param>
    /// <returns>An array of the positions of the lanes</returns>
    public async Task<float[]> RenderRoad(int lanes) {
        // Set appropriate spacing (size) of the lanes
        // TODO: find formula
        GetComponent<VerticalLayoutGroup>().spacing = lanes switch {
            > 7 => 120,
            7 => 135,
            6 => 160,
            5 => 200,
            4 => 225,
            3 => 325,
            _ => 0
        };
        
        // Destroy children
        while (laneParent.childCount > 0) {
            DestroyImmediate(laneParent.GetChild(0));
        }

        for (int i = 0; i <= lanes; i++) {
            Instantiate(lanePrefab, laneParent);
        }

        // Wait for the children to be sorted by the LayoutGroup
        await Task.Delay(50);

        float[] positions = new float[lanes];
        for (int i = 0; i < positions.Length; i++) {
            float upper = laneParent.GetChild(i).position.y;
            float lower = laneParent.GetChild(i + 1).position.y;
            positions[i] = Mathf.Lerp(lower, upper, 0.5f);
        }

        return positions;
    }

    /// <summary>
    /// Spawn a car on the right edge of the screen
    /// </summary>
    /// <param name="yPos">The Y-Position of the spawned car (will never change)</param>
    /// <param name="speed">The speed (in Units/second) with which the car travels towards the left edge of the screen</param>
    public void SpawnCar(float yPos, float speed) {
        EnemyCar car = Instantiate(carPrefab, carParent);
        cars.Add(car);
        car.Init(GameObject.FindWithTag("Player"), yPos, speed);

        car.OnCollided += () => {
            agent.OnLost();
        };
    }

    public void ResetMl() {
        foreach (EnemyCar car in cars) {
            Destroy(car.gameObject);
        }

        cars = new List<EnemyCar>();
    }
}