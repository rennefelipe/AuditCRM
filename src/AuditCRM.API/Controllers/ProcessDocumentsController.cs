using System.Security.Claims;
using AuditCRM.API.Models;
using AuditCRM.Application.Features.ProcessDocuments.DTOs;
using AuditCRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuditCRM.API.Controllers;

[ApiController]
[Route("api/process-documents")]
[Authorize]
public sealed class ProcessDocumentsController : ControllerBase
{
    private const long MaxFileSize = 20 * 1024 * 1024;

    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf",
            ".doc",
            ".docx",
            ".xls",
            ".xlsx",
            ".csv",
            ".txt",
            ".jpg",
            ".jpeg",
            ".png"
        };

    private readonly IProcessDocumentService _service;
    private readonly IWebHostEnvironment _environment;

    public ProcessDocumentsController(
        IProcessDocumentService service,
        IWebHostEnvironment environment)
    {
        _service = service;
        _environment = environment;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProcessDocumentDto>>> GetAll(
        [FromQuery] Guid storeProcessId,
        CancellationToken cancellationToken)
    {
        if (storeProcessId == Guid.Empty)
        {
            return BadRequest(new
            {
                message = "O processo da loja é obrigatório."
            });
        }

        var documents = await _service.GetAllAsync(
            storeProcessId,
            cancellationToken);

        return Ok(documents);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProcessDocumentDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var document = await _service.GetByIdAsync(
            id,
            cancellationToken);

        if (document is null)
        {
            return NotFound(new
            {
                message = "Documento não encontrado."
            });
        }

        return Ok(document);
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ProcessDocumentDto>> Upload(
        [FromForm] ProcessDocumentUploadRequest request,
        CancellationToken cancellationToken)
    {
        if (request.StoreProcessId == Guid.Empty)
        {
            return BadRequest(new
            {
                message = "O processo da loja é obrigatório."
            });
        }

        if (request.File is null || request.File.Length == 0)
        {
            return BadRequest(new
            {
                message = "Selecione um arquivo para upload."
            });
        }

        if (request.File.Length > MaxFileSize)
        {
            return BadRequest(new
            {
                message = "O arquivo deve possuir no máximo 20 MB."
            });
        }

        var extension = Path
            .GetExtension(request.File.FileName)
            .ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(extension) ||
            !AllowedExtensions.Contains(extension))
        {
            return BadRequest(new
            {
                message =
                    "Tipo de arquivo não permitido. " +
                    "São aceitos PDF, Word, Excel, CSV, TXT, JPG e PNG."
            });
        }

        var originalFileName =
            Path.GetFileName(request.File.FileName);

        var storedFileName =
            $"{Guid.NewGuid():N}{extension}";

        var relativeDirectory = Path.Combine(
            "processes",
            request.StoreProcessId.ToString(),
            "documents");

        var relativePath = Path.Combine(
            relativeDirectory,
            storedFileName);

        var storageRoot = GetStorageRoot();

        var physicalDirectory = Path.Combine(
            storageRoot,
            relativeDirectory);

        Directory.CreateDirectory(physicalDirectory);

        var physicalPath = Path.Combine(
            storageRoot,
            relativePath);

        try
        {
            await using (var stream =
                new FileStream(
                    physicalPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None))
            {
                await request.File.CopyToAsync(
                    stream,
                    cancellationToken);
            }

            var document = await _service.CreateAsync(
                request.StoreProcessId,
                GetCurrentUserId(),
                originalFileName,
                storedFileName,
                NormalizeRelativePath(relativePath),
                string.IsNullOrWhiteSpace(request.File.ContentType)
                    ? "application/octet-stream"
                    : request.File.ContentType,
                request.File.Length,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = document.Id },
                document);
        }
        catch (InvalidOperationException ex)
        {
            DeletePhysicalFileIfExists(physicalPath);

            return Conflict(new
            {
                message = ex.Message
            });
        }
        catch
        {
            DeletePhysicalFileIfExists(physicalPath);
            throw;
        }
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(
        Guid id,
        CancellationToken cancellationToken)
    {
        var document = await _service.GetEntityByIdAsync(
            id,
            cancellationToken);

        if (document is null)
        {
            return NotFound(new
            {
                message = "Documento não encontrado."
            });
        }

        var storageRoot = GetStorageRoot();

        var relativePath = document.RelativePath
            .Replace(
                '/',
                Path.DirectorySeparatorChar);

        var physicalPath = Path.GetFullPath(
            Path.Combine(
                storageRoot,
                relativePath));

        if (!IsPathInsideStorage(
                storageRoot,
                physicalPath))
        {
            return BadRequest(new
            {
                message = "Caminho de documento inválido."
            });
        }

        if (!System.IO.File.Exists(physicalPath))
        {
            return NotFound(new
            {
                message =
                    "O registro do documento existe, " +
                    "mas o arquivo físico não foi encontrado."
            });
        }

        var bytes = await System.IO.File.ReadAllBytesAsync(
            physicalPath,
            cancellationToken);

        return File(
            bytes,
            document.ContentType,
            document.OriginalFileName);
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var document = await _service.GetEntityByIdAsync(
            id,
            cancellationToken);

        if (document is null)
        {
            return NotFound(new
            {
                message = "Documento não encontrado."
            });
        }

        try
        {
            await _service.DeleteAsync(
                id,
                GetCurrentUserId(),
                cancellationToken);

            var storageRoot = GetStorageRoot();

            var relativePath = document.RelativePath
                .Replace(
                    '/',
                    Path.DirectorySeparatorChar);

            var physicalPath = Path.GetFullPath(
                Path.Combine(
                    storageRoot,
                    relativePath));

            if (IsPathInsideStorage(
                    storageRoot,
                    physicalPath))
            {
                DeletePhysicalFileIfExists(
                    physicalPath);
            }

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    private string GetStorageRoot()
    {
        var storageRoot = Path.GetFullPath(
            Path.Combine(
                _environment.ContentRootPath,
                "..",
                "..",
                "storage"));

        Directory.CreateDirectory(
            storageRoot);

        return storageRoot;
    }

    private static string NormalizeRelativePath(
        string relativePath)
    {
        return relativePath.Replace(
            Path.DirectorySeparatorChar,
            '/');
    }

    private static bool IsPathInsideStorage(
        string storageRoot,
        string physicalPath)
    {
        var normalizedRoot = Path.GetFullPath(
            storageRoot)
            .TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;

        var normalizedFile = Path.GetFullPath(
            physicalPath);

        return normalizedFile.StartsWith(
            normalizedRoot,
            StringComparison.OrdinalIgnoreCase);
    }

    private static void DeletePhysicalFileIfExists(
        string physicalPath)
    {
        if (System.IO.File.Exists(
                physicalPath))
        {
            System.IO.File.Delete(
                physicalPath);
        }
    }

    private Guid? GetCurrentUserId()
    {
        var value = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        return Guid.TryParse(
            value,
            out var userId)
            ? userId
            : null;
    }
}