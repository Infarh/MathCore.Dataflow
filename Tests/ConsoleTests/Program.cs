using System;
using System.Threading.Tasks.Dataflow;

using ConsoleTests;


var pipe = DataFlow.PipeLine<Profile[]>(out var input)
   .When(pp => pp is { Length: > 0 })
   .SelectMany(pp => pp)
   .Select(async p => await ProfileImage.Load(p.ProfileImage));

Console.WriteLine();