using System.Text;

namespace Ksk2021;

public class Item(int id, string name, decimal price, int quantity, int type)
{
    public int Id { get; set; } = id;

    public string Name { get; set; } = name;

    public decimal Price { get; set; } = price;

    public int Quantity { get; set; } = quantity;

    public int Type { get; set; } = type;

    public override string ToString() =>
        $"{Id}, {Name}, {Price} лв., {Quantity} бр., {Type} тип, {Id}{Name[..2]}{Type}";
}

public class Program
{
    public static void Main()
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        var items = new List<Item>();
        while (true)
        {
            Console.WriteLine("1. Въведи артикули");
            Console.WriteLine("2. Всички сортирани по име");
            Console.WriteLine("3. Справка по тип");
            Console.WriteLine("4. Артикули с тип 4");
            Console.WriteLine("0. Изход");

            var choice = ReadInt("Избор: ", 0, 4);

            switch (choice)
            {
                case 1:
                    if (items.Count <= 2000)
                    {
                        items.Add(EnterItem(items.Count));
                    }
                    else
                    {
                        Console.WriteLine("Достигнат е лимитът от 2000 артикула.");
                    }

                    break;

                case 2:
                    PrintSortedByName(items);
                    break;

                case 3:
                    var type = ReadInt("Въведете тип: ", 1, 5);
                    PrintByTypeSortedByTotalPrice(items, type);
                    break;

                case 4:
                    PrintStatisticsForTypeFour(items);
                    break;

                case 0:
                    return;
            }
        }
    }

    private static int ReadInt(string message, int minValue, int maxValue)
    {
        while (true)
        {
            Console.Write(message);

            var input = Console.ReadLine();
            var isValid = int.TryParse(input, out int result);

            if (isValid && result >= minValue && result <= maxValue)
            {
                return result;
            }

            Console.WriteLine($"Въведете цяло число в интервала [{minValue}; {maxValue}].");
        }
    }

    private static string ReadString(string message, int minLength, int maxLength)
    {
        while (true)
        {
            Console.Write(message);

            var input = Console.ReadLine();

            if (input is not null && input.Length >= minLength && input.Length <= maxLength)
            {
                return input;
            }

            Console.WriteLine($"Въведете стринг с дължина в интервала [{minLength}; {maxLength}].");
        }
    }

    private static decimal ReadDecimal(string message, decimal minValue, decimal maxValue)
    {
        while (true)
        {
            Console.Write(message);

            var input = Console.ReadLine();
            var isValid = decimal.TryParse(input, out decimal result);

            if (isValid && result >= minValue && result <= maxValue)
            {
                return result;
            }

            Console.WriteLine($"Въведете число в интервала [{minValue}; {maxValue}].");
        }
    }

    private static Item EnterItem(int lastId)
    {
        var id = lastId + 1;
        var name = ReadString("Име: ", 2, 20);
        var price = ReadDecimal("Цена: ", 0.1m, decimal.MaxValue);
        var quantity = ReadInt("Количество: ", 1, int.MaxValue);
        var type = ReadInt("Тип: ", 1, 5);

        return new Item(id, name, price, quantity, type);
    }

    private static void Sort(List<Item> items, Func<Item, Item, bool> predicate)
    {
        for (int i = 0; i < items.Count - 1; i++)
        {
            for (int j = i + 1; j < items.Count; j++)
            {
                if (predicate(items[i], items[j]))
                {
                    (items[i], items[j]) = (items[j], items[i]);
                }
            }
        }
    }

    private static bool ShouldSwapByName(Item a, Item b) =>
        string.Compare(a.Name, b.Name, StringComparison.Ordinal) > 0;

    private static bool ShouldSwapByTotalPrice(Item a, Item b) => a.Price * a.Quantity > b.Price * b.Quantity;

    private static void PrintList(List<Item> items)
    {
        foreach (var item in items)
        {
            Console.WriteLine(item.ToString());
        }
    }

    private static void PrintSortedByName(List<Item> items)
    {
        Sort(items, ShouldSwapByName);

        PrintList(items);
    }

    private static void PrintByTypeSortedByTotalPrice(List<Item> items, int type)
    {
        var filteredItems = new List<Item>();
        foreach (var item in items)
        {
            if (item.Type == type)
            {
                filteredItems.Add(item);
            }
        }

        Sort(filteredItems, ShouldSwapByTotalPrice);

        PrintList(filteredItems);
    }

    private static void PrintStatisticsForTypeFour(List<Item> items)
    {
        var filteredItems = new List<Item>();
        foreach (var item in items)
        {
            if (item.Type == 4)
            {
                filteredItems.Add(item);
            }
        }

        if (filteredItems.Count == 0)
        {
            Console.WriteLine("Няма артикули с тип 4.");

            return;
        }

        var combinedPrices = 0m;
        var totalAmount = 0m;

        foreach (var item in filteredItems)
        {
            combinedPrices += item.Price;
            totalAmount += item.Quantity * item.Price;
        }

        var avgPrice = combinedPrices / filteredItems.Count;

        Console.WriteLine($"Обща стойност на артикулите с тип 4: {totalAmount:F2}");
        Console.WriteLine($"Средна цена на артикулите с тип 4: {avgPrice:F2}");
    }
}