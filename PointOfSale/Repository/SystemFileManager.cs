namespace PointOfSale.Repository
{
    /// <summary>
    /// Concrete implementation of basic file IO operations.
    /// </summary>
    public class SystemFileManager : IFileManager
    {
        public string GetFileText(string path)
        {
            return System.IO.File.ReadAllText(path);
        }
    }
}
