using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime.Serialization;

// makes things a little easier to read.
using CubicCoordinate = System.ValueTuple<int, int, int>;
using AxialCoordinate = System.ValueTuple<int, int>;

using static ArrayMonoids;
using static Map.CoordinateExtensionMethods;

namespace Map
{
    /// <summary>
    /// Represents the bounds of a single dimension of the logical 
    /// </summary>
    [Serializable]
    public class MapDimension
    {
        /// <summary>
        /// The start and end indices (inclusive) on this logical dimension.
        /// 
        /// </summary>
        public int Min, Max;

        /// <summary>
        /// returns the size of this dimension (which is the difference between end and start plus 1)
        /// </summary>
        /// <exception cref="ArithmeticException">if <see cref="Min"/> is strictly less than <see cref="Max"/></exception>

        public int Size
        {
            get {
                if (Min > Max) throw new ArithmeticException("Start must be less than End");

                return Max - Min + 1;
            }

        }

        public override string ToString() => $"MapDimension(Min: {Min}, Max: {Max}, Size: {Size})";
    }

    /// <summary>
    /// Represents a graticule of cells.  The name 'Grid' causes issues with unity, so this is named 'Graticule'.
    /// </summary>
    public class Graticule : MonoBehaviour
    {

        #region Fields Exposed To Unity Editor
        /// <summary>
        /// The etents of this map in logical R,S,T units
        /// </summary>
        public MapDimension RDim, SDim, TDim;
        #endregion


        #region Internal State Variables
        /// <summary>
        /// the cell storage.
        /// </summary>
        #pragma warning disable IDE0044 // Add readonly modifier
        private Cell[,] cells;
        #pragma warning restore IDE0044 // Add readonly modifier

        #endregion

        #region Unity Lifecycle Methods
        void Awake()
        {
        }
        #endregion

        #region Accessors and Iterators
        /// <summary>
        /// Get a cell
        /// </summary>
        /// <param name="r">the R index</param>
        /// <param name="s">the S index</param>
        /// <param name="t">the T index</param>
        /// <exception cref="IndexOutOfRangeException">When the sum of the indices is non-zero</exception>
        /// <returns>The cell at (r,s,t)</returns>
        public Cell this[int r, int s, int t]
        {
            get
            {
                (r, s, t).Validate(); 
                return cells[r - RDim.Min, s - SDim.Min];
            }

            set
            {
                (r, s, t).Validate();
                cells[r - RDim.Min, s - SDim.Min] = value;
            }
        }

        /// <summary>
        /// Get a cell, alias for <see cref="this[int, int, int]"/>
        /// </summary>
        /// <param name="c">The tuple (r,s,t) for each index</param>
        /// <returns>The cell at (r,s,t)</returns>
        public Cell this[(int r, int s, int t) c]
        {
            get => this[c.r, c.s, c.t];
            set => this[c.r, c.s, c.t] = value;
        }

        /// <summary>
        /// Gets a range of cells
        /// </summary>
        /// <param name="coords">an array of coordinates for cells to get</param>
        /// <returns>The cells at each specified index</returns>
        public Cell[] this[CubicCoordinate[] coords]
        {
            get => coords.Map(c => this[c.Item1, c.Item2, c.Item3]);
        }

        /// <summary>
        /// Applies an action to all non-zero cells in the graticule.
        /// </summary>
        /// <remarks>
        /// Iterates over the internal collection as a single dimension, so is fast for dense storage.
        /// </remarks>
        /// <param name="action">The action to apply</param>
        public void Apply(Action<Cell> action)
        {
            foreach (Cell cell in cells)
                if (cell != null)
                    action(cell);
        }

        /// <summary>
        /// Applies an action to each location in the internal storage
        /// Alias for <see cref="Apply(Action{int, int, int})"/>
        /// </summary>
        /// <param name="action">The action to apply</param>
        public void Apply(Action<CubicCoordinate> action)
        {
            Apply( (int r, int s, int t) => action((r, s, t)));
        }

        /// <summary>
        /// Applies an action to each location in the internal storage.
        /// </summary>
        /// <remarks>
        /// Since this iterates over the dimensions this can be used to populate girds.
        /// TODO: currently pretty inefficient, as it iterates over the entire cube defined for this graticule, not just the interscting plane that defines the hexes.
        /// </remarks>
        /// <param name="action">The action to apply</param>
        public void Apply(Action<int, int, int> action)
        {
            for (int r = RDim.Min; r <= RDim.Max; r++)
                for (int s = SDim.Min; s <= SDim.Max; s++)
                    for (int t = TDim.Min; t <= TDim.Max; t++)
                        if (r + s + t == 0)
                            action(r, s, t);
        }

        public void BetterApply(Action<int, int, int> action)
        {
            for (int r = RDim.Min; r <= RDim.Max; r++) {
                int si = Math.Max(SDim.Min, -r - TDim.Max);
                int sf = Math.Max(SDim.Max, -r - TDim.Min);
                for (int s = si; s <= sf; s++)
                {
                    var t = -r - s;
                    action(r, s, t);
                }
            }
        }
        #endregion

        public void InitStorage()
        {
            cells = new Cell[RDim.Size, SDim.Size];
        }

    }

}