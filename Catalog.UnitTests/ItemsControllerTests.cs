using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Catalog.Api.Repositories;
using Catalog.Api.Entities;
using Catalog.src.Controllers;
using Catalog.Api.Dtos;

namespace Catalog.UnitTests
{
    public class ItemsControllerTest
    {
        private readonly Mock<IItemsRepository> repositoryStub = new();
        
        private readonly Random random=new();

        [Fact]
        public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound() //UnitOfWork_StateUnderTest_ExpectNehavior name convention
        {            
            // Arrange
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                            .ReturnsAsync((Item)null);

            var controller = new ItemsController(repositoryStub.Object);
            
            // Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Fact]
        public async Task GetItemAsync_WithExistingItem_ReturnsItem()
        {
            //Arange    
            var expectedItem = CreateRandomItem();
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                           .ReturnsAsync(expectedItem);

            var controller = new ItemsController(repositoryStub.Object);

            //Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            //Assert
            result.Value.Should().BeEquivalentTo
            (expectedItem,
            options => options.ComparingByMembers<Item>());
        }

        [Fact]
        public async Task GetItemsAsync_WithExistingItem_ReturnsAllItems()
        {
            //Arrange
            var expectedItems = new[] {CreateRandomItem(), CreateRandomItem(), CreateRandomItem()};
            repositoryStub.Setup(repo => repo.GetItemsAsync())
                           .ReturnsAsync(expectedItems);
            var controller = new ItemsController(repositoryStub.Object);

            //Act
            var actualItems = await controller.GetItemsAsync();

            //Assert
            actualItems.Should().BeEquivalentTo
            (expectedItems,
            options => options.ComparingByMembers<Item>());

        }

        [Fact]
        public async Task CreateItemAsync_WithitemToCreate_ReturnsCreatedItem()
        {
        //Arrange
        var itemToCreate = new CreateItemDto(Guid.NewGuid().ToString(),random.Next(1000));
            
        var controller = new ItemsController(repositoryStub.Object);

        //Act
        var result = await controller.CreateItemAsync(itemToCreate);

        //Assert
        var createdItem = (result.Result as CreatedAtActionResult).Value as ItemDto;
        itemToCreate.Should().BeEquivalentTo
            (createdItem,
            options => options.ComparingByMembers<ItemDto>().ExcludingMissingMembers()
        );
        createdItem.Id.Should().NotBeEmpty();
        
        } 
        private Item CreateRandomItem()
        {
            return new(){
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Price = random.Next(1000),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}
 