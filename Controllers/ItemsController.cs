using Microsoft.AspNetCore.Mvc;
using Catalog.Repositories;
using System.Collections.Generic;
using Catalog.Entities;
using System;
using System.Linq;
using Catalog.Dtos;

namespace Catalog.Controllers
{   
    [ApiController] //Brings additional behaviors to the class 
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository repository; //Some bad choices will be made for didatical purposes

        public ItemsController(IItemsRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet] //Here is the verb indicating which methos is gonna be called 
        public IEnumerable<ItemDto> GetItems()
        {
            var items = repository.GetItems().Select(item => item.AsDto());
            return items;
        }
        //ROUTE = GET /items/id
        [HttpGet("{id}")]
        public ActionResult<ItemDto> GetItem(Guid id) //O ActionResult allow us to return more than one type of data (in our case null or Item) in a method
        {
            var item = repository.GetItem(id);
            if(item is null)
            {
                return NotFound();
            }
            return item.AsDto();
        }
        //POST / items
        [HttpPost] //comvention is return the object and the adress
        public ActionResult<ItemDto> CreateItem(CreateItemDto itemDto)
        {
            Item item = new(){
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            repository.CreateItem(item); 
            return CreatedAtAction(nameof(GetItem), new {id = item.Id}, item.AsDto());
        }
        // PUT / items / id
        [HttpPut("{id}")] //convention is no result
        public ActionResult UpdateItem(Guid id, UpdateItemDto itemDto)
        {
            var existingItem = repository.GetItem(id);
            if (existingItem is null)
            {
                return NotFound();
            }
            Item updatedItem = existingItem with { //WITH EXPRESSIONS WE CREATE A COPY OF THE ITEM WITH ONLY THE MODIFICATIONS PASSED ON THE METHOD
                Name = itemDto.Name,
                Price = itemDto.Price
            };
            repository.UpdateItem(updatedItem);
            return NoContent();
        }
        // DEL /items /id
        [HttpDelete("{id}")]
        public ActionResult DeleteItem(Guid id)
        {
             var existingItem = repository.GetItem(id);
            if (existingItem is null)
            {
                return NotFound();
            }
            repository.DeleteItem(id);
            return NoContent();
        }
      
    }

}