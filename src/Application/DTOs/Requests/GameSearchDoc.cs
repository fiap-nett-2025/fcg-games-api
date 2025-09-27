using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class GameSearchDoc
    {
        public string Title { get; set; } = default!;
        public decimal Price { get; set; }
        public string Description { get; set; } = default!;
        public List<GameGenre> Genre { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
