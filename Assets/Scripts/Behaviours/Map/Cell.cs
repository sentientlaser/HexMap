using System;
using UnityEngine;

// makes things a little easier to read.
using CubicCoordinate = System.ValueTuple<int, int, int>;

namespace Map
{
    /// <summary>
    /// Data for a cell gameobject.
    /// </summary>
    [ExecuteInEditMode]
    public class Cell : MonoBehaviour
    {

        #region Fields Exposed To Unity Editor
        public GameObject occupant; // TODO: this might need a different behaviour.
        public Vector3Int CoordVector; // this is only used to serialise the coodinates.
        #endregion

        #region Exposed Data
        /// <summary>
        /// the _logical_ coordinates of this cell (the graticule r,s,t) values.
        /// See <see cref="CoordinateExtensionMethods"/> for an explanation of how the <c>(int,int,int)</c> tuples work
        /// </summary>
        public (int r, int s, int t) Coordinates;

        /// <summary>
        /// the _cartesian_ coordinates of this cell (the 3d graphics x,y,z values)
        /// </summary>
        public Vector3 CenterVertex
        {
            get => transform.position;
        }
        #endregion

        #region Type casts, juggling
        /// <summary>
        /// All math operations are done to the CubicCoordinate type, so this allows unboxing to <c>(int, int, int)</c>
        /// </summary>
        /// <remarks>
        /// Note that this is ONLY a unidirectional cast.  You can't cast from <c>(int, int, int)</c> to <c>HexCell</c> without knowing the graticule the cell should come from.
        /// Since there's no implicit evidence system in C#, this is not possible.
        /// However the indexer <see cref="Graticule.this[CubicCoordinate]"/> represents the 'inverse' (it's not injective so it's also not a proper inverse) of this cast.
        /// </remarks>
        /// <param name="that">the cell to unbox</param>
        public static implicit operator CubicCoordinate(Cell that) => that.Coordinates;
        #endregion


        #region Unity Lifecycle Methods
        void Awake()
        {
            Coordinates = (CoordVector.x, CoordVector.y, CoordVector.z);
            var center = Geometry.Default.CenterVertexAt(Coordinates);
            transform.position = center;
        }
        #endregion

        #region Unity Events
        private void OnMouseEnter()
        {
            FindObjectOfType<Caret.CaretHoverSelector>().Location = this;
        }

        private void OnMouseDown()
        {
            if (Input.GetMouseButton(0))
            {
                FindObjectOfType<Caret.CaretClickSelector>().Location = this;
            }
        }
        #endregion


        public override string ToString()
        {
            return "Hex" + Coordinates;
        }

        /// <remarks>
        /// TODO: assets should be handled as bundles, this is just Quick and Dirty for testing.
        /// </remarks>
        #region Assets
        public static class Materials
        {
            public static readonly Lazy<Material>  // TODO: mixing lazies with unity's lifecycle model may cause issues later
                normalHexMat = new Lazy<Material>(() => Resources.Load<Material>("Materials/CellMaterials/CellMaterialPlain")),
                dotHexMat = new Lazy<Material>(() => Resources.Load<Material>("Materials/CellMaterials/CellMaterialCenterDot")),
                blockedHexMat = new Lazy<Material>(() => Resources.Load<Material>("Materials/CellMaterials/CellMaterialBlocked"));

            public static Material NormalHexMat
            {
                get => normalHexMat.Value;
            }
            public static Material DotHexMat
            {
                get => dotHexMat.Value;
            }
            public static Material BlockedHexMat
            {
                get => blockedHexMat.Value;
            }
        }

        public static readonly Lazy<GameObject> BaseModel = new Lazy<GameObject>(() => Resources.Load<GameObject>("Models/Cell"));

        #endregion
    }
}
