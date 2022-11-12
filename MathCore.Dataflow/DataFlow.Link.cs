namespace System.Threading.Tasks.Dataflow;

public static partial class DataFlow
{
    /// <summary>Связь между потоками, которую можно разорвать вызвав метод <see cref="Dispose()"/></summary>
    /// <typeparam name="TResult">Тип элементов потока</typeparam>
    /// <typeparam name="TBlock">Тип потока, с которым выполнена связь</typeparam>
    public readonly ref struct Link<TResult, TBlock>
    {
        /// <summary>
        /// Объект, обеспечивающий при вызове метода <see cref="IDisposable"/>.<see cref="IDisposable.Dispose()"/>
        /// разрыв связи, установленной между потоками
        /// </summary>
        public IDisposable Unsubscriber { get; }

        /// <summary>Поток, к которому установлена связь</summary>
        public TBlock Result { get; }

        /// <summary>Инициализация новой связи с потоком</summary>
        /// <param name="Unsubscriber">Объект разрыва связи</param>
        /// <param name="Result">Поток, к которому установлена связь</param>
        internal Link(IDisposable Unsubscriber, TBlock Result)
        {
            this.Unsubscriber = Unsubscriber;
            this.Result = Result;
        }

        /// <summary>Вызов этого метода обеспечивает разрыв связи</summary>
        public void Dispose() => Unsubscriber.Dispose();

        /// <summary>Оператор неявного приведения объекта связи к типу результирующего потока</summary>
        /// <param name="link">Объект связи</param>
        /// <returns>Результирующий поток</returns>
        public static implicit operator TBlock(Link<TResult, TBlock> link) => link.Result;
    }
}
