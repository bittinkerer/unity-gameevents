using System.Collections.Generic;
using System.Linq;
using Packages.Estenis.GameEvent_;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Packages.Estenis.UnityExts_;

[CustomEditor( typeof( GameEventObject ) )]
public class GameEventEditor : Editor {
  const string              PREFABS_PATH        = @"Assets/Prefabs/";
  private GameEventObject   _target;

  private ReorderableList   _referencesList;
  List<GameObject>          _references = new ();
  private bool              _showReferences = false;

  private void OnEnable( ) {
    this._target = (GameEventObject) target;
    // References
    //_references = FindPrefabReferences( _target ).ToList(); // !! expensive computation
    _referencesList = new ReorderableList(
      _references,
      typeof( GameObject ) ) {
      drawElementCallback = DrawReferenceEntry,
      drawHeaderCallback = ( rect ) => GUI.Label( new Rect( rect.x, rect.y, rect.width, rect.height )
      , $"Prefabs ({_referencesList.count})" )
    };
  }

  public override void OnInspectorGUI( ) {
    DrawDefaultInspector();
    _target = (GameEventObject) target;
    using ( new EditorGUI.DisabledScope( !Application.isPlaying ) ) {
      if ( GUILayout.Button( "TRIGGER" ) ) {
        int eventId = GameEvent<object>.GlobalId;
        if ( !string.IsNullOrEmpty( _target._goNameForManualTrigger ) ) {
          GameObject eventTargetGO = GameObject.Find(_target._goNameForManualTrigger );
          eventId = eventTargetGO.GetComponent<EventMonoBehaviour>().EventId;
        }
        _target.Raise( eventId, null, null );
      }

      // Draw References
      _showReferences = EditorGUILayout.Foldout( _showReferences, $"Prefab References" );
      if ( _showReferences ) {
        if ( _references == null || _references.Count <= 0 ) {
          _references = FindPrefabReferences( _target ).ToList();
          _referencesList = 
            new ReorderableList(
              _references,
              typeof( GameObject ) ) {
                    drawElementCallback = DrawReferenceEntry,
                    drawHeaderCallback = ( rect ) => GUI.Label( new Rect( rect.x, rect.y, rect.width, rect.height )
                    , $"Prefabs ({_referencesList.count})" )
                  };
        }
        _referencesList.DoLayoutList();
      }
    }
  }

  /// <summary>
  /// Goes through all prefabs and find prefabs which in their hierarchy contain
  /// a reference to the given eventObject in a MonoBehaviour
  /// </summary>
  /// <param name="ttableName"></param>
  /// <returns></returns>
  private IEnumerable<GameObject> FindPrefabReferences( GameEventObject eventObject ) {
    var allPrefabs = AssetDatabase.FindAssets( "t:Prefab", new string[] { PREFABS_PATH } )
        .Select( guid => {
          string path = AssetDatabase.GUIDToAssetPath( guid );
          var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
          return go;
        } );
    var prefabsWithEvent = allPrefabs
      .Where(go => // gameObject has at least one component with eventObject
        go.GetComponentsRecursive<MonoBehaviour>()
        .Where(co =>  // component has at least one field with eventObject
          co != null &&
          co.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
          .Where(fi =>  fi.FieldType == typeof(GameEventObject) && ((GameEventObject)fi.GetValue(co)) != null &&  ((GameEventObject)fi.GetValue(co)).name == eventObject.name)
          .Count() > 0
        )
        .Count() > 0)
        .Select(go => go);

    return prefabsWithEvent;
  }

  private void DrawReferenceEntry( Rect rect, int index, bool isActive, bool isFocused ) {
    Rect ttableRect = new(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
    EditorGUI.ObjectField(
          ttableRect, "", _references[index], typeof( GameObject ), false );
  }
}