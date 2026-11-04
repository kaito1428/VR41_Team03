using UnityEngine;

public class TransactionTotalUI : MonoBehaviour
{
    [Header("‰ïŒv‹àŠzUI")]
    [SerializeField] private TMPro.TextMeshProUGUI m_transactionTotalText;

    private int m_totalAmount = 0;

    private void Start()
    {
        m_transactionTotalText.text = "0";
    }   

    public void UpdateTransactionTotal(int amount)
    {
        m_totalAmount += amount;
        m_transactionTotalText.text = "" + m_totalAmount.ToString();
    }

    public void ResetTransactionTotal()
    {
        m_totalAmount = 0;
        m_transactionTotalText.text = "0";
    }
}
