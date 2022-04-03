using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGSpawner : MonoBehaviour
{
    public GameObject[] Prefabs;
    public Transform parent;

    public float XPos;
    public float YRange;

    public float spawnTime;
    public float spawnChance;

    float nextSpawn;
    // Start is called before the first frame update
    void Start()
    {
        nextSpawn = Time.time + spawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawn)
        {
            nextSpawn += spawnTime;
            if (Random.value < spawnChance)
            {
                int idx = Random.Range(0, Prefabs.Length);
                Instantiate(Prefabs[idx], new Vector3(XPos, Random.Range(-YRange, YRange), 0), Quaternion.identity, parent);
            }
        }
    }
}
