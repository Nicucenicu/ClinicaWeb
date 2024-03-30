using ClinicaWeb.Shared.ExceptionHandlingModels;

namespace ClinicaWeb.Application.ExceptionHandling
{
    public class BusinessException : ApplicationException
    {
        public BusinessException(FailureReason failureReason, string message = null)
        {
            FailureReason = failureReason;
            Message = message;
        }

        public FailureReason FailureReason { get; }
        public override string Message { get; }
    }
}
