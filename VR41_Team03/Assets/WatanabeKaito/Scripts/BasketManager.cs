using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasketManager : MonoBehaviour
{
    [Header("会計金額UI")]
    [SerializeField] private TransactionTotalUI m_transactionTotalUI;

    [Header("合計金額UI")]
    [SerializeField] private TotalAmountUI m_totalAmountUI;

    [SerializeField] private QueueManager m_QueueManager;

    private List <GameObject> basketItems = new List<GameObject>(); // バスケット内のアイテムを格納するリスト

    private CustomerAI m_CurrentCustomer;

    private void Update()
    {
        // 現在の先頭客を取得
        CustomerAI customer = m_QueueManager != null ? m_QueueManager.FrontCustomer : null;

        // 客が変わった場合
        if (customer != m_CurrentCustomer)
        {
            m_CurrentCustomer = customer;
        }

        AutoCompleteCheckout();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ITEM"))
        {
            Item item = other.GetComponent<Item>();
            if (item != null)
            {
                item.SetBasketIn(true); // アイテムがバスケットに入ったことを設定

                // 商品がスキャン済みかどうかを確認し、スキャン済みの場合のみバスケットに追加
                if (item.IsScanned() && !basketItems.Contains(other.gameObject))
                {
                    basketItems.Add(other.gameObject); // アイテムをバスケットに追加

                    Debug.Log($"商品がかごに入りました：{other.gameObject.name}");
                }
            }
        }
    }

    // バスケットに入っているアイテムのリストを取得するメソッド
    public List<GameObject> GetBasketItems()
    {
        return basketItems;
    }

    // バスケット内のアイテムを全て削除するメソッド
    public void ClearBasket()
    {
        //foreach (GameObject item in basketItems)
        //{
        //    Destroy(item); // アイテムを削除
        //}
        basketItems.Clear(); // リストをクリア

        m_totalAmountUI.UpdateTotalAmount(m_transactionTotalUI.GetTransactionTotal()); // 会計金額を合計金額に加算

        m_transactionTotalUI.ResetTransactionTotal(); // 会計金額UIをリセット

        //m_CurrentCustomer.CompleteCheckout(); // 会計完了
    }

    // 自動会計判定
    private void AutoCompleteCheckout()
    {
        if (m_CurrentCustomer == null)
            return;

        int expectedCount = m_CurrentCustomer.ExpectedProductCount;

        if (expectedCount <= 0)
            return;

        GetBasketItems();

        if (basketItems.Count != expectedCount)
            return;

        ClearBasket();

        // 会計完了通知
        m_CurrentCustomer.OnAllProductsCollected();
    }
}
