namespace Utils
{
    public static class GameUtils
    {
        public static string GetNameByPath(string path)
        {
            var lastIndexOfSlash = path.LastIndexOf('/');
            return lastIndexOfSlash == -1 ? path : path.Substring(lastIndexOfSlash + 1);
        }
    }
}