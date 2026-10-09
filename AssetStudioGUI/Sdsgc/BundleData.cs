using System.Collections.Immutable;
using System.IO;

namespace AssetStudioGUI.Sdsgc;

/// <summary>Info about an unity bundle</summary>
public class BundleData
{
    public bool Compressed { get; }

    public bool Encrypt { get; }

    public bool UseStreamingLoad { get; }

    /// <summary>0 = Scene bundle, 1 = Asset bundle</summary>
    public byte Type { get; }

    public int Priority { get; }

    public int Version { get; }

    public uint Crc { get; }

    public ushort Crc16 { get; }

    public long Size { get; }

    /// <summary>Hidden name of the file</summary>
    public string Name { get; }

    public string Variant { get; }

    /// <summary>Name of the file when stored</summary>
    public string Checksum { get; }

    public ImmutableArray<string> Assets { get; }

    public ImmutableArray<string> Dependencies { get; }

    public ImmutableArray<string> AutoDependencies { get; }
    
    private BundleData(BinaryReader reader)
    {
        Compressed = reader.ReadBoolean();
        Encrypt = reader.ReadBoolean();
        UseStreamingLoad = reader.ReadBoolean();
        Type = reader.ReadByte();
        Crc = reader.ReadUInt32();
        Crc16 = reader.ReadUInt16();
        Priority = reader.ReadInt32();
        Version = reader.ReadInt32();
        Size = reader.ReadInt64();
        Name = reader.ReadString();
        Variant = reader.ReadString();
        Checksum = reader.ReadString();

        var arrayLength = reader.ReadInt32();
        var array = ImmutableArray.CreateBuilder<string>(arrayLength);
        for (var i = 0; i < arrayLength; i++)
            array.Add(reader.ReadString());
        Assets = array.MoveToImmutable();
        
        arrayLength = reader.ReadInt32();
        array = ImmutableArray.CreateBuilder<string>(arrayLength);
        for (var i = 0; i < arrayLength; i++)
            array.Add(reader.ReadString());
        Dependencies = array.MoveToImmutable();
        
        arrayLength = reader.ReadInt32();
        array = ImmutableArray.CreateBuilder<string>(arrayLength);
        for (var i = 0; i < arrayLength; i++)
            array.Add(reader.ReadString());
        AutoDependencies = array.MoveToImmutable();
    }
    
    /// <summary>Deserialize the bundles from the raw bmdata bytes</summary>
    public static BundleData[] FromBmdata(byte[] bundleData)
    {
        using var memoryStream = new MemoryStream(bundleData);
        using var reader = new BinaryReader(memoryStream);
        var bundles = new BundleData[reader.ReadInt32()];
        for (var i = 0; i < bundles.Length; i++)
            bundles[i] = new BundleData(reader);
        return bundles;
    }
}