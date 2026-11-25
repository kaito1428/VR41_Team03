using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class ColorBallRespawner : MonoBehaviour
{
    [Header("生成開始距離")]
    [SerializeField] private float m_spawnDistance = 0.15f;

    private XRGrabInteractable m_grabInteractable;
    private Rigidbody m_rigidbody;

    // ボールの生成位置
    private Vector3 m_spawnPosition;
    private Quaternion m_spawnRotation;

    // すでに次のボールを生成したか
    private bool m_hasSpawned = false;

    // 生成処理中か
    private bool m_isSpawning = false;

    private void Awake()
    {
        m_grabInteractable = GetComponent<XRGrabInteractable>();
        m_rigidbody = GetComponent<Rigidbody>();

        m_spawnPosition = transform.position;
        m_spawnRotation = transform.rotation;

        // Rigidbodyの初期状態を設定
        m_rigidbody.isKinematic = false;
        m_rigidbody.useGravity = true;

        // 置いてある間は固定
        m_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void OnEnable()
    {
        m_grabInteractable.selectEntered.AddListener(OnGrabbed);
        m_grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // 掴んだら固定を解除
        m_rigidbody.constraints = RigidbodyConstraints.None;

        m_rigidbody.linearVelocity = Vector3.zero;
        m_rigidbody.angularVelocity = Vector3.zero;

        // すでに生成済みなら終了
        if (m_hasSpawned || m_isSpawning)
        {
            return;
        }

        m_isSpawning = true;

        StartCoroutine(SpawnBallAfterMoved());
    }

    private IEnumerator SpawnBallAfterMoved()
    {
        // ボールが元の位置から離れるまで待つ
        while (m_grabInteractable.isSelected &&
               Vector3.Distance(transform.position, m_spawnPosition) < m_spawnDistance)
        {
            yield return null;
        }

        // 十分に移動していなければ生成しない
        if (Vector3.Distance(transform.position, m_spawnPosition) < m_spawnDistance)
        {
            m_isSpawning = false;
            yield break;
        }

        // このボールは生成済みにする
        m_hasSpawned = true;

        //新しいボールを生成
        GameObject newBall = Instantiate(
            gameObject,
            m_spawnPosition,
            m_spawnRotation
            );

        // 生成されたボールの状態を初期化
        Rigidbody newRb = newBall.GetComponent<Rigidbody>();

        if (newRb != null)
        {
            newRb.isKinematic = false;
            newRb.useGravity = true;

            // 置いてある間は固定
            newRb.constraints = RigidbodyConstraints.FreezeAll;

            newRb.linearVelocity = Vector3.zero;
            newRb.angularVelocity = Vector3.zero;
        }

        // 生成されたボールは次の生成ができる状態にする
        ColorBallRespawner newRespawner =
            newBall.GetComponent<ColorBallRespawner>();

        if (newRespawner != null)
        {
            newRespawner.m_hasSpawned = false;
            newRespawner.m_isSpawning = false;
        }

        m_isSpawning = false;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        StartCoroutine(ReleaseBall());
    }

    private IEnumerator ReleaseBall()
    {
        // XR Interaction Toolkitの解放処理を待つ
        yield return null;

        // 重力と物理演算を有効にする
        m_rigidbody.isKinematic = false;
        m_rigidbody.useGravity = true;
        m_rigidbody.constraints = RigidbodyConstraints.None;

        // 次のボールを生成済みなら掴めなくする
        if (m_hasSpawned)
        {
            m_grabInteractable.enabled = false;
        }
    }

    private void OnDestroy()
    {
        if (m_grabInteractable != null)
        {
            m_grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            m_grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}