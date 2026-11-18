using UnityEngine;

public class TotalAmountUI : MonoBehaviour
{
    [Header("合計金額UI")]
    [SerializeField] private TMPro.TextMeshProUGUI m_totalAmountText;

    [Header("目標金額")]
    [SerializeField ] private int m_targetAmount = 10000; // 目標金額を設定

    [Header("ゲームマネージャー")]
    [SerializeField] private GameManager m_gameManager;

    private int m_totalAmount;

    private bool m_isTargetReached = false; // 目標金額に到達したかどうかのフラグ

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_totalAmount = 0;

        // 初期表示
        m_totalAmountText.text = "¥" + m_totalAmount.ToString("N0") + "/¥" + m_targetAmount.ToString("N0");
    }

    // 合計金額を更新するメソッド
    public void UpdateTotalAmount(int amount)
    {
        m_totalAmount += amount;
        m_totalAmountText.text = "¥" +  m_totalAmount.ToString("N0") + "/¥" + m_targetAmount.ToString("N0");

        ScoreManager.Instance.AddSales(amount);

        if(m_totalAmount >= m_targetAmount)
        {
            if(!m_isTargetReached)
            {
                m_gameManager.SceneChangeResult();
                m_isTargetReached = true;
            }
        }
    }
}
