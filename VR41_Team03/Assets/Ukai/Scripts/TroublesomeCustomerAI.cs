
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class TroublesomeCustomerAI : MonoBehaviour
{
    private enum CustomerState
    {
        Entering,
        Wandering,
        Leaving
    }

    [Header("移動設定")]

    [Tooltip("店内に滞在する時間（秒）")]
    [SerializeField] private float m_StayDuration = 30f;

    [Tooltip("目的地に到着してから次の目的地へ移動するまでの待機時間（秒）")]
    [SerializeField] private float m_MoveInterval = 3f;

    [Tooltip("NavMesh上の移動可能地点を検索するときの検索半径")]
    [SerializeField] private float m_SampleDistance = 2f;

    [Tooltip("店内のランダムな移動先を探す最大試行回数")]
    [SerializeField] private int m_MaxSampleAttempts = 10;
    private NavMeshAgent m_Agent;

    // Spawnerから受け取るシーン固有の参照
    private Collider m_WalkArea;
    private Transform m_EntrancePoint;
    private Transform m_ExitPoint;

    private CustomerState m_State;

    private float m_StayTimer;
    private float m_MoveTimer;

    private bool m_IsInitialized;
    private bool m_HasDestination;

    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Spawnerから移動に必要な参照を受け取る
    /// </summary>
    public void Initialize(
        Collider walkArea,
        Transform entrancePoint,
        Transform exitPoint)
    {
        if (walkArea == null || entrancePoint == null || exitPoint == null)
        {
            Debug.LogError($"{name}: Initializeに渡された参照が不足しています。");
            return;
        }

        Debug.Log("迷惑客の登場だ！！");

        m_WalkArea = walkArea;
        m_EntrancePoint = entrancePoint;
        m_ExitPoint = exitPoint;

        m_IsInitialized = true;
    }

    private void Start()
    {
        if (!m_IsInitialized)
        {
            Debug.LogError($"{name}: Initializeが呼び出されていません。");
            enabled = false;
            return;
        }

        if (!m_Agent.isOnNavMesh)
        {
            Debug.LogError($"{name}: NavMesh上に配置されていません。");
            enabled = false;
            return;
        }

        m_State = CustomerState.Entering;

        if (!TrySetDestination(m_EntrancePoint.position))
        {
            Debug.LogError($"{name}: 入店地点への移動経路を設定できません。");
            enabled = false;
        }
    }

    private void Update()
    {
        switch (m_State)
        {
            case CustomerState.Entering:
                UpdateEntering();
                break;

            case CustomerState.Wandering:
                UpdateWandering();
                break;

            case CustomerState.Leaving:
                UpdateLeaving();
                break;
        }
    }

    /// <summary>
    /// 入店中
    /// </summary>
    private void UpdateEntering()
    {
        if (!HasReachedDestination())
            return;

        m_State = CustomerState.Wandering;
        m_StayTimer = m_StayDuration;
        m_MoveTimer = 0f;

        SetRandomDestination();
    }

    /// <summary>
    /// 店内を徘徊中
    /// </summary>
    private void UpdateWandering()
    {
        m_StayTimer -= Time.deltaTime;

        if (m_StayTimer <= 0f)
        {
            StartLeaving();
            return;
        }

        // 移動中は到着するまで待つ
        if (m_HasDestination)
        {
            if (!HasReachedDestination())
                return;
        }

        // 到着後、一定時間待ってから次の目的地を設定
        m_MoveTimer += Time.deltaTime;

        if (m_MoveTimer >= m_MoveInterval)
        {
            m_MoveTimer = 0f;
            SetRandomDestination();
        }
    }

    /// <summary>
    /// 退店開始
    /// </summary>
    private void StartLeaving()
    {
        m_State = CustomerState.Leaving;
        m_MoveTimer = 0f;

        if (!TrySetDestination(m_ExitPoint.position))
        {
            Debug.LogWarning($"{name}: 退店地点への移動経路を設定できません。");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 退店中
    /// </summary>
    private void UpdateLeaving()
    {
        if (HasReachedDestination())
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 店内のランダムな地点を目的地に設定
    /// </summary>
    private void SetRandomDestination()
    {
        Bounds bounds = m_WalkArea.bounds;

        for (int i = 0; i < m_MaxSampleAttempts; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                bounds.center.y,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            if (!NavMesh.SamplePosition(
                randomPosition,
                out NavMeshHit hit,
                m_SampleDistance,
                NavMesh.AllAreas))
            {
                continue;
            }

            // WalkAreaの範囲外なら採用しない
            if (!bounds.Contains(hit.position))
                continue;

            if (TrySetDestination(hit.position))
                return;
        }

        Debug.LogWarning($"{name}: 店内の目的地を見つけられませんでした。");
    }

    /// <summary>
    /// 指定地点への移動経路を設定
    /// </summary>
    private bool TrySetDestination(Vector3 destination)
    {
        if (m_Agent == null || !m_Agent.isOnNavMesh)
            return false;

        NavMeshPath path = new NavMeshPath();

        if (!m_Agent.CalculatePath(destination, path))
            return false;

        if (path.status != NavMeshPathStatus.PathComplete)
            return false;

        if (!m_Agent.SetDestination(destination))
            return false;

        m_HasDestination = true;
        return true;
    }

    /// <summary>
    /// 目的地に到着したか判定
    /// </summary>
    private bool HasReachedDestination()
    {
        if (!m_HasDestination)
            return false;

        if (m_Agent.pathPending)
            return false;

        if (m_Agent.remainingDistance > m_Agent.stoppingDistance)
            return false;

        if (m_Agent.velocity.sqrMagnitude > 0.01f)
            return false;

        m_HasDestination = false;
        return true;
    }
}
