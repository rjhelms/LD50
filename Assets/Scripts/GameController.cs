using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public enum State
    {
        WAVE,
        WAVE_CLEAR,
        WAVE_CLEAR_READY,
        STARTING,
        ENDING
    };

    public int ZScore;
    public int Bombs = 3;

    public Text BombsText;
    public Text WaveClearText;
    public Text GetReadyText;
    public Image ZBar;
    public State gameState = State.WAVE;

    public float toyFireRate;      // fire rate in projectiles/second
    public float toyNextFireTime;  // time to next projectile

    public int CatCollisionCost;
    public int[] ProjectileCollisionCost;
    public float WaveClearTime;
    public float GetReadyTime;

    public float EnemySpawnXStart;
    public float EnemySpawnXStep;
    public int MaxEnemySpawnIndex = 2;

    public float SpawnTarget = 3;
    public float SpawnGrowthFactor = 1.2f;

    public Transform liveEnemiesParent;
    public GameObject[] enemyPrefabs;
        
    private float nextStateTime;
    private float thisEnemySpawnX;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ZBar.rectTransform.sizeDelta = new Vector2(ZScore / 10f, 1);
        BombsText.text = "CATNIP: " + Bombs;

        if (gameState == State.WAVE)
        {
            if (liveEnemiesParent.childCount == 0)
            {
                gameState = State.WAVE_CLEAR;
                nextStateTime = Time.time + WaveClearTime;
                WaveClearText.enabled = true;
            }
        } else if (gameState == State.WAVE_CLEAR)
        {
            if (Time.time > nextStateTime)
            {
                gameState = State.WAVE_CLEAR_READY;
                nextStateTime = Time.time + GetReadyTime;
                WaveClearText.enabled = false;
                GetReadyText.enabled = true;
                thisEnemySpawnX = EnemySpawnXStart;
            }
        } else if (gameState == State.WAVE_CLEAR_READY)
        {
            if (liveEnemiesParent.childCount < SpawnTarget)
            {
                SpawnEnemy();
            }
            else if (Time.time > nextStateTime)
            {
                if (MaxEnemySpawnIndex < enemyPrefabs.Length)
                {
                    MaxEnemySpawnIndex++;
                }
                GetReadyText.enabled = false;
                gameState = State.WAVE;
                EnemyController[] enemies = liveEnemiesParent.GetComponentsInChildren<EnemyController>();
                foreach (EnemyController e in enemies)
                {
                    e.Started = true;
                }
                SpawnTarget = Mathf.CeilToInt(SpawnTarget * SpawnGrowthFactor);
            }
        }
    }

    public void PlayerHitByCat()
    {
        ZScore -= CatCollisionCost;
        if (ZScore <= 0)
        {
            ZScore = 0;
            Debug.Log("GAME OVER!");
        }
    }

    public void PlayerHitByProjectile(Projectile.ProjectileType type)
    {
        ZScore -= ProjectileCollisionCost[(int)type];
        if (ZScore <= 0)
        {
            ZScore = 0;
            Debug.Log("GAME OVER!");
        }
    }
    private void SpawnEnemy()
    {
        GameObject prefab = enemyPrefabs[Random.Range(0, MaxEnemySpawnIndex)];
        GameObject newEnemy = Instantiate(prefab, new Vector3(thisEnemySpawnX, 0, 0), Quaternion.identity, liveEnemiesParent);
        EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
        enemyController.Started = false;
        enemyController.BuildPath();
        newEnemy.transform.position = new Vector3(newEnemy.transform.position.x, enemyController.path[0].position.y, newEnemy.transform.position.z);
        thisEnemySpawnX += EnemySpawnXStep;
    }
}
