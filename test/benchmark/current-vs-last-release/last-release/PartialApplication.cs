namespace DtmfDetection.LastRelease
{
    using System;

    public static class PartialApplication
    {
        // Partial application of binary Func's.
        public static Func<T2, TResult> Apply<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 arg1) =>
            arg2 => func(arg1, arg2);

        // Partial application of ternary Action's.
        public static Action<T2, T3> Apply<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1) =>
            (arg2, arg3) => action(arg1, arg2, arg3);
    }
}
