using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyTypeSpawn
{
    [Tooltip("Prefab enemy (phải có EnemyController / EnemyHealthController).")]
    public GameObject enemyPrefab;

    [Tooltip("Số lượng enemy loại này sẽ sinh ra.")]
    [Min(0)]
    public int spawnCount = 1;

    [Tooltip("Các điểm spawn — kéo Transform vào đây (mỗi vị trí bạn đã sắp xếp trong scene).")]
    public Transform[] spawnPoints;
}

public class EnemyAIContainer : MonoBehaviour
{
    public static EnemyAIContainer instance;

    [Header("Cấu hình từng loại Enemy")]
    public EnemyTypeSpawn[] enemyTypes;

    [Header("Spawn")]
    [Tooltip("Tự động spawn khi vào scene.")]
    public bool spawnOnStart = true;

    [Tooltip("Gom enemy sinh ra làm con của object này.")]
    public bool parentEnemiesToContainer = true;

    private readonly List<GameObject> activeEnemies = new List<GameObject>();

    public int ActiveEnemyCount => activeEnemies.Count;

    public IReadOnlyList<GameObject> ActiveEnemies => activeEnemies;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnAllEnemies();
        }
    }

    public void SpawnAllEnemies()
    {
        ClearAllEnemies();

        if (enemyTypes == null || enemyTypes.Length == 0)
        {
            return;
        }

        for (int i = 0; i < enemyTypes.Length; i++)
        {
            SpawnEnemyType(enemyTypes[i], i);
        }
    }

    private void SpawnEnemyType(EnemyTypeSpawn config, int typeIndex)
    {
        if (config == null || config.enemyPrefab == null)
        {
            Debug.LogWarning($"EnemyAIContainer: Loại enemy #{typeIndex} chưa gán prefab.", this);
            return;
        }

        if (config.spawnCount <= 0)
        {
            return;
        }

        if (config.spawnPoints == null || config.spawnPoints.Length == 0)
        {
            Debug.LogWarning($"EnemyAIContainer: Loại enemy #{typeIndex} ({config.enemyPrefab.name}) chưa có spawn point.", this);
            return;
        }

        for (int i = 0; i < config.spawnCount; i++)
        {
            Transform point = config.spawnPoints[i % config.spawnPoints.Length];

            if (point == null)
            {
                Debug.LogWarning($"EnemyAIContainer: Spawn point null tại index {i} (loại {config.enemyPrefab.name}).", this);
                continue;
            }

            GameObject enemy = Instantiate(config.enemyPrefab, point.position, point.rotation);

            if (parentEnemiesToContainer)
            {
                enemy.transform.SetParent(transform);
            }

            EnemyContainerMember member = enemy.GetComponent<EnemyContainerMember>();
            if (member == null)
            {
                member = enemy.AddComponent<EnemyContainerMember>();
            }

            member.Init(this);

            activeEnemies.Add(enemy);
        }
    }

    public void NotifyEnemyDestroyed(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
    }

    public void ClearAllEnemies()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] != null)
            {
                Destroy(activeEnemies[i]);
            }
        }

        activeEnemies.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        if (enemyTypes == null)
        {
            return;
        }

        Gizmos.color = Color.red;

        foreach (EnemyTypeSpawn config in enemyTypes)
        {
            if (config?.spawnPoints == null)
            {
                continue;
            }

            foreach (Transform point in config.spawnPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawWireSphere(point.position, 0.5f);
                }
            }
        }
    }
}

public class EnemyContainerMember : MonoBehaviour
{
    private EnemyAIContainer container;

    public void Init(EnemyAIContainer owner)
    {
        container = owner;
    }

    private void OnDestroy()
    {
        if (container != null)
        {
            container.NotifyEnemyDestroyed(gameObject);
        }
    }
}
