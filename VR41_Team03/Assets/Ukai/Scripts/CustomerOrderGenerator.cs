
using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomerOrderGenerator : MonoBehaviour
{
    [Serializable]
    private class ProductData
    {
        public string productId;
        public GameObject productPrefab;
    }

    [Header("購入候補商品")]
    [SerializeField] private ProductData[] m_ProductData;

    [Header("購入商品数")]
    [SerializeField, Min(1)] private int m_MinItemCount = 1;
    [SerializeField, Min(1)] private int m_MaxItemCount = 5;


    /// <summary>
    /// 購入予定をランダム生成
    /// </summary>
    public ProductOrder[] GenerateOrder()
    {
        List<ProductData> validProducts = new List<ProductData>();

        foreach (ProductData data in m_ProductData)
        {
            if (data == null ||
                string.IsNullOrWhiteSpace(data.productId) ||
                data.productPrefab == null)
            {
                continue;
            }

            validProducts.Add(data);
        }

        if (validProducts.Count == 0)
        {
            Debug.LogError("購入候補商品が正しく設定されていません。");
            return null;
        }

        // 購入総数をランダム決定（最低1個）
        int minCount = Mathf.Max(1, m_MinItemCount);
        int maxCount = Mathf.Max(minCount, m_MaxItemCount);

        int totalItemCount = UnityEngine.Random.Range(
            minCount,
            maxCount + 1
        );

        List<ProductOrder> orders = new List<ProductOrder>();

        // ランダムに商品を選択
        for (int i = 0; i < totalItemCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(
                0,
                validProducts.Count
            );

            string productId =
                validProducts[randomIndex].productId;

            // 同じ商品が既に注文されているか確認
            ProductOrder existingOrder =
                orders.Find(x => x.productId == productId);

            if (existingOrder != null)
            {
                // 同じ商品なら数量を加算
                existingOrder.quantity++;
            }
            else
            {
                // 新しい商品を注文に追加
                orders.Add(new ProductOrder
                {
                    productId = productId,
                    quantity = 1
                });
            }
        }

        return orders.ToArray();
    }


    /// <summary>
    /// 商品IDからPrefabを取得
    /// </summary>
    public GameObject GetProductPrefab(string productId)
    {
        foreach (ProductData data in m_ProductData)
        {
            if (data == null)
                continue;

            if (data.productId == productId)
            {
                return data.productPrefab;
            }
        }

        Debug.LogError($"商品Prefabが見つかりません: {productId}");

        return null;
    }


    /// <summary>
    /// 注文商品のPrefabがすべて登録されているか確認
    /// </summary>
    public bool HasProductPrefabs(ProductOrder[] orders)
    {
        if (orders == null || orders.Length == 0)
            return false;

        foreach (ProductOrder order in orders)
        {
            if (order == null ||
                order.quantity <= 0 ||
                GetProductPrefab(order.productId) == null)
            {
                return false;
            }
        }

        return true;
    }
}
