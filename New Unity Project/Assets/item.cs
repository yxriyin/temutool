using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class item : MonoBehaviour
{
    // Start is called before the first frame update
    public Text No;
    public Text DZ;
    public RawImage image; 
    void Start()
    {
        
    }

    public void clear()
    {
        No.text = "";
        DZ.text = "";
        image.texture = null;
    }

    public void setData(string no, string sku, string path)
    {
        No.text = "No." + no;
        DZ.text = sku;
        Texture2D texture = new Texture2D(300, 300);
        if(path != "")
        {
            byte[] bytes = File.ReadAllBytes(path);
            texture.LoadImage(bytes);
            image.texture = texture;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
