Shader "Unlit/GridLine" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            Stencil {
                Ref 2
                Comp NotEqual
                Pass Replace
            }
            CGPROGRAM
                #pragma vertex vert
			    #pragma fragment frag
			    #include "UnityCG.cginc"

			    struct v2f {
				    float4 vertex : SV_POSITION;
				    half2 texcoord : TEXCOORD0;
                    float4 color : Color;
			    };

			    sampler2D _MainTex;
			    float4 _MainTex_ST;
			
			    v2f vert (appdata_full v)
			    {
				    v2f o;
				    o.vertex = UnityObjectToClipPos(v.vertex);
				    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                    o.color = v.color;
				    return o;
			    }
			
			    fixed4 frag (v2f i) : SV_Target
			    {
				    fixed4 col = tex2D(_MainTex, i.texcoord);
				    col = col * i.color;
				    return col;
			    }
			ENDCG
		}
	}
	Fallback Off
}