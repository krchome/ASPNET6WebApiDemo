using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASPNET6WebApiDemo.Contexts;
using ASPNET6WebApiDemo.Models;
using ASPNET6WebApiDemo.Wrappers;
using ASPNET6WebApiDemo.Filter;

namespace ASPNET6WebApiDemo.Controllers
{
    [Route("api/[controller]", Name = "DefaultApi")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Customer

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Customers.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize).ToListAsync();
            var totalRecords = await _context.Customers.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / validFilter.PageSize);

            // Add Previous and Next page links
            string prevPageLink = validFilter.PageNumber > 1
                ? Url.Link("DefaultApi", new { PageNumber = validFilter.PageNumber - 1, PageSize = validFilter.PageSize })
                : null;

            string nextPageLink = validFilter.PageNumber < totalPages
                ? Url.Link("DefaultApi", new { PageNumber = validFilter.PageNumber + 1, PageSize = validFilter.PageSize })
                : null;

            var response = new PagedResponse<List<Customer>>(pagedData, validFilter.PageNumber, validFilter.PageSize)
            {
                TotalPages = totalPages,
                TotalRecords = totalRecords,
                PrevPageLink = prevPageLink,
                NextPageLink = nextPageLink
            };

            return Ok(response);
        }

        //
        // GET: api/Customer/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
         
            var customer = await _context.Customers.Where(a=>a.Id == id).FirstOrDefaultAsync();
            return Ok(new Response <Customer>(customer));
        }

        // PUT: api/Customer/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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

        // POST: api/Customer
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
          if (_context.Customers == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Customers'  is null.");
          }
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }

        // DELETE: api/Customer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return (_context.Customers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
