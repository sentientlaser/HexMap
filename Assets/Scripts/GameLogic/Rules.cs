using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rules : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
namespace Model
{
    public abstract class Rule
    {
        public readonly string FlavourText;
    }

    public abstract class NamedRule
    {
        public readonly string Name;
    }
}