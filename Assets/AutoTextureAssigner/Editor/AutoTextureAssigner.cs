using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace AutoTextureAssigner
{

    public class AutoTextureAssigner : EditorWindow
    {
        private List<Material> materials = new List<Material> ();
        private string textureFolderPath = "";
        private Vector2 scrollPosition;

        private const string PREFS_KEY = "AutoTextureAssigner_TextureFolder";

        [MenuItem ("Tools/Auto Texture Assigner")]
        public static void ShowWindow ()
        {
            GetWindow<AutoTextureAssigner> ("Assign Textures");
        }

        private void OnEnable ()
        {
            // Retrieve the saved texture folder path, defaulting to Application.dataPath
            textureFolderPath = EditorPrefs.GetString (PREFS_KEY, Application.dataPath);
        }

        private void OnDisable ()
        {
            // Save the texture folder path when the window is closed
            EditorPrefs.SetString (PREFS_KEY, textureFolderPath);
        }
        private void OnGUI ()
        {
            GUILayout.Space (8);
            GUIStyle titleStyle = new GUIStyle (EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
            GUILayout.Label ("Auto Texture Assigner", titleStyle);
            GUILayout.Space (10);

            // Display the list of materials
            if (materials.Count > 0)
            {
                scrollPosition = EditorGUILayout.BeginScrollView (scrollPosition, GUILayout.Height (120));
                for (int i = 0; i < materials.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal ();
                    EditorGUILayout.ObjectField (materials [i], typeof (Material), false);
                    if (GUILayout.Button (new GUIContent ("Remove", "Remove this material"), GUILayout.Width (70), GUILayout.Height (20)))
                    {
                        materials.RemoveAt (i);
                        i--;
                    }
                    EditorGUILayout.EndHorizontal ();
                }
                EditorGUILayout.EndScrollView ();
            }

            GUILayout.Space (5);

            // Drag & Drop area
            GUIStyle dragDropStyle = new GUIStyle (EditorStyles.helpBox)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black }
            };

            Rect dragRect = EditorGUILayout.GetControlRect (false, 40, GUILayout.ExpandWidth (true));
            dragRect.x += 3;
            dragRect.width -= 6;
            GUI.Box (dragRect, "DRAG & DROP MATERIALS HERE", dragDropStyle);

            Event evt = Event.current;
            if ((evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform) && dragRect.Contains (evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag ();
                    foreach (Object dragged in DragAndDrop.objectReferences)
                    {
                        Material mat = dragged as Material;
                        if (mat != null && !materials.Contains (mat))
                            materials.Add (mat);
                    }
                }
                evt.Use ();
            }

            GUILayout.Space (5);

            // Buttons for material management
            EditorGUILayout.BeginHorizontal ();
            if (GUILayout.Button (new GUIContent ("Get Materials From Selection", "Get materials from selected GameObjects in the scene"), GUILayout.Height (25)))
            {
                GetMaterialsFromSelection ();
            }
            if (GUILayout.Button (new GUIContent ("Remove All Materials", "Clear the materials list"), GUILayout.Height (25)))
            {
                materials.Clear ();
            }
            EditorGUILayout.EndHorizontal ();

            GUILayout.Space (5);

            // Texture folder selection section
            GUILayout.Label ("Texture Folder", EditorStyles.label);
            EditorGUILayout.BeginHorizontal ();
            textureFolderPath = EditorGUILayout.TextField ("Folder Path", textureFolderPath);
            if (GUILayout.Button (new GUIContent ("Select Folder", "Select the texture folder (opens in Assets)"), GUILayout.Width (100), GUILayout.Height (20)))
            {
                textureFolderPath = EditorUtility.OpenFolderPanel ("Select Texture Folder", Application.dataPath, "");
            }
            EditorGUILayout.EndHorizontal ();

            GUILayout.Space (10);

            // Custom style for the "Assign Textures" button (bold)
            GUIStyle assignButtonStyle = new GUIStyle (GUI.skin.button)
            {
                fontSize = 14,
                fixedHeight = 30,
                fontStyle = FontStyle.Bold
            };

            // Custom style for the "Clear Textures" button (regular)
            GUIStyle clearButtonStyle = new GUIStyle (GUI.skin.button)
            {
                fontSize = 12,
                fixedHeight = 30,
                fontStyle = FontStyle.Normal
            };

            EditorGUILayout.BeginHorizontal ();
            if (GUILayout.Button (new GUIContent ("Assign Textures", "Assign textures to materials based on naming"), assignButtonStyle, GUILayout.ExpandWidth (true)))
            {
                if (materials.Count > 0 && !string.IsNullOrEmpty (textureFolderPath))
                {
                    AssignTextures ();
                }
                else
                {
                    Debug.LogError ("Add materials and select a texture folder first.");
                }
            }
            if (GUILayout.Button (new GUIContent ("Clear Textures", "Remove textures from materials of selected GameObjects"), clearButtonStyle, GUILayout.Width (130)))
            {
                RemoveTexturesFromSelectedMaterials ();
            }
            EditorGUILayout.EndHorizontal ();
        }


        // Retrieves materials from the selected GameObjects in the scene.
        private void GetMaterialsFromSelection ()
        {
            materials.Clear ();
            foreach (GameObject go in Selection.gameObjects)
            {
                MeshRenderer mr = go.GetComponent<MeshRenderer> ();
                if (mr != null)
                {
                    foreach (Material m in mr.sharedMaterials)
                    {
                        if (m != null && !materials.Contains (m))
                            materials.Add (m);
                    }
                }
                SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer> ();
                if (smr != null)
                {
                    foreach (Material m in smr.sharedMaterials)
                    {
                        if (m != null && !materials.Contains (m))
                            materials.Add (m);
                    }
                }
            }
        }

        // Assigns textures to materials based on naming conventions.
        private void AssignTextures ()
        {
            Undo.RecordObjects (materials.ToArray (), "Assign Textures");
            Dictionary<string, List<Texture2D>> textureDict = new Dictionary<string, List<Texture2D>> ();
            // Get all files from the texture folder and filter by supported extensions.
            string [] allFiles = Directory.GetFiles (textureFolderPath, "*.*", SearchOption.AllDirectories);
            List<string> texturePaths = new List<string> ();
            string [] supportedExtensions = new string [] { ".png", ".jpg", ".jpeg", ".tga", ".tiff", ".bmp", ".psd", ".gif", ".hdr", ".exr" };
            foreach (string file in allFiles)
            {
                string ext = Path.GetExtension (file).ToLower ();
                if (System.Array.IndexOf (supportedExtensions, ext) >= 0)
                {
                    texturePaths.Add (file);
                }
            }

            foreach (string path in texturePaths)
            {
                string assetPath = "Assets" + path.Replace (Application.dataPath, "").Replace ("\\", "/");
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D> (assetPath);
                if (texture != null)
                {
                    string key = Path.GetFileNameWithoutExtension (texture.name).ToLower ();
                    if (!textureDict.ContainsKey (key))
                        textureDict [key] = new List<Texture2D> ();
                    textureDict [key].Add (texture);
                }
            }

            foreach (Material m in materials)
            {
                if (m != null)
                    AssignTexturesToMaterialStatic (m, textureDict);
            }

            Undo.FlushUndoRecordObjects ();
            Debug.Log ("Textures assigned to materials.");
        }

        // Assigns textures to a material based on naming conventions and render pipeline.
        private static void AssignTexturesToMaterialStatic (Material material, Dictionary<string, List<Texture2D>> textures)
        {
            // Channel keyword arrays
            string [] baseColorNames = { "basecolor", "albedo", "diffuse", "basemap", "albedotransparency" };
            string [] metallicNames = { "metallicsmoothness", "metallicglossmap", "maskmap", "glossiness" };
            string [] normalNames = { "normal", "bumpmap" };
            string [] emissionNames = { "emission", "emissionmap", "emissive" };
            string [] heightMapNames = { "heightmap", "displacement", "parallax" };
            string [] occlusionNames = { "occlusion", "ambientocclusion", "ao" };
            string [] detailMaskNames = { "detailmask" };
            string [] detailAlbedoNames = { "detailalbedo", "detaildiffuse", "detailcolor", "detailmap" };
            string [] detailNormalNames = { "detailnormal", "detailbump" };
            string [] specularNames = { "specular", "specularmap" };

            string pipeline = GetRenderPipeline (material);
            List<string> materialTokens = ExtractTokens (material.name, null);
            Texture2D baseColorTex = GetBestTextureForChannel (material, materialTokens, textures, baseColorNames);
            if (baseColorTex != null)
            {
                if (pipeline == "URP" && material.HasProperty ("_BaseMap"))
                    material.SetTexture ("_BaseMap", baseColorTex);
                else if (pipeline == "HDRP" && material.HasProperty ("_BaseColorMap"))
                    material.SetTexture ("_BaseColorMap", baseColorTex);
                else if (material.HasProperty ("_MainTex"))
                    material.SetTexture ("_MainTex", baseColorTex);
            }

            Texture2D metallicTex = GetBestTextureForChannel (material, materialTokens, textures, metallicNames);
            if (metallicTex != null)
            {
                if ((pipeline == "URP" || pipeline == "HDRP") && material.HasProperty ("_MaskMap"))
                {
                    material.SetTexture ("_MaskMap", metallicTex);
                    material.EnableKeyword ("_MASKMAP");
                }
                else if (material.HasProperty ("_MetallicGlossMap"))
                {
                    material.SetTexture ("_MetallicGlossMap", metallicTex);
                    material.EnableKeyword ("_METALLICGLOSSMAP");
                }
            }

            Texture2D normalTex = GetBestTextureForChannel (material, materialTokens, textures, normalNames);
            if (normalTex != null)
            {
                if (pipeline == "HDRP" && material.HasProperty ("_NormalMap"))
                {
                    material.SetTexture ("_NormalMap", normalTex);
                    material.EnableKeyword ("_NORMALMAP");
                }
                else if (material.HasProperty ("_BumpMap"))
                {
                    material.SetTexture ("_BumpMap", normalTex);
                    material.EnableKeyword ("_NORMALMAP");
                }
            }

            Texture2D emissionTex = GetBestTextureForChannel (material, materialTokens, textures, emissionNames);
            if (emissionTex != null)
            {
                if (pipeline == "HDRP" && material.HasProperty ("_EmissiveColorMap"))
                {
                    material.SetTexture ("_EmissiveColorMap", emissionTex);
                    material.EnableKeyword ("_EMISSION");
                }
                else if (material.HasProperty ("_EmissionMap"))
                {
                    material.SetTexture ("_EmissionMap", emissionTex);
                    material.EnableKeyword ("_EMISSION");
                }
            }

            Texture2D heightMapTex = GetBestTextureForChannel (material, materialTokens, textures, heightMapNames);
            if (heightMapTex != null)
            {
                if (pipeline == "HDRP" && material.HasProperty ("_HeightMap"))
                    material.SetTexture ("_HeightMap", heightMapTex);
                else if (material.HasProperty ("_ParallaxMap"))
                {
                    material.SetTexture ("_ParallaxMap", heightMapTex);
                    material.EnableKeyword ("_PARALLAXMAP");
                }
            }

            Texture2D occlusionTex = GetBestTextureForChannel (material, materialTokens, textures, occlusionNames);
            if (occlusionTex != null)
            {
                if (pipeline == "HDRP" && material.HasProperty ("_AmbientOcclusionMap"))
                    material.SetTexture ("_AmbientOcclusionMap", occlusionTex);
                else if (material.HasProperty ("_OcclusionMap"))
                    material.SetTexture ("_OcclusionMap", occlusionTex);
            }

            Texture2D detailMaskTex = GetBestTextureForChannel (material, materialTokens, textures, detailMaskNames);
            if (detailMaskTex != null)
            {
                if (pipeline == "HDRP" && material.HasProperty ("_DetailMask"))
                    material.SetTexture ("_DetailMask", detailMaskTex);
                else if (material.HasProperty ("_DetailMask"))
                    material.SetTexture ("_DetailMask", detailMaskTex);
            }

            Texture2D detailAlbedoTex = GetBestTextureForChannel (material, materialTokens, textures, detailAlbedoNames);
            if (detailAlbedoTex != null)
            {
                if (pipeline == "HDRP" && material.HasProperty ("_DetailAlbedoMap"))
                    material.SetTexture ("_DetailAlbedoMap", detailAlbedoTex);
                else if (material.HasProperty ("_DetailAlbedoMap"))
                    material.SetTexture ("_DetailAlbedoMap", detailAlbedoTex);
            }

            Texture2D detailNormalTex = GetBestTextureForChannel (material, materialTokens, textures, detailNormalNames);
            if (detailNormalTex != null)
            {
                if (material.HasProperty ("_DetailNormalMap"))
                {
                    material.SetTexture ("_DetailNormalMap", detailNormalTex);
                    if (material.HasProperty ("_DetailNormalMapScale"))
                        material.SetFloat ("_DetailNormalMapScale", 1f);
                    material.EnableKeyword ("_DETAIL_NORMALMAP");
                }
            }

            Texture2D specularTex = GetBestTextureForChannel (material, materialTokens, textures, specularNames);
            if (specularTex != null)
            {
                if (pipeline == "HDRP" && material.HasProperty ("_SpecularColorMap"))
                {
                    material.SetTexture ("_SpecularColorMap", specularTex);
                    material.EnableKeyword ("_SPECGLOSSMAP");
                }
                else if (material.HasProperty ("_SpecGlossMap"))
                {
                    material.SetTexture ("_SpecGlossMap", specularTex);
                    material.EnableKeyword ("_SPECGLOSSMAP");
                }
            }
        }

        private static string GetRenderPipeline (Material material)
        {
            string shaderName = material.shader.name.ToLower ();
            if (shaderName.Contains ("universal"))
                return "URP";
            if (shaderName.Contains ("hdrp"))
                return "HDRP";
            return "BuiltIn";
        }

        // Compares tokens and scores them based on token length and position.
        // Applies a penalty if the texture candidate has extra tokens compared to the material.
        private static Texture2D GetBestTextureForChannel (Material material, List<string> materialTokens, Dictionary<string, List<Texture2D>> textures, string [] channelKeywords)
        {
            List<Texture2D> candidates = new List<Texture2D> ();
            // Filter candidates: if the texture filename (without extension) contains any of the channel keywords, it is a candidate.
            foreach (var kvp in textures)
            {
                foreach (Texture2D tex in kvp.Value)
                {
                    string texName = Path.GetFileNameWithoutExtension (tex.name).ToLower ();
                    foreach (string keyword in channelKeywords)
                    {
                        if (texName.Contains (keyword.ToLower ()))
                        {
                            candidates.Add (tex);
                            break;
                        }
                    }
                }
            }
            if (candidates.Count == 0)
                return null;

            int bestScore = int.MinValue;
            Texture2D bestCandidate = null;

            foreach (Texture2D tex in candidates)
            {
                // Extract tokens filtering out tokens that are channel names.
                List<string> texTokens = ExtractTokens (tex.name, channelKeywords);
                if (texTokens.Count == 0 || materialTokens.Count == 0)
                    continue;

                int score = 0;
                bool [] used = new bool [texTokens.Count];

                // For each material token, try to find a match in the texture tokens.
                // - If found in the same position, add bonus proportional to the token's length.
                // - Otherwise, search in a different position (if not used) and add a reduced bonus.
                // - If not found, subtract a fixed penalty.
                for (int i = 0; i < materialTokens.Count; i++)
                {
                    bool found = false;
                    if (i < texTokens.Count && ApproximatelyEqual (materialTokens [i], texTokens [i]))
                    {
                        score += materialTokens [i].Length * 10;
                        used [i] = true;
                        found = true;
                    }
                    else
                    {
                        for (int j = 0; j < texTokens.Count; j++)
                        {
                            if (!used [j] && ApproximatelyEqual (materialTokens [i], texTokens [j]))
                            {
                                score += (materialTokens [i].Length * 10) / 2; // Reduced bonus for out-of-order match.
                                used [j] = true;
                                found = true;
                                break;
                            }
                        }
                    }
                    if (!found)
                    {
                        score -= 9; // Penalty if the material token is not found.
                    }
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    bestCandidate = tex;
                }
            }

            return bestScore > 0 ? bestCandidate : null;
        }

        private static List<string> ExtractTokens (string input, string [] channelKeywords = null)
        {
            List<string> tokens = new List<string> ();

            // Split the string using space, underscore, and hyphen as delimiters.
            string [] parts = input.Split (new char [] { ' ', '_', '-' }, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < parts.Length; i++)
            {
                // Convert the token to lowercase and trim it.
                string token = parts [i].ToLower ().Trim ();

                // If the next token is numeric, concatenate it to the current token.
                if (i + 1 < parts.Length && int.TryParse (parts [i + 1], out _))
                {
                    token += parts [i + 1].ToLower ().Trim ();
                    i++; // Skip the next token since it was concatenated.
                }

                // Ignore unwanted tokens.
                if (token == "t" || token == "tex" || token == "texture" ||
                    token == "m" || token == "mat" || token == "material")
                    continue;

                // If channelKeywords is provided, ignore tokens that exactly match a channel name.
                if (channelKeywords != null)
                {
                    bool isChannel = false;
                    foreach (string keyword in channelKeywords)
                    {
                        if (token.Equals (keyword, System.StringComparison.OrdinalIgnoreCase))
                        {
                            isChannel = true;
                            break;
                        }
                    }
                    if (isChannel)
                        continue;
                }

                tokens.Add (token);
            }

            return tokens;
        }

        // Compares tokens; for tokens shorter than 3 characters, requires exact equality.
        private static bool ApproximatelyEqual (string a, string b)
        {
            int numA, numB;
            bool isNumA = int.TryParse (a, out numA);
            bool isNumB = int.TryParse (b, out numB);
            if (isNumA && isNumB)
                return numA == numB;

            return (a == b);
        }

        /*
        private static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];
            if (n == 0) return m;
            if (m == 0) return n;
            for (int i = 0; i <= n; i++)
                d[i, 0] = i;
            for (int j = 0; j <= m; j++)
                d[0, j] = j;
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                    d[i, j] = Mathf.Min(
                        Mathf.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost
                    );
                }
            }
            return d[n, m];
        }*/

        private static bool ContainsAny (string textureName, string [] keywords)
        {
            foreach (string keyword in keywords)
            {
                if (textureName.Contains (keyword))
                    return true;
            }
            return false;
        }

        [MenuItem ("Assets/Assign Textures to Selected Materials", false, 200)]
        private static void ContextAssignTexturesToSelectedMaterials ()
        {
            List<Material> selectedMaterials = new List<Material> ();
            foreach (Object obj in Selection.objects)
            {
                Material mat = obj as Material;
                if (mat != null && !selectedMaterials.Contains (mat))
                    selectedMaterials.Add (mat);
            }
            if (selectedMaterials.Count == 0)
            {
                Debug.LogError ("No materials selected.");
                return;
            }
            string textureFolder = EditorUtility.OpenFolderPanel ("Select Texture Folder", Application.dataPath, "");
            if (string.IsNullOrEmpty (textureFolder))
            {
                Debug.Log ("Operation cancelled: no folder selected.");
                return;
            }
            Dictionary<string, List<Texture2D>> textureDict = new Dictionary<string, List<Texture2D>> ();
            string [] allFiles = Directory.GetFiles (textureFolder, "*.*", SearchOption.AllDirectories);
            List<string> texturePaths = new List<string> ();
            string [] supportedExtensions = new string [] { ".png", ".jpg", ".jpeg", ".tga", ".tiff", ".bmp", ".psd", ".gif", ".hdr", ".exr" };
            foreach (string file in allFiles)
            {
                string ext = Path.GetExtension (file).ToLower ();
                if (System.Array.IndexOf (supportedExtensions, ext) >= 0)
                {
                    texturePaths.Add (file);
                }
            }
            foreach (string path in texturePaths)
            {
                string assetPath = "Assets" + path.Replace (Application.dataPath, "").Replace ("\\", "/");
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D> (assetPath);
                if (texture != null)
                {
                    string key = Path.GetFileNameWithoutExtension (texture.name).ToLower ();
                    if (!textureDict.ContainsKey (key))
                        textureDict [key] = new List<Texture2D> ();
                    textureDict [key].Add (texture);
                }
            }
            foreach (Material m in selectedMaterials)
            {
                Undo.RecordObject (m, "Assign Textures");
                AssignTexturesToMaterialStatic (m, textureDict);
            }

            Undo.FlushUndoRecordObjects ();
            Debug.Log ("Textures assigned to selected materials.");
        }

        // Debug method: removes textures from selected materials.
        private void RemoveTexturesFromSelectedMaterials ()
        {
            if (materials.Count == 0)
            {
                Debug.Log ("No materials in the Auto Texture Assigner window.");
                return;
            }

            Undo.RecordObjects (materials.ToArray (), "Remove Textures");

            // List of common texture property names
            string [] textureProperties = new string [] {
                "_MainTex",
                "_BaseMap",
                "_BaseColorMap",
                "_MaskMap",
                "_MetallicGlossMap",
                "_BumpMap",
                "_NormalMap",
                "_EmissionMap",
                "_HeightMap",
                "_ParallaxMap",
                "_OcclusionMap",
                "_DetailMask",
                "_DetailAlbedoMap",
                "_DetailNormalMap",
                "_SpecGlossMap",
                "_SpecularColorMap"
            };

            foreach (Material m in materials)
            {
                foreach (string prop in textureProperties)
                {
                    if (m.HasProperty (prop))
                        m.SetTexture (prop, null);
                }
            }
            Undo.FlushUndoRecordObjects ();
            Debug.Log ("Textures removed from materials in the Auto Texture Assigner window.");
        }

    }
}