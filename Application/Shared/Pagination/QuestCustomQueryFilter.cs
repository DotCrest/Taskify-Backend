using System.ComponentModel.DataAnnotations;

namespace Application.Shared.Pagination;

public class QuestCustomQueryFilter : QueryFilter
{
    private DateTime? _createdAt;
    private DateTime? _dueDate;
    private DateTime? _updatedAt;
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public int? CategoryId { get; set; }
    public int? TagId { get; set; }
    [DataType(DataType.Date)]
    public DateTime? CreatedAt
    {
        get => _createdAt;
        set => _createdAt = value?.Date;
    }
    [DataType(DataType.Date)]
    public DateTime? UpdatedAt
    {
        get => _updatedAt;
        set => _updatedAt = value?.Date;
    }
    [DataType(DataType.Date)]
    public DateTime? DueDate
    {
        get => _dueDate;
        set => _dueDate = value?.Date;
    }
}
