using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Response
{
    public class SearchGameResponseDTO
    {
        public SearchGameResponseDTO(string title, decimal price) { 
            Title = title;

            Price = price;
        }
        public required string Title { get; set; }
        public required decimal Price { get; set; }
    }
}
