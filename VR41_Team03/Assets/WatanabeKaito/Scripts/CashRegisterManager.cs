using UnityEngine;

public class CashRegisterManager : MonoBehaviour
{
    [Header("‰ïŒv‹àŠzUI")]
    [SerializeField] private TransactionTotalUI m_transactionTotalUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ITEM"))
        {
            Item item = other.GetComponent<Item>();
            if (item != null)
            {
                if (!item.IsScanned())
                {
                    int price = item.GetPrice();

                    m_transactionTotalUI.UpdateTransactionTotal(price);

                    item.SetScanned(true);
                }
                else
                {
                    return;
                }
            }
        }
    }
}
