namespace InventarioSystem.Core.DTOs
{
    public class ServiceResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ServiceResponse<T> SuccessResponse(T data, string message = "Operación exitosa")
        {
            return new ServiceResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ServiceResponse<T> FailureResponse(string message)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Message = message,
                Data = default
            };
        }
    }
}
