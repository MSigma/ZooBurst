using System.IO;

namespace ZooBurst.View.Fonts
{
    public struct Page
    {
        public string FileName;
        public int Id;

        public Page(int id, string fileName)
        {
            FileName = fileName;
            Id = id;
        }

        public override string ToString()
        {
            return $"{Id} ({Path.GetFileName(FileName)})";
        }
    }
}
