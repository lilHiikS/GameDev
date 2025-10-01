using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public DialogueDataEntry[] entrys;
}

[System.Serializable]
public class DialogueDataEntry
{
    public string Name;
    [TextArea(3, 10)]
    public string[] lines;
}
