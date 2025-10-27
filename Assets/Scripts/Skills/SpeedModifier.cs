using System.Collections.Generic;

public class SpeedModifier
{
    private Dictionary<string, float> modifiers = new Dictionary<string, float>();

    public void SetModifier(string key, float value)
    {
        modifiers[key] = value;
    }

    public void RemoveModifier(string key)
    {
        if (modifiers.ContainsKey(key))
        {
            modifiers.Remove(key);
        }
    }

    public float GetModifier(string key, float defaultValue = 1f)
    {
        if (modifiers.TryGetValue(key, out float value))
        {
            return value;
        }
        return defaultValue;
    }

    public float ApplyAllModifiers()
    {
        float finalModifier = 1f;
        foreach (var modifier in modifiers.Values)
        {
            finalModifier *= modifier;
        }
        return finalModifier;
    }
}
