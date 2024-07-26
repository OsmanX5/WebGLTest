using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CTemplate
{
    public class GameManager : MonoBehaviour
    {
        public int      m_DotsXY;
        public float    m_OffsetXY;
        public float    m_CurRadiusXY, m_MaxRadiusXY;

        public Canvas       m_Canvas;
        public GameObject   m_GridOverlay;
        public Transform    m_GridParent;
        public Transform    m_LinesParent;
        public GameObject   m_DotPrefab;
        public Text         m_LevelText;

        public Gradient m_Gradient;

        public bool m_IsEditMode;

        public bool m_OffsetYAxis;

        public Vector2 m_MouseCanvasPosition {
            get {
                if (m_Canvas != null)
                {
                    return Camera.main.MousePositionToCanvasPosition(m_Canvas);
                }
                else {
                    return Vector2.zero;
                }
            }
        }

        public List<Dot> m_Dots             { get; set; }
        public List<Connector> m_Connectors { get; set; }

        public Color m_ConnectorColor;
        public Color m_GradientTopColor;
        public Color m_GradientBottomColor;

        public LevelCreator m_LevelCreator;
        private List<Level> m_Levels;
        public int m_CurLevel;
        public int m_Index;

        public void Start()
        {
            if (m_IsEditMode)
            {
                try { m_LevelCreator = gameObject.GetComponent<LevelCreator>(); }
                catch { }
            }
            m_ConnectorColor = Color.red;
            SetRadius();

            
            if (!m_IsEditMode)
            {
                m_Levels = this.GetComponent<LevelManager>().GetAllLevels();
                LoadLevel(m_Levels[m_CurLevel]);
                EventTrigger tempTrigger = m_GridOverlay.GetComponent<EventTrigger>();
                EventTrigger.Entry tempEntry2 = new EventTrigger.Entry();
                tempEntry2.eventID = EventTriggerType.PointerDown;
                tempEntry2.callback.AddListener((e) => { OnMouseDown(); });
                tempTrigger.triggers.Add(tempEntry2);
                EventTrigger.Entry tempEntry = new EventTrigger.Entry();
                tempEntry.eventID = EventTriggerType.Drag;
                tempEntry.callback.AddListener((e) => { try { m_Connectors[m_Index - 1].OnDrag(m_MouseCanvasPosition); } catch { } });
                tempTrigger.triggers.Add(tempEntry);
            }
            else if(Application.isEditor) {
                m_LevelCreator.m_Level.dotsXY = m_DotsXY;
                m_Dots = new List<Dot>();
                m_Connectors = new List<Connector>();
                m_GridParent.RemoveAllChildren();
                if (m_GridParent.childCount == 0)
                {
                    float offsetY = 0;
                    for (int x = 0; x < m_DotsXY; x++)
                    {
                        for (int y = 0; y < m_DotsXY; y++)
                        {
                            float offset = m_CurRadiusXY / 2;
                            float tempY = (offset + (y * m_CurRadiusXY)) - ((m_DotsXY * m_CurRadiusXY) / 2);
                            float tempX = (offset + (x * m_CurRadiusXY)) - ((m_DotsXY * m_CurRadiusXY) / 2);
                            tempY += offsetY;
                            GameObject dot = Instantiate(m_DotPrefab, m_GridParent);
                            dot.GetComponent<RectTransform>().localPosition = new Vector3(tempX, tempY, 0.0f);
                            dot.GetComponent<RectTransform>().sizeDelta = new Vector3(m_CurRadiusXY, m_CurRadiusXY);
                            dot.AddComponent<EventTrigger>();
                            m_Dots.Add(new Dot(dot));
                        }
                        if (m_OffsetYAxis)
                        {
                            offsetY = (offsetY == 0) ? m_CurRadiusXY / 2 : 0;
                        }
                    }
                }
                EventTrigger tempTrigger = m_GridOverlay.GetComponent<EventTrigger>();
                EventTrigger.Entry tempEntry = new EventTrigger.Entry();
                tempEntry.eventID = EventTriggerType.PointerDown;
                tempEntry.callback.AddListener((e) => { OnSelectDot(); });
                tempTrigger.triggers.Add(tempEntry);
            }
            
        }

        //Loads any level that's in the argument
        public void LoadLevel(Level level)
        {
            m_Dots = new List<Dot>();
            m_Connectors = new List<Connector>();
            m_GridParent.RemoveAllChildren();
            m_LinesParent.RemoveAllChildren();
            //Debug.Log(level.SetOffset.ToString());
            m_OffsetYAxis = level.SetOffset;
            if (m_GridParent.childCount == 0)
            {
                float offsetY = 0;
                for (int x = 0; x < level.dotsXY; x++)
                {
                    for (int y = 0; y < level.dotsXY; y++)
                    {
                        float offset = m_CurRadiusXY / 2;
                        float tempY = (offset + (y * m_CurRadiusXY)) - ((level.dotsXY * m_CurRadiusXY) / 2);
                        float tempX = (offset + (x * m_CurRadiusXY)) - ((level.dotsXY * m_CurRadiusXY) / 2);
                        tempY += offsetY;
                        GameObject dot = Instantiate(m_DotPrefab, m_GridParent);
                        dot.GetComponent<RectTransform>().localPosition = new Vector3(tempX, tempY, 0.0f);
                        dot.GetComponent<RectTransform>().sizeDelta = new Vector3(m_CurRadiusXY, m_CurRadiusXY);
                        dot.AddComponent<EventTrigger>();
                        Dot d = new Dot(dot);
                        d.Line.transform.SetParent(m_LinesParent);
                        m_Dots.Add(d);
                    }
                    if (m_OffsetYAxis)
                    {
                        offsetY = (offsetY == 0) ? m_CurRadiusXY / 2 : 0;
                    }
                }
            }

            for (int i = 0; i < level.connectorsXY.Count; i++)
            {
                int index = level.connectorsXY[i];
                Color color = level.colorsXY[i];
                m_Connectors.Add(new Connector(m_Dots[index], color, this));
            }
            for (int i = 0; i < level.multiConnectorsXY.Count; i++)
            {
                int index = level.multiConnectorsXY[i];
                m_Dots[index].SetAsMulti();
            }
            for (int i = 0; i < level.portalsXY.Count; i++)
            {
                int indexA = level.portalsXY[i].x;
                int indexB = level.portalsXY[i].y;
                Dot a = m_Dots[indexA];
                Dot b = m_Dots[indexB];
                a.SetAsPortal(b);
                b.SetAsPortal(a);
            }
            for (int i = 0; i < level.disabledDotsXY.Count; i++)
            {
                int index = level.disabledDotsXY[i];
                m_Dots[index].SetActive(false);
            }
            for (int i = 0; i < level.removedDotsXY.Count; i++)
            {
                int index = level.removedDotsXY[i];
                m_Dots[index].SetActive(false);
                m_Dots[index].GetDotObject.SetActive(false);
            }

            m_GradientTopColor = level.m_Top;
            m_GradientBottomColor = level.m_Bottom;

            if (m_Gradient != null) 
            {
                m_Gradient.gameObject.SetActive(false);
                m_Gradient.m_TopColor =    new Color32((byte)(m_GradientTopColor.r * 256),
                                                       (byte)(m_GradientTopColor.g * 256),
                                                       (byte)(m_GradientTopColor.b * 256),
                                                       (byte)(255));
                m_Gradient.m_BottomColor = new Color32((byte)(m_GradientBottomColor.r * 256),
                                                       (byte)(m_GradientBottomColor.g * 256),
                                                       (byte)(m_GradientBottomColor.b * 256),
                                                       (byte)(255));
                m_Gradient.gameObject.SetActive(true);
            }
        }

        //this Loads the next level
        public void NextLevel() {
            bool allLinked = true;
            for (int i = 0; i < m_Dots.Count; i++)
            {
                if (!m_Dots[i].linked && m_Dots[i].isActive)
                {
                    allLinked = false;
                }
            }
            if (allLinked)
            {
                if (this.m_Levels.Count != m_CurLevel + 1)
                {
                    m_CurLevel++;
                    LoadLevel(m_Levels[m_CurLevel]);
                }
            }
        }

        //Disabling the selected dpt
        public void DisableDot(Dot dot) {
            dot.SetActive(false);
            m_LevelCreator.AddDisabled(GetDotIndex(dot));
        }

        //Removing the selected dot
        public void RemoveDot(Dot dot)
        {
            int index = GetDotIndex(dot);
            dot.SetActive(false);
            dot.GetDotObject.SetActive(false);
            m_LevelCreator.AddRemoved(index);
        }

        //Setting the selected dot as a connector
        public void AddConnector(Dot dot) 
        {
            if (m_Connectors.Count > 0) {
                for (int i = 0; i < m_Connectors.Count; i++)
                {
                    if (dot == m_Connectors[i].m_ConnectorDot) {
                        
                        m_Connectors[i] = new Connector(dot, m_ConnectorColor, this);
                        return;
                    }
                }
                
            }
            m_Connectors.Add(new Connector(dot, m_ConnectorColor, this));
            m_LevelCreator.ConnectorAdded(GetDotIndex(dot), m_ConnectorColor);
        }

        //Setting the two selected dot as portals
        public void AddPortal(Dot a, Dot b) {
            a.SetAsPortal(b);
            b.SetAsPortal(a);
            m_LevelCreator.PortalAdded(GetDotIndex(a), GetDotIndex(b));
        }

        //Setting the selected dot as a multi connector
        public void AddMultiConnector(Dot dot)
        {
            dot.SetAsMulti();
            m_LevelCreator.MultiConnectorAdded(GetDotIndex(dot));
        }

        void Update() 
        {
            if (!m_IsEditMode)
            {
                m_LevelText.text = string.Format("Level : {0}", m_CurLevel + 1);
            }
            else {
                if (m_Gradient != null)
                {
                    m_Gradient.gameObject.SetActive(false);
                    m_Gradient.m_TopColor = new Color32((byte)(m_GradientTopColor.r * 256),
                                                        (byte)(m_GradientTopColor.g * 256),
                                                        (byte)(m_GradientTopColor.b * 256),
                                                        (byte)(255));
                    m_Gradient.m_BottomColor = new Color32((byte)(m_GradientBottomColor.r * 256),
                                                           (byte)(m_GradientBottomColor.g * 256),
                                                           (byte)(m_GradientBottomColor.b * 256),
                                                           (byte)(255));
                    m_Gradient.gameObject.SetActive(true);
                }
            }
        }

        public void OnMouseDown() {
            for (int i = 0; i < m_Connectors.Count; i++) {
                if (m_Connectors[i].IsClicked(m_MouseCanvasPosition)) {
                    m_Index = i + 1;
                    return ;
                }
            }
            m_Index = 0;
        }

        public void OnSelectDot() {
            for (int i = 0; i < m_Dots.Count; i++) {
                if (m_Dots[i].isClicked(this, m_MouseCanvasPosition)) {
                    try
                    {
                        if (m_LevelCreator.SelectedDot.isActive)
                        {
                            m_LevelCreator.SelectedDot.UnselectDot();
                        }
                    }
                    catch { }
                    m_Dots[i].SelectDot();
                    m_LevelCreator.AddDot(m_Dots[i]);
                    return;
                }
            }
        }

        public void SetRadius()
        {
            //Set Dot Radius by dividing Canvas width with Dots X.
            float width = m_Canvas.GetComponent<RectTransform>().rect.width;
            float tempRadius = width / m_DotsXY;
            if (tempRadius < m_MaxRadiusXY)
            {
                m_CurRadiusXY = tempRadius - ((tempRadius / 100) * 5);
            }
            else {
                m_CurRadiusXY = m_MaxRadiusXY;
            }
        }

        public Dot GetNearestDot(Vector2 mousePos) {
            float distance = m_CurRadiusXY * 3;
            Dot result = null;
            for(int i = 0; i < m_Dots.Count; i++){
                float tempDist = Vector2.Distance(mousePos, m_Dots[i].localPosition);
                if (tempDist < distance) {
                    result = m_Dots[i];
                    distance = tempDist;
                }
            }

            return result;
        }

        public Dot GetNearestDot(Vector2 curPos, Vector2 mousePos, float dist) {
            float distance = dist + m_CurRadiusXY / 2;
            float radius = m_CurRadiusXY + m_CurRadiusXY/m_DotsXY;
            if (Vector2.Distance(mousePos, curPos) > (m_CurRadiusXY * 3))
            {
                mousePos = new Vector2(
                    curPos.x + (m_CurRadiusXY *
                    (int)Camera.main.MousePositionToViewPortPosition().x),
                    curPos.y + (m_CurRadiusXY *
                    (int)Camera.main.MousePositionToViewPortPosition().y)
                    );
            }
            Dot result = null;
            for (int i = 0; i < m_Dots.Count; i++) {
                if (curPos != m_Dots[i].localPosition)
                {
                    Vector2 c = curPos;
                    Vector2 m = mousePos;
                    float r = radius;
                    Vector2 d = m_Dots[i].localPosition;
                    float tempDist = (Vector2.Distance(c, m) > r) ? Vector2.Distance(c, d) : (m_OffsetYAxis) ? Vector2.Distance(m, d) : Vector2.Distance(c, d);
                                     
                    if (tempDist < distance)
                    {
                        result = m_Dots[i];
                        distance = tempDist;
                    }
                    else if (tempDist == distance)
                    {
                        if (!m_OffsetYAxis && result != null)
                        {
                            Vector2 a = result.localPosition;
                            Vector2 b = d;
                            if (a != b) {
                                float distA = Vector2.Distance(a, m);
                                float distB = Vector2.Distance(b, m);
                                if (distA > distB) {
                                    result = m_Dots[i];
                                    distance = tempDist;
                                }
                            }
                        }
                    }
                }
                
            }
            return result;
        }

        //Getting the selected dot's index
        public int GetDotIndex(Dot dot) {
            for (int i = 0; i < m_Dots.Count; i++)
            {
                if (m_Dots[i] == dot) {
                    return i;
                }
            }
            return -1;
        }
    }
}