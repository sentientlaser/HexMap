using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Map;

namespace Actors
{
    [ExecuteInEditMode]
    public class BasicActor : SingleCellOccupantOrientable
    {
        public override void OnChange()
        {
        }

        // Start is called before the first frame update
        void Start()
        {
            quaternion = transform.rotation;
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = Location.transform.position;
            transform.rotation = quaternion * Geometry.Default.Face(direction);
        }

        
    }
}
