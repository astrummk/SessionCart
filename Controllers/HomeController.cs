using Microsoft.AspNetCore.Mvc;
using SessionCart.Models;
using System.Diagnostics;
using Newtonsoft.Json;

namespace SessionCart.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _contex;

        public HomeController(IHttpContextAccessor contex)
        {
            //_logger = logger;
            _contex = contex;
        }


        [Route("")]
        [Route("Home/Index/{id}")]
        public IActionResult Index(string id)
        {
            string variableSession = HttpContext.Session.GetString("varSession");
            if (string.IsNullOrEmpty(variableSession))
            {
                var newGuid = Guid.NewGuid();
                HttpContext.Session.SetString("varSession", newGuid.ToString());
                variableSession = HttpContext.Session.GetString("varSession");
            }

            string CartQty = "";

            string localOrderStr;
            List<Order> localOrder = new List<Order>();

            string previusOrder = _contex.HttpContext.Session.GetString("LocalOrder");
            if (previusOrder != null)
            {
                localOrder = JsonConvert.DeserializeObject<List<Order>>(previusOrder);
                CartQty = localOrder.Sum(o => o.OrderProductQty).ToString();
            }

            if (!string.IsNullOrEmpty(id))
            {
                Product product = new Product();
                switch (id)
                {
                    case "A2":
                        product.ProductID = "A2";
                        product.ProductName = "Air Max";
                        break;
                    case "B3":
                        product.ProductID = "B3";
                        product.ProductName = "Air Tailwind";
                        break;
                    case "D4":
                        product.ProductID = "D4";
                        product.ProductName = "Air Zoom Pegasus";
                        break;
                    default:
                        // code block
                        break;
                }

                Order foundModel = localOrder.Find(localOrder => localOrder.OrderProductID == id);

                var maxRec = 0;
                if (localOrder.Count() > 0)
                {
                    maxRec = localOrder.Max(a => a.OrderNumberID);
                }

                if (foundModel != null)
                {
                    var qtyPrevius = foundModel.OrderProductQty;
                    maxRec = foundModel.OrderNumberID;
                    int qtyDiff = qtyPrevius + 1;
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

                CartQty = localOrder.Sum(o => o.OrderProductQty).ToString();
                localOrder = localOrder.OrderBy(a => a.OrderNumberID).ToList();

                localOrderStr = JsonConvert.SerializeObject(localOrder);

                _contex.HttpContext.Session.SetString("LocalOrder", localOrderStr);

            }
            ViewBag.varSession = variableSession;

            if (string.IsNullOrEmpty(CartQty))
            {
                CartQty = "0";
            }

            ViewData["CartQty"] = CartQty;

            return View();

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
