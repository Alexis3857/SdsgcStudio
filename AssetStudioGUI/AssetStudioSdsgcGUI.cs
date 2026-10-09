using System.Drawing;
using System.Windows.Forms;
using AssetStudio;
using Font = System.Drawing.Font;

namespace AssetStudioGUI;

public partial class AssetStudioGUIForm
{
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
            _sdsgcMenu.DropDownItems.Add(textBox);
        }
    }
    
    private static ToolStripTextBox DefaultSdsgcToolStripTextBox() => new()
    {
        BorderStyle = BorderStyle.FixedSingle,
        Font = new Font("Segoe UI", 9F),
        Size = new Size(300, 23)
    };
}