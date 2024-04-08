using AzureStorageLibrary;
using AzureStorageLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using MVCWebApp.Models;
using System.Diagnostics;
using System.Net;

namespace MVCWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly INoSqlStorage<Product> _noSqlStorage;
        private readonly INoSqlStorage<Store> _storeNoSqlStorage;

        public HomeController(INoSqlStorage<Product> noSqlStorage, INoSqlStorage<Store> storeNoSqlStorage)
        {
            _noSqlStorage = noSqlStorage;
            _storeNoSqlStorage = storeNoSqlStorage;
        }

        public async Task<IActionResult> Index()
        {
            //var stores = await _storeNoSqlStorage.All();
            //var cityNames = stores.Select(store => store.CityName).ToList();
            ////ViewBag.CityNames = cityNames;
            ViewBag.stores=(await _storeNoSqlStorage.All()).ToList();
            ViewBag.products = (await _noSqlStorage.All()).ToList();

            ViewBag.IsUpdate = false;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            product.RowKey = Guid.NewGuid().ToString();
            product.PartitionKey = "Proucts";

            await _noSqlStorage.Add(product);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> CreateStore(Store store)
        {
            store.RowKey = Guid.NewGuid().ToString();
            store.PartitionKey = "Internal";
            await _storeNoSqlStorage.Add(store);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(string rowKey, string partitionKey)
        {
            var product = await _noSqlStorage.Get(rowKey, partitionKey);
            ViewBag.products = (await _noSqlStorage.All()).ToList();
            ViewBag.IsUpdate = true;

            return View("Index", product);
        }


        [HttpPost]
        public async Task<IActionResult> Update(Product product)
        {
            ViewBag.IsUpdate = true;
            product.ETag = "*";
            try
            {
                await _noSqlStorage.Update(product);
                return RedirectToAction("Index");

            }
            catch (StorageException ex)
            {
                ex.RequestInformation.HttpStatusCode = (int)HttpStatusCode.PreconditionFailed;
                throw;
            }

        }


        [HttpGet]
        public async Task<IActionResult> Delete(string rowKey, string partitionKey)
        {

            await _noSqlStorage.Delete(rowKey, partitionKey);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteStore(string rowKey, string partitionKey)
        {

            await _storeNoSqlStorage.Delete(rowKey, partitionKey);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Query(int filterPrice)
        {
            ViewBag.IsUpdate = false;
            ViewBag.products = (await _noSqlStorage.Query(x => x.Price > filterPrice)).ToList();
            return View("Index");
        }
    }
}
