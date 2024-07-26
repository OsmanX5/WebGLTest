using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CTemplate
{
    [System.Serializable]
    public class Connector
    {
        public List<Dot> m_Dots = new List<Dot>();

        public Dot m_ConnectorDot;

        public Color m_DotsColor;

        public int m_Index;

        public Vector2 m_MousePosition;
        public Vector2 m_DotPosition;
        public Vector2 MouseCanvasPosition;

        public float Distance;

        public GameManager m_GameManagerInstance;

        public Connector(Dot dot, Color color, GameManager instance) {
            this.m_ConnectorDot = dot;
            this.m_DotsColor = color;
            this.m_GameManagerInstance = instance;
            this.m_ConnectorDot.ConnectDot(this.m_DotsColor);
        }

        public void Update() {
            m_MousePosition = Input.mousePosition;
        }

        public bool IsClicked(Vector2 tempMouseCanvasPosition) {
            m_DotPosition       = m_ConnectorDot.GetDotObject.transform.localPosition;
            MouseCanvasPosition = tempMouseCanvasPosition;
            Distance            = Vector2.Distance(m_DotPosition, tempMouseCanvasPosition);
            float Radius = (m_GameManagerInstance.m_CurRadiusXY / 2);
            if (Distance < Radius)
            {
                return true;
            }
            else {
                //check if Mouse is clicking a dot that's linked using this connector
                for (int i = 0; i < m_Dots.Count; i++)
                {
                    float dist = Vector2.Distance(tempMouseCanvasPosition, this.m_Dots[i].localPosition);
                    if (dist < Radius) {
                        if (this.m_Dots[i].isMultiConnector)
                        {
                            //Dot is multi Connector and probably has two connectors, So we need to get the last linked object 
                            if (i + 1 == this.m_Dots.Count) //This dot has just been linked with the current connector dot
                            {
                                int length = m_Dots[i].m_CurrentLinkedDots.Length;
                                if (this.m_ConnectorDot.GetDotObject == m_Dots[i].m_CurrentLinkedDots[length - 1])
                                {
                                    this.ResetPosition(i - 1);
                                    return false;
                                }
                            }
                            else for (int j = i; j < this.m_Dots.Count; j++)
                                {
                                    int length = this.m_Dots[i].m_CurIndex;
                                    if (this.m_Dots[i] != this.m_Dots[j])
                                    {
                                        if (this.m_Dots[i].m_CurrentLinkedDots[length - 1] == this.m_Dots[j].GetDotObject)
                                        {
                                            this.ResetPosition(i - 1);
                                            return false;
                                        }
                                        
                                        
                                    }
                                }
                        }
                        else
                        {
                            ResetPosition(i);
                        }
                    }
                }
                return false;
            }
        }

        public void OnDrag(Vector2 tempMouseCanvasPosition) {
            m_DotPosition = m_ConnectorDot.GetDotObject.transform.localPosition;
            MouseCanvasPosition = tempMouseCanvasPosition;
            Distance = Vector2.Distance(m_DotPosition, tempMouseCanvasPosition);
            float Radius = m_GameManagerInstance.m_CurRadiusXY;
            if (Distance >= (Radius - (Radius * 0.25f)) && Distance <= Radius + (Radius * 0.5f))
            {
                Dot nearestDot = m_GameManagerInstance.GetNearestDot(m_DotPosition, MouseCanvasPosition, Distance);
                if (nearestDot != null) {
                    if (!isConnector(nearestDot))
                    {
                        ConnectDot(nearestDot);
                    }
                }
            }
        }

        public bool isConnector(Dot dot) 
        {
            List<Connector> connectors = m_GameManagerInstance.m_Connectors;
            for (int i = 0; i < connectors.Count; i++)
            {
                if (connectors[i] != this) {
                    if (connectors[i].m_ConnectorDot == dot)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void ConnectDot(Dot dot) {
            if (!dot.isLinked && dot.isActive) {
                if (m_Dots.Count > 0)
                {
                    if (dot == m_Dots[m_Dots.Count - 1] && dot.isMultiConnector)
                    {
                        //Prevent Connector to Connect to Multi link if dragging backwards, instead it's gonna Reset it's position
                        ResetPosition(m_Dots.Count - 1);
                        return; 
                    }
                }
                try
                {
                    dot.Fade.SetActive(false);
                    Dot a = m_ConnectorDot;
                    Dot b = dot;
                    
                    a.LinkDot(m_DotsColor, b);

                    int index = m_Index;
                    EventTrigger tempTrigger = dot.GetDotObject.GetComponent<EventTrigger>();
                    try { tempTrigger.triggers.RemoveAt(0); }
                    catch { }

                    EventTrigger.Entry tempEntry = new EventTrigger.Entry();
                    tempEntry.eventID = EventTriggerType.PointerDown;
                    tempEntry.callback.AddListener((e) => { ResetPosition(index); });
                    tempTrigger.triggers.Add(tempEntry);
                    m_Dots.Add(a);
                    if (b.isPortal)
                    {
                        Dot c = b.endOfPortal;
                        b.ConnectDot(m_DotsColor);
                        b.Ring.SetActive(false);
                        a.Ring.SetActive(false);
                        m_ConnectorDot = c;
                        m_ConnectorDot.ConnectDot(m_DotsColor);
                        m_Dots.Add(b);
                
                    }
                    else
                    {
                        m_ConnectorDot = b;
                        m_ConnectorDot.ConnectDot(m_DotsColor);
                    }
                    m_ConnectorDot.Fade.SetActive(true);
                }
                catch (Exception e) {
                    Debug.Log(e);
                }
                m_Index++;
            }
            else if (dot.isLinked) {
                //if the connector is being dragged to the last dot it linked it should reset position
                if (dot == m_Dots[m_Dots.Count - 1]) 
                {
                    ResetPosition(m_Dots.Count - 1);
                }
            }
        }

        public void ResetPosition(int index) {
            Dot a = m_Dots[index];

            while (a.isPortal) {
                index--;
                a = m_Dots[index];
            }

            Dot b = m_ConnectorDot;
            b.DisconnectDot();
            Dot prevDot = null;
            while (m_Dots.Count > index) {
                int i = m_Dots.Count - 1;
                Dot c = m_Dots[i];
                bool wasPrevConnector = false;
                if (c.isMultiConnector)
                {
                    if (prevDot != null)
                    {
                        if (c.m_CurIndex > 1)
                        {
                            if (c.m_CurrentLinkedDots[0] == prevDot.GetDotObject)
                            {
                                c.m_CurrentColors[0] = c.m_CurrentColors[1];
                                c.m_CurrentLinkedDots[0] = c.m_CurrentLinkedDots[1];
                                c.m_CurrentLinkedDots[1] = null;
                                wasPrevConnector = true;
                            }
                        }
                    }
                    else
                    {
                        if (c.m_CurIndex > 1)
                        {
                            if (c.m_CurrentLinkedDots[0] == m_ConnectorDot.GetDotObject)
                            {
                                c.m_CurrentColors[0] = c.m_CurrentColors[1];
                                c.m_CurrentLinkedDots[0] = c.m_CurrentLinkedDots[1];
                                c.m_CurrentLinkedDots[1] = null;
                                wasPrevConnector = true;
                            }
                        }
                    }
                }
                else {
                    prevDot = m_Dots[i];
                }
                
                m_Dots[i].DisconnectDot();

                if (wasPrevConnector)
                {
                    c.Lines[0].SetActive(false);
                    c.Lines[1].SetActive(true);
                    wasPrevConnector = false;
                }
                else if (c.multiConnector && c.m_CurIndex == 0)
                {
                    {
                        c.Lines[1].SetActive(false);
                    }
                }
                m_Dots.RemoveAt(i);
            }
            m_ConnectorDot =  a;
            m_ConnectorDot.ConnectDot(m_DotsColor);
        }
    }
}