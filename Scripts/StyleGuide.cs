// Using statements at the very top of the program.
// Top section is stuff built-in
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Space and then mid section

// Mid section is plugins
//using NaughtyAttributes; <- Only commented to avoid compiler errors (since this assembly defintion does not have access to this namespace).

// Bottom section is stuff we wrote
//using Atma; <- Only commented to avoid compiler errors (since this assembly defintion does not have access to this namespace).

// Original Author - AuthorName
// This is not for 'ownership' purposes, but so that other programmers know who
// to start asking if they have questions about this scripts.

/// <summary>
/// Description of the class and what it does.
/// Pre Conditions: ~~~~~ (if applicable)
/// Post Conditions: ~~~~~ (if applicable)
/// </summary>
public class StyleGuide : MonoBehaviour
{
    // General spacing rules:
    // In a section, the same access modifiers (public/private)
    // should be grouped together. If changing modifiers in a section for
    // variables and properties include one line of blank space to separate them.
    // If changing modifiers in a section for functions,
    // include 2 lines of blank space.

    // 1) Constants should go at the very top of the class.
    // Constants should use all capital letters.
    public const float MY_CONSTANT = 3.14159f;
    // Since you cannot have const variables for structs, we can use readonly static
    // and treat them like constants.
    public readonly static Vector3 MY_READONLY_STATIC_VAR = new Vector3(2.0f, 1.0f, 7.0f);
    // One line of blank space between public and private constants

    // Private constants should go below public constants
    private const bool IS_DEBUGGING = false;

    // 2) After constants, should go any static properties and variables.
    // Order should go: public properties, private properties, private variables.
    // THERE SHOULD BE NO public variables. Instead use a public property that 
    // wraps a private variable or an auto-property.
    // Prefix static variables with an s and an underscore.
    // Properties should be camelCase with no underscore (_).
    public static int myStaticAutoProperty { get; private set; }
    // One line of blank space when changing from public to private.

    private static string myPrivateStaticControlledProperty => s_controlledStaticProperty;
    // One line of blank space between properties and variables.
    
    private static int s_myPrivateStaticVariable = 0;
    private static string s_controlledStaticProperty = "";

    // 3) After static variables, we declare any non-static properties and variables.
    // Order should go: public properties, private properties,
    // private variables with SerializeField, and private variables.
    // SerializeFields should be above other variables.
    // THERE SHOULD BE NO public variables. Instead use a public property that 
    // Properties should be camelCase with no underscore (_).
    public string myAutoProperty { get; private set; }
    public float controlledMemberVariable => m_controlledMemberVariable;
    // One line of blank space blank space between public and private

    [SerializeField] private float m_mySerializedField = 0.0f;
    // One line of blank space blank space between serialize fields and 
    // non serialized fields.

    // Private variables should have reference types above primitive types.
    private Rigidbody m_memberVariable1 = null;
    private int m_memberVariable2 = 0;
    private float m_controlledMemberVariable = 0;


    // At least 2 spaces separate the member variables from the functions.
    // 4) Functions.
    // 4a) The 1st group of functions should be unity message functions (Awake, Start, Update, etc.)
    // If any function groups get rather large, you can wrap them in a region for easy collapsing
    #region UnityMessages
    private void Awake()
    {
        // In awake "Domestic Initialization" should be done.
        // Any initialization/setting of variables that doesn't rely on other scripts.
        //
        // When doing something that isn't 100% assured to return the expected value
        // (most often GetComponent calls), do an assertion right afterward.
        // There are many functions in CustomDebug to assist with this.
        // Wrapping the assertions in a region can also help visibility.
        m_memberVariable1 = GetComponent<Rigidbody>();
        #region Asserts
        //CustomDebug.AssertComponentIsNotNull(m_memberVariable1, this);
        #endregion Asserts
        m_memberVariable2 = 0;
    }
    private void Start()
    {
        // In start "Foreign Initialization" should be done.
        // Any initialization/setting of variables that does rely on other scripts.
        m_controlledMemberVariable = m_memberVariable1.mass;
    }
    private void Update()
    {
        // Try to use update calls as little as possible, using it only for things that actually
        // need to be active for every frame they are in the game.
        // For things that only need to be active for a few frames after some event, we should use Coroutines.
    }
    #endregion UnityMessages


    // At least 2 spaces separate the unity message functions from the public functions.
    // 4b) The 2nd group of functions should be public functions.
    /// <summary>
    /// Description of the function.
    /// Pre Conditions: ~~~~~
    /// Post Conditions: ~~~~~
    /// </summary>
    /// <param name="param1">Description for what the parameter is used for.</param>
    /// <param name="param2">Description for what the parameter is used for.</param>
    public void MyPublicFunction(float param1, float param2)
    {
        m_memberVariable2 = (int)(param1 + param2);
    }
    /// <summary>
    /// Description of the function.
    /// Pre Conditions: ~~~~~
    /// Post Conditions: ~~~~~
    /// </summary>
    /// <param name="param">Description for what the parameter is used for.</param>
    /// <returns>What the output of the function can be.</returns>
    public Vector2 MyOtherPublicFunction(float param)
    {
        return new Vector2(param, m_memberVariable2);
    }


    // At least 2 spaces separate the public functions from the protected functions.
    // 4c) The 3rd group of functions should be protected functions.
    /// <summary>
    /// Description of the function.
    /// Pre Conditions: ~~~~~
    /// Post Conditions: ~~~~~
    /// </summary>
    protected void MyProtectedFunction()
    {

    }


    // At least 2 spaces separate the protected functions from the private functions.
    // 4d) The 4th group of functions should be private functions.
    /// <summary>
    /// Description of the function.
    /// Pre Conditions: ~~~~~
    /// Post Conditions: ~~~~~
    /// </summary>
    private void MyPrivateFunction()
    {
        int[] t_imaginaryList = { 0, 1, 2, 3, 4 };
        int t_currentIndex = 0;
        // This is a comment explaining the while loop and what it does
        while (t_currentIndex < t_imaginaryList.Length)
        {
            int t_currentVariable = t_imaginaryList[t_currentIndex];
            ++t_currentIndex;
        }
    }


    #region Debug
    // At least 2 spaces separate the private functions from the debug/test functions.
    // 4e) The 5th group of functions should be private functions used only for debugging.
    // They should all be prefixed with 'Debug.'
    private void DebugMyFunction()
    {
        CustomDebug.Log($"StyleGuide's Debug. I am debugging some unused variables to get rid of warnings in the console. {s_myPrivateStaticVariable} {m_mySerializedField} {m_memberVariable1}", IS_DEBUGGING);
    }
    #endregion Debug
}

/// <summary>
/// Interfaces have a summary as well.
/// They also have the prefix I.
/// </summary>
public interface IMyInterface
{

}


// Naming Conventions and my reasons for the ones in here:
// Want to make all names different to easy tell scope
// and what it is (function/member variable/property)

// ClassName - PascalCase to match C# standards.
// Interfaces - Prefixed with an I to match C# standards.
// CONSTANTS - Standard Capitalization.
// Functions() - PascalCase to match C# standards.
// memberProperties - LowerCamelCase to match Unity's property naming conventions (ex: transform, gameObject).
//      I'm unsure if this is best thing for properties because we like to make temp variables and parameters
//      with this same naming convention in functions and such.
// m_privateMemberVariables - Underscore lowerCamelCase to tell these are private members.
// s_staticVariables - s_ and lowerCamelCase to tell these are private static variables.
// t_temporaryVariables - t_ and lowerCamelCase to tell these are temporary variables that
//      exist only in the scope of the funciton.