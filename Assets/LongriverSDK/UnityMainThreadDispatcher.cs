/*
Copyright 2015 Pim de Witte All Rights Reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

/// Author: Pim de Witte (pimdewitte.com) and contributors, https://github.com/PimDeWitte/UnityMainThreadDispatcher
/// <summary>
/// A thread-safe class which holds a queue with actions to execute on the next Update() method. It can be used to make calls to the main thread for
/// things such as UI Manipulation in Unity. It was developed for use in combination with the Firebase Unity plugin, which uses separate threads for event handling
/// </summary>
public class UnityMainThreadDispatcher : MonoBehaviour
{

	private static readonly Queue<Action> _executionQueue = new Queue<Action>();

	public void Update()
	{
		lock (_executionQueue)
		{
			while (_executionQueue.Count > 0)
			{
				_executionQueue.Dequeue().Invoke();
			}
		}
	}

	/// <summary>
	/// Locks the queue and adds the IEnumerator to the queue
	/// </summary>
	/// <param name="action">IEnumerator function that will be executed from the main thread.</param>
	public void Enqueue(IEnumerator action)
	{
		lock (_executionQueue)
		{
			_executionQueue.Enqueue(() => {
				StartCoroutine(action);
			});
		}
	}

	/// <summary>
	/// Locks the queue and adds the Action to the queue
	/// </summary>
	/// <param name="action">function that will be executed from the main thread.</param>
	public void Enqueue(Action action)
	{
		Enqueue(ActionWrapper(action));
	}

	public static void WrappedAction(Action action, TaskCompletionSource<bool> tcs)
	{
		try
		{
			action();
			tcs.TrySetResult(true);
		}
		catch (Exception ex)
		{
			tcs.TrySetException(ex);
		}
	}

	/// <summary>
	/// Locks the queue and adds the Action to the queue, returning a Task which is completed when the action completes
	/// </summary>
	/// <param name="action">function that will be executed from the main thread.</param>
	/// <returns>A Task that can be awaited until the action completes</returns>
	public Task EnqueueAsync(Action action)
	{
		var tcs = new TaskCompletionSource<bool>();
        Action<Action, TaskCompletionSource<bool>> warppedAction = WrappedAction;
		Enqueue(ActionWrapper(warppedAction, action, tcs));
		return tcs.Task;
	}

	IEnumerator ActionWrapper(Action a)
	{
		a();
		yield return null;
	}

	IEnumerator ActionWrapper(Action<Action, TaskCompletionSource<bool>> a, Action subAction, TaskCompletionSource<bool> tcs)
	{
		a(subAction, tcs);
		yield return null;
	}


	private static UnityMainThreadDispatcher _instance = null;

	public static bool Exists()
	{
		return _instance != null;
	}

	public static UnityMainThreadDispatcher Instance_backup()
	{
		if (!Exists())
		{
			throw new Exception("UnityMainThreadDispatcher could not find the UnityMainThreadDispatcher object. Please ensure you have added the MainThreadExecutor Prefab to your scene.");
		}
		return _instance;
	}

	public static UnityMainThreadDispatcher Instance()
	{
		if (_isQuitting)
		{
			Debug.LogWarning($"Trying to access singleton during application quit.");
			return null;
		}

		if (_instance != null)
			return _instance;

		if (!Application.isPlaying)
			return null;

#if UNITY_2023_1_OR_NEWER
        _instance = UnityEngine.Object.FindFirstObjectByType<UnityMainThreadDispatcher>();
#else
        _instance = Object.FindObjectOfType<UnityMainThreadDispatcher>();
#endif
		if (_instance != null)
            return _instance;

		// 自动创建并挂载
		GameObject go = new(typeof(UnityMainThreadDispatcher).Name);
		_instance = go.AddComponent<UnityMainThreadDispatcher>();
		UnityEngine.Object.DontDestroyOnLoad(go);

		if (null == _instance)
		{
			throw new Exception("UnityMainThreadDispatcher could not find the UnityMainThreadDispatcher object. Please ensure you have added the MainThreadExecutor Prefab to your scene.");
		}

		return _instance;
	}

	static private bool _isQuitting;
	private void OnApplicationQuit() 
	{
		_isQuitting = true;
	}

	void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
	}

	void OnDestroy()
	{
		_instance = null;
	}


}


