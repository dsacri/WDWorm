
/// <summary>
/// settings menu with savable data
/// </summary>
public interface ISaveable
{
    /// <summary>
    /// load data from container
    /// </summary>
    public void Load(MatlabSerializedData matlabSerializedData);
    /// <summary>
    /// save data to container
    /// </summary>
    public void Save(MatlabSerializedData matlabSerializedData);
}
