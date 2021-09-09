using Microsoft.AspNetCore.Mvc;
using Catalog.Repositories;
using System.Collections.Generic;
using Catalog.Entities;
using System;
using System.Linq;
using Catalog.Dtos;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.Controllers
{   
    [Authorize]
    [ApiController] //Brings additional behaviors to the class 
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository repository;

        public ItemsController(IItemsRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet] //Here is the verb indicating which methos is gonna be called 
        public async Task<IEnumerable<ItemDto>> GetItemsAsync()
        {
            var items = (await repository.GetItemsAsync())
                        .Select(item => item.AsDto());
            return items;
        }
        //ROUTE = GET /items/id
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id) //O ActionResult allow us to return more than one type of data (in our case null or Item) in a method
        {
            var item = await repository.GetItemAsync(id);
            if(item is null)
            {
                return NotFound();
            }
            return item.AsDto();
        }
        //POST / items
        [HttpPost] //convention is return the object and the adress
        public async Task<ActionResult<ItemDto>> CreateItem(CreateItemDto itemDto)
        {
            Item item = new(){
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await repository.CreateItemAsync(item); 
            return CreatedAtAction(nameof(GetItemAsync), new {id = item.Id}, item.AsDto());
        }
        // PUT / items / id
        [HttpPut("{id}")] //convention is no result
        public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto itemDto)
        {
            var existingItem = await repository.GetItemAsync(id);
            if (existingItem is null)
            {
                return NotFound();
            }
            Item updatedItem = existingItem with { //WITH EXPRESSIONS WE CREATE A COPY OF THE ITEM WITH ONLY THE MODIFICATIONS PASSED ON THE METHOD
                Name = itemDto.Name,
                Price = itemDto.Price
            };
            await repository.UpdateItemAsync(updatedItem);
            return NoContent();
        }
        // DEL / items / id
        public async Task<ActionResult> DeleteItem(Guid id)
        {
             var existingItem = repository.GetItemAsync(id);
            if (existingItem is null)
            {
                return NotFound();
            }
            await repository.DeleteItemAsync(id);
            return NoContent();
        }
    }     

}