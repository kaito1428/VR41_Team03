using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasketManager : MonoBehaviour
{
    [Header("会計金額UI")]
    [SerializeField] private TransactionTotalUI m_transactionTotalUI;

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
            basketItems.Add(other.gameObject); // アイテムをバスケットに追加
        }
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
    }
}
