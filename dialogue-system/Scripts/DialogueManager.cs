using Godot;
using System.Collections.Generic;
using System.Text.Json;

public partial class DialogueManager : Node
{
    public Dictionary<string, DialogueNode> Nodes = new();
    public DialogueNode CurrentNode;
    public HashSet<string> Flags = new();
    private Player player;

    [Signal]
    public delegate void NodeChangedEventHandler(string nodeId);

    public override void _Ready()
    {
        LoadDialogue("res://DialogueData/sample_dialogue.json");
        var players = GetTree().GetNodesInGroup("Player");
        if (players.Count > 0)
            player = players[0] as Player;

        GD.Print("Dialogue Manager is ready."); // DEBUG
    }

    public void LoadDialogue(string jsonPath)
    {
        var file = FileAccess.Open(jsonPath, FileAccess.ModeFlags.Read);
        string json = file.GetAsText();
        var doc = JsonDocument.Parse(json);

        foreach (var node in doc.RootElement.GetProperty("nodes").EnumerateArray())
        {
            var dialogueNode = new DialogueNode(node);
            Nodes[dialogueNode.Id] = dialogueNode;
        }
    }

    public void StartDialogue(string startId)
    {
        GD.Print($"Starting dialogue at node: {startId}"); // DEBUG
        player?.SetMovementEnabled(false); // Disable player movement during dialogue
        AdvanceToNode(startId);
    }

    public void EndDialogue()
    {
        GD.Print("Ending dialogue."); // DEBUG
        player?.SetMovementEnabled(true); // Enable movement
        CurrentNode = null;
    }

    public void AdvanceToNode(string id)
    {
        if (!Nodes.ContainsKey(id))
        {
            GD.PrintErr($"Dialogue node {id} not found.");
            return;
        }

        var node = Nodes[id];

        // TODO: Test the conditions functionality
        if (!node.ConditionsMet(Flags))
        {
            GD.Print("Conditions not met, dialogue ends.");
            return;
        }

        CurrentNode = node;
        EmitSignal(SignalName.NodeChanged, node.Id);

        if (node.Events != null)
        {
            foreach (string ev in node.Events)
                ExecuteEvent(ev);
        }

        if (node.Next != null && node.Choices.Count == 0)
            AdvanceToNode(node.Next);
    }

    public void SelectChoice(int index)
    {
        var choice = CurrentNode.Choices[index];
        if (choice.SetFlags != null)
        {
            foreach (string flag in choice.SetFlags)
                Flags.Add(flag);
        }
        AdvanceToNode(choice.Next);
    }

    private void ExecuteEvent(string ev)
    {
        // Will have to define how events are handled
        // For now, just print the event name
        GD.Print($"Event triggered: {ev}");
    }
}
