using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    [Header("待機位置")]
    [SerializeField]
    private Transform[] m_QueuePoints;

    private List<CustomerAI> m_Customers = new();


    /// <summary>
    /// 先頭の客
    /// </summary>
    public CustomerAI FrontCustomer
    {
        get
        {
            if (m_Customers.Count == 0)
                return null;

            return m_Customers[0];
        }
    }


    /// <summary>
    /// 客を待機列に追加
    /// </summary>
    public bool JoinQueue(CustomerAI customer)
    {
        if (customer == null ||
            m_Customers.Contains(customer))
        {
            return false;
        }

        if (m_Customers.Count >= m_QueuePoints.Length)
            return false;

        m_Customers.Add(customer);

        UpdateQueuePositions();

        return true;
    }


    /// <summary>
    /// 客を待機列から削除
    /// </summary>
    public void LeaveQueue(CustomerAI customer)
    {
        if (customer == null)
            return;

        if (!m_Customers.Remove(customer))
            return;

        UpdateQueuePositions();
    }


    /// <summary>
    /// 先頭の客か確認
    /// </summary>
    public bool IsFront(CustomerAI customer)
    {
        return FrontCustomer == customer;
    }


    /// <summary>
    /// 待機位置を更新
    /// </summary>
    private void UpdateQueuePositions()
    {
        for (int i = 0; i < m_Customers.Count; i++)
        {
            if (m_QueuePoints[i] == null ||
                m_Customers[i] == null)
            {
                continue;
            }

            m_Customers[i].SetQueueDestination(
                m_QueuePoints[i]
            );
        }
    }
}
