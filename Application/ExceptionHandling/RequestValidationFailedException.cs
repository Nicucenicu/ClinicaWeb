namespace ClinicaWeb.Application.ExceptionHandling
{
    public class RequestValidationFailedException : ApplicationException
    {
        public RequestValidationFailedException(IEnumerable<string> failures)
        {
            Failures = failures;
        }
        public IEnumerable<string> Failures { private set; get; }
    }
}
