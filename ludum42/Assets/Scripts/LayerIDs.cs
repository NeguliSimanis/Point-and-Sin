using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para>Contains Layer IDs set in editor.</para>
/// <para>This is not set automatically, so any changes in editor layer order need to be entered manually here.</para>
/// </summary>
public class LayerIDs
{
    /// <summary>
    /// used to determine whether player can walk to this location
    /// </summary>
    public static int groundLayer = 8;
    /// <summary>
    /// used to determine whether player can walk to this location
    /// </summary>
    public static int enemyLayer = 10;
    public static int topBorder = 11;
    public static int leftBorder = 12;
    public static int bottomBorder = 13;
    public static int rightBorder = 14;
}
