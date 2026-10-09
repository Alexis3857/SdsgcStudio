#nullable enable
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AssetStudio;

namespace AssetStudioGUI.Sdsgc;

public class SdsgcClient(string patch, string version, string key)
{
    private static readonly HttpClient s_httpClient = new();

    // nanagb = international version of the game
    // aos = android (other values are winos and ios)
    // m = the path that contains images and 3d models
    private readonly string _bundleUrl = $"https://nanagb.gcdn.netmarble.com/nanagb/bundle/aos/astc/{patch}/{version}/m/";

    public async Task GetPatchIndexAsync()
    {
        // Special Unity bundle name that acts as a manifest of all other Unity bundles
        await using var stream = await DownloadBundleAsync("bmdata");
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        
        // Decrypt the bmdata and export its content
        ExportBmdata(Decrypt(ms.ToArray()), out var bundleDatas, out var bundlePackDatas);

        if (bundleDatas == null || bundlePackDatas == null)
            throw new Exception("Failed to export bmdata TextAsset");
    }

    /// <summary>Downloads a bundle</summary>
    /// <param name="bundleName"><see cref="BundlePackData.Name"/></param>
    /// <returns>A zip file of several Unity bundles</returns>
    /// <exception cref="HttpRequestException">The response status code was not <see cref="HttpStatusCode.OK"/></exception>
    public async Task<Stream> DownloadBundleAsync(string bundleName)
    {
        var uri = new Uri($"{_bundleUrl}{bundleName}");
        Logger.Verbose($"Downloading {uri}");
        
        var response = await s_httpClient.GetAsync(uri).ConfigureAwait(false);
        
        if (response.StatusCode == HttpStatusCode.OK)
            return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        
        response.Dispose();
        throw new HttpRequestException($"Failed to fetch {uri} ({response.StatusCode}). Are the patch and version values correct?", null, response.StatusCode);
    }
    
    // Decrypts encrypted Unity bundles
    private byte[] Decrypt(byte[] data)
    {
        try
        {
            var salt = data[..32];
            
            // The game uses new Rfc2898DeriveBytes(password, salt) which is equivalent to the line below
            var derived = Rfc2898DeriveBytes.Pbkdf2(key, salt, 1000, HashAlgorithmName.SHA1, 40);
            byte[] iv = [..derived[..8], ..salt[8..16]];

            using var aes = Aes.Create();
            aes.Key = derived[8..];
            return aes.DecryptCbc(data[32..], iv);
        }
        catch
        {
            throw new CryptographicException($"Failed to decrypt data, the key {key} is probably incorrect.");
        }
    }

    /// <summary>Tries to export the <see cref="BundleData"/> and the <see cref="BundlePackData"/> from a bmdata</summary>
    /// <param name="bmdata">Decrypted bmdata</param>
    /// <param name="bundleData">Parsed <see cref="BundleData"/>, if found</param>
    /// <param name="bundlePackData">Parsed <see cref="BundlePackData"/>, if found</param>
    private static void ExportBmdata(byte[] bmdata, out BundleData[]? bundleData, out BundlePackData[]? bundlePackData)
    {
        // Load the file without saving to disk
        var assetsManager = new AssetsManager();
        using var reader = new FileReader("bmdata", new MemoryStream(bmdata));
        var bundle = new BundleFile(reader, assetsManager.Options.BundleOptions);
        
        bundleData = null;
        bundlePackData = null;
        
        // Loop to find two TextAsset named "BundleData" and "BundlePackData"
        foreach (var file in bundle.fileList)
        {
            if (file.stream == null)
                continue;
            
            file.stream.Position = 0;
            if (new FileReader(file.fileName, file.stream) is not { FileType: FileType.AssetsFile } subReader)
                continue;

            var assetsFile = new SerializedFile(subReader, assetsManager);
            foreach (var objectInfo in assetsFile.m_Objects)
            {
                if (new ObjectReader(assetsFile.reader, assetsFile, objectInfo) is not { type: ClassIDType.TextAsset } objectReader)
                    continue;
                
                var textAsset = new TextAsset(objectReader);
                
                // When a TextAsset is found, deserialize it
                switch (textAsset.m_Name)
                {
                    case "BundleData":
                        Logger.Verbose("Found BundleData");
                        bundleData = BundleData.FromBmdata(textAsset.m_Script);
                        break;
                    case "BundlePackData":
                        Logger.Verbose("Found BundlePackData");
                        bundlePackData = BundlePackData.FromBmdata(textAsset.m_Script);
                        break;
                }
            }
        }
    }
}