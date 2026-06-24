namespace Ksk2019_2;

public enum StarClassification
{
    Hypergiant = 1,
    Supergiant = 2,
    BrightGiant = 3,
    Giant = 4,
    Subgiant = 5,
    Dwarf = 6,
    Subdwarf = 7,
    RedDwarf = 8,
    BrownDwarf = 9
}

public class Star(
    string designation,
    decimal distanceLightYears,
    StarClassification classification,
    decimal solarMass,
    string constellationName)
{
    public string Designation { get; set; } = designation;

    public decimal DistanceLightYears { get; set; } = distanceLightYears;

    public StarClassification Classification { get; set; } = classification;

    public decimal SolarMass { get; set; } = solarMass;

    public string ConstellationName { get; set; } = constellationName;

    public override string ToString()
    {
        var classificationString = Classification switch
        {
            StarClassification.Hypergiant => "хипергигант",
            StarClassification.Supergiant => "свръхгигант",
            StarClassification.BrightGiant => "ярък гигант",
            StarClassification.Giant => "гигант",
            StarClassification.Subgiant => "субгигант",
            StarClassification.Dwarf => "джудже",
            StarClassification.Subdwarf => "субджудже",
            StarClassification.RedDwarf => "червено джудже",
            StarClassification.BrownDwarf => "кафяво джудже",
            _ => "неизвестна класификация"
        };

        return
            $"{Designation}, {DistanceLightYears} св.г., {classificationString}, {SolarMass} сл.м., {ConstellationName}";
    }
}

public class Program
{
    public static void Main()
    {
        var stars = new List<Star>();

        while (true)
        {
            Console.WriteLine("1. Въведи звезди");
            Console.WriteLine("2. Всички звезди сортирани по разстояние");
            Console.WriteLine("3. Всички звезди сортирани по съзвездие");
            Console.WriteLine("4. Създездия със средна маса");
            Console.WriteLine("0. Изход");

            var choice = ReadInt(0, 4, "Избор: ");

            switch (choice)
            {
                case 1:
                    var count = ReadInt(0, 2000, "Брой звезди: ");
                    if (stars.Count + count > 2000)
                    {
                        Console.WriteLine("Превишавате максималния брой звезди (2000).");
                        break;
                    }

                    var starsToInclude = BuildStars(count);
                    stars.AddRange(starsToInclude);
                    break;

                case 2:
                    Sort(stars, ShouldSwapByDistance);
                    PrintStarsList(stars);
                    break;

                case 3:
                    Sort(stars, ShouldSwapByConstellationAndMass);
                    PrintStarsList(stars);
                    break;

                case 4:
                    PrintAvgMassByConstellation(stars);
                    break;

                case 0:
                    return;
            }
        }
    }

    private static void PrintAvgMassByConstellation(List<Star> stars)
    {
        var constellations = new List<string>();
        foreach (var star in stars)
        {
            if (!constellations.Contains(star.ConstellationName))
            {
                constellations.Add(star.ConstellationName);
            }
        }

        foreach (var constellation in constellations)
        {
            var numberOfStars = 0;
            var totalMass = 0m;
            foreach (var star in stars)
            {
                if (star.ConstellationName == constellation)
                {
                    numberOfStars++;
                    totalMass += star.SolarMass;
                }
            }

            Console.WriteLine($"Съзвездие: {constellation}, Средна маса: {totalMass / numberOfStars} сл.м.");
        }
    }

    private static bool ShouldSwapByConstellationAndMass(Star first, Star second)
    {
        var constellationComparison = string.Compare(
            first.ConstellationName,
            second.ConstellationName,
            StringComparison.CurrentCultureIgnoreCase);

        if (constellationComparison > 0)
        {
            return true;
        }

        if (constellationComparison < 0)
        {
            return false;
        }

        return first.SolarMass < second.SolarMass;
    }

    private static void Sort<T>(List<T> list, Func<T, T, bool> shouldSwap)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            for (int j = i + 1; j < list.Count; j++)
            {
                if (shouldSwap(list[i], list[j]))
                {
                    (list[i], list[j]) = (list[j], list[i]);
                }
            }
        }
    }

    private static bool ShouldSwapByDistance(Star star1, Star star2) =>
        star1.DistanceLightYears > star2.DistanceLightYears;

    private static bool ShouldSwapByConstellation(Star star1, Star star2) =>
        string.Compare(star1.ConstellationName, star2.ConstellationName, StringComparison.Ordinal) > 0;

    private static List<Star> BuildStars(int count)
    {
        var stars = new List<Star>();
        for (int i = 0; i < count; i++)
        {
            var designation = ReadStringInput(20, $"Въведете обозначение на звезда {i + 1}: ");
            var distanceLightYears = ReadPositiveDecimal($"Въведете разстояние в светлинни години на звезда {i + 1}: ");
            var classification = ReadStarClassification();
            var solarMass = ReadPositiveDecimal($"Въведете маса в слънчеви маси на звезда {i + 1}: ");
            var constellationName = ReadStringInput(30, $"Въведете име на съзвездие на звезда {i + 1}: ");

            var star = new Star(designation, distanceLightYears, classification, solarMass, constellationName);
            stars.Add(star);
        }

        return stars;
    }

    private static void PrintStarsList(List<Star> stars)
    {
        foreach (var star in stars)
        {
            Console.WriteLine(star.ToString());
        }
    }

    private static StarClassification ReadStarClassification()
    {
        while (true)
        {
            var classificationNumber = ReadInt(1, 9, "Въведете класификация на звезда (1-9): ");

            return (StarClassification)classificationNumber;
        }
    }

    private static string ReadStringInput(int maxLength, string message)
    {
        Console.Write(message);

        while (true)
        {
            var input = Console.ReadLine()?.Trim();

            if (input is not null && input.Length <= maxLength)
            {
                return input;
            }

            Console.WriteLine("Невалиден вход.");
        }
    }

    private static decimal ReadPositiveDecimal(string message)
    {
        Console.Write(message);

        while (true)
        {
            var input = Console.ReadLine()?.Trim();
            var isValid = decimal.TryParse(input, out var number) && number > 0;

            if (isValid)
            {
                return number;
            }

            Console.WriteLine("Моля въведете положително число.");
        }
    }

    private static int ReadInt(int min, int max, string message)
    {
        Console.Write(message);

        while (true)
        {
            var input = Console.ReadLine();
            var isValid = int.TryParse(input, out int result) && result >= min && result <= max;

            if (isValid)
            {
                return result;
            }

            Console.WriteLine("Невалиден вход.");
        }
    }
}