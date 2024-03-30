namespace ClinicaWeb.Shared.ExceptionHandlingModels
{
    public class ErrorResponse
    {
        public ErrorResponse()
        {

        }

        public ErrorResponse(FailureReason failureReason)
        {
            FailureReason = failureReason;
        }

        public ErrorResponse(FailureReason failureReason, IEnumerable<string> messages)
        {
            FailureReason = failureReason;
            Messages = messages;
        }

        public ErrorResponse(FailureReason failureReason, string message)
        {
            FailureReason = failureReason;
            Message = message;
        }

        public FailureReason FailureReason { get; set; }
        public string Message { get; set; }
        public IEnumerable<string> Messages { get; set; }
    }
}
