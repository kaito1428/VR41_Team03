using UnityEngine;

public class TransactionTotalUI : MonoBehaviour
{
    [Header("会計金額UI")]
    [SerializeField] private TMPro.TextMeshProUGUI m_transactionTotalText;

    private int m_transactionTotal = 0;

    private void Start()
    {
        m_transactionTotalText.text = "¥0";
    }   

    public void UpdateTransactionTotal(int amount)
    {
        m_transactionTotal += amount;
        m_transactionTotalText.text = "¥" + m_transactionTotal.ToString();
    }

    public void ResetTransactionTotal()
    {
        m_transactionTotal = 0;
        m_transactionTotalText.text = "¥0";
    }

    public int GetTransactionTotal()
    {
        return m_transactionTotal;
    }
}
