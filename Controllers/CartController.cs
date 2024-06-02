using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SessionCart.Models;

namespace SessionCart.Controllers
{
    public class CartController : Controller
    {
        private readonly IHttpContextAccessor _contex;

        public CartController(IHttpContextAccessor contex)
        {
            _contex = contex;
        }

        public IActionResult Index()
        {
            List<Order> localOrder = new List<Order>();

            string orderCurrent = _contex.HttpContext.Session.GetString("LocalOrder");
            if (orderCurrent != null)
            {
                localOrder = JsonConvert.DeserializeObject<List<Order>>(orderCurrent);
            }

            string CartQty = "";

            if (string.IsNullOrEmpty(CartQty))
            {
                CartQty = "0";
            }

            CartQty = localOrder.Sum(o => o.OrderProductQty).ToString();

            ViewData["CartQty"] = CartQty;
            ViewData["cart"] = localOrder;

            return View();
        }

        public IActionResult Edit(string id)
        {
            string qty;
            string sifraID;

            sifraID = HttpContext.Request.Form["txtID"].ToString();
            qty = HttpContext.Request.Form["txtKol"].ToString();

            Product product = new Product();
            string localOrderStr;   
            List<Order> localOrder = new List<Order>();

            string previusOrder = _contex.HttpContext.Session.GetString("LocalOrder");
            if (previusOrder != null)
            {
                localOrder = JsonConvert.DeserializeObject<List<Order>>(previusOrder);
            }

            Order foundModel = localOrder.Find(localOrder => localOrder.OrderProductID == sifraID);

            var maxRec = 0;
            string CartQty = "";
            if (localOrder.Count() > 0)
            {
                maxRec = localOrder.Max(a => a.OrderNumberID);
            }

            product.ProductID = foundModel.OrderProductID;
            product.ProductName = foundModel.OrderProductName;

            if (foundModel != null)
            {
                var qtyPrevius = foundModel.OrderProductQty;
                maxRec = foundModel.OrderNumberID;
                int qtyDiff = qtyPrevius + Convert.ToInt32(qty);
                if (qtyDiff == 0)
                {
                    localOrder.Remove(foundModel);
                }
                else
                {
                    localOrder.Remove(foundModel);
                    localOrder.Add(new Order(product.ProductID, product.ProductName, qtyDiff, maxRec));
                }
            }
            else
            {
                maxRec += 1;
                localOrder.Add(new Order(product.ProductID, product.ProductName, 1, maxRec));

            }

            localOrder = localOrder.OrderBy(a => a.OrderNumberID).ToList();

            CartQty = localOrder.Sum(o => o.OrderProductQty).ToString();
            localOrder = localOrder.OrderBy(a => a.OrderNumberID).ToList();

            localOrderStr = JsonConvert.SerializeObject(localOrder);

            _contex.HttpContext.Session.SetString("LocalOrder", localOrderStr);

            if (string.IsNullOrEmpty(CartQty))
            {
                CartQty = "0";
            }

            ViewData["CartQty"] = CartQty;
            ViewData["cart"] = localOrder;

            return View("Index");
        }

        public IActionResult Order()
        {
            List<Order> localOrder = new List<Order>();
            string orderCurrent = _contex.HttpContext.Session.GetString("LocalOrder");
            if (orderCurrent != null)
            {
                localOrder = JsonConvert.DeserializeObject<List<Order>>(orderCurrent);
            }

            foreach (Order po in localOrder.ToList())
            {
                //Add here code to instert order in database
            }

            HttpContext.Session.Remove("LocalOrder");

            return View("Order");
        }

    }
}
