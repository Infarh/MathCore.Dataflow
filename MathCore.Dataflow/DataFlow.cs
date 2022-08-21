namespace System.Threading.Tasks.Dataflow;

/// <summary>Поток данных</summary>
public static partial class DataFlow
{
    /// <summary>Инициализация обычного буферезированного потока</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <returns>Новый экземпляр <see cref="BufferBlock{T}"/></returns>
    public static BufferBlock<T> PipeLine<T>() => new();

    /// <summary>Инициализация обычного буферезированного потока</summary>
    /// <param name="options">параметры блока</param>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <returns>Новый экземпляр <see cref="BufferBlock{T}"/></returns>
    public static BufferBlock<T> PipeLine<T>(DataflowBlockOptions options) => new(options);

    /// <summary>Инициализация обычного буферезированного потока</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <param name="input">Вход потока</param>
    /// <returns>Новый экземпляр <see cref="BufferBlock{T}"/></returns>
    public static BufferBlock<T> PipeLine<T>(out BufferBlock<T> input) => input = new();

    /// <summary>Инициализация обычного буферезированного потока</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <param name="input">Вход потока</param>
    /// <param name="options">параметры блока</param>
    /// <returns>Новый экземпляр <see cref="BufferBlock{T}"/></returns>
    public static BufferBlock<T> PipeLine<T>(out BufferBlock<T> input, DataflowBlockOptions options) => input = new(options);

    /// <summary>Инициализация обычного буферезированного потока</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <param name="Capacity">Ёмкость буфера</param>
    /// <param name="Scheduler">Планировщик задач потока</param>
    /// <param name="Cancel">Флаг отмены асинхронной операции</param>
    /// <returns>Новый экземпляр <see cref="BufferBlock{T}"/></returns>
    public static BufferBlock<T> PipeLine<T>(int Capacity, TaskScheduler? Scheduler = null, CancellationToken Cancel = default) =>
        new(new DataflowBlockOptions
        {
            BoundedCapacity = Capacity,
            TaskScheduler = Scheduler ?? TaskScheduler.Current,
            CancellationToken = Cancel,
        });

    /// <summary>Инициализация обычного буферезированного потока</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <param name="Capacity">Ёмкость буфера</param>
    /// <param name="input">Вход потока</param>
    /// <param name="Scheduler">Планировщик задач потока</param>
    /// <param name="Cancel">Флаг отмены асинхронной операции</param>
    /// <returns>Новый экземпляр <see cref="BufferBlock{T}"/></returns>
    public static BufferBlock<T> PipeLine<T>(int Capacity, out BufferBlock<T> input, TaskScheduler? Scheduler = null, CancellationToken Cancel = default) =>
        input = new(new DataflowBlockOptions
        {
            BoundedCapacity = Capacity,
            TaskScheduler = Scheduler ?? TaskScheduler.Current,
            CancellationToken = Cancel,
        });

    /// <summary>Инициализация обычного буферезированного потока</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <param name="Configuration">Конфигурация потока</param>
    /// <returns>Новый экземпляр <see cref="BufferBlock{T}"/></returns>
    public static BufferBlock<T> PipeLine<T>(Action<BufferBlock<T>> Configuration)
    {
        var input = new BufferBlock<T>();
        Configuration(input);
        return input;
    }

    /// <summary>Инициализация обычного буферезированного потока</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <param name="input">Вход потока</param>
    /// <param name="Configuration">Конфигурация потока</param>
    /// <returns>Новый экземпляр <see cref="BufferBlock{T}"/></returns>
    public static BufferBlock<T> PipeLine<T>(out BufferBlock<T> input, Action<BufferBlock<T>> Configuration)
    {
        input = new BufferBlock<T>();
        Configuration(input);
        return input;
    }
}