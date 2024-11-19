using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Club
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ComputerClub computerClub = new ComputerClub(8);
            computerClub.Work();
        }

        class ComputerClub
        {
            private int _money = 0;
            private List<Computer> _computers = new List<Computer>();
            private Queue<Client> _clients = new Queue<Client>();
            private int _totalMinutesWorked = 0;

            public ComputerClub(int computersCount)
            {
                Random random = new Random();

                for (int i = 0; i < computersCount; i++)
                {
                    _computers.Add(new Computer(random.Next(5, 15)));
                }
                CreateNewClients(25, random);
            }

            public void CreateNewClients(int count, Random random)
            {
                for (int i = 0; i < count; i++)
                {
                    _clients.Enqueue(new Client(random.Next(1000, 2500), random));
                }

            }

            public void Work()
            {
                
                while (_clients.Count > 0)
                {


                    Client newClient = _clients.Dequeue();
                    Console.WriteLine($" Баланс компьютерного клуба: {_money} руб. Ждём нового клиента.");
                    Console.WriteLine($"У вас новый клиент, он хочет купить {newClient.DesiredMinutes} минут.");
                    ShowAllComputersState();

                    Console.Write("\nВы предлогаете компьютер под номером: ");
                    string userInput = Console.ReadLine();

                    if (int.TryParse(userInput, out int computerNumber))
                    {
                        computerNumber -= 1;

                        if (computerNumber >= 0 && computerNumber < _computers.Count)
                        {
                            if (_computers[computerNumber].IsTaken)
                            {
                                Console.WriteLine("Вы пытаетесь посадить клиента за компьютер, который уже занят. Клиент разозлился и ушёл.");
                            }
                            else
                            {
                                
                                if (newClient.CanAfford(_computers[computerNumber]))
                                {
                                    Console.WriteLine($"Клиент оплатил и сел за компьютер {computerNumber + 1}");
                                    _money += newClient.Pay();
                                    _computers[computerNumber].BecomeTaken(newClient);
                                }
                                else
                                {
                                    Console.WriteLine("У клиента не хватило денег и он ушёл.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Вы не смогли посадить клиента за компьютер, он ушёл.");
                        }
                    }
                    else
                    {
                        CreateNewClients(1, new Random());
                        Console.WriteLine("Неверный ввод! Повторите снова.");
                    }
                    Console.WriteLine("Чтобы перейти в слудующему клиенту, нажмите любую клавишу. ");
                    Console.ReadKey();
                    Console.Clear();
                    SpendOneMinute();

                    // Проверка ввода
                   
                   
                }
                int _totalEarnings = _money;
                Console.WriteLine($"\nРабочий день завершён. Итоговая выручка: {_totalEarnings} руб.");
                Console.WriteLine($"Общее время работы клуба: {_totalMinutesWorked} минут.");
                Console.ReadKey();

            }

            private void ShowAllComputersState()
            {

                Console.WriteLine("\nСписок всех компьютеров: ");
                for (int i = 0; i < _computers.Count; i++)
                {
                    Console.Write(i + 1 + " - ");
                    _computers[i].ShowState();
                }
            }
            private void SpendOneMinute()
            {
                foreach (var computer in _computers)
                {
                    computer.SpendOneMinutes(); 

                }
                _totalMinutesWorked++;
            }



            class Computer
            {
                private Client _client;
                private int _minutesRemaining;

                public bool IsTaken
                {
                    get
                    {
                        return _minutesRemaining > 0;
                    }
                }

                public int PricePerMinutes { get; private set; }

                public Computer(int pricePerMinute)
                {
                    PricePerMinutes = pricePerMinute;
                }
                public void BecomeTaken(Client client)
                {
                    _client = client;
                    _minutesRemaining = _client.DesiredMinutes;
                }
                public void BecomeEmpty()
                {
                    _client = null;
                }

                public void SpendOneMinutes()
                {
                    _minutesRemaining--;
                }

                public void ShowState()
                {
                    if (IsTaken)
                        Console.WriteLine($"Компьютер занят, осталось минут: {_minutesRemaining}");
                    else
                        Console.WriteLine($"Компьютер свободен, цена за минуту: {PricePerMinutes}");
                }
            }

            class Client
            {
                private int _money;
                private int _moneyToPay;
                public int DesiredMinutes { get; private set; }

                public Client(int money, Random random)
                {
                    _money = money;
                    DesiredMinutes = random.Next(10, 30);
                }

                public bool CanAfford(Computer coumputer)
                {
                    _moneyToPay = DesiredMinutes * coumputer.PricePerMinutes;
                    if (_money >= _moneyToPay)
                    {
                        return true;
                    }
                    else
                    {
                        _moneyToPay = 0;
                        return false;
                    }

                }
                public int Pay() 
                {
                    _money -= _moneyToPay;
                    return _moneyToPay;
                }
            }
        }
    }
}
