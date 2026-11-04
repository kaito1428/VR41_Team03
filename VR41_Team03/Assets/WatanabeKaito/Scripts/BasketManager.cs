using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasketManager : MonoBehaviour
{
    [Header("会計金額UI")]
    [SerializeField] private TransactionTotalUI m_transactionTotalUI;

    [SerializeField] private QueueManager m_QueueManager;

    private List <GameObject> basketItems = new List<GameObject>(); // バスケット内のアイテムを格納するリスト

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ClearBasket();
        }
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
                if (item.IsScanned())
                {
                    basketItems.Add(other.gameObject); // アイテムをバスケットに追加
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
        foreach (GameObject item in basketItems)
        {
            Destroy(item); // アイテムを削除
        }
        basketItems.Clear(); // リストをクリア

        m_transactionTotalUI.ResetTransactionTotal(); // 会計金額UIをリセット

        CustomerAI customer = m_QueueManager.FrontCustomer;

        if (customer != null)
        {
            customer.CompleteCheckout(); // 会計を完了
        }
    }
}
