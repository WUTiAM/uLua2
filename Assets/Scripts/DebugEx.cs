using System.Collections;
using UnityEngine;

public sealed class DebugEx
{
    //------------------------------------------------------------------------------------------------------------------
    // LogDebug
    // Log
    // LogWarning
    // LogError
    //
    // Assert
    //------------------------------------------------------------------------------------------------------------------

    static DebugEx()
    {
#if UNITY_IOS
        // 转发 Unity log 到 Ios/Mac 可见的输出目标
//		Application.logMessageReceived( NativeOs.PrintLogInXcodeOrgnizerConsole );
#endif
    }

    public static void LogDebug( object message )
    {
        Debug.Log( "[DEBUG] " + message.ToString() );
    }

    public static void LogDebug( object message, Object context )
    {
        Debug.Log( "[DEBUG] " + message.ToString(), context );
    }

    public static void Log( object message )
    {

        Debug.Log( "[LOG] " + message.ToString() );
    }

    public static void Log( object message, Object context )
    {
        Debug.Log( "[LOG] " + message.ToString(), context );
    }

    public static void LogWarning( object message )
    {
        Debug.LogWarning( "[WARNING] " + message.ToString() );
    }

    public static void LogWarning( object message, Object context )
    {
        Debug.LogWarning( "[WARNING] " + message.ToString(), context );
    }

    public static void LogError( object message )
    {
        Debug.LogError( "[ERROR] " + message.ToString() );
    }

    public static void LogError( object message, Object context )
    {
        Debug.LogError( "[ERROR] " + message.ToString(), context );
    }

    public static void Assert( bool condition )
    {
        Assert( condition, string.Empty, true );
    }

    public static void Assert( bool condition, string assertString )
    {
        Assert( condition, assertString, true );
    }

    public static void Assert( bool condition, string assertString, bool pauseOnFail )
    {
        if( !condition )
        {
            Debug.LogError( "[ASSERTION] Failed! " + assertString );
            if( pauseOnFail )
                Debug.Break();
        }
    }
}
