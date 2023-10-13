using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;
using System.Globalization;
using System.Text;

/// <summary>
/// # 사용시 주의사항
/// 1.Enum Type은 사용할 Type을 수동으로 코드에서 추가해줘야 한다 => SetField 쪽에서 추가 코드를 적어줘야함.
/// 2.Struct, Class Type 데이터는 아직 구현되지 않았다.
/// 3.Array Type 사용시 Csv 파일 작성 규칙을 따라야한다.
/// 4.List Type 사용시 Csv 파일 작성 규칙을 따라야한다.
/// 
/// # 작성 규칙
/// 1. Array Type 작성시 해당 열을 따라 아래칸으로 값을 적는다. 해당 행은 빈 공간(Null)로 둔다.
/// 2. List Type 작성시 해당 칸 값들 사이에 '|' 을 삽입해 작성한다 ex)  1|2|3|4|5 
/// </summary>

public class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static T SimpleRead<T>(TextAsset textAsset, string file = null) where T : IData, new()
    {
        if (textAsset == null)
        {
            textAsset = Resources.Load(file) as TextAsset;
        }
        var lines = Regex.Split(textAsset.text, LINE_SPLIT_RE);
        var header = Regex.Split(lines[0], SPLIT_RE);

        T data = new T();
        Type dataType = data.GetType();
        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue; //라인 Skip

            for (int j = 0; j < 2 && j < 2; j++)
            {
                if (values[0].Length == 0 || values[j] == "") continue;
                string value = values[1];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");

                FieldInfo fieldInfo = dataType.GetField(values[0]);
                if (fieldInfo != null)
                {
                    //Generic 변수 처리━ => List
                    if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        SetFeied<T>(data, fieldInfo, value);
                    }
                    // 일반 변수 처리
                    else if (!CheckArrayField(fieldInfo.FieldType.Name))
                    {
                        SetField<T>(data, fieldInfo, value);
                    }
                    // Array 변수 처리
                    else
                    {
                        SetField<T>(data, fieldInfo, lines.Skip(i).Take(lines.Length - 1).ToArray(), j);
                    }
                }

                PropertyInfo propertyInfo = dataType.GetProperty(values[0]);
                if(propertyInfo != null)
                {
                    SetProperty(propertyInfo, data, value);
                }
            }
        }
        return data;
    }

    #region  Resources Read
    public static T Read<T>(TextAsset textAsset ,string filePath =null) where T : IData, new()
    {
        if (textAsset == null)
        {
            textAsset = Resources.Load(filePath) as TextAsset;
        }
        var lines = Regex.Split(textAsset.text, LINE_SPLIT_RE);
        var header = Regex.Split(lines[0], SPLIT_RE);

        T data = new T();
        Type dataType = data.GetType();
        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue; //라인 Skip

            for (int j = 0; j < header.Length && j < values.Length; j++)
            {
                if (values[j].Length == 0 || values[j] == "") continue;
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                FieldInfo fieldInfo = dataType.GetField(header[j]);
                if (fieldInfo != null)
                {
                    //Generic 변수 처리 => List
                    if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        SetFeied<T>(data, fieldInfo, value);
                    }
                    // 일반 변수 처리
                    else if (!CheckArrayField(fieldInfo.FieldType.Name))
                    {
                        SetField<T>(data, fieldInfo, value);
                    }
                    // Array 변수 처리━
                    else
                    {
                        SetField<T>(data, fieldInfo, lines.Skip(i).Take(lines.Length - 1).ToArray(), j);
                    }
                }
            }
        }
        return data;
    }

    public static List<T2> Read<T1, T2>(TextAsset textAsset, string filePath =null) where T1 : T2, new() where T2 : IData
    {
        if (textAsset == null)
        {
            textAsset = Resources.Load(filePath) as TextAsset;
        }
        var lines = Regex.Split(textAsset.text, LINE_SPLIT_RE);
        var header = Regex.Split(lines[0], SPLIT_RE);

        List<T2> dataList = new List<T2>(lines.Length);
        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue; //라인 Skip

            T1 data = new T1();
            Type dataType = data.GetType();
            
            for (int j = 0; j < header.Length && j < values.Length; j++)
            {
                if (values[j].Length == 0 || values[j] == "") continue;
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");

                FieldInfo fieldInfo = dataType.GetField(header[j]);
                if (fieldInfo != null)
                {
                    //Generic 변수 처리━ => List
                    if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        SetFeied<T1>(data, fieldInfo, value);
                    }
                    // 변수 처리
                    else if (!CheckArrayField(fieldInfo.FieldType.Name))
                    {
                        SetField<T1>(data, fieldInfo, value);
                    }
                    // Array 변수 처리
                    else
                    {
                        SetField<T1>(data, fieldInfo, lines.Skip(i).Take(lines.Length-1).ToArray(), j);
                    }
                }
            }
            dataList.Add(data);
        }
        return dataList;
    }
    #endregion

    #region SetFeild
    // Set Array
    private static void SetField<T>(T data, FieldInfo fieldInfo, string[] lines, int index) 
    {
        //int, float, string
        List<object> list = new List<object>(5);

        for(int i=0; i< lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length < index) break;
            if (i != 0)
            {
                if (values[0] == "" && values[index] != "")
                {
                    list.Add(values[index]);
                }
                else break;
            }
            else
            {
                list.Add(values[index]);
            }
        }

        switch (fieldInfo.FieldType.Name)
        {
            case "Int32[]":
                int[] intArray = list.Select(x => GetStringToInt(x.ToString())).ToArray();
                fieldInfo.SetValue(data, intArray);
                break;
            case "Single[]":
                float[] floatArray = list.Select(x => GetStringToFloat(x.ToString())).ToArray();
                fieldInfo.SetValue(data, list.OfType<float>().ToArray());
                break;
            case "String[]":
                fieldInfo.SetValue(data, list.OfType<string>().Select(x=> EncodingUTF8(x)).ToArray());
                break;
        }
    }

    // Set Normal Variable
    private static void SetField<T>(T data, FieldInfo fieldInfo, string value)
    {
        switch (fieldInfo.FieldType.Name)
        {
            case "Int32":
                fieldInfo.SetValue(data, GetStringToInt(value));
                break;
            case "Single":
                fieldInfo.SetValue(data, GetStringToFloat(value));
                break;
            case "String":
                fieldInfo.SetValue(data, EncodingUTF8(value));
                break;
            case "Boolean":
                fieldInfo.SetValue(data, bool.Parse(value.ToString()));
                break;
            // ======= Enum Field ============
            case "eAbType":
                fieldInfo.SetValue(data, (eAbType)Enum.Parse(typeof(eAbType), value));
                break;
            case "eSkillAbType":
                fieldInfo.SetValue(data, (eSkillAbType)Enum.Parse(typeof(eSkillAbType), value));
                break;
            case "eSkillType":
                var skillTypeStr = value.Split('|');

                eSkillType skillType = eSkillType.None;
                for (int i = 0; i < skillTypeStr.Length; i++)
                {
                    skillType |= (eSkillType)Enum.Parse(typeof(eSkillType), skillTypeStr[i]);
                }
                fieldInfo.SetValue(data, skillType);
                break;
            case "eActConditionType":
                var conditionTypeStr = value.Split('|');

                eActConditionType conditionType = eActConditionType.None;
                for (int i = 0; i < conditionTypeStr.Length; i++)
                {
                    conditionType |= (eActConditionType)Enum.Parse(typeof(eActConditionType), conditionTypeStr[i]);
                }
                fieldInfo.SetValue(data, conditionType);
                break;
            case "eBuffActType":
                fieldInfo.SetValue(data, (eBuffActType)Enum.Parse(typeof(eBuffActType), value));
                break;
            case "eBuffEffectType":
                fieldInfo.SetValue(data, (eBuffEffectType)Enum.Parse(typeof(eBuffEffectType), value));
                break;
            case "eCompareType":
                fieldInfo.SetValue(data, (eCompareType)Enum.Parse(typeof(eCompareType), value));
                break;
            case "eComparePosType":
                fieldInfo.SetValue(data, (eComparePosType)Enum.Parse(typeof(eComparePosType), value));
                break;
            case "eCharacterType":
                fieldInfo.SetValue(data, (eCharacterType)Enum.Parse(typeof(eCharacterType), value));
                break;
            case "eShakeType":
                fieldInfo.SetValue(data, (eShakeType)Enum.Parse(typeof(eShakeType), value));
                break;
            case "eNPCType":
                fieldInfo.SetValue(data, (eNPCType)Enum.Parse(typeof(eNPCType), value));
                break;
        }
    }

    // Set Generic List 
    private static void SetFeied<T>(T data, FieldInfo fieldInfo, string value)
    {
        Type listItemType = fieldInfo.FieldType.GetGenericArguments()[0];
        if (listItemType == typeof(int))
        {
            var values = value.Split("|");
            List<int> intList = new List<int>();
            for(int i=0; i < values.Length; i++)
            {
                intList.Add(GetStringToInt(values[i]));
            }
            fieldInfo.SetValue(data, intList);
        }
        else if (listItemType == typeof(float))
        {
            var values = value.Split("|");
            List<float> floatList = new List<float>();
            for (int i = 0; i < values.Length; i++)
            {
                floatList.Add(GetStringToFloat(values[i]));
            }
            fieldInfo.SetValue(data, floatList);
        }
        else if (listItemType == typeof(bool))
        {
            var values = value.Split("|");
            List<bool> boolList = new List<bool>();
            for (int i = 0; i < values.Length; i++)
            {
                boolList.Add(bool.Parse(values[i]));
            }
            fieldInfo.SetValue(data, boolList);
        }
		else if (listItemType == typeof(string))
        {
            var values = value.Split("|");
            List<string> stringList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                stringList.Add(EncodingUTF8(values[i]));
            }
            fieldInfo.SetValue(data, stringList);
        }
        // Enum
        else if (listItemType == typeof(eCharacterType))
        {
            var values = value.Split("|");
            List<eCharacterType> characterList = new List<eCharacterType>();
            for (int i = 0; i < values.Length; i++)
            {
                characterList.Add((eCharacterType)Enum.Parse(typeof(eCharacterType), values[i]));
            }
            fieldInfo.SetValue(data, characterList);
        }
        else
        {

        }
    }
    #endregion

    // Set Property
    private static void SetProperty<T>(PropertyInfo propertyInfo, T data, string value)
    {
        switch (propertyInfo.PropertyType.Name)
        {
            case "Int32":
                propertyInfo.SetValue(data, GetStringToInt(value));
                break;
            case "Single":
                propertyInfo.SetValue(data, GetStringToFloat(value));
                break;
            case "String":
                propertyInfo.SetValue(data, EncodingUTF8(value));
                break;
            case "eAbType":
                propertyInfo.SetValue(data, (eAbType)Enum.Parse(typeof(eAbType), value));
                break;
            case "eActConditionType":
                var values = value.Split('|');

                eActConditionType conditionType = eActConditionType.None;
                for (int i = 0; i < values.Length; i++)
                {
                    conditionType |= (eActConditionType)Enum.Parse(typeof(eActConditionType), values[i]);
                }
                propertyInfo.SetValue(data, conditionType);
                break;
            case "eBufferActType":
                propertyInfo.SetValue(data, (eBuffActType)Enum.Parse(typeof(eBuffActType), value));
                break;
            case "eBufferEffectType":
                propertyInfo.SetValue(data, (eBuffEffectType)Enum.Parse(typeof(eBuffEffectType), value));
                break;
            case "eCompareType":
                propertyInfo.SetValue(data, (eCompareType)Enum.Parse(typeof(eCompareType), value));
                break;
            case "eComparePosType":
                propertyInfo.SetValue(data, (eComparePosType)Enum.Parse(typeof(eComparePosType), value));
                break;
            case "eCharacterType":
                propertyInfo.SetValue(data, (eCharacterType)Enum.Parse(typeof(eCharacterType), value));
                break;
            case "eShakeType":
                propertyInfo.SetValue(data, (eShakeType)Enum.Parse(typeof(eShakeType), value));
                break;
            case "eNPCType":
                propertyInfo.SetValue(data, (eNPCType)Enum.Parse(typeof(eNPCType), value));
                break;
        }
    }

    #region Safe Parser
    private static int GetStringToInt(string value)
    {
        int resultI = 0;
        int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out resultI);
        return resultI;
    }

    private static float GetStringToFloat(string value)
    {
        float resultF = 0f;
        float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out resultF);
        return resultF;
    }

    private static string EncodingUTF8(string value)
    {
        byte[] bytes = Encoding.Default.GetBytes(value);
        value = Encoding.UTF8.GetString(bytes);
        return value;
    }

    #endregion

    #region Check Array Field
    private static bool CheckArrayField(string typeName)
    {
        switch (typeName)
        {
            case "Int32[]":
                return true;
            case "Single[]":
                return true;
            case "String[]":
                return true;
            default:
                return false;
        }
    }
    #endregion
}