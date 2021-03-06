using Library.Shared.Models.Response;

namespace UserProfile.API.Application.Handlers.UpdateEmailAddress
{
    public record UpdateEmailAddressResponse : BaseResponse
    {
        public long UserId { get; init; }
        public string CurrentEmailAddress { get; init; }

        public UpdateEmailAddressResponse(Error error = null) : base(error)
        {
        }
    }
}