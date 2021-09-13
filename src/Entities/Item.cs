using System;

namespace Catalog.Api.Entities
{
    public class Item
    {
        public Guid Id {get; set;}//init property inicializers (value can only be set during inicialization)
        public string Name {get; set;}
        public decimal Price {get; set;}
        public DateTimeOffset CreatedDate {get; set;}
    }
}