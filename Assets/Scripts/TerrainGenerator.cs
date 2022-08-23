using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private int maxTerrainCount = 30;
    [SerializeField] private List<GameObject> terrains = new List<GameObject>();

    // TODO cambiar a un sistema de pool (por suerte ya tengo uno armado xD)
    private Queue<GameObject> terrainsQueue = new Queue<GameObject>();
    private Vector3 currentPosition = Vector3.zero;

    private void Start()
    {
        Random.InitState(Mathf.RoundToInt(Time.time));

        for (int i = 0; i < maxTerrainCount; i++)
        {
            SpawnTerrain();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            SpawnTerrain();
        }
    }

    private void SpawnTerrain()
    {
        GameObject terrain = Instantiate(terrains[Random.Range(0, terrains.Count)], currentPosition, Quaternion.identity);
        terrainsQueue.Enqueue(terrain);
        
        if(terrainsQueue.Count > maxTerrainCount)
        {
            GameObject temp = terrainsQueue.Dequeue();
            Destroy(temp);
        }
        currentPosition.z++;
    }
}