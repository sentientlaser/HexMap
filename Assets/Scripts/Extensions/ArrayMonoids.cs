using System;

/// <summary>
/// A collection of simple monoids (these aren't pure monads) that apply only to inArrayays.
///
/// I am aware that this sort of functionality is available in linq, but linq is slow and requires juggling from IEnumerable.
/// </summary>
public static class ArrayMonoids
{

    /// <summary>
    /// Apply an operation to an array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="inArray">the array to apply the function to</param>
    /// <param name="op">the operation to apply</param>
    /// <returns>A new array containing the result of the functions</returns>
    public static R[] Map<T, R>(this T[] inArray, Func<T, R> op)
    {
        R[] retVal = new R[inArray.Length];
        for (int i = 0; i < inArray.Length; i++)
        {
            retVal[i] = op(inArray[i]);
        }
        return retVal;
    }

    /// <summary>
    /// Apply an action to all memebers of an array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inArray">the array to apply the function to</param>
    /// <param name="op">the operation to apply</param>
    public static void Apply<T>(this T[] inArray, Action<T> op)
    {
        for (int i = 0; i < inArray.Length; i++)
        {
            op(inArray[i]);
        }
    }

    /// <summary>
    /// Count the members of an array that satisfy a predicate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inArray">the array to apply the predicate to</param>
    /// <param name="pr">the predicate to use</param>
    /// <returns>The number of elements where <paramref name="pr"/> is true.</returns>
    public static int Count<T>(this T[] inArray, Predicate<T> pr)
    {
        int c = 0;
        for (int i = 0; i < inArray.Length; i++)
        {
            if (pr(inArray[i]))
            {
                c++; // no, this is C#
            } 
        }
        return c;
    }

    /// <summary>
    /// Get the elements of an array that satisfy a predicate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inArray">the array to apply the predicate to</param>
    /// <param name="pr">the predicate to use</param>
    /// <returns>A new array with elements where <paramref name="pr"/> is true.</returns>
    public static T[] Filter<T>(this T[] inArray, Predicate<T> pr)
    {
        int v = 0;
        T[] retVal = new T[inArray.Count(pr)];
        for (int i = 0; i < inArray.Length; i++)
        {
            if (pr(inArray[i]))
            {
                retVal[v++] = inArray[i];
            }
        }
        return retVal;
    }

    /// <summary>
    /// Cumulatively apply a function to the members of an array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="inArray">The array to use</param>
    /// <param name="iVal">an initial value to use</param>
    /// <param name="func">the function to apply</param>
    /// <returns>The cumulative value of applying this function</returns>
    public static R Fold<T,R>(this T[] inArray, R iVal, Func<R, T, R> func)
    {
        R tmp = iVal;
        foreach(T v in inArray)
        {
            tmp = func(tmp, v);
        }
        return tmp;
    }

    /// <summary>
    /// Cumulatively apply a function to the members of an array without an initial value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inArray">The array to use</param>
    /// <param name="func">the function to apply</param>
    /// <returns>The cumulative value of applying this function</returns>
    public static T Fold<T> (this T[] inArray, Func<T,T,T> func)
    {

        T tmp = inArray[0];
        for (int i = 1; i < inArray.Length; i++)
        {
            tmp = func(tmp, inArray[i]);
        }
        return tmp;
    }

    // TODO: okay this is getting to the point I might have to scope out a library.
    /// <summary>
    /// Used to help pretty print arrays
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inArray">The array to join</param>
    /// <param name="sep">the separator to use</param>
    /// <returns>A string with the separator delimiting all members.</returns>
    public static string MkString<T>(this T[] inArray, string sep) => inArray.Map(v=>v.ToString()).Fold((result, value) => result + sep + value);
}
