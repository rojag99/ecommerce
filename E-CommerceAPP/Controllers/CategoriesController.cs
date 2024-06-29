using E_CommerceAPP.Data;
using E_CommerceAPP.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using E_CommerceAPP.Models.Entities; // Import your entity models namespace

namespace E_CommerceAPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly EcommerceDbContext categoriesdbcontext;
        private readonly ILogger<CategoriesController> _logger; // Define ILogger
        private readonly JsonSerializerOptions _jsonOptions;
        public CategoriesController(EcommerceDbContext categoriesdbcontext, ILogger<CategoriesController> logger)
        {
            this.categoriesdbcontext = categoriesdbcontext;
            _logger = logger;
        }
        //------------------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categories>>> GetCategories()
        {
            return await categoriesdbcontext.Categories.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Categories>> GetCategoryById(int id)
        {
            var category = await categoriesdbcontext.Categories
                                        .FirstOrDefaultAsync(c => c.Category_ID == id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        //---------------------------------------------------------------------------------------------------------------------------------------

        [HttpGet("{id}/products")]
        public async Task<ActionResult<CategoriesDTO>> GetCategoryWithProducts(int id)
        {
            var categoryWithProducts = await categoriesdbcontext.Categories
                .Where(c => c.Category_ID == id)
                .Select(c => new CategoriesDTO
                {
                    Category_ID = c.Category_ID,
                    Category_Name = c.Category_Name,
                    Products = c.Products.Select(p => new ProductDTO
                    {
                        Product_ID = p.Product_ID,
                        Product_Name = p.Product_Name,
                        Product_Description = p.Product_Description,
                        Product_Price = p.Product_Price
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (categoryWithProducts == null)
            {
                return NotFound();
            }

            return categoryWithProducts;
        }
        //-------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<Categories>> PostCategory(CategoriesDTO categoryWithProducts)
        {
            // Validate the incoming DTO
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create a new category entity
            var newCategory = new Categories
            {
                Category_Name = categoryWithProducts.Category_Name,
                Products = new List<Products>() // Initialize Products collection
            };

            // Add products to the new category
            foreach (var productDTO in categoryWithProducts.Products)
            {
                var product = new Products
                {
                    Product_Name = productDTO.Product_Name,
                    Product_Description = productDTO.Product_Description,
                    Product_Price = productDTO.Product_Price,
                    Category_ID = newCategory.Category_ID // Assign Category_ID to product
                };

                newCategory.Products.Add(product);
            }

            // Save changes to the database
            categoriesdbcontext.Categories.Add(newCategory);
            await categoriesdbcontext.SaveChangesAsync();

            // Return a response with status 201 (Created)
            // Include the newly created category in the response
            return CreatedAtAction(nameof(GetCategory), new { id = newCategory.Category_ID }, newCategory);

        }

        private object GetCategory()
        {
            throw new NotImplementedException();
        }

        //------------------------------------------------------------------------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateonlyCategoryDTO updatedCategory)
        {
            var categoryToUpdate = await categoriesdbcontext.Categories.FindAsync(id);

            if (categoryToUpdate == null)
            {
                return NotFound();
            }

            // Update only the category name
            categoryToUpdate.Category_Name = updatedCategory.Category_Name;

            try
            {
                await categoriesdbcontext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        //-------------------------------------------------------------

        [HttpPut("update-with-products/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdatecategoryDTO updatedCategory)
        {
            // Retrieve existing category including products
            var categoryToUpdate = await categoriesdbcontext.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Category_ID == id);

            if (categoryToUpdate == null)
            {
                return NotFound();
            }

            // Update category details
            categoryToUpdate.Category_Name = updatedCategory.Category_Name;

            // Update or add products
            foreach (var updatedProduct in updatedCategory.Products)
            {
                var existingProduct = categoryToUpdate.Products.FirstOrDefault(p => p.Product_ID == updatedProduct.Product_ID);
                if (existingProduct != null)
                {
                    // Update existing product
                    existingProduct.Product_Name = updatedProduct.Product_Name;
                    existingProduct.Product_Description = updatedProduct.Product_Description;
                    existingProduct.Product_Price = updatedProduct.Product_Price;
                }
                else
                {
                    // Add new product to the category (if necessary)
                    var newProduct = new Products
                    {
                        Product_Name = updatedProduct.Product_Name,
                        Product_Description = updatedProduct.Product_Description,
                        Product_Price = updatedProduct.Product_Price,
                        Category_ID = id // Assign category ID to link product with category
                    };
                    categoryToUpdate.Products.Add(newProduct);
                }
            }

            // Remove products that are not in updatedCategory.Products
            var productsToRemove = categoryToUpdate.Products.Where(p => !updatedCategory.Products.Any(up => up.Product_ID == p.Product_ID)).ToList();
            foreach (var productToRemove in productsToRemove)
            {
                categoriesdbcontext.Products.Remove(productToRemove);
            }

            try
            {
                await categoriesdbcontext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        //---------------------------------------------------------------------------


        private bool CategoryExists(int id)
        {
            return categoriesdbcontext.Categories.Any(e => e.Category_ID == id);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await categoriesdbcontext.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            try
            {
                categoriesdbcontext.Categories.Remove(category);
                await categoriesdbcontext.SaveChangesAsync();
                return NoContent(); // 204 No Content
            }
            catch (DbUpdateException ex)
            {
                // Handle specific exception for foreign key constraint (if using Restrict)
                return BadRequest(new { error = "Cannot delete category. It has associated products." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deleting the category." });
            }
        }
    }

}
