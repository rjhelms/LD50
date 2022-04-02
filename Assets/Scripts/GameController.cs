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

    public float WaveClearTime;
    public float GetReadyTime;

    public Transform liveEnemiesParent;

    private float nextStateTime;
    private bool spawnDone = false;
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
                spawnDone = false;
            }
        } else if (gameState == State.WAVE_CLEAR_READY)
        {
            // spawn here
            spawnDone = true;

            if (spawnDone & Time.time > nextStateTime)
            {
                GetReadyText.enabled = false;
                gameState = State.WAVE;
                // activate enemies
            }
        }
    }
}
