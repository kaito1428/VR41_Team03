
using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [Header("制限時間（分）")]
    [SerializeField] private int m_limitMinutes = 5;

    [Header("時間表示")]
    [SerializeField] private TMP_Text m_timerText;

    // 残り時間（秒）
    private float m_remainingTime;

    // カウントダウン中かどうか
    private bool m_isRunning;

    private void Start()
    {
        // 制限時間を秒に変換
        m_remainingTime = m_limitMinutes * 60f;

        m_isRunning = true;

        UpdateTimerText();
    }

    private void Update()
    {
        if (!m_isRunning)
        {
            return;
        }

        // 経過時間分を減らす
        m_remainingTime -= Time.deltaTime;

        // 0秒以下になった場合
        if (m_remainingTime <= 0f)
        {
            m_remainingTime = 0f;
            m_isRunning = false;
        }

        UpdateTimerText();
    }

    // 時間表示を更新
    private void UpdateTimerText()
    {
        // 1/100秒単位に変換
        int totalCentiseconds =
            Mathf.CeilToInt(m_remainingTime * 100f);

        int minutes = totalCentiseconds / 6000;
        int seconds = (totalCentiseconds / 100) % 60;
        int centiseconds = totalCentiseconds % 100;

        // 表示を更新
        m_timerText.text =
            minutes.ToString("00") + ":" +
            seconds.ToString("00") + "." +
            centiseconds.ToString("00");
    }
}
