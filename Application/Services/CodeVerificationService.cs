using Application.Dtos;
using Application.ServiceAbstractions;
using Application.Shared;
using Application.Shared.Errors;
using Domain.Contracts;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace Application.Services;

public class CodeVerificationService(IUnitOfWork unitOfWork,
                                 IEmailService emailService,
                                 ILogger<CodeVerificationService> logger) : ICodeVerificationService
{
    private readonly IGenericRepository<VerificationCode> verificationCodeRepo = unitOfWork.Repository<VerificationCode>();
    public async Task<BaseToReturnDto> SendCode(string email)
    {
        // check if code already exists for email and is active
        string codeToSend = await CheckForExistedCode(email);
        // if code exists and active return it, else generate new code then save to db
        if (string.IsNullOrEmpty(codeToSend))
        {
            codeToSend = GenerateCode(6);
            await SaveCode(email, codeToSend);
        }
        // send code to email
        var subject = "taskify.com - Account Verification!";
        var body = $@"
            Welcome to taskify.com! 
            This is your Verification Code {codeToSend},
            Please enter this code to verify your identity. 
            'DO NOT SHARE IT' with anyone";
        await emailService.SendEmailAsync(email, subject, body);
        // any errors come from email service will be globally handled by exception middleware
        return new BaseToReturnDto
        {
            IsSuccess = true,
            Message = "Verification code sent successfully."
        };
    }
    // map to endpoint
    public async Task<Result<BaseToReturnDto>> ValidateCode(VerifyCodeDto verifyCodeDto)
    {
        // get code from db by email
        var codeFromDb = await GetCodeByEmail(verifyCodeDto.Email);
        // check nullability
        if (codeFromDb == null)
        {
            logger.LogWarning("Verification code not found for email: {Email}", verifyCodeDto.Email);
            return Result<BaseToReturnDto>.Failure(VerificationCodeErrors.InvalidCode);
        }
        // check if code is active
        if (!codeFromDb.IsActive)
        {
            logger.LogWarning("Verification code expired for email: {Email}", verifyCodeDto.Email);
            return Result<BaseToReturnDto>.Failure(VerificationCodeErrors.InvalidCode);
        }
        // compare codes
        if (codeFromDb.Code != verifyCodeDto.Code)
        {
            logger.LogWarning("Verification code mismatch for email: {Email}", verifyCodeDto.Email);
            return Result<BaseToReturnDto>.Failure(VerificationCodeErrors.InvalidCode);
        }
        // if valid update IsVerified to true in db
        codeFromDb.IsVerified = true;
        await UpdateCode(codeFromDb);
        // return result
        return Result<BaseToReturnDto>.Success(new BaseToReturnDto
        {
            IsSuccess = true,
            Message = "Code verified successfully."
        });
    }
    public async Task<bool> IsValidated(string email)
    {
        var codeFromDb = await GetCodeByEmail(email);
        if (codeFromDb != null && codeFromDb.IsVerified)
        {
            await DeleteCode(codeFromDb);
            return true;
        }
        return false;
    }
    private async Task<VerificationCode?> GetCodeByEmail(string email)
    {
        return await verificationCodeRepo.Find(vc => vc.Email == email);
        // get code from db by email
    }
    private async Task SaveCode(string email, string code)
    {

        var verificationCode = new VerificationCode
        {
            Email = email,
            Code = code,
            CreatedAt = DateTime.UtcNow
        };
        await verificationCodeRepo.AddAsync(verificationCode);
        await unitOfWork.SaveAsync();
    }
    private async Task DeleteCode(VerificationCode code)
    {
        // delete code from db
        verificationCodeRepo.Delete(code);
        await unitOfWork.SaveAsync();
    }
    private async Task UpdateCode(VerificationCode code)
    {
        verificationCodeRepo.Update(code);
        await unitOfWork.SaveAsync();
    }
    private async Task<string> CheckForExistedCode(string email)
    {
        var existCode = await GetCodeByEmail(email);
        string codeToReturn;
        if (existCode is not null && existCode.IsActive)
        {
            codeToReturn = existCode.Code;
            return codeToReturn;
        }
        else if (existCode is not null && !existCode.IsActive)
        {
            // generate code
            codeToReturn = GenerateCode(6);
            existCode.Code = codeToReturn;
            // update activiation time
            existCode.CreatedAt = DateTime.UtcNow;
            verificationCodeRepo.Update(existCode);
            await unitOfWork.SaveAsync();
            return codeToReturn;
        }
        return string.Empty;
    }
    private string GenerateCode(int length) => RandomNumberGenerator.GetInt32(0, (int)Math.Pow(10, length)).ToString($"D{length}");
}
