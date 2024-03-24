using log4net;
using System;
using System.Threading;
using System.Windows.Automation;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]
public class SITAO
{
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private static string windowName;
    private static string hyperlinkText;

    public static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            log.Warn("Usage: SignInToAppOnly.exe <windowName> <hyperlinkText>");
            return;
        }

        windowName = args[0];
        hyperlinkText = args[1];

        log.Info($"Starting monitoring for window: {windowName} and hyperlink: {hyperlinkText}...");

        // Start a separate thread for monitoring
        Thread monitorThread = new Thread(MonitorWindows)
        {
            IsBackground = true
        };
        monitorThread.Start();

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static void MonitorWindows()
    {
        while (true) // Replace with a condition to stop the monitoring if necessary
        {
            TryFindAndInvokeHyperlink();
            Thread.Sleep(1000); // Wait for 1 second before trying again
        }
    }

    private static void TryFindAndInvokeHyperlink()
    {
        // Retrieve the root element on the desktop
        AutomationElement rootElement = AutomationElement.RootElement;

        // Search for the window using the window title
        var windowCondition = new PropertyCondition(AutomationElement.NameProperty, windowName);
        AutomationElement window = rootElement.FindFirst(TreeScope.Children, windowCondition);

        if (window != null)
        {

            // Find the Hyperlink control named hyperlinkText
            var hyperLinkCondition = new AndCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Hyperlink),
                new PropertyCondition(AutomationElement.NameProperty, hyperlinkText)
            );

            AutomationElement hyperLink = window.FindFirst(TreeScope.Descendants, hyperLinkCondition);

            if (hyperLink != null && hyperLink.TryGetCurrentPattern(InvokePattern.Pattern, out object pattern))
            {
                var invokePattern = (InvokePattern)pattern;
                invokePattern.Invoke();
                log.Info($"Found window: {windowName} and invoked hyperlink: {hyperlinkText}.");
            }
        }
    }
}
