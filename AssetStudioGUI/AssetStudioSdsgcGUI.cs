using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows.Forms;
using AssetStudio;
using AssetStudioGUI.Sdsgc;
using Font = System.Drawing.Font;

namespace AssetStudioGUI;

public partial class AssetStudioGUIForm
{
    // Number of bundles to download and process in parallel
    private const int ConcurrentDownloadCount = 6;
    
    private readonly ToolStripMenuItem _sdsgcMenu = new("Sdsgc");
    private readonly ToolStripTextBox _sdsgcPatchTextBox = DefaultSdsgcToolStripTextBox();
    private readonly ToolStripTextBox _sdsgcVersionTextBox = DefaultSdsgcToolStripTextBox();
    private readonly ToolStripTextBox _sdsgcKeyTextBox = DefaultSdsgcToolStripTextBox();
    private readonly ToolStripMenuItem _sdsgcSearchBundlesMenuItem = new("Search bundles")
    {
        Enabled = false
    };
    
    private void InitializeSdsgcMenu()
    {
        Logger.Verbose("Initializing the Sdsgc menu");
        
        // The 3 fields where the user can type in
        AddSdsgcTextInput(_sdsgcPatchTextBox, "Patch");
        AddSdsgcTextInput(_sdsgcVersionTextBox, "Version");
        AddSdsgcTextInput(_sdsgcKeyTextBox, "Key");
        
        // Final search button
        _sdsgcMenu.DropDownItems.Add(new ToolStripSeparator());
        _sdsgcMenu.DropDownItems.Add(_sdsgcSearchBundlesMenuItem);
        
        // We need to force the focus on the search button otherwise it won't glow blue
        _sdsgcSearchBundlesMenuItem.MouseEnter += (_, _) => _sdsgcMenu.DropDown.Focus();
        _sdsgcSearchBundlesMenuItem.Click += SdsgcSearchBundles_Click;

        // Place menu before the "About" menu
        menuStrip1.Items.Insert(menuStrip1.Items.IndexOf(aboutToolStripMenuItem), _sdsgcMenu);
        return;

        // Add a ToolStripTextBox to _sdsgcMenu with a label above it
        void AddSdsgcTextInput(ToolStripTextBox textBox, string labelText)
        {
            _sdsgcMenu.DropDownItems.Add(new ToolStripMenuItem
            {
                Enabled = false,
                ShowShortcutKeys = false,
                Text = labelText
            });
            textBox.TextChanged += RefreshSdsgcSearchButtonState;
            _sdsgcMenu.DropDownItems.Add(textBox);
        }
    }

    // Event called when the user clicks on _sdsgcSearchBundlesMenuItem
    private async void SdsgcSearchBundles_Click(object sender, EventArgs e)
    {
        // Download the bundles manifest and show the form to let the user pick
        var client = new SdsgcClient(_sdsgcPatchTextBox.Text, _sdsgcVersionTextBox.Text, _sdsgcKeyTextBox.Text);
        var indexer = await client.GetPatchIndexAsync();
        using var searchForm = new BundleSearcherForm(indexer);
        if (await searchForm.ShowDialogAsync(this) != DialogResult.OK || searchForm.SelectedBundles is not { Count: > 0 } packs)
            return;

        // Download the packs and unzip the needed bundles only
        // Save their content in a Stream so no file is created in the process
        Logger.Verbose($"Downloading {packs.Count} packs");
        var bundles = new ConcurrentBag<(string Name, Stream Stream)>();
        await Parallel.ForEachAsync(packs, new ParallelOptions { MaxDegreeOfParallelism = ConcurrentDownloadCount }, async (pack, ct) =>
        {
            await using var packStream = await client.DownloadBundleAsync(pack.Key.Name);
            await using var zip = new ZipArchive(packStream);
            foreach (var bundle in pack)
            {
                if (zip.GetEntry(bundle.Checksum) is not { } entry)
                {
                    Logger.Warning($"Bundle {bundle.Checksum} ({bundle.Name}) wasn't found in pack {pack.Key.Name}");
                    continue;
                }

                await using var unzipped = await entry.OpenAsync(ct);
                var bundleContent = new byte[entry.Length];
                await unzipped.ReadExactlyAsync(bundleContent, ct);
                bundles.Add((entry.Name, new MemoryStream(SdsgcClient.FixFakeHeader(bundleContent))));
            }
        });
        
        ResetForm();
        await Task.Run(() => Studio.assetsManager.LoadFromStream([..bundles]));
        BuildAssetStructures();
    }
    
    // Only enable the search button when all the text inputs of the menu are filled
    private void RefreshSdsgcSearchButtonState(object sender, EventArgs e)
    {
        _sdsgcSearchBundlesMenuItem.Enabled =
            !string.IsNullOrWhiteSpace(_sdsgcPatchTextBox.Text) &&
            !string.IsNullOrWhiteSpace(_sdsgcVersionTextBox.Text) &&
            !string.IsNullOrWhiteSpace(_sdsgcKeyTextBox.Text);
    }
    
    private static ToolStripTextBox DefaultSdsgcToolStripTextBox() => new()
    {
        BorderStyle = BorderStyle.FixedSingle,
        Font = new Font("Segoe UI", 9F),
        Size = new Size(200, 23)
    };
}