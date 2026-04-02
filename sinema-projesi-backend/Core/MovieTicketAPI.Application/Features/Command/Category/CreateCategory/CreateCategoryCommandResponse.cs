namespace MovieTicketAPI.Application.Features.Command.Category.CreateCategory
{
    public class CreateCategoryCommandResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Id { get; set; }
    }
}
