using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AssetStudioGUI.Sdsgc;

public class SdsgcClient(string patch, string version, string key)
{
    // Special Unity bundle name that acts as a manifest of all other Unity bundles
    private const string BmdataName = "bmdata";
    
    private static readonly HttpClient s_httpClient = new();

    // nanagb = international version of the game
    // aos = android (other values are winos and ios)
    // m = the path that contains images and 3d models
    private readonly string _bundleUrl = $"https://nanagb.gcdn.netmarble.com/nanagb/bundle/aos/astc/{patch}/{version}/m/";
    private readonly string _key = key;

    /// <summary>Downloads a bundle from <see cref="_bundleUrl"/></summary>
    /// <param name="bundleName"><see cref="BmdataName"/> or <see cref="BundlePackData.Name"/></param>
    /// <returns>
    /// Bundle content stream<br/>
    /// Either an encrypted Unity bundle if <see cref="BmdataName"/> was requested, or a zip file of several Unity bundles
    /// </returns>
    /// <exception cref="HttpRequestException">The response status code was not <see cref="HttpStatusCode.OK"/></exception>
    private async Task<Stream> DownloadBundleAsync(string bundleName)
    {
        var uri = new Uri($"{_bundleUrl}{bundleName}");
        var response = await s_httpClient.GetAsync(uri).ConfigureAwait(false);
        
        if (response.StatusCode == HttpStatusCode.OK)
            return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        
        response.Dispose();
        throw new HttpRequestException($"Failed to fetch {uri} ({response.StatusCode}). Are the patch and version values correct?", null, response.StatusCode);
    }
}