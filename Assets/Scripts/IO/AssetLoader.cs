using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AssetLoader
{
#if LUA_ENCRYPTED
#	if UNITY_IOS
	[DllImport("__Internal")]
	private static extern System.IntPtr IosUtils_XXTeaDecrypt( byte[] data, int dataLen, ref uint retLen );
	[DllImport("__Internal")]
	private static extern void IosUtils_FreeXXTeaDecryptionBuffer( System.IntPtr bufferPtr );
#	endif
#endif

	static List<LuaFunction> _SceneLoadedCallbacks = new List<LuaFunction>();

	public static void LoadScene( string sceneName )
	{
		LoadScene( sceneName, null );
	}

	public static void LoadScene( string sceneName, object luaCallbackFunc )
	{
		_SceneLoadedCallbacks.Add( luaCallbackFunc as LuaFunction );

		SceneManager.LoadScene( sceneName );
	}

	public static void LoadAdditiveScene( string sceneName )
	{
		SceneManager.LoadScene( sceneName, LoadSceneMode.Additive );
	}

	public static void OnSceneLoaded( int level )
	{
		Resources.UnloadUnusedAssets();
		System.GC.Collect();

		if( _SceneLoadedCallbacks.Count > 0 )
		{
			LuaFunction callback = _SceneLoadedCallbacks[0];
			_SceneLoadedCallbacks.RemoveAt( 0 );

			if( callback != null )
			{
				callback.Call( level );
			}
		}
	}

	public static byte[] LoadLuaScript( string luaScriptPath )
	{
		byte[] s = null;

#if !UNITY_EDITOR
		TextAsset ta = null;

		try
		{
			ta = Resources.Load( "LuaScripts/" + luaScriptPath ) as TextAsset;
		}
		catch
		{
			DebugEx.LogWarning( "Lua script file does NOT exist! (" + luaScriptPath + ")" );
		}

		if( ta != null )
		{
			s = ta.bytes;
		}
		else
		{
			DebugEx.LogError( "Load Lua script file failed! (" + luaScriptPath + ")" );
		}
#else

		string path = Path.Combine( Application.dataPath, Path.Combine( "LuaScripts", luaScriptPath + ".lua" ) );
		if( !File.Exists( path ) )
		{
			DebugEx.LogError( "Lua script file does NOT exist! (" + luaScriptPath + ")" );
			return null;
		}

		s = File.ReadAllBytes( path );
#endif

		return s;
	}

	public static Texture2D LoadTexture( string texturePath )
	{
		Texture2D tex = null;

		if( tex == null )
		{
			try
			{
				tex = Resources.Load( "Textures/" + texturePath ) as Texture2D;
			}
			catch
			{
				DebugEx.LogError( "Texture does NOT exist! (" + texturePath + ")" );
			}
		}

		return tex;
	}

	public static AudioClip LoadAudio( string audioPath )
	{
		AudioClip audioClip = null;

		if( audioClip == null )
		{
			try
			{
				Object o = Resources.Load( "Audio/" + audioPath );
				if( o == null )
				{
					DebugEx.LogError( "Failed to load audio: " + audioPath );
					return null;
				}

				audioClip = o as AudioClip;
			}
			catch
			{
				DebugEx.LogError( "Audio does NOT exist! (" + audioPath + ")" );
			}
		}

		return audioClip;
	}

	public static GameObject LoadPrefab( string prefabPathName )
	{
		return LoadPrefab( prefabPathName, null );
	}

	public static GameObject LoadPrefab( string prefabPathName, GameObject parent )
	{
		GameObject prefab = null;

		Object o = Resources.Load( prefabPathName );
		if( o == null )
		{
			DebugEx.LogError( "Failed to load prefab: " + prefabPathName );
			return null;
		}

		prefab = MonoBehaviour.Instantiate( o ) as GameObject;
		if( prefab != null && parent != null )
		{
			prefab.transform.parent = parent.transform;
			prefab.transform.localPosition = Vector3.zero;
			prefab.transform.localEulerAngles = Vector3.zero;
			prefab.transform.localScale = Vector3.one;
		}

		return prefab;
	}

	public static GameObject LoadBattleUnitPrefab( string prefabPathName )
	{
		return LoadBattleUnitPrefab( prefabPathName, null );
	}

	public static GameObject LoadBattleUnitPrefab( string prefabPathName, GameObject parent )
	{
		Object o = null;

		if( o == null )
		{
			try
			{
				o = Resources.Load( "BattleUnits/" + prefabPathName );
				if( o == null )
				{
					DebugEx.LogError( "Failed to load battle unit prefab: " + prefabPathName );
					return null;
				}
			}
			catch
			{
				DebugEx.LogError( "Battle unit prefab does NOT exist! (" + prefabPathName + ")" );
			}
		}

		GameObject go = MonoBehaviour.Instantiate( o ) as GameObject;
		if( go != null )
		{
			if( parent != null )
			{
				go.transform.parent = parent.transform;
			}
		}
		else
		{
			DebugEx.LogError( "Load/Instantiate battle unit prefab failed! (" + prefabPathName + ")" );
		}

		return go;
	}

	public static GameObject LoadFxPrefab( string prefabPathName )
	{
		return LoadFxPrefab( prefabPathName, null );
	}

	public static GameObject LoadFxPrefab( string prefabPathName, GameObject parent )
	{
		Object o = null;
		GameObject go = null;

		try
		{
			o = Resources.Load( "Fx/" + prefabPathName );
			if( o == null )
			{
				DebugEx.LogError( "Failed to load FX prefab: " + prefabPathName );
				return null;
			}
		}
		catch
		{
			DebugEx.LogWarning( "FX prefab does NOT exist! (" + prefabPathName + ")" );
		}

		go = MonoBehaviour.Instantiate( o ) as GameObject;
		if( go != null )
		{
			if( parent != null )
			{
				go.transform.parent = parent.transform;
			}
		}
		else
		{
			DebugEx.LogError( "Load/Instantiate FX prefab failed! (" + prefabPathName + ")" );
		}

		return go;
	}

	public static GameObject LoadUIPrefab( string prefabPathName )
	{
		return LoadUIPrefab( prefabPathName, null );
	}

	public static GameObject LoadUIPrefab( string prefabPathName, GameObject parent )
	{
		Object o = null;
		GameObject go = null;

		try
		{
			o = Resources.Load( "UI/" + prefabPathName );
			if( o == null )
			{
				DebugEx.LogError( "Failed to load UI prefab: " + prefabPathName );
				return null;
			}
		}
		catch
		{
			DebugEx.LogWarning( "UI prefab does NOT exist! (" + prefabPathName + ")" );
		}

		if( o != null )
		{
			int i = o.name.LastIndexOf( '/' );
			if( i >= 0 )
			{
				o.name = o.name.Substring( i + 1 );
			}
		}

		go = MonoBehaviour.Instantiate( o ) as GameObject;
		if( go != null )
		{
			if( parent != null )
			{
				go.transform.parent = parent.transform;
			}
		}
		else
		{
			DebugEx.LogError( "Load/Instantiate UI prefab failed! (" + prefabPathName + ")" );
		}

		return go;
	}
}
