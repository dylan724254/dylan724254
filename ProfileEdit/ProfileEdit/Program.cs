namespace ProfileEdit
{
    internal class Program
    {
        const string pathPrefix = @"";
        const string profileFileName = "README.md";
        static void Main(string[] args)
        {
            var text = args.Length > 0
                ? string.Join(",", args)
                : $"{DateTime.Now:yyyyMMddHHmmss}";

            var filePath = Path.Combine(pathPrefix, profileFileName);

            File.Delete(filePath);
            using var writer = File.CreateText(filePath);
            writer.WriteLine(text);
        }
    }
}
