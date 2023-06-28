namespace RecordHub.Shared.DTO
{
    public class BaseResponseDTO
    {
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
