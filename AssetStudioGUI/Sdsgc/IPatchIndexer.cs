using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace AssetStudioGUI.Sdsgc;

/// <summary>Interface to search for assets</summary>
public interface IPatchIndexer
{
    /// <summary>Number of indexed Unity bundles</summary>
    int BundleCount { get; }

    /// <summary>Searches for bundles that match the input</summary>
    /// <param name="query">Search string</param>
    /// <param name="flags">Search options</param>
    /// <returns>Matching bundles</returns>
    List<IndexedBundle> Search(string query, SearchFlags flags);
    
    /// <summary>An indexed asset</summary>
    /// <param name="Name">Asset file name</param>
    /// <param name="FullName">Asset container</param>
    public record IndexedAsset(string Name, string FullName);
    
    /// <summary>An indexed bundle</summary>
    /// <param name="Bundle">BundleData</param>
    /// <param name="Pack">Pack in which the bundle is in</param>
    /// <param name="Assets">Indexed assets</param>
    public record IndexedBundle(BundleData Bundle, BundlePackData Pack, ImmutableArray<IndexedAsset> Assets);

    /// <summary>Search options</summary>
    [Flags] public enum SearchFlags
    {
        None,
        /// <summary>Include <see cref="IndexedAsset.Name"/> in the search</summary>
        Name = 1 << 0,
        /// <summary>Include <see cref="IndexedAsset.FullName"/> in the search</summary>
        FullName = 1 << 1,
        /// <summary>Include <see cref="IndexedBundle.Bundle"/>.Name in the search</summary>
        BundleName = 1 << 2,
        /// <summary>If the query string should be parsed as regex</summary>
        Regex = 1 << 3
    }
}