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
    private const long OneGb = 1 << 30;  // 1 gb in bytes
    
    // If the user tries to download more than DownloadWarningThreshold, a confirmation MessageBox will appear
    private const long DownloadWarningThreshold = OneGb;
    
    // Only search if the user typed more than 2 chars
    private const int MinQueryLen = 2;

    private readonly IPatchIndexer _indexer;

    // Bundles selected by the user with their pack, kept across searches
    private readonly HashSet<IPatchIndexer.IndexedBundle> _selectedBundles = [];

    /// <summary>
    /// Bundles selected when the user before closing the form<br/>
    /// The <see cref="BundleData"/> can only be bundles from the <see cref="BundlePackData.IncludeBundles"/> of their key
    /// </summary>
    public ILookup<BundlePackData, BundleData> SelectedBundles { get; private set; }

    public BundleSearcherForm(IPatchIndexer indexer)
    {
        InitializeComponent();
        _indexer = indexer;
    }

    // Search the matching bundles and update _resultsTreeView
    private async void SearchButton_Click(object sender, EventArgs e)
    {
        if (_queryTextBox.Text is not { Length: >= MinQueryLen } query)
            return;
        
        Logger.Verbose($"Searching \'{query}\' with flags: " +
                       $"{(_nameCheckBox.Checked ? SearchFlags.Name : string.Empty)}" +
                       $"{(_fullNameCheckBox.Checked ? SearchFlags.FullName : string.Empty)}" +
                       $"{(_bundleNameCheckBox.Checked ? SearchFlags.BundleName : string.Empty)}" +
                       $"{(_regexCheckBox.Checked ? SearchFlags.Regex : string.Empty)}");
        
        List<IPatchIndexer.IndexedBundle> results;
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
            var bundleNode = new TreeNode(bundle.Bundle.Name) { Tag = bundle, Checked = _selectedBundles.Contains(bundle) };
            _resultsTreeView.Nodes.Add(bundleNode);
            foreach (var asset in bundle.Assets)
            {
                var assetNode = bundleNode.Nodes.Add(asset.FullName);
                assetNode.Tag = asset;
                HideCheckBox(assetNode);
            }
        }
        _resultsTreeView.EndUpdate();

        RefreshResultsState();
    }

    // Only enable the search button if there is a query long enough, and at least one of the 3 search flag is checked
    private void RefreshSearchButtonState(object sender, EventArgs e) => 
        _searchButton.Enabled = _queryTextBox.Text.Length >= MinQueryLen && (_nameCheckBox.Checked || _fullNameCheckBox.Checked || _bundleNameCheckBox.Checked);

    // Prevent assets from being checked in the tree view
    private void ResultsTreeView_BeforeCheck(object sender, TreeViewCancelEventArgs e) => e.Cancel = e.Node?.Parent is not null;

    // Only bundle nodes reach AfterCheck, since BeforeCheck cancels assets
    private void ResultsTreeView_AfterCheck(object sender, TreeViewEventArgs e)
    {
        var bundle = (IPatchIndexer.IndexedBundle)e.Node!.Tag!;
        if (e.Node.Checked) _selectedBundles.Add(bundle);
        else _selectedBundles.Remove(bundle);

        RefreshResultsState();
    }

    // Check every unselected bundle in the results, AfterCheck refreshes the results state
    private void SelectAllButton_Click(object sender, EventArgs e)
    {
        foreach (var bundle in _resultsTreeView.Nodes.Cast<TreeNode>().Where(b => !b.Checked))
            bundle.Checked = true;
    }

    // Unselect every bundle, including the ones selected in previous searches
    private void UnselectAllButton_Click(object sender, EventArgs e)
    {
        _selectedBundles.Clear();
        foreach (var bundle in _resultsTreeView.Nodes.Cast<TreeNode>().Where(b => b.Checked))
            bundle.Checked = false;

        RefreshResultsState();
    }

    private void DownloadButton_Click(object sender, EventArgs e)
    {
        // Group the selected bundles by pack, since packs contain several bundles
        var selectedBundles = _selectedBundles.ToLookup(b => b.Pack, b => b.Bundle);

        // If the download size is too big a confirmation MessageBox will appear
        var downloadSize = selectedBundles.Sum(p => p.Key.Size);
        if (downloadSize > DownloadWarningThreshold)
        {
            var gb = downloadSize / (double)OneGb;
            if (MessageBox.Show($@"Attempting to download {gb:0.##} GB. Continue?", @"Confirm Download", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) 
                return;
        }

        SelectedBundles = selectedBundles;
        DialogResult = DialogResult.OK;  // Close the form
    }

    // Show the results count and how many bundles are selected, and only enable the download and unselect buttons if at least one is
    // The select button is only enabled if at least one bundle in the results is not selected
    private void RefreshResultsState()
    {
        var bundles = _resultsTreeView.Nodes.Cast<TreeNode>().ToList();
        var selectedCount = _selectedBundles.Count;
        _statusLabel.Text = bundles.Count == 0 ? "No results" : $"{bundles.Count} bundles";
        _downloadButton.Text = $@"Download {selectedCount} Bundles";
        _downloadButton.Enabled = selectedCount > 0;
        _selectAllButton.Enabled = bundles.Any(b => !b.Checked);
        _unselectAllButton.Enabled = selectedCount > 0;
    }

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
    
    // In a normal TreeView double clicks visually check the box without updating TreeNode.Checked or calling BeforeCheck/AfterCheck
    // This converts double clicks on a checkbox into single clicks
    public class CheckBoxTreeView : TreeView
    {
        private const int WmLButtonDown = 0x201;
        private const int WmLButtonDblClk = 0x203;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WmLButtonDblClk && CheckBoxes)
            {
                var lParam = (long)m.LParam;
                if (HitTest((short)lParam, (short)(lParam >> 16)).Location == TreeViewHitTestLocations.StateImage)
                    m.Msg = WmLButtonDown;
            }
            base.WndProc(ref m);
        }
    }
}