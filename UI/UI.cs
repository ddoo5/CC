using System;
namespace Certificate_Creator.UI
{
    internal sealed class UI
    {
        public static void Greeting()
        {
            Console.WriteLine("Welcome to the 'Certificate Creator'!\n" +
                "Here you can create root or default certificate. You also can do it from the face of your company or like self-developer, submit by them buildings, projects, emails and other things\n");
        }


        public static void SelectOptionsOfCreatingSertificate()
        {
            Console.WriteLine("Choose options: \n" +
                "1) Create root certificate\n" +
                "2) Create default certificate\n" +
                "0 - Exit\n");
            Console.Write("Enter: ");
        }
    }
}

