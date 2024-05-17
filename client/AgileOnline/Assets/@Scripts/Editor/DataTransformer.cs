using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using Data;
using System;
using System.Reflection;
using System.Collections;
using System.ComponentModel;

public class DataTransformer : EditorWindow
{
#if UNITY_EDITOR
	[MenuItem("Tools/ParseExcel %#K")]
	public static void ParseExcelDataToJson()
	{
		ParseExcelDataToJson<PlayerDataLoader, PlayerData>("Player");
		ParseExcelDataToJson<MonsterDataLoader, MonsterData>("Monster");
		ParseExcelDataToJson<StageDataLoader, StageData>("Stage");
		ParseExcelDataToJson<SkillDataLoader, SkillData>("Skill");
		
		Debug.Log("DataTransformer Completed");
	}

	#region Helpers
	
	private static void ParseExcelDataToJson<Loader, LoaderData>(string filename) where Loader : new() where LoaderData : new()
	{
		Loader loader = new Loader();
		FieldInfo field = loader.GetType().GetFields()[0];
		field.SetValue(loader, ParseExcelDataToList<LoaderData>(filename));

		string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
		File.WriteAllText($"{Application.dataPath}/@Resources/Data/JsonData/{filename}Data.json", jsonStr);
		AssetDatabase.Refresh();
	}

	private static List<LoaderData> ParseExcelDataToList<LoaderData>(string filename) where LoaderData : new()
	{
		List<LoaderData> loaderDatas = new List<LoaderData>();

		string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/ExcelData/{filename}Data.csv").Split("\n");

		for (int l = 1; l < lines.Length; l++)
		{
			string[] row = lines[l].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

			LoaderData loaderData = new LoaderData();

			System.Reflection.FieldInfo[] fields = typeof(LoaderData).GetFields();
			for (int f = 0; f < fields.Length; f++)
			{
				FieldInfo field = loaderData.GetType().GetField(fields[f].Name);
				Type type = field.FieldType;

				if (type.IsGenericType)
				{
					object value = ConvertList(row[f], type);
					field.SetValue(loaderData, value);
				}
				else
				{
					object value = ConvertValue(row[f], field.FieldType);
					field.SetValue(loaderData, value);
				}
			}

			loaderDatas.Add(loaderData);
		}

		return loaderDatas;
	}

	private static object ConvertValue(string value, Type type)
	{
		if (string.IsNullOrEmpty(value))
			return null;

		TypeConverter converter = TypeDescriptor.GetConverter(type);
		return converter.ConvertFromString(value);
	}

	private static object ConvertList(string value, Type type)
	{
		if (string.IsNullOrEmpty(value))
			return null;

		// Reflection
		Type valueType = type.GetGenericArguments()[0];
		Type genericListType = typeof(List<>).MakeGenericType(valueType);
		var genericList = Activator.CreateInstance(genericListType) as IList;

		// Parse Excel
		var list = value.Split('&').Select(x => ConvertValue(x, valueType)).ToList();

		foreach (var item in list)
			genericList.Add(item);

		return genericList;
	}
	#endregion

#endif
}