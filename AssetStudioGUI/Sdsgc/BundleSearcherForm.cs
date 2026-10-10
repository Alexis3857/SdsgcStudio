using System;
using System.Collections.Generic;
using System.Linq;
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
                bundleNode.Nodes.Add(asset.FullName).Tag = asset;
        }
        _resultsTreeView.EndUpdate();

        _statusLabel.Text = results.Count == 0 ? "No results" : $"{results.Sum(b => b.Count())} assets in {results.Count} bundles";
    }

    // Only enable the search button if there is a query long enough, and at least one of the 3 search flag is checked
    private void RefreshSearchButtonState(object sender, EventArgs e) => 
        _searchButton.Enabled = _queryTextBox.Text.Length >= MinQueryLen && (_nameCheckBox.Enabled || _fullNameCheckBox.Checked || _bundleNameCheckBox.Checked);
}