namespace System.Threading.Tasks.Dataflow;

public static partial class DataFlow
{
    /// <summary>Формирование разветвления на несколько потоков-приёмников</summary>
    /// <typeparam name="T">Тип элемента потока</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="Clonner">Метод копирования каждого элемента данных потока (если не указан, то каждый элемент данных потока будет передаваться без изменений)</param>
    /// <returns></returns>
    public static Link<T, BroadcastBlock<T>> Braodcast<T>(this ISourceBlock<T> source, Func<T, T>? Clonner = null)
    {
        Clonner ??= v => v;
        var result = new BroadcastBlock<T>(Clonner);
        var unsubscriber = source.LinkTo(result, new DataflowLinkOptions { PropagateCompletion = true });
        return new(unsubscriber, result);
    }

    public static Link<T, BroadcastBlock<T>> Braodcast<T, TSourceBlock>(this Link<T, TSourceBlock> source, Func<T, T>? Clonner = null)
        where TSourceBlock : ISourceBlock<T> =>
        source.Result.Braodcast(Clonner);
}
