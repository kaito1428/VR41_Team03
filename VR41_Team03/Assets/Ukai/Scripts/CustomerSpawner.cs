using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("‹q‚Ì¶¬İ’è")]
    [SerializeField] private GameObject m_CustomerPrefab;
    [SerializeField] private Transform m_SpawnPoint;

    [Header("‹q‚ÌˆÚ“®æ")]
    [SerializeField] private Transform m_InsidePoint;
    [SerializeField] private Transform m_ExitPoint;

    [Header("¤•i”z’uˆÊ’u")]
    [SerializeField] private Transform m_ProductPoint;

    [Header("‚¨‹à”z’uˆÊ’u")]
    [SerializeField] private Transform m_MoneyPoint;

    [Header("ŠÇ—ƒNƒ‰ƒX")]
    [SerializeField] private QueueManager m_QueueManager;
    [SerializeField] private CustomerOrderGenerator m_OrderGenerator;

    [Header("¶¬İ’è")]
    [SerializeField] private float m_SpawnInterval = 5.0f;
    [SerializeField] private int m_MaxCustomers = 5;
    [SerializeField] private bool m_IsSpawning = true;

    // ¶¬Ï‚İ‚ÅA‚Ü‚¾”jŠü‚³‚ê‚Ä‚¢‚È‚¢‹q‚ğŠÇ—
    private readonly List<CustomerAI> m_Customers = new List<CustomerAI>();

    private void Start()
    {
        if (m_CustomerPrefab == null ||
            m_SpawnPoint == null ||
            m_InsidePoint == null ||
            m_ExitPoint == null ||
            m_ProductPoint == null ||
            m_MoneyPoint == null ||
            m_QueueManager == null ||
            m_OrderGenerator == null)
        {
            Debug.LogError("CustomerSpawner‚ÌQÆ‚ªİ’è‚³‚ê‚Ä‚¢‚Ü‚¹‚ñB");
            enabled = false;
            return;
        }

        m_MaxCustomers = Mathf.Max(1, m_MaxCustomers);

        if (m_IsSpawning)
        {
            StartCoroutine(SpawnCustomers());
        }
    }

    /// <summary>
    /// ‹q‚ğˆê’èŠÔŠu‚Å¶¬
    /// </summary>
    private IEnumerator SpawnCustomers()
    {
        while (true)
        {
            SpawnCustomer();

            yield return new WaitForSeconds(m_SpawnInterval);
        }
    }

    /// <summary>
    /// ”jŠüÏ‚İ‚Ì‹q‚ğƒŠƒXƒg‚©‚çœŠO
    /// </summary>
    private void CleanupCustomers()
    {
        m_Customers.RemoveAll(customer => customer == null);
    }

    /// <summary>
    /// Œ»İ‘¶İ‚µ‚Ä‚¢‚é‹q‚Ì”
    /// </summary>
    public int CurrentCustomerCount
    {
        get
        {
            CleanupCustomers();
            return m_Customers.Count;
        }
    }

    /// <summary>
    /// ‹q‚ğ¶¬
    /// </summary>
    public void SpawnCustomer()
    {
        CleanupCustomers();

        // “¯‚É‘¶İ‚Å‚«‚é‹q”‚ğ’´‚¦‚Ä‚¢‚½‚ç¶¬‚µ‚È‚¢
        if (m_Customers.Count >= m_MaxCustomers)
            return;

        // ’•¶‚ğ¶¬
        ProductOrder[] orders = m_OrderGenerator.GenerateOrder();

        if (!_HasValidProductPrefabs(orders))
            return;

        // ‹q‚ğ¶¬
        GameObject customerObject = Instantiate(
            m_CustomerPrefab,
            m_SpawnPoint.position,
            m_SpawnPoint.rotation
        );

        CustomerAI customer = customerObject.GetComponent<CustomerAI>();

        if (customer == null)
        {
            Debug.LogError("CustomerAI‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñB");
            Destroy(customerObject);
            return;
        }

        // ‹q‚Ì‰Šú‰»
        customer.Initialize(
            m_InsidePoint,
            m_ExitPoint,
            m_ProductPoint,
            m_MoneyPoint,
            m_QueueManager,
            m_OrderGenerator,
            orders
        );

        // ¶¬‚µ‚½‹q‚ğŠÇ—ƒŠƒXƒg‚É’Ç‰Á
        m_Customers.Add(customer);
    }

    /// <summary>
    /// ’•¶‚É‘Î‰‚·‚é¤•iPrefab‚ª‘¶İ‚·‚é‚©Šm”F
    /// </summary>
    private bool _HasValidProductPrefabs(ProductOrder[] orders)
    {
        return m_OrderGenerator.HasProductPrefabs(orders);
    }
}
