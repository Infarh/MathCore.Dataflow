
var strings = DataFlow.PipeLine<string?>(out var input)
   .When(s => s is { Length: > 0 })
   .Braodcast();

strings.ForEach(s => Console.WriteLine("str = {0}", s ?? "<null>"));
strings.SelectMany(s => s!.Split(' ')
   .Select(v => (s, v)))
   .ForEach(s => Console.WriteLine("\tvalue = {0}", s));

input.Post("Str 123 321");
input.Post("");
input.Post(null);
input.Post("qwe ewq");
input.Post("qwe1 333");
input.Post("qwe2 333 44 555 666");
input.Post("qwe3 123 321 123");
input.Complete();


Console.WriteLine("Complete");
Console.ReadLine();
Console.WriteLine("End");
