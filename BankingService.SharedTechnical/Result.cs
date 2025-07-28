namespace BankingService.SharedTechnical
{
    public class Result
    {
        public static Result Success()
        {
            return new Result
            {
                IsSuccess = true,
            };
        }

        public static Result Failure(string errorMessage)
        {
            return new Result
            {
                IsSuccess = false,
                Message = errorMessage
            };
        }

        public bool IsSuccess { get; protected set; }
        public string Message { get; protected set; } = "Success";
    }

    public class Result<T> : Result
    {
        public static Result<T> Success(T result)
        {
            return new Result<T>
            {
                IsSuccess = true,
                Payload = result
            };
        }

        public static new Result<T> Failure(string errorMessage)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Message = errorMessage
            };
        }

        public T? Payload { get; private set; } = default;
    }
}
