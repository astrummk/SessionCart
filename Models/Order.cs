namespace SessionCart.Models
{
    public class Order
    {

        public string OrderProductID { get; set; }
        public string OrderProductName { get; set; }
        public int OrderProductQty { get; set; }
        public int OrderNumberID { get; set; }

        public Order(string orderProductID, string orderProductName, int orderProductQty, int orderNumberID)
        {
            OrderProductID = orderProductID;
            OrderProductName = orderProductName;
            OrderProductQty = orderProductQty;
            OrderNumberID = orderNumberID;
        }

    }
}
