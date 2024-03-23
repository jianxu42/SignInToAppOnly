using System;
using System.Threading;
using System.Windows.Automation;

public class SITAO
{
    public static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: SignInToAppOnly.exe <windowName> <hyperlinkText>");
            return;
        }

        string windowName = args[0];
        string hyperlinkText = args[1];

        Console.WriteLine($"Starting monitoring for window: {windowName} and hyperlink: {hyperlinkText}...");

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
                    Console.WriteLine($"Found window: {windowName}");

                    // Find the Hyperlink control named hyperlinkText
                    var hyperLinkCondition = new AndCondition(
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Hyperlink),
                        new PropertyCondition(AutomationElement.NameProperty, hyperlinkText)
                    );

                    AutomationElement hyperLink = window.FindFirst(TreeScope.Descendants, hyperLinkCondition);

                    if (hyperLink != null)
                    {
                        Console.WriteLine($"Found hyperlink: {hyperlinkText}.");
                        if (hyperLink.TryGetCurrentPattern(InvokePattern.Pattern, out object pattern))
                        {
                            var invokePattern = (InvokePattern)pattern;
                            invokePattern.Invoke();
                            Console.WriteLine($"Clicked on hyperlink: {hyperlinkText}.");
                        }
                        else
                        {
                            Console.WriteLine("The hyperlink cannot be invoked.");
                        }
                    }
                    /*else
                    {
                        Console.WriteLine($"Could not find the hyperlink: {hyperlinkText}.");
                    }*/
                }
                /*else
                {
                    Console.WriteLine($"No window found with title: {windowName}.");
                }*/
            }

            Thread.Sleep(500);
        }
    }
}
