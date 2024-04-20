using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Inventaire : MonoBehaviour
{
    public Camera Camera;
    // Start is called before the first frame update
    bool YouSee;
    [SerializeField] List<GameObject> Inv = new List<GameObject>();
    GameObject Selected;
    public GameObject PanelInv;
    Sprite Reset;
    public List<GameObject> Inv1 { get => Inv; set => Inv = value; }

    void Start()
    {
        Reset = PanelInv.GetComponent<RectTransform>().GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite;
        PanelInv.SetActive(false);
        YouSee = false;
        Selected = null;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) 
        {
            if(YouSee == false)
            {
                PanelInv.SetActive(true);
                YouSee = true;
            }
            else
            {
                PanelInv.SetActive(false);
                YouSee = false;
            }
        }

        if (YouSee == true)
        {
            ShowInv();
        }

        if(Inv.Count < 3) 
        {
            if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider != null)
                {
                    if (hit.collider.tag == "Takable")
                    {
                        AddintoInv(hit.collider.gameObject);
                        hit.collider.gameObject.SetActive(false);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftControl))
        {
            if (YouSee == true) 
            {
                
                if (Selected != null)
                {
                    for(int i = 0; i < Inv.Count; i++)
                    {
                        if (Inv[i] == Selected)
                        {
                            Vector3 WorldPos = Camera.ScreenToWorldPoint(Input.mousePosition);
                            WorldPos.z = 0;

                            Inv1[i].GetComponent<Transform>().position = WorldPos;
                            Inv1[i].SetActive(true);
                            print(i);
                            PanelInv.GetComponent<RectTransform>().GetChild(Inv.Count-1 ).GetComponent<UnityEngine.UI.Image>().sprite = Reset;
                            PanelInv.GetComponent<RectTransform>().GetChild(Inv.Count-1 + 4).GetComponent<TextMeshProUGUI>().text = "";
                            Inv1.Remove(Inv1[i]);
                            Selected = null;
                            ShowInv();
                            
                        }
                    }
                    //DeleteFromPanel();
                }
            }
            
        }
    }

    public void AddintoInv(GameObject IntoInv)
    {
        Inv.Add(IntoInv); 
    }
    public void ShowInv()
    {
        for (int i = 0; i < Inv.Count; i++)
        {
            if(i > 3)
            {
                Debug.LogError("InvLimitPass");
            }
            PanelInv.GetComponent<RectTransform>().GetChild(i).GetComponent<UnityEngine.UI.Image>().sprite = Inv[i].GetComponent<SpriteRenderer>().sprite;
            PanelInv.GetComponent<RectTransform>().GetChild(i+4).GetComponent<TextMeshProUGUI>().text = Inv[i].name;
        }
        if(Inv.Count == 0) 
        {
            for (int i = 0; i < 3; i++)
            {
                PanelInv.GetComponent<RectTransform>().GetChild(i).GetComponent<UnityEngine.UI.Image>().sprite = Reset;
                PanelInv.GetComponent<RectTransform>().GetChild(i + 4).GetComponent<TextMeshProUGUI>().text = "";
            }
        }

    }
    public void Isclicked(GameObject ObjSprite) 
    {
        for (int i = 0;i < 4 ;i++) 
        {
            if(PanelInv.GetComponent<Transform>().GetChild(i) == ObjSprite.transform)
            {
                TextMeshProUGUI NameObj = PanelInv.GetComponent<Transform>().GetChild(i + 4).GetComponent<TextMeshProUGUI>();
                for (int j = 0; j < Inv.Count; j++) 
                {
                    if (NameObj.text == Inv[j].name && Inv[j].GetComponent<SpriteRenderer>().sprite == ObjSprite.GetComponent<UnityEngine.UI.Image>().sprite) 
                    {
                        Selected = Inv[j];
                    }
                    
                }
                if(Selected == null)
                {
                    Debug.LogError("Nobody");
                }  
            }
            else if (PanelInv.GetComponent<Transform>().GetChild(i) != ObjSprite.transform && i == 3 && Selected == null)
            {
                Debug.LogError("NoMatch");
            }
        }
        
    }
}
