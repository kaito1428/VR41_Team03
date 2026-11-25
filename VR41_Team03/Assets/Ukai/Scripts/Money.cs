using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Money : MonoBehaviour
{
    private XRGrabInteractable m_GrabInteractable;
    private CustomerAI m_CustomerAI;

    private bool m_IsReceived;

    public void Initialize(CustomerAI customerAI)
    {
        m_CustomerAI = customerAI;
    }

    private void Awake()
    {
        m_GrabInteractable = GetComponent<XRGrabInteractable>();

        if (m_GrabInteractable == null)
        {
            Debug.LogError("XRGrabInteractableが見つかりません。");
            return;
        }

        m_GrabInteractable.selectEntered.AddListener(OnGrab);
    }

    private void OnDestroy()
    {
        if (m_GrabInteractable != null)
        {
            m_GrabInteractable.selectEntered.RemoveListener(OnGrab);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (m_IsReceived)
            return;

        m_IsReceived = true;

        Debug.Log("プレイヤーがお金を受け取りました。");

        if (m_CustomerAI != null)
        {
            m_CustomerAI.CompleteCheckout();
        }
    }
}