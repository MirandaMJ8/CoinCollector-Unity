using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// Eina d'editor per generar les escenes del joc.
// Menú: Eines > Generar escenes
public static class LevelSetup
{
    [MenuItem("Eines/Generar escenes")]
    public static void Generate()
    {
        AddTag("Ground");

        Material floorMat = MakeMaterial("FloorMat", new Color(0.2f, 0.24f, 0.32f), 0f, 0.3f, false);
        Material obstacleMat = MakeMaterial("ObstacleMat", new Color(0.55f, 0.58f, 0.65f), 0f, 0.4f, false);
        Material ballMat = MakeMaterial("BallMat", new Color(0.92f, 0.32f, 0.15f), 0.2f, 0.8f, false);
        Material coinMat = MakeMaterial("CoinMat", new Color(1f, 0.84f, 0.2f), 0.4f, 0.7f, true);

        GameObject coinPrefab = MakeCoinPrefab(coinMat);

        MakeLevel(floorMat, obstacleMat, ballMat, coinPrefab);
        MakeMenu();

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MenuPrincipal.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Level1.unity", true),
        };

        if (AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/SampleScene.unity") != null)
        {
            AssetDatabase.DeleteAsset("Assets/Scenes/SampleScene.unity");
        }
        AssetDatabase.SaveAssets();
        Debug.Log("Escenes generades");
    }

    private static void AddTag(string newTag)
    {
        var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagList = tagManager.FindProperty("tags");

        for (int i = 0; i < tagList.arraySize; i++)
        {
            if (tagList.GetArrayElementAtIndex(i).stringValue == newTag) return;
        }

        tagList.arraySize++;
        tagList.GetArrayElementAtIndex(tagList.arraySize - 1).stringValue = newTag;
        tagManager.ApplyModifiedPropertiesWithoutUndo();
    }

    private static Material MakeMaterial(string fileName, Color color, float metallic, float smoothness, bool glow)
    {
        string path = "Assets/Materials/" + fileName + ".mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        bool isNew = mat == null;
        if (isNew)
        {
            mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        }

        mat.SetColor("_BaseColor", color);
        mat.SetFloat("_Metallic", metallic);
        mat.SetFloat("_Smoothness", smoothness);

        if (glow)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", color * 0.6f);
        }

        if (isNew)
        {
            AssetDatabase.CreateAsset(mat, path);
        }
        else
        {
            EditorUtility.SetDirty(mat);
        }
        return mat;
    }

    private static GameObject MakeCoinPrefab(Material coinMat)
    {
        // Moneda en forma de gemma: un cub petit inclinat
        GameObject coin = GameObject.CreatePrimitive(PrimitiveType.Cube);
        coin.name = "Coin";
        coin.transform.localScale = Vector3.one * 0.45f;
        coin.transform.rotation = Quaternion.Euler(45f, 0f, 45f);
        coin.GetComponent<Renderer>().sharedMaterial = coinMat;

        Object.DestroyImmediate(coin.GetComponent<BoxCollider>());
        var trigger = coin.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 0.9f;

        coin.AddComponent<Coin>();
        coin.AddComponent<Spinner>();
        coin.AddComponent<CoinFloat>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(coin, "Assets/Prefabs/Coin.prefab");
        Object.DestroyImmediate(coin);
        return prefab;
    }

    private static void MakeLevel(Material floorMat, Material obstacleMat, Material ballMat, GameObject coinPrefab)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Terra de 24 x 16
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.tag = "Ground";
        floor.transform.localScale = new Vector3(2.4f, 1f, 1.6f);
        floor.GetComponent<Renderer>().sharedMaterial = floorMat;

        // Límits invisibles
        Transform limits = new GameObject("Limits").transform;
        AddLimit(limits, "Limit_Front", new Vector3(0f, 1f, 8f), new Vector3(24f, 2f, 1f));
        AddLimit(limits, "Limit_Back", new Vector3(0f, 1f, -8f), new Vector3(24f, 2f, 1f));
        AddLimit(limits, "Limit_Right", new Vector3(12f, 1f, 0f), new Vector3(1f, 2f, 16f));
        AddLimit(limits, "Limit_Left", new Vector3(-12f, 1f, 0f), new Vector3(1f, 2f, 16f));

        // Obstacles (pilars sòlids)
        Transform obstacles = new GameObject("Obstacles").transform;
        Vector3[] pillarPositions = { new Vector3(-5f, 1f, 2f), new Vector3(0f, 1f, -2.5f), new Vector3(5f, 1f, 2f) };
        foreach (Vector3 p in pillarPositions)
        {
            GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pillar.name = "Pillar";
            pillar.transform.SetParent(obstacles);
            pillar.transform.position = p;
            pillar.transform.localScale = new Vector3(1.5f, 2f, 1.5f);
            pillar.GetComponent<Renderer>().sharedMaterial = obstacleMat;
        }

        // KillZone
        GameObject killZone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        killZone.name = "KillZone";
        killZone.transform.position = new Vector3(0f, -8f, 0f);
        killZone.transform.localScale = new Vector3(150f, 1f, 150f);
        killZone.GetComponent<Renderer>().enabled = false;
        killZone.GetComponent<Collider>().isTrigger = true;
        killZone.AddComponent<KillZone>();

        // Jugador
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = "Ball";
        ball.tag = "Player";
        ball.transform.position = new Vector3(-9f, 0.5f, -5f);
        ball.GetComponent<Renderer>().sharedMaterial = ballMat;
        var rb = ball.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        ball.AddComponent<PlayerController>();

        // GameManager
        var gm = new GameObject("GameManager");
        gm.AddComponent<GameManager>();

        // Monedes en ziga-zaga
        Transform coinParent = new GameObject("Coins").transform;
        Vector3[] coinPositions =
        {
            new Vector3(-9f, 0.8f, 5f), new Vector3(-7f, 0.8f, -1f), new Vector3(-3f, 0.8f, 5.5f),
            new Vector3(-2f, 0.8f, -5.5f), new Vector3(0f, 0.8f, 2f), new Vector3(2.5f, 0.8f, -5f),
            new Vector3(3f, 0.8f, 5.5f), new Vector3(7f, 0.8f, -1f), new Vector3(9f, 0.8f, 5f),
            new Vector3(9.5f, 0.8f, -5.5f),
        };
        foreach (Vector3 p in coinPositions)
        {
            var c = (GameObject)PrefabUtility.InstantiatePrefab(coinPrefab, coinParent);
            c.transform.position = p;
        }

        // Càmera en diagonal des d'una cantonada
        Camera cam = Camera.main;
        cam.transform.position = new Vector3(-14f, 16f, -19f);
        cam.transform.LookAt(new Vector3(0f, 0f, -1f));
        cam.fieldOfView = 50f;

        // Llum amb un angle diferent
        Light sun = Object.FindFirstObjectByType<Light>();
        sun.transform.rotation = Quaternion.Euler(60f, 45f, 0f);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Level1.unity");
    }

    private static void AddLimit(Transform parent, string limitName, Vector3 position, Vector3 size)
    {
        GameObject limit = GameObject.CreatePrimitive(PrimitiveType.Cube);
        limit.name = limitName;
        limit.transform.SetParent(parent);
        limit.transform.position = position;
        limit.transform.localScale = size;
        limit.GetComponent<MeshRenderer>().enabled = false;
        limit.GetComponent<BoxCollider>().isTrigger = true;
    }

    private static void MakeMenu()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        Camera cam = Camera.main;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.08f, 0.1f, 0.18f);

        var canvasObj = new GameObject("Canvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        canvasObj.AddComponent<GraphicRaycaster>();

        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<InputSystemUIInputModule>();

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        MakeText(canvasObj.transform, "Title", "COIN COLLECTOR 3D", font, 72, new Color(1f, 0.84f, 0.2f), new Vector2(0f, 150f), new Vector2(1000f, 120f));
        MakeText(canvasObj.transform, "Subtitle", "Recull totes les monedes!", font, 32, Color.white, new Vector2(0f, 70f), new Vector2(800f, 60f));

        var buttonObj = new GameObject("PlayButton");
        buttonObj.transform.SetParent(canvasObj.transform, false);
        var rect = buttonObj.AddComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0f, -60f);
        rect.sizeDelta = new Vector2(260f, 80f);
        buttonObj.AddComponent<Image>().color = new Color(0.92f, 0.32f, 0.15f);
        var button = buttonObj.AddComponent<Button>();

        Text label = MakeText(buttonObj.transform, "Label", "Play", font, 40, Color.white, Vector2.zero, new Vector2(260f, 80f));
        label.fontStyle = FontStyle.Bold;

        var menuObj = new GameObject("Menu");
        var menu = menuObj.AddComponent<MenuController>();
        UnityEventTools.AddPersistentListener(button.onClick, menu.OnPlayPressed);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MenuPrincipal.unity");
    }

    private static Text MakeText(Transform parent, string objName, string content, Font font, int size, Color color, Vector2 pos, Vector2 boxSize)
    {
        var obj = new GameObject(objName);
        obj.transform.SetParent(parent, false);
        var rect = obj.AddComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = boxSize;

        var text = obj.AddComponent<Text>();
        text.text = content;
        text.font = font;
        text.fontSize = size;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        return text;
    }
}
