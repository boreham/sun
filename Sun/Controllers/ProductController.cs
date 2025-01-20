using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sun.Models;
using Sun.Repository;

namespace Sun.Controllers;

[Authorize]
// [Authorize(Roles = "Admin")]
// [Authorize(Policy = "CanViewPage")]
public class ProductController : Controller
{
    private readonly IProductRepository _productRepository;

    public ProductController(IConfiguration configuration)
    {
        _productRepository = new ProductRepository(configuration);
    }

    //public ProductController(IProductRepository productRepository)
    //{
    //    _productRepository = productRepository;
    //}

    // GET: /Product/Index
    public async Task<IActionResult> Index()
    {
        var products = await _productRepository.GetAllProductsAsync();
        return View(products);
    }

    // GET: /Product/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Product/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (ModelState.IsValid)
        {
            await _productRepository.AddProductAsync(product);
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    // GET: /Product/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productRepository.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    // POST: /Product/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _productRepository.UpdateProductAsync(product);
            return RedirectToAction(nameof(Index));
        }

        return View(product);
    }

    // GET: /Product/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productRepository.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    // POST: /Product/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _productRepository.DeleteProductAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
