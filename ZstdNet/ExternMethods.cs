﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using size_t = System.UIntPtr;

namespace ZstdNet
{
	internal static class ExternMethods
	{
		static ExternMethods()
		{
			if(Environment.OSVersion.Platform == PlatformID.Win32NT)
				SetWinDllDirectory();
		}

		private static void SetWinDllDirectory()
		{
			string path;

			var location = Assembly.GetExecutingAssembly().Location;
			if(string.IsNullOrEmpty(location) || (path = Path.GetDirectoryName(location)) == null)
			{
				Trace.TraceWarning($"{nameof(ZstdNet)}: Failed to get executing assembly location");
				return;
			}

			// Nuget package
			if(Path.GetFileName(path).StartsWith("net", StringComparison.Ordinal) && Path.GetFileName(Path.GetDirectoryName(path)) == "lib" && File.Exists(Path.Combine(path, "../../zstdnet.nuspec")))
				path = Path.Combine(path, "../../build");

			var platform = Environment.Is64BitProcess ? "x64" : "x86";
			if(!SetDllDirectory(Path.Combine(path, platform)))
				Trace.TraceWarning($"{nameof(ZstdNet)}: Failed to set DLL directory to '{path}'");
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool SetDllDirectory(string path);

		private const string DllName = "libzstd";

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZDICT_trainFromBuffer(byte[] dictBuffer, size_t dictBufferCapacity, byte[] samplesBuffer, size_t[] samplesSizes, uint nbSamples);
        internal static object ZSTD_flushStream(IntPtr zSTD_CStream, ref object outBuffer) => throw new NotImplementedException();
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern uint ZDICT_isError(size_t code);
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ZDICT_getErrorName(size_t code);

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ZSTD_createCCtx();
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZSTD_freeCCtx(IntPtr cctx);
		
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ZSTD_createDCtx();
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZSTD_freeDCtx(IntPtr cctx);

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZSTD_compressCCtx(IntPtr ctx, IntPtr dst, size_t dstCapacity, IntPtr src, size_t srcSize, int compressionLevel);
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZSTD_decompressDCtx(IntPtr ctx, IntPtr dst, size_t dstCapacity, IntPtr src, size_t srcSize);

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ZSTD_createCDict(byte[] dict, size_t dictSize, int compressionLevel);
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZSTD_freeCDict(IntPtr cdict);
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZSTD_compress_usingCDict(IntPtr cctx, IntPtr dst, size_t dstCapacity, IntPtr src, size_t srcSize, IntPtr cdict);

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ZSTD_createDDict(byte[] dict, size_t dictSize);
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZSTD_freeDDict(IntPtr ddict);
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZSTD_decompress_usingDDict(IntPtr dctx, IntPtr dst, size_t dstCapacity, IntPtr src, size_t srcSize, IntPtr ddict);

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ZSTD_getDecompressedSize(IntPtr src, size_t srcSize);

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int ZSTD_maxCLevel();
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZSTD_compressBound(size_t srcSize);
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern uint ZSTD_isError(size_t code);
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ZSTD_getErrorName(size_t code);



        #region Streaming APIs
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ZSTD_createCStream();
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZSTD_freeCStream(IntPtr zcs);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZSTD_initCStream(IntPtr zcs, int compressionLevel);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZSTD_compressStream(IntPtr zcs, ref ZSTD_Buffer output, ref ZSTD_Buffer input);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZSTD_flushStream(IntPtr zcs, ref ZSTD_Buffer output);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZSTD_endStream(IntPtr zcs, ref ZSTD_Buffer output);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZSTD_CStreamInSize();
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZSTD_CStreamOutSize();
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ZSTD_createDStream();
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZSTD_freeDStream(IntPtr zds);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZSTD_initDStream(IntPtr zds);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZSTD_decompressStream(IntPtr zds, ref ZSTD_Buffer output, ref ZSTD_Buffer input);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZSTD_DStreamInSize();
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZSTD_DStreamOutSize();


        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZSTD_initDStream_usingDDict(IntPtr zds, IntPtr dict);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZSTD_initCStream_usingCDict(IntPtr zds, IntPtr dict);


        [StructLayout(LayoutKind.Sequential)]
        internal struct ZSTD_Buffer
        {
            public ZSTD_Buffer(ArraySegmentPtr segmentPtr)
            {
                buffer = segmentPtr;
                size = (size_t)segmentPtr.Length;
                pos = default(size_t);
            }

            /// <summary>
            /// Start of the buffer
            /// </summary>
            public IntPtr buffer;

            /// <summary>
            /// Size of the buffer
            /// </summary>
            public size_t size;

            /// <summary>
            /// Position where reading/writing stopped. Will be updated. Necessarily 0 <= pos <= size
            /// </summary>
            public size_t pos;

            public bool IsFullyConsumed => UnconsumedSpace <= 0;
            public int UnconsumedSpace => IntSize - IntPos;
            public int IntSize => (int)size;
            public int IntPos => (int)pos;
        }
        #endregion
    }
}

