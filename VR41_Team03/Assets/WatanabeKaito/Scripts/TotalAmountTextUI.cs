using UnityEngine;

public class TotalAmountTextUI : MonoBehaviour
{
    [Header("çáåvã‡äzUI")]
    [SerializeField] private TMPro.TextMeshProUGUI m_totalAmountText;

    private int m_totalAmount = 0;

    private void Start()
    {
        m_totalAmountText.text = "Åè" + m_totalAmount.ToString();
    }   

    public void UpdateTotalAmount(int amount)
    {
        m_totalAmount += amount;
        m_totalAmountText.text = "Åè" + m_totalAmount.ToString();
    }
}
