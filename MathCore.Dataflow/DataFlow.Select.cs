using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Threading.Tasks.Dataflow;

public static partial class DataFlow
{
    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="TSource">Тип элементов потока-источника</typeparam>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<TResult, TransformBlock<TSource, TResult>> Select<TSource, TResult>(
        this ISourceBlock<TSource> source,
        Func<TSource, TResult> transform)
    {
        var transformer = new TransformBlock<TSource, TResult>(transform);
        var unsubscriber = source.LinkTo(transformer);
        return new(unsubscriber, transformer);
    }

    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="TSource">Тип элементов потока-источника</typeparam>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<TResult, TransformBlock<TSource, TResult>> Select<TSource, TResult>(
        this ISourceBlock<TSource> source,
        Func<TSource, Task<TResult>> transform)
    {
        var transformer = new TransformBlock<TSource, TResult>(transform);
        var unsubscriber = source.LinkTo(transformer);
        return new(unsubscriber, transformer);
    }

    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="TSource">Тип элементов потока-источника</typeparam>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <typeparam name="TSourceBlock">Тип потока-источника</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<TResult, TransformBlock<TSource, TResult>> Select<TSource, TSourceBlock, TResult>(
        this Link<TSource, TSourceBlock> source,
        Func<TSource, TResult> transform)
        where TSourceBlock : ISourceBlock<TSource> =>
        source.Result.Select(transform);

    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="TSource">Тип элементов потока-источника</typeparam>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <typeparam name="TSourceBlock">Тип потока-источника</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<TResult, TransformBlock<TSource, TResult>> Select<TSource, TSourceBlock, TResult>(
        this Link<TSource, TSourceBlock> source,
        Func<TSource, Task<TResult>> transform)
        where TSourceBlock : ISourceBlock<TSource> =>
        source.Result.Select(transform);

    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<TResult, TransformBlock<Tuple<T1, T2>, TResult>> Select<T1, T2, TResult>(
        this JoinBlockLink<T1, T2> source,
        Func<Tuple<T1, T2>, TResult> transform) =>
        source.Joiner.Select(transform);

    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<TResult, TransformBlock<Tuple<IList<T1>, IList<T2>>, TResult>> Select<T1, T2, TResult>(
        this BatchedJoinBlockLink<T1, T2> source,
        Func<Tuple<IList<T1>, IList<T2>>, TResult> transform) =>
        source.Joiner.Select(transform);

    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<TResult, TransformBlock<Tuple<IList<T1>, IList<T2>>, TResult>> Select<T1, T2, TResult>(
        this BatchedJoinBlockLink<T1, T2> source,
        Func<Tuple<IList<T1>, IList<T2>>, Task<TResult>> transform) =>
        source.Joiner.Select(transform);

    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="TSource">Тип элементов потока-источника</typeparam>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <param name="TransformCompletion">Задача, завершаемая при завершении обработки данных в потоке-результата</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<TResult, TransformBlock<TSource, TResult>> Select<TSource, TResult>(
        this ISourceBlock<TSource> source,
        Func<TSource, TResult> transform,
        out Task TransformCompletion)
    {
        var transformer = new TransformBlock<TSource, TResult>(transform);
        var unsubscriber = source.LinkTo(transformer);
        TransformCompletion = transformer.Completion;
        return new(unsubscriber, transformer);
    }

    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="TSource">Тип элементов потока-источника</typeparam>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <param name="TransformCompletion">Задача, завершаемая при завершении обработки данных в потоке-результата</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<TResult, TransformBlock<TSource, TResult>> Select<TSource, TResult>(
        this ISourceBlock<TSource> source,
        Func<TSource, Task<TResult>> transform,
        out Task TransformCompletion)
    {
        var transformer = new TransformBlock<TSource, TResult>(transform);
        var unsubscriber = source.LinkTo(transformer);
        TransformCompletion = transformer.Completion;
        return new(unsubscriber, transformer);
    }

    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="TSource">Тип элементов потока-источника</typeparam>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <typeparam name="TSourceBlock">Тип потока-источника</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <param name="TransformCompletion">Задача, завершаемая при завершении обработки данных в потоке-результата</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<TResult, TransformBlock<TSource, TResult>> Select<TSource, TSourceBlock, TResult>(
        this Link<TSource, TSourceBlock> source,
        Func<TSource, TResult> transform,
        out Task TransformCompletion)
        where TSourceBlock : ISourceBlock<TSource> =>
        source.Result.Select(transform, out TransformCompletion);

    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="TSource">Тип элементов потока-источника</typeparam>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <typeparam name="TSourceBlock">Тип потока-источника</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <param name="TransformCompletion">Задача, завершаемая при завершении обработки данных в потоке-результата</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<TResult, TransformBlock<TSource, TResult>> Select<TSource, TSourceBlock, TResult>(
        this Link<TSource, TSourceBlock> source,
        Func<TSource, Task<TResult>> transform,
        out Task TransformCompletion)
        where TSourceBlock : ISourceBlock<TSource> =>
        source.Result.Select(transform, out TransformCompletion);

    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <param name="TransformCompletion">Задача, завершаемая при завершении обработки данных в потоке-результата</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<TResult, TransformBlock<Tuple<T1, T2>, TResult>> Select<T1, T2, TResult>(
        this JoinBlockLink<T1, T2> source,
        Func<Tuple<T1, T2>, TResult> transform,
        out Task TransformCompletion) =>
        source.Joiner.Select(transform, out TransformCompletion);

    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <param name="TransformCompletion">Задача, завершаемая при завершении обработки данных в потоке-результата</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<TResult, TransformBlock<Tuple<T1, T2>, TResult>> Select<T1, T2, TResult>(
        this JoinBlockLink<T1, T2> source,
        Func<Tuple<T1, T2>, Task<TResult>> transform,
        out Task TransformCompletion) =>
        source.Joiner.Select(transform, out TransformCompletion);

    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <param name="TransformCompletion">Задача, завершаемая при завершении обработки данных в потоке-результата</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<TResult, TransformBlock<Tuple<IList<T1>, IList<T2>>, TResult>> Select<T1, T2, TResult>(
        this BatchedJoinBlockLink<T1, T2> source,
        Func<Tuple<IList<T1>, IList<T2>>, TResult> transform,
        out Task TransformCompletion) =>
        source.Joiner.Select(transform, out TransformCompletion);

    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <param name="TransformCompletion">Задача, завершаемая при завершении обработки данных в потоке-результата</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<TResult, TransformBlock<Tuple<IList<T1>, IList<T2>>, TResult>> Select<T1, T2, TResult>(
        this BatchedJoinBlockLink<T1, T2> source,
        Func<Tuple<IList<T1>, IList<T2>>, Task<TResult>> transform,
        out Task TransformCompletion) =>
        source.Joiner.Select(transform, out TransformCompletion);

    /// <summary>Преобразование элемента входного потока в набор элементов выходного потока</summary>
    /// <typeparam name="TSource">Тип элементов входного потока</typeparam>
    /// <typeparam name="TResult">Тип элементов выходного потока</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="selector">Метод формирования набора элементов на основе каждого из элементов входного потока</param>
    /// <returns>Связь с созданным преобразователем</returns>
    public static Link<TResult, TransformManyBlock<TSource, TResult>> SelectMany<TSource, TResult>(
        this ISourceBlock<TSource> source,
        Func<TSource, IEnumerable<TResult>> selector)
    {
        var transformer = new TransformManyBlock<TSource, TResult>(selector);
        var unsubscriber = source.LinkTo(transformer);
        return new(unsubscriber, transformer);
    }
}
