using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameInfo: MonoBehaviour
{
    public TMP_Text tempText;
    public float enemyInterval;
    public Transform enemyPrefab;
    public Transform resurectionPrefab;

    static int score = 0;
    static int lives = 5;
    static TMP_Text scoreText;
    float enemyTimer;
    

    void Start()
    {
        scoreText = tempText;
        scoreText.text = "Score: " + score;
        enemyTimer = enemyInterval;
    }

    static public void IncScore(int _score)
    {
        score += _score;
        scoreText.text = "Score: " + score;
    }

    private void Update()
    {
        enemyTimer -= Time.deltaTime;
        if(enemyTimer < 0)
        {
            CreateEnemy();
        }
        //Debug.Log(1/Time.deltaTime);
    }


    void CreateEnemy()
    {
        List<Vector2Int> rooms = MapGeneration.rooms;
        int index = new System.Random().Next(0, rooms.Count);
        Vector2Int tile = rooms[index];
        Vector3 enemyPosition = Helper._2dToWorld(tile);
        Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
        Vector3 resPosition = new Vector3(enemyPosition.x, 0f, enemyPosition.z);
        Instantiate(resurectionPrefab, resPosition, Quaternion.LookRotation(Vector3.up));

        enemyTimer = enemyInterval;
    }
}
