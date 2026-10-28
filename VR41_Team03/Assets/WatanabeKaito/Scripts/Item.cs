using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("この商品の値段")]
    [SerializeField] private int m_price;

    private bool m_isScannedFlag = false;  // レジにスキャンしたかどうかのフラグ

    public int GetPrice()
    {
        return m_price;
    }

    public void SetScanned(bool isScanned)
    {
        m_isScannedFlag = isScanned;
    }

    public bool IsScanned()
    {
        return m_isScannedFlag;
    }
}
