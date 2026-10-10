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
    // All indexed bundles
    private readonly ImmutableArray<IndexedBundle> _indexedBundles;

    public int BundleCount => _indexedBundles.Length;

    public List<IndexedBundle> Search(string query, SearchFlags flags)
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

    private List<IndexedBundle> Search(Func<string, bool> isMatch, SearchFlags flags) =>
    [
        .._indexedBundles
            .Where(b => 
                flags.HasFlag(SearchFlags.BundleName) && isMatch(b.Bundle.Name) || 
                b.Assets.Any(a => 
                    (flags.HasFlag(SearchFlags.Name) && isMatch(a.Name)) || 
                    (flags.HasFlag(SearchFlags.FullName) && isMatch(a.FullName))))
    ];

    private TMIndexer(IEnumerable<IndexedBundle> indexedBundles)
    {
        _indexedBundles = [..indexedBundles];
        Logger.Verbose($"Indexed {BundleCount} bundles");
    }

    public static TMIndexer Create(BundleData[] bundleData, BundlePackData[] packData)
    {
        Logger.Verbose($"Indexing {bundleData.Length} bundles");
        
        Dictionary<string, BundlePackData> packDict = [];
        foreach (var pack in packData)
            foreach (var bundle in pack.IncludeBundles)
                packDict.Add(bundle, pack);

        // Find all bundles that contain at least one texture or model and create the TMIndexer instance
        List<IndexedBundle> bundles = [];
        foreach (var bundle in bundleData)
        {
            var indexedAssets = bundle.Assets.Where(a => 
                    a.EndsWith(".png", StringComparison.Ordinal) ||
                    a.EndsWith(".asset", StringComparison.Ordinal) ||
                    a.EndsWith(".prefab", StringComparison.Ordinal))
                .Select(a => new IndexedAsset(Path.GetFileName(a), a))
                .ToImmutableArray();

            if (!indexedAssets.IsEmpty)
                bundles.Add(new IndexedBundle(bundle, packDict[bundle.Checksum], indexedAssets));
        }
        return new TMIndexer(bundles);
    }
}