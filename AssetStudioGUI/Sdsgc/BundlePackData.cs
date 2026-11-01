using System.Collections.Immutable;
using System.IO;

namespace AssetStudioGUI.Sdsgc;

/// <summary>Zip file containing several <see cref="BundleData"/> bundles</summary>
public class BundlePackData
{
    /// <summary>Pack name for download</summary>
    public string Name { get; }

    public long OriginSize { get; }

    public long Size { get; }
    
    /// <summary>Bundles included in the pack (<see cref="BundleData.Checksum"/>)</summary>
    public ImmutableArray<string> IncludeBundles { get; }
    public Section Section { get; }
    
    private BundlePackData(BinaryReader reader, Section section)
    {
        Name = reader.ReadString();
        OriginSize = reader.ReadInt64();
        Size = reader.ReadInt64();
        
        var arrayLength = reader.ReadInt32();
        var array = ImmutableArray.CreateBuilder<string>(arrayLength);
        for (var i = 0; i < arrayLength; i++)
            array.Add(reader.ReadString());
        IncludeBundles = array.MoveToImmutable();

        Section = section;
    }
    
    /// <summary>Deserialize the packs from the raw bmdata bytes</summary>
    public static BundlePackData[] FromBmdata(byte[] bundlePackData, Section section)
    {
        using var memoryStream = new MemoryStream(bundlePackData);
        using var reader = new BinaryReader(memoryStream);
        var packs = new BundlePackData[reader.ReadInt32()];
        for (var i = 0; i < packs.Length; i++)
            packs[i] = new BundlePackData(reader, section);
        return packs;
    }
}