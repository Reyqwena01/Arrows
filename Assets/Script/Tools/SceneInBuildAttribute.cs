using UnityEngine;
using System;

/// <summary>
/// Change an int or string field into a selection field.
/// Displayed options are Scenes which can be found in build.
/// The field's type remains an int or string.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class SceneInBuildAttribute : PropertyAttribute
{

}