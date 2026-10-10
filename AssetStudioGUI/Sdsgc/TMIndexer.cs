using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AssetStudio;
using static AssetStudioGUI.Sdsgc.IPatchIndexer;

namespace AssetStudioGUI.Sdsgc;

/// <summary>Textures and models indexer (I suck at naming things)</summary>
public class TMIndexer : IPatchIndexer
{
    // All indexed assets
    private readonly ImmutableArray<IndexedAsset> _indexedAssets;

    public int BundleCount { get; }

    public int AssetCount => _indexedAssets.Length;

    public List<IGrouping<string, IndexedAsset>> Search(string query, SearchFlags flags)
    {
        if (query == string.Empty)
            return [];

        if (flags.HasFlag(SearchFlags.Regex))
        {
            var regex = new Regex(query);
            return Search(regex.IsMatch, flags);
        }
        
        // Name is a suffix of FullName, so a substring match on Name implies one on FullName
        if (flags.HasFlag(SearchFlags.FullName))
            flags &= ~SearchFlags.Name;

        return Search(s => s.Contains(query, StringComparison.OrdinalIgnoreCase), flags);
    }

    private List<IGrouping<string, IndexedAsset>> Search(Func<string, bool> isMatch, SearchFlags flags) =>
    [
        .._indexedAssets
            .Where(a => 
                (flags.HasFlag(SearchFlags.Name) && isMatch(a.Name)) || 
                (flags.HasFlag(SearchFlags.FullName) && isMatch(a.FullName)) || 
                (flags.HasFlag(SearchFlags.BundleName) && isMatch(a.BundleName)))
            .GroupBy(a => a.BundleName)
    ];

    private TMIndexer(IEnumerable<IndexedAsset> indexedAssets)
    {
        _indexedAssets = [.. indexedAssets];
        BundleCount = _indexedAssets.Select(a => a.BundleName).Distinct().Count();
        
        Logger.Verbose($"Indexed {BundleCount} bundles and {AssetCount} assets");
    }

    public static TMIndexer Create(BundleData[] bundleData, BundlePackData[] packData)
    {
        Logger.Verbose($"Indexing {bundleData.Length} bundles");
        
        Dictionary<string, string> packDict = [];
        foreach (var pack in packData)
        foreach (var bundle in pack.IncludeBundles)
            packDict.Add(bundle, pack.Name);

        // Find all assets that are either a texture or a model and create a TMIndexer instance from them
        return new TMIndexer(bundleData
            .SelectMany(b => b.Assets.Where(a =>
                    a.EndsWith(".png", StringComparison.Ordinal) ||
                    a.EndsWith(".obj", StringComparison.Ordinal) ||
                    a.EndsWith(".prefab", StringComparison.Ordinal))
                .Select(a => new IndexedAsset(Path.GetFileName(a), a, b.Name, b.Checksum, packDict[b.Checksum]))));
    }
}