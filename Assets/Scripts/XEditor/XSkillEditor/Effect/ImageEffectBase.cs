#if UNITY_EDITOR
using UnityEngine;

[RequireComponent (typeof(Camera))]
[AddComponentMenu("")]
public class ImageEffectBase : MonoBehaviour {
	/// Provides a shader property that is set in the inspector
	/// and a material instantiated from the shader
	public Shader   m_shader;
	private Material m_Material;
    protected string m_shaderName = "Mobile/Diffuse";
	protected virtual void Start ()
	{
		// Disable if we don't support image effects
		if (!SystemInfo.supportsImageEffects) {
			enabled = false;
			return;
		}
		
		// Disable the image effect if the shader can't
		// run on the users graphics card
		if (!shader || !shader.isSupported)
			enabled = false;
	}
    public virtual Shader shader
    {
        get
        {
            if (m_shader == null)
            {
                m_shader = Shader.Find(m_shaderName);
            }
            return m_shader;
        }
        set
        {
            m_shader = value;
        }
    }
	protected Material material {
		get {
			if (m_Material == null) {
				m_Material = new Material (shader);
				m_Material.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_Material;
		} 
	}
	
	protected virtual void OnDisable() {
		if( m_Material ) {
			DestroyImmediate( m_Material );
		}
	}
}
#endif