namespace Ecom.IdentityServer.Models
{
    public class Result<T>
    {
        public Result() { }
        public bool IsSuccess { get; set; }
        public string? Noti { get; set; }
        public T? Data { get; set; }

        protected Result(bool isSuccess, T? data, string? noti)
        {
            IsSuccess = isSuccess;
            Data = data;
            Noti = noti;
        }
        public static Result<T> Success(T data, string mess)
       => new(true, data, mess);

        public static Result<T> Failure(string error, bool IsSuccess = false)
            => new(IsSuccess, default, error);
    }
}
