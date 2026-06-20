using System.Globalization;

namespace Ksk2022;

internal enum ProductType
{
    G,
    S
}

internal class Product
{
    internal string Code { get; }

    internal string Name { get; }

    internal int QuantityInKg { get; }

    internal int ShelfLifeInDays { get; }

    internal ProductType Type { get; }

    internal DateTime EntryDate { get; }

    internal string StorageLocation { get; }

    internal decimal? StorageTemperature { get; }

    internal Product(
        string code,
        string name,
        int quantityInKg,
        int shelfLifeInDays,
        ProductType type,
        DateTime entryDate,
        string storageLocation,
        decimal? storageTemperature)
    {
        Code = code;
        Name = name;
        QuantityInKg = quantityInKg;
        ShelfLifeInDays = shelfLifeInDays;
        Type = type;
        EntryDate = entryDate;
        StorageLocation = storageLocation;
        StorageTemperature = storageTemperature;
    }
}

internal class Program
{
    private static void Main()
    {
        var productsCount = ReadIntInRange(
            "Въведете броя на изделията [0..1000]: ",
            0,
            1000);

        var products = InitializeProducts(productsCount);

        Console.WriteLine();
        Console.WriteLine("--- Всички изделия, подредени по позиция в склада ---");

        SortProductsByStorageLocation(products);
        PrintProducts(products);

        Console.WriteLine();
        Console.WriteLine("--- Специални изделия, подредени по дата и трайност ---");

        PrintSpecialProductsSortedByEntryDate(products);

        Console.WriteLine();

        var code = ReadStringWithMaxLength(
            "Въведете код на изделие за търсене: ",
            5);

        Console.WriteLine();
        Console.WriteLine("--- Резултат от търсенето ---");

        PrintProductsByCode(products, code);
    }

    private static List<Product> InitializeProducts(int productsCount)
    {
        var products = new List<Product>(productsCount);

        for (var i = 0; i < productsCount; i++)
        {
            Console.WriteLine();
            Console.WriteLine($"Изделие номер {i + 1}:");

            var code = ReadStringWithMaxLength(
                "Код на изделието (до 5 знака): ",
                5);

            var name = ReadStringWithMaxLength(
                "Име на изделието (до 60 знака): ",
                60);

            var quantityInKg = ReadPositiveInt(
                "Количество в килограми: ");

            var shelfLifeInDays = ReadPositiveInt(
                "Трайност в дни: ");

            var type = ReadValidProductType();

            var entryDate = ReadDate(
                "Дата на постъпване (ДД.ММ.ГГГГ): ");

            var storageLocation = ReadStringWithMaxLength(
                "Позиция в склада (до 10 знака): ",
                10);

            decimal? storageTemperature = null;

            if (type == ProductType.S)
            {
                storageTemperature = ReadDecimalWithMaxDecimalDigits(
                    "Температура на съхранение: ",
                    2);
            }

            var product = new Product(
                code,
                name,
                quantityInKg,
                shelfLifeInDays,
                type,
                entryDate,
                storageLocation,
                storageTemperature);

            products.Add(product);
        }

        return products;
    }

    private static void PrintSpecialProductsSortedByEntryDate(
        List<Product> products)
    {
        var specialProducts = new List<Product>();

        foreach (var product in products)
        {
            if (product.Type == ProductType.S)
            {
                specialProducts.Add(product);
            }
        }

        SortSpecialProductsByEntryDate(specialProducts);
        PrintProducts(specialProducts);
    }

    private static void PrintProductsByCode(
        List<Product> products,
        string code)
    {
        var filteredProducts = new List<Product>();
        var totalQuantityInKg = 0;
        decimal? minimumTemperature = null;

        foreach (var product in products)
        {
            if (!string.Equals(
                    product.Code,
                    code,
                    StringComparison.Ordinal))
            {
                continue;
            }

            filteredProducts.Add(product);
            totalQuantityInKg += product.QuantityInKg;

            if (product.Type == ProductType.S &&
                product.StorageTemperature.HasValue)
            {
                if (!minimumTemperature.HasValue ||
                    product.StorageTemperature.Value <
                    minimumTemperature.Value)
                {
                    minimumTemperature = product.StorageTemperature.Value;
                }
            }
        }

        if (filteredProducts.Count == 0)
        {
            Console.WriteLine("Няма налични изделия с този код.");

            return;
        }

        PrintProducts(filteredProducts);

        Console.WriteLine(
            $"Общо количество: {totalQuantityInKg} кг.");

        if (minimumTemperature.HasValue)
        {
            var formattedTemperature = minimumTemperature.Value
                .ToString("F1", CultureInfo.InvariantCulture);

            Console.WriteLine(
                $"Минимална температура на съхранение: " +
                $"{formattedTemperature} °C");
        }
    }

    private static void SortProductsByStorageLocation(
        List<Product> products)
    {
        for (var i = 0; i < products.Count - 1; i++)
        {
            var minIndex = i;

            for (var j = i + 1; j < products.Count; j++)
            {
                var comparison = string.Compare(
                    products[j].StorageLocation,
                    products[minIndex].StorageLocation,
                    StringComparison.Ordinal);

                if (comparison < 0)
                {
                    minIndex = j;
                }
            }

            if (minIndex != i)
            {
                Swap(products, i, minIndex);
            }
        }
    }

    private static void SortSpecialProductsByEntryDate(
        List<Product> products)
    {
        for (var i = 0; i < products.Count - 1; i++)
        {
            var bestIndex = i;

            for (var j = i + 1; j < products.Count; j++)
            {
                var currentProduct = products[j];
                var bestProduct = products[bestIndex];

                var shouldComeBefore =
                    currentProduct.EntryDate < bestProduct.EntryDate ||
                    (currentProduct.EntryDate == bestProduct.EntryDate &&
                     currentProduct.ShelfLifeInDays >
                     bestProduct.ShelfLifeInDays);

                if (shouldComeBefore)
                {
                    bestIndex = j;
                }
            }

            if (bestIndex != i)
            {
                Swap(products, i, bestIndex);
            }
        }
    }

    private static void Swap(
        List<Product> products,
        int firstIndex,
        int secondIndex) =>
        (products[firstIndex], products[secondIndex]) = (products[secondIndex], products[firstIndex]);

    private static void PrintProducts(List<Product> products)
    {
        if (products.Count == 0)
        {
            Console.WriteLine("Няма данни за извеждане.");

            return;
        }

        foreach (var product in products)
        {
            Console.WriteLine(ToDisplayString(product));
        }
    }

    private static int ReadIntInRange(
        string message,
        int min,
        int max)
    {
        while (true)
        {
            var input = ReadInput(message);

            var isValid = int.TryParse(
                input,
                out var result);

            if (isValid && result >= min && result <= max)
            {
                return result;
            }

            Console.WriteLine(
                $"Невалиден вход. Въведете цяло число от {min} до {max}.");
        }
    }

    private static int ReadPositiveInt(string message)
    {
        return ReadIntInRange(message, 1, int.MaxValue);
    }

    private static string ReadStringWithMaxLength(
        string message,
        int maxLength)
    {
        while (true)
        {
            var input = ReadInput(message);

            if (!string.IsNullOrWhiteSpace(input) &&
                input.Length <= maxLength)
            {
                return input;
            }

            Console.WriteLine(
                $"Въведете непразен текст с дължина до {maxLength} знака.");
        }
    }

    private static ProductType ReadValidProductType()
    {
        while (true)
        {
            var input = ReadInput(
                "Група стоки (G - нормално, S - специално): ");

            switch (input?.ToUpperInvariant())
            {
                case "G":
                    return ProductType.G;

                case "S":
                    return ProductType.S;

                default:
                    Console.WriteLine(
                        "Невалиден тип. Въведете G или S.");
                    break;
            }
        }
    }

    private static DateTime ReadDate(string message)
    {
        while (true)
        {
            var input = ReadInput(message);

            var isValid = DateTime.TryParseExact(
                input,
                "dd.MM.yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var result);

            if (isValid)
            {
                return result;
            }

            Console.WriteLine(
                "Невалидна дата. Пример: 25.03.2022");
        }
    }

    private static decimal ReadDecimalWithMaxDecimalDigits(
        string message,
        int maxDecimalDigits)
    {
        while (true)
        {
            var input = ReadInput(message);

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Въведете стойност.");

                continue;
            }

            var normalizedInput = input.Replace(',', '.');

            var hasValidDecimalDigits = HasValidDecimalDigits(
                normalizedInput,
                maxDecimalDigits);

            var isValid = decimal.TryParse(
                normalizedInput,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var result);

            if (isValid && hasValidDecimalDigits)
            {
                return result;
            }

            Console.WriteLine(
                $"Въведете реално число с до {maxDecimalDigits} знака след десетичния разделител.");
        }
    }

    private static bool HasValidDecimalDigits(
        string input,
        int maxDecimalDigits)
    {
        var decimalPointIndex = input.IndexOf('.');

        if (decimalPointIndex == -1)
        {
            return true;
        }

        var decimalDigitsCount = input.Length - decimalPointIndex - 1;

        return decimalDigitsCount <= maxDecimalDigits;
    }

    private static string ToDisplayString(Product product)
    {
        var result =
            $"{product.StorageLocation}, {product.Code}, {product.Name}, {product.QuantityInKg} кг., " +
            $"{product.EntryDate:dd.MM.yyyy}, {product.ShelfLifeInDays}";

        if (product is { Type: ProductType.S, StorageTemperature: not null })
        {
            var temperature = product.StorageTemperature.Value
                .ToString("F1", CultureInfo.InvariantCulture);

            result += $", tC={product.StorageTemperature}";
        }

        return result;
    }

    private static string ReadInput(string message)
    {
        Console.Write(message);

        return Console.ReadLine()?.Trim() ?? string.Empty;
    }
}