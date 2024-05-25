using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrazyCars : MonoBehaviour {
    [SerializeField] private TMP_Text scoreText;
    private static string fileName = Application.persistentDataPath + "Scores.txt";

    private List<int> scores = LoadScores().ToList();

    private static IEnumerable<int> LoadScores() {
        return File.ReadAllLines(fileName).Select(int.Parse);
    }

    private static void SaveScores(IEnumerable<int> scores) {
        File.WriteAllLines(fileName, scores.Select(c => c.ToString()));
    }

    /// <summary>
    /// The initializer function that Renders the road and starts the game
    /// </summary>
    private async void Init() {
        await RoadManager.Instance.RenderRoad(5);
        int score = MoveLine();
        scores.Add(score);
        scores.Sort();

        SaveScores(scores);
    }

    /// <summary>
    /// The function that moves the car
    /// </summary>
    /// <param name="pos">The position of the car as a vector</param>
    /// <returns></returns>
    private Vector2Int HandleCarInput(Vector2Int pos) {
        //Console.SetCursorPosition(xPos, yPos);
        Console.Write("🚘");
        if (!Input.anyKey) return pos;
        
        if (Input.GetKey(KeyCode.UpArrow) && pos.y - 4 > 0) {
            // Overwrite the car
            Console.SetCursorPosition(pos.x, pos.y);
            Console.Write(" ");
            pos.y -= 4;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && pos.y + 4 < 20) {
            // Overwrite the car
            Console.SetCursorPosition(pos.x, pos.y);
            Console.Write(" ");
            pos.y += 4;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && pos.x - 3 > 0) {
            // Overwrite the car
            Console.SetCursorPosition(pos.x, pos.y);
            Console.Write(" ");
            pos.x -= 3;
        }
        else if (Input.GetKey(KeyCode.RightArrow) && pos.x + 3 < 80) {
            // Overwrite the car
            Console.SetCursorPosition(pos.x, pos.y);
            Console.Write(" ");
            pos.x += 3;
        }

        return pos;
    }

    /// <summary>
    /// The main function to run the game
    /// </summary>
    /// <returns>The score</returns>
    private int MoveLine() {
        int delay = 110;
        int xPos;
        int yPos;
        int repetition = 0;
        Vector2Int posCar = Vector2Int.zero;
        int carsOnScreen = 0;
        int[,] positions = new int [25, 4];
        bool carHit = false;
        int score = -999;
        int maxGhostDrivers = 10;
        while (!carHit) {
            posCar = HandleCarInput(posCar);
            (carsOnScreen, maxGhostDrivers) =
                GhostDriver(ref positions, carsOnScreen, score, maxGhostDrivers);
            carHit = Score(ref score, positions, posCar.x, posCar.y, ref delay);
            if (score < 0) {
                score = 0;
            }

            for (yPos = 4; yPos < 17; yPos += 4) {
                for (xPos = 0; xPos < 81; xPos += 10) {
                    if (xPos - 1 - repetition >= 0) {
                        Console.SetCursorPosition(xPos - 1 - repetition, yPos);
                        Console.Write("━");
                    }

                    Console.SetCursorPosition(xPos + 3 - repetition, yPos);
                    Console.Write(" ");
                }
            }

            scoreText.text = $"Score: {score}";
            Thread.Sleep(delay);
            if (++repetition == 4) {
                repetition = -7;
            }
        }

        return score;
    }

    private (int, int) GhostDriver(ref int[,] positions, int carsOnScreen, int score, int maxGhostDrivers) {
        string[] possibleGhostRiders = { "🚗", "🚕", "🚐", "🚚" };
        int ghostDriver = Random.Range(0, 4);
        if (score % 20 == 0) {
            if (score != 0) {
                maxGhostDrivers++;
            }

            if (maxGhostDrivers > 25) {
                maxGhostDrivers = 25;
            }
        }

        if (Random.Range(0, 101) >= 90 - (score / 7) && carsOnScreen < maxGhostDrivers) {
            int posY = 2 + Random.Range(0, 5) * 4;
            Console.SetCursorPosition(79, posY);
            Console.Write(possibleGhostRiders[ghostDriver]);
            positions[carsOnScreen, 0] = 79;
            positions[carsOnScreen, 1] = posY;
            positions[carsOnScreen, 2] = ghostDriver;
            positions[carsOnScreen, 3] = 0;
            carsOnScreen++;
        }

        for (int i = 0; i < carsOnScreen && positions[i, 0] >= 0; i++) {
            Console.SetCursorPosition(positions[i, 0], positions[i, 1]);
            Console.Write(" ");
            positions[i, 0]--;
            if (positions[i, 0] == -1) {
                carsOnScreen--;
                for (int j = 0; j < positions.GetLength(0); j++) {
                    if (j < positions.GetLength(0) - 1) {
                        positions[j, 0] = positions[j + 1, 0];
                        positions[j, 1] = positions[j + 1, 1];
                        positions[j, 2] = positions[j + 1, 2];
                        positions[j, 3] = positions[j + 1, 3];
                    }
                    else {
                        positions[j, 0] = 0;
                    }
                }
            }

            Console.SetCursorPosition(positions[i, 0], positions[i, 1]);
            Console.Write(possibleGhostRiders[positions[i, 2]]);
        }

        return (carsOnScreen, maxGhostDrivers);
    }

    /// <summary>
    /// Updates the score and difficulty and tells you if the game is over or not
    /// </summary>
    /// <param name="score"></param>
    /// <param name="positions"></param>
    /// <param name="xCar"></param>
    /// <param name="yCar"></param>
    /// <param name="delay"></param>
    /// <returns>If the car got hit</returns>
    private bool Score(ref int score, int[,] positions, int xCar, int yCar, ref int delay) {
        for (int i = 0; i < positions.GetLength(0); i++) {
            if (positions[i, 0] == xCar && positions[i, 1] == yCar) {
                return true;
            }

            if (positions[i, 0] >= xCar || positions[i, 3] != 0) continue;

            score++;
            if (score % 20 == 0) {
                delay = (int)(delay / 1.15);
                if (delay <= 5) {
                    delay = 5;
                }
            }

            positions[i, 3] = 1;
        }

        return false;
    }
}