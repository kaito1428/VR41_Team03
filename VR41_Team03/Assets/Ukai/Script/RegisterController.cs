using UnityEngine;
using UnityEngine.InputSystem;

public class RegisterController : MonoBehaviour
{
    [SerializeField] private QueueManager m_QueueManager;

    private void Update()
    {
        if(Keyboard.current.enterKey.IsPressed())
        {
            CompleteCheckout();
        }
    }

    // 会計終了時に呼び出す
    public void CompleteCheckout()
    {
        if(m_QueueManager == null)
        {
            Debug.Log("QueueManagerが設定されていません。");
            return;
        }

        CustomerAI customer = m_QueueManager.FrontCustomer;

        if (customer == null)
        {
            Debug.Log("会計対象の客がいません。");
            return;
        }

        if(!customer.IsWaitingForCheckout)
        {
            Debug.Log("客が商品を置き終わるまでまってください。");
            return;
        }

        customer.CompleteCheckout();
        Debug.Log("会計を完了しました。");
    }
}
