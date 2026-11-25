public enum CustomerState
{
    Entering,           // 入店、店内への移動
    Queuing,            // レジの列に並ぶ
    PlacingProducts,    // 商品をレジに置く
    WaitingForCheckout, // 会計待ち
    Paying,             // 支払い
    Leaving,            // 退店
    Finished            // 処理終了
}