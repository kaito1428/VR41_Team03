
using UnityEngine;
using TMPro;

public class ResultUI : MonoBehaviour
{
    [Header("リザルト表示")]
    [SerializeField]
    private TMP_Text m_salesText;

    [SerializeField]
    private TMP_Text m_targetText;

    [SerializeField]
    private TMP_Text m_rateText;

    [SerializeField]
    private TMP_Text m_rankText;

    private void Start()
    {
        UpdateResult();
    }

    // リザルト画面の表示を更新する
    public void UpdateResult()
    {
        ScoreManager scoreManager = ScoreManager.Instance;

        if (scoreManager == null)
        {
            Debug.LogError("ScoreManagerが見つかりません。");
            return;
        }

        // 売上金額
        m_salesText.text = "売上金額：" + scoreManager.TotalSales.ToString("N0") + "円";

        // 目標金額
        m_targetText.text = "目標金額：" + scoreManager.TargetAmount.ToString("N0") + "円";

        // 達成割合
        m_rateText.text = "達成割合：" + scoreManager.GetAchievementRate().ToString("F1") + "%";

        // ランク
        m_rankText.text = "ランク：" + scoreManager.GetRank();
    }
}
