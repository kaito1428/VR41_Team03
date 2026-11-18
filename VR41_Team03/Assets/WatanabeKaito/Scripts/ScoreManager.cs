
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // シングルトン
    public static ScoreManager Instance { get; private set; }

    [Header("目標金額")]
    [SerializeField]
    private int m_targetAmount = 10000;

    // 現在の売上金額
    private int m_totalSales = 0;

    // 売上金額の取得
    public int TotalSales
    {
        get { return m_totalSales; }
    }

    // 目標金額の取得
    public int TargetAmount
    {
        get { return m_targetAmount; }
    }

    private void Awake()
    {
        // すでにインスタンスが存在する場合は重複を防ぐ
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // シーンを移動しても破棄されないようにする
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // ゲーム開始時の初期化
    public void StartGame()
    {
        m_totalSales = 0;
    }

    // 売上金額を加算する
    public void AddSales(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        m_totalSales += amount;
    }

    // 目標金額に対する達成割合を取得する
    public float GetAchievementRate()
    {
        if (m_targetAmount <= 0)
        {
            return 0f;
        }

        return (float)m_totalSales / m_targetAmount * 100f;
    }

    // 達成割合からランクを取得する
    public string GetRank()
    {
        float rate = GetAchievementRate();

        if (rate >= 100f)
        {
            return "S";
        }
        else if (rate >= 80f)
        {
            return "A";
        }
        else if (rate >= 60f)
        {
            return "B";
        }
        else if (rate >= 40f)
        {
            return "C";
        }
        else
        {
            return "D";
        }
    }
}
