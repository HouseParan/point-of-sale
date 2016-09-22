// ReSharper disable once CheckNamespace
namespace PointOfSale.Repository
{
    /// <summary>
    /// Basic file IO operations.
    /// </summary>
    public interface IFileManager
    {        
        /// <summary>
        /// Read and return all text from the file.
        /// </summary>
        /// <param name="path">Path to file.</param>
        /// <returns></returns>
        string GetFileText(string path);
    }
}
