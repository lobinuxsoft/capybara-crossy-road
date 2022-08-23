using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private int maxTerrainCount;
    [SerializeField] private GameObject terrainPref;

    private ObjectPool pool;
    private Vector3 currentPosition = Vector3.zero;

    private void Start()
    {
        pool = GetComponent<ObjectPool>();
        pool.InitPool(terrainPref, maxTerrainCount);

        for (int i = 0; i < pool.Size; i++)
        {
            GameObject temp = pool.GetFromPool();
            temp.transform.localPosition = currentPosition;
            pool.ReturnToPool(temp, true);
            currentPosition.z++;
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
        GameObject terrainTile = pool.GetFromPool();
        terrainTile.transform.localPosition = currentPosition;
        pool.ReturnToPool(terrainTile, true);
        currentPosition.z++;
    }
}