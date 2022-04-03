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
    public int Wave = 1;

    public Text BombsText;
    public Text WaveClearText;
    public Text GetReadyText;
    public Text WaveText;
    public Text TimeText;

    public Image ZBar;
    public State gameState = State.WAVE;

    public float toyFireRate;      // fire rate in projectiles/second
    public float toyNextFireTime;  // time to next projectile

    public int CatCollisionCost;
    public int[] ProjectileCollisionCost;
    public float WaveClearTime;
    public float GetReadyTime;

    public GameObject[] PowerUpPrefabs;
    public float PowerUpChance;
    public Transform PowerUpParent;

    public float EnemySpawnXStart;
    public float EnemySpawnXStep;
    public int MaxEnemySpawnIndex = 2;

    public float SpawnTarget = 3;
    public float SpawnGrowthFactor = 1.2f;

    public float MagicSpeed = 24f;

    public Transform liveEnemiesParent;
    public GameObject[] enemyPrefabs;

    public AudioClip MeowSound;
    public AudioClip MagicSound;
    public AudioClip PlayerHitSound;
    public AudioClip CatHitSound;
    public AudioClip PlayerPowerupSound;
    public AudioClip WaveClearSound;
    public AudioClip PlayerFireSound;
    public AudioClip CatnipBlowSound;
    public AudioClip LoseSound;

    public float ZeroTime;
    public float ElapsedTime;
    public float ClockScale;

    private float nextStateTime;
    private float thisEnemySpawnX;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            EnemyController[] enemies = liveEnemiesParent.GetComponentsInChildren<EnemyController>();
            foreach (EnemyController e in enemies)
            {
                e.Die();
            }
        }
        ZBar.rectTransform.sizeDelta = new Vector2(ZScore / 10f, 1);
        BombsText.text = "CATNIP: " + Bombs;
        float scaledTime = ElapsedTime / ClockScale;
        int hours = Mathf.FloorToInt(scaledTime / 60) + 4;
        int minutes = Mathf.FloorToInt(scaledTime % 60);
        TimeText.text = string.Format("{0:D2}:{1:D2}", hours, minutes);

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
                Wave++;
                WaveText.text = "WAVE " + Wave;
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
        } else if (gameState == State.STARTING)
        {
            gameState = State.WAVE_CLEAR_READY;
            nextStateTime = Time.time + GetReadyTime;
            WaveClearText.enabled = false;
            GetReadyText.enabled = true;
            thisEnemySpawnX = EnemySpawnXStart;
            WaveText.text = "WAVE " + Wave;
            ZeroTime = Time.time;
        }
        ElapsedTime = Time.time - ZeroTime;
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
        int spawnIdx = Random.Range(-2, MaxEnemySpawnIndex);
        // jankery to make the first two types more common
        if (spawnIdx < 0)
        {
            spawnIdx += 2;
        }
        GameObject prefab = enemyPrefabs[spawnIdx];
        GameObject newEnemy = Instantiate(prefab, new Vector3(thisEnemySpawnX, 0, 0), Quaternion.identity, liveEnemiesParent);
        EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
        enemyController.Started = false;
        enemyController.BuildPath();
        newEnemy.transform.position = new Vector3(newEnemy.transform.position.x, enemyController.path[0].position.y, newEnemy.transform.position.z);
        thisEnemySpawnX += EnemySpawnXStep;
    }

    public void RegisterPowerUp(int _ZScore, int _BScore, float _PScore)
    {
        audioSource.PlayOneShot(PlayerPowerupSound);
        ZScore += _ZScore;
        if (ZScore > 100)
        {
            ZScore = 100;
        }
        Bombs += _BScore;
        toyFireRate *= (1 + _PScore);
    }

    public void TrySpawnPowerup(Vector3 position)
    {
        if (Random.value < PowerUpChance)
        {
            int powerUpIdx = Random.Range(0, PowerUpPrefabs.Length);
            Instantiate(PowerUpPrefabs[powerUpIdx], position, Quaternion.identity, PowerUpParent);
        }
    }

    public void PlayMeow()
    {
        audioSource.PlayOneShot(MeowSound);
    }

    public void PlayMagic()
    {
        audioSource.PlayOneShot(MagicSound);
    }
}
