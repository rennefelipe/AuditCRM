using AuditCRM.Application.Features.Erps.DTOs;

namespace AuditCRM.Application.Interfaces;

public interface IErpService
{
    Task<IReadOnlyList<ErpDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ErpDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ErpDto> CreateAsync(
        CreateErpRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task<ErpDto> UpdateAsync(
        Guid id,
        UpdateErpRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task ActivateAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task DeactivateAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);
}