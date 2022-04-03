using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameOver : MonoBehaviour
{

    public Text scoreText;
    ScoreManager scoreManager;
    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        scoreText.text = scoreManager.Time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
