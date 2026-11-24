
using System.Collections.Generic;
using UnityEngine;

public class TroublesomeCustomerSpawner : MonoBehaviour
{
    [Header("生成設定")]
    [SerializeField] private GameObject m_CustomerPrefab;
    [SerializeField] private Transform m_SpawnPoint;

    [Header("移動設定")]
    [SerializeField] private Collider m_WalkArea;
    [SerializeField] private Transform m_EntrancePoint;
    [SerializeField] private Transform m_ExitPoint;

    [Header("生成制限")]
    [SerializeField] private int m_MaxTotalCount = 10;
    [SerializeField] private int m_MaxActiveCount = 3;
    [SerializeField] private float m_SpawnInterval = 10f;

    private readonly List<GameObject> m_ActiveCustomers =
        new List<GameObject>();

    private int m_TotalSpawnedCount;
    private float m_SpawnTimer;
    private bool m_IsGameActive;

    public int ActiveCount => m_ActiveCustomers.Count;
    public int TotalSpawnedCount => m_TotalSpawnedCount;

    private void Update()
    {
        if (!m_IsGameActive)
            return;

        // DestroyされたNPCをリストから除外
        m_ActiveCustomers.RemoveAll(customer => customer == null);

        // 累計生成数の上限
        if (m_TotalSpawnedCount >= m_MaxTotalCount)
            return;

        // 同時出現数の上限
        if (m_ActiveCustomers.Count >= m_MaxActiveCount)
            return;

        m_SpawnTimer += Time.deltaTime;

        if (m_SpawnTimer >= m_SpawnInterval)
        {
            if (SpawnCustomer())
                m_SpawnTimer = 0f;
        }
    }

    /// <summary>
    /// ゲーム開始時に呼び出す
    /// </summary>
    public void BeginGame()
    {
        if (!ValidateSettings())
            return;

        m_TotalSpawnedCount = 0;
        m_SpawnTimer = 0f;
        m_ActiveCustomers.Clear();

        m_IsGameActive = true;

        // ゲーム開始直後に1体生成　（デバッグ用）
        if (m_MaxTotalCount > 0 && m_MaxActiveCount > 0)
        {
            SpawnCustomer();
        }
    }

    /// <summary>
    /// ゲーム終了時に呼び出す
    /// </summary>
    public void EndGame()
    {
        m_IsGameActive = false;
    }

    /// <summary>
    /// Inspectorの設定を確認
    /// </summary>
    private bool ValidateSettings()
    {
        if (m_CustomerPrefab == null ||
            m_SpawnPoint == null ||
            m_WalkArea == null ||
            m_EntrancePoint == null ||
            m_ExitPoint == null)
        {
            Debug.LogError($"{name}: Spawnerの参照が設定されていません。");
            return false;
        }

        if (m_MaxTotalCount <= 0 ||
            m_MaxActiveCount <= 0 ||
            m_SpawnInterval <= 0f)
        {
            Debug.LogError($"{name}: 生成制限の設定が不正です。");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 迷惑客を生成する
    /// </summary>
    private bool SpawnCustomer()
    {
        GameObject customer = Instantiate(
            m_CustomerPrefab,
            m_SpawnPoint.position,
            m_SpawnPoint.rotation
        );

        TroublesomeCustomerAI customerAI =
            customer.GetComponent<TroublesomeCustomerAI>();

        if (customerAI == null)
        {
            Debug.LogError("PrefabにTroublesomeCustomerAIがありません。");
            Destroy(customer);
            return false;
        }

        // シーン固有の参照をNPCに渡す
        customerAI.Initialize(
            m_WalkArea,
            m_EntrancePoint,
            m_ExitPoint
        );

        m_ActiveCustomers.Add(customer);
        m_TotalSpawnedCount++;

        return true;
    }
}
