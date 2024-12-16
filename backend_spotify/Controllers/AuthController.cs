using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly NeonDbContext _context;
    private readonly OtpService _otpService;

    public AuthController(NeonDbContext context, OtpService otpService)
    {
        _context = context;
        _otpService = otpService;
    }

    [HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterRequest request)
{
    if (request.Password != request.ConfirmPassword)
    {
        return BadRequest("Password and Confirm Password do not match.");
    }

    if (await _context.Users.AnyAsync(u => u.Email == request.Email))
    {
        return BadRequest("Email already exists.");
    }

    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

    var user = new User
    {
        FullName = request.FullName,
        Email = request.Email,
        Password = hashedPassword,
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return Ok("User registered successfully.");
}

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            return Unauthorized("Invalid password.");
        }

        return Ok("User signed in successfully.");
    }
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        var otp = _otpService.GenerateOtp(request.Email);
        SendEmail(request.Email, otp);

        return Ok("OTP has been sent to your email.");
    }

    [HttpPost("verify-otp")]
    public IActionResult VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        if (!_otpService.ValidateOtp(request.Email, request.Otp))
        {
            return BadRequest("Invalid or expired OTP.");
        }

        return Ok("OTP verified successfully.");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (request.NewPassword != request.ConfirmNewPassword)
        {
            return BadRequest("New Password and Confirm Password do not match.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _context.SaveChangesAsync();

        return Ok("Password reset successfully.");
    }

    private void SendEmail(string toEmail, string otp)
    {
        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new System.Net.NetworkCredential("adodoo.hr.company@gmail.com", "jozqsqffwavpnfeq"),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("adodoo.hr.company@gmail.com"),
            Subject = "Your OTP Code",
            Body = $"Your OTP is: {otp}. It is valid for 5 minutes.",
            IsBodyHtml = false,
        };

        mailMessage.To.Add(toEmail);
        smtpClient.Send(mailMessage);
    }
}
