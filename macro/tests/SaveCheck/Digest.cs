using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FD.Macro;

namespace SaveCheck;

/// <summary>
/// Per-day state digest, computed independently of the save code: SHA-256 over
///  - the System.Text.Json form of the world (<see cref="Json.Options"/>: every public field, JsObj/JsNumObj in JS key
///    order, so a key-order difference is a difference),
///  - the RNG state (bits) and call counter,
///  - both path caches (keys and lists, in enumeration order; null entries included) and ShoreW.
/// Printed as 32 hex digits (first 16 bytes).
/// </summary>
internal static class Digest
{
    public static string Of(Sim s)
    {
        using var h = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        using (var hs = new HashStream(h)) JsonSerializer.Serialize(hs, s.W, Json.Options);
        Span<byte> b = stackalloc byte[8];
        BinaryPrimitives.WriteInt64LittleEndian(b, BitConverter.DoubleToInt64Bits(s.Rng.State())); h.AppendData(b);
        BinaryPrimitives.WriteInt64LittleEndian(b, s.Rng.Calls); h.AppendData(b);
        Cache(h, 'P', s.PathCacheView);
        Cache(h, 'N', s.NavCache);
        if (s.ShoreW == null) h.AppendData("S-"u8);
        else { h.AppendData("S+"u8); h.AppendData(s.ShoreW); }
        return Convert.ToHexString(h.GetHashAndReset(), 0, 16).ToLowerInvariant();
    }

    private static void Cache(IncrementalHash h, char tag, IReadOnlyDictionary<string, List<int>> c)
    {
        var ms = new MemoryStream(1 << 16);
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
        {
            bw.Write(tag);
            bw.Write(c.Count);
            foreach (var kv in c)
            {
                bw.Write(kv.Key);                         // length-prefixed
                if (kv.Value == null) { bw.Write(-1); continue; }
                bw.Write(kv.Value.Count);
                foreach (int t in kv.Value) bw.Write(t);
            }
        }
        h.AppendData(ms.GetBuffer(), 0, (int)ms.Length);
    }

    /// <summary>Write-only stream feeding an IncrementalHash.</summary>
    private sealed class HashStream : Stream
    {
        private readonly IncrementalHash _h;
        public HashStream(IncrementalHash h) { _h = h; }
        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => _h.AppendData(buffer, offset, count);
        public override void Write(ReadOnlySpan<byte> buffer) => _h.AppendData(buffer);
    }
}
