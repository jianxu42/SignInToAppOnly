using System.Threading;
using System.Windows.Automation;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]
public class SITAO
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    public static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            log.Warn("Usage: SignInToAppOnly.exe <windowName> <hyperlinkText>");
            return;
        }

        string windowName = args[0];
        string hyperlinkText = args[1];

        log.Info($"Starting monitoring for window: {windowName} and hyperlink: {hyperlinkText}...");

        while (true)
        {
            // Retrieve the root element on the desktop
            AutomationElement rootElement = AutomationElement.RootElement;

            if (rootElement != null)
            {
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

                    if (hyperLink != null)
                    {
                        log.Info($"Found window: {windowName} and hyperlink: {hyperlinkText}.");
                        if (hyperLink.TryGetCurrentPattern(InvokePattern.Pattern, out object pattern))
                        {
                            var invokePattern = (InvokePattern)pattern;
                            invokePattern.Invoke();
                            log.Info($"Clicked on hyperlink: {hyperlinkText}.");
                        }
                        else
                        {
                            log.Error("The hyperlink cannot be invoked.");
                        }
                    }
                }
            }

            Thread.Sleep(500);
        }
    }
}
