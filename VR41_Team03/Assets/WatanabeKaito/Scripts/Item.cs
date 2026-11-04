using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("この商品の値段")]
    [SerializeField] private int m_price;

    private bool m_isScannedFlag = false;  // レジにスキャンしたかどうかのフラグ

    private bool m_basketInFlag = false; // バスケットに入っているかどうかのフラグ

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

    public void SetBasketIn(bool isInBasket)
    {
        m_basketInFlag = isInBasket;
    }

    public bool IsBasketIn()
    {
        return m_basketInFlag;
    }
}
