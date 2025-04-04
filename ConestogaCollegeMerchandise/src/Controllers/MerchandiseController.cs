using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;          // For EntityState, DbUpdateConcurrencyException, etc.
using Microsoft.AspNetCore.JsonPatch;         // For JsonPatchDocument<T>
using ConestogaCollegeMerchandise.Data;       // For MerchandiseContext
using ConestogaCollegeMerchandise.Models;     // For Merchandise model

namespace ConestogaCollegeMerchandise.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MerchandiseController : ControllerBase
    {
        private readonly MerchandiseContext _context;

        public MerchandiseController(MerchandiseContext context)
        {
            _context = context;
        }

        // GET: api/Merchandise
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.MerchandiseItems.ToListAsync();
            return Ok(items);
        }

        // GET: api/Merchandise/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _context.MerchandiseItems.FindAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // POST: api/Merchandise
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Merchandise item)
        {
            if (item == null)
                return BadRequest();

            _context.MerchandiseItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
        }

        // PUT: api/Merchandise/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Merchandise item)
        {
            if (id != item.Id)
                return BadRequest("ID mismatch");

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MerchandiseExists(id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        // PATCH: api/Merchandise/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> PartialUpdate(int id, [FromBody] JsonPatchDocument<Merchandise> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var item = await _context.MerchandiseItems.FindAsync(id);
            if (item == null)
                return NotFound();

            patchDoc.ApplyTo(item, ModelState);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MerchandiseExists(id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        // DELETE: api/Merchandise/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.MerchandiseItems.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.MerchandiseItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // OPTIONS: api/Merchandise
        [HttpOptions]
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "GET,POST,PUT,PATCH,DELETE,OPTIONS");
            return Ok();
        }

        private bool MerchandiseExists(int id)
        {
            return _context.MerchandiseItems.Any(e => e.Id == id);
        }
    }
}
