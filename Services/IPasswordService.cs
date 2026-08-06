namespace BlogPlatform.Services
{
    // Băm và kiểm tra mật khẩu — thay cho phần Identity làm sẵn trước đây
    public interface IPasswordService
    {
        string Hash(string password);
        bool Verify(string password, string passwordHash);
    }
}
