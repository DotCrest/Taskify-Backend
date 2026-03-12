using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.QuestDtos
{
    public class QuestToCreateDto
    {
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CategoryId { get; set; }
        public int Status { get; set; }
        public int Priority { get; set; }
    }
}
