using Identity.Controllers.Users.Requests;
using Identity.Controllers.Users.Responses;
using Identity.Jwt.Interfaces;
using Identity.Models;
using Identity.Repositories.Users;
using Identity.Services.Emails.Interfaces;
using Identity.Services.Interfaces;
using Identity.Services.UnitOfWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers.Users;

[ApiController]
[Route("api/users")]
public sealed class UserController(
    IUserRepository userRepository,
    IJwtManager jwtManager,
    IUnitOfWork unitOfWork,
    IHashingService hashingService,
    IIdentityEmailService identityEmailService) : ControllerBase
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IHashingService _hashingService = hashingService;
    private readonly IIdentityEmailService _identityEmailService = identityEmailService;
    private readonly IJwtManager _jwtManager = jwtManager;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var isExist = await _userRepository.EmailIsExist(request.Email);

        if (!isExist)
        {
            //TODO: change logic
            return Unauthorized("Email is not exist");
        }

        //var isConfirmed = await _userRepository.EmailIsConfirmed(request.Email);

        //if (!isConfirmed)
        //{
        //    return Unauthorized("Email is not confirmed");
        //}

        var user = await _userRepository.GetByEmailAsync(request.Email);

        var passwordIsCorrect =
            _hashingService.Verify(request.Password, user.PasswordSalt, user.PasswordHash);

        if (!passwordIsCorrect)
        {
            return Unauthorized();
        }

        var token = _jwtManager.CreateTokenString(user);
        var refreshToken = _jwtManager.CreateRefreshToken();

        user.RefreshTokens.Add(refreshToken);

        await _unitOfWork.Commit();

        var response = new LoginResponse(token, refreshToken.Token);

        return Ok(response);
    }

    //TODO: Add transaction
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var emailIsExist = await _userRepository.EmailIsExist(request.Email);

        if (emailIsExist)
        {
            //TODO: change logic
            return BadRequest("Email is exist");
        }

        var usernameIsExist = await _userRepository.UsernameIsExist(request.Username);

        if (usernameIsExist)
        {
            return BadRequest("Username is exist");
        }

        var passwordSalt = _hashingService.GenerateSalt();
        var passwordHash = _hashingService.Hash(request.Password, passwordSalt);
        var emailConfirmationToken = new EmailConfirmationToken { Token = Guid.NewGuid().ToString() };

        var user = new User
        {
            Email = request.Email,
            Username = request.Username,
            RoleId = request.RoleId,
            PasswordSalt = passwordSalt,
            PasswordHash = passwordHash,
            EmailConfirmationToken = emailConfirmationToken
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.Commit();
        await _identityEmailService.SendConfirmationEmailAsync(user, request.ReturnUrl);

        return Ok();
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest request)
    {
        var isExist = await _userRepository.IsExist(request.UserId);

        if (!isExist)
        {
            return NotFound("User is not exist");
        }

        var user = await _userRepository.GetAsync(request.UserId);

        if(user.EmailConfirmationToken is null)
        {
            return BadRequest("Email is confirmed");
        }

        if (user.EmailConfirmationToken.Token != request.EmailConfirmationToken)
        {
            return BadRequest("Token is incorrect");
        }

        user.EmailConfirmationToken = null;
        _userRepository.Update(user);
        await _unitOfWork.Commit();

        return Ok();
    }

    [HttpPut("update-refresh-token")]
    public async Task<IActionResult> UpdateRefreshToken([FromBody] UpdateRefreshTokenRequest request)
    {
        var email = _jwtManager.GetEmailFromToken(request.Token);
        var isEmailExist = await _userRepository.EmailIsExist(email);

        if (!isEmailExist)
        {
            return Unauthorized("Email is not exist");
        }

        var user = await _userRepository.GetByEmailAsync(email);

        var tokenIsExist = user.RefreshTokens.Any(x => x.Token == request.RefreshToken);

        if (!tokenIsExist)
        {
            return Unauthorized("Token is not exist");
        }

        user.RefreshTokens.Remove(user.RefreshTokens.First(x => x.Token == request.RefreshToken));

        var newRefreshToken = _jwtManager.CreateRefreshToken();
        user.RefreshTokens.Add(newRefreshToken);

        _userRepository.Update(user);
        await _unitOfWork.Commit();
        var token = _jwtManager.CreateTokenString(user);

        var response = new UpdateRefreshTokenResponse(token, newRefreshToken.Token);

        return Ok(response);
    }

}
