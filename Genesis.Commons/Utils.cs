﻿using Genesis.Commons.Exceptions;
using K4os.Compression.LZ4;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public delegate void UpdateEventHandler(object? obj = null);
    public delegate void ProgressEventHandler(NetProgress progress);

    public static class Utils
    {
        public static byte[] Compress(byte[] data) =>
            LZ4Pickler.Pickle(data);

        public static byte[] Decompress(byte[] data) =>
            LZ4Pickler.Unpickle(data);

        public static byte[] HashFile(string path)
        {
            using var md5 = MD5.Create();
            using var file = File.OpenRead(path);
            return md5.ComputeHash(file);
        }

        public static byte[] Serialize<T>(T? obj, bool throwException = false)
        {
            try
            {
                return MessagePackSerializer.Typeless.Serialize(obj);
            }
            catch (Exception ex)
            {
                if (throwException)
                {
                    throw new SerializationException(ex);
                }
                else
                {
                    return new byte[0];
                }
            }
        }

        public static T? TypeDeserialize<T>(byte[]? data, bool throwException = false) where T : class
        {
            try
            {
                return Deserialize(data) as T;
            }
            catch (Exception ex)
            {
                if (throwException)
                {
                    throw new SerializationException(ex);
                }
                else
                {
                    return null;
                }
            }
        }

        public static object? Deserialize(byte[]? data, bool throwException = false)
        {
            try
            {
                return MessagePackSerializer.Typeless.Deserialize(data);
            }
            catch (Exception ex)
            {
                if (throwException)
                {
                    throw new SerializationException(ex);
                }
                else
                {
                    return null;
                }
            }
        }

        public static bool IsEmpty(this string? str) => 
            string.IsNullOrEmpty(str);

        public static int ToInt(this byte[] data) =>
            BitConverter.ToInt32(data);

        public static long ToLong(this byte[] data) =>
            BitConverter.ToInt64(data);

        public static byte[] ToBytes(this int num) =>
            BitConverter.GetBytes(num);

        public static byte[] ToBytes(this long num) =>
            BitConverter.GetBytes(num);

        public static Guid AcceptedToken => Guid.Parse("8d5ca768-bfa0-4d8b-bbed-89ba5d37b5aa");
        public static Guid DisconnectToken => Guid.Parse("6054c0e4-a59b-4f6c-8921-74cfe18009a8");
    }
}
