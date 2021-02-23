namespace System.Threading.Tasks.Dataflow
{
    public static class DataFlow
    {
        public static BufferBlock<T> PipeLine<T>() => new();
        public static BufferBlock<T> PipeLine<T>(out BufferBlock<T> input) => input = new();
        public static BufferBlock<T> PipeLine<T>(int Capacity, TaskScheduler? Scheduler = null, CancellationToken Cancel = default) =>
            new(new DataflowBlockOptions
            {
                BoundedCapacity = Capacity,
                TaskScheduler = Scheduler ?? TaskScheduler.Current,
                CancellationToken = Cancel,
            });

        public static BufferBlock<T> PipeLine<T>(Action<BufferBlock<T>> Configuration)
        {
            var input = new BufferBlock<T>();
            Configuration(input);
            return input;
        }


        private static T SetLinkTo<TOutput, T>(
            this ISourceBlock<TOutput> source,
            T result)
            where T : ITargetBlock<TOutput>
        {
            _ = source.LinkTo(result, new DataflowLinkOptions { PropagateCompletion = true });
            return result;
        }

        private static T SetLinkTo<TOutput, T>(
            this ISourceBlock<TOutput> source,
            T result,
            out Task Completion)
            where T : ITargetBlock<TOutput>
        {
            _ = source.LinkTo(result, new DataflowLinkOptions { PropagateCompletion = true });
            Completion = source.Completion;
            return result;
        }

        public static TransformBlock<TSource, TResult> Select<TSource, TResult>(
            this ISourceBlock<TSource> source,
            Func<TSource, TResult> transform) =>
            source.SetLinkTo(new TransformBlock<TSource, TResult>(transform));

        public static TransformBlock<TSource, TResult> Select<TSource, TResult>(
            this ISourceBlock<TSource> source,
            Func<TSource, TResult> transform,
            out Task TransformCompletion) =>
            source.SetLinkTo(new TransformBlock<TSource, TResult>(transform), out TransformCompletion);

        public static BatchBlock<T> Aggregate<T>(this ISourceBlock<T> source, int BatchSize) =>
            source.SetLinkTo(new BatchBlock<T>(BatchSize));

        public static ActionBlock<T> ForEach<T>(this ISourceBlock<T> source, Action<T> action) =>
            source.SetLinkTo(new ActionBlock<T>(action));

        public static JoinBlock<T1, T2> Join<T1, T2>(
            this ISourceBlock<T1> source,
            out BufferBlock<T2> Input,
            bool Greedy = false)
        {
            Input = new BufferBlock<T2>();
            var join = new JoinBlock<T1, T2>(new GroupingDataflowBlockOptions { Greedy = Greedy });
            _ = source.LinkTo(join.Target1, new DataflowLinkOptions { PropagateCompletion = true });
            _ = Input.LinkTo(join.Target2, new DataflowLinkOptions { PropagateCompletion = true });
            return join;
        }

        public static BatchedJoinBlock<T1, T2> BatchedJoin<T1, T2>(
            this ISourceBlock<T1> source,
            out BufferBlock<T2> Input,
            int BatchSize,
            bool Greedy = false)
        {
            Input = new BufferBlock<T2>();
            var join = new BatchedJoinBlock<T1, T2>(BatchSize, new GroupingDataflowBlockOptions { Greedy = Greedy });
            _ = source.LinkTo(join.Target1, new DataflowLinkOptions { PropagateCompletion = true });
            _ = Input.LinkTo(join.Target2, new DataflowLinkOptions { PropagateCompletion = true });
            return join;
        }

        public static BufferBlock<T> When<T>(this ISourceBlock<T> source, Predicate<T> predicate)
        {
            var result = new BufferBlock<T>();
            _ = source.LinkTo(result, new DataflowLinkOptions { PropagateCompletion = true }, predicate);
            return result;
        }
    }
}
