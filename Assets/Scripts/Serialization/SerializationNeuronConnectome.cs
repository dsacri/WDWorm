using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class SerializationNeuronConnectome
{

    /// <summary>
    /// get neuron names
    /// </summary>
    public static void AddNames(List<NeuronManager.DeserializedNeuron> deserializedNeurons)     //index, name
    {
        //reading file from location
        StringReader reader = SerializationUtil.GetStringReader(SerializationUtil.Paths.NeuronNames);

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            //storing to variable
            string[] dataValues = line.Split(';');

            int.TryParse(dataValues[1], out int index);
            string name = StandardizeNeuronName(dataValues[0]);

            bool foundIndexOrName = false;

            foreach (NeuronManager.DeserializedNeuron deserializedNeuron in deserializedNeurons)
            {
                if (deserializedNeuron.Index == index || deserializedNeuron.Name == name)
                {
                    deserializedNeuron.Index = index;
                    deserializedNeuron.Name = name;
                    foundIndexOrName = true;
                }
            }

            if (!foundIndexOrName)
            {
                NeuronManager.DeserializedNeuron deserializedNeuron = new()
                {
                    Index = index,
                    Name = name
                };
                deserializedNeurons.Add(deserializedNeuron);
            }
        }

        reader.Close();
    }

    /// <summary>
    /// get neuron positions
    /// </summary>
    public static void AddPositions(List<NeuronManager.DeserializedNeuron> deserializedNeurons)     //name, position
    {
        //reading file from location
        StringReader reader = SerializationUtil.GetStringReader(SerializationUtil.Paths.NeuronPositions);
        
        bool firstLine = true;
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (firstLine)
            {
                firstLine = false;
                continue;
            }

            //storing to variable
            string[] dataValues = line.Split(';');

            Vector3 vector = new();
            float.TryParse(dataValues[1], out vector.x);
            float.TryParse(dataValues[2], out vector.y);
            float.TryParse(dataValues[3], out vector.z);
            string name = StandardizeNeuronName(dataValues[0]);

            foreach (NeuronManager.DeserializedNeuron deserializedNeuron in deserializedNeurons)
            {
                if (deserializedNeuron.Name == name)
                {
                    deserializedNeuron.Position = vector;
                }
            }
        }

        reader.Close();
    }

    /// <summary>
    /// get neuron groups
    /// </summary>
    public static void AddNeuronGroups(List<NeuronManager.DeserializedNeuron> deserializedNeurons, Dictionary<string, NeuronManager.Group> groupAbbrDictionary)      //name, group(s) abbr, group(s) name
    {
        StringReader reader = SerializationUtil.GetStringReader(SerializationUtil.Paths.NeuronTypes);
        
        bool firstLine = true;
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (firstLine)
            {
                firstLine = false;
                continue;
            }

            List<string> dataValues = new();
            StringBuilder currentElement = new();
            bool insideQuotes = false;

            foreach (char c in line)
            {
                if (c == '"')
                {
                    insideQuotes = !insideQuotes;
                }
                else if (c == ',' && !insideQuotes)
                {
                    dataValues.Add(currentElement.ToString());
                    currentElement.Clear();
                }
                else
                {
                    currentElement.Append(c);
                }
            }

            // last string (no , at the end of line)
            if (currentElement.Length > 0)
            {
                dataValues.Add(currentElement.ToString());
            }

            string neuronName = StandardizeNeuronName(dataValues[5]);
            string groupName = StandardizeNormalName(dataValues[4]);

            SetGroups(neuronName, groupName, deserializedNeurons, groupAbbrDictionary);
        }

        reader.Close();
    }

    /// <summary>
    /// add neuron to a group
    /// </summary>
    private static void SetGroups(string neuronName, string groupName, List<NeuronManager.DeserializedNeuron> deserializedNeurons, Dictionary<string, NeuronManager.Group> groupDictionary)
    {
        if (groupName.Equals(StandardizeNormalName("Muscle cell")))
            return;

        if (!groupDictionary.ContainsKey(groupName))
        {
            NeuronManager.Group group = new()
            {
                Name = groupName
            };

            groupDictionary.Add(groupName, group);
        }

        bool foundNeuronName = false;

        foreach (NeuronManager.DeserializedNeuron deserializedNeuron in deserializedNeurons)
        {
            if (neuronName == deserializedNeuron.Name)
            {
                deserializedNeuron.Group = groupDictionary[groupName];
                groupDictionary[groupName].Members.Add(deserializedNeuron);
                foundNeuronName = true;
            }
        }

        if (!foundNeuronName)
        {
            NeuronManager.DeserializedNeuron deserializedNeuron = new()
            {
                Name = neuronName,
                Group = groupDictionary[groupName]
            };
            deserializedNeurons.Add(deserializedNeuron);
            groupDictionary[groupName].Members.Add(deserializedNeuron);
        }
    }

    /// <summary>
    /// uniform neuron names to avoid string comparison mistakes
    /// </summary>
    public static string StandardizeNeuronName(string name)
    {
        if (name.Length >= 4 && name[2] == '0')
            name = name.Remove(2, 1);

        return StandardizeNormalName(name);
    }

    /// <summary>
    /// uniform names to avoid string comparison mistakes
    /// </summary>
    public static string StandardizeNormalName(string name)
    {
        return name.ToUpper();
    }

}
