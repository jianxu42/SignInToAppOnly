using log4net;
using System;
using System.Threading;
using System.Windows.Automation;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]
public class SITAO
{
    private static readonly ILog log = LogManager.GetLogger(typeof(SITAO));
    private static string windowName = null;
    private static string hyperlinkText;

    public static void Main(string[] args)
    {
        if (args.Length < 1)
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

        Thread monitorThread = new Thread(MonitorWindows) { IsBackground = true };
        monitorThread.Start();

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static void MonitorWindows()
    {
        while (true)
        {
            TryFindAndInvokeHyperlinks();
            Thread.Sleep(500);
        }
    }

    private static void TryFindAndInvokeHyperlinks()
    {
        AutomationElement rootElement = AutomationElement.RootElement;
        var hyperLinkCondition = new AndCondition(
            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Hyperlink),
            new PropertyCondition(AutomationElement.NameProperty, hyperlinkText)
        );

        if (windowName != null)
        {
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
            var windows = rootElement.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window));
            foreach (AutomationElement window in windows)
            {
                FindAndInvokeHyperlinksInWindow(window, hyperLinkCondition);
            }
        }
    }

    private static void FindAndInvokeHyperlinksInWindow(AutomationElement window, Condition hyperLinkCondition)
    {
        var hyperLinks = window.FindAll(TreeScope.Descendants, hyperLinkCondition);
        foreach (AutomationElement hyperLink in hyperLinks)
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
        }
    }
}
