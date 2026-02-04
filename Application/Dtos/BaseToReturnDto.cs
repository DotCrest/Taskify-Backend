namespace Application.Dtos;

public class BaseToReturnDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}
