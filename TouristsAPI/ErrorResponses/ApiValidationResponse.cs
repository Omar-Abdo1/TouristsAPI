namespace TouristsAPI.ErrorResponses;

public class ApiValidationResponse : ApiErrorResponse
{
    public IEnumerable<string>? Errors { get; set; }

    public ApiValidationResponse() : base(400, "One or more validation errors occurred.")
    {
        Errors = new List<string>();
    }
}