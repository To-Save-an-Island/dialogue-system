using System.Collections.Generic;
using System.Text.Json;

public class DialogueNode
{
    public string Id;
    public string Text;
    public List<Choice> Choices = new();
    // TODO: Right now conditions are just a list of strings, logic on how conditions are 
    // evaluated is not yet clear
    public List<string> Conditions = new();
    public List<string> Events = new();
    public string Next;

    public DialogueNode(JsonElement nodeJson)
    {
        Id = nodeJson.GetProperty("id").GetString();
        Text = nodeJson.GetProperty("text").GetString();

        if (nodeJson.TryGetProperty("choices", out var choicesJson))
            foreach (var choice in choicesJson.EnumerateArray())
                Choices.Add(new Choice(choice));

        if (nodeJson.TryGetProperty("conditions", out var condJson))
            foreach (var cond in condJson.EnumerateArray())
                Conditions.Add(cond.GetString());

        if (nodeJson.TryGetProperty("events", out var eventsJson))
            foreach (var ev in eventsJson.EnumerateArray())
                Events.Add(ev.GetString());

        if (nodeJson.TryGetProperty("next", out var nextJson))
            Next = nextJson.GetString();
    }

    public bool ConditionsMet(HashSet<string> flags)
    {
        foreach (string cond in Conditions)
        {
            if (cond.StartsWith("!"))
            {
                if (flags.Contains(cond.Substring(1)))
                    return false;
            }
            else
            {
                if (!flags.Contains(cond))
                    return false;
            }
        }
        return true;
    }
}

public class Choice
{
    public string Text;
    public string Next;
    public List<string> SetFlags = new();

    public Choice(JsonElement choiceJson)
    {
        Text = choiceJson.GetProperty("text").GetString();
        Next = choiceJson.GetProperty("next").GetString();

        if (choiceJson.TryGetProperty("set_flags", out var flagsJson))
            foreach (var flag in flagsJson.EnumerateArray())
                SetFlags.Add(flag.GetString());
    }
}

