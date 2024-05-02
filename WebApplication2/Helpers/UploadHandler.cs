namespace WebApplication1.Helpers
{
    public static class UploadHandler
    {
        public class UploadResult
        {
            public string ErrorMessage { get; set; }
            public string FileName { get; set; }
        }

        public static UploadResult Upload(IFormFile file, string folder)
        {
            //extension
            List<string> validExtentions = new List<string>() { ".jpg", ".png" };
            string extention = Path.GetExtension(file.FileName);
            if (!validExtentions.Contains(extention.ToLower()))
            {
                return new UploadResult {
                    ErrorMessage = $"Extention is not valid ({string.Join(',', validExtentions)})",
                    FileName= null
                };
            }

            //file size
            long size = file.Length;
            if(size > (10 * 1024 * 1024))
            {
                return new UploadResult
                {
                    ErrorMessage = "Maximum size can be 10mb",
                    FileName = null
                };
            }
            try
            {
                string fileName = Guid.NewGuid().ToString() + extention;
                string folderName = Path.Combine("Resources", folder);
                string path = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                using (var stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return new UploadResult
                {
                    ErrorMessage = null,
                    FileName = Path.Combine(folderName, fileName)
                };
            }
            catch (Exception ex)
            {
                return new UploadResult
                {
                    ErrorMessage = ex.Message,
                    FileName = null
                };
            }
        }
    }
}
