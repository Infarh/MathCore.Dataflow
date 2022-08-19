namespace System.Threading.Tasks.Dataflow;

/// <summary>Поток данных</summary>
public static class DataFlow
{
    /// <summary>Инициализация обычного буферезированного потока</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <returns>Новый экземпляр <see cref="BufferBlock{T}"/></returns>
    public static BufferBlock<T> PipeLine<T>() => new();

    /// <summary>Инициализация обычного буферезированного потока</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <param name="input">Вход потока</param>
    /// <returns>Новый экземпляр <see cref="BufferBlock{T}"/></returns>
    public static BufferBlock<T> PipeLine<T>(out BufferBlock<T> input) => input = new();

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
    /// <param name="Configuration">Конфигурация потока</param>
    /// <returns>Новый экземпляр <see cref="BufferBlock{T}"/></returns>
    public static BufferBlock<T> PipeLine<T>(Action<BufferBlock<T>> Configuration)
    {
        var input = new BufferBlock<T>();
        Configuration(input);
        return input;
    }

    /// <summary>Связь между потоками, которую можно разорвать вызвав метод <see cref="Dispose()"/></summary>
    /// <typeparam name="TItem">Тип элементов потока</typeparam>
    /// <typeparam name="T">Тип потока, с которым выполнена связь</typeparam>
    public readonly ref struct Link<TItem, T> where T : ITargetBlock<TItem>
    {
        /// <summary>
        /// Объект, обеспечивающий при вызове метода <see cref="IDisposable"/>.<see cref="IDisposable.Dispose()"/>
        /// разрыв связи, установленной между потоками
        /// </summary>
        public IDisposable Unsubscriber { get; }

        /// <summary>Поток, к которому установлена связь</summary>
        public T Result { get; }

        /// <summary>Инициализация новой связи с потоком</summary>
        /// <param name="Unsubscriber">Объект разрыва связи</param>
        /// <param name="Result">Поток, к которому установлена связь</param>
        public Link(IDisposable Unsubscriber, T Result)
        {
            this.Unsubscriber = Unsubscriber;
            this.Result = Result;
        }

        /// <summary>Вызов этого метода обеспечивает разрыв связи</summary>
        public void Dispose() => Unsubscriber.Dispose();

        /// <summary>Оператор неявного приведения объекта связи к типу результирующего потока</summary>
        /// <param name="link">Объект связи</param>
        /// <returns>Результирующий поток</returns>
        public static implicit operator T(Link<TItem, T> link) => link.Result;
    }

    /// <summary>Формирование соединения потока-источника с потоком-результата</summary>
    /// <typeparam name="TOutput">Тип элементов потока-результата</typeparam>
    /// <typeparam name="T">Тип элементов потока-источника</typeparam>
    /// <param name="source">Исходный потока из которого формируется соединение</param>
    /// <param name="result">Поток-приёмник, формирующий результат обработки данных</param>
    /// <returns>Поток-приёмник к которому выполнено соединение</returns>
    private static Link<TOutput, T> SetLinkTo<TOutput, T>(
        this ISourceBlock<TOutput> source,
        T result)
        where T : ITargetBlock<TOutput>
    {
        var unsubscriber = source.LinkTo(result, new DataflowLinkOptions { PropagateCompletion = true });
        return new(unsubscriber, result);
    }

    /// <summary>Формирование соединения потока-источника с потоком-результата</summary>
    /// <typeparam name="TOutput">Тип элементов потока-результата</typeparam>
    /// <typeparam name="T">Тип элементов потока-источника</typeparam>
    /// <param name="source">Исходный потока из которого формируется соединение</param>
    /// <param name="result">Поток-приёмник, формирующий результат обработки данных</param>
    /// <param name="Completion">Задача, завершаемая при завершении обработки данных в потоке-результата</param>
    /// <returns>Поток-приёмник к которому выполнено соединение</returns>
    private static Link<TOutput, T> SetLinkTo<TOutput, T>(
        this ISourceBlock<TOutput> source,
        T result,
        out Task Completion)
        where T : ITargetBlock<TOutput>
    {
        var unsubscriber = source.LinkTo(result, new DataflowLinkOptions { PropagateCompletion = true });
        Completion = source.Completion;
        return new(unsubscriber, result);
    }

    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="TSource">Тип элементов потока-источника</typeparam>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<TSource, TransformBlock<TSource, TResult>> Select<TSource, TResult>(
        this ISourceBlock<TSource> source,
        Func<TSource, TResult> transform) =>
        source.SetLinkTo(new TransformBlock<TSource, TResult>(transform));

    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="TSource">Тип элементов потока-источника</typeparam>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <typeparam name="TSourceBlock">Тип потока-источника</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<TSource, TransformBlock<TSource, TResult>> Select<TSource, TSourceBlock, TResult>(
        this Link<TSource, TSourceBlock> source,
        Func<TSource, TResult> transform) 
        where TSourceBlock : ITargetBlock<TSource>, ISourceBlock<TSource> =>
        source.Result.Select(transform);

    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<Tuple<T1, T2>, TransformBlock<Tuple<T1, T2>, TResult>> Select<T1, T2, TResult>(
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
    public static Link<Tuple<IList<T1>, IList<T2>>, TransformBlock<Tuple<IList<T1>, IList<T2>>, TResult>> Select<T1, T2, TResult>(
        this BatchedJoinBlockLink<T1, T2> source,
        Func<Tuple<IList<T1>, IList<T2>>, TResult> transform) =>
        source.Joiner.Select(transform);

    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="TSource">Тип элементов потока-источника</typeparam>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <param name="TransformCompletion">Задача, завершаемая при завершении обработки данных в потоке-результата</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<TSource, TransformBlock<TSource, TResult>> Select<TSource, TResult>(
        this ISourceBlock<TSource> source,
        Func<TSource, TResult> transform,
        out Task TransformCompletion) =>
        source.SetLinkTo(new TransformBlock<TSource, TResult>(transform), out TransformCompletion);

    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="TSource">Тип элементов потока-источника</typeparam>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <typeparam name="TSourceBlock">Тип потока-источника</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <param name="TransformCompletion">Задача, завершаемая при завершении обработки данных в потоке-результата</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<TSource, TransformBlock<TSource, TResult>> Select<TSource, TSourceBlock, TResult>(
        this Link<TSource, TSourceBlock> source,
        Func<TSource, TResult> transform,
        out Task TransformCompletion)
        where TSourceBlock : ITargetBlock<TSource>, ISourceBlock<TSource> =>
        source.Result.Select(transform, out TransformCompletion);

    /// <summary>Преобразование входных данных из потока-источника</summary>
    /// <typeparam name="TResult">Тип элементов потока-результата</typeparam>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <param name="source">Поток-источник</param>
    /// <param name="transform">Метод формирования элементов потока-результата на основе элементов потока-источника</param>
    /// <param name="TransformCompletion">Задача, завершаемая при завершении обработки данных в потоке-результата</param>
    /// <returns>Связь, установленная между исходным потоком-источником данных с новым потоком преобразованных денных</returns>
    public static Link<Tuple<T1, T2>, TransformBlock<Tuple<T1, T2>, TResult>> Select<T1, T2, TResult>(
        this JoinBlockLink<T1, T2> source,
        Func<Tuple<T1, T2>, TResult> transform,
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
    public static Link<Tuple<IList<T1>, IList<T2>>, TransformBlock<Tuple<IList<T1>, IList<T2>>, TResult>> Select<T1, T2, TResult>(
        this BatchedJoinBlockLink<T1, T2> source,
        Func<Tuple<IList<T1>, IList<T2>>, TResult> transform,
        out Task TransformCompletion) =>
        source.Joiner.Select(transform, out TransformCompletion);

    /// <summary>Объединение элементов потока в пакеты указанного размера</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <param name="source">Поток-источник данных</param>
    /// <param name="BatchSize">Размер пакета</param>
    /// <returns>Связь с созданным новым потоком-агрегатором</returns>
    public static Link<T, BatchBlock<T>> Aggregate<T>(this ISourceBlock<T> source, int BatchSize) =>
        source.SetLinkTo(new BatchBlock<T>(BatchSize));

    /// <summary>Объединение элементов потока в пакеты указанного размера</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <typeparam name="TSourceBlock">Тип потока-источника</typeparam>
    /// <param name="source">Поток-источник данных</param>
    /// <param name="BatchSize">Размер пакета</param>
    /// <returns>Связь с созданным новым потоком-агрегатором</returns>
    public static Link<T, BatchBlock<T>> Aggregate<T, TSourceBlock>(
        this Link<T, TSourceBlock> source, 
        int BatchSize)
        where TSourceBlock : ITargetBlock<T>, ISourceBlock<T> =>
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

    /// <summary>Выполнение указанного действия для всех элементов потока</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <param name="source">Поток-источник элементов над которыми требуется выполнить действие</param>
    /// <param name="action">Выполняемое действие</param>
    /// <returns>Связь с вновь созданным обработчиком элементов потока</returns>
    public static Link<T, ActionBlock<T>> ForEach<T>(this ISourceBlock<T> source, Action<T> action) =>
        source.SetLinkTo(new ActionBlock<T>(action));

    /// <summary>Выполнение указанного действия для всех элементов потока</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <typeparam name="TSourceBlock">Тип потока-источника</typeparam>
    /// <param name="source">Поток-источник элементов над которыми требуется выполнить действие</param>
    /// <param name="action">Выполняемое действие</param>
    /// <returns>Связь с вновь созданным обработчиком элементов потока</returns>
    public static Link<T, ActionBlock<T>> ForEach<T, TSourceBlock>(
        this Link<T, TSourceBlock> source,
        Action<T> action)
        where TSourceBlock : ITargetBlock<T>, ISourceBlock<T> =>
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
    public static Link<Tuple<IList<T1>, IList<T2>>, ActionBlock<Tuple<IList<T1>, IList<T2>>>> ForEach<T1, T2>(
        this BatchedJoinBlockLink<T1, T2> source,
        Action<Tuple<IList<T1>, IList<T2>>> action) =>
        source.Joiner.ForEach(action);

    /// <summary>Передача элемента из потока-источника в новый буферизованный поток-приёмник при выполнении условия</summary>
    /// <typeparam name="T">Тип элементов потока</typeparam>
    /// <param name="source">Поток-источник элементов</param>
    /// <param name="predicate">Условие, при выполнении которого элемент потока-источника попадёт в выходной поток</param>
    /// <returns>Связь с созданным новым буферизованным потоком, элементы которого будут отфильтрованными из исходного потока указанным способом</returns>
    public static Link<T, BufferBlock<T>> When<T>(this ISourceBlock<T> source, Predicate<T> predicate)
    {
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
    public static Link<T, BufferBlock<T>> When<T, TSourceBlock>(
        this Link<T, TSourceBlock> source,
        Predicate<T> predicate)
        where TSourceBlock : ITargetBlock<T>, ISourceBlock<T> =>
        source.Result.When(predicate);

    /// <summary>Передача элемента из потока-источника в новый буферизованный поток-приёмник при выполнении условия</summary>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <param name="source">Поток-источник элементов</param>
    /// <param name="predicate">Условие, при выполнении которого элемент потока-источника попадёт в выходной поток</param>
    /// <returns>Связь с созданным новым буферизованным потоком, элементы которого будут отфильтрованными из исходного потока указанным способом</returns>
    public static Link<Tuple<T1, T2>, BufferBlock<Tuple<T1, T2>>> When<T1, T2>(
        this JoinBlockLink<T1, T2> source,
        Predicate<Tuple<T1, T2>> predicate) =>
        source.Joiner.When(predicate);

    /// <summary>Передача элемента из потока-источника в новый буферизованный поток-приёмник при выполнении условия</summary>
    /// <typeparam name="T1">Тип данных первого потока</typeparam>
    /// <typeparam name="T2">Тип данных второго потока</typeparam>
    /// <param name="source">Поток-источник элементов</param>
    /// <param name="predicate">Условие, при выполнении которого элемент потока-источника попадёт в выходной поток</param>
    /// <returns>Связь с созданным новым буферизованным потоком, элементы которого будут отфильтрованными из исходного потока указанным способом</returns>
    public static Link<Tuple<IList<T1>, IList<T2>>, BufferBlock<Tuple<IList<T1>, IList<T2>>>> When<T1, T2>(
        this BatchedJoinBlockLink<T1, T2> source,
        Predicate<Tuple<IList<T1>, IList<T2>>> predicate) =>
        source.Joiner.When(predicate);

    /// <summary>Связь с потоком, объединяющим в себе элементы из двух других потоков и формирующим кортежи из этих элементов</summary>
    /// <typeparam name="T1">Тип элементов первого потока</typeparam>
    /// <typeparam name="T2">Тип элементов второго потока</typeparam>
    public readonly ref struct JoinBlockLink<T1, T2>
    {
        /// <summary>Объект разрыва связи с потоком-источником первого набора элементов</summary>
        public IDisposable SourceUnsubscriber { get; }

        /// <summary>Объект разрыва связи с потоком-источником второго набора элементов</summary>
        public IDisposable BufferInputUnsubscriber { get; }

        /// <summary>Поток-объединитель элементов</summary>
        public JoinBlock<T1, T2> Joiner { get; }

        /// <summary>Формирование новой связи с потоком-объединителем данных из двух других потоков</summary>
        /// <param name="SourceUnsubscriber">Поток-источник первого элемента кортежа</param>
        /// <param name="BufferInputUnsubscriber">Поток-источник второго элемента кортежа</param>
        /// <param name="Joiner">Поток-объединитель элементов</param>
        public JoinBlockLink(IDisposable SourceUnsubscriber, IDisposable BufferInputUnsubscriber, JoinBlock<T1, T2> Joiner)
        {
            this.SourceUnsubscriber = SourceUnsubscriber;
            this.BufferInputUnsubscriber = BufferInputUnsubscriber;
            this.Joiner = Joiner;
        }

        /// <summary>Оператор неявного приведения объекта связи к типу результирующего потока</summary>
        /// <param name="link">Объект связи</param>
        /// <returns>Результирующий поток</returns>
        public static implicit operator JoinBlock<T1, T2>(JoinBlockLink<T1, T2> link) => link.Joiner;

        /// <summary>Вызов этого метода приведёт к разрыву созданных связей со всеми потоками-источниками данных</summary>
        public void Dispose()
        {
            SourceUnsubscriber.Dispose();
            BufferInputUnsubscriber.Dispose();
        }
    }

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

    /// <summary>Связь с потоком, объединяющим в себе элементы из двух других потоков и формирующим кортежи из наборов этих элементов заданной длины</summary>
    /// <typeparam name="T1">Тип элементов первого потока</typeparam>
    /// <typeparam name="T2">Тип элементов второго потока</typeparam>
    public readonly ref struct BatchedJoinBlockLink<T1, T2>
    {
        /// <summary>Объект разрыва связи с потоком-источником первого набора элементов</summary>
        public IDisposable SourceUnsubscriber { get; }
        
        /// <summary>Объект разрыва связи с потоком-источником второго набора элементов</summary>
        public IDisposable BufferInputUnsubscriber { get; }
        
        /// <summary>Поток-объединитель элементов</summary>
        public BatchedJoinBlock<T1, T2> Joiner { get; }

        /// <summary>Формирование новой связи с потоком-объединителем данных из двух других потоков</summary>
        /// <param name="SourceUnsubscriber">Поток-источник первого элемента кортежа</param>
        /// <param name="BufferInputUnsubscriber">Поток-источник второго элемента кортежа</param>
        /// <param name="Joiner">Поток-объединитель элементов</param>
        public BatchedJoinBlockLink(IDisposable SourceUnsubscriber, IDisposable BufferInputUnsubscriber, BatchedJoinBlock<T1, T2> Joiner)
        {
            this.SourceUnsubscriber = SourceUnsubscriber;
            this.BufferInputUnsubscriber = BufferInputUnsubscriber;
            this.Joiner = Joiner;
        }

        /// <summary>Оператор неявного приведения объекта связи к типу результирующего потока</summary>
        /// <param name="link">Объект связи</param>
        /// <returns>Результирующий поток</returns>
        public static implicit operator BatchedJoinBlock<T1, T2>(BatchedJoinBlockLink<T1, T2> link) => link.Joiner;

        /// <summary>Вызов этого метода приведёт к разрыву созданных связей со всеми потоками-источниками данных</summary>
        public void Dispose()
        {
            SourceUnsubscriber.Dispose();
            BufferInputUnsubscriber.Dispose();
        }
    }

    /// <summary>Объединение элементов из двух потоков в один поток кортежей блоков элементов заданной длины</summary>
    /// <typeparam name="T1">Тип элементов первого потока-источника</typeparam>
    /// <typeparam name="T2">Тип элементов второго потока-источника</typeparam>
    /// <param name="source">Поток-источник первых элементов кортежей</param>
    /// <param name="Input">Новый созданный поток-источник вторых элементов кортежей</param>
    /// <param name="BatchSize">Размер формируемого блока</param>
    /// <param name="Greedy">Жадный поток</param>
    /// <returns>Связь, созданная с новым потоком-объединителем элементов</returns>
    public static BatchedJoinBlockLink<T1, T2> BatchedJoin<T1, T2>(
        this ISourceBlock<T1> source,
        out BufferBlock<T2> Input,
        int BatchSize,
        bool Greedy = false)
    {
        Input = new BufferBlock<T2>();
        var join = new BatchedJoinBlock<T1, T2>(BatchSize, new GroupingDataflowBlockOptions { Greedy = Greedy });
        var source_unsubscriber = source.LinkTo(join.Target1, new DataflowLinkOptions { PropagateCompletion = true });
        var input_unsubscriber = Input.LinkTo(join.Target2, new DataflowLinkOptions { PropagateCompletion = true });
        return new(source_unsubscriber, input_unsubscriber, join);
    }
}