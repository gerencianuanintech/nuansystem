using NuanSystem.Application.Abstractions.Authentication;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityUsers.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityUsers.Commands;

public sealed class UpdateUserCommandHandler(
    IUserAdminRepository repository,
    IPasswordHasher passwordHasher) : ICommandHandler<UpdateUserCommand, UserAdminDto>
{
    public async Task<Result<UserAdminDto>> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        if (await repository.GetByIdAsync(request.Id, cancellationToken) is null)
        {
            return Result<UserAdminDto>.Failure(
                "Usuario no encontrado.",
                [new ApiError("SecurityUserNotFound", "El usuario no existe.", nameof(request.Id))]);
        }

        var userName = request.UserName.Trim();
        if (await repository.ExistsByUserNameAsync(userName, request.Id, cancellationToken))
        {
            return Result<UserAdminDto>.Failure(
                "Ya existe un usuario con ese nombre.",
                [new ApiError("UserNameAlreadyExists", "El nombre de usuario ya existe.", nameof(request.UserName))]);
        }

        await repository.UpdateAsync(new UpdateUserData(
            request.Id,
            userName,
            string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim(),
            request.EmailConfirmed,
            request.PhoneNumberConfirmed,
            string.IsNullOrWhiteSpace(request.FirstName) ? null : request.FirstName.Trim(),
            string.IsNullOrWhiteSpace(request.LastName) ? null : request.LastName.Trim(),
            request.DisplayName.Trim(),
            string.IsNullOrWhiteSpace(request.Password) ? null : passwordHasher.HashPassword(request.Password),
            request.RoleId,
            request.IsActive,
            request.IsLocked,
            request.CanUseWeb,
            request.CanUseMobile,
            request.MustChangePassword,
            request.LockoutEndAt,
            request.TwoFactorEnabled,
            string.IsNullOrWhiteSpace(request.ProfileImageUrl) ? null : request.ProfileImageUrl.Trim(),
            request.ProfileImage,
            string.IsNullOrWhiteSpace(request.ProfileImageContentType) ? null : request.ProfileImageContentType.Trim(),
            string.IsNullOrWhiteSpace(request.ProfileImageFileName) ? null : request.ProfileImageFileName.Trim(),
            request.AuditUserId,
            request.AuditUserName?.Trim()), cancellationToken);

        var user = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("El usuario fue actualizado pero no pudo consultarse.");

        return Result<UserAdminDto>.Success(user, "Usuario actualizado correctamente.");
    }
}

