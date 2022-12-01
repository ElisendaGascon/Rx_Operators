// See https://aka.ms/new-console-template for more information
using Rx_Operators;
using System.Reactive.Linq;

Console.WriteLine("Hello, World!");

IObservable<long> ticks = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1)).Take(5);

IObservable<long> filteredTicks = new WhereOperator<long>(ticks, value => value % 2 == 1);

IDisposable sub = filteredTicks.Subscribe(
    tickCount => Console.WriteLine(tickCount),
    () => Console.WriteLine("Done"));
Console.ReadLine();
sub.Dispose();
Console.WriteLine("Unsubscribed");
Console.ReadLine();
