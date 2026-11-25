using System;
using UnityEngine;

[Serializable]
public class ProductOrder
{
    [Header("¤•iID")]
    public string productId;

    [Header("w“üŒÂ”")]
    [Min(1)]
    public int quantity = 1;
}