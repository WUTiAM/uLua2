﻿using LuaInterface;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

public static class L2U
{
	//public static void ConnectToHost( string ip, int port, int selectedServerId )
	//{
	//	NetworkSystem.Instance().SelectedServerId = selectedServerId;
	//	Game.Instance.StartCoroutine( NetworkSystem.Instance().ConnectToHost( ip, port ) );
	//}

	//public static object GetCsharpMemory( int len )
	//{
	//	return new byte[len];
	//}

	//public static void SendProtocol( int protocolType, byte[] message, bool needResponse, int dstId )
	//{
	//	NetworkSystem.Instance().Send( protocolType, message, needResponse, dstId );
	//}

	//
	// Unity
	//

	public static GameObject FindGameObject( string goName )
	{
		return FindGameObject( goName, null );
	}
	public static GameObject FindGameObject( string goName, GameObject parentGO )
	{
		return FindGameObject( goName, parentGO, true );
	}
	public static GameObject FindGameObject( string goName, GameObject parentGO, bool mustExist )
	{
		GameObject go = null;

		if( parentGO != null )
		{
			Transform t = parentGO.transform.Find( goName );
			if( t != null )
			{
				go = t.gameObject;
			}
		}
		else
		{
			go = GameObject.Find( goName );
		}

		if( mustExist && go == null )
		{
			DebugEx.LogError( string.Format( "Cannot find gameobject with name {0} under gameObject {1}", goName, parentGO ) );
		}

		return go;
	}

	public static GameObject CloneGameObject( GameObject go )
	{
		return CloneGameObject( go, null );
	}
	public static GameObject CloneGameObject( GameObject go, GameObject parentGO )
	{
		if( go == null )
			return null;

		GameObject clonedGO = GameObject.Instantiate( go ) as GameObject;

		if( parentGO == null && go.transform.parent != null )
		{
			parentGO = go.transform.parent.gameObject;
		}
		SetGameObjectParent( clonedGO, parentGO );

		return clonedGO;
	}

	public static void SetGameObjectParent( GameObject go, GameObject parentGO )
	{
		if( parentGO != null )
		{
			Vector3 pos = go.transform.localPosition;
			Vector3 angles = go.transform.localEulerAngles;
			Vector3 scale = go.transform.localScale;

			go.transform.parent = parentGO.transform;
			go.transform.localPosition = pos;
			go.transform.localEulerAngles = angles;
			go.transform.localScale = scale;
		}
	}

	public static Component AddComponentToGameObject( GameObject go, string componentTypeName )
	{
		return go.AddComponent( Type.GetType( componentTypeName ) );
	}

	public static Component AddUnityEngineComponentToGameObject( GameObject go, string componentTypeName )
	{
		return go.AddComponent( Type.GetType( "UnityEngine." + componentTypeName + ", UnityEngine" ) );
	}

	public static Component FindGameObjectAsComponent( string goName, string componentType )
	{
		return FindGameObjectAsComponent( goName, componentType, null );
	}
	public static Component FindGameObjectAsComponent( string goName, string componentType, GameObject parentGO )
	{
		GameObject go = FindGameObject( goName, parentGO );
		if( go == null )
			return null;

		return go.GetComponent( componentType );
	}

	// 位置

	static Vector3 _vec = new Vector3();

	public static void SetGameObjectLocalPosition( GameObject go, float x, float y, float z )
	{
		_vec.x = x;
		_vec.y = y;
		_vec.z = z;

		go.transform.localPosition = _vec;
	}
	public static void SetGameObjectWorldPosition( GameObject go, float x, float y, float z )
	{
		_vec.x = x;
		_vec.y = y;
		_vec.z = z;

		go.transform.position = _vec;
	}

	public static void SetGameObjectLocalPositionToTheOther( GameObject go, GameObject theOtherGO )
	{
		go.transform.localPosition = theOtherGO.transform.position;
	}
	public static void SetGameObjectWorldPositionToTheOther( GameObject go, GameObject theOtherGO )
	{
		go.transform.position = theOtherGO.transform.position;
	}

	// 旋转

	public static void SetGameObjectLocalRotation( GameObject go, float orientationX, float orientationY, float orientationZ )
	{
		_vec.x = orientationX;
		_vec.y = orientationY;
		_vec.z = orientationZ;

		go.transform.localRotation = Quaternion.LookRotation( _vec );
	}
	public static void SetGameObjectWorldRotation( GameObject go, float orientationX, float orientationY, float orientationZ )
	{
		_vec.x = orientationX;
		_vec.y = orientationY;
		_vec.z = orientationZ;

		go.transform.rotation = Quaternion.LookRotation( _vec ) * go.transform.rotation;
	}

	public static void SetGameObjectLocalRotationToTheOther( GameObject go, GameObject theOtherGO )
	{
		go.transform.localRotation = theOtherGO.transform.rotation;
	}
	public static void SetGameObjectWorldRotationToTheOther( GameObject go, GameObject theOtherGO )
	{
		go.transform.rotation = theOtherGO.transform.rotation;
	}

	public static void SetGameObjectEulerAngles( GameObject go, float angleX, float angleY, float angleZ )
	{
		_vec.x = angleX;
		_vec.y = angleY;
		_vec.z = angleZ;

		go.transform.localEulerAngles = _vec;
	}

	// 缩放

	public static void SetGameObjectLocalScale( GameObject go, float scale )
	{
		_vec.x = scale;
		_vec.y = scale;
		_vec.z = scale;

		go.transform.localScale = _vec;
	}
	public static void SetGameObjectLocalScaleXYZ( GameObject go, float scaleX, float scaleY, float scaleZ )
	{
		_vec.x = scaleX;
		_vec.y = scaleY;
		_vec.z = scaleZ;

		go.transform.localScale = _vec;
	}

	// 动作
	
	public static float PlayGameObjectAnimation( GameObject go, string animName )
	{
		return PlayGameObjectAnimation( go, animName, 1f, -1f );
	}
	public static float PlayGameObjectAnimation( GameObject go, string animName, float speed )
	{
		return PlayGameObjectAnimation( go, animName, speed, -1f );
	}
	public static float PlayGameObjectAnimation( GameObject go, string animName, float speed, float startTime )
	{
		if( go == null )
			return -1;

		Animation animation = go.GetComponent<Animation>();
		AnimationState animState = animation[animName];
		if( animState != null )
		{
			if( startTime < 0 )
				startTime = animState.time;

			animState.speed = speed;
			animState.time = startTime;
			animation.CrossFade( animName );

			return animState.clip.length;
		}
		else
		{
			DebugEx.LogError( "No animation named " + animName + " on game object " + go.ToString() );

			return -1;
		}
	}

	public static void StopGameObjectAnimation( GameObject go, string animName )
	{
		DebugEx.Assert( go != null );

		Animation animation = go.GetComponent<Animation>();
		AnimationState animState = animation[animName];
		if( animState != null )
		{
			animation.Stop( animName );
		}
		else
		{
			DebugEx.LogError( "No animation named " + animName + " on game object " + go.ToString() );
		}
	}

	public static void SetGameObjectAnimationSpeed( GameObject go, string animName, float speed )
	{
		DebugEx.Assert( go != null );

		Animation animation = go.GetComponent<Animation>();
		AnimationState animState = animation[animName];
		if( animState != null )
		{
			animState.speed = speed;
		}
		else
		{
			DebugEx.LogError( "No animation named " + animName + " on game object " + go.ToString() );
		}
	}

	public static float GetGameObjectAnimationLength( GameObject go, string animName )
	{
		if( go == null )
			return 0f;

		Animation animation = go.GetComponent<Animation>();
		AnimationState animState = animation[animName];
		if( animState != null )
		{
			return animState.clip.length;
		}
		else
		{
			DebugEx.LogError( "No animation named " + animName + " on game object " + go.ToString() );

			return 0;
		}
	}

	public static void BlendGameObjectAnimation( GameObject go, string animName )
	{
		DebugEx.Assert( go != null );

		Animation animation = go.GetComponent<Animation>();
		animation.Blend( animName );
	}


	// 其他

	public static void SetGameObjectLayer( GameObject go, string layerName, bool withChildren )
	{
		DebugEx.Assert( go != null );
		DebugEx.Assert( !string.IsNullOrEmpty( layerName ) );
		_SetGameObjectLayer( go, LayerMask.NameToLayer( layerName ), withChildren );
	}
	public static void SetGameObjectLayerSameAsOther( GameObject go, GameObject other )
	{
		DebugEx.Assert( go != null && other != null );
		_SetGameObjectLayer( go, other.layer, true );
	}
	static void _SetGameObjectLayer( GameObject go, int layer, bool withChildren )
	{
		DebugEx.Assert( go != null );
		go.layer = layer;

		if( withChildren )
		{
			for( int i = 0; i < go.transform.childCount; ++i )
			{
				Transform child = go.transform.GetChild( i );
				_SetGameObjectLayer( child.gameObject, layer, withChildren );
			}
		}
	}

	// 注意：这个gameObject的layer需要为 SelectableModel
	//public static GameObject TryGetPickedGameObjectInSelectableModel( Vector2 pickPos )
	//{
	//	if( Camera.main != null )
	//	{
	//		Ray kRay = Camera.main.ScreenPointToRay( new Vector3( pickPos.x, pickPos.y, 0f ) );
	//		int nLayerMask = 1 << (int)Layers.SelectableModel;
	//		RaycastHit hit;
	//		if( Physics.Raycast( kRay, out hit, Mathf.Infinity, nLayerMask ) == true )
	//		{
	//			return hit.collider.gameObject;
	//		}
	//	}
	public static Vector3 PickHorizontalPlane( Vector2 pickPos, float planeY )
	{
		if( Camera.main == null )
			return Vector3.zero;

		Plane plane = new Plane( Vector3.up, new Vector3( 0f, planeY, 0f ) );
		Ray ray = Camera.main.ScreenPointToRay( new Vector3( pickPos.x, pickPos.y, 0f ) );
		float enter;
		if( plane.Raycast( ray, out enter ) )
		{
			return ray.origin + ray.direction * enter;
		}

		return Vector3.zero;
	}

	//	return null;
	//}

}
