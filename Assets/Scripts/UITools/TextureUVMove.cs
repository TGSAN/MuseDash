using UnityEngine; using System.Collections;        
public class TextureUVMove : MonoBehaviour 
{    
   	public float _XSpeed=1;    
	public float _YSpeed=1;
	private UITexture m_texture;  
	//public UI2DSprite m_sprite;
	//public  Material _ScrollMaterial;



	 void Start()     
	 {         
		m_texture=GetComponent<UITexture>();

  	}          


	 void Update()    
	 {
		Rect temp=m_texture.uvRect;

		temp.x+=_XSpeed*0.01f;
		temp.y-=_YSpeed*0.01f;

		m_texture.uvRect=temp;

//		this
//			._ScrollMaterial.mainTextureOffset
//			= 
//				new
//
//				Vector2(_Speed * Time.time, 0);

	 } 
}

