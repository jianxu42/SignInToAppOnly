using log4net;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]
public class SITAO
{
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private static string windowName = null; // Initialize to null to indicate it's optional
    private static string hyperlinkText;

    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: SignInToAppOnly.exe <hyperlinkText> [windowName]");
            return;
        }

        hyperlinkText = args[0];
        if (args.Length > 1) // Window name is provided as an optional second argument
        {
            windowName = args[1];
        }

        log.Info($"Starting monitoring for hyperlink: {hyperlinkText}" + (windowName != null ? $" in window: {windowName}" : "") + "...");

        // Start a separate thread for monitoring
        Thread monitorThread = new Thread(MonitorWindows);
        monitorThread.IsBackground = true;
        monitorThread.Start();

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static void MonitorWindows()
    {
        while (true)
        {
            TryFindAndInvokeHyperlinks();
            Thread.Sleep(1000); // Wait for 1 second before trying again
        }
    }

    private static void TryFindAndInvokeHyperlinks()
    {
        // Retrieve the root element on the desktop
        AutomationElement rootElement = AutomationElement.RootElement;
        var hyperLinkCondition = new AndCondition(
            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Hyperlink),
            new PropertyCondition(AutomationElement.NameProperty, hyperlinkText)
        );

        if (windowName != null)
        {
            // If a window name is specified, search for the window
            var windowCondition = new PropertyCondition(AutomationElement.NameProperty, windowName);
            AutomationElement window = rootElement.FindFirst(TreeScope.Children, windowCondition);
            if (window != null)
            {
                FindAndInvokeHyperlinksInWindow(window, hyperLinkCondition);
            }
            else
            {
                log.Warn($"Window: {windowName} not found.");
            }
        }
        else
        {
            // No window name specified, search through all top-level windows
            var windows = rootElement.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window));
            Parallel.ForEach(windows.Cast<AutomationElement>(), window =>
            {
                FindAndInvokeHyperlinksInWindow(window, hyperLinkCondition);
            });
        }
    }

    private static void FindAndInvokeHyperlinksInWindow(AutomationElement window, Condition hyperLinkCondition)
    {
        var hyperLinks = window.FindAll(TreeScope.Descendants, hyperLinkCondition);
        Parallel.ForEach(hyperLinks.Cast<AutomationElement>(), hyperLink =>
        {
            if (hyperLink.TryGetCurrentPattern(InvokePattern.Pattern, out object pattern))
            {
                var invokePattern = (InvokePattern)pattern;
                invokePattern.Invoke();
                log.Info($"Invoked hyperlink: {hyperlinkText} in window: {window.Current.Name}.");
            }
            else
            {
                log.Warn($"Hyperlink: {hyperlinkText} in window: {window.Current.Name} could not be invoked.");
            }
        });
    }
}
