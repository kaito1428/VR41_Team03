using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]
public class CustomerAI : MonoBehaviour
{
    [Header("移動先")]
    [SerializeField] private Transform m_InsidePoint;
    [SerializeField] private Transform m_ExitPoint;

    [Header("参照")]
    [SerializeField] private QueueManager m_QueueManager;

    [Header("行動時間")]
    [SerializeField] private float m_PlaceProductsTime = 2.0f;

    private NavMeshAgent m_Agent;
    private CustomerState m_State;

    public CustomerState State => m_State;

    public bool IsWaitingForCheckout => m_State == CustomerState.WaitingForCheckout;

    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if(m_InsidePoint == null || m_ExitPoint == null || m_QueueManager == null)
        {
            Debug.LogError($"{name}: CustomerAIの参照が設定されていません。", this);
            enabled = false;
            return;
        }

        // 入店開始
        m_State = CustomerState.Entering;
        MoveTo(m_InsidePoint.position);
    }

    private void Update()
    {
        switch(m_State)
        {
            case CustomerState.Entering:
                UpdateEntering();
                break;

            case CustomerState.Queuing:
                UpdateQueuing();
                break;

            case CustomerState.Leaving:
                UpdateLeaving();
                break;
        }
    }

    // 入店地点に到着したら列に加わる
    private void UpdateEntering()
    {
        if (!HasReachedDestination())
            return;

        m_State = CustomerState.Queuing;
        m_QueueManager.JoinQueue(this);
    }

    // レジの先頭に到着したら商品を置く
    private void UpdateQueuing()
    {
        if (!m_QueueManager.IsFront(this))
            return;

        if (!HasReachedDestination())
            return;

        m_State = CustomerState.PlacingProducts;
        StartCoroutine(PlacsProducts());
    }

    private IEnumerator PlacsProducts()
    {
        m_Agent.isStopped = true;

        yield return new WaitForSeconds(m_PlaceProductsTime);

        // 会計完了の通知が来るまで待機
        m_State = CustomerState.WaitingForCheckout;

        Debug.Log($"{name}: 会計待ち。");
    }

    // レジ側から呼び出す
    public void CompleteCheckout()
    {
        if (m_State != CustomerState.WaitingForCheckout)
            return;

        // 列から抜ける
        m_QueueManager.LeaveQueue(this);

        // 退店開始
        m_State = CustomerState.Leaving;
        m_Agent.isStopped = false;
        MoveTo(m_ExitPoint.position);

        Debug.Log($"{name}: 会計完了。退店します。");
    }

    // 列の待機位置を設定する
    public void SetQueueDestination(Transform point)
    {
        if (point == null || m_State != CustomerState.Queuing)
            return;

        MoveTo(point.position);
    }

    private void UpdateLeaving()
    {
        if (!HasReachedDestination())
            return;

        m_State = CustomerState.Finished;
        Debug.Log($"{name}: 退店しました。");

        Destroy(gameObject);
    }

    private void MoveTo(Vector3 destination)
    {
        if(!m_Agent.isOnNavMesh)
        {
            Debug.LogError($"{name}: NavMesh上に配置されていません。");
            return;
        }

        m_Agent.isStopped = false;
        m_Agent.SetDestination(destination);
    }

    private bool HasReachedDestination()
    {
        return m_Agent.isOnNavMesh && !m_Agent.pathPending && m_Agent.hasPath && m_Agent.remainingDistance 
            <= m_Agent.stoppingDistance + 0.05f;
    }
}
