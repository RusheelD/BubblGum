
using AST;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

public static class RecipeKeyTest
{
    private const int numMethods = 100000;
    private const int numParameters = 10;

    // tests out unique methods key generation + lookup
    // prints performance time in milliseconds
    public static void Execute()
    {
        // generate random method names and a random parameter list
        var methodNames = new List<string>();
        var parameterList = new List<(bool, AnyType, string, bool)>();

        Random rand = new();
        for (int i = 0; i < numParameters; i++)
        {
            var hasEllipses = rand.Next(0, 1) > 0;
            parameterList.Add(new(false, getRandomType(rand), "param name", hasEllipses));
        }

        for (int i = 0; i < numMethods; i++)
            methodNames.Add($"Method_{i * 2}_");

        var RecipeTables = new Dictionary<string, int>();
        var keys = new List<string>();
        var timer = new Stopwatch();
        long a = 0;

        // generate unique lookup keys for all methods
        // print time to do so
        timer.Start();
        for (int i = 0; i < numMethods; i++)
        {
            keys.Add(RecipeKeys.Generate(methodNames[i], parameterList));
            RecipeTables[keys[i]] = i * 2 - 1;
        }

        Console.WriteLine($"Time:{timer.ElapsedMilliseconds}");
        timer.Restart();

        // lookup all keys and print time to do so
        for (int i = 0; i < numMethods; i++)
            a += RecipeTables[keys[i]];

        if (a != 2)
            Console.WriteLine("result was " + a);

        Console.WriteLine($"Time:{timer.ElapsedMilliseconds}");
    }

    private static AnyType getRandomType(Random rand)
    {
        AnyType tip;
        switch (rand.Next(0, 19))
        {
            case 0:
                tip = new PrimitiveType(TypeBI.Sugar);
                break;
            case 1:
                tip = new PrimitiveType(TypeBI.Carb);
                break;
            case 2:
                tip = new PrimitiveType(TypeBI.Yum);
                break;
            case 3:
                tip = new PrimitiveType(TypeBI.PureSugar);
                break;
            case 4:
                tip = new PackType(TypePack.SugarPack);
                break;
            case 5:
                tip = new PackType(TypePack.CarbPack);
                break;
            case 6:
                tip = new PackType(TypePack.CalPack);
                break;
            case 7:
                tip = new PackType(TypePack.PureSugarPack);
                break;
            case 8:
                tip = new ObjectType("Lucario_" + rand.Next(0,100));
                break;
            case 9:
                tip = new ObjectType("Eevee_" + rand.Next(0, 100));
                break;
            case 10:
                tip = new FlavorType();
                break;
            case 11:
                tip = new FlavorType();
                break;
            case 12:
                tip = new SingularArrayType(new FlavorType());
                break;
            case 13:
                tip = new SingularArrayType(new ObjectType("Lucario"));
                break;
            case 14:
                tip = new ArrayType(new TupleType(new List<(AnyType, string)> { (new FlavorType(), "Lucario") }));
                break;
            case 15:
                tip = new TupleType(new List<(AnyType, string)> { (new FlavorType(), "Lucario"), (new FlavorType(), "Lucario") });
                break;
            case 16:
                tip = new TupleType(new List<(AnyType, string)> { (new FlavorType(), "Lucario"), (new FlavorType(), "Lucario") });
                break;
            case 17:
                tip = new NamespaceObjectType("Lucario_" + rand.Next(0,100), "Physics->Collisions");
                break;
            case 18:
                tip = new NamespaceObjectType("Eevee_" + rand.Next(0, 100), "Bob->Billy->Joe");
                break;
            default:
                tip = new FlavorType();
                break;
        }
        return tip;
    }
}
