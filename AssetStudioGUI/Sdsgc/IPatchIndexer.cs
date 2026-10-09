using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AssetStudioGUI.Sdsgc;

/// <summary>Interface to search for assets</summary>
public interface IPatchIndexer
{
    /// <summary>Number of indexed Unity bundles</summary>
    int BundleCount { get; }
    
    /// <summary>Number of indexed assets</summary>
    int AssetCount { get; }

    /// <summary>Searches for assets that match the input</summary>
    /// <param name="query">Search string</param>
    /// <param name="flags">Search options</param>
    /// <returns>Matching assets, grouped by <see cref="IndexedAsset.BundleName"/></returns>
    List<IGrouping<string, IndexedAsset>> Search(string query, SearchFlags flags);

    /// <summary>Searches for assets that match the input</summary>
    /// <param name="query">Search regex</param>
    /// <param name="flags">Search options</param>
    /// <returns>Matching assets, grouped by <see cref="IndexedAsset.BundleName"/></returns>
    List<IGrouping<string, IndexedAsset>> Search(Regex query, SearchFlags flags);
    
    /// <summary>A single asset file</summary>
    /// <param name="Name">Asset name</param>
    /// <param name="FullName">Asset full name (container)</param>
    /// <param name="BundleName">In which bundle the asset is in (<see cref="BundleData.Name"/>)</param>
    /// <param name="BundleChecksum">Checksum of the bundle the asset is in (<see cref="BundleData.Checksum"/>)</param>
    /// <param name="PackName">In which pack the bundle is in (<see cref="BundlePackData.Name"/>)</param>
    public record IndexedAsset(string Name, string FullName, string BundleName, string BundleChecksum, string PackName);

    /// <summary>Search options</summary>
    [Flags] public enum SearchFlags
    {
        None,
        /// <summary>Include <see cref="IndexedAsset.Name"/> in the search</summary>
        Name = 1 << 0,
        /// <summary>Include <see cref="IndexedAsset.FullName"/> in the search</summary>
        FullName = 1 << 1,
        /// <summary>Include <see cref="IndexedAsset.BundleName"/> in the search</summary>
        BundleName = 1 << 2
    }
}