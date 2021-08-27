namespace Serious.Abbot.CommandLine
{
    public class CodeResult
    {
        public static CodeResult Success(string code)
        {
            return new CodeResult(true, code, null);
        }
        
        public static CodeResult Fail(string message)
        {
            return new CodeResult(false, null, message);
        }

        CodeResult(bool success, string? code, string? errorMessage)
        {
            IsSuccess = success;
            Code = code;
            ErrorMessage = errorMessage;
        }

        public bool IsSuccess { get; }
        
        public string? ErrorMessage { get; }
        
        public string? Code { get; }
    }
}