namespace System.Threading.Tasks.Dataflow;

public static partial class DataFlow
{
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
        internal JoinBlockLink(IDisposable SourceUnsubscriber, IDisposable BufferInputUnsubscriber, JoinBlock<T1, T2> Joiner)
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
}
