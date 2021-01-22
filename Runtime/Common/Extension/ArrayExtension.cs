namespace RFramework.Common.Extension
{
    public static class ArrayExtension
    {
        public static void Fill<T>(this T[] arr, T value)
        {
            for (var i = 0; i < arr.Length; i++)
                arr[i] = value;
        }
    }
}