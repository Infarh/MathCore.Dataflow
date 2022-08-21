using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Threading.Tasks.Dataflow;

public static partial class DataFlow
{
    /// <summary>Преобразование элемента входного потока в набор элементов выходного потока</summary>
    /// <typeparam name="TSource">Тип элементов входного потока</typeparam>
    /// <typeparam name="TResult">Тип элементов выходного потока</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="selector">Метод формирования набора элементов на основе каждого из элементов входного потока</param>
    /// <returns>Связь с созданным преобразователем</returns>
    public static Link<TResult, TransformManyBlock<TSource, TResult>> SelectMany<TSource, TResult>(
        this ISourceBlock<TSource> source,
        Func<TSource, Task<IEnumerable<TResult>>> selector)
    {
        var transformer = new TransformManyBlock<TSource, TResult>(selector);
        var unsubscriber = source.LinkTo(transformer);
        return new(unsubscriber, transformer);
    }

    /// <summary>Преобразование элемента входного потока в набор элементов выходного потока</summary>
    /// <typeparam name="TSource">Тип элементов входного потока</typeparam>
    /// <typeparam name="TResult">Тип элементов выходного потока</typeparam>
    /// <typeparam name="TSourceBlock">Тип потока-источника</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="selector">Метод формирования набора элементов на основе каждого из элементов входного потока</param>
    /// <returns>Связь с созданным преобразователем</returns>
    public static Link<TResult, TransformManyBlock<TSource, TResult>> SelectMany<TSource, TSourceBlock, TResult>(
        this Link<TSource, TSourceBlock> source,
        Func<TSource, IEnumerable<TResult>> selector)
        where TSourceBlock : ISourceBlock<TSource> =>
        source.Result.SelectMany(selector);

    /// <summary>Преобразование элемента входного потока в набор элементов выходного потока</summary>
    /// <typeparam name="TSource">Тип элементов входного потока</typeparam>
    /// <typeparam name="TResult">Тип элементов выходного потока</typeparam>
    /// <typeparam name="TSourceBlock">Тип потока-источника</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="selector">Метод формирования набора элементов на основе каждого из элементов входного потока</param>
    /// <returns>Связь с созданным преобразователем</returns>
    public static Link<TResult, TransformManyBlock<TSource, TResult>> SelectMany<TSource, TSourceBlock, TResult>(
        this Link<TSource, TSourceBlock> source,
        Func<TSource, Task<IEnumerable<TResult>>> selector)
        where TSourceBlock : ISourceBlock<TSource> =>
        source.Result.SelectMany(selector);

    /// <summary>Преобразование элемента входного потока в набор элементов выходного потока</summary>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <typeparam name="TResult">Тип элементов выходного потока</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="selector">Метод формирования набора элементов на основе каждого из элементов входного потока</param>
    /// <returns>Связь с созданным преобразователем</returns>
    public static Link<TResult, TransformManyBlock<Tuple<T1, T2>, TResult>> SelectMany<T1, T2, TResult>(
        this JoinBlockLink<T1, T2> source,
        Func<Tuple<T1, T2>, IEnumerable<TResult>> selector)
    {
        var transformer = new TransformManyBlock<Tuple<T1, T2>, TResult>(selector);
        var unsubscriber = source.Joiner.LinkTo(transformer);
        return new(unsubscriber, transformer);
    }

    /// <summary>Преобразование элемента входного потока в набор элементов выходного потока</summary>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <typeparam name="TResult">Тип элементов выходного потока</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="selector">Метод формирования набора элементов на основе каждого из элементов входного потока</param>
    /// <returns>Связь с созданным преобразователем</returns>
    public static Link<TResult, TransformManyBlock<Tuple<T1, T2>, TResult>> SelectMany<T1, T2, TResult>(
        this JoinBlockLink<T1, T2> source,
        Func<Tuple<T1, T2>, Task<IEnumerable<TResult>>> selector)
    {
        var transformer = new TransformManyBlock<Tuple<T1, T2>, TResult>(selector);
        var unsubscriber = source.Joiner.LinkTo(transformer);
        return new(unsubscriber, transformer);
    }

    /// <summary>Преобразование элемента входного потока в набор элементов выходного потока</summary>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <typeparam name="TResult">Тип элементов выходного потока</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="selector">Метод формирования набора элементов на основе каждого из элементов входного потока</param>
    /// <returns>Связь с созданным преобразователем</returns>
    public static Link<TResult, TransformManyBlock<Tuple<IList<T1>, IList<T2>>, TResult>> SelectMany<T1, T2, TResult>(
        this BatchedJoinBlockLink<T1, T2> source,
        Func<Tuple<IList<T1>, IList<T2>>, IEnumerable<TResult>> selector)
    {
        var transformer = new TransformManyBlock<Tuple<IList<T1>, IList<T2>>, TResult>(selector);
        var unsubscriber = source.Joiner.LinkTo(transformer);
        return new(unsubscriber, transformer);
    }

    /// <summary>Преобразование элемента входного потока в набор элементов выходного потока</summary>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <typeparam name="TResult">Тип элементов выходного потока</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="selector">Метод формирования набора элементов на основе каждого из элементов входного потока</param>
    /// <returns>Связь с созданным преобразователем</returns>
    public static Link<TResult, TransformManyBlock<Tuple<IList<T1>, IList<T2>>, TResult>> SelectMany<T1, T2, TResult>(
        this BatchedJoinBlockLink<T1, T2> source,
        Func<Tuple<IList<T1>, IList<T2>>, Task<IEnumerable<TResult>>> selector)
    {
        var transformer = new TransformManyBlock<Tuple<IList<T1>, IList<T2>>, TResult>(selector);
        var unsubscriber = source.Joiner.LinkTo(transformer);
        return new(unsubscriber, transformer);
    }

    /// <summary>Преобразование элемента входного потока в набор элементов выходного потока</summary>
    /// <typeparam name="TSource">Тип элементов входного потока</typeparam>
    /// <typeparam name="TResult">Тип элементов выходного потока</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="selector">Метод формирования набора элементов на основе каждого из элементов входного потока</param>
    /// <param name="TransformCompletion">Задача, завершаемая при завершении обработки данных в потоке-результата</param>
    /// <returns>Связь с созданным преобразователем</returns>
    public static Link<TSource, TransformManyBlock<TSource, TResult>> SelectMany<TSource, TResult>(
        this ISourceBlock<TSource> source,
        Func<TSource, IEnumerable<TResult>> selector,
        out Task TransformCompletion)
    {
        var transformer = new TransformManyBlock<TSource, TResult>(selector);
        var unsubscriber = source.LinkTo(transformer);
        TransformCompletion = transformer.Completion;
        return new(unsubscriber, transformer);
    }

    /// <summary>Преобразование элемента входного потока в набор элементов выходного потока</summary>
    /// <typeparam name="TSource">Тип элементов входного потока</typeparam>
    /// <typeparam name="TResult">Тип элементов выходного потока</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="selector">Метод формирования набора элементов на основе каждого из элементов входного потока</param>
    /// <param name="TransformCompletion">Задача, завершаемая при завершении обработки данных в потоке-результата</param>
    /// <returns>Связь с созданным преобразователем</returns>
    public static Link<TSource, TransformManyBlock<TSource, TResult>> SelectMany<TSource, TResult>(
        this ISourceBlock<TSource> source,
        Func<TSource, Task<IEnumerable<TResult>>> selector,
        out Task TransformCompletion)
    {
        var transformer = new TransformManyBlock<TSource, TResult>(selector);
        var unsubscriber = source.LinkTo(transformer);
        TransformCompletion = transformer.Completion;
        return new(unsubscriber, transformer);
    }

    /// <summary>Преобразование элемента входного потока в набор элементов выходного потока</summary>
    /// <typeparam name="TSource">Тип элементов входного потока</typeparam>
    /// <typeparam name="TResult">Тип элементов выходного потока</typeparam>
    /// <typeparam name="TSourceBlock">Тип потока-источника</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="selector">Метод формирования набора элементов на основе каждого из элементов входного потока</param>
    /// <param name="TransformCompletion">Задача, завершаемая при завершении обработки данных в потоке-результата</param>
    /// <returns>Связь с созданным преобразователем</returns>
    public static Link<TSource, TransformManyBlock<TSource, TResult>> SelectMany<TSource, TSourceBlock, TResult>(
        this Link<TSource, TSourceBlock> source,
        Func<TSource, IEnumerable<TResult>> selector,
        out Task TransformCompletion)
        where TSourceBlock : ISourceBlock<TSource> =>
        source.Result.SelectMany(selector, out TransformCompletion);

    /// <summary>Преобразование элемента входного потока в набор элементов выходного потока</summary>
    /// <typeparam name="TSource">Тип элементов входного потока</typeparam>
    /// <typeparam name="TResult">Тип элементов выходного потока</typeparam>
    /// <typeparam name="TSourceBlock">Тип потока-источника</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="selector">Метод формирования набора элементов на основе каждого из элементов входного потока</param>
    /// <param name="TransformCompletion">Задача, завершаемая при завершении обработки данных в потоке-результата</param>
    /// <returns>Связь с созданным преобразователем</returns>
    public static Link<TSource, TransformManyBlock<TSource, TResult>> SelectMany<TSource, TSourceBlock, TResult>(
        this Link<TSource, TSourceBlock> source,
        Func<TSource, Task<IEnumerable<TResult>>> selector,
        out Task TransformCompletion)
        where TSourceBlock : ISourceBlock<TSource> =>
        source.Result.SelectMany(selector, out TransformCompletion);

    /// <summary>Преобразование элемента входного потока в набор элементов выходного потока</summary>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <typeparam name="TResult">Тип элементов выходного потока</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="selector">Метод формирования набора элементов на основе каждого из элементов входного потока</param>
    /// <param name="TransformCompletion">Задача, завершаемая при завершении обработки данных в потоке-результата</param>
    /// <returns>Связь с созданным преобразователем</returns>
    public static Link<Tuple<T1, T2>, TransformManyBlock<Tuple<T1, T2>, TResult>> SelectMany<T1, T2, TResult>(
        this JoinBlockLink<T1, T2> source,
        Func<Tuple<T1, T2>, IEnumerable<TResult>> selector,
        out Task TransformCompletion) =>
        source.Joiner.SelectMany(selector, out TransformCompletion);

    /// <summary>Преобразование элемента входного потока в набор элементов выходного потока</summary>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <typeparam name="TResult">Тип элементов выходного потока</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="selector">Метод формирования набора элементов на основе каждого из элементов входного потока</param>
    /// <param name="TransformCompletion">Задача, завершаемая при завершении обработки данных в потоке-результата</param>
    /// <returns>Связь с созданным преобразователем</returns>
    public static Link<Tuple<T1, T2>, TransformManyBlock<Tuple<T1, T2>, TResult>> SelectMany<T1, T2, TResult>(
        this JoinBlockLink<T1, T2> source,
        Func<Tuple<T1, T2>, Task<IEnumerable<TResult>>> selector,
        out Task TransformCompletion) =>
        source.Joiner.SelectMany(selector, out TransformCompletion);

    /// <summary>Преобразование элемента входного потока в набор элементов выходного потока</summary>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <typeparam name="TResult">Тип элементов выходного потока</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="selector">Метод формирования набора элементов на основе каждого из элементов входного потока</param>
    /// <param name="TransformCompletion">Задача, завершаемая при завершении обработки данных в потоке-результата</param>
    /// <returns>Связь с созданным преобразователем</returns>
    public static Link<Tuple<IList<T1>, IList<T2>>, TransformManyBlock<Tuple<IList<T1>, IList<T2>>, TResult>> SelectMany<T1, T2, TResult>(
        this BatchedJoinBlockLink<T1, T2> source,
        Func<Tuple<IList<T1>, IList<T2>>, IEnumerable<TResult>> selector,
        out Task TransformCompletion) =>
        source.Joiner.SelectMany(selector, out TransformCompletion);

    /// <summary>Преобразование элемента входного потока в набор элементов выходного потока</summary>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <typeparam name="TResult">Тип элементов выходного потока</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="selector">Метод формирования набора элементов на основе каждого из элементов входного потока</param>
    /// <param name="TransformCompletion">Задача, завершаемая при завершении обработки данных в потоке-результата</param>
    /// <returns>Связь с созданным преобразователем</returns>
    public static Link<Tuple<IList<T1>, IList<T2>>, TransformManyBlock<Tuple<IList<T1>, IList<T2>>, TResult>> SelectMany<T1, T2, TResult>(
        this BatchedJoinBlockLink<T1, T2> source,
        Func<Tuple<IList<T1>, IList<T2>>, Task<IEnumerable<TResult>>> selector,
        out Task TransformCompletion) =>
        source.Joiner.SelectMany(selector, out TransformCompletion);
}
