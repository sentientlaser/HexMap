using UnityEngine;

using static GameObjectExtensionMethods;
using static ArrayMonoids;

using CubicCoordinate = System.ValueTuple<int, int, int>;

namespace Caret
{
    /// <summary>
    /// Handles the detection of which hex the user has clicked on and moves the click caret there.
    /// </summary>
    public class CaretHoverSelector : Actors.SingleCellOccupant, SelectorType
    {

        #region Internal State Variables
        private Map.Graticule graticule;
        #endregion

        #region Unity Lifecycle Methods
        void Start()
        {
            graticule = Object.FindObjectOfType<Map.Graticule>();
        }

        void Update()
        {
           // Map.HexCellAt.MouseHover(apply: loc => Location = loc);
        }
        #endregion

        #region Update Logic
        /// <summary>
        /// Called after the position is updated.
        /// </summary>
        public override void OnChange()
        {
            transform.position = Location.transform.position;
        }
        #endregion

    }

}