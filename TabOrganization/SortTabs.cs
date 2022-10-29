using System;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.VisualStudio.PlatformUI.Shell;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace TabOrganization
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class SortTabs
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("1c79ba22-6635-4477-bc21-1cb6f58ad271");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="SortTabs"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private SortTabs(AsyncPackage package, OleMenuCommandService commandService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(Sort, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static SortTabs Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IAsyncServiceProvider ServiceProvider => _package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in SortTabs's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new SortTabs(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Sort(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var activeView = ViewManager.Instance.ActiveView;
            var tabGroup = activeView.Parent;
            var visibleChildren = tabGroup.VisibleChildren.OfType<View>();
            var numPinnedChildren = visibleChildren.Where(x => x.IsPinned).Count();
            var sortedChildren = visibleChildren.Where(x => !x.IsPinned).OrderBy(x => x.Title?.ToString(), StringComparer.CurrentCultureIgnoreCase).ToList();

            foreach (var childWithIndex in sortedChildren.Select((x, i) => (child: x, index: i)))
            {
                var (child, index) = childWithIndex;
                DockOperations.MoveTab(child, null, index + numPinnedChildren, false);
            }
        }
    }
}
