using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameInfo: MonoBehaviour
{
    public TMP_Text tempText;
    public float enemyInterval;
    public Transform enemyPrefab;
    public Transform resurectionPrefab;
    public GameObject hpBar;
    public float minimumDirectionInterval;
    public float maximumDirectionInterval;
    public float minimumEnemySpeed;
    public float maximumEnemySpeed;

    static public float animationTime;
    static public float div;
    static int score = 0;
    static int lifes;
    static TMP_Text scoreText;
    float enemyTimer;
    int enemiesSpawned;
    static Sprite[] sprites;
    static Image hpImage;
    static GameObject loseModal;
    static float hpAnimation;
    

    void Start()
    {
        animationTime = 0.1f;
        div = 10f / animationTime;
        hpAnimation = -animationTime - 1f;
        enemiesSpawned = 0;
        lifes = 5;
        score = 0;
        scoreText = tempText;
        scoreText.text = "Score: " + score;
        enemyTimer = enemyInterval;
        sprites = Resources.LoadAll<Sprite>("hp2");
        hpImage = hpBar.GetComponent<Image>();
        loseModal = GameObject.FindGameObjectWithTag("lost");
        loseModal.SetActive(false);
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

        if(hpAnimation > -animationTime)
        {
            Color color = hpImage.color;
            hpAnimation -= Time.deltaTime;
            color.a = 0.45f + Mathf.Pow(div * hpAnimation, 2)/285f;
            hpImage.color = color;
            if(hpAnimation <= -animationTime)
            {
                color.a = 0.8f;
                hpImage.color = color;
                hpAnimation = -animationTime - 1f;
            }
        }
        //Debug.Log(1/Time.deltaTime)
    }


    void CreateEnemy()
    {
        List<Vector2Int> rooms = MapGeneration.rooms;
        int index = new System.Random().Next(0, rooms.Count);
        Vector2Int tile = rooms[index];
        Vector3 enemyPosition = Helper._2dToWorld(tile);
        Transform enemy = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);

        enemy.gameObject.GetComponent<Enemy>().directionOffset = Mathf.Max (minimumDirectionInterval, maximumDirectionInterval - enemiesSpawned * 0.1f);
        //Debug.Log(Mathf.Exp((enemiesSpawned - 50f) / 25f));
        enemy.gameObject.GetComponent<Enemy>().speed = Mathf.Min(minimumEnemySpeed + Mathf.Exp((enemiesSpawned - 50f) / 25f)*3f, maximumEnemySpeed);

        Vector3 resPosition = new Vector3(enemyPosition.x, 0f, enemyPosition.z);
        Instantiate(resurectionPrefab, resPosition, Quaternion.LookRotation(Vector3.up));

        enemyTimer = enemyInterval;
        enemiesSpawned++;
    }

    static public void PlayerShot()
    {
        lifes = Mathf.Max(0, lifes-1);
        hpImage.sprite = sprites[lifes];
        if(lifes == 0)
        {
            LostGame();
        }
        hpAnimation = animationTime;
    }

    public static void LostGame()
    {
        Time.timeScale = 0;
        loseModal.SetActive(true);
        GameObject.FindGameObjectWithTag("score").GetComponent<TextMeshProUGUI>().text = score.ToString();
    }
}
