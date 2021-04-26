using UnityEngine;
using UnityEditor;
namespace MD.CraftingSystem
{
    [CustomEditor(typeof(CraftingRecipe))]
    public class CraftingRecipeSaveButton : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = (CraftingRecipe) target;
            if (GUILayout.Button("Save Recipes"))
            {
                script.SaveRecipes();
            }
        }
    }
}