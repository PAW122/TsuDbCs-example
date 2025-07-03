using System;
using System.Runtime.InteropServices;

public static class TsunamiClient
{
    private const string DLL = "tsuclient.dll";

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int Save(string key, string table, byte[] data, int length);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int Read(string key, string table, out IntPtr outBuf, out int outLen);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void FreeBuf(IntPtr ptr);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int Free(string key, string table);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SaveEncrypted(string key, string table, string encKey, byte[] data, int length);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int ReadEncrypted(string key, string table, string encKey, out IntPtr outBuf, out int outLen);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetKeysByRegex(string regex, int max, out IntPtr resultPtr, out int count);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void InitNetworkManager(int port, IntPtr peers, int count);


    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void FreeKeysArray(IntPtr array, int count);

    public static byte[] ReadBytes(string key, string table)
    {
        if (Read(key, table, out var ptr, out var len) != 0)
            return null;

        var data = new byte[len];
        Marshal.Copy(ptr, data, 0, len);
        FreeBuf(ptr);
        return data;
    }


    public static byte[] ReadEncryptedBytes(string key, string table, string encKey)
    {
        if (ReadEncrypted(key, table, encKey, out var ptr, out var len) != 0)
            return null;
        var data = new byte[len];
        Marshal.Copy(ptr, data, 0, len);
        FreeBuf(ptr);
        return data;
    }

    public static string[] GetKeys(string regex, int max)
    {
        if (GetKeysByRegex(regex, max, out var arrayPtr, out var count) != 0)
            return null;

        var result = new string[count];
        for (int i = 0; i < count; i++)
        {
            IntPtr strPtr = Marshal.ReadIntPtr(arrayPtr, i * IntPtr.Size);
            result[i] = Marshal.PtrToStringAnsi(strPtr);
        }

        FreeKeysArray(arrayPtr, count);
        return result;
    }

    public static void InitNetworkManager(int port, string[] peers)
    {
        if (peers == null || peers.Length == 0)
        {
            InitNetworkManager(port, IntPtr.Zero, 0);
            return;
        }

        IntPtr[] ptrs = new IntPtr[peers.Length];
        for (int i = 0; i < peers.Length; i++)
        {
            ptrs[i] = Marshal.StringToHGlobalAnsi(peers[i]);
        }

        IntPtr arrayPtr = Marshal.AllocHGlobal(IntPtr.Size * ptrs.Length);
        for (int i = 0; i < ptrs.Length; i++)
        {
            Marshal.WriteIntPtr(arrayPtr, i * IntPtr.Size, ptrs[i]);
        }

        InitNetworkManager(port, arrayPtr, peers.Length);

        for (int i = 0; i < ptrs.Length; i++)
            Marshal.FreeHGlobal(ptrs[i]);
        Marshal.FreeHGlobal(arrayPtr);
    }
}
