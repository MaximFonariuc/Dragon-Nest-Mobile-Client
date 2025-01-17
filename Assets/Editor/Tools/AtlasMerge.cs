using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AtlasMerge : EditorWindow
{

    [MenuItem("XEditor/Tools/AtlasMerge")]
    static void Open()
    {
        GetWindow<AtlasMerge>("AtlasMerge", true);
    }

    private Dictionary<string, bool> _SelectSprites = new Dictionary<string, bool>();
    private UIAtlas _SourceAtlas = null;
    private UIAtlas _TargetAtlas = null;
    private Vector2 _Pos;
    private float _ClickTime;
    private UISprite _Sprite;

    void OnGUI()
    {

        GUILayout.BeginHorizontal();
        {
            ComponentSelector.Draw<UIAtlas>("Atlas", _SourceAtlas, OnSelectLeftAtlas, true, GUILayout.MinWidth(80f));
            if (GUILayout.Button(string.Format("Move To---->")))
            {
                if (_SourceAtlas != null && _TargetAtlas != null && _SourceAtlas != _TargetAtlas)
                {
                    foreach (KeyValuePair<string, bool> index in _SelectSprites)
                    {
                        if (index.Value)
                        {
                            SwitchSprite(_SourceAtlas, _TargetAtlas, _SourceAtlas.GetSprite(index.Key));
                        }
                    }
                }
                else
                {
                    ShowNotification(new GUIContent("移动失败！！！"));
                }
            }
            ComponentSelector.Draw<UIAtlas>("Atlas", _TargetAtlas, OnSelectRightAtlas, true, GUILayout.MinWidth(80f));
        }
        GUILayout.EndHorizontal();

        ShowAtlasDetail(_SourceAtlas, ref _Pos, ref _ClickTime, ref _Sprite);
    }

    void OnSelectLeftAtlas(Object obj)
    {
        if (_SourceAtlas != obj)
        {
            _SourceAtlas = obj as UIAtlas;
            _SelectSprites.Clear();
            Repaint();
        }
    }

    void OnSelectRightAtlas(Object obj)
    {
        if (_TargetAtlas != obj)
        {
            _TargetAtlas = obj as UIAtlas;
            Repaint();
        }
    }

    void SwitchSprite(UIAtlas source, UIAtlas target, UISpriteData sd)
    {
        if (target.GetSprite(sd.name) != null)
        {
            EditorUtility.DisplayDialog("Error To Move", string.Format("不能移动 Sprite {0} 在目标atlas {1} 中有同名Sprite", sd.name, target), "OK");
            return;
        }
        NGUISettings.unityPacking = false;
        NGUISettings.forceSquareAtlas = true;

        UIAtlasMaker.SpriteEntry entry = null;
        List<UIAtlasMaker.SpriteEntry> sprites = new List<UIAtlasMaker.SpriteEntry>();
        UIAtlasMaker.ExtractSprites(source, sprites);
        for (int i = sprites.Count; i > 0;)
        {
            UIAtlasMaker.SpriteEntry ent = sprites[--i];
            if (ent.name == sd.name)
            {
                entry = ent;
                sprites.RemoveAt(i);
            }
        }
        UIAtlasMaker.UpdateAtlas(source, sprites);

        sprites.Clear();
        UIAtlasMaker.ExtractSprites(target, sprites);
        sprites.Add(entry);
        UIAtlasMaker.UpdateAtlas(target, sprites);

        ModifyPrefab(source, target, sd.name);

        ShowNotification(new GUIContent("移动成功！！！"));
    }

    void ModifyPrefab(UIAtlas source, UIAtlas target, string spriteName)
    {
        Object[] list = Resources.LoadAll("UI");
        for (int i = 0; i < list.Length; ++i)
        {
            EditorUtility.DisplayProgressBar(string.Format("Replace...({0}/{1})", i, list.Length), "", 1.0f * i / list.Length);
            Object obj = list[i];
            if (AssetDatabase.GetAssetPath(obj).Contains(".prefab"))
            {
                bool change = false;
                GameObject go = PrefabUtility.InstantiatePrefab(obj) as GameObject;

                UISprite[] spList = go.GetComponentsInChildren<UISprite>(true);
                foreach (UISprite sp in spList)
                {
                    if (sp.atlas == source && sp.spriteName == spriteName)
                    {
                        change = true;
                        sp.atlas = target;
                    }
                }

                XUISprite[] xspList = go.GetComponentsInChildren<XUISprite>(true);
                foreach (XUISprite sp in xspList)
                {
                    if (sp.SpriteAtlasPath == AssetDatabase.GetAssetPath(source).Replace(".prefab", "").Replace("Assets/Resources/atlas/UI/", "") && sp.SPriteName == spriteName)
                    {
                        change = true;
                        sp.SpriteAtlasPath = AssetDatabase.GetAssetPath(target).Replace(".prefab", "").Replace("Assets/Resources/atlas/UI/", "");
                    }
                }

                if (change)
                {
                    Debug.Log(string.Format("{0}: Move {1} from {2} to {3} ", go.name, spriteName, source.name, target.name));
                    Object prefab = (obj == null) ? PrefabUtility.CreateEmptyPrefab(AssetDatabase.GetAssetPath(obj)) : (obj as GameObject);

                    PrefabUtility.ReplacePrefab(go, prefab);
                }
                DestroyImmediate(go);
            }
        }
        EditorUtility.ClearProgressBar();
    }

    void ShowAtlasDetail(UIAtlas data,ref Vector2 mPos,ref float mClickTime,ref UISprite mSprite)
    {
        if (data == null)
        {
            GUILayout.Label("No Atlas selected.", "LODLevelNotifyText");
        }
        else
        {
            UIAtlas atlas = data;
            bool close = false;
            GUILayout.Label(atlas.name + " Sprites", "LODLevelNotifyText");
            NGUIEditorTools.DrawSeparator();

            Texture2D tex = atlas.texture as Texture2D;

            if (tex == null)
            {
                GUILayout.Label("The atlas doesn't have a texture to work with");
                return;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(84f);

            string before = NGUISettings.partialSprite;
            string after = EditorGUILayout.TextField("", before, "SearchTextField");
            NGUISettings.partialSprite = after;

            if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
            {
                NGUISettings.partialSprite = "";
                GUIUtility.keyboardControl = 0;
            }
            GUILayout.Space(84f);
            GUILayout.EndHorizontal();

            BetterList<string> sprites = atlas.GetListOfSprites(NGUISettings.partialSprite);

            float size = 80f;
            float padded = size + 10f;
            int columns = Mathf.FloorToInt(Screen.width / padded);
            if (columns < 1) columns = 1;

            int offset = 0;
            Rect rect = new Rect(10f, 0, size, size);

            GUILayout.Space(10f);
            mPos = GUILayout.BeginScrollView(mPos);
            int rows = 1;

            while (offset < sprites.size)
            {
                GUILayout.BeginHorizontal();
                {
                    int col = 0;
                    rect.x = 10f;

                    for (; offset < sprites.size; ++offset)
                    {
                        UISpriteData sprite = atlas.GetSprite(sprites[offset]);
                        if (sprite == null) continue;

                        // Button comes first
                        if (GUI.Button(rect, ""))
                        {
                            if (Event.current.button == 0)
                            {
                                float delta = Time.realtimeSinceStartup - mClickTime;
                                mClickTime = Time.realtimeSinceStartup;

                                if (!_SelectSprites.ContainsKey(sprite.name)) _SelectSprites[sprite.name] = false;
                                _SelectSprites[sprite.name] = !_SelectSprites[sprite.name];
                            }
                        }

                        if (Event.current.type == EventType.Repaint)
                        {
                            // On top of the button we have a checkboard grid
                            NGUIEditorTools.DrawTiledTexture(rect, NGUIEditorTools.backdropTexture);
                            Rect uv = new Rect(sprite.x, sprite.y, sprite.width, sprite.height);
                            uv = NGUIMath.ConvertToTexCoords(uv, tex.width, tex.height);

                            // Calculate the texture's scale that's needed to display the sprite in the clipped area
                            float scaleX = rect.width / uv.width;
                            float scaleY = rect.height / uv.height;

                            // Stretch the sprite so that it will appear proper
                            float aspect = (scaleY / scaleX) / ((float)tex.height / tex.width);
                            Rect clipRect = rect;

                            if (aspect != 1f)
                            {
                                if (aspect < 1f)
                                {
                                    // The sprite is taller than it is wider
                                    float padding = size * (1f - aspect) * 0.5f;
                                    clipRect.xMin += padding;
                                    clipRect.xMax -= padding;
                                }
                                else
                                {
                                    // The sprite is wider than it is taller
                                    float padding = size * (1f - 1f / aspect) * 0.5f;
                                    clipRect.yMin += padding;
                                    clipRect.yMax -= padding;
                                }
                            }

                            GUI.DrawTextureWithTexCoords(clipRect, tex, uv);

                            // Draw the selection
                            bool flag = false;
                            if (_SelectSprites.TryGetValue(sprite.name, out flag) && flag)
                            {
                                NGUIEditorTools.DrawOutline(rect, new Color(0.4f, 1f, 0f, 1f));
                            }
                        }

                        GUI.backgroundColor = new Color(1f, 1f, 1f, 0.5f);
                        GUI.contentColor = new Color(1f, 1f, 1f, 0.7f);
                        GUI.Label(new Rect(rect.x, rect.y + rect.height, rect.width, 32f), sprite.name, "ProgressBarBack");
                        GUI.contentColor = Color.white;
                        GUI.backgroundColor = Color.white;

                        if (++col >= columns)
                        {
                            ++offset;
                            break;
                        }
                        rect.x += padded;
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(padded);
                rect.y += padded + 26;
                ++rows;
            }
            GUILayout.Space(rows * 26);
            GUILayout.EndScrollView();

            if (close) Close();
        }
    }
}
