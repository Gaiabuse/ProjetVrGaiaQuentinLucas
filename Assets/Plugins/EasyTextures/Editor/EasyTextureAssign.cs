namespace EasyTextureAssign
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class EasyTextureAssign
    {
        #region VARIABLES
        private const string defaultMaterial = "Default-Material";

        private static List<Material> selectedMats = new List<Material>();
        private static Material mat = null;

        private static bool foundTextures = false;
        private static Texture albedo, metallic, emission, normal, height, occlusion = null;
        #endregion

        /// <summary>
        /// Saves selected material and textures, calls assigns textures
        /// </summary>
        [MenuItem("Assets/Easy Textures/Assign Textures to Material " + Hotkeys_ET.ET_ASSIGN_HOTKEY, false, 100)]
        private static void EasyAssignTextures()
        {
            Object[] objects = Selection.objects;
            FindTextures(objects);
            AssignTextures();
        }

        /// <summary>
        /// Creates a new material and assigns selected textures
        /// </summary>
        [MenuItem("Assets/Easy Textures/Create New Material from Selection " + Hotkeys_ET.ET_CREATE_HOTKEY, false, 101)]
        private static void CreateMatFromTexture()
        {
            //  get selection
            Object[] objects = Selection.objects;

            //  find material and textures
            FindTextures(objects);
            if (!foundTextures) return;

            //  prompt
            if (EditorUtility.DisplayDialog("Create NEW Material?", "Create new material from selection?\n\nYou cannot undo this action.", "Create", "Cancel"))
            {
                Material material = new Material(Shader.Find("Standard"));

                string path = AssetDatabase.GetAssetPath(albedo);
                path = System.IO.Path.GetDirectoryName(path);

                string fileName = "";
                if (albedo != null) fileName = albedo.name;
                else if (albedo != null) fileName = albedo.name;
                else if (metallic != null) fileName = metallic.name;
                else if (normal != null) fileName = normal.name;
                else if (height != null) fileName = height.name;
                else if (occlusion != null) fileName = occlusion.name;
                else if (emission != null) fileName = emission.name;

                string fileNameTest = fileName.ToLower();

                fileNameTest = fileNameTest.Replace(ETSettings.albedoName.ToLower(), "^#$^");

                int index = fileNameTest.IndexOf("^#$^");
                if (index > 0)
                    fileName = fileName.Substring(0, index);

                AssetDatabase.CreateAsset(material, path + "/" + fileName + "Material.mat");
                mat = material;
                AssignTextures(false);
            }
        }

        /// <summary>
        /// Find textures and material in selection
        /// </summary>
        private static void FindTextures(Object[] objects)
        {
            if (objects.Length == 0) return;

            HashSet<Object> allObjects = new HashSet<Object>(objects);

            //  find textures in folder
            string path = "";
            foreach (var obj in objects)
            {
                path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
                if (path.Length > 0)
                {
                    if (System.IO.Directory.Exists(path))
                    {
                        string[] guids2 = AssetDatabase.FindAssets("t:texture2D", new[] { path });
                        foreach (var guid in guids2)
                        {
                            allObjects.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Object)));
                        }
                    }
                }
            }

            //  reset variables
            mat = null;
            foundTextures = false;
            albedo = metallic = emission = normal = height = occlusion = null;

            //  find textures and material
            foreach (Object obj in allObjects)
            {
                //  find material
                if (obj is Material && mat == null)
                {
                    mat = obj as Material;
                }
                //  find material on MeshRenderer
                else if (obj is GameObject && mat == null)
                {
                    GameObject go = (GameObject)obj;
                    Renderer renderer = go.GetComponent<Renderer>();
                    if (renderer != null && (renderer is SkinnedMeshRenderer || renderer is MeshRenderer))
                    {
                        mat = renderer.sharedMaterial;
                    }
                }
                //  find textures
                else if (obj is Texture)
                {
                    Texture texture = obj as Texture;

                    if (texture.name.ToLower().Contains(ETSettings.albedoName.ToLower()) && albedo == null)
                    {
                        albedo = texture;
                        foundTextures = true;
                    }
                    else if (texture.name.ToLower().Contains(ETSettings.emissionName.ToLower()) && emission == null)
                    {
                        emission = texture;
                        foundTextures = true;
                    }
                    else if (texture.name.ToLower().Contains(ETSettings.metallicName.ToLower()) && metallic == null)
                    {
                        metallic = texture;
                        foundTextures = true;
                    }
                    else if (texture.name.ToLower().Contains(ETSettings.normalName.ToLower()) && normal == null)
                    {
                        normal = texture;
                        foundTextures = true;
                    }
                    else if (texture.name.ToLower().Contains(ETSettings.heightName.ToLower()) && height == null)
                    {
                        height = texture;
                        foundTextures = true;
                    }
                    else if (texture.name.ToLower().Contains(ETSettings.occlusionName.ToLower()) && occlusion == null)
                    {
                        occlusion = texture;
                        foundTextures = true;
                    }
                }
            }
        }

        /// <summary>
        /// Displays confirm window and assigns textures
        /// </summary>
        private static void AssignTextures(bool undo = true)
        {
            //  stop if no material or textures, or if Default-Material
            if (mat == null || !foundTextures || mat.name == defaultMaterial) return;

            //  confirm message
            string message = "";
            if (albedo != null) { message += "   " + albedo.name + "\n"; }
            if (metallic != null) { message += "   " + metallic.name + "\n"; }
            if (normal != null) { message += "   " + normal.name + "\n"; }
            if (height != null) { message += "   " + height.name + "\n"; }
            if (occlusion != null) { message += "   " + occlusion.name + "\n"; }
            if (emission != null) { message += "   " + emission.name + "\n"; }

            if (undo)
                Undo.RecordObject(mat, "Assigned Textures to " + mat.name);

            //  set albedo map
            if (albedo != null)
            {
                float alpha = mat.color.a;
                mat.color = new Color(1, 1, 1, alpha);
                mat.SetTexture("_MainTex", albedo);
            }

            //  set emission map
            if (emission != null)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.white);
                mat.SetTexture("_EmissionMap", emission);
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.AnyEmissive;
            }

            //  set metallic map
            if (metallic != null)
            {
                mat.EnableKeyword("_METALLICGLOSSMAP");
                mat.SetTexture("_MetallicGlossMap", metallic);
            }

            //  reimport normal texture as normal map
            if (normal != null)
            {
                if (ETSettings.autoFixNormals)
                {
                    string path = AssetDatabase.GetAssetPath(normal);
                    TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
                    if (importer.textureType != TextureImporterType.NormalMap)
                    {
                        importer.textureType = TextureImporterType.NormalMap;
                        importer.SaveAndReimport();
                    }
                }
                mat.EnableKeyword("_NORMALMAP");
                mat.SetTexture("_BumpMap", normal);
            }

            //  set height map
            if (height != null)
            {
                mat.EnableKeyword("_PARALLAXMAP");
                mat.SetTexture("_ParallaxMap", height);
            }

            //  set occlusion map
            if (occlusion != null)
            {
                mat.SetTexture("_OcclusionMap", occlusion);
            }
        }

        /// <summary>
        /// Clears textures off selected materials
        /// </summary>
        [MenuItem("Assets/Easy Textures/Clear Textures " + Hotkeys_ET.ET_CLEAR_HOTKEY, false, 111)]
        private static void ClearTexturesWindow()
        {
            //  get selection
            Object[] objects = Selection.objects;
            if (objects.Length == 0) return;

            //  find all materials in selection
            selectedMats.Clear();
            foreach (var obj in objects)
            {
                if (obj is Material)
                {
                    if (!selectedMats.Contains(obj as Material))
                    {
                        selectedMats.Add(obj as Material);
                    }
                }
                else if (obj is GameObject)
                {
                    GameObject go = (GameObject)obj;
                    Renderer renderer = go.GetComponent<Renderer>();
                    if (renderer != null && (renderer is SkinnedMeshRenderer || renderer is MeshRenderer))
                    {
                        if (!selectedMats.Contains(renderer.sharedMaterial))
                        {
                            selectedMats.Add(renderer.sharedMaterial);
                        }
                    }
                }
            }

            //Clear Materials
            DoClear(selectedMats);

            Selection.activeGameObject = null;
        }

        /// <summary>
        /// Clears textures off materials in a list
        /// </summary>
        private static void DoClear(List<Material> mats)
        {
            if (selectedMats.Count <= 0) return;

            foreach (var selectedMat in mats)
            {
                Undo.RecordObject(selectedMat, "Clear Textures off " + selectedMat.name);

                selectedMat.color = Color.white;
                selectedMat.SetTexture("_MainTex", null);
                selectedMat.mainTextureOffset = Vector2.zero;
                selectedMat.mainTextureScale = Vector2.one;

                selectedMat.DisableKeyword("_METALLICGLOSSMAP");
                selectedMat.SetTexture("_MetallicGlossMap", null);

                selectedMat.DisableKeyword("_EMISSION");
                selectedMat.SetTexture("_BumpMap", null);

                selectedMat.DisableKeyword("_NORMALMAP");
                selectedMat.SetTexture("_ParallaxMap", null);

                selectedMat.SetTexture("_OcclusionMap", null);

                selectedMat.DisableKeyword("_DETAIL_MULX2");
                selectedMat.SetTexture("_DetailMask", null);
                selectedMat.SetTexture("_DetailAlbedoMap", null);
                selectedMat.SetTexture("_DetailNormalMap", null);

                selectedMat.DisableKeyword("_EMISSION");
                selectedMat.SetColor("_EmissionColor", Color.clear);
                selectedMat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                selectedMat.SetTexture("_EmissionMap", null);
            }
        }
    }

    /// <summary>
    /// Easy Texture Settings Window
    /// </summary>
    public class ETSettings : EditorWindow
    {
        #region VARIABLES
        private Vector2 scrollPos;

        private const string ALBEDO_NAME = "albedo";
        private const string METALLIC_NAME = "metallic";
        private const string EMISSION_NAME = "emission";
        private const string NORMAL_NAME = "normal";
        private const string HEIGHT_NAME = "height";
        private const string OCCLUSION_NAME = "occlusion";
        private const string DETAIL_MASK = "mask";
        private const string DETAIL_ALBEDO = "detail albedo";
        private const string DETAIL_NORMAL = "detail normal";

        private const string hotkeyTooltip = "Displays hotkey this action is currently set for.\nTo modify edit 'EasyTextureHotkeys.cs'";
        private const string hotkeyTooltipLabel = "To change hotkeys, edit 'EasyTextureHotkeys.cs'";
        private const string textFieldTooltip = "Unique string to look for in the file name for this texture type";
        private const string normalFixTooltip = "Automatically fixs normal maps not set as normal maps";

        public static string albedoName = "albedo";
        public static string metallicName = "metallic";
        public static string emissionName = "emission";
        public static string normalName = "normal";
        public static string heightName = "height";
        public static string occlusionName = "occlusion";

        public static bool autoFixNormals = true;
        #endregion

        /// <summary>
        /// Opens window
        /// </summary>
        [MenuItem("Window/Easy Textures/Settings", false, 1)]
        public static void OpenWindow()
        {
            ShowWindow();
        }

        /// <summary>
        /// Opens window
        /// </summary>
        [MenuItem("Assets/Easy Textures/Settings", false, 1000)]
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow(typeof(ETSettings), false, "Settings");
            window.minSize = new Vector2(275, 150);
        }

        /// <summary>
        /// Initialization
        /// </summary>
        private void OnEnable()
        {
            if (EditorPrefs.HasKey("Normal Map Fix"))
                autoFixNormals = EditorPrefs.GetBool("Normal Map Fix");
            else
                EditorPrefs.SetBool("Normal Map Fix", autoFixNormals);
        }

        /// <summary>
        /// GUI for window
        /// </summary>
        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            //  UNIQUE TEXTURE KEYS
            GUILayout.Label("UNIQUE TEXTURE KEYS", EditorStyles.boldLabel);

            //  albedo
            if (EditorPrefs.HasKey("Albedo Unique Name"))
                albedoName = EditorPrefs.GetString("Albedo Unique Name");
            else
                EditorPrefs.SetString("Albedo Unique Name", albedoName);
            albedoName = EditorGUILayout.TextField(new GUIContent("Albedo", textFieldTooltip), albedoName, GUILayout.MaxWidth(300));
            EditorPrefs.SetString("Albedo Unique Name", albedoName);

            //  metallic
            if (EditorPrefs.HasKey("Metallic Unique Name"))
                metallicName = EditorPrefs.GetString("Metallic Unique Name");
            else
                EditorPrefs.SetString("Metallic Unique Name", metallicName);
            metallicName = EditorGUILayout.TextField(new GUIContent("Metallic/Smoothness", textFieldTooltip), metallicName, GUILayout.MaxWidth(300));
            EditorPrefs.SetString("Metallic Unique Name", metallicName);

            //  emission
            if (EditorPrefs.HasKey("Emission Unique Name"))
                emissionName = EditorPrefs.GetString("Emission Unique Name");
            else
                EditorPrefs.SetString("Emission Unique Name", emissionName);
            emissionName = EditorGUILayout.TextField(new GUIContent("Emission", textFieldTooltip), emissionName, GUILayout.MaxWidth(300));
            EditorPrefs.SetString("Emission Unique Name", emissionName);

            //  normal
            if (EditorPrefs.HasKey("Normal Unique Name"))
                normalName = EditorPrefs.GetString("Normal Unique Name");
            else
                EditorPrefs.SetString("Normal Unique Name", normalName);
            normalName = EditorGUILayout.TextField(new GUIContent("Normal", textFieldTooltip), normalName, GUILayout.MaxWidth(300));
            EditorPrefs.SetString("Normal Unique Name", normalName);

            //  height
            if (EditorPrefs.HasKey("Height Unique Name"))
                heightName = EditorPrefs.GetString("Height Unique Name");
            else
                EditorPrefs.SetString("Height Unique Name", heightName);
            heightName = EditorGUILayout.TextField(new GUIContent("Height", textFieldTooltip), heightName, GUILayout.MaxWidth(300));
            EditorPrefs.SetString("Height Unique Name", heightName);

            //  occlusion
            if (EditorPrefs.HasKey("Occlusion Unique Name"))
                occlusionName = EditorPrefs.GetString("Occlusion Unique Name");
            else
                EditorPrefs.SetString("Occlusion Unique Name", occlusionName);
            occlusionName = EditorGUILayout.TextField(new GUIContent("Occlusion", textFieldTooltip), occlusionName, GUILayout.MaxWidth(300));
            EditorPrefs.SetString("Occlusion Unique Name", occlusionName);

            GUILayout.Space(5);

            //  button to reset texture names
            if (GUILayout.Button(new GUIContent("Reset Keys to Default", "Resets all texture names to default values"), GUILayout.ExpandWidth(false), GUILayout.MaxWidth(300)))
            {
                ResetNames();
            }

            GUILayout.Space(10);

            //  ASSIGN SETTINGS
            GUILayout.Label("ASSIGN SETTINGS", EditorStyles.boldLabel);

            //  Normal map fix toggle
            if (EditorPrefs.HasKey("Normal Map Fix"))
                autoFixNormals = EditorPrefs.GetBool("Normal Map Fix");
            else
                EditorPrefs.SetBool("Normal Map Fix", autoFixNormals);
            autoFixNormals = EditorGUILayout.Toggle(new GUIContent("Auto Fix Normal Maps", normalFixTooltip), autoFixNormals);
            EditorPrefs.SetBool("Normal Map Fix", autoFixNormals);

            GUILayout.Space(10);

            //  HOTKEYS
            GUILayout.Label("HOTKEYS", EditorStyles.boldLabel);
            GUILayout.Label(hotkeyTooltipLabel, EditorStyles.wordWrappedMiniLabel);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Assign Hotkey", EditorStyles.label);
            GUILayout.Label(new GUIContent(GetHotKey(Hotkeys_ET.ET_ASSIGN_HOTKEY), hotkeyTooltip), EditorStyles.largeLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Create Hotkey", EditorStyles.label);
            GUILayout.Label(new GUIContent(GetHotKey(Hotkeys_ET.ET_CREATE_HOTKEY), hotkeyTooltip), EditorStyles.largeLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Clear Hotkey", EditorStyles.label);
            GUILayout.Label(new GUIContent(GetHotKey(Hotkeys_ET.ET_CLEAR_HOTKEY), hotkeyTooltip), EditorStyles.largeLabel);
            GUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Resets names to default values 
        /// </summary>
        private void ResetNames()
        {
            albedoName = ALBEDO_NAME;
            EditorPrefs.SetString("Albedo Unique Name", albedoName);

            metallicName = METALLIC_NAME;
            EditorPrefs.SetString("Metallic Unique Name", metallicName);

            emissionName = EMISSION_NAME;
            EditorPrefs.SetString("Emission Unique Name", emissionName);

            normalName = NORMAL_NAME;
            EditorPrefs.SetString("Normal Unique Name", normalName);

            heightName = HEIGHT_NAME;
            EditorPrefs.SetString("Height Unique Name", heightName);

            occlusionName = OCCLUSION_NAME;
            EditorPrefs.SetString("Occlusion Unique Name", occlusionName);

        }

        /// <summary>
        /// Converts hotkey symbols to easy to read string
        /// </summary>
        private string GetHotKey(string hotkey)
        {
            if (string.IsNullOrEmpty(hotkey)) return "None";

            string modifiers = "";

            if (hotkey.ToLower().Contains("%"))
            {
                modifiers += "Ctrl + ";
            }
            if (hotkey.ToLower().Contains("#"))
            {
                modifiers += "Shift + ";
            }
            if (hotkey.ToLower().Contains("&"))
            {
                modifiers += "Alt + ";
            }
            string key = string.Join("", hotkey.Split('_', '%', '#', '&')).ToUpper();
            return modifiers + key;
        }
    }
}
