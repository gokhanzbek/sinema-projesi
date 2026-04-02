namespace MovieTicketAPI.Application.Features.Queries.AppUser.GetCurrentUserProfile
{
    public class GetCurrentUserProfileQueryResponse
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
    }
}
