using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CTemplate
{
    [System.Serializable]
    public class Dot
    {
        //Dot Object
        private GameObject dot;

        //Dot's Children Objects
        public  GameObject Sprite;
        public  GameObject Fade;
        public  GameObject Portal;
        public  GameObject Ring;
        public  GameObject Multi;
        public  GameObject[] Lines;

        //TempLine is the line to get linked dot Angle
        private Transform tempLine;

        //Sprites of the dot if its enabled,disabled or connected
        private Sprite m_CircleEnabled;
        private Sprite m_CircleRing;
        private Sprite m_CircleDisabled;

        //Dot sprite scale when its acive or inactive
        private Vector3 activeScale;
        private Vector3 inactiveScale;

        //Dot's current colors for each line pointing in different directions.
        public Color[] m_CurrentColors;

        //Dot's current lines pointing in different directions.
        public GameObject[] m_CurrentLinkedDots;

        
        public bool portal;
        public bool multiConnector;
        public bool linked;
        public bool active;

        public GameObject Line;

        public Dot endOfPortal;

        //Indexes to enable/disable specific Lines & Colors
        public int m_CurIndex;
        public int m_MultiIndex;

        //Initializing Dot
        public Dot(GameObject dot) {

            //Initializing Dot as normal dot changes can be made afterwards
            this.portal = false;
            this.linked = false;
            this.active = true;
            this.m_CurrentLinkedDots = new GameObject[1];
            this.m_CurrentColors = new Color[1];
            //Initializing Dot's Gameobjects
            this.dot = dot;
            GameObject[] tempGroup = dot.transform.GetChild(0).gameObject.GetChildrens();
            for (int i = 0; i < tempGroup.Length; i++) {
                if (tempGroup[i].name.Contains("Sprite")) 
                {
                    this.Sprite = tempGroup[i];
                    GameObject[] childs = tempGroup[i].GetChildrens();
                    for (int j = 0; j < childs.Length; j++) 
                    {
                        if (childs[j].name.Contains("Multi"))
                        {
                            this.Multi = childs[j];
                        }
                    }
                }
                if (tempGroup[i].name.Contains("Fade"))     { this.Fade = tempGroup[i];                 }
                if (tempGroup[i].name.Contains("Portal"))   { this.Portal = tempGroup[i];               }
                if (tempGroup[i].name.Contains("Ring"))     { this.Ring = tempGroup[i]; }
                if (tempGroup[i].name.Contains("Lines"))    { this.Line = tempGroup[i].gameObject; this.Lines = tempGroup[i].GetChildrens(); }
                if (tempGroup[i].name.Contains("tempLine")) { this.tempLine = tempGroup[i].transform;   }
            }

            //Disable all Lines
            foreach (GameObject i in Lines) {
                i.SetActive(false);
                float radius = dot.GetComponent<RectTransform>().sizeDelta.x;
                GameObject tempLineImage = i.GetChildrens()[0];
                tempLineImage.GetComponent<RectTransform>().localPosition = new Vector3(0, radius / 2, 0);
                tempLineImage.GetComponent<RectTransform>().sizeDelta = new Vector2(radius / 10, radius);
            }

            //Set Sprites
            this.m_CircleEnabled    = Resources.Load<Sprite>("Sprites/Circle-Enable");
            this.m_CircleDisabled   = Resources.Load<Sprite>("Sprites/Circle-Disable");
            this.m_CircleRing       = Resources.Load<Sprite>("Sprites/Circle-Ring");

            this.activeScale = this.Sprite.GetComponent<RectTransform>().localScale;
            this.inactiveScale = activeScale / 1.25f;
            
            //Disconnecting this dot
            DisconnectDot();
        }

        //Disconnect dot from linked dot
        public void DisconnectDot() {
            if (this.m_CurIndex > 0) { this.m_CurIndex--; }

            this.Ring.SetActive(false);
            this.Fade.SetActive(false);
            
            this.Multi.SetActive(this.multiConnector);
            this.linked = false;

            this.Lines[this.m_CurIndex].SetActive(false);
            this.Sprite.GetComponent<Image>().sprite = m_CircleRing;
            this.SetColor((this.m_CurIndex > 0) ? m_CurrentColors[this.m_CurIndex - 1] : Color.white);
        }

        //Set dot as current Connector
        public void ConnectDot(Color dotColor) {
            this.m_CurrentColors[this.m_CurIndex] = dotColor;
            this.Ring.SetActive(true);
            this.Sprite.GetComponent<Image>().sprite = (this.m_CurIndex + 1 < this.m_MultiIndex) ? this.m_CircleRing : this.m_CircleEnabled;
            this.SetColor(dotColor);
            if (m_CurIndex == m_MultiIndex)
            {
                this.linked = true;
            }
            
            this.m_CurIndex++;
        }

        //Link this dot with the next dot
        public void LinkDot(Color dotColor, Dot dot) {
            this.Ring.SetActive(false);
            this.m_CurrentLinkedDots[m_CurIndex-1] = dot.GetDotObject;

            this.Lines[this.m_CurIndex-1].SetActive(true);
            this.RotateLine(this.Lines[(this.m_CurIndex > 0) ? this.m_CurIndex - 1 : 0], this.m_CurrentLinkedDots[(this.m_CurIndex > 0) ? this.m_CurIndex - 1 : 0]);
            this.SetLineColor(m_CurrentColors[this.m_CurIndex - 1]);
            if (m_CurIndex == m_MultiIndex)
            {
                this.linked = true;
                this.Multi.SetActive(false);
            }
            
        }

        public void SetAsMulti() {
            this.m_MultiIndex = 2;
            this.m_CurrentLinkedDots = new GameObject[2];
            this.m_CurrentColors = new Color[2];
            this.multiConnector = true;
            this.DisconnectDot();
        }

        public void SetAsPortal(Dot endOfPortal) {
            this.Portal.SetActive(true);
            this.endOfPortal = endOfPortal;
            this.portal = true;
            this.DisconnectDot();
        }

        public void SetActive(bool activity) {
            this.Sprite.GetComponent<Image>().sprite = (activity) ?
                ((this.m_CurIndex + 1 < this.m_MultiIndex) ? this.m_CircleRing : this.m_CircleEnabled)
                :
                this.m_CircleDisabled;
            this.Sprite.GetComponent<RectTransform>().localScale = (activity) ? this.activeScale : this.inactiveScale;
            this.active = activity;
        }

        //Rotate Line to linked dot object
        private void RotateLine(GameObject line, GameObject linkdot)
        {
            if (linkdot != null && line != null && tempLine != null) 
            {
                Quaternion rot = Quaternion.LookRotation(linkdot.transform.position - tempLine.position, tempLine.TransformDirection(Vector3.left));
                line.transform.rotation = new Quaternion(0, 0, rot.x + rot.w, (rot.y + rot.z) * -1);
            }
        }

        //Sets the color of this dot
        private void SetColor(Color color)
        {
            this.Ring.GetChildrens()[0].GetComponent<Image>().color = color;
            this.Sprite.GetComponent<Image>().color = color;
            this.Multi.GetComponent<Image>().color = color;
        }

        //If in editmode and it selects dot
        public void SelectDot() {
            this.SetColor(Color.cyan);
        }

        //When clicking somewhere else, it should unselect the dot
        public void UnselectDot() {
            this.SetColor(Color.white);
        }

        private void SetLineColor(Color color) 
        {
            this.Lines[(this.m_CurIndex > 0) ? this.m_CurIndex - 1 : 0].GetChildrens()[0].GetComponent<Image>().color = color;
        }

        public GameObject GetDotObject {
            get {
                return this.dot;
            }
        }

        public Vector2 localPosition {
            get {
                return this.dot.transform.localPosition;
            }
        }

        public bool isPortal {
            get {
                return portal;
            }
        }

        public bool isActive {
            get {
                return this.active;
            }
        }

        public bool isLinked {
            get {
                return this.linked;
            }
        }

        public bool isMultiLinked {
            get
            {
                if (this.m_MultiIndex == this.m_CurIndex)
                {
                    return true;

                }
                else {
                    return false;
                }
            }
        }

        public bool isMultiConnector {
            get {
                return multiConnector;
            }
        }

        public bool isClicked(GameManager instance, Vector2 tempMouseCanvasPosition) 
        {
            float Distance = Vector2.Distance(this.localPosition, tempMouseCanvasPosition);
            float Radius = (instance.m_CurRadiusXY / 2);
            if (Distance < Radius)
            {
                this.SelectDot();
                return true;
            }
            else{
                return false;
            }
        }
    }
}