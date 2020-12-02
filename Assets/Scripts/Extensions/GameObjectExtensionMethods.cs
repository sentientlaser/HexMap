using UnityEngine;
using System;

/// <summary>
/// Taken from the <see href="https://wiki.unity3d.com/index.php/GetOrAddComponent">Unity Wiki article</see> But heavily modified.
/// </summary>
static public class GameObjectExtensionMethods
{
    /// <summary>
    /// Returns the component of Type type. If one doesn't already exist on the GameObject it will be added.
    /// </summary>
    /// <typeparam name="T">The type of Component to return.</typeparam>
    /// <param name="gameObject">The GameObject this Component is attached to.</param>
    /// <returns>Component</returns>
    static public T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
    }

    /// <summary>
    /// Sets a material to a gameobject
    /// </summary>
    /// <param name="gameObject">the object to apply the material to</param>
    /// <param name="material">the material to apply</param>
    /// <returns>the object modified, to allow fluent expressions</returns>
    public static GameObject SetMaterial(this GameObject gameObject, Material material)
    {
        gameObject.GetComponent<Renderer>().material = material;
        return gameObject;
    }

    /// <summary>
    /// Adds a component, but applies an init method
    /// </summary>
    /// <remarks>
    /// WARNING: init should happen in the "awake" method, but passing information down programattically is not well supported by unity.
    /// This method will potentially interrupt unity's threads.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameObject">the object to apply the </param>
    /// <param name="initAction">the action to use as an initialiser</param>
    /// <returns>The initialised object</returns>
    public static T AddComponentWithInit<T>(this GameObject gameObject, Action<T> initAction) where T : Component
    {
        bool oldState = gameObject.activeSelf;
        gameObject.SetActive(false);
        T comp = gameObject.AddComponent<T>();
        initAction?.Invoke(comp);
        gameObject.SetActive(oldState);
        return comp;
    }
}

