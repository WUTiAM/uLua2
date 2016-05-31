using LuaInterface;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class Lua
{
	static LuaState _luaState;

	public static LuaState CreateLuaState()
	{
		_luaState = new LuaState();
		LuaRegistrationHelper.TaggedStaticMethods( _luaState, typeof( Lua ) );

#if _DEBUG_
		_luaState["_DEBUG_"] = true;
#endif
#if _MOCK_
		_luaState["_MOCK_"] = true;
#endif
#if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
		_luaState["_MOBILE_PLATFORM_"] = true;
#endif

		// 初始化 Lua 环境，对接 Unity 到 Lua
		RequireLua( "defs" );

		return _luaState;
	}

	public static void Initialize()
	{
		RequireLua( "initialize" );
	}

	[LuaGlobalAttribute()]
	public static object RequireLua( string luaFilePath )
	{
		byte[] luaCode = AssetLoader.LoadLuaScript( luaFilePath );
		object[] results = null;

		if( luaCode != null )
		{
			try
			{
				results = _luaState.DoString( luaCode, luaFilePath, null );
			}
			catch( LuaScriptException e )
			{
				DebugEx.LogError( string.Format( "Failed to do RequireLua(\"{0}\"): {1} {2}", luaFilePath, e.Source, e.Message ) );
			}
			catch( Exception e )
			{
				DebugEx.LogError( string.Format( "Failed to do RequireLua(\"{0}\"): {1} {2}", luaFilePath, e.Source, e.Message ) );
			}
		}

		return ( results != null && results.Length > 0 ) ? results[0] : null;
	}

	[LuaGlobalAttribute()]
	public static void LuaPrint( string message )
	{
		Debug.Log( "[DEBUG:Lua] " + message );
	}

	[LuaGlobalAttribute()]
	public static void LuaLog( string message )
	{
		Debug.Log( "[LOG:Lua] " + message );
	}

	[LuaGlobalAttribute()]
	public static void LuaError( string message )
	{
		Debug.LogError( "[ERROR:Lua] " + message );
	}

	// 解决: PlayerSetting的 stripping level选择 strip bytecode 时， 一些函数被 strip 掉，而 lua 中又使用它
	// 方法: 强制这些函数不被 stip 掉
	// 注意: 定义在这里的函数是不会被真正调用的
	static void ____()
	{
		GameObject go = new GameObject();
		go = go.gameObject;
		go.SendMessage( "" );


		Animation anim = go.GetComponent<Animation>();
		anim.Blend( "" );
		anim.GetClip( "" );
		anim.IsPlaying( "" );
		anim.CrossFade( "" );
		anim.CrossFadeQueued( "" );

		AudioClip clip = new AudioClip();
		float audioClipLength = clip.length;

		Camera camera = new Camera();
		Vector3 vector3 = go.transform.localEulerAngles;
		camera.transform.localEulerAngles = vector3;

		Shader shader = Shader.Find( "" );

		Material m = new Material( shader );
		m.GetFloat( "" );
		m.SetFloat( "", audioClipLength );

		RenderSettings.fog = RenderSettings.fog;
		RenderSettings.fogColor = RenderSettings.fogColor;
		RenderSettings.fogDensity = RenderSettings.fogDensity;
		RenderSettings.fogStartDistance = RenderSettings.fogStartDistance;
		RenderSettings.fogEndDistance = RenderSettings.fogEndDistance;
	}
}
