using System.Text;

namespace ServerStatusBot.Extensions;

public static class ReaderExtensions
{
    public static byte ReadByte(this byte[] array, ref int index)
    {
        var value = array[index];
        index++;
        return value;
    }
    
    public static void SkipValues(this byte[] array, ref int index, int count)
    {
        index += count;
    }
    
    public static void SkipString(this byte[] array, ref int index)
    {
        for (var i = index; i < array.Length; i++)
        {
            if (array[i] != 0x00)
                continue;

            index = i + 1;
            return;
        }
    }
    
    public static string ReadString(this byte[] array, ref int index)
    {
        for (var i = index ; i < array.Length; i++)
        {
            if (array[i] != 0x00) 
                continue;
            
            var result = Encoding.ASCII.GetString(array, index, i - index);
            index = i + 1;
            return result;
        }

        return string.Empty;
    }
}