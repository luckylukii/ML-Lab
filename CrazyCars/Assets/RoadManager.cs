using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RoadManager : MonoBehaviour {
    public static RoadManager Instance;
    private List<RectTransform> cars = new();

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    [SerializeField] private Transform laneParent;
    [SerializeField] private GameObject lanePrefab;
    [Space, SerializeField] private RectTransform carPrefab;
    [SerializeField] private Transform carParent;

    /// <summary>
    /// Renders a Road
    /// </summary>
    /// <param name="lanes">The number of lanes the road should have</param>
    /// <returns>An array of the positions of the lanes</returns>
    public async Task<float[]> RenderRoad(int lanes) {
        // Destroy children
        while (laneParent.childCount > 0) {
            DestroyImmediate(laneParent.GetChild(0));
        }

        for (int i = 0; i < lanes; i++) {
            Instantiate(lanePrefab, laneParent);
        }

        // Wait for the children to be sorted by the LayoutGroup
        await Task.Delay(50);

        float[] positions = new float[lanes];
        for (int i = 0; i < positions.Length; i++) {
            positions[i] = laneParent.GetChild(i).position.y;
        }

        return positions;
    }

    public async void SpawnCar(float yPos, float speed) {
        RectTransform car = Instantiate(carPrefab, carParent);
        cars.Add(car);
        car.anchoredPosition = new Vector2(Screen.width / 2, yPos);
        while (car.position.x > -Screen.width / 2) {
            car.position = new Vector3(Mathf.MoveTowards(car.position.x, -Screen.width, speed * Time.deltaTime), yPos);


            Vector2 player = GameObject.FindWithTag("Player").transform.position;
            if (Math.Abs(player.x - car.position.x) <= 50 && player.y == car.position.y) {
                Debug.Log("Lost");
                break;
            }


            await Task.Yield();
        }

        cars.Remove(car);
        Destroy(car.gameObject);
    }

    public void ResetMl() {
        foreach (RectTransform car in cars) {
            Destroy(car.gameObject);
        }

        cars = new();
    }
}