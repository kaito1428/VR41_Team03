
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CustomerAI : MonoBehaviour
{
    [Header("商品配置設定")]
    [SerializeField] private float m_ProductSpacing = 0.2f;
    [SerializeField] private int m_ProductColumns = 3;
    [SerializeField] private float m_ProductRandomOffset = 0.05f;

    [Header("支払い設定")]
    [SerializeField] private GameObject m_MoneyPrefab;

    private NavMeshAgent m_Agent;

    private Transform m_InsidePoint;
    private Transform m_ExitPoint;
    private Transform m_ProductPoint;
    private Transform m_MoneyPoint;

    private QueueManager m_QueueManager;
    private CustomerOrderGenerator m_OrderGenerator;

    private ProductOrder[] m_ProductOrders;

    // この客が生成した商品をすべて管理
    private readonly List<GameObject> m_SpawnedProducts =
        new List<GameObject>();

    // この客が生成したMoney
    private GameObject m_SpawnedMoney;

    private CustomerState m_State;

    private bool m_IsInitialized;


    // =================================
    // プロパティ
    // =================================

    public CustomerState State => m_State;

    public ProductOrder[] ProductOrders => m_ProductOrders;

    public bool Paying =>
        m_State == CustomerState.Paying;


    // =================================
    // 初期化
    // =================================

    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }


    private void Start()
    {
        if (!m_IsInitialized)
        {
            Debug.LogError("CustomerAIが初期化されていません。");
            enabled = false;
            return;
        }

        if (!HasOrder())
        {
            Debug.LogError("注文リストが設定されていません。");
            enabled = false;
            return;
        }

        // 入店開始
        m_State = CustomerState.Entering;

        MoveTo(m_InsidePoint.position);
    }


    /// <summary>
    /// CustomerSpawnerから初期設定を受け取る
    /// </summary>
    public void Initialize(
        Transform insidePoint,
        Transform exitPoint,
        Transform productPoint,
        Transform moneyPoint,
        QueueManager queueManager,
        CustomerOrderGenerator orderGenerator,
        ProductOrder[] orders)
    {
        m_InsidePoint = insidePoint;
        m_ExitPoint = exitPoint;
        m_ProductPoint = productPoint;
        m_MoneyPoint = moneyPoint;

        m_QueueManager = queueManager;
        m_OrderGenerator = orderGenerator;

        SetOrder(orders);

        m_IsInitialized = true;
    }


    // =================================
    // 注文リスト
    // =================================

    /// <summary>
    /// 注文リストを設定
    /// </summary>
    public void SetOrder(ProductOrder[] orders)
    {
        if (orders == null || orders.Length == 0)
            return;

        m_ProductOrders = new ProductOrder[orders.Length];

        for (int i = 0; i < orders.Length; i++)
        {
            if (orders[i] == null)
                continue;

            m_ProductOrders[i] = new ProductOrder
            {
                productId = orders[i].productId,
                quantity = orders[i].quantity
            };
        }
    }


    /// <summary>
    /// 注文リストを確認
    /// </summary>
    public bool HasOrder()
    {
        if (m_ProductOrders == null ||
            m_ProductOrders.Length == 0)
        {
            return false;
        }

        foreach (ProductOrder order in m_ProductOrders)
        {
            if (order == null ||
                string.IsNullOrEmpty(order.productId) ||
                order.quantity <= 0)
            {
                return false;
            }
        }

        return true;
    }


    /// <summary>
    /// 注文された商品の合計数
    /// </summary>
    public int ExpectedProductCount
    {
        get
        {
            if (m_ProductOrders == null)
                return 0;

            int count = 0;

            foreach (ProductOrder order in m_ProductOrders)
            {
                if (order != null)
                {
                    count += order.quantity;
                }
            }

            return count;
        }
    }


    // =================================
    // 更新
    // =================================

    private void Update()
    {
        switch (m_State)
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


    // =================================
    // 入店
    // =================================

    private void UpdateEntering()
    {
        if (!HasReachedDestination())
            return;

        // JoinQueueが成功した場合のみ待機状態へ
        if (m_QueueManager.JoinQueue(this))
        {
            m_State = CustomerState.Queuing;
        }
    }


    // =================================
    // 順番待ち
    // =================================

    private void UpdateQueuing()
    {
        // 先頭になるまで待機
        if (!m_QueueManager.IsFront(this))
            return;

        // 待機位置に到着するまで待機
        if (!HasReachedDestination())
            return;

        m_State = CustomerState.PlacingProducts;

        StartCoroutine(PlaceProducts());
    }


    // =================================
    // 商品を置く
    // =================================

    private IEnumerator PlaceProducts()
    {
        m_Agent.isStopped = true;

        Debug.Log($"{name}: 商品を配置します。");

        int itemIndex = 0;

        // 列数が0以下にならないようにする
        int columns = Mathf.Max(1, m_ProductColumns);

        foreach (ProductOrder order in m_ProductOrders)
        {
            GameObject prefab =
                m_OrderGenerator.GetProductPrefab(order.productId);

            if (prefab == null)
                continue;

            for (int i = 0; i < order.quantity; i++)
            {
                int column = itemIndex % columns;
                int row = itemIndex / columns;

                // 格子状に配置
                Vector3 offset = new Vector3(
                    column * m_ProductSpacing,
                    0f,
                    row * m_ProductSpacing
                );

                // ランダムな位置ずれ
                Vector2 randomOffset =
                    Random.insideUnitCircle * m_ProductRandomOffset;

                offset += new Vector3(
                    randomOffset.x,
                    0f,
                    randomOffset.y
                );

                Vector3 position =
                    m_ProductPoint.TransformPoint(offset);

                GameObject product = Instantiate(
                    prefab,
                    position,
                    m_ProductPoint.rotation
                );

                // 生成した商品を記録
                m_SpawnedProducts.Add(product);

                itemIndex++;

                // 商品を少しずつ置く
                yield return new WaitForSeconds(0.3f);
            }
        }

        // 商品配置完了
        m_State = CustomerState.WaitingForCheckout;

        Debug.Log($"{name}: 会計待ちです。");
    }


    // =================================
    // 会計待ち・支払い開始
    // =================================

    /// <summary>
    /// BasketManagerから商品回収完了の通知を受け取る
    /// </summary>
    public void OnAllProductsCollected()
    {
        if (m_State != CustomerState.WaitingForCheckout)
            return;

        // 支払い状態へ遷移
        m_State = CustomerState.Paying;

        // Moneyを生成
        SpawnMoney();

        Debug.Log($"{name}: 支払いを開始します。");
    }


    /// <summary>
    /// 支払い金を生成する
    /// </summary>
    private void SpawnMoney()
    {
        if (m_MoneyPrefab == null)
        {
            Debug.LogError($"{name}: Money Prefabが設定されていません。");
            return;
        }

        // 二重生成防止
        if (m_SpawnedMoney != null)
            return;

        // 商品配置位置を基準に生成（Offsetなし）
        Vector3 position =
            m_MoneyPoint.TransformPoint(Vector3.zero);

        m_SpawnedMoney = Instantiate(
            m_MoneyPrefab,
            position,
            m_MoneyPoint.rotation
        );

        // Moneyにこの客のCustomerAIを渡す
        Money money = m_SpawnedMoney.GetComponent<Money>();

        if (money != null)
        {
            money.Initialize(this);
        }
        else
        {
            Debug.LogError(
                $"{name}: Money PrefabにMoneyスクリプトがありません。"
            );
        }
    }


    // =================================
    // 会計完了
    // =================================

    /// <summary>
    /// Moneyを掴んだ時に呼び出される
    /// </summary>
    public void CompleteCheckout()
    {
        if (!Paying)
            return;

        // 生成した商品をすべて削除
        DestroyAllProducts();

        // Moneyを削除
        if (m_SpawnedMoney != null)
        {
            Destroy(m_SpawnedMoney);
            m_SpawnedMoney = null;
        }

        // 待機列から退場
        m_QueueManager.LeaveQueue(this);

        // 退店開始
        m_State = CustomerState.Leaving;

        m_Agent.isStopped = false;

        MoveTo(m_ExitPoint.position);

        Debug.Log($"{name}: 会計完了。退店します。");
    }


    /// <summary>
    /// 生成した商品をすべて削除
    /// </summary>
    private void DestroyAllProducts()
    {
        foreach (GameObject product in m_SpawnedProducts)
        {
            if (product != null)
            {
                Destroy(product);
            }
        }

        m_SpawnedProducts.Clear();
    }


    // =================================
    // 待機位置変更
    // =================================

    /// <summary>
    /// QueueManagerから呼び出される
    /// 待機列ポイントへの移動先を設定
    /// </summary>
    public void SetQueueDestination(Transform point)
    {
        if (point == null)
            return;

        // 入店中または待機中のみ移動先を変更可能
        if (m_State != CustomerState.Entering &&
            m_State != CustomerState.Queuing)
        {
            return;
        }

        MoveTo(point.position);
    }


    // =================================
    // 退店
    // =================================

    private void UpdateLeaving()
    {
        if (!HasReachedDestination())
            return;

        m_State = CustomerState.Finished;

        Debug.Log($"{name}: 退店しました。");

        Destroy(gameObject);
    }


    // =================================
    // 移動
    // =================================

    private void MoveTo(Vector3 destination)
    {
        if (!m_Agent.isOnNavMesh)
        {
            Debug.LogError($"{name}: NavMesh上にいません。");
            return;
        }

        m_Agent.isStopped = false;

        m_Agent.SetDestination(destination);
    }


    // =================================
    // 到着判定
    // =================================

    private bool HasReachedDestination()
    {
        if (!m_Agent.isOnNavMesh)
            return false;

        if (m_Agent.pathPending)
            return false;

        if (!m_Agent.hasPath)
            return false;

        // 経路が正常に目的地まで到達できるか確認
        if (m_Agent.pathStatus != NavMeshPathStatus.PathComplete)
            return false;

        // 目的地までの距離を確認
        if (m_Agent.remainingDistance >
            m_Agent.stoppingDistance + 0.05f)
            return false;

        return true;
    }
}
