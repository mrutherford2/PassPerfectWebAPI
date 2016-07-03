
namespace PassPerfectWebAPI.Models
{
    public class TestModel
    {
        public string GeneratedPassword { get; set; }
        public string HashOfPassword { get; set; }
        public string CipherText { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}