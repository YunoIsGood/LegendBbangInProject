using UnityEngine;

public class HealSpawner : MonoBehaviour
{
    [Header("힐팩 프리팹")]
    public GameObject healPrefab; 

    [Header("생성할 힐팩 개수")]
    public int spawnCount = 20;    

    [Header("스폰 범위 설정")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    void Start()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnHeal();
        }
    }

    void SpawnHeal()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

        Instantiate(healPrefab, spawnPosition, Quaternion.identity, this.transform);
    }

}