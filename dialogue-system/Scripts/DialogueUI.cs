using Godot;

public partial class DialogueUI : Control
{
    private Label dialogueLabel;
    private VBoxContainer choicesContainer;
    private DialogueManager dialogueManager;

    public override void _Ready()
    {
        GD.Print("Dialogue UI is ready.");
        dialogueLabel = GetNode<Label>("Panel/Label");
        choicesContainer = GetNode<VBoxContainer>("Panel/VBoxContainer");
        dialogueManager = GetNode<DialogueManager>("/root/DialogueManager");

        dialogueManager.NodeChanged += OnNodeChanged;
        Hide();
    }

    private async void OnNodeChanged(string nodeId)
    {
        var node = dialogueManager.Nodes[nodeId];
        Show();
        dialogueLabel.Text = node.Text;

        foreach (var child in choicesContainer.GetChildren())
            child.QueueFree();

        for (int i = 0; i < node.Choices.Count; i++)
        {
            var choice = node.Choices[i];
            var btn = new Button
            {
                Text = choice.Text
            };
            int idx = i;
            btn.Pressed += () => OnChoiceSelected(idx);
            choicesContainer.AddChild(btn);
        }

        // Auto advance if no choices and a next node is available
        if (node.Next != null && node.Choices.Count == 0)
        {
            TimerAdvance(node.Next, 2.0f);
        }

        // Auto end if no choices and no next node
        if (node.Next == null && node.Choices.Count == 0)
        {
            GD.Print("No choices or next node, ending dialogue.");
            // Delay 2 secs
            // TODO: This should be a signal to the DialogueManager to end the dialogue
            await ToSignal(GetTree().CreateTimer(2.0f), "timeout");
            dialogueLabel.Text = ""; // Clear dialogue text
            dialogueManager.CurrentNode = null; // Clear current node
            Hide();

            dialogueManager.EndDialogue(); // Notify DialogueManager to end dialogue
            return;
        }
    }

    private async void Timer(float delay)
    {
        await ToSignal(GetTree().CreateTimer(delay), "timeout");
    }

    private async void TimerAdvance(string nextId, float delay)
    {
        await ToSignal(GetTree().CreateTimer(delay), "timeout");
        dialogueManager.AdvanceToNode(nextId);
    }

    private void OnChoiceSelected(int index)
    {
        dialogueManager.SelectChoice(index);
    }
}
