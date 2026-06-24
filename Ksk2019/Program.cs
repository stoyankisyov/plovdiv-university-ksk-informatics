using System.Globalization;

namespace Ksk2019;

public class Client(string name, DateTime registrationDate, int orderCount, decimal totalAmount)
{
    public string Name { get; set; } = name;

    public DateTime RegistrationDate { get; set; } = registrationDate;

    public int OrderCount { get; set; } = orderCount;

    public decimal TotalAmount { get; set; } = totalAmount;

    public int Rating { get; set; } = orderCount switch
    {
        <= 99 => 1,
        <= 299 => 2,
        <= 499 => 3,
        <= 999 => 4,
        _ => 5
    };

    public override string ToString()
    {
        var ratingStars = new string('*', Rating);

        return $"{Name}, {OrderCount},  {TotalAmount}, {RegistrationDate:dd.MM.yyyy}, {ratingStars}";
    }
}

public class Program
{
    public static void Main()
    {
        var clients = new List<Client>();

        while (true)
        {
            Console.WriteLine("1. Въведи клиенти");
            Console.WriteLine("2. Всички клиенти сортирани по име");
            Console.WriteLine("3. Кленти с рейтинг ** с намаляваща обща сума");
            Console.WriteLine("4. Брой клиенти за рейтинг");
            Console.WriteLine("0. Изход");

            var choice = ReadInt(0, 4, "Избор: ");

            switch (choice)
            {
                case 1:
                    var clientCount = ReadInt(1, 5000, "Брой клиенти за въвеждане: ");
                    clients.AddRange(BuildClients(clientCount));
                    break;

                case 2:
                    Sort(clients, ShouldSwapByName);
                    PrintClients(clients);
                    break;

                case 3:
                    var clientsWithTwoStars = FilterByRating(clients, 2);
                    Sort(clientsWithTwoStars, ShouldSwapByTotalAmount);
                    PrintClients(clientsWithTwoStars);
                    break;

                case 4:
                    var rating = ReadInt(1, 5, "Въведете рейтинг (1-5): ");
                    PrintClientCountByRatingForEachYear(clients, rating);
                    break;

                case 0:
                    return;
            }
        }
    }

    private static List<Client> BuildClients(int count)
    {
        var clients = new List<Client>();

        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($"Въвеждане на клиент {i + 1}:");
            var name = ReadName(1, 40, "Име: ");
            var registrationDate = ReadRegistrationDate("Дата на регистрация (dd.MM.yyyy): ");
            var orderCount = ReadInt(1, 9999, "Брой поръчки: ");
            var totalAmount = ReadTotalAmount(0, "Обща сума: ");
            var client = new Client(name, registrationDate, orderCount, totalAmount);

            clients.Add(client);
        }

        return clients;
    }

    private static void PrintClientCountByRatingForEachYear(List<Client> clients, int rating)
    {
        var filteredClients = FilterByRating(clients, rating);

        var years = new List<int>();
        foreach (var client in filteredClients)
        {
            if (!years.Contains(client.RegistrationDate.Year))
            {
                years.Add(client.RegistrationDate.Year);
            }
        }

        Sort(years, (first, second) => first > second);

        foreach (var year in years)
        {
            var clientCount = 0;
            foreach (var client in filteredClients)
            {
                if (client.RegistrationDate.Year == year)
                {
                    clientCount++;
                }
            }

            Console.WriteLine($"{year} - {clientCount}");
        }
    }

    private static List<Client> FilterByRating(List<Client> clients, int rating)
    {
        var filteredClients = new List<Client>();
        foreach (var client in clients)
        {
            if (client.Rating == rating)
            {
                filteredClients.Add(client);
            }
        }

        return filteredClients;
    }

    private static void Sort<T>(List<T> items, Func<T, T, bool> shouldSwap)
    {
        for (var i = 0; i < items.Count - 1; i++)
        {
            for (var j = i + 1; j < items.Count; j++)
            {
                if (shouldSwap(items[i], items[j]))
                {
                    (items[i], items[j]) = (items[j], items[i]);
                }
            }
        }
    }

    private static bool ShouldSwapByName(Client first, Client second) =>
        string.Compare(first.Name, second.Name, StringComparison.Ordinal) > 0;

    private static bool ShouldSwapByTotalAmount(Client first, Client second)
    {
        if (first.TotalAmount == second.TotalAmount)
        {
            return ShouldSwapByName(first, second);
        }

        return first.TotalAmount < second.TotalAmount;
    }

    private static void PrintClients(List<Client> clients)
    {
        foreach (var client in clients)
        {
            Console.WriteLine(client.ToString());
        }
    }

    private static int ReadInt(int min, int max, string message)
    {
        Console.Write(message);

        while (true)
        {
            var input = Console.ReadLine();
            var isValid = int.TryParse(input, out int result);

            if (isValid && result >= min && result <= max)
            {
                return result;
            }

            Console.WriteLine($"Моля въведете число между {min} и {max}.");
        }
    }

    private static string ReadName(int minLength, int maxLength, string message)
    {
        Console.Write(message);

        while (true)
        {
            var input = Console.ReadLine();

            if (input is not null
                && input.Length >= minLength
                && input.Length <= maxLength
                && input.Split(' ').Length == 2)
            {
                return input;
            }

            Console.WriteLine($"Моля въведете стринг с дължина между {minLength} и {maxLength}.");
        }
    }

    private static DateTime ReadRegistrationDate(string message)
    {
        Console.Write(message);

        while (true)
        {
            var input = Console.ReadLine();
            var isValid = DateTime.TryParseExact(input, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var date);

            if (isValid)
            {
                return date;
            }

            Console.WriteLine("Моля въведете дата във формат dd.MM.yyyy.");
        }
    }

    private static decimal ReadTotalAmount(int min, string message)
    {
        Console.Write(message);

        while (true)
        {
            var input = Console.ReadLine();
            var isValid = decimal.TryParse(input, out var result);

            if (isValid && result >= min)
            {
                return result;
            }

            Console.WriteLine($"Моля въведете число по-голямо или равно на {min}.");
        }
    }
}