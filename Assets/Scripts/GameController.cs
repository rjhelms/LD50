using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public int ZScore;
    public int Bombs = 3;

    public Text BombsText;
    public Image ZBar;

    public float toyFireRate;      // fire rate in projectiles/second
    public float toyNextFireTime;  // time to next projectile

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ZBar.rectTransform.sizeDelta = new Vector2(ZScore / 10f, 1);
        BombsText.text = "CATNIP: " + Bombs;
    }
}
