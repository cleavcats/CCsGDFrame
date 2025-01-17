using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Cleavcats
{
    public static unsafe class DataFactory
    {
        public static class TypeStream
        {
            static readonly byte[] buffer_simple = new byte[64], buffer_string = new byte[2048];
            // 这批方法使用了 指针 来进行转换，解决了 BitConvert 所产生的 内存碎片问题。
            public static void AllWrite(Stream stream, params object[] args)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] is bool) BoolWrite(stream, (bool)args[i]);
                    else if (args[i] is short) ShortWrite(stream, (short)args[i]);
                    else if (args[i] is int) IntWrite(stream, (int)args[i]);
                    else if (args[i] is float) FloatWrite(stream, (float)args[i]);
                    else if (args[i] is double) DoubleWrite(stream, (double)args[i]);
                    else if (args[i] is string) StringWrite(stream, (string)args[i]);
                    else if (args[i] is byte[]) BytesWrite(stream, (byte[])args[i], ((byte[])args[i]).Length);
                    else throw new Exception("DataFactory AllWrite 传入了不支持的类型");
                }
            }
            public static void BoolWrite(Stream stream, bool value) { fixed (byte* p = &buffer_simple[0]) { *(bool*)p = value; } stream.Write(buffer_simple, 0, 1); }
            public static void ShortWrite(Stream stream, short value) { fixed (byte* p = &buffer_simple[0]) { *(short*)p = value; } stream.Write(buffer_simple, 0, 2); }
            public static void IntWrite(Stream stream, int value) { fixed (byte* p = &buffer_simple[0]) { *(int*)p = value; } stream.Write(buffer_simple, 0, 4); }
            public static void FloatWrite(Stream stream, float value) { fixed (byte* p = &buffer_simple[0]) { *(float*)p = value; } stream.Write(buffer_simple, 0, 4); }
            public static void DoubleWrite(Stream stream, double value) { fixed (byte* p = &buffer_simple[0]) { *(double*)p = value; } stream.Write(buffer_simple, 0, 8); }
            public static void StringWrite(Stream stream, string value)
            {
                if (value == null) { IntWrite(stream, 0); return; }
                int len = System.Text.Encoding.UTF8.GetBytes(value, 0, value.Length, buffer_string, 0);
                IntWrite(stream, len);
                stream.Write(buffer_string, 0, len);
            }
            public static void BytesWrite(Stream stream, byte[] data, int count) { IntWrite(stream, count); stream.Write(data, 0, count); }

            public static void AllRead<T>(Stream stream, out T arg0)
            {
                arg0 = default(T);
                if (arg0 is bool) arg0 = (T)(object)BoolRead(stream);
                else if (arg0 is short) arg0 = (T)(object)ShortRead(stream);
                else if (arg0 is int) arg0 = (T)(object)IntRead(stream);
                else if (arg0 is float) arg0 = (T)(object)FloatRead(stream);
                else if (arg0 is double) arg0 = (T)(object)DoubleRead(stream);
                else if (arg0 is string) arg0 = (T)(object)StringRead(stream);
                else if (arg0 is byte[]) arg0 = (T)(object)BytesRead(stream);
                else throw new Exception("DataFactory AllRead 传入了不支持的类型");
            }
            public static void AllRead<T, T1>(Stream stream, out T arg0, out T1 arg1) { AllRead(stream, out arg0); AllRead(stream, out arg1); }
            public static void AllRead<T, T1, T2>(Stream stream, out T arg0, out T1 arg1, out T2 arg2) { AllRead(stream, out arg0); AllRead(stream, out arg1); AllRead(stream, out arg2); }
            public static void AllRead<T, T1, T2, T3>(Stream stream, out T arg0, out T1 arg1, out T2 arg2, out T3 arg3)
            {
                AllRead(stream, out arg0, out arg1, out arg2);
                AllRead(stream, out arg3);
            }
            public static void AllRead<T, T1, T2, T3, T4>(Stream stream, out T arg0, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4)
            {
                AllRead(stream, out arg0, out arg1, out arg2);
                AllRead(stream, out arg3, out arg4);
            }
            public static void AllRead<T, T1, T2, T3, T4, T5>(Stream stream, out T arg0, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5)
            {
                AllRead(stream, out arg0, out arg1, out arg2);
                AllRead(stream, out arg3, out arg4, out arg5);
            }
            public static void AllRead<T, T1, T2, T3, T4, T5, T6>(Stream stream, out T arg0, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6)
            {
                AllRead(stream, out arg0, out arg1, out arg2);
                AllRead(stream, out arg3, out arg4, out arg5);
                AllRead(stream, out arg6);
            }
            public static bool BoolRead(Stream stream) { stream.Read(buffer_simple, 0, 1); return BitConverter.ToBoolean(buffer_simple, 0); }
            public static short ShortRead(Stream stream) { stream.Read(buffer_simple, 0, 2); return BitConverter.ToInt16(buffer_simple, 0); }
            public static int IntRead(Stream stream) { stream.Read(buffer_simple, 0, 4); return BitConverter.ToInt32(buffer_simple, 0); }
            public static float FloatRead(Stream stream) { stream.Read(buffer_simple, 0, 4); return BitConverter.ToSingle(buffer_simple, 0); }
            public static double DoubleRead(Stream stream) { stream.Read(buffer_simple, 0, 8); return BitConverter.ToDouble(buffer_simple, 0); }
            public static string StringRead(Stream stream)
            {
                int len = IntRead(stream);
                if (len == 0) return null;
                stream.Read(buffer_simple, 0, len);
                return System.Text.Encoding.UTF8.GetString(buffer_simple, 0, len);
            }
            public static byte[] BytesRead(Stream stream) { int count = IntRead(stream); byte[] returns = new byte[count]; stream.Read(returns, 0, count); return returns; }
            public static int BytesRead(Stream stream, byte[] to, int max_len = 0)
            {
                int count = IntRead(stream);
                if (max_len == 0) max_len = to.Length;
                if (max_len > count) max_len = count;
                return stream.Read(to, 0, max_len);
            }
        }
        public static class TypeBytes
        {
            static readonly byte[] buffer_simple = new byte[64], buffer_string = new byte[2048];
            static bool BufferCopy/*数组复制，返回值为是否成功*/(byte[] target, ref int offset, byte[] from, int len) { if (offset + len >= target.Length) return false; Array.Copy(from, 0, target, offset, len); offset += len; return true; }

            public static void BoolWrite(byte[] target, ref int offset, bool value) { fixed (byte* p = &buffer_simple[0]) { *(bool*)p = value; } BufferCopy(target, ref offset, buffer_simple, 1); }
            public static void ShortWrite(byte[] target, ref int offset, short value) { fixed (byte* p = &buffer_simple[0]) { *(short*)p = value; } BufferCopy(target, ref offset, buffer_simple, 2); }
            public static void IntWrite(byte[] target, ref int offset, int value) { fixed (byte* p = &buffer_simple[0]) { *(int*)p = value; } BufferCopy(target, ref offset, buffer_simple, 4); }
            public static void FloatWrite(byte[] target, ref int offset, float value) { fixed (byte* p = &buffer_simple[0]) { *(float*)p = value; } BufferCopy(target, ref offset, buffer_simple, 4); }
            public static void DoubleWrite(byte[] target, ref int offset, double value) { fixed (byte* p = &buffer_simple[0]) { *(double*)p = value; } BufferCopy(target, ref offset, buffer_simple, 8); }
            public static void StringWrite(byte[] target, ref int offset, string value)
            {
                if (value == null) { IntWrite(target, ref offset, 0); return; }
                int len = System.Text.Encoding.UTF8.GetBytes(value, 0, value.Length, buffer_string, 0);
                IntWrite(target, ref offset, len);
                BufferCopy(target, ref offset, buffer_string, len);
            }
            public static void BytesWrite(byte[] target, byte[] data, ref int offset, int count = -1)
            {
                if (data == null) { IntWrite(target, ref offset, 0); return; }
                if (count < 0) count = data.Length;
                IntWrite(target, ref offset, count);
                BufferCopy(target, ref offset, data, count);
            }

            public static bool BoolRead(byte[] from, ref int offset) { bool returns = BitConverter.ToBoolean(from, offset); offset += 1; return returns; }
            public static short ShortRead(byte[] from, ref int offset) { short returns = BitConverter.ToInt16(from, offset); offset += 2; return returns; }
            public static int IntRead(byte[] from, ref int offset) { int returns = BitConverter.ToInt32(from, offset); offset += 4; return returns; }
            public static float FloatRead(byte[] from, ref int offset) { float returns = BitConverter.ToSingle(from, offset); offset += 4; return returns; }
            public static double DoubleRead(byte[] from, ref int offset) { double returns = BitConverter.ToDouble(from, offset); offset += 8; return returns; }
            public static string StringRead(byte[] from, ref int offset)
            {
                int len = IntRead(from, ref offset);
                string returns = System.Text.Encoding.UTF8.GetString(from, offset, len);
                offset += len;
                return returns;
            }
            public static byte[] BytesRead(byte[] from, ref int offset)
            {
                int count = IntRead(from, ref offset);
                if (count == 0) return null;
                byte[] returns = new byte[count]; Array.Copy(from, offset, returns, 0, count); offset += count; return returns;
            }
            public static int BytesRead/*返回读取的字节数*/(byte[] from, byte[] to, ref int offset) { int count = IntRead(from, ref offset); Array.Copy(from, offset, to, 0, count); offset += count; return count; }
        }

        public static int StreamToBytes/*返回0则表示缓冲区长度不够，转换失败*/(Stream stream, byte[] buffer)
        {
            if (buffer.Length < stream.Length) return 0;
            stream.Position = 0;
            stream.Read(buffer, 0, (int)stream.Length);
            return (int)stream.Length;
        }
        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] returns = new byte[stream.Length];
            stream.Read(returns, 0, (int)stream.Length);
            return returns;
        }
        public static void BytesToStream(byte[] buffer, Stream stream, int offset, int count) { stream.SetLength(0); stream.Write(buffer, offset, count); }

        static bool IsUtf8(byte[] buff)
        {
            for (int i = 0; i < buff.Length; i++)
            {
                if ((buff[i] & 0xE0) == 0xC0)    // 110x xxxx 10xx xxxx
                {
                    if ((buff[i + 1] & 0x80) != 0x80)
                    {
                        return false;
                    }
                }
                else if ((buff[i] & 0xF0) == 0xE0)  // 1110 xxxx 10xx xxxx 10xx xxxx
                {
                    if ((buff[i + 1] & 0x80) != 0x80 || (buff[i + 2] & 0x80) != 0x80)
                    {
                        return false;
                    }
                }
                else if ((buff[i] & 0xF8) == 0xF0)  // 1111 0xxx 10xx xxxx 10xx xxxx 10xx xxxx
                {
                    if ((buff[i + 1] & 0x80) != 0x80 || (buff[i + 2] & 0x80) != 0x80 || (buff[i + 3] & 0x80) != 0x80)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public static string ReadAllTextEncoding(byte[] buff)
        {
            string strReslut = string.Empty;
            if (buff.Length > 3)
            {
                if (buff[0] == 239 && buff[1] == 187 && buff[2] == 191)
                {// utf-8
                    strReslut = System.Text.Encoding.UTF8.GetString(buff);
                }
                else if (buff[0] == 254 && buff[1] == 255)
                {// big endian unicode
                    strReslut = System.Text.Encoding.BigEndianUnicode.GetString(buff);
                }
                else if (buff[0] == 255 && buff[1] == 254)
                {// unicode
                    strReslut = System.Text.Encoding.Unicode.GetString(buff);
                }
                else if (IsUtf8(buff))
                {// utf-8
                    strReslut = System.Text.Encoding.UTF8.GetString(buff);
                }
                else
                {// ansi
                    strReslut = System.Text.Encoding.Default.GetString(buff);
                }
            }
            return strReslut;
        }
        public static string ReadAllTextEncoding(string url)
        {
            byte[] bytes;
            try { bytes = System.IO.File.ReadAllBytes(url); } catch { return null; }
            return ReadAllTextEncoding(bytes);
        }
    }
}