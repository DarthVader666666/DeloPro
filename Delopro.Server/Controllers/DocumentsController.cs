using Delopro.Bll;
using Delopro.Bll.Interfaces;
using Delopro.Server.Configuration;
using Delopro.Server.Enums;
using Delopro.Server.Models;

using Google;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using System.Text.Json;

namespace Delopro.Server.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowClient")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly string? docsPath;
        private readonly string? documentsDirectoryName;
        private readonly string? webRootPath;

        private readonly IDriveService _driveService;
        private readonly IMemoryCache _memoryCache;

        public DocumentsController(IDriveService driveService, IMemoryCache memoryCache)
        {
            docsPath = ConfigurationHelper.DocsPath;
            webRootPath = ConfigurationHelper.WebRootPath;
            documentsDirectoryName = ConfigurationHelper.DocsFolderName;
            _driveService = driveService;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetDocuments()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.DocumentNodesKey, out IEnumerable<DocumentResponse>? documentResponse))
            {
                try
                {
                    documentResponse = new DirectoryInfo(docsPath ?? string.Empty).GetFiles()
                        .Select(x =>
                            new DocumentResponse
                            {
                                Name = x.Name,
                                Path = docsPath
                            }
                        );

                    _memoryCache.Set(CacheKeys.DocumentsKey, documentResponse, TimeSpan.FromMinutes(5));
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = ex.Message });
                }
            }

            return Ok(documentResponse);
        }

        void FillNodes(string? path, DocumentNode? parentNode = null)
        {
            parentNode ??= new DocumentNode();

            var rootDirectoryInfo = new DirectoryInfo(path ?? string.Empty);
            var directoryInfoArray = rootDirectoryInfo.GetDirectories();
            var shortPath = path?.Replace(webRootPath + Path.DirectorySeparatorChar, string.Empty);

            parentNode.Key = $"{shortPath?.Replace(Path.DirectorySeparatorChar, '-') + (shortPath == documentsDirectoryName ? "" : '-' + path)}";
            parentNode.Icon = shortPath == documentsDirectoryName ? "pi pi-ellipsis-h" : "pi pi-folder";
            parentNode.Data = new TreeNode
            {
                Name = shortPath == documentsDirectoryName ? "" : rootDirectoryInfo.Name,
                Path = shortPath,
                Type = shortPath == documentsDirectoryName ? nameof(DocumentType.Root).ToLower() : nameof(DocumentType.Folder).ToLower(),
            };

            foreach (var directoryInfo in directoryInfoArray ?? [])
            {
                var node = new DocumentNode();
                parentNode.Children?.Add(node);
                FillNodes(directoryInfo.FullName, node);
            }

            var files = rootDirectoryInfo.GetFiles();
            var fileNodes = files.Select(f => new DocumentNode
            {
                Key = $"{shortPath.Replace(Path.DirectorySeparatorChar, '-')}-{f.FullName}",
                Icon = "pi pi-file",
                Data = new TreeNode
                {
                    Name = f.Name,
                    Path = Path.Combine(shortPath ?? string.Empty, f.Name),
                    Type = nameof(DocumentType.File).ToLower(),
                    Size = ByteLengthToSizeString(f.Length),
                }
            });

            parentNode.Children?.AddRange(fileNodes);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetDocumentNodes()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.DocumentNodesKey, out DocumentNode? node))
            {
                try
                {
                    node = new DocumentNode();
                    FillNodes(docsPath, node);
                    _memoryCache.Set(CacheKeys.DocumentNodesKey, node, TimeSpan.FromMinutes(5));
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = ex.Message });
                }
            }

            return Ok(new List<DocumentNode?> { node });
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "Owner, Admin")]
        public async Task<IActionResult> DeleteDocument([FromBody] DocumentPathModel documentPathModel)
        {
            if (documentPathModel == null || documentPathModel.Path == null || documentPathModel.Type == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка при удалении файла" });
            }

            var path = Path.Combine(webRootPath ?? string.Empty, documentPathModel.Path);

            try
            {
                if (documentPathModel.Type.Equals(nameof(DocumentType.File), StringComparison.OrdinalIgnoreCase))
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                        _ = Task.Run(() => _driveService.Delete(path));

                        _memoryCache.Remove(CacheKeys.DocumentsKey);
                        _memoryCache.Remove(CacheKeys.DocumentNodesKey);
                    }
                    else
                    {
                        return NotFound(new { errorText = "Файл не найден" });
                    }
                }
                else
                {
                    if (Directory.Exists(path))
                    {
                        foreach (var filePath in Directory.GetFiles(path))
                        {
                            System.IO.File.Delete(filePath);
                        }

                        Directory.Delete(path, recursive: true);
                        _ = Task.Run(() => _driveService.Delete(path, isFolder: true));

                        _memoryCache.Remove(CacheKeys.DocumentsKey);
                        _memoryCache.Remove(CacheKeys.DocumentNodesKey);
                    }
                    else
                    {
                        return NotFound(new { errorText = "Папка не найдена" });
                    }
                }
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = $"Ошибка при удалении\n\r{ex.Message}" });
            }

            return Ok(new { okText = documentPathModel.Type.Equals(nameof(DocumentType.File), StringComparison.OrdinalIgnoreCase) 
                ? $"Файл \"{Path.GetFileName(path)}\" успешно удален" 
                : $"Папка \"{path.Split(Path.DirectorySeparatorChar).Last()}\" успешно удалена" });
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "Owner, Admin")]
        public async Task<IActionResult> AddFolder([FromBody] FolderPathModel? folderPathModel)
        {
            var reader = new StreamReader(HttpContext.Request.Body);
            var folderPath = folderPathModel?.FolderPath?.Replace('-', ' ');
            var path = Path.Combine(webRootPath!, folderPath!);

            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path ?? string.Empty);
                    _ = Task.Run(() => _driveService.CreateFolder(folderPath ?? string.Empty));

                    _memoryCache.Remove(CacheKeys.DocumentsKey);
                    _memoryCache.Remove(CacheKeys.DocumentNodesKey);
                }
                else
                {
                    return BadRequest(new { errorText = "Папка уже существует" });
                }
            }
            catch (GoogleApiException)
            {
                return StatusCode(StatusCodes.Status304NotModified, new { warningText = "Папка не была создана в облаке" });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка при создании папки" });
            }

            return Ok(new { okText = $"Папка \"{folderPath}\" успешно создана" });
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "Owner, Admin")]
        public async Task<IActionResult> UploadDocuments([FromForm] DocumentUploadForm? uploadDocumentForm)
        {
            if (uploadDocumentForm == null || uploadDocumentForm.Files == null || !uploadDocumentForm.Files.Any())
            {
                return BadRequest(new { errorText = "Нет выбранных файлов" });
            }

            var fileNames = new List<string>();
            var filePaths = new List<string>();

            try
            {
                foreach (IFormFile file in uploadDocumentForm.Files)
                {
                    fileNames.Add(file.FileName);

                    var filePath = Path.Combine(webRootPath ?? string.Empty,
                        uploadDocumentForm.FolderName ?? string.Empty, file.FileName);

                    if (!System.IO.File.Exists(filePath))
                    {
                        filePaths.Add(filePath);

                        using Stream fileStream = new FileStream(filePath, FileMode.Create);
                        await file.CopyToAsync(fileStream);
                    }
                }

                _ = Task.Run(() =>
                {
                    foreach (var filePath in filePaths)
                    {
                        if (System.IO.File.Exists(filePath))
                        {
                            _driveService.CreateFile(filePath);
                        }
                    }

                    _memoryCache.Remove(CacheKeys.DocumentsKey);
                    _memoryCache.Remove(CacheKeys.DocumentNodesKey);
                });
            }
            catch (GoogleApiException)
            {
                return StatusCode(StatusCodes.Status304NotModified, new { warningText = $"Файл \"{fileNames}\" не был создан в облаке" });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = $"Ошибка загрузки файла \"{fileNames}\"" });
            }

            return Ok(new
            {
                okText = fileNames?.Count() > 1
                    ? $"Файлы \"{string.Join(", ", fileNames)}\" успешно загружены"
                    : $"Файл \"{fileNames?.First()}\" успешно загружен"
            });
        }

        [HttpPut]
        [Route("[action]")]
        [Authorize(Roles = "Owner, Admin")]
        public IActionResult UpdateDocument([FromBody] DocumentUpdateRequest? updateDocumentRequest)
        {
            if (updateDocumentRequest == null || updateDocumentRequest.NewName == null || updateDocumentRequest.Path == null || updateDocumentRequest.Type == null)
            {
                return BadRequest(new { errorText = "Запрос не полный" });
            }

            try
            {
                var path = Path.Combine(webRootPath ?? string.Empty, Path.Combine(updateDocumentRequest.Path.Split(Path.DirectorySeparatorChar)[..^1]));
                var sourcePath = Path.Combine(webRootPath ?? string.Empty, updateDocumentRequest.Path);
                var destPath = Path.Combine(path, updateDocumentRequest.NewName);

                if (updateDocumentRequest.Type.Equals(nameof(DocumentType.Folder), StringComparison.OrdinalIgnoreCase))
                {
                    Directory.Move(sourcePath, destPath);
                    Task.Run(() => _driveService.Rename(sourcePath, updateDocumentRequest.NewName, isFolder: true));
                }
                else if (updateDocumentRequest.Type.Equals(nameof(DocumentType.File), StringComparison.OrdinalIgnoreCase))
                {
                    System.IO.File.Move(sourcePath, destPath);
                    Task.Run(() => _driveService.Rename(sourcePath, updateDocumentRequest.NewName));
                }
                else
                {
                    return BadRequest(new { errorText = "Не указан тип документа" });
                }

                _memoryCache.Remove(CacheKeys.DocumentsKey);
                _memoryCache.Remove(CacheKeys.DocumentNodesKey);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = $"Ошибка при переименовании" });
            }

            return Ok(new { okText = "Имя успешно обновлено" });
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "Owner, Admin")]
        public async Task<IActionResult> Move([FromBody] MoveFileModel? moveFileModel)
        {
            if (moveFileModel == null || moveFileModel.OldPath == null || moveFileModel.NewPath == null)
            {
                return BadRequest(new { errorText = "Не указан путь для перемещения файла" });
            }

            var oldPath = Path.Combine(webRootPath!, Path.Combine(moveFileModel.OldPath.Split(Path.DirectorySeparatorChar)));
            var newPath = Path.Combine(webRootPath!, documentsDirectoryName!, Path.Combine(moveFileModel.NewPath.Split(Path.DirectorySeparatorChar)));

            if (oldPath == newPath)
            {
                return BadRequest(new { errorText = "Пути совпадают" });
            }

            if (System.IO.File.Exists(newPath))
            {
                return BadRequest(new { errorText = $"Файл с именем \"{newPath.Split(Path.DirectorySeparatorChar).Last()}\" уже существует" });
            }

            try
            {
                var overwrite = System.IO.File.Exists(newPath);
                Directory.Move(oldPath, newPath);

                _ = Task.Run(() =>
                {
                    _driveService.Delete(oldPath);

                    if (overwrite)
                    {
                        _driveService.Delete(newPath);
                    }

                    _driveService.CreateFile(newPath);
                });

                _memoryCache.Remove(CacheKeys.DocumentsKey);
                _memoryCache.Remove(CacheKeys.DocumentNodesKey);

                return Ok(new { okText = $"Файл \"{Path.GetFileName(oldPath)}\" успешно перемещен" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = $"Ошибка при перемещении файла: {ex.Message}" });
            }
        }

        private static string? ByteLengthToSizeString(long? length)
        {
            return length switch
            {
                >= 1000 and < 999999 => $"{length / 1000} Кб",
                >= 1000000 and < 999999999 => $"{length / 1000000} Mб",
                _ => $"{length} Байт",
            };
        }
    }
}
