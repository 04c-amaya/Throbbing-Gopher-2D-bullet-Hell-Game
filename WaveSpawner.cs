using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

[System.Serializable]

public class Wave
{
    public int noOfEnemies;
    public float spawnInterval;
    public bool isBossRound;
    public bool finalWave;
}



public class WaveSpawner : MonoBehaviour
{
    PlayerManager playerManager;
    GameOver gameOver;
    BossCheckList checkList;

    [Header("Type of Enemies")]
    public GameObject normalEnemy;
    public GameObject airEnemy;
    public GameObject tankEnemy;
    public static WaveSpawner instance { get; private set; }

    [Header("Arrays & Lists")]
    public int [] normalStages;
    [HideInInspector]
    public List <GameObject> typeOfEnemies;
    //[HideInInspector]
    public List<GameObject> bossList;

    public Wave[] waves;
    [HideInInspector]
    public List<GameObject> spawnPoints;

    [Header("Wave Info")]
    [HideInInspector]

    public Wave currentWave;
    [HideInInspector]
    public int currentWaveNumber;
    public float EnemySpawnTimer;
    public GameObject[] enemiesLeft;
  //  [HideInInspector]
    public bool canSpawnEnemies = true;

    public bool loadNewArena = false;

    [Header("Timers")]
    [HideInInspector]
    public int waitForNextWaveTimer= 5;

    [Header("Boss Info")]
    public GameObject finalBoss;
    public GameObject currentBoss;

    public bool bossDead;

    public bool bossIsSpawned;

    [Header("UI")]

    public Text currentEnemyCount;
    public Text currentWaveCount;
    public GameObject WavesUI;
    public GameObject InfoUI;

    private void Awake()
    {
        checkList = GameObject.Find("Boss Checklist").GetComponent<BossCheckList>();
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {

            instance = this;

        }
    }
    private void Start()
    {
        typeOfEnemies.Clear();
        typeOfEnemies.Add(normalEnemy);

        bossList.Clear();
        bossList.Add(normalEnemy);
        bossList.Add(airEnemy);
        bossList.Add(tankEnemy);

        gameOver = GameObject.Find("Game Over Manager").GetComponent<GameOver>();
    }

    private void Update()
    {
        currentEnemyCount.text = enemiesLeft.Length.ToString();
        currentWaveCount.text = (currentWaveNumber + 1).ToString();
        enemiesLeft = GameObject.FindGameObjectsWithTag("Enemy");
        //Debug.Log(currentWaveNumber + 1+"" + waves.Length);

        if (playerManager.isDead == false && playerManager != null)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        currentWave = waves[currentWaveNumber];
        SpawnWave();
        BossWave();
        if (enemiesLeft.Length == 0 && !canSpawnEnemies)
        {
            StartCoroutine(NextWaveTimer());
        }
    }
    //Checks to see if the boss has been defeated and then destroys all enemies on the stage and loads a new stage
    public void bossDefeated()
    {
        // StartCoroutine(TextPopup(bossI));
        currentWave.noOfEnemies = 0;
        canSpawnEnemies = false;
        foreach (GameObject enemy in enemiesLeft)
            {
                Destroy(enemy);
            }
            if (loadNewArena == true)
            {
                Debug.Log("Load New Area");
                var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
                player.spawnProtection = true;
                player.cantMove = true;
                player.TimerforGroundTimer();
                GoToNewStage();
                player.transform.position = new Vector2(0, 0);
                StartCoroutine(ClearAndGetNewSpawnPoints());
                loadNewArena = false;
        }
    }
    //countdown for the new wave
    IEnumerator NextWaveTimer()
    {
        Debug.Log("NextWaveTimer");
        yield return new WaitForSeconds(waitForNextWaveTimer);
        StartNextWave();
    }
//Starts a new wave after all enemies have been defeated
    public void StartNextWave()
    {
        if (canSpawnEnemies==false && enemiesLeft.Length ==0)
        {
            Debug.Log("StartNextWave");
            currentWaveNumber++;
            canSpawnEnemies = true;
            return;
        }

    }
    //Controls the enemy spawns
    public void SpawnWave()
    {
        Debug.Log("SpawnWave");
        if (canSpawnEnemies && EnemySpawnTimer < Time.time)
        {
            if (spawnPoints.Count != 0)
            {
                Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Count)].transform;
                Instantiate(typeOfEnemies[Random.Range(0, typeOfEnemies.Count)], randomPoint.position, Quaternion.identity);
                currentWave.noOfEnemies--;
                EnemySpawnTimer = Time.time + currentWave.spawnInterval;
            }
                if (currentWave.noOfEnemies <= 0 && (currentWaveNumber != waves.Length))
                {
                    if (bossIsSpawned == false)
                    {
                        canSpawnEnemies = false;
                    }
                }
        }
    }
    //Checks to see if the current wave is a boss wave and the if the boss is the final boss
    public void BossWave()
    {
        Debug.Log("BossWave");

        if(currentBoss != null)
        {
            bossIsSpawned = true;
        }
    if(currentBoss == null && bossIsSpawned== true)
        {
            bossDead = true;
        }

        if (currentWave.finalWave == false)
        {
            if (currentWave.isBossRound && currentBoss == null && bossIsSpawned == false)
            {
               
                if(spawnPoints.Count != 0)
                {
                    Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Count)].transform;
                    var randomnumber = Random.Range(0, bossList.Count);
                    currentBoss = Instantiate(bossList[randomnumber], randomPoint.position, Quaternion.identity);
                }


                var bossManager = currentBoss.GetComponent<EnemyManager>();
                bossManager.isBoss = true;
                currentWave.isBossRound = false;
            }
            if (bossDead == true) 
            {
                AddorRemoveEnemyFromLists();
                bossIsSpawned = false;
                currentBoss = null;
                loadNewArena = true;
                bossDefeated();
                bossDead = false;
                Debug.Log("Boss Dead");
            }
        }
        else
        {
            if (currentWave.isBossRound && currentBoss == null && bossIsSpawned == false)
            {
                bossDead = false;
                // Fighting finalBoss
                if (spawnPoints.Count != 0)
                {
                    Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Count)].transform;
                    currentBoss = Instantiate(finalBoss, randomPoint.position, Quaternion.identity);
                }
                currentWave.isBossRound = false;
            }
            if (bossDead == true)
            {
                bossIsSpawned = false;
                currentBoss = null;
                bossDefeated();
                bossDead = false;
                WavesUI.SetActive(false);
                gameOver.GameIsOver();
                Destroy(gameObject);
            }

        }
    }
    //Randomly load a different stage when boss is defeated
    public void GoToNewStage()
    {
        bossDead = false;
        var i = Random.Range(0, normalStages.Length);
        SceneManager.LoadScene(normalStages[i]);
        
    }
    //clears all enemy spawn points when new area is loaded and then grabs that areas spawnpoints
    IEnumerator ClearAndGetNewSpawnPoints()
    {
        yield return new WaitForSeconds(2);
        Debug.Log("Cleared SpawnPoints");
    }
    public void AddorRemoveEnemyFromLists()
    {

        if (checkList.normalBossIsDefeated == true)
        {
            bossList.Remove(normalEnemy);
        }
        if (checkList.airBossisDefeated == true)
        {
            typeOfEnemies.Add(airEnemy);
            bossList.Remove(airEnemy);
        }
        if (checkList.tankBossIsDefeated == true)
        {
            typeOfEnemies.Add(tankEnemy);
            bossList.Remove(tankEnemy);
        }
    }
}
