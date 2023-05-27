namespace QuickShop.Models
{
    public class FileData
    {
        public string FileName {get; set;}
        public string FileContent {get; set;}

        public FileData(string fileName, string fileContent)
        {
            FileName = fileName;
            FileContent = fileContent;
        }
    }
}