namespace Agotbg.Server.Utilities
{
  public class Result
  {
    public static Result SUCCESS() { return new Result { Success = true }; }
    public static Result FAILURE(string message) { return new Result { Success = false, Message = message }; }

    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
  }
}
