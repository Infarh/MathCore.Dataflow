using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Threading.Tasks.Dataflow;

public static partial class DataFlow
{
    /// <summary>Выполнение указанного действия для всех элементов потока</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <param name="source">Поток-источник элементов над которыми требуется выполнить действие</param>
    /// <param name="action">Выполняемое действие</param>
    /// <returns>Связь с вновь созданным обработчиком элементов потока</returns>
    public static Link<T, ActionBlock<T>> ForEach<T>(this ISourceBlock<T> source, Action<T> action)
    {
        var action_block = new ActionBlock<T>(action);
        var unsubscriber = source.LinkTo(action_block);
        return new(unsubscriber, action_block);
    }

    /// <summary>Выполнение указанного действия для всех элементов потока</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <param name="source">Поток-источник элементов над которыми требуется выполнить действие</param>
    /// <param name="action">Выполняемое действие</param>
    /// <returns>Связь с вновь созданным обработчиком элементов потока</returns>
    public static Link<T, ActionBlock<T>> ForEach<T>(this ISourceBlock<T> source, Func<T, Task> action)
    {
        var action_block = new ActionBlock<T>(action);
        var unsubscriber = source.LinkTo(action_block);
        return new(unsubscriber, action_block);
    }

    /// <summary>Выполнение указанного действия для всех элементов потока</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <typeparam name="TSourceBlock">Тип потока-источника</typeparam>
    /// <param name="source">Поток-источник элементов над которыми требуется выполнить действие</param>
    /// <param name="action">Выполняемое действие</param>
    /// <returns>Связь с вновь созданным обработчиком элементов потока</returns>
    public static Link<T, ActionBlock<T>> ForEach<T, TSourceBlock>(
        this Link<T, TSourceBlock> source,
        Action<T> action)
        where TSourceBlock : ISourceBlock<T> =>
        source.Result.ForEach(action);

    /// <summary>Выполнение указанного действия для всех элементов потока</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <typeparam name="TSourceBlock">Тип потока-источника</typeparam>
    /// <param name="source">Поток-источник элементов над которыми требуется выполнить действие</param>
    /// <param name="action">Выполняемое действие</param>
    /// <returns>Связь с вновь созданным обработчиком элементов потока</returns>
    public static Link<T, ActionBlock<T>> ForEach<T, TSourceBlock>(
        this Link<T, TSourceBlock> source,
        Func<T, Task> action)
        where TSourceBlock : ISourceBlock<T> =>
        source.Result.ForEach(action);

    /// <summary>Выполнение указанного действия для всех элементов потока</summary>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <param name="source">Поток-источник элементов над которыми требуется выполнить действие</param>
    /// <param name="action">Выполняемое действие</param>
    /// <returns>Связь с вновь созданным обработчиком элементов потока</returns>
    public static Link<Tuple<T1, T2>, ActionBlock<Tuple<T1, T2>>> ForEach<T1, T2>(
        this JoinBlockLink<T1, T2> source,
        Action<Tuple<T1, T2>> action) =>
        source.Joiner.ForEach(action);

    /// <summary>Выполнение указанного действия для всех элементов потока</summary>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <param name="source">Поток-источник элементов над которыми требуется выполнить действие</param>
    /// <param name="action">Выполняемое действие</param>
    /// <returns>Связь с вновь созданным обработчиком элементов потока</returns>
    public static Link<Tuple<T1, T2>, ActionBlock<Tuple<T1, T2>>> ForEach<T1, T2>(
        this JoinBlockLink<T1, T2> source,
        Func<Tuple<T1, T2>, Task> action) =>
        source.Joiner.ForEach(action);

    /// <summary>Выполнение указанного действия для всех элементов потока</summary>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <param name="source">Поток-источник элементов над которыми требуется выполнить действие</param>
    /// <param name="action">Выполняемое действие</param>
    /// <returns>Связь с вновь созданным обработчиком элементов потока</returns>
    public static Link<Tuple<IList<T1>, IList<T2>>, ActionBlock<Tuple<IList<T1>, IList<T2>>>> ForEach<T1, T2>(
        this BatchedJoinBlockLink<T1, T2> source,
        Action<Tuple<IList<T1>, IList<T2>>> action) =>
        source.Joiner.ForEach(action);

    /// <summary>Выполнение указанного действия для всех элементов потока</summary>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <param name="source">Поток-источник элементов над которыми требуется выполнить действие</param>
    /// <param name="action">Выполняемое действие</param>
    /// <returns>Связь с вновь созданным обработчиком элементов потока</returns>
    public static Link<Tuple<IList<T1>, IList<T2>>, ActionBlock<Tuple<IList<T1>, IList<T2>>>> ForEach<T1, T2>(
        this BatchedJoinBlockLink<T1, T2> source,
        Func<Tuple<IList<T1>, IList<T2>>, Task> action) =>
        source.Joiner.ForEach(action);
}
