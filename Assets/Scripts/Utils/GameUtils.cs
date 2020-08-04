namespace Utils
{
    public static class GameUtils
    {
        public static string GetNameByPath(string path)
        {
            int lastIndexOfSlash = path.LastIndexOf('/');
            return lastIndexOfSlash == -1 ? path : path.Substring(lastIndexOfSlash + 1);
        }
    }
}