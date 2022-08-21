using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Threading.Tasks.Dataflow;

public static partial class DataFlow
{
    /// <summary>Передача элемента из потока-источника в новый буферизованный поток-приёмник при выполнении условия</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <param name="source">Поток-источник элементов</param>
    /// <param name="predicate">Условие, при выполнении которого элемент потока-источника попадёт в выходной поток</param>
    /// <returns>Связь с созданным новым буферизованным потоком, элементы которого будут отфильтрованными из исходного потока указанным способом</returns>
    public static Link<T, ISourceBlock<T>> When<T>(this ISourceBlock<T> source, Predicate<T> predicate)
    {
        if (source is not BroadcastBlock<T>) 
            return source.Braodcast().When(predicate);

        var result = new BufferBlock<T>();
        var unsubscriber = source.LinkTo(result, new DataflowLinkOptions { PropagateCompletion = true }, predicate);
        return new(unsubscriber, result);
    }

    /// <summary>Передача элемента из потока-источника в новый буферизованный поток-приёмник при выполнении условия</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <typeparam name="TSourceBlock">Тип потока-источника</typeparam>
    /// <param name="source">Поток-источник элементов</param>
    /// <param name="predicate">Условие, при выполнении которого элемент потока-источника попадёт в выходной поток</param>
    /// <returns>Связь с созданным новым буферизованным потоком, элементы которого будут отфильтрованными из исходного потока указанным способом</returns>
    public static Link<T, ISourceBlock<T>> When<T, TSourceBlock>(
        this Link<T, TSourceBlock> source,
        Predicate<T> predicate)
        where TSourceBlock : ISourceBlock<T> =>
        source.Result.When(predicate);

    /// <summary>Передача элемента из потока-источника в новый буферизованный поток-приёмник при выполнении условия</summary>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <param name="source">Поток-источник элементов</param>
    /// <param name="predicate">Условие, при выполнении которого элемент потока-источника попадёт в выходной поток</param>
    /// <returns>Связь с созданным новым буферизованным потоком, элементы которого будут отфильтрованными из исходного потока указанным способом</returns>
    public static Link<Tuple<T1, T2>, ISourceBlock<Tuple<T1, T2>>> When<T1, T2>(
        this JoinBlockLink<T1, T2> source,
        Predicate<Tuple<T1, T2>> predicate) =>
        source.Joiner.When(predicate);

    /// <summary>Передача элемента из потока-источника в новый буферизованный поток-приёмник при выполнении условия</summary>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <param name="source">Поток-источник элементов</param>
    /// <param name="predicate">Условие, при выполнении которого элемент потока-источника попадёт в выходной поток</param>
    /// <returns>Связь с созданным новым буферизованным потоком, элементы которого будут отфильтрованными из исходного потока указанным способом</returns>
    public static Link<Tuple<IList<T1>, IList<T2>>, ISourceBlock<Tuple<IList<T1>, IList<T2>>>> When<T1, T2>(
        this BatchedJoinBlockLink<T1, T2> source,
        Predicate<Tuple<IList<T1>, IList<T2>>> predicate) =>
        source.Joiner.When(predicate);
}
