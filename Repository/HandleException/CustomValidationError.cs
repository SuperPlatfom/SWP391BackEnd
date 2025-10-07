namespace Repository.HandleException
{
    public class CustomValidationError : Exception
    {
        public Dictionary<string, string> Errors { get; }

        public CustomValidationError(Dictionary<string, string> errors)
        {
            Errors = errors;
        }
    }

}
