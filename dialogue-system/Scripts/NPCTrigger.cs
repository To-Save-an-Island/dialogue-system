using Godot;

public partial class NPCTrigger : Area2D
{
    [Export]
    public string DialogueStartNode = "start";

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            GD.Print("Player entered NPC trigger area. Starting dialogue..."); //DEBUG
            var manager = GetNode<DialogueManager>("/root/DialogueManager");
            manager.StartDialogue(DialogueStartNode);
        }
    }
}
