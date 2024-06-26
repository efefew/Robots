﻿using System;

using UnityEngine;

public static class UnityExtentions
{
    #region Methods

    public static Sprite ToSprite(this Texture2D tex) => Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

    public static Texture2D ToTexture2D(this byte[] bytes)
    {
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        texture.Apply();
        return texture;
    }

    public static void Clear(this Transform transform)
    {
        if (transform.childCount == 0)
            return;
        for (int idChild = 0; idChild < transform.childCount; idChild++)
            UnityEngine.Object.Destroy(transform.GetChild(idChild).gameObject);
    }

    public static DateTimeOffset ToDateTimeOffset(this string text)
    {
        string[] date = text.Split(' ')[0].Split('.');
        string[] clock = text.Split(' ')[1].Split(':');
        string[] offset = text.Split(' ')[2].Split(':');
        return new DateTimeOffset(
            Convert.ToInt32(date[2]), Convert.ToInt32(date[1]), Convert.ToInt32(date[0]),
            Convert.ToInt32(clock[0]), Convert.ToInt32(clock[1]), Convert.ToInt32(clock[2]),
            new TimeSpan(Convert.ToInt32(offset[0]), Convert.ToInt32(offset[1]), 0));
    }

    public static Vector3 X(this Vector3 vector, float value) => new Vector3(value, vector.y, vector.z);
    public static Vector3 Y(this Vector3 vector, float value) => new Vector3(vector.x, value, vector.z);
    public static Vector3 Z(this Vector3 vector, float value) => new Vector3(vector.x, vector.y, value);

    public static bool[,] AddBorders(this bool[,] arr, bool value = true)
    {
        bool[,] newArr = new bool[arr.GetLength(0) + 2, arr.GetLength(1) + 2];
        for (int x = 0; x < newArr.GetLength(0); x++)
        {
            newArr[x, 0] = value;
            newArr[x, newArr.GetLength(0) - 1] = value;
        }
        for (int y = 0; y < newArr.GetLength(1); y++)
        {
            newArr[0, y] = value;
            newArr[newArr.GetLength(1) - 1, y] = value;
        }

        for (int x = 0; x < arr.GetLength(0); x++)
        {
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                newArr[x + 1, y + 1] = arr[x, y];
            }
        }
        return newArr;
    }
    /// <summary>
    /// метод визуализации массива
    /// </summary> 
    /// <returns>строка визуализации</returns>
    public static string ShowArray<T>(this T[] arr)
    {
        string str = "";
        for (int i = 0; i < arr.Length; i++)
            str += arr[i] + "\n";
        return str;
    }
    /// <summary>
    /// метод визуализации 2D массива
    /// </summary>
    /// <returns>строка визуализации</returns>
    public static string ShowArray<T>(this T[,] arr, string separator = " ")
    {
        string str = "";
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            for (int y = 0; y < arr.GetLength(1); y++)
                str += arr[x, y] + separator;
            str += "\n";
        }
        return str;
    }
    #endregion Methods
}

public static class StringModification
{
    #region Methods

    public static float ToFloat(this string s)
    {
        return Convert.ToSingle(s.Replace('.', ','));
    }

    public static float[] ToFloat(this string[] s)
    {
        float[] sFloat = new float[s.Length];
        for (int i = 0; i < s.Length; i++)
        {
            sFloat[i] = Convert.ToSingle(s[i].Replace('.', ','));
        }
        return sFloat;
    }

    public static int ToInt(this string s)
    {
        return Convert.ToInt32(s);
    }

    public static int[] ToInt(this string[] s)
    {
        int[] sInt = new int[s.Length];
        for (int i = 0; i < s.Length; i++)
        {
            sInt[i] = Convert.ToInt32(s[i]);
        }
        return sInt;
    }

    public static Vector2 ToVector2(this string s)
    {
        float[] Arr = s.Split(' ').ToFloat();
        return new Vector2(Arr[0], Arr[1]);
    }

    public static Vector2[] ToVector2(this string[] s)
    {
        float[] Arr = s.ToFloat();
        Vector2[] vec = new Vector2[s.Length / 2];
        for (int i = 0; i < vec.Length; i++)
            vec[i] = new Vector2(Arr[i * 2], Arr[i * 2 + 1]);
        return vec;
    }

    public static bool ToBool(this string s)
    {
        if (s == "true" || s == "1")
            return true;
        else
            return false;
    }

    public static bool[] ToArrBool(this string s)
    {
        bool[] Be = new bool[s.Length];
        for (int i = 0; i < s.Length; i++)
            Be[i] = s[i] == '1';
        return Be;
    }

    public static string ToOneString(this string[] s, string separator = "", bool ClearNull = true)
    {
        string str = "";
        if (s.Length > 0)
        {//d "" ff
            str = s[0];
            if (s.Length > 1)
                for (int i = 1; i < s.Length; i++)
                    if (s[i] != "" || !ClearNull)
                        str += separator + s[i];
                    else
                        str += s[i];
        }
        return str;
    }

    public static string ToOneString(this bool[] b)
    {
        string str = "";
        if (b.Length > 0)
        {
            for (int i = 0; i < b.Length; i++)
                if (b[i])
                    str += '1';
                else
                    str += '0';
        }
        return str;
    }

    #endregion Methods
}

public static class CharModification
{
    #region Methods

    public static int ToInt(this char c)
    {
        switch (c)
        {
            case '0': return 0;
            case '1': return 1;
            case '2': return 2;
            case '3': return 3;
            case '4': return 4;
            case '5': return 5;
            case '6': return 6;
            case '7': return 7;
            case '8': return 8;
            case '9': return 9;
            default: return -1;
        }
    }

    public static char ToChar(this int i)
    {
        switch (i)
        {
            case 0: return '0';
            case 1: return '1';
            case 2: return '2';
            case 3: return '3';
            case 4: return '4';
            case 5: return '5';
            case 6: return '6';
            case 7: return '7';
            case 8: return '8';
            case 9: return '9';
            default: return '-';
        }
    }

    #endregion Methods
}
public class Regulator
{
    private float oldValue, I = 0, Kp, Ki, Kd;
    public Regulator(float value, float Kp = 0.01f, float Ki = 0, float Kd = 0.5f)
    {
        oldValue = value;
        ChangeCoefficients(Kp, Ki, Kd);
    }
    public void ChangeCoefficients(float Kp = 0, float Ki = 0, float Kd = 0)
    {
        this.Kp = Kp; this.Ki = Ki; this.Kd = Kd; I = 0;
    }
    public float PID(float value, float target)
    {
        float deltaT = Time.fixedDeltaTime;
        float delta = (value - target);
        float P = delta;
        float Ti = Kp / Ki;
        float Td = Kd / Kp;
        I += (1 / Ti) * delta * deltaT;
        float D = -Td * (value - oldValue) / deltaT;
        float rez = Kp * (P + I + D);
        oldValue = value;
        return rez;
    }
}