﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class EditorMenuItems : MonoBehaviour
{
	const string EDITOR_MENU_ROOT_NAME = "__My_uLua_Project__";

	//
	// Base utilities
	//

	[MenuItem( EDITOR_MENU_ROOT_NAME + "/Open the First Scene", false, 1000 )]
	static void OpenMainScene()
	{
		if( EditorBuildSettings.scenes.Length > 0 )
		{
			EditorSceneManager.OpenScene( EditorBuildSettings.scenes[0].path );
		}
		else
		{
			DebugEx.LogError( "No scenes in build! Please add at least one scene in \"Build Settings | Scenes in Build\"" );
		}
	}

	[MenuItem( EDITOR_MENU_ROOT_NAME + "/Generate: *.proto => *_pb.lua", false, 1100 )]
	static void GenerateProtoLua()
	{
		string protoPath = Path.GetFullPath( Path.Combine( Application.dataPath, "../Proto" ) );

		string protocExeVersionedFolderName = "protobuf-2.4.1/src";
		string protocGenLuaPluginFolderName = "protoc-gen-lua/plugin";
#if UNITY_EDITOR_WIN
		string protocFilename = "protoc.exe";
		string protocGenLuaFilename = "protoc-gen-lua.bat";
		string protocGenLuaGlobalFilename = "protoc-gen-lua-global.bat";
#else
		string protocFilename = "protoc";
		string protocGenLuaFilename = "protoc-gen-lua.sh";
		string protocGenLuaGlobalFilename = "protoc-gen-lua-global.sh";
#endif
		string protocPathName = Path.GetFullPath( Application.dataPath + "/../Tools/" + Path.Combine( protocExeVersionedFolderName, protocFilename ) );
		string protocGenLuaPathName = Path.GetFullPath( Application.dataPath + "/../Tools/" + Path.Combine( protocGenLuaPluginFolderName, protocGenLuaFilename ) );
		string protocGenLuaGlobalPathName = Path.GetFullPath( Application.dataPath + "/../Tools/" + Path.Combine( protocGenLuaPluginFolderName, protocGenLuaGlobalFilename ) );
		string generatedProtoLuaPath = Path.GetFullPath( Application.dataPath + "/LuaScripts/pb" );

		string arg = "--plugin=protoc-gen-lua=\"" + protocGenLuaPathName + "\" --lua_out=" + generatedProtoLuaPath + " ";
		string argGlobal = "--plugin=protoc-gen-lua=\"" + protocGenLuaGlobalPathName + "\" --lua_out=" + generatedProtoLuaPath + " ";

		string[] globalProtoFilenames = { "__common_enums.proto", "__message_types.proto" };

		try
		{
			foreach( var globalProtoFilename in globalProtoFilenames )
			{
				if( _ExecuteGenerateProtoLua( protoPath, protocPathName, argGlobal + globalProtoFilename ) )
				{
					DebugEx.Log( "Generated global lua file for: " + globalProtoFilename );
				}
			}

			foreach( var filePathName in Directory.GetFiles( protoPath, "*.proto" ) )
			{
				bool isHidden = ( ( File.GetAttributes( filePathName ) & FileAttributes.Hidden ) == FileAttributes.Hidden );
				if( isHidden )
					continue;

				string protoFilename = Path.GetFileName( filePathName );
				if( protoFilename.StartsWith( "__" ) )
					continue;

				if( _ExecuteGenerateProtoLua( protoPath, protocPathName, arg + protoFilename ) )
				{
					DebugEx.Log( "Generated lua file for " + protoFilename );
				}
			}
		}
		finally
		{
			AssetDatabase.Refresh();
		}
	}

	static bool _ExecuteGenerateProtoLua( string protoPath, string protocPathName, string arg )
	{
		var psi = new System.Diagnostics.ProcessStartInfo( protocPathName, arg );
		psi.CreateNoWindow = true;
		psi.RedirectStandardError = true;
		psi.UseShellExecute = false;
		psi.WorkingDirectory = protoPath;
		using( var process = System.Diagnostics.Process.Start( psi ) )
		{
			process.WaitForExit();

			if( process.ExitCode != 0 )
			{
				DebugEx.LogError( process.StandardError.ReadToEnd() );
				return false;
			}
		}

		return true;
	}

	//
	// Debug switchers
	//

	const string DEBUG_MODE_SYMBOL = "_DEBUG_";
	static bool _debugModeEnabled = false;

	[MenuItem( EDITOR_MENU_ROOT_NAME + "/Debug Mode/Enable", false, 9100 )]
	public static void EnableDebugMode()
	{
		_debugModeEnabled = true;

		AddScriptingSymbol( DEBUG_MODE_SYMBOL );
	}
	[MenuItem( EDITOR_MENU_ROOT_NAME + "/Debug Mode/Enable", true )]
	static bool ValidateEnableDebugMode()
	{
		_debugModeEnabled = _IsScriptingSymbolEnabled( DEBUG_MODE_SYMBOL );
		return !_debugModeEnabled;
	}

	[MenuItem( EDITOR_MENU_ROOT_NAME + "/Debug Mode/Disable", false, 9101 )]
	public static void DisableDebugMode()
	{
		_debugModeEnabled = false;

		RemoveScriptingSymbol( DEBUG_MODE_SYMBOL );
	}
	[MenuItem( EDITOR_MENU_ROOT_NAME + "/Debug Mode/Disable", true )]
	static bool ValidateDisableDebugMode()
	{
		_debugModeEnabled = _IsScriptingSymbolEnabled( DEBUG_MODE_SYMBOL );
		return _debugModeEnabled;
	}

	const string MOCK_MODE_SYMBOL = "_MOCK_";
	static bool _mockModeEnabled = false;

	[MenuItem( EDITOR_MENU_ROOT_NAME + "/Mock Mode/Enable", false, 9110 )]
	static void EnableMockMode()
	{
		_mockModeEnabled = true;

		AddScriptingSymbol( MOCK_MODE_SYMBOL );
	}
	[MenuItem( EDITOR_MENU_ROOT_NAME + "/Mock Mode/Enable", true )]
	static bool ValidateEnableMockMode()
	{
		_mockModeEnabled = _IsScriptingSymbolEnabled( MOCK_MODE_SYMBOL );
		return !_mockModeEnabled;
	}

	[MenuItem( EDITOR_MENU_ROOT_NAME + "/Mock Mode/Disable", false, 9111 )]
	static void DisableMockMode()
	{
		_mockModeEnabled = false;

		RemoveScriptingSymbol( MOCK_MODE_SYMBOL );
	}
	[MenuItem( EDITOR_MENU_ROOT_NAME + "/Mock Mode/Disable", true )]
	static bool ValidateDisableMockMode()
	{
		_mockModeEnabled = _IsScriptingSymbolEnabled( MOCK_MODE_SYMBOL );
		return _mockModeEnabled;
	}

	// Symbol utils

	public static void AddScriptingSymbol( string symbol )
	{
		if( !_IsScriptingSymbolEnabled( symbol ) )
		{
			string symbolsString = PlayerSettings.GetScriptingDefineSymbolsForGroup( EditorUserBuildSettings.selectedBuildTargetGroup );
			symbolsString += ";" + symbol;

			PlayerSettings.SetScriptingDefineSymbolsForGroup( EditorUserBuildSettings.selectedBuildTargetGroup, symbolsString );
		}
	}

	public static void RemoveScriptingSymbol( string symbol )
	{
		string symbolsString = PlayerSettings.GetScriptingDefineSymbolsForGroup( EditorUserBuildSettings.selectedBuildTargetGroup );
		string[] symbols = symbolsString.Split( ';' );

		symbolsString = "";

		foreach( string s in symbols )
		{
			if( s != symbol )
			{
				if( symbolsString.Length > 0 )
				{
					symbolsString += ';';
				}
				symbolsString += s;
			}
		}
		PlayerSettings.SetScriptingDefineSymbolsForGroup( EditorUserBuildSettings.selectedBuildTargetGroup, symbolsString );
	}

	static bool _IsScriptingSymbolEnabled( string symbol )
	{
		string symbolsString = PlayerSettings.GetScriptingDefineSymbolsForGroup( EditorUserBuildSettings.selectedBuildTargetGroup );

		return symbolsString.Contains( symbol );
	}

	//
	// Show project windows
	//

	[MenuItem( EDITOR_MENU_ROOT_NAME + "/Show Player Settings Window", false, 10010 )]
	static void ShowPlayerSettingsInspector()
	{
		EditorApplication.ExecuteMenuItem( "Edit/Project Settings/Player" );
	}

	[MenuItem( EDITOR_MENU_ROOT_NAME + "/Show Quality Settings Window", false, 10020 )]
	static void ShowQualitySettingsInspector()
	{
		EditorApplication.ExecuteMenuItem( "Edit/Project Settings/Quality" );
	}

	[MenuItem( EDITOR_MENU_ROOT_NAME + "/Show Graphics Settings Window", false, 10030 )]
	static void ShowGraphicsSettingsInspector()
	{
		EditorApplication.ExecuteMenuItem( "Edit/Project Settings/Graphics" );
	}

	//
	// Build project and AssetBundles
	//

	[MenuItem( EDITOR_MENU_ROOT_NAME + "/Build AssetBundle - LuaScripts", false, 11010 )]
	static void BuildAssetBundleLuaScripts()
	{
		string luaScriptsFolderName = "LuaScripts";
		string luaScriptsResourcesFolderName = "Resources/" + luaScriptsFolderName;

		string sourceDir = Path.Combine( Application.dataPath, luaScriptsFolderName );
		string targetDir = Path.Combine( Application.dataPath, luaScriptsResourcesFolderName );
		_DeleteAllInDirectory( targetDir );
		_CopyDirectory( sourceDir, targetDir, true, "*.lua", ".bytes" );

		string abName = "luascripts";

		foreach( var assetGUID in AssetDatabase.FindAssets( "", new string[] { "Assets/" + luaScriptsResourcesFolderName } ) )
		{
			string assetPath = AssetDatabase.GUIDToAssetPath( assetGUID );
			if( Path.GetExtension( assetPath ).ToLower() != ".bytes" )
				continue;

			AssetImporter assetImporter = AssetImporter.GetAtPath( assetPath );
			if( assetImporter != null )
			{
				assetImporter.assetBundleName = abName;
			}
		}

		AssetDatabase.Refresh();

		string abRootPath = Path.GetFullPath( Path.Combine( Application.dataPath, "../AssetBundles" ) );

		AssetBundleBuild abBuild = new AssetBundleBuild();
		abBuild.assetBundleName = abName;

		BuildPipeline.BuildAssetBundles( abRootPath );
	}

	static void _CopyLuaFile( string sourcePath, string targetPath )
	{
		using( StreamReader sr = new StreamReader( sourcePath ) )
		{
			string content = sr.ReadToEnd();

			using( StreamWriter sw = new StreamWriter( targetPath ) )
			{
				sw.Write( content );
			}
		}
	}

	static void _CopyDirectory( string sourceDir, string targetDir, bool recursive = true, string searchPattern = "*", string targetFileExt = null )
	{
		if( !Directory.Exists( sourceDir ) )
			return;

		if( !Directory.Exists( targetDir ) )
			Directory.CreateDirectory( targetDir );

		foreach( var filePathName in Directory.GetFiles( sourceDir, searchPattern ) )
		{
			bool isHidden = ( ( File.GetAttributes( filePathName ) & FileAttributes.Hidden ) == FileAttributes.Hidden );
			if( isHidden )
				continue;

			string targetFilename = Path.GetFileName( filePathName );
			if( targetFileExt != null )
			{
				targetFilename = Path.ChangeExtension( targetFilename, targetFileExt );
			}
			File.Copy( filePathName, Path.Combine( targetDir, targetFilename ), true );
		}

		if( recursive )
		{
			foreach( var directory in Directory.GetDirectories( sourceDir ) )
			{
				DirectoryInfo info = new DirectoryInfo( directory );
				bool isHidden = ( ( info.Attributes & FileAttributes.Hidden ) == FileAttributes.Hidden );
				if( isHidden )
					continue;

				string targetPath = Path.Combine( targetDir, Path.GetFileName( directory ) );
				if( !Directory.Exists( targetPath ) )
				{
					Directory.CreateDirectory( targetPath );
				}
				_CopyDirectory( directory, targetPath, recursive, searchPattern, targetFileExt );
			}
		}
	}

	static void _DeleteAllInDirectory( string dir )
	{
		if( !Directory.Exists( dir ) )
			return;

		foreach( var file in Directory.GetFiles( dir ) )
		{
			File.Delete( file );
		}

		foreach( var directory in Directory.GetDirectories( dir ) )
		{
			Directory.Delete( directory, true );
		}
	}
}
