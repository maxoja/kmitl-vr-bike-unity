using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class PopupEmote : MonoBehaviour {

    [Tooltip("This is the actual emote object that will spawn when you show the emote. You almost definitely don't need to change this reference. You're welcome to change the prefab itself though.")]
    [SerializeField]
    private GameObject _emotePrefab;

    [Tooltip("Select the type of popup animation that you want to use. There's stored in PopupEmote/Animation.")]
    [SerializeField]
    private AnimationClip _popupAnimation;

    [Tooltip("The list of all of the built-in emotes. If you want to add your own emotes, add them to the 'Custom Emotes' field below.")]
    [SerializeField]
    private Sprite[] _builtInSprites;

    [Tooltip("The list of all of your custom emotes. They'll be automatically referenced by name.")]
    [SerializeField]
    private Sprite[] _customSprites;

    [Tooltip("The transform that marks the (0,0,0) location of the spawned emotes. The emotes will spawn at (0,0,0) + (offset) relative to this transform. If not specified, this field will default to the origin of the game object that this script is attached to.")]
    [SerializeField]
    private Transform _emoteOrigin;

    [Tooltip("This offset will be applied to the emote's origin with respect to the 'Emote Origin' field.")]
    [SerializeField]
    private Vector3 _offset = new Vector3(0f, 0f, 0f);

    [Tooltip("This scale will be applied to the emote to allow you to shrink or expand the emote")]
    [SerializeField]
    private Vector2 _scale = new Vector2(1f, 1f);

    public int selectedTexture = -1;
    public string[] assetPaths = new string[0];

    private Dictionary<string, Sprite> _spritesByName;
    private GameObject _activeEmote = null;

    void Awake() {
        // This is a little trick I use to automatically set up each new instance of the emote script 
        // to be pre-loaded with all of the built-in emote types
#if UNITY_EDITOR
        if (selectedTexture == -1) {
            Reload();
        }
#endif

        // Make sure there's an origin setup. Default to the current object's transform
        if (_emoteOrigin == null) {
            _emoteOrigin = transform;
        }

        // Set up the sprite map, using the name as reference
        // NOTE: All of the sprite names are converted to lower-case!
        _spritesByName = new Dictionary<string, Sprite>();


        foreach (var sprite in _builtInSprites) {
            _spritesByName[sprite.name.ToLower()] = sprite;
        }

        if (_customSprites != null) {
            foreach (var customSprite in _customSprites) {
                _spritesByName[customSprite.name.ToLower()] = customSprite;
            }
        }
    }

    /// <summary>
    /// Actually display the emote. The emote will pop up at location (0,0,0) with respect to the game object 
    /// that holds this script. 
    /// </summary>
    /// <param name="emote">The name of the sprite that should be shown</param>
    public void ShowEmote(string emote) {
        string key = emote.ToLower();

        if (!_spritesByName.ContainsKey(key)) {
            Debug.LogError("The requested emote does not exist: " + key);
            return;
        }

        if (_activeEmote != null) {
            Destroy(_activeEmote);
        }

        _activeEmote = Instantiate(_emotePrefab, _emoteOrigin, true);
        _activeEmote.transform.localPosition = _offset;
        _activeEmote.transform.localScale = _scale;
        _activeEmote.GetComponentInChildren<SpriteRenderer>().sprite = _spritesByName[key];
        _activeEmote.GetComponentInChildren<Animation>().clip = _popupAnimation;
    }

    /// <summary>
    /// Clear out any emote that's currently showing for this instance
    /// </summary>
    public void CloseEmote() {
        if (_activeEmote != null) {
            Destroy(_activeEmote);
        }
    }

    /// <summary>
    /// Get a list of all of the currently registered emotes for this popup instance
    /// </summary>
    public string[] EmoteNames {
        get { return _spritesByName.Keys.ToArray(); }
    }

// Please ignore this code; it's pretty awful
#if UNITY_EDITOR
    public void SelectTexture(int selected) {
        if (selected >= 0 && selected < assetPaths.Length && selected != SelectedTextureIndex) {
            string path = assetPaths[selected];
            _builtInSprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
            selectedTexture = selected;
        }
    }

    public void Reload(bool force = false) {
        string[] guids = AssetDatabase.FindAssets("l:popup-texture");
        if (guids.Length == 0) {
            throw new NotSupportedException("Requires at least one built-in asset from the PopupEmotes library!");
        }

        assetPaths = guids
            .Select(AssetDatabase.GUIDToAssetPath)
            .Distinct()
            .OrderBy(x => x)    // Alphabetical
            .ToArray();

        if (force || selectedTexture < 0 || selectedTexture > assetPaths.Length - 1) {
            selectedTexture = 0;
        }

        _builtInSprites = AssetDatabase.LoadAllAssetsAtPath(assetPaths[selectedTexture]).OfType<Sprite>().ToArray();

        guids = AssetDatabase.FindAssets("Emote l:popup-prefab");
        foreach (var guid in guids) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            _emotePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }

        guids = AssetDatabase.FindAssets("l:popup-animation");
        foreach (var guid in guids) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            _popupAnimation = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        }
    }

    public int SelectedTextureIndex {
        get { return selectedTexture; }
    }
#endif
}
