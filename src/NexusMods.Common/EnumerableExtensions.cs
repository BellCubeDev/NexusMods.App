namespace NexusMods.Common;

/// <summary>
/// Extensions for collections implementing the enumerable interface.
/// </summary>
// ReSharper disable once InconsistentNaming
public static class EnumerableExtensions
{
    /// <summary>
    /// Transforms a <see cref="IEnumerable{T}"/> into a <see cref="IAsyncEnumerable{TOut}"/> via a transform function
    /// </summary>
    /// <param name="coll">The collection to apply the operation on.</param>
    /// <param name="fn">The function that returns an output for each input.</param>
    /// <typeparam name="TIn">Type of input item used.</typeparam>
    /// <typeparam name="TOut">Type of output item used.</typeparam>
    public static async IAsyncEnumerable<TOut> SelectAsync<TIn, TOut>(this IEnumerable<TIn> coll,
        Func<TIn, ValueTask<TOut>> fn)
    {
        foreach (var itm in coll)
            yield return await fn(itm);
    }

    /// <summary>
    /// Reduces a <see cref="IAsyncEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/> into a <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="coll">The collection to apply the operation on.</param>
    /// <typeparam name="TKey">Type of key used.</typeparam>
    /// <typeparam name="TValue">Type of value used.</typeparam>
    public static async Task<Dictionary<TKey, TValue>> ToDictionary<TKey, TValue>(this IAsyncEnumerable<KeyValuePair<TKey, TValue>> coll)
        where TKey : notnull
    {
        var dict = new Dictionary<TKey, TValue>();
        await foreach (var itm in coll)
            dict.Add(itm.Key, itm.Value);

        return dict;
    }

    /// <summary>
    /// Transforms a IAsyncEnumerable into a dictionary creating keys with a key selector
    /// </summary>
    /// <param name="coll">The collection to apply the operation on.</param>
    /// <param name="keySelector">Function which returns the key given an item.</param>
    /// <typeparam name="TItem">The type of item we are operating on.</typeparam>
    /// <typeparam name="TKey">The key bound to each item.</typeparam>
    public static async Task<Dictionary<TKey, TItem>> ToDictionary<TItem, TKey>(this IAsyncEnumerable<TItem> coll, Func<TItem, TKey> keySelector)
        where TKey : notnull
    {
        var dict = new Dictionary<TKey, TItem>();
        await foreach (var itm in coll)
            dict.Add(keySelector(itm), itm);

        return dict;
    }
}
