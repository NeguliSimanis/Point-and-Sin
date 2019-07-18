using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains global methods used for pixel perfect character movements and object placement
/// </summary>
public static class PixelClamp
{
   public static float ClampValue(float value)
   {
        float valueInPixels = value * PlayerData.current.pixelsPerUnit;
        float clampedValue = valueInPixels / PlayerData.current.pixelsPerUnit;
        return clampedValue;
    }
}
