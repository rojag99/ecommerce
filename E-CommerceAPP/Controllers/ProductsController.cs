using E_CommerceAPP.Data;
using E_CommerceAPP.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using System.Text.Json;

namespace E_CommerceAPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly EcommerceDbContext productsdbcontext;
        private readonly SieveProcessor sieveProcessor;
        private readonly JsonSerializerOptions _jsonOptions;
        public ProductsController(EcommerceDbContext productsdbcontext, SieveProcessor sieveProcessor)
        {
            this.productsdbcontext = productsdbcontext;
            this.sieveProcessor = sieveProcessor;
        }
        // GET: api/products

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Products>>> GetProducts()
        {
            try
            {
                var products = await productsdbcontext.Products.ToListAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //----------------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<Products>> GetProduct(int id)
        {
            var product = await productsdbcontext.Products
                                          .Include(p => p.Categories) // Ensure Categories are included in the query
                                          .FirstOrDefaultAsync(p => p.Product_ID == id);

            if (product == null)
            {
                return NotFound(); // Handle case where product with given ID is not found
            }

            return product;
        }
        //----------------------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<Products>> PostProduct(AddProductDTOwithcategory productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Retrieve the associated category from the database
            var category = await productsdbcontext.Categories.FindAsync(productDto.category_ID);

            if (category == null)
            {
                return BadRequest("Invalid Category ID");
            }

            // Map the DTO to the entity
            var product = new Products
            {
                Product_Name = productDto.product_Name,
                Product_Description = productDto.product_Description,
                Product_Price = productDto.product_Price,
                Category_ID = productDto.category_ID,
                Categories = category // Associate the product with the category
            };

            // Add the product to the context and save changes
            productsdbcontext.Products.Add(product);
            await productsdbcontext.SaveChangesAsync();

            // Return the created product
            return CreatedAtAction("GetProduct", new { id = product.Product_ID }, product);
        }
        //--------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductDTO updateDTO)
        {
            if (id != updateDTO.Product_ID)
            {
                return BadRequest("Product ID mismatch");
            }

            // Retrieve the existing product from the database
            var product = await productsdbcontext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            // Update product properties
            product.Product_Name = updateDTO.Product_Name;
            product.Product_Description = updateDTO.Product_Description;
            product.Product_Price = updateDTO.Product_Price;

            // Ensure a valid category ID is provided
            if (updateDTO.Category_ID != 0)
            {
                // Retrieve the associated category from the database
                var category = await productsdbcontext.Categories.FindAsync(updateDTO.Category_ID);
                if (category == null)
                {
                    return BadRequest("Invalid Category ID");
                }

                // Associate the product with the category
                product.Category_ID = updateDTO.Category_ID;
                product.Categories = category;
            }

            // Save changes
            try
            {
                await productsdbcontext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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
        private bool ProductExists(int id)
        {
            return productsdbcontext.Products.Any(e => e.Product_ID == id);
        }

        //---------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await productsdbcontext.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found");
                }

                productsdbcontext.Products.Remove(product);
                await productsdbcontext.SaveChangesAsync();

                return NoContent(); // HTTP 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}

