using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public class ResourceManager
{
	private Dictionary<string, Object> _resources = new Dictionary<string, Object>();
	private Dictionary<string, AsyncOperationHandle> _handles = new Dictionary<string, AsyncOperationHandle>();

	#region Load Resource
	public T Load<T>(string key) where T : Object
	{
		if (_resources.TryGetValue(key, out Object resource))
			return resource as T;

		return null;
	}

	public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
	{
		GameObject prefab = Load<GameObject>(key);
		if (prefab == null)
		{
			Debug.LogError($"Failed to load prefab : {key}");
			return null;
		}

		if (pooling)
			return Managers.Pool.Pop(prefab);

		GameObject go = Object.Instantiate(prefab, parent);
		go.name = prefab.name;

		return go;
	}

	public void Destroy(GameObject go)
	{
		if (go == null)
			return;

		if (Managers.Pool.Push(go))
			return;

		Object.Destroy(go);
	}
	#endregion

	#region Addressable
	private void LoadAsync<T>(string key, Action<T> callback = null) where T : UnityEngine.Object
	{
		// Cache
		if (_resources.TryGetValue(key, out Object resource))
		{
			callback?.Invoke(resource as T);
			return;
		}

		string loadKey = key;
		if (key.Contains(".sprite"))
			loadKey = $"{key}[{key.Replace(".sprite", "")}]";

		var asyncOperation = Addressables.LoadAssetAsync<T>(loadKey);
		asyncOperation.Completed += (op) =>
		{
			_resources.Add(key, op.Result);
			_handles.Add(key, asyncOperation);
			callback?.Invoke(op.Result);
		};
	}

	public void LoadAllAsync<T>(string label, Action<string, int, int> callback) where T : UnityEngine.Object
	{
		var opHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));
		opHandle.Completed += (op) =>
		{
			int loadCount = 0;
			int totalCount = op.Result.Count;

			foreach (var result in op.Result)
			{
				if (result.PrimaryKey.Contains(".sprite"))
				{
					LoadAsync<Sprite>(result.PrimaryKey, (obj) =>
					{
						loadCount++;
						callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
					});
				}
				else
				{
					LoadAsync<T>(result.PrimaryKey, (obj) =>
					{
						loadCount++;
						callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
					});
				}
			}
		};
	}

	public void Clear()
	{
		_resources.Clear();

		foreach (var handle in _handles)
			Addressables.Release(handle);

		_handles.Clear();
	}
	#endregion
}
