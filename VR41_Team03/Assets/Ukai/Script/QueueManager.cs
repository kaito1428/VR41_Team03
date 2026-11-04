using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    [Header("レジ待機位置（先頭から順番に登録）")]
    [SerializeField] private Transform[] m_QueuePoints;

    private readonly List<CustomerAI> m_Customers = new();

    public CustomerAI FrontCustomer => m_Customers.Count > 0 ? m_Customers[0] : null;

    // 客が列に加わる
    public void JoinQueue(CustomerAI customer)
    {
        if (customer == null || m_Customers.Contains(customer))
            return;

        m_Customers.Add(customer);
        UpdateQueuePositions();
    }

    // 客が列から離れる
    public void LeaveQueue(CustomerAI customer)
    {
        if (customer == null)
            return;

        m_Customers.Remove(customer);
        UpdateQueuePositions();
    }

    // 指定された客が先頭かどうか
    public bool IsFront(CustomerAI customer)
    {
        return FrontCustomer == customer;
    }

    // 列の位置更新
    private void UpdateQueuePositions()
    {
        for(int i = 0; i < m_Customers.Count; i++)
        {
            if(i >= m_QueuePoints.Length)
            {
                Debug.LogWarning("レジの待機位置が不足しています。");
                break;
            }

            m_Customers[i].SetQueueDestination(m_QueuePoints[i]);
        }
    }
}
