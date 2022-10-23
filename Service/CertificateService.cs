using System;
using Certificate_Creator.Models;
using Certificate_Creator.Service;
using Certificate_Creator.Services;

namespace Certificate_Creator.Services
{
    public class CertificateService
    {

        /// <summary>
        /// 
        /// </summary>
        public void CreateRoot()
        {
            CreatorService _service = new();
            var model = CreateModelForRoot();

            var checker = _service.GenerateRootCertificate(model);

            if (!checker)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something wrong, try change data\n\n");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Success, don't forget to save your password\n\n");
                Console.ResetColor();
            }

            Thread.Sleep(3000);
        }


        /// <summary>
        /// 
        /// </summary>
        public void CreateDefault()
        {
            CreatorService _service = new();

            var certificateExplorer = Display();
            var model = CreateModelForDefault(certificateExplorer);

            var checker = _service.GenerateCertificate(model);

            if (!checker)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something wrong, try change data\n\n");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Success, don't forget to save your password\n\n");
                Console.ResetColor();
            }

            Thread.Sleep(3000);
        }


        private CertificateExplorer Display()
        {
            int counter = 0;
            CertificateExplorer certificateExplorer = new(true);
            certificateExplorer.LoadCertificates();
            foreach (var certificate in certificateExplorer.Certificates)
            {
                Console.WriteLine($"{counter++} >>> {certificate} \n");
            }
            return certificateExplorer;
        }


        private CertificateDataModel CreateModelForRoot()
        {
            string pass = GeneratePassword();
            Console.Write("Enter company name: ");
            string name = Console.ReadLine();
            Console.Write("Enter the number of half years(1 = 182 days): ");
            int halfYears = Convert.ToInt32(Console.ReadLine());
            string folder = Directory.GetCurrentDirectory();

            CertificateDataModel model = new()
            {
                CertificateName = name,
                OutFolder = folder,
                CertificateDuration = halfYears,
                Password = pass
            };

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Your password: {pass}");
            Console.WriteLine($"Certificate folder: {folder}");
            Console.ResetColor();

            return model;
        }


        private CertificateDataModel CreateModelForDefault(CertificateExplorer certificateExplorer)
        {
            Console.Write("Select number of certificate: ");
            int certificateNumb = Convert.ToInt32(Console.ReadLine());
            string pass = GeneratePassword();
            Console.Write("Enter company name: ");
            string name = Console.ReadLine();
            Console.Write("Enter the number of half years(1 = 182 days): ");
            int halfYears = Convert.ToInt32(Console.ReadLine());
            string folder = Directory.GetCurrentDirectory();

            CertificateDataModel model = new()
            {
                RootCertificate = certificateExplorer.Certificates[int.Parse(certificateNumb.ToString())].Certificate,
                CertificateName = name,
                OutFolder = folder,
                CertificateDuration = halfYears,
                Password = pass
            };

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Your password: {pass}");
            Console.WriteLine($"Certificate folder: {folder}");
            Console.ResetColor();

            return model;
        }


        /// <summary>
        /// Code was taken from
        /// </summary>
        /// <see cref="https://github.com/ddoo5/PC">PC</see>
        private string GeneratePassword()
        {
            string password = "";
            int x;
            Random rnd = new();
            int _randomLenght = rnd.Next(5, 10);

            List<char> _lowlet = new List<char>();
            _lowlet.AddRange("abcdefghijklmnopqrstuvwxyz");

            List<char> _caplet = new List<char>();
            _caplet.AddRange("ABCDEFGHIJKLMNOPQRSTUVWXYZ");

            List<char> _sign = new List<char>();
            _sign.AddRange("-'^*~@/!#$%&");

            List<char> _numbers = new List<char>();
            _numbers.AddRange("1234567890");

            char RandomLowlet()
            {
                x = rnd.Next(0, _lowlet.Count);
                char c = _lowlet[x];

                return c;
            }

            char RandomCaplet()
            {
                x = rnd.Next(0, _caplet.Count);
                char c = _caplet[x];

                return c;
            }

            char RandomSign()
            {
                x = rnd.Next(0, _sign.Count);
                char c = _sign[x];

                return c;
            }

            char RandomNumber()
            {
                x = rnd.Next(0, _numbers.Count);
                char c = _numbers[x];

                return c;
            }

            char Randomizer(int q)
            {
                char letter = 'x';
                int r = rnd.Next(0, q + 1);

                if (q == 4)
                {
                    r = rnd.Next(0, q - 1);
                    switch (r)
                    {
                        case 0:
                            letter = RandomLowlet();
                            break;
                        case 1:
                            letter = RandomCaplet();
                            break;
                        case 2:
                            letter = RandomSign();
                            break;
                    }
                }
                else
                {
                    switch (r)
                    {
                        case 0:
                            letter = RandomLowlet();
                            break;
                        case 1:
                            letter = RandomCaplet();
                            break;
                        case 2:
                            letter = RandomNumber();
                            break;
                        case 3:
                            letter = RandomSign();
                            break;
                    }
                }
                return letter;
            }

            for (int i = 0; i < _randomLenght; i++)
            {
                char f = Randomizer(3);
                password += f;
            }

            return password;
        }
    }
}

