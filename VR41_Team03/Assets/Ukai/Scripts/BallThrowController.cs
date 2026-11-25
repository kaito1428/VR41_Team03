
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// XRGrabInteractableで掴んだボールの投擲処理を管理するクラス
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class BallThrowController : MonoBehaviour
{
    [Header("投擲設定")]

    [SerializeField]
    [Tooltip("投擲速度に掛ける倍率")]
    private float m_ThrowSpeedMultiplier = 1.5f;

    [SerializeField]
    [Tooltip("投擲速度の最大値")]
    private float m_MaxThrowSpeed = 10.0f;

    [Header("速度計算設定")]

    [SerializeField]
    [Tooltip("速度計算に使用する時間")]
    private float m_SampleDuration = 0.15f;

    [SerializeField]
    [Tooltip("投擲に必要な最低速度")]
    private float m_MinThrowSpeed = 0.1f;

    // コンポーネント
    private Rigidbody m_Rigidbody;
    private XRGrabInteractable m_GrabInteractable;

    // 掴んでいるコントローラーのTransform
    private Transform m_InteractorTransform;

    // 位置と時間の記録
    private struct PositionSample
    {
        public Vector3 Position;
        public float Time;

        public PositionSample(Vector3 position, float time)
        {
            Position = position;
            Time = time;
        }
    }

    private readonly List<PositionSample> m_PositionSamples =
        new List<PositionSample>();

    // 掴んでいるかどうか
    private bool m_IsGrabbed = false;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_GrabInteractable = GetComponent<XRGrabInteractable>();

        // XRI標準の投擲処理と重複しないようにする
        m_GrabInteractable.throwOnDetach = false;
    }

    private void OnEnable()
    {
        m_GrabInteractable.selectEntered.AddListener(OnGrab);
        m_GrabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        m_GrabInteractable.selectEntered.RemoveListener(OnGrab);
        m_GrabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void Update()
    {
        if (!m_IsGrabbed || m_InteractorTransform == null)
        {
            return;
        }

        // コントローラーの位置を記録
        RecordPosition();

        // 設定時間より古い記録を削除
        RemoveOldSamples();
    }

    /// <summary>
    /// ボールを掴んだときの処理
    /// </summary>
    private void OnGrab(SelectEnterEventArgs args)
    {
        // 掴んだコントローラーのTransformを取得
        m_InteractorTransform =
            args.interactorObject.GetAttachTransform(m_GrabInteractable);

        m_IsGrabbed = true;

        // 過去の位置情報を初期化
        m_PositionSamples.Clear();

        // 掴んだ瞬間の位置を記録
        RecordPosition();

        // 掴んだときの速度をリセット
        m_Rigidbody.linearVelocity = Vector3.zero;
        m_Rigidbody.angularVelocity = Vector3.zero;
    }

    /// <summary>
    /// ボールを離したときの処理
    /// </summary>
    private void OnRelease(SelectExitEventArgs args)
    {
        if (!m_IsGrabbed)
        {
            return;
        }

        // 離した瞬間の位置も記録
        RecordPosition();

        // コントローラーの移動速度を計算
        Vector3 throwVelocity = CalculateThrowVelocity();

        // 投擲速度の調整
        throwVelocity *= m_ThrowSpeedMultiplier;

        // 最大速度を制限
        throwVelocity = Vector3.ClampMagnitude(
            throwVelocity,
            m_MaxThrowSpeed
        );

        // 最低速度未満なら投げない
        if (throwVelocity.magnitude < m_MinThrowSpeed)
        {
            throwVelocity = Vector3.zero;
        }

        // ボールに速度を設定
        m_Rigidbody.linearVelocity = throwVelocity;

        // 掴み状態を解除
        m_IsGrabbed = false;
        m_InteractorTransform = null;
        m_PositionSamples.Clear();
    }

    /// <summary>
    /// コントローラーの位置を記録する
    /// </summary>
    private void RecordPosition()
    {
        if (m_InteractorTransform == null)
        {
            return;
        }

        PositionSample sample = new PositionSample(
            m_InteractorTransform.position,
            Time.time
        );

        m_PositionSamples.Add(sample);
    }

    /// <summary>
    /// 設定時間より古い位置情報を削除する
    /// </summary>
    private void RemoveOldSamples()
    {
        float limitTime = Time.time - m_SampleDuration;

        while (m_PositionSamples.Count > 2 &&
               m_PositionSamples[1].Time < limitTime)
        {
            m_PositionSamples.RemoveAt(0);
        }
    }

    /// <summary>
    /// 記録した位置情報から投擲速度を計算する
    /// </summary>
    private Vector3 CalculateThrowVelocity()
    {
        if (m_PositionSamples.Count < 2)
        {
            return Vector3.zero;
        }

        PositionSample first = m_PositionSamples[0];
        PositionSample last =
            m_PositionSamples[m_PositionSamples.Count - 1];

        float deltaTime = last.Time - first.Time;

        if (deltaTime <= 0.0f)
        {
            return Vector3.zero;
        }

        return (last.Position - first.Position) / deltaTime;
    }
}
