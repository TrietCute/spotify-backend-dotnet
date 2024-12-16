using System.Collections.Concurrent;

public class OtpService
{
    private readonly ConcurrentDictionary<string, (string Otp, DateTime Expiry)> _otpStore = new();

    public string GenerateOtp(string email)
    {
        var otp = new Random().Next(100000, 999999).ToString(); // 6 chữ số
        var expiry = DateTime.UtcNow.AddMinutes(5); // OTP hết hạn sau 5 phút

        _otpStore[email] = (otp, expiry);
        return otp;
    }

    public bool ValidateOtp(string email, string otp)
    {
        if (_otpStore.TryGetValue(email, out var otpData))
        {
            if (otpData.Otp == otp && otpData.Expiry > DateTime.UtcNow)
            {
                _otpStore.TryRemove(email, out _); // Xóa OTP sau khi sử dụng
                return true;
            }
        }
        return false;
    }
}
