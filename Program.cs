// See https://aka.ms/new-console-template for more information
using Rx_Operators;
using System.Reactive.Linq;

Console.WriteLine("Hello, World!");

int[] numbers = { 1, 2, 3, 4, 5, 6 };
var groupedNumbers = numbers.GroupBy(x => x % 3).ToArray();

IObservable<long> ticks = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1)).Take(5);

IObservable<long> filteredTicks = new WhereOperator<long>(ticks, value => value % 2 == 1);
IObservable<IGroupedObservable<long, long>> groupedTicks = new GroupByOperator<long, long>(ticks, value => value % 3);

/*IDisposable sub = filteredTicks.Subscribe(
    tickCount => Console.WriteLine(tickCount),
    () => Console.WriteLine("Done"));*/
IDisposable sub = groupedTicks.Subscribe(
    group =>
    {
        Console.WriteLine($"group {group.Key}");
        group.Subscribe(
            tickCount => Console.WriteLine($"Group {group.Key} produced {tickCount}"),
            () => Console.WriteLine($"Group {group.Key} ended"));
    });

Console.ReadLine();
sub.Dispose();
Console.WriteLine("Unsubscribed");
Console.ReadLine();
