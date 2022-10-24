using System.Runtime.CompilerServices;
using Certificate_Creator;
using Certificate_Creator.Services;
using Certificate_Creator.UI;



CertificateService _certificateService = new();


Console.Title = "Certificate Creator - Console app";
Console.Clear();
Console.CursorVisible = false;

UI.Greeting();

Thread.Sleep(5000);

while (true)
{
    Console.ResetColor();

    try
    {
        UI.SelectOptionsOfCreatingSertificate();
        Console.CursorVisible = true;

        short enter = Convert.ToInt16(Console.ReadLine());

        Console.CursorVisible = false;

        switch (enter)
        {
            case 0:
                return;
            case 1:
                _certificateService.CreateRoot();
                break;
            case 2:
                _certificateService.CreateDefault();
                break;
            default:
                var ex = new ArgumentException("Uncorrect enter, try again", $"{enter}");
                ex.HelpLink = "https://www.wikihow.com/Increase-Your-IQ";
                throw ex;
        }

    }
    catch(Exception a)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"I caught exception: {a.Message}\n" +
            $"Here's help link: {a.HelpLink}\n");
        Thread.Sleep(6500);
        Console.Clear();
    }

}