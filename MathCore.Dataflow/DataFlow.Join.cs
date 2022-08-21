using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Threading.Tasks.Dataflow;

public static partial class DataFlow
{
    /// <summary>Объединение элементов из двух потоков в один поток кортежей</summary>
    /// <typeparam name="T1">Тип элементов первого потока-источника</typeparam>
    /// <typeparam name="T2">Тип элементов второго потока-источника</typeparam>
    /// <param name="source">Поток-источник первых элементов кортежей</param>
    /// <param name="Input">Новый созданный поток-источник вторых элементов кортежей</param>
    /// <param name="Greedy">Жадный поток</param>
    /// <returns>Связь, созданная с новым потоком-объединителем элементов</returns>
    public static JoinBlockLink<T1, T2> Join<T1, T2>(
        this ISourceBlock<T1> source,
        out BufferBlock<T2> Input,
        bool Greedy = false)
    {
        Input = new BufferBlock<T2>();
        var join = new JoinBlock<T1, T2>(new GroupingDataflowBlockOptions { Greedy = Greedy });
        var source_unsubscriber = source.LinkTo(join.Target1, new DataflowLinkOptions { PropagateCompletion = true });
        var input_unsubscriber = Input.LinkTo(join.Target2, new DataflowLinkOptions { PropagateCompletion = true });
        return new(source_unsubscriber, input_unsubscriber, join);
    }
}
