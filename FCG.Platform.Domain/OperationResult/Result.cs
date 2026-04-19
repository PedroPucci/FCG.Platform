using FluentValidation.Results;

namespace FCG.Platform.Domain.OperationResult
{
    public class Result<T>
    {
        public Result(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public Result(bool success, IEnumerable<ValidationFailure> errors)
        {
            Success = success;
            Errors = errors;
        }

        public Result() { }

        public bool Success { get; set; }
        public T Data { get; set; }
        public string? AccessToken { get; set; }
        public string Message { get; set; }
        public IEnumerable<ValidationFailure> Errors { get; }

        public static Result<T> Ok(T data) => new() { Success = true, Data = data };

        public static Result<T> Ok(string responseMessage = null, T responseData = default)
        {
            return new Result<T>(success: true, message: responseMessage, data: responseData);
        }

        public static Result<T> Error(string responseMessage)
        {
            return new Result<T>(success: false, responseMessage, data: default);
        }
    }
}