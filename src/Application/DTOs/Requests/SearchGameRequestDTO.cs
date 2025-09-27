using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class SearchGameRequestDTO
    {
        public required string Title { get; set; }
        public required decimal Price { get; set; }
        public required string Description { get; set; }
        public required List<GameGenre> Genre { get; set; }
        
        public int Page {  get; set; }
    }
}
