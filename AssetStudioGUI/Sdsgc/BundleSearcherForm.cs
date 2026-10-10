using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using AssetStudio;
using SearchFlags = AssetStudioGUI.Sdsgc.IPatchIndexer.SearchFlags;

namespace AssetStudioGUI.Sdsgc;

public partial class BundleSearcherForm : Form
{
    // Only search if the user typed more than 2 chars
    private const int MinQueryLen = 2;

    private readonly IPatchIndexer _indexer;

    public BundleSearcherForm(IPatchIndexer indexer)
    {
        InitializeComponent();
        _indexer = indexer;
    }

    // Search the matching assets and update _resultsTreeView
    private async void SearchButton_Click(object sender, EventArgs e)
    {
        if (_queryTextBox.Text is not { Length: >= MinQueryLen } query)
            return;
        
        Logger.Verbose($"Searching {query}: " +
                       $"{(_nameCheckBox.Checked ? SearchFlags.Name : string.Empty)}" +
                       $"{(_fullNameCheckBox.Checked ? SearchFlags.FullName : string.Empty)}" +
                       $"{(_bundleNameCheckBox.Checked ? SearchFlags.BundleName : string.Empty)}" +
                       $"{(_regexCheckBox.Checked ? SearchFlags.Regex : string.Empty)}");
        
        List<IGrouping<string, IPatchIndexer.IndexedAsset>> results;
        _searchButton.Enabled = false;
        _statusLabel.Text = @"Searching...";
        try
        {
            var flags = (_nameCheckBox.Checked ? SearchFlags.Name : SearchFlags.None)
                        | (_fullNameCheckBox.Checked ? SearchFlags.FullName : SearchFlags.None)
                        | (_bundleNameCheckBox.Checked ? SearchFlags.BundleName : SearchFlags.None)
                        | (_regexCheckBox.Checked ? SearchFlags.Regex : SearchFlags.None);
            
            // Search off the UI thread because it may take some time
            results = await Task.Run(() => _indexer.Search(query, flags));
        }
        catch (RegexParseException ex)
        {
            _statusLabel.Text = $@"Invalid regex: {ex.Message}";
            return;
        }
        finally
        {
            _searchButton.Enabled = true;
        }

        // Create the tree with bundle names as node and assets as children
        _resultsTreeView.BeginUpdate();
        _resultsTreeView.Nodes.Clear();
        foreach (var bundle in results)
        {
            var bundleNode = _resultsTreeView.Nodes.Add(bundle.Key);
            foreach (var asset in bundle)
            {
                var assetNode = bundleNode.Nodes.Add(asset.FullName);
                assetNode.Tag = asset;
                HideCheckBox(assetNode);
            }
        }
        _resultsTreeView.EndUpdate();

        _statusLabel.Text = results.Count == 0 ? "No results" : $"{results.Sum(b => b.Count())} assets in {results.Count} bundles";
    }

    // Only enable the search button if there is a query long enough, and at least one of the 3 search flag is checked
    private void RefreshSearchButtonState(object sender, EventArgs e) => 
        _searchButton.Enabled = _queryTextBox.Text.Length >= MinQueryLen && (_nameCheckBox.Checked || _fullNameCheckBox.Checked || _bundleNameCheckBox.Checked);

    // Prevent assets from being checked in the tree view
    private void ResultsTreeView_BeforeCheck(object sender, TreeViewCancelEventArgs e) => e.Cancel = e.Node?.Tag is IPatchIndexer.IndexedAsset;

    #region Hide checkboxes in asset nodes
    
    // I have no idea

    private const int TvmSetItemW = 0x1100 + 63;
    private const uint TvifState = 0x8;
    private const uint TvifHandle = 0x10;
    private const uint TvisStateImageMask = 0xF000;
    
    private void HideCheckBox(TreeNode node)
    {
        var item = new TvItem
        {
            Mask = TvifHandle | TvifState,
            HItem = node.Handle,
            StateMask = TvisStateImageMask,
            State = 0
        };
        SendMessage(_resultsTreeView.Handle, TvmSetItemW, IntPtr.Zero, ref item);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct TvItem
    {
        public uint Mask;
        public IntPtr HItem;
        public uint State;
        public uint StateMask;
        public IntPtr Text;
        public int TextMax;
        public int Image;
        public int SelectedImage;
        public int Children;
        public IntPtr LParam;
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref TvItem lParam);

    #endregion
}