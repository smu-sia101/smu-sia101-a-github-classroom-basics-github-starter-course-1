using Microsoft.AspNetCore.Mvc;
using MonolithApp.Demo.Models;
using MonolithApp.Demo.Services;

namespace MonolithApp.Demo.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductsService _productsService;
        public ProductsController()
        {
            _productsService = new ProductsService();
        }

        public IActionResult Index()
        {
            ViewBag.ProductId = TempData["ProductId"]?.ToString();
            var products = _productsService.GetAllProducts();
            return View(products);
        }

        public IActionResult Details([FromQuery] int id)
        {
            try
            {
                Product product = _productsService.GetProductById(id);
                return View(product);
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult SubmitProduct(Product product)
        {
            _productsService.AddProduct(product);
            TempData["ProductId"] = product.Id;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                var product = _productsService.GetProductById(id);
                if (product == null)
                {
                    TempData["Error"] = $"Product with ID {id} not found";
                    return RedirectToAction(nameof(Index));
                }
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error retrieving product: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public IActionResult UpdateProduct(Product product)
        {
            try
            {
                if (product == null || product.Id == 0)
                {
                    TempData["Error"] = "Invalid product data";
                    return RedirectToAction(nameof(Index));
                }

                var existingProduct = _productsService.GetProductById(product.Id);
                if (existingProduct == null)
                {
                    TempData["Error"] = $"Product with ID {product.Id} not found";
                    return RedirectToAction(nameof(Index));
                }

                _productsService.UpdateProduct(product);
                TempData["Success"] = $"Product {product.Id} was successfully updated";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating product: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                var product = _productsService.GetProductById(id);
                if (product == null)
                {
                    TempData["Error"] = $"Product with ID {id} not found";
                    return RedirectToAction(nameof(Index));
                }

                try
                {
                    _productsService.DeleteProductByID(product);
                    TempData["Success"] = $"Product {id} was successfully deleted";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error deleting product {id}: {ex.Message}";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error retrieving product {id}: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
