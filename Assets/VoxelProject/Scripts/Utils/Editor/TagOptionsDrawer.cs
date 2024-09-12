using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TagOptionsAttribute))]

//Shows the options in the tag dropdown - attatched to world manager via tag options attribute
public class TagOptionsDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get all tags dynamically
        string[] tags = UnityEditorInternal.InternalEditorUtility.tags;

        if (property.propertyType == SerializedPropertyType.String)
        {
            // Find the current tag index
            int index = Mathf.Max(0, System.Array.IndexOf(tags, property.stringValue));
            // Create a dropdown list
            index = EditorGUI.Popup(position, label.text, index, tags);
            // Set the selected tag
            property.stringValue = tags[index];
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.HelpBox(position, "TagOptions can only be used with string fields.", MessageType.Error);
        }
    }
}
