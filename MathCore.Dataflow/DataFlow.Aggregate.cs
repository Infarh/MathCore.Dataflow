using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Threading.Tasks.Dataflow;

public static partial class DataFlow
{
    /// <summary>Объединение элементов потока в пакеты указанного размера</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <param name="source">Поток-источник данных</param>
    /// <param name="BatchSize">Размер пакета</param>
    /// <returns>Связь с созданным новым потоком-агрегатором</returns>
    public static Link<T, BatchBlock<T>> Aggregate<T>(this ISourceBlock<T> source, int BatchSize)
    {
        var transformer = new BatchBlock<T>(BatchSize);
        var unsubscriber = source.LinkTo(transformer);
        return new(unsubscriber, transformer);
    }

    /// <summary>Объединение элементов потока в пакеты указанного размера</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <typeparam name="TSourceBlock">Тип потока-источника</typeparam>
    /// <param name="source">Поток-источник данных</param>
    /// <param name="BatchSize">Размер пакета</param>
    /// <returns>Связь с созданным новым потоком-агрегатором</returns>
    public static Link<T, BatchBlock<T>> Aggregate<T, TSourceBlock>(
        this Link<T, TSourceBlock> source,
        int BatchSize)
        where TSourceBlock : ISourceBlock<T> =>
        source.Result.Aggregate(BatchSize);

    /// <summary>Объединение элементов потока в пакеты указанного размера</summary>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <param name="source">Поток-источник данных</param>
    /// <param name="BatchSize">Размер пакета</param>
    /// <returns>Связь с созданным новым потоком-агрегатором</returns>
    public static Link<Tuple<T1, T2>, BatchBlock<Tuple<T1, T2>>> Aggregate<T1, T2>(
        this JoinBlockLink<T1, T2> source,
        int BatchSize) =>
        source.Joiner.Aggregate(BatchSize);

    /// <summary>Объединение элементов потока в пакеты указанного размера</summary>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <param name="source">Поток-источник данных</param>
    /// <param name="BatchSize">Размер пакета</param>
    /// <returns>Связь с созданным новым потоком-агрегатором</returns>
    public static Link<Tuple<IList<T1>, IList<T2>>, BatchBlock<Tuple<IList<T1>, IList<T2>>>> Aggregate<T1, T2>(
        this BatchedJoinBlockLink<T1, T2> source,
        int BatchSize) =>
        source.Joiner.Aggregate(BatchSize);
}
