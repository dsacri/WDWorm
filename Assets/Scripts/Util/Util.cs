using System;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Mathematics;
using UnityEngine;

public static class Util
{
    private const float degToRad = Mathf.PI / 180;

    public static float CalculatePythagorasLeg(float leg, float hypotenuse)
    {
        hypotenuse *= hypotenuse;
        leg *= leg;
        float otherLeg = hypotenuse - leg;
        otherLeg = (float)Math.Sqrt(otherLeg);

        return otherLeg;
    }

    public static float3 RotateVectorY(float3 vector, float angle)   //angle in degrees
    {
        angle *= degToRad;
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);
        return new float3(cos * vector.x - sin * vector.z, vector.y, sin * vector.x + cos * vector.z);
    }

    public static string ConvertText(string text)
    {
        if (text.Length == 0)
            return string.Empty;
        else if (text.Length == 1)
            return text[0].ToString().ToUpper();
        else
            return text[0].ToString().ToUpper() + text[1..].ToLower();
    }

    public static string TimelineTime(float value)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(value);
        return timeSpan.ToString("m':'ss'.'f");
    }

    public static Color ColorMultiply(Color color, float value)
    {
        return new(color.r * value, color.g * value, color.b * value, color.a);
    }


    public static readonly Color[] HeatmapColors = new Color[]
    {
        new Color32(62, 38, 168, 255),
        new Color32(72, 82, 244, 255),
        new Color32(46, 135, 247, 255),
        new Color32(18, 177, 214, 255),
        new Color32(55, 200, 151, 255),
        new Color32(171, 199, 57, 255),
        new Color32(254, 195, 56, 255),
        new Color32(249, 251, 21, 255)
    };

    public static Color GetInterpolatedColor(float value)
    {
        float scaledValue = value * (HeatmapColors.Length - 1);
        int index = Mathf.FloorToInt(scaledValue);
        int nextIndex = Mathf.Clamp(index + 1, 0, HeatmapColors.Length - 1);
        float t = scaledValue - index;
        return Color.Lerp(HeatmapColors[index], HeatmapColors[nextIndex], t);
    }

    public static bool[] InvertBoolArray(bool[] array)
    {
        return array.Select(b => !b).ToArray();
    }

    public static string GetFilename(string pathAndNameAndExtension)
    {
        return Regex.Replace(pathAndNameAndExtension, @".*[/\\]", "");
    }
}
