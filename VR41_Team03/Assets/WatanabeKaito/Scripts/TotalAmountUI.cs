using UnityEngine;

public class TotalAmountUI : MonoBehaviour
{
    [Header("合計金額UI")]
    [SerializeField] private TMPro.TextMeshProUGUI m_totalAmountText;

    private int m_totalAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_totalAmount = 0;

        // 初期表示
        m_totalAmountText.text = m_totalAmount.ToString();
    }

    // 合計金額を更新するメソッド
    public void UpdateTotalAmount(int amount)
    {
        m_totalAmount += amount;
        m_totalAmountText.text = m_totalAmount.ToString();
    }
}
