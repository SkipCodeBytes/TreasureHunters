using UnityEngine;

public class DynamicComponentRequirementAttribute : PropertyAttribute
{
    public string controllingFieldName;

    public DynamicComponentRequirementAttribute(string controllingFieldName)
    {
        this.controllingFieldName = controllingFieldName;
    }
}
