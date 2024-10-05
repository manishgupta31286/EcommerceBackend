using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController(EcommerceContext dbContext) : ControllerBase
    {
        private readonly EcommerceContext _dbContext = dbContext;

        [HttpGet]
        public async Task<IActionResult> GetAll(string searchTerm = null, int pageNumber = 1, int pageSize = 10)
        {
            var contactsQuery = _dbContext.Contacts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string lowerSearchTerm = searchTerm.ToLower();
                contactsQuery = contactsQuery.Where(c =>
                    (c.FirstName ?? string.Empty).ToLower().Contains(lowerSearchTerm) ||
                    (c.LastName ?? string.Empty).ToLower().Contains(lowerSearchTerm) ||
                    (c.Email ?? string.Empty).ToLower().Contains(lowerSearchTerm));
            }

            var totalContacts = await contactsQuery.CountAsync();
            var contacts = await contactsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                TotalCount = totalContacts,
                Contacts = contacts
            });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactById(int id)
        {
            var contact = await _dbContext.Contacts.FindAsync(id);
            return Ok(contact);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Contact contact)
        {
            if (contact == null)
            {
                return BadRequest("Contact cannot be null.");
            }

            // Optionally, validate the model (e.g., using ModelState)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Add the new contact
            await _dbContext.Contacts.AddAsync(contact);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContactById), new { id = contact.Id }, contact);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Contact contact)
        {
            if (contact == null)
            {
                return BadRequest("Contact cannot be null.");
            }

            if (id != contact.Id)
            {
                return BadRequest("Contact ID mismatch.");
            }

            var existingContact = await _dbContext.Contacts.FindAsync(id);
            if (existingContact == null)
            {
                return NotFound($"Contact with ID {id} not found.");
            }

            // Update the properties
            existingContact.FirstName = contact.FirstName;
            existingContact.LastName = contact.LastName;
            existingContact.Email = contact.Email;

            await _dbContext.SaveChangesAsync();

            return Ok(existingContact); // Return the updated contact
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            // Find the contact by ID
            var existingContact = await _dbContext.Contacts.FindAsync(id);
            if (existingContact == null)
            {
                return NotFound($"Contact with ID {id} not found.");
            }

            // Remove the contact from the database
            _dbContext.Contacts.Remove(existingContact);
            await _dbContext.SaveChangesAsync();

            // Return a No Content response
            return NoContent();
        }
    }
}