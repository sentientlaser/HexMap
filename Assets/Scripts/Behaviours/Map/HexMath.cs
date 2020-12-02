using System;
using UnityEngine;
using System.Runtime.CompilerServices;

// makes things a little easier to read.
using CubicCoordinate = System.ValueTuple<int, int, int>;
using ApproxCubicCoordinate = System.ValueTuple<float, float, float>;
using AxialCoordinate = System.ValueTuple<int, int>;

using static Map.HexMath;

namespace Map
{
    /// <summary>
    /// Utility class of extension methods to simplify complex operations on Coordinate Tuples
    /// </summary>
    public static class CoordinateExtensionMethods
    {
        /// <summary>
        /// used to pass the `+` operator as a function
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int add(int a, int b) => a + b;

        /// <summary>
        /// used to pass the `-` operator as a function
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int sub(int a, int b) => a - b;

        /// <summary>
        /// Apply a unary function to all members of a trinary tuple
        /// </summary>
        /// <param name="c">the trinary tuple to apply the function to</param>
        /// <param name="func">the function to apply</param>
        /// <returns>a new tuple with the result of the function</returns>
        public static ValueTuple<R, R, R> Apply<T, R>(this ValueTuple<T, T, T> c, Func<T, R> func)
            => (func(c.Item1), func(c.Item2), func(c.Item3));

        /// <summary>
        /// Apply a binary function to all members of a trinary tuple and a second supplied trinary tuple
        /// </summary>
        /// <param name="a">the trinary tuple to apply the function to (second param)</param>
        /// <param name="b">the trinary tuple to apply the function to (second param)</param>
        /// <param name="func">the function to apply</param>
        /// <returns>a new tuple with the result of the function</returns>
        public static ValueTuple<R, R, R> Apply<T, R>(this ValueTuple<T, T, T> a, ValueTuple<T, T, T> b, Func<T, T, R> func)
            => (func(a.Item1, b.Item1), func(a.Item2, b.Item2), func(a.Item3, b.Item3));

        /// <summary>
        /// cumulative apply a function to a trinary tuple
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c">the trinary tuple to apply the function to</param>
        /// <param name="func">the function to apply</param>
        /// <returns>The cumulatve result of the function</returns>
        public static T Fold<T>(this ValueTuple<T, T, T> c, Func<T, T, T> func)
            => func(c.Item3, func(c.Item1, c.Item2));

        /// <summary>
        /// Adds two coords
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>the vector a+b</returns>
        public static CubicCoordinate Add(this CubicCoordinate a, CubicCoordinate b) => a.Apply(b, add);

        /// <summary>
        /// Adds two coords
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>the vector a*b</returns>
        public static CubicCoordinate Sub(this CubicCoordinate a, CubicCoordinate b) => a.Apply(b, sub);

        /// <summary>
        /// The magnitude of this coordinate 
        /// </summary>
        public static int Sum(this CubicCoordinate c) => c.Fold(add);

        #pragma warning disable IDE1006 // lower case on methods
        /// <summary>
        /// alias for <c>Item1</c> of a cubic coord
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public static T r<T>(this ValueTuple<T, T, T> c) => c.Item1;

        /// <summary>
        /// alias for <c>Item2</c> of a cubic coord
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T s<T>(this ValueTuple<T, T, T> c) => c.Item2;

        /// <summary>
        /// alias for <c>Item3</c> of a cubic coord
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T t<T>(this ValueTuple<T, T, T> c) => c.Item3;

        /// <summary>
        /// alias for <c>Item1</c> of an axial coord
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T p<T>(this ValueTuple<T, T> c) => c.Item1;

        /// <summary>
        /// alias for <c>Item2</c> of an axial coord
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T q<T>(this ValueTuple<T, T> c) => c.Item2;
        #pragma warning restore IDE1006 // Naming Styles

        /// <summary>
        /// Convert a cubic to an axial coord
        /// </summary>
        public static AxialCoordinate AsAxial(this CubicCoordinate hex) => (hex.r(), hex.s());

        /// <summary>
        /// Convert an axial to a cubic coord
        /// </summary>
        public static CubicCoordinate AsCubic(this AxialCoordinate hex) => (hex.p(), -hex.p() - hex.q(), hex.q());

        /// <summary>
        /// Check that <c>r+s+t == 0</c>
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">if <c>r+s+t != 0</c></exception>
        /// <returns>the supplied coordinate</returns>
        public static CubicCoordinate Validate(this CubicCoordinate c)
        {
            if (c.Sum() != 0) throw new IndexOutOfRangeException($"Cubic Coordinates {c} are not valid (sum is non-zero)");
            return c;
        }
    }

    public enum Direction : int
    {
        /// <summary>
        /// +z 
        /// </summary>
        Foward,
        /// <summary>
        /// +z, +x
        /// </summary>
        FowardRight,
        /// <summary>
        /// -z +x
        /// </summary>
        BackRight,
        /// <summary>
        /// -z
        /// </summary>
        Back,
        /// <summary>
        /// -z, -x
        /// </summary>
        BackLeft,
        /// <summary>
        /// +z, -x
        /// </summary>
        FowardLeft,
    }


    public static class HexMath
    {
        /// <summary>
        /// The vector coordinates of the neighbourhood in hex grids
        /// </summary>
        public static readonly CubicCoordinate[] Neighbours = new CubicCoordinate[] {
            (0, +1, -1), // fwd
            (+1, 0, -1), // fwdrht
            (+1, -1, 0), // bckrht
            (0, -1, +1), // bck
            (-1, 0, +1), // bckleft
            (-1, +1, 0), // fwdlft
        };

        /// <summary>
        /// gets the vector from Neighbours for a direction
        /// </summary>
        public static CubicCoordinate Vector(this Direction d) => Neighbours[(int)d];

        /// <summary>
        /// Move a coords
        /// </summary>
        /// <param name="c">the starting coord</param>
        /// <param name="d">the direction to move</param>
        /// <returns>the coords of this move</returns>
        public static CubicCoordinate Move(this CubicCoordinate c, Direction d) => c.Add(d.Vector());

        /// <summary>
        /// get the coords of the neighbours of a given hex
        /// </summary>
        /// <returns>all neighbours of <paramref name="hex"/></returns>
        public static CubicCoordinate[] Neighbourhood(CubicCoordinate hex) => Neighbours.Map(v => v.Add(hex));

        /// <summary>
        /// The manhattan distance of a coord to the origin
        /// </summary>
        public static int ManhattanDistance(CubicCoordinate c) => c.Apply(Math.Abs).Sum();
        /// <summary>
        /// The manhattan distance between <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static int ManhattanDistance(CubicCoordinate a, CubicCoordinate b) => ManhattanDistance(a.Sub(b));

        /// <summary>
        /// The distance in hexes from <paramref name="a"/> (inclusive) to <paramref name="b"/> (exclusive)
        /// </summary>
        public static int Distance(CubicCoordinate a, CubicCoordinate b) => ManhattanDistance(a, b) / 2;

        /// <summary>
        /// returns the hex that matches approximate coordinates <paramref name="c"/>, essentially a complex cast to (int, int, int)
        /// </summary>
        public static CubicCoordinate Round(ApproxCubicCoordinate c)
        {
            (float r, float s, float t) q = c.Apply(Mathf.Round);
            (float r, float s, float t) d = q.Apply(c, (a, b) => a - b).Apply(Mathf.Abs);
            switch (true)
            {
                case bool t when (d.r > d.s && d.r > d.t):
                    return (-q.s - q.t, q.s, q.t).Apply(v => (int)v);

                case bool t when (d.s > d.t):
                    return (q.r, -q.r - q.t, q.t).Apply(v => (int)v);

                default:
                    return (q.r, q.s, -q.r - q.s).Apply(v => (int)v);

            }
        }

        public static CubicCoordinate[] Line(CubicCoordinate start, CubicCoordinate end)// TODO: test
        {
            Func<int, int, float> lerp(float t) // {} syntax because returning a function 
            {
                return (int a, int b) => a + (b - a) * t;
            }

            ApproxCubicCoordinate lerp3(CubicCoordinate a, CubicCoordinate b, float t) => a.Apply(b, lerp(t));

            var d = Distance(start, end);

            CubicCoordinate[] retVal = new CubicCoordinate[d + 1];
            for (int i = 0; i < retVal.Length; i++)
            {
                retVal[i] = Round(lerp3(start, end, (1f / d) * i));
            }

            return retVal;
        }
    }
}
