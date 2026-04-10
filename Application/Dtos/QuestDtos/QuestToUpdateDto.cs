namespace Application.Dtos.QuestDtos
{
    public class QuestToUpdateDto
    {
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public int Status { get; set; }
        public int Priority { get; set; }
    }
}
