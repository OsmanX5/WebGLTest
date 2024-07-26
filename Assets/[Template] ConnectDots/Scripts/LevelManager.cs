using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public TextAsset m_LevelFile;

    public List<Level> m_Levels = new List<Level>();

    public void Start() {
        m_Levels = GetAllLevels();
    }

    //Loading all the levels in the text file
    public List<Level> GetAllLevels() {
        List<Level> result = new List<Level>();

        if (m_LevelFile != null) {
            string[] lines = Regex.Split(m_LevelFile.text, "\n|\r|\r|\n");
            for (int i = 0; i < lines.Length; i++)
            {
                int index = i;
                Level tempLevel = new Level(0);
                while (lines[i] == "Level")
                {

                    index++;
                    if (index == lines.Length)
                    {
                        if (tempLevel.dotsXY > 0)
                        {
                            result.Add(tempLevel);
                        }
                        i = index;
                        break;
                    }
                    if (lines[index].StartsWith("Dots"))
                    {
                        string tempStr = lines[index].Replace("Dots:[", "").Replace("]", "");
                        tempLevel.dotsXY = int.Parse(tempStr);
                    }
                    else if (lines[index].StartsWith("YAxis"))
                    {
                        string tempStr = lines[index].Replace("YAxis:[", "").Replace("]", "");

                        if (tempStr.Contains("True"))
                        {
                            tempLevel.AddOffsetOnYAxis();
                        }
                        //tempLevel.SetOffset = bool.Parse(tempStr);
                    }
                    else if (lines[index].StartsWith("GradientColors"))
                    {
                        string tempStr = lines[index].Replace("GradientColors:[", "").Replace("]", "");
                        string[] colors = tempStr.Split('|');
                        string[] topColor = colors[0].Split(',');
                        string[] botColor = colors[1].Split(',');
                        tempLevel.m_Top = new Color(float.Parse(topColor[0]), float.Parse(topColor[1]),
                                                    float.Parse(topColor[2]), float.Parse(topColor[3]));
                        tempLevel.m_Bottom = new Color(float.Parse(botColor[0]), float.Parse(botColor[1]),
                                                       float.Parse(botColor[2]), float.Parse(botColor[3]));

                    }
                    else if (lines[index].StartsWith("Connectors"))
                    {
                        string tempStr = lines[index].Replace("Connectors:[", "").Replace("]", "");
                        tempLevel.connectorsXY = tempStr.ToListInt();
                    }
                    else if (lines[index].StartsWith("Disabled"))
                    {
                        string tempStr = lines[index].Replace("Disabled:[", "").Replace("]", "");
                        tempLevel.disabledDotsXY = tempStr.ToListInt();
                    }
                    else if (lines[index].StartsWith("Removed"))
                    {
                        string tempStr = lines[index].Replace("Removed:[", "").Replace("]", "");
                        tempLevel.removedDotsXY = tempStr.ToListInt();
                    }
                    else if (lines[index].StartsWith("Multi"))
                    {
                        string tempStr = lines[index].Replace("Multi:[", "").Replace("]", "");
                        tempLevel.multiConnectorsXY = tempStr.ToListInt();
                    }
                    else if (lines[index].StartsWith("Portals"))
                    {
                        string tempStr = lines[index].Replace("Portals:[", "").Replace("]", "");
                        tempLevel.portalsXY = tempStr.ToListVector2Int();
                    }
                    else if (lines[index].StartsWith("Colors"))
                    {
                        string tempStr = lines[index].Replace("Colors:[", "").Replace("]", "");
                        tempLevel.colorsXY = tempStr.ToListColor();
                    }
                    else if (lines[index].StartsWith("Level"))
                    {
                        if (tempLevel.dotsXY > 0)
                        {
                            result.Add(tempLevel);
                        }
                        i = index - 1;
                    }
                }
            }
        }

        return result;
    }
}
