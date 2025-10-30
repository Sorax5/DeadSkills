using UnityEngine;
using UnityEditor;

public class RenameObjects : EditorWindow
{
    private string baseName = "obj";
    private int startNumber = 0;
    private bool useUnderscore = true;

    [MenuItem("Tools/Renommer les objets sélectionnés")]
    public static void ShowWindow()
    {
        GetWindow<RenameObjects>("Renommer objets");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Paramètres de renommage", EditorStyles.boldLabel);
        baseName = EditorGUILayout.TextField("Nom de base :", baseName);
        startNumber = EditorGUILayout.IntField("Numéro de départ :", startNumber);
        useUnderscore = EditorGUILayout.Toggle("Utiliser un underscore (_)", useUnderscore);

        if (GUILayout.Button("Renommer la sélection"))
        {
            RenameSelectedObjects();
        }
    }

    private void RenameSelectedObjects()
    {
        GameObject[] selected = Selection.gameObjects;

        if (selected.Length == 0)
        {
            EditorUtility.DisplayDialog("Aucun objet sélectionné",
                "Sélectionne au moins un objet dans la hiérarchie avant de renommer.",
                "OK");
            return;
        }

        // On garde l'ordre dans la hiérarchie pour un renommage cohérent
        System.Array.Sort(selected, (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

        Undo.RecordObjects(selected, "Renommer objets");

        for (int i = 0; i < selected.Length; i++)
        {
            string separator = useUnderscore ? "_" : "";
            selected[i].name = $"{baseName}{separator}{startNumber + i}";
        }

        Debug.Log($"✅ {selected.Length} objets renommés en \"{baseName}_#\"");
    }
}
