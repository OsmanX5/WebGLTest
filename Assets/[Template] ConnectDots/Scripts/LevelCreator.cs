using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CTemplate { 
    public class LevelCreator : MonoBehaviour {

        public GameManager m_GameManager;

        [SerializeField]
        public Level m_Level;

        private List<Level> m_Levels;

        public int DotsXY;
        public float RadiusXY;

        [HideInInspector]
        public Dot SelectedDot;

        public TextAsset m_LevelsTextFile;

        public bool isAddingPortal;

        public void Start() {
            m_Levels = new List<Level>();
            m_Level = new Level(0);
            InitLevels();
        }

        public void Update()
        {
            if (this.m_GameManager == null)
            {
                m_GameManager = this.GetComponent<GameManager>();
            }
            else 
            {
                m_Level.dotsXY = m_GameManager.m_DotsXY;
            }
        }

        public void AddDisabled() {
            if (SelectedDot.isActive)
            {
                m_GameManager.DisableDot(SelectedDot);
            }
        }

        public void AddDisabled(int index)
        {
            m_Level.AddDisabledDot(index);
        }

        public void AddRemoved() {
            if (SelectedDot.isActive)
            {
                m_GameManager.RemoveDot(SelectedDot);
            }
        }

        public void AddRemoved(int index) {
            m_Level.AddRemovedDot(index);
        }

        public void AddConnector() {
            if (SelectedDot.isActive) {
                if (!SelectedDot.isMultiConnector && !SelectedDot.isPortal)
                {
                    m_GameManager.AddConnector(SelectedDot);
                }
            }
        }

        public void ConnectorAdded(int index, Color color) {
            m_Level.AddConnector(index, color);
        }

        public void AddMultiConnector() {
            if (SelectedDot.isActive) {
                if (!SelectedDot.isMultiConnector && !SelectedDot.isPortal)
                {
                    m_GameManager.AddMultiConnector(SelectedDot);
                }
            }
        }

        public void MultiConnectorAdded(int index) 
        {
            this.m_Level.AddMultiConnector(index);
        }

        public void AddPortal() {
            this.isAddingPortal = true;
        }

        public void PortalAdded(int a, int b) {
            //a = x, b = y (It does'nt really matter if its switched)
            this.m_Level.AddPortal(new Vector2Int(a, b));
        }

        public void AddDot(Dot newDot) {
            if (isAddingPortal)
            {
                if (SelectedDot != null)
                {
                    Dot a = SelectedDot;
                    Dot b = newDot;
                    if (!a.isMultiConnector && !a.isPortal && !b.isMultiConnector && !b.isPortal)
                    {
                        m_GameManager.AddPortal(a, b);
                    }
                    isAddingPortal = false;
                }
            }
            else {
                SelectedDot = newDot;
            }
        }

        public void CreateNewLevel() {
            SelectedDot = null;
            isAddingPortal = false;
            m_GameManager.Start();
        }

        public void SaveLevel() {
            if (Application.isEditor)
            {
                if (m_GameManager.m_OffsetYAxis) {
                    m_Level.AddOffsetOnYAxis();
                }
                m_Level.AddGradientColors(m_GameManager.m_GradientTopColor, m_GameManager.m_GradientBottomColor);
                m_Level.Save();
            }
        }

        public void LoadLevels()
        {
            m_GameManager.LoadLevel(m_Levels[0]);
        }

        public void InitLevels() {
            string path = "Assets\\[Template] ConnectDots\\Resources\\Texts\\Levels.txt";
            using (StreamReader sr = new StreamReader(path))
            {
                string line = "";
                List<string> lines = new List<string>();
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }
                for (int i = 0; i < lines.Count; i++)
                {
                    int index = i;
                    Level tempLevel = new Level(0);
                    while (lines[i] == "Level")
                    {
                        
                        index++;
                        if (index == lines.Count)
                        {
                            if (tempLevel.dotsXY > 0) {
                                m_Levels.Add(tempLevel);
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
                            
                            if (tempStr.Contains("True")) {
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
                                m_Levels.Add(tempLevel);
                            }
                            i = index - 1;
                        }
                    }
                }
                sr.Close();
            }
        }
    }
}

[System.Serializable]
public class Level {
    public int dotsXY;
    public bool SetOffset;
    public List<int> connectorsXY;
    public List<int> disabledDotsXY;
    public List<int> removedDotsXY;
    public List<int> multiConnectorsXY;
    public List<Color>      colorsXY;
    public List<Vector2Int> portalsXY;
    public Color m_Bottom;
    public Color m_Top;
    public Level(int dotsXY) {
        this.dotsXY = dotsXY;
        this.connectorsXY       = new List<int>();
        this.disabledDotsXY     = new List<int>();
        this.removedDotsXY      = new List<int>();
        this.multiConnectorsXY  = new List<int>();
        this.colorsXY   = new List<Color>();
        this.portalsXY  = new List<Vector2Int>();
    }

    public void AddConnector(int index,Color color) {
        this.connectorsXY.Add(index);
        this.colorsXY.Add(color);
    }
    public void AddMultiConnector(int index) {
        this.multiConnectorsXY.Add(index);
    }
    public void AddPortal(Vector2Int portalXY) {
        portalsXY.Add(portalXY);
    }
    public void AddDisabledDot(int index) {
        this.disabledDotsXY.Add(index);
    }
    public void AddRemovedDot(int index) {
        this.removedDotsXY.Add(index);
    }
    public void AddGradientColors(Color top, Color bot)
    {
        m_Top = top;
        m_Bottom = bot;
    }
    public void AddOffsetOnYAxis() {
        SetOffset = true;
    }

    public void Save() {
        string path = "Assets\\[Template] ConnectDots\\Resources\\Texts\\Levels.txt";
        //dotsXY *= 5;
        using (StreamWriter sw = new StreamWriter(path))
        {
            sw.WriteLine("Level");
            sw.WriteLine(string.Format("Dots:[{0}]", (dotsXY).ToString()));
            sw.WriteLine(string.Format("YAxis:[{0}]", SetOffset.ToString()));
            sw.WriteLine(string.Format("GradientColors:[{0}|{1}]", m_Top.ToString().Replace("RGBA(", "").Replace(")", ""), m_Bottom.ToString().Replace("RGBA(", "").Replace(")", "")));
            sw.WriteLine(string.Format("Connectors:[{0}]", connectorsXY.ListIntToString()));
            sw.WriteLine(string.Format("Multi:[{0}]", multiConnectorsXY.ListIntToString()));
            sw.WriteLine(string.Format("Disabled:[{0}]", disabledDotsXY.ListIntToString()));
            sw.WriteLine(string.Format("Removed:[{0}]", removedDotsXY.ListIntToString()));
            sw.WriteLine(string.Format("Colors:[{0}]", colorsXY.ListColorToString()));
            sw.WriteLine(string.Format("Portals:[{0}]", portalsXY.ListVectorToString()));

            sw.Dispose();
        }
    }

}