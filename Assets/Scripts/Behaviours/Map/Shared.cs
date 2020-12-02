using System;
using UnityEngine;

// makes things a little easier to read.
using CubicCoordinate = System.ValueTuple<int, int, int>;
using ApproxCubicCoordinate = System.ValueTuple<float, float, float>;
using AxialCoordinate = System.ValueTuple<int, int>;

using static ArrayMonoids;

namespace Map {
    
    
    /// <summary>
    /// Provides cartesian (R3) space util functions for manipulating hexes.
    /// </summary>
    public class Geometry //TODO: turn this into a behaviour?  then I could edit in the unity editor and pass it down.
    {
        public static readonly Geometry Default = Geometry.WithRadius(1f);
        /// <summary>
        /// Create a CartesianSpaceResolver using the provided radius as a geometric scaling factor
        /// </summary>
        /// <param name="radius">the radius (as defined by <see cref="Radius"/> of all hexes.</param>
        protected Geometry(float radius)
        {
                this.Radius = radius;
                this.Apothem = (Sqrt3 / 2) * Radius;
        }

        /// <summary>
        /// Factory method for radius values
        /// </summary>
        /// <param name="radius">the radius (distance from center to a vertex)</param>
        /// <returns>A new geometry with the provided scale</returns>
        public static Geometry WithRadius(float radius)
        {
            return new Geometry(radius);
        }

        /// <summary>
        /// Factory method for apothem values
        /// </summary>
        /// <param name="apothem">the apothem (distance from center to an edge normal)</param>
        /// <returns>A new geometry with the provided scale</returns>
        public static Geometry WithApothem(float apothem)
        {
            return new Geometry(apothem / (Sqrt3 / 2));
        }

        /// <summary>
        /// The square root of three.
        ///
        /// Computed using Unity <see cref="Mathf"/>'s square root function to ensure numerical consistency.
        /// </summary>
        public static readonly float Sqrt3 = Mathf.Sqrt(3f);

        /// <summary>
        /// The radius (in geometric units) of a circumcircle (circle that touches all points of the hex)
        /// </summary>
        public readonly float Radius;

        /// <summary>
        /// The distance from the center to an edge.
        ///
        /// Defined as <code>(Math.Sqrt(3) / 2) * Radius</code>
        /// </summary>
        public readonly float Apothem;

        /// <summary>
        /// Returns the cartisian vertex at the center of a hex.
        /// </summary>
        /// <param name="hex">The hex </param>
        /// <returns>A unity style 3space vertex that is the center of the given hex</returns>
        public Vector3 CenterVertexAt(CubicCoordinate hex)
        {
            (int p, int q) axial = hex.AsAxial();
            var x = Radius * (3f / 2 * axial.p);
            var z = Radius * (Sqrt3 / 2 * axial.p + Sqrt3 * axial.q);
            return new Vector3(x, 0, z);
        }

        /// <summary>
        /// The vertices that describe the hex
        /// </summary>
        /// <param name="hex">The hex to generate the vertices for<The /param>
        /// <returns>An array of 7 vertices.  Element 0 is always the center.  Elements 1-6 are the outer vertices in clockwise linear order.</returns>
        public Vector3[] MeshVerticesAt(CubicCoordinate hex)
        {
            Vector3 center = CenterVertexAt(hex);

            float x = center.x, y = center.y, z = center.z;

            return new[]
            {
                center,
                new Vector3(x - Radius    , y,           z),
                new Vector3(x - Radius / 2, y, z + Apothem),
                new Vector3(x + Radius / 2, y, z + Apothem),
                new Vector3(x + Radius    , y,           z),
                new Vector3(x + Radius / 2, y, z - Apothem),
                new Vector3(x - Radius / 2, y, z - Apothem),
            };
        }

        public static readonly float[] FacingAngles = new float[]
        {
            0f, // fwd
            300f, // fwdrht
            240f, // bckrht
            180f, // bck
            120f, // bckleft
            60f, // fwdlft
        };

        public static readonly Quaternion[] FacingQuaternions = FacingAngles.Map(a => Quaternion.Euler(0, a, 0));

        public Quaternion Face(Direction direction)
        {
            return FacingQuaternions[(int)direction];
        }


        public override string ToString()
        {
            return $"Geometry(Radius: {Radius}, Apothem: {Apothem}";
        }
    }

    /// <summary>
    /// Utility class for finding the hex under the mouse
    /// </summary>
    public static class HexCellAt
    {

        /// <summary>
        /// Gets the cell under the mouse's current position.
        /// </summary>
        /// <param name="apply">Optional action to apply to the detected hex</param>
        /// <returns>The cell under the mouse location</returns>
        public static Cell MouseHover(Action<Cell> apply = null)
        {
            Cell o = null;
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 200.0f))
            {
                o = hit.collider.gameObject.GetComponent<Cell>();
                apply?.Invoke(o);
            }
            return o;
        }

        /// <summary>
        /// Gets the cell under the mouse's current position if the specified button was clicked in the most recent frame.
        /// </summary>
        /// <param name="apply">Optional action to apply to the detected hex</param>
        /// <param name="button">The button number to detect (defaults to 0)</param>
        /// <returns>The cell under the mouse click location</returns>
        public static Cell MouseClick(Action<Cell> apply = null, int button = 0)
        {
            if (Input.GetMouseButton(button))
            {
                return MouseHover(apply);
            }
            return null;
        }
    }
}