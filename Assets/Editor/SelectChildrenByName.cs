using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SelectChildrenByName : EditorWindow
{
    private string searchString = "obj";
    private bool caseSensitive = false;

    [MenuItem("Tools/Sélectionner enfants par nom")]
    public static void ShowWindow()
    {
        GetWindow<SelectChildrenByName>("Sélection enfants");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Sélectionner les enfants contenant :", EditorStyles.boldLabel);
        searchString = EditorGUILayout.TextField("Texte à rechercher :", searchString);
        caseSensitive = EditorGUILayout.Toggle("Sensible à la casse", caseSensitive);

        if (GUILayout.Button("Sélectionner les objets correspondants"))
        {
            SelectMatchingChildren();
        }
    }

    private void SelectMatchingChildren()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            EditorUtility.DisplayDialog("Aucun objet sélectionné",
                "Sélectionne d'abord un GameObject dans la hiérarchie.",
                "OK");
            return;
        }

        List<GameObject> matches = new List<GameObject>();
        SearchRecursive(selected.transform, matches);

        if (!caseSensitive)
            searchString = searchString.ToLower();

        matches.RemoveAll(obj =>
            !(caseSensitive ? obj.name.Contains(searchString) : obj.name.ToLower().Contains(searchString))
        );

        if (matches.Count == 0)
        {
            EditorUtility.DisplayDialog("Aucun résultat",
                $"Aucun enfant ne contient \"{searchString}\" dans son nom.",
                "OK");
            return;
        }

        Selection.objects = matches.ToArray();
        Debug.Log($"✅ {matches.Count} objets trouvés contenant \"{searchString}\".");
    }

    private void SearchRecursive(Transform parent, List<GameObject> results)
    {
        foreach (Transform child in parent)
        {
            results.Add(child.gameObject);
            SearchRecursive(child, results);
        }
    }
}
