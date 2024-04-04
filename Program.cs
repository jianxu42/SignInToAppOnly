using log4net;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Automation;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]
public class SITAO
{
    private static readonly ILog log = LogManager.GetLogger(typeof(SITAO));
    private static string windowName = null;
    private static string hyperlinkText;
    private const int DelayTime = 500;

    public static void Main(string[] args)
    {
        if (args.Length < 1 || string.IsNullOrWhiteSpace(args[0]))
        {
            Console.WriteLine("Usage: SignInToAppOnly.exe <hyperlinkText> [windowName]");
            return;
        }

        hyperlinkText = args[0];
        if (args.Length > 1)
        {
            windowName = args[1];
        }

        log.Info($"Starting monitoring for hyperlink: {hyperlinkText}" + (windowName != null ? $" in window: {windowName}" : "") + "...");

        Task.Run(MonitorWindows);

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static async Task MonitorWindows()
    {
        while (true)
        {
            await TryFindAndInvokeHyperlinksAsync().ConfigureAwait(false);
            await Task.Delay(DelayTime).ConfigureAwait(false);
        }
    }

    private static async Task TryFindAndInvokeHyperlinksAsync()
    {
        AutomationElement rootElement = AutomationElement.RootElement;

        var hyperLinkCondition = new AndCondition(
            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Hyperlink),
            new PropertyCondition(AutomationElement.NameProperty, hyperlinkText)
        );

        if (windowName != null)
        {
            await FindAndInvokeHyperlinksInSpecificWindowAsync(rootElement, hyperLinkCondition).ConfigureAwait(false);
        }
        else
        {
            await FindAndInvokeHyperlinksInAllWindowsAsync(rootElement, hyperLinkCondition).ConfigureAwait(false);
        }
    }

    private static async Task FindAndInvokeHyperlinksInSpecificWindowAsync(AutomationElement rootElement, Condition hyperLinkCondition)
    {
        var windowCondition = new PropertyCondition(AutomationElement.NameProperty, windowName);
        AutomationElement window = rootElement.FindFirst(TreeScope.Children, windowCondition);
        if (window != null)
        {
            await FindAndInvokeHyperlinksInWindowAsync(window, hyperLinkCondition).ConfigureAwait(false);
        }
        else
        {
            log.Warn($"Window: {windowName} not found.");
        }
    }

    private static async Task FindAndInvokeHyperlinksInAllWindowsAsync(AutomationElement rootElement, Condition hyperLinkCondition)
    {
        var windows = rootElement.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window));
        var tasks = windows.Cast<AutomationElement>().Select(window => FindAndInvokeHyperlinksInWindowAsync(window, hyperLinkCondition));
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private static async Task FindAndInvokeHyperlinksInWindowAsync(AutomationElement window, Condition hyperLinkCondition)
    {
        var hyperLinks = window.FindAll(TreeScope.Descendants, hyperLinkCondition);
        var tasks = hyperLinks.Cast<AutomationElement>().Select(async currentHyperLink =>
        {
            if (currentHyperLink.TryGetCurrentPattern(InvokePattern.Pattern, out object pattern))
            {
                var invokePattern = (InvokePattern)pattern;
                await Task.Run(() => invokePattern.Invoke()).ConfigureAwait(false);
                log.Info($"Invoked hyperlink: {hyperlinkText} in window: {window.Current.Name}.");
            }
            else
            {
                log.Warn($"Hyperlink: {hyperlinkText} in window: {window.Current.Name} could not be invoked.");
            }
        });

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
