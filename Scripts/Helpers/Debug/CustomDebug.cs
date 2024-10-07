using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using Helpers.UnityInterfaces;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik, Aaron Duffey, and Zachary Gross

/// <summary>
/// Customly wrapped defined calls for Unity's Debug class.
/// </summary>
public static class CustomDebug
{
    #region Log variants
    /// <summary>
    /// Debug.Log that does not print if isDebugging is false.
    /// Also only printed in editor and in debug builds.
    /// </summary>
    public static void Log(object message, bool isDebugging)
    {
        if (!isDebugging) { return; }
        if (!Debug.isDebugBuild) { return; }
        Debug.Log(message);
    }

    /// <summary>
    /// Debug.LogWarning that is only printed in editor and in debug builds.
    /// </summary>
    /// <param name="message"></param>
    public static void LogWarning(object message)
    {
        if (!Debug.isDebugBuild) { return; }
        Debug.LogWarning(message);
    }
    public static void LogWarningForComponent(object message, Component requester)
    {
        LogWarning($"{requester.name}'s {requester.GetType().Name} {message}");
    }

    /// <summary>
    /// Debug.LogError that is only printed in editor and in debug builds.
    /// </summary>
    /// <param name="message"></param>
    public static void LogError(object message)
    {
        if (!Debug.isDebugBuild) { return; }
        Debug.LogError(message);
    }
    public static void LogErrorForComponent(object message, Component requester)
    {
        LogError($"{requester.name}'s {requester.GetType().Name} {message}");
    }

    public static void LogForComponent(object message, Component requester,
        bool isDebugging)
    {
        Log($"{requester.name}'s {requester.GetType().Name} {message}",
            isDebugging);
    }
    public static void LogForComponentFullPath(object message, Component requester,
        bool isDebugging)
    {
        Log($"{requester.gameObject.GetFullName()}'s {requester.GetType().Name} {message}",
            isDebugging);
    }
    public static void LogForObject(object message, object requester,
        bool isDebugging)
    {
        Log($"{requester.GetType().Name}'s {message}",
            isDebugging);
    }
    #endregion  Log variants

    #region LogForContainerGameObjects
    /// <summary>
    /// Logs the input message after displaying the contents of a GameObject List.
    /// <code>For example, this function will print out:
    /// [NAME OF THE LIST] contains [x, y, z, ...].
    /// [YOUR MESSAGE HERE].</code>
    /// </summary>
    /// <param name="gameObjects">List of GameObjects to Log.</param>
    /// <param name="isDebugging">Whether this function runs (only runs when debugging is on).</param>
    public static void LogForContainerGameObjects(object message, List<GameObject> gameObjects, bool isDebugging)
    {
        string temp_objNames = "";
        foreach (GameObject go in gameObjects)
        {
            temp_objNames += go + ", ";
        }
        Log($"{gameObjects} contains {temp_objNames}. \n {message}", isDebugging);
    }
    /// <summary>
    /// Logs the input message after displaying the contents of a GameObject IReadOnlyList.
    /// <code>For example, this function will print out:
    /// [NAME OF THE LIST] contains [x, y, z, ...].
    /// [YOUR MESSAGE HERE].</code>
    /// </summary>
    /// <param name="gameObjects">IReadOnlyList of GameObjects to Log.</param>
    /// <param name="isDebugging">Whether this function runs (only runs when debugging is on).</param>
    public static void LogForContainerGameObjects(object message, IReadOnlyList<GameObject> gameObjects, bool isDebugging)
    {
        string temp_objNames = "";
        foreach (GameObject go in gameObjects)
        {
            temp_objNames += go + ", ";
        }
        Log($"{gameObjects} contains {temp_objNames}. \n {message}", isDebugging);
    }
    /// <summary>
    /// Logs the input message after displaying the contents of a GameObject array.
    /// <code>For example, this function will print out:
    /// [NAME OF THE ARRAY contains [x, y, z, ...].
    /// [YOUR MESSAGE HERE].</code>
    /// </summary>
    /// <param name="message">The message displayed in the log after the contents of the array.</param>
    /// <param name="gameObjects">List of GameObjects to Log.</param>
    /// <param name="isDebugging">Whether this function runs (only runs when debugging is on).</param>
    public static void LogForContainerGameObjects(object message, GameObject[] gameObjects, bool isDebugging)
    {
        string temp_objNames = "";
        foreach (GameObject go in gameObjects)
        {
            temp_objNames += go + ", ";
        }
        Log($"{gameObjects} contains {temp_objNames}. \n {message}", isDebugging);
    }
    #endregion LogForContainerGameObjects
    #region LogForContainerObjects
    /// <summary>
    /// Logs the input message after displaying the contents of an object List.
    /// <code>For example, this function will print out:
    /// [NAME OF THE LIST] contains [x, y, z, ...].
    /// [YOUR MESSAGE HERE].</code>
    /// </summary>
    /// <param name="objects">List of objects to Log.</param>
    /// <param name="isDebugging">Whether this function runs (only runs when debugging is on).</param>
    public static void LogForContainerObjects(object message, List<object> objects, bool isDebugging)
    {
        string temp_objNames = "";
        foreach (object obj in objects)
        {
            temp_objNames += obj + ", ";
        }
        Log($"{objects} contains {temp_objNames}. \n {message}", isDebugging);
    }
    /// <summary>
    /// Logs the input message after displaying the contents of an object array.
    /// <code>For example, this function will print out:
    /// [NAME OF THE ARRAY] contains [x, y, z, ...].
    /// [YOUR MESSAGE HERE].</code>
    /// </summary>
    /// <param name="objects">Array of objects to Log.</param>
    /// <param name="isDebugging">Whether this function runs (only runs when debugging is on).</param>
    public static void LogForContainerObjects(object message, object[] objects, bool isDebugging)
    {
        string temp_objNames = "";
        foreach (object obj in objects)
        {
            temp_objNames += obj + ", ";
        }
        Log($"{objects} contains {temp_objNames}. \n {message}", isDebugging);
    }
    #endregion LogForContainerObjects
    #region LogForContainerElements
    /// <summary>
    /// Logs the input message after displaying the contents of a T array.
    /// <code>For example, this function will print out:
    /// [NAME OF THE ARRAY] contains [x, y, z, ...].
    /// [YOUR MESSAGE HERE].</code>
    /// </summary>
    /// <param name="objects">Array of T to Log.</param>
    /// <param name="isDebugging">Whether this function runs (only runs when debugging is on).</param>
    public static void LogForContainerElements<T>(object message, T[] objects, bool isDebugging)
    {
        string temp_objNames = "";
        foreach (T obj in objects)
        {
            temp_objNames += obj + ", ";
        }
        Log($"{objects} contains {temp_objNames}. \n {message}", isDebugging);
    }
    /// <summary>
    /// Logs the input message after displaying the contents of a T List.
    /// <code>For example, this function will print out:
    /// [NAME OF THE LIST] contains [x, y, z, ...].
    /// [YOUR MESSAGE HERE].</code>
    /// </summary>
    /// <param name="objects">T List to Log.</param>
    /// <param name="isDebugging">Whether this function runs (only runs when debugging is on).</param>
    public static void LogForContainerElements<T>(object message, List<T> objects, bool isDebugging)
    {
        string temp_objNames = "";
        foreach (T obj in objects)
        {
            temp_objNames += obj + ", ";
        }
        Log($"{objects} contains {temp_objNames}. \n {message}", isDebugging);
    }
    /// <summary>
    /// Logs the input message after displaying the contents of a T IReadOnlyList.
    /// <code>For example, this function will print out:
    /// [NAME OF THE LIST] contains [x, y, z, ...].
    /// [YOUR MESSAGE HERE].</code>
    /// </summary>
    /// <param name="objects">T IReadOnlyList to Log.</param>
    /// <param name="isDebugging">Whether this function runs (only runs when debugging is on).</param>
    public static void LogForContainerElements<T>(object message, IReadOnlyList<T> objects, bool isDebugging)
    {
        string temp_objNames = "";
        foreach (T obj in objects)
        {
            temp_objNames += obj + ", ";
        }
        Log($"{objects} contains {temp_objNames}. \n {message}", isDebugging);
    }
    #endregion LogForContainerElements

    #region UnhandledEnum
    public static void UnhandledEnum<T>(T enumVal, string querier) where T : Enum
    {
        if (!Debug.isDebugBuild) { return; }
        Debug.LogError($"Unhandled enum of type {enumVal.GetType()} with value " +
            $"{enumVal} in {querier}");
    }
    public static void UnhandledEnum<T>(T enumVal, UnityEngine.Object querier)
        where T : Enum
    {
        if (!Debug.isDebugBuild) { return; }
        Debug.LogError($"Unhandled enum of type {enumVal.GetType()} with value " +
            $"{enumVal} in {querier.name}'s {querier.GetType().Name}");
    }
    public static void UnhandledEnum(int enumVal, string querier)
    {
        if (!Debug.isDebugBuild) { return; }
        Debug.LogError($"Unhandled enum of type {enumVal.GetType()} with value " +
            $"{enumVal} in {querier}");
    }
    public static void UnhandledValue(int enumVal, UnityEngine.Object querier)
    {
        if (!Debug.isDebugBuild) { return; }
        Debug.LogError($"Unhandled value {enumVal} " +
            $"in {querier.name}'s {querier.GetType().Name}");
    }
    public static void UnhandledValue(string strVal, string querier)
    {
        if (!Debug.isDebugBuild)
        { return; }
        Debug.LogError($"Unhandled value {strVal} in {querier}");
    }
    public static void UnhandledValue(string strVal, UnityEngine.Object querier)
    {
        UnhandledValue(strVal, $"{querier.name}'s {querier.GetType().Name}");
    }
    #endregion UnhandledEnum

    #region AssertComponentIsNotNull
    public static void AssertComponentIsNotNull<T>(T varToCheck,
        Component queryComp, GameObject getCompTarget) where T : Component
    {
        AssertComponentIsNotNull(varToCheck, typeof(T), $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}", getCompTarget.name);
    }
    public static void AssertComponentIsNotNull<T>(T varToCheck,
        Component queryComp) where T : Component
    {
        AssertComponentIsNotNull(varToCheck, $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}");
    }
    public static void AssertComponentIsNotNull<T>(T varToCheck,
        Type queryType) where T : Component
    {
        AssertComponentIsNotNull(varToCheck, queryType.Name);
    }
    public static void AssertComponentIsNotNull(object varToCheck,
        Type typeOfVar, Type queryType)
    {
        AssertComponentIsNotNull(varToCheck, typeOfVar, queryType.Name,
            queryType.Name);
    }
    public static void AssertComponentIsNotNull<T>(T varToCheck,
        string querierName) where T : Component
    {
        AssertComponentIsNotNull(varToCheck, typeof(T), querierName, querierName);
    }
    public static void AssertComponentIsNotNull(object varToCheck,
        Type typeOfVar, string querierName, string getCompTargetName)
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.IsNotNull(varToCheck, $"{querierName} expected to have " +
            $"{typeOfVar.Name} attached to {getCompTargetName} but " +
            $"none was found.");
    }
    #endregion AssertComponentIsNotNull

    #region AssertIComponentIsNotNull
    public static void AssertIComponentIsNotNull<T>(T varToCheck,
        Component queryComp) where T : IComponent
    {
        AssertIComponentIsNotNull(varToCheck, $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}");
    }
    public static void AssertIComponentIsNotNull<T>(T varToCheck,
        string querierName) where T : IComponent
    {
        AssertComponentIsNotNull(varToCheck, typeof(T), querierName, querierName);
    }
    #endregion AssertIComponentIsNotNull

    #region AssertComponentOnOtherIsNotNull
    public static void AssertComponentOnOtherIsNotNull<T>(T varToCheck,
        GameObject other, Component queryComp) where T : Component
    {
        AssertComponentOnOtherIsNotNull(varToCheck, other, $"{queryComp.name}'s {queryComp.GetType().Name}");
    }
    public static void AssertComponentOnOtherIsNotNull<T>(T varToCheck,
        GameObject other, string querierName) where T : Component
    {
        AssertComponentIsNotNull(varToCheck, typeof(T), querierName, other.name);
    }
    #endregion AssertComponentOnOtherIsNotNull

    #region AssertIComponentOnOtherIsNotNull
    public static void AssertIComponentOnOtherIsNotNull<T>(T varToCheck,
        GameObject other, Component queryComp) where T : IComponent
    {
        AssertIComponentOnOtherIsNotNull(varToCheck, other, $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}");
    }
    public static void AssertIComponentOnOtherIsNotNull<T>(T varToCheck,
        GameObject other, object querierObj) where T : IComponent
    {
        AssertIComponentOnOtherIsNotNull(varToCheck, other, 
            querierObj.GetType().Name);
    }
    public static void AssertIComponentOnOtherIsNotNull<T>(T varToCheck,
        GameObject other, string querierName) where T : IComponent
    {
        AssertComponentIsNotNull(varToCheck, typeof(T), querierName, other.name);
    }
    #endregion AssertIComponentOnOtherIsNotNull

    #region AssertComponentInChildrenIsNotNull
    public static void AssertComponentInChildrenIsNotNull<T>(T varToCheck,
    Component queryComp, GameObject getCompTarget) where T : Component
    {
        AssertComponentInChildrenIsNotNull(varToCheck, typeof(T), $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}", getCompTarget.name);
    }
    public static void AssertComponentInChildrenIsNotNull<T>(T varToCheck,
        Component queryComp) where T : Component
    {
        AssertComponentInChildrenIsNotNull(varToCheck, $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}");
    }
    public static void AssertComponentInChildrenIsNotNull<T>(T varToCheck,
        Type queryType) where T : Component
    {
        AssertComponentInChildrenIsNotNull(varToCheck, queryType.Name);
    }
    public static void AssertComponentInChildrenIsNotNull(object varToCheck,
        Type typeOfVar, Type queryType)
    {
        AssertComponentInChildrenIsNotNull(varToCheck, typeOfVar, queryType.Name,
            queryType.Name);
    }
    public static void AssertComponentInChildrenIsNotNull<T>(T varToCheck,
        string querierName) where T : Component
    {
        AssertComponentInChildrenIsNotNull(varToCheck, typeof(T), querierName, querierName);
    }
    public static void AssertComponentInChildrenIsNotNull(object varToCheck,
        Type typeOfVar, string querierName, string getCompTargetName)
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.IsNotNull(varToCheck, $"{querierName} expected to have " +
            $"{typeOfVar.Name} attached to {getCompTargetName} or its children but " +
            $"none was found.");
    }
    #endregion AssertComponentInChildrenIsNotNull

    #region AssertComponentInChildrenIsNotNull
    public static void AssertIComponentInChildrenIsNotNull<T>(T varToCheck,
    Component queryComp, GameObject getCompTarget) where T : IComponent
    {
        AssertIComponentInChildrenIsNotNull(varToCheck, typeof(T), $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}", getCompTarget.name);
    }
    public static void AssertIComponentInChildrenIsNotNull<T>(T varToCheck,
        Component queryComp) where T : IComponent
    {
        AssertIComponentInChildrenIsNotNull(varToCheck, $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}");
    }
    public static void AssertIComponentInChildrenIsNotNull<T>(T varToCheck,
        Type queryType) where T : IComponent
    {
        AssertIComponentInChildrenIsNotNull(varToCheck, queryType.Name);
    }
    public static void AssertIComponentInChildrenIsNotNull(object varToCheck,
        Type typeOfVar, Type queryType)
    {
        AssertIComponentInChildrenIsNotNull(varToCheck, typeOfVar, queryType.Name,
            queryType.Name);
    }
    public static void AssertIComponentInChildrenIsNotNull<T>(T varToCheck,
        string querierName) where T : IComponent
    {
        AssertIComponentInChildrenIsNotNull(varToCheck, typeof(T), querierName, querierName);
    }
    public static void AssertIComponentInChildrenIsNotNull(object varToCheck,
        Type typeOfVar, string querierName, string getCompTargetName)
    {
        AssertComponentInChildrenIsNotNull(varToCheck, typeOfVar, querierName, getCompTargetName);
    }
    #endregion AssertComponentInChildrenIsNotNull

    #region AssertComponentInChildrenOnOtherIsNotNull
    public static void AssertComponentInChildrenOnOtherIsNotNull<T>(T varToCheck,
        GameObject other, UnityEngine.Object queryObj) where T : Component
    {
        AssertComponentInChildrenIsNotNull(varToCheck, typeof(T), $"{queryObj.name}'s " +
            $"{queryObj.GetType().Name}", other.name);
    }
    public static void AssertComponentInChildrenOnOtherIsNotNull<T>(T varToCheck,
    GameObject other, string queryName) where T : Component
    {
        AssertComponentInChildrenIsNotNull(varToCheck, typeof(T), queryName, other.name);
    }
    #endregion AssertComponentInChildrenOnOtherIsNotNull

    #region AssertIComponentInChildrenOnOtherIsNotNull
    public static void AssertIComponentInChildrenOnOtherIsNotNull<T>(T varToCheck,
        GameObject other, Component queryComp) where T : IComponent
    {
        AssertComponentInChildrenIsNotNull(varToCheck, typeof(T), $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}", other.name);
    }
    #endregion AssertIComponentInChildrenOnOtherIsNotNull

    #region AssertComponentInParentIsNotNull
    public static void AssertComponentInParentIsNotNull<T>(T varToCheck,
    Component queryComp, GameObject getCompTarget) where T : Component
    {
        AssertComponentInParentIsNotNull(varToCheck, typeof(T), $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}", getCompTarget.name);
    }
    public static void AssertComponentInParentIsNotNull<T>(T varToCheck,
        Component queryComp) where T : Component
    {
        AssertComponentInParentIsNotNull(varToCheck, $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}");
    }
    public static void AssertComponentInParentIsNotNull<T>(T varToCheck,
        Type queryType) where T : Component
    {
        AssertComponentInParentIsNotNull(varToCheck, queryType.Name);
    }
    public static void AssertComponentInParentIsNotNull(object varToCheck,
        Type typeOfVar, Type queryType)
    {
        AssertComponentInParentIsNotNull(varToCheck, typeOfVar, queryType.Name,
            queryType.Name);
    }
    public static void AssertComponentInParentIsNotNull<T>(T varToCheck,
        string querierName) where T : Component
    {
        AssertComponentInParentIsNotNull(varToCheck, typeof(T), querierName, querierName);
    }
    public static void AssertComponentInParentIsNotNull(object varToCheck,
        Type typeOfVar, string querierName, string getCompTargetName)
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.IsNotNull(varToCheck, $"{querierName} expected to have " +
            $"{typeOfVar.Name} attached to {getCompTargetName} or its parents but " +
            $"none was found.");
    }
    #endregion AssertComponentInParentIsNotNull

    #region AssertIComponentInParentIsNotNull
    public static void AssertIComponentInParentIsNotNull<T>(T varToCheck,
        Component queryComp) where T : IComponent
    {
        AssertComponentInParentIsNotNull(varToCheck, typeof(T), $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}", $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}");
    }
    public static void AssertIComponentInParentOnOtherIsNotNull<T>(T varToCheck,
        GameObject other, Component queryComp) where T : IComponent
    {
        AssertComponentInParentIsNotNull(varToCheck, typeof(T), $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}", $"{other.name}");
    }
    #endregion AssertIComponentInParentIsNotNull

    #region AssertSingletonMonoBehaviourIsNotNull
    public static void AssertSingletonMonoBehaviourIsNotNull<T>(T varToCheck,
        Component queryComp) where T : MonoBehaviour
    {
        AssertSingletonMonoBehaviourIsNotNull(varToCheck,
            $"{queryComp.name}'s {queryComp.GetType().Name}");
    }
    public static void AssertSingletonMonoBehaviourIsNotNull<T>(T varToCheck,
        object queryObj) where T : MonoBehaviour
    {
        AssertSingletonMonoBehaviourIsNotNull(varToCheck,
            $"{queryObj.GetType().Name}");
    }
    public static void AssertSingletonMonoBehaviourIsNotNull<T>(T varToCheck,
        string querierName) where T : MonoBehaviour
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.IsNotNull(varToCheck, $"{querierName} expected the " +
            $"singleton {typeof(T).Name} " +
            $"to exist in the scene, but none was found");
    }
    #endregion AssertSingletonMonoBehaviourIsNotNull

    #region AssertSingletonIsNotNull
    public static void AssertSingletonIsNotNull<T>(T varToCheck,
        Component queryComp) where T : class
    {
        AssertSingletonIsNotNull(varToCheck, 
            $"{queryComp.name}'s {queryComp.GetType().Name}");
    }
    public static void AssertSingletonIsNotNull<T>(T varToCheck,
        object queryObj) where T : class
    {
        AssertSingletonIsNotNull(varToCheck, queryObj.GetType().Name);
    }
    public static void AssertSingletonIsNotNull<T>(T varToCheck,
        string querierName) where T : class
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.IsNotNull(varToCheck, $"{querierName} expected the " +
            $"singleton {typeof(T).Name} to exist.");
    }
    #endregion AssertSingletonIsNotNull

    #region AssertGameObjectWithTagIsNotNull
    public static void AssertGameObjectWithTagIsNotNull(GameObject retrievedObj, string tag, Component queryComp)
    {
        AssertGameObjectWithTagIsNotNull(retrievedObj, tag, $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}");
    }
    public static void AssertGameObjectWithTagIsNotNull(GameObject retrievedObj, string tag, object queryObj)
    {
        AssertGameObjectWithTagIsNotNull(retrievedObj, tag, queryObj.GetType());
    }
    public static void AssertGameObjectWithTagIsNotNull(GameObject retrievedObj, string tag, Type queryType)
    {
        AssertGameObjectWithTagIsNotNull(retrievedObj, tag, queryType.Name);
    }
    public static void AssertGameObjectWithTagIsNotNull(GameObject retrievedObj, string tag, string querierName)
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.IsNotNull(retrievedObj, $"{querierName} expected an object with the " +
            $"'{tag}' tag to be in the scene.");
    }
    #endregion AssertGameObjectWithTagIsNotNull

    #region AssertListsAreSameSize
    public static void AssertListsAreSameSize<T, G>(IReadOnlyCollection<T> listOne,
        IReadOnlyCollection<G> listTwo, string listOneName, string listTwoName,
        Component queryComp)
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.AreEqual(listOne.Count, listTwo.Count, $"{listOneName} and " +
            $"{listTwoName} must be the same exact length but are of different " +
            $"lengths {listOne.Count} and {listTwo.Count} respectively for " +
            $"{queryComp.name}'s {queryComp.GetType().Name}.");
    }

    public static void AssertListIsSize<T>(IReadOnlyCollection<T> listOne,
        string listOneName, int expectedSize, Component queryComp)
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.AreEqual(listOne.Count, expectedSize, $"{listOneName} is expected " +
            $"to have {expectedSize} elements but instead had {listOne.Count} " +
            $"for {queryComp.name}'s {queryComp.GetType().Name}.");
    }
    #endregion AssertListsAreSameSize

    #region AssertIndexIsInRange
    public static void AssertIndexIsInRange<T>(int index,
        IReadOnlyCollection<T> collection, Component querierComp)
    {
        AssertIndexIsInRange(index, collection,
            $"{querierComp.name}'s {querierComp.GetType().Name}");
    }
    public static void AssertIndexIsInRange<T>(int index,
        IReadOnlyCollection<T> collection, object querierObj)
    {
        AssertIndexIsInRange(index, collection,
            $"{querierObj.GetType().Name}");
    }
    public static void AssertIndexIsInRange<T>(int index,
        IReadOnlyCollection<T> collection, string querierName)
    {
        AssertIndexIsInRange(index, 0, collection.Count, querierName);
    }
    /// <param name="lowerBound">Inclusive</param>
    /// <param name="upperBound">Non-inclusive</param>
    public static void AssertIndexIsInRange<G>(int index,
        int lowerBound, int upperBound, G querierComp) where G : Component
    {
        AssertIndexIsInRange(index, lowerBound, upperBound,
            $"{querierComp.name}'s {querierComp.GetType().Name}");
    }
    /// <param name="lowerBound">Inclusive</param>
    /// <param name="upperBound">Non-inclusive</param>
    public static void AssertIndexIsInRange(int index,
        int lowerBound, int upperBound, string querierName)
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.IsTrue(index >= lowerBound && index < upperBound, $"index {index} " +
            $"out of bounds for {querierName}. Expected to be in range " +
            $"[0, {upperBound - 1}]");
    }
    #endregion AssertIndexIsInRange

    #region AssertSerializeFieldIsNotNull
    public static void AssertSerializeFieldIsNotNull(object varToCheck,
        string nameOfVar, Component queryComp)
    {
        AssertSerializeFieldIsNotNull(varToCheck, nameOfVar, $"{queryComp.name}'s {queryComp.GetType().Name}");
    }
    public static void AssertSerializeFieldIsNotNull<T>(T varToCheck,
        string nameOfVar, Component queryComp) where T : UnityEngine.Object
    {
        AssertSerializeFieldIsNotNull(varToCheck, nameOfVar, $"{queryComp.name}'s {queryComp.GetType().Name}");
    }
    public static void AssertSerializeFieldIsNotNull<T>(T varToCheck, string nameOfVar, object querierObj) where T : UnityEngine.Object
    {
        AssertSerializeFieldIsNotNull(varToCheck, nameOfVar, querierObj.GetType().Name);
    }
    public static void AssertSerializeFieldIsNotNull(object varToCheck,
        string nameOfVar, object querierObj)
    {
        AssertSerializeFieldIsNotNull(varToCheck, nameOfVar, querierObj.GetType().Name);
    }
    public static void AssertSerializeFieldIsNotNull<T>(T varToCheck,
        string nameOfVar, string querierName) where T : UnityEngine.Object
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.IsNotNull(varToCheck, $"{querierName} expected to have {nameOfVar}" +
            $" ({typeof(T).Name}) serialized but none was specified");
    }
    public static void AssertSerializeFieldIsNotNull(object varToCheck,
        string nameOfVar, string querierName)
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.IsNotNull(varToCheck, $"{querierName} expected to have {nameOfVar}" +
            $" serialized but it was specified");
    }
    #endregion AssertSerializeFieldIsNotNull

    #region AssertIsTrue variants
    public static void AssertIsTrueForComponent(bool condition, string expectation,
        Component queryComp)
    {
        AssertIsTrue(condition, expectation,
            $"{queryComp.name}'s {queryComp.GetType().Name}");
    }
    public static void AssertIsTrueForObject(bool condition, string expectation,
        UnityEngine.Object queryComp)
    {
        AssertIsTrue(condition, expectation,
            $"{queryComp.name}'s {queryComp.GetType().Name}");
    }
    public static void AssertIsTrueForObj(bool condition, string expectation,
        object queryObj)
    {
        AssertIsTrue(condition, expectation, $"{queryObj.GetType().Name}");
    }
    public static void AssertIsTrue(bool condition, string expectation,
        string querierName)
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.IsTrue(condition, $"{querierName} expected {expectation}");
    }
    #endregion AssertIsTrue variants

    #region AssertEnumIs
    public static void AssertEnumIs<T>(T actualValue, T expectedValue,
        Component queryComp) where T : Enum
    {
        AssertIsTrue(actualValue.Equals(expectedValue), $"enum value ({actualValue}) to be {expectedValue}.", $"{queryComp.name}'s {queryComp.GetType().Name}");
    }
    public static void AssertEnumIs<T>(T actualValue, T expectedValue,
        UnityEngine.Object queryComp) where T : Enum
    {
        AssertIsTrue(actualValue.Equals(expectedValue), $"enum value ({actualValue}) to be {expectedValue}.", $"{queryComp.name}'s {queryComp.GetType().Name}");
    }
    public static void AssertEnumIs<T>(T actualValue, T expectedValue,
        object queryObj) where T : Enum
    {
        AssertIsTrue(actualValue.Equals(expectedValue), $"enum value ({actualValue}) to be {expectedValue}.", $"{queryObj.GetType().Name}");
    }
    public static void AssertEnumIs<T>(T actualValue, T expectedValue,
        string querierName) where T : Enum
    {
        AssertIsTrue(actualValue.Equals(expectedValue), $"enum value ({actualValue}) to be {expectedValue}.", querierName);
    }
    #endregion AssertEnumIs

    #region ThrowAssertionFail
    public static void ThrowAssertionFail(string expectation,
        Component querierComp)
    {
        ThrowAssertionFail(expectation, $"{querierComp.name}'s {querierComp.GetType().Name}");
    }
    public static void ThrowAssertionFail(string errorMessage,
        object queryObj)
    {
        ThrowAssertionFail(errorMessage, queryObj.GetType().Name);
    }
    public static void ThrowAssertionFail(string expectation,
        string querierName)
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.IsTrue(false, $"{querierName} expected {expectation}");
    }
    #endregion ThrowAssertionFail


    #region DrawSquare
    public static void DrawSquare(Vector2 center, Vector2 size, bool isDebugging)
        => DrawSquare(center, size, Color.white, 0.0f, isDebugging);
    public static void DrawSquare(Vector2 center, Vector2 size, Color color, bool isDebugging)
        => DrawSquare(center, size, color, 0.0f, isDebugging);
    public static void DrawSquare(Vector2 center, Vector2 size, Color color, float duration,
        bool isDebugging, bool breakAfterDraw = false)
    {
        if (!isDebugging)
        { return; }
        if (!Debug.isDebugBuild)
        { return; }

        Vector2 t_halfSize = size * 0.5f;
        Vector2 t_botLeft = center - t_halfSize;
        Vector2 t_topRight = center + t_halfSize;
        Vector2 t_botRight = new Vector2(t_topRight.x, t_botLeft.y);
        Vector2 t_topLeft = new Vector2(t_botLeft.x, t_topRight.y);

        Debug.DrawLine(t_botLeft, t_botRight, color, duration);
        Debug.DrawLine(t_botRight, t_topRight, color, duration);
        Debug.DrawLine(t_topRight, t_topLeft, color, duration);
        Debug.DrawLine(t_topLeft, t_botLeft, color, duration);

        if (breakAfterDraw)
        {
            Debug.Break();
        }
    }
    #endregion DrawSquare

    #region DrawCrossHair
    public static void DrawCrossHair(Vector2 center, float size, bool isDebugging) => DrawCrossHair(center, new Vector2(size, size), isDebugging);
    public static void DrawCrossHair(Vector2 center, float size, Color color, bool isDebugging) => DrawCrossHair(center, new Vector2(size, size), color, isDebugging);
    public static void DrawCrossHair(Vector2 center, float size, Color color, float duration, bool isDebugging, bool breakAfterDraw = false) => DrawCrossHair(center, new Vector2(size, size), color, duration, isDebugging, breakAfterDraw);
    public static void DrawCrossHair(Vector2 center, Vector2 size, bool isDebugging)
        => DrawCrossHair(center, size, Color.white, 0.0f, isDebugging);
    public static void DrawCrossHair(Vector2 center, Vector2 size, Color color, bool isDebugging)
        => DrawCrossHair(center, size, color, 0.0f, isDebugging);
    public static void DrawCrossHair(Vector2 center, Vector2 size, Color color, float duration,
        bool isDebugging, bool breakAfterDraw = false)
    {
        if (!isDebugging) { return; }
        if (!Debug.isDebugBuild) { return; }

        Vector2 t_halfSize = size * 0.5f;
        Debug.DrawRay(center, new Vector2(0.0f, -t_halfSize.y), color, duration);
        Debug.DrawRay(center, new Vector2(0.0f, t_halfSize.y), color, duration);
        Debug.DrawRay(center, new Vector2(-t_halfSize.x, 0.0f), color, duration);
        Debug.DrawRay(center, new Vector2(t_halfSize.x, 0.0f), color, duration);

        if (breakAfterDraw)
        {
            Debug.Break();
        }
    }
    #endregion DrawCrossHair

    #region DrawLine
    public static void DrawLine(Vector2 start, Vector2 end, Color color, float duration, bool depthTest, bool isDebugging)
    {
        if (!isDebugging) { return; }
        Debug.DrawLine(start, end, color, duration, depthTest);
    }
    public static void DrawLine(Vector2 start, Vector2 end, Color color, float duration, bool isDebugging)
    {
        if (!isDebugging) { return; }
        Debug.DrawLine(start, end, color, duration);
    }
    public static void DrawLine(Vector2 start, Vector2 end, Color color, bool isDebugging)
    {
        if (!isDebugging) { return; }
        Debug.DrawLine(start, end, color);
    }
    public static void DrawLine(Vector2 start, Vector2 end, bool isDebugging)
    {
        if (!isDebugging) { return; }
        Debug.DrawLine(start, end);
    }
    #endregion DrawLine

    #region DrawRay
    public static void DrawRay(Vector2 start, Vector2 dir, Color color, float duration, bool depthTest, bool isDebugging)
    {
        if (!isDebugging) { return; }
        Debug.DrawRay(start, dir, color, duration, depthTest);
    }
    public static void DrawRay(Vector2 start, Vector2 dir, Color color, float duration, bool isDebugging)
    {
        if (!isDebugging) { return; }
        Debug.DrawRay(start, dir, color, duration);
    }
    public static void DrawRay(Vector2 start, Vector2 dir, Color color, bool isDebugging)
    {
        if (!isDebugging) { return; }
        Debug.DrawRay(start, dir, color);
    }
    public static void DrawRay(Vector2 start, Vector2 dir, bool isDebugging)
    {
        if (!isDebugging) { return; }
        Debug.DrawRay(start, dir);
    }
    #endregion DrawRay

    #region DrawCircle 
    public static void DrawCircle(Vector2 center, float radius, int segments, Color color, bool isDebugging)
    {
         if (!isDebugging) { return; }

        // If either radius or number of segments are less or equal to 0, skip drawing
        if (radius <= 0.0f || segments <= 0)
        {
            return;
        }

        // Single segment of the circle covers (360 / number of segments) degrees
        float t_angleStep = 360.0f / segments;

        // Result is multiplied by Mathf.Deg2Rad constant which transforms degrees to radians
        // which are required by Unity's Mathf class trigonometry methods

        t_angleStep *= Mathf.Deg2Rad;

        // lineStart and lineEnd variables are declared outside of the following for loop
        Vector2 t_lineStart = Vector2.zero;
        Vector2 t_lineEnd = Vector2.zero;

        for (int i = 0; i < segments; i++)
        {
            // Line start is defined as starting angle of the current segment (i)
            t_lineStart.x = Mathf.Cos(t_angleStep * i);
            t_lineStart.y = Mathf.Sin(t_angleStep * i);

            // Line end is defined by the angle of the next segment (i+1)
            t_lineEnd.x = Mathf.Cos(t_angleStep * (i + 1));
            t_lineEnd.y = Mathf.Sin(t_angleStep * (i + 1));

            // Results are multiplied so they match the desired radius
            t_lineStart *= radius;
            t_lineEnd *= radius;

            // Results are offset by the desired position/origin 
            t_lineStart += center;
            t_lineEnd += center;

            // Points are connected using DrawLine method and using the passed color
            DrawLine(t_lineStart, t_lineEnd, color, isDebugging);
        }
    }
    #endregion DrawCircle


    #region RunDebugFunction
    public static void RunDebugFunction(Action function)
    {
        if (!Debug.isDebugBuild) { return; }
        function?.Invoke();
    }
    public static void RunDebugFunction(Action function, bool isDebugging)
    {
        if (!isDebugging) { return; }
        RunDebugFunction(function);
    }
    #endregion RunDebugFunction

    public static void Break(bool isDebugging = true)
    {
        if (!isDebugging) { return; }
        if (!Debug.isDebugBuild) { return; }
        Debug.Break();
    }

    public static string GetComponentDebugName(Component queryComp)
    {
        return $"{queryComp.name}'s {queryComp.GetType().Name}";
    }
    public static string GetComponentDebugName(IComponent queryComp)
    {
        return $"{queryComp.name}'s {queryComp.GetType().Name}";
    }
}
