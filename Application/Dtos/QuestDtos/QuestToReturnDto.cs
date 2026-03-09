using Application.Dtos.TagDtos;
using Application.Dtos.UserQuestDtos;

namespace Application.Dtos.QuestDtos
{
    public class QuestToReturnDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = null!;
        public string Priority { get; set; } = null!;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public ICollection<UserQuestDto> UserQuests { get; set; } = [];
        public ICollection<TagToReturnDto> Tags { get; set; } = [];
        public string AuthorName { get; set; } = null!;
    }
}
