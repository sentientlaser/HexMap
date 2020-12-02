using UnityEngine;

namespace Caret {
    /// <summary>
    /// Simple Interface to denote a selector behaviour
    /// </summary>
    public interface SelectorType
    {
        Map.Cell Location
        {
            get;
            set;
        }
    }
}