using System;
using System.Threading.Tasks.Dataflow;

using ConsoleTests;


var pipe = DataFlow.PipeLine<Profile[]>();
var link1 = pipe.Select(p => p.Length);

Console.WriteLine();