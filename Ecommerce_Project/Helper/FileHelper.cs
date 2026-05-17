namespace Ecommerce_Project.Helpers
{
    public class FileHelper
    {
        public static async Task<string> UploadFile(IFormFile file, string folderPath)
        {
            if (file == null || file.Length == 0)
                return null;

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            var fullFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderPath);

            if (!Directory.Exists(fullFolderPath))
            {
                Directory.CreateDirectory(fullFolderPath);
            }

            var fullPath = Path.Combine(fullFolderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/" + folderPath + "/" + fileName;
        }
    }
}