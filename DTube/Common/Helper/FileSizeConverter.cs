using System;

namespace DTube.Common.Helper
{
    public static class FileSizeConverter
    {
        public static string ToMiB(long size) => Math.Round(size / 1024f / 1024, 2).ToString() + " MiB";
    }
}
