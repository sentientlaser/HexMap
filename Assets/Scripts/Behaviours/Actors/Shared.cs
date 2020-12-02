using UnityEngine;

using Map;

namespace Actors
{
    /// <summary>
    /// Defined a behaviour for gameobjects that logically occupy a hex.
    /// </summary>
    public interface Occupant
    {
        /// <summary>
        /// The cell this object is currently occupying
        /// </summary>
        Cell Location { get; set; }

        /// <summary>
        /// The cell this object was previously occupying (changed in case of moves, etc)
        /// </summary>
        Cell PreviousLocation { get; }

        /// <summary>
        /// This is called after the location values are updated
        /// </summary>
        void OnChange();
    }

    public abstract class SingleCellOccupant : MonoBehaviour, Occupant 
    {
        /// <summary>
        /// This is exposed to unity.
        /// </summary>
        public Cell location = null; // TODO: needs some kind of update behaviour.

        //private Cell actualLocation = null;
        public Cell Location
        {
            set
            {
                PreviousLocation = location;
                location = value;
                if (location != null && PreviousLocation != null)
                {
                    OnChange();
                }
            }
            get => location;
        }
        public Cell PreviousLocation
        {
            get;
            protected set;
        }

        public abstract void OnChange();
    }

    public abstract class SingleCellOccupantOrientable : SingleCellOccupant
    {
        public Direction direction;

        protected Quaternion quaternion;

    }

    //public abstract class MultipleCellOccupant : SingleCellOccupant, Occupant
    //{
    //    protected abstract (int, int, int)[] Shape { get;  }

    //    private Cell _occupiedCells;
    //    public Cell OccupiedCells
    //    {
    //        get;
    //        private set;
    //    }
    //}
}