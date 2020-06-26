using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Pool : MonoBehaviour
{
	[SerializeField] private int initialQty;
	[SerializeField] private GameObject poolItemPrefab;
	[SerializeField] private List<PoolItemMonobehaviour> inactives = new List<PoolItemMonobehaviour> ();

	public PoolItemMonobehaviour GetPoolItem (Vector3 pos, Quaternion rot, Transform parent = null)
	{
		PoolItemMonobehaviour poolItemToReturn;

		if (inactives.Count == 0)
		{
			poolItemToReturn = Instantiate (poolItemPrefab, pos, rot).GetComponent<PoolItemMonobehaviour> ();
			poolItemToReturn.myPool = this;
		}
		else
		{
			poolItemToReturn = inactives.FirstOrDefault ();

			if (poolItemToReturn == null)
				return GetPoolItem (pos, rot, parent);
		}

		inactives.Remove (poolItemToReturn);

		poolItemToReturn.transform.SetParent (parent, false);
		poolItemToReturn.transform.localScale = Vector3.one;
		poolItemToReturn.transform.position = pos;
		poolItemToReturn.transform.rotation = rot;
		poolItemToReturn.gameObject.SetActive (true);
		poolItemToReturn.OnSpawn ();

		return poolItemToReturn;
	}

	public void RemovePoolItem (PoolItemMonobehaviour poolItem)
	{
		if (poolItem == null) return;

		poolItem.transform.SetParent (transform);
		poolItem.OnDespawn ();
		poolItem.gameObject.SetActive (false);
		inactives.Add (poolItem);
	}

	[CustomEditor (typeof (Pool))]
	public class PoolEditor : Editor
	{
		private SerializedObject me;
		private SerializedProperty initialQty;
		private SerializedProperty inactives;
		private SerializedProperty poolItemPrefab;
		private Pool myself;

		private void OnEnable ()
		{
			me = new SerializedObject (target);
			myself = (Pool) target;

			initialQty = me.FindProperty ("initialQty");
			poolItemPrefab = me.FindProperty ("poolItemPrefab");
			inactives = me.FindProperty ("inactives");
		}

		public override void OnInspectorGUI ()
		{
			me.Update ();

			EditorGUILayout.LabelField ("Properties:", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (initialQty);
			EditorGUILayout.Space ();
			EditorGUILayout.LabelField ("References:", EditorStyles.boldLabel);
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (poolItemPrefab);
			if (EditorGUI.EndChangeCheck ())
			{
				if (inactives.arraySize > 0 && poolItemPrefab.objectReferenceValue == null)
					Clean ();
			}

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			if (poolItemPrefab.objectReferenceValue != null)
			{
				GUILayout.BeginHorizontal ();
				{
					if (GUILayout.Button ("Populate"))
						Preload ();
					EditorGUILayout.Space ();
					EditorGUILayout.LabelField ("Instances Count: " + inactives.arraySize, EditorStyles.boldLabel);

					EditorGUILayout.LabelField ("");
				}
				GUILayout.EndHorizontal ();
			}

			me.ApplyModifiedProperties ();
		}

		private void Preload ()
		{
			if (poolItemPrefab == null) return;

			Clean ();

			PoolItemMonobehaviour poolItemToPreload;

			for (int i = 0; i < initialQty.intValue; i++)
			{
				poolItemToPreload = (Instantiate (poolItemPrefab.objectReferenceValue, myself.transform, false) as GameObject).GetComponent<PoolItemMonobehaviour> ();
				poolItemToPreload.myPool = myself;
				poolItemToPreload.gameObject.SetActive (false);
				poolItemToPreload.name = poolItemPrefab.objectReferenceValue.name + (i).ToString ();

				inactives.InsertArrayElementAtIndex (inactives.arraySize);
				inactives.GetArrayElementAtIndex (inactives.arraySize - 1).objectReferenceValue = poolItemToPreload;
			}
		}

		private void Clean ()
		{
			if (inactives.arraySize <= 0) return;

			for (int i = inactives.arraySize - 1; i >= 0; i--)
				DestroyImmediate (myself.inactives[i].gameObject);

			inactives.arraySize = 0;
		}
	}
}