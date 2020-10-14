// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "BlendModes/ColorBurn" {
	    Properties {
        _MainTex ("Texture", 2D) = "" {}
        _Color ("Blend Color", Color) = (1, 1, 1 ,1)
    }
 
    SubShader {
 
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
       
        Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Fog { Mode Off }
       
        Pass {  
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
 
            #include "UnityCG.cginc"
 
            struct appdata_t {
                fixed4 vertex : POSITION;
                fixed4 color : COLOR;
                fixed2 texcoord : TEXCOORD0;
            };
 
            struct v2f {
                fixed4 vertex : POSITION;
                fixed4 color : COLOR;
                fixed2 texcoord : TEXCOORD0;
            };
 
            sampler2D _MainTex;
 
            uniform fixed4 _MainTex_ST;
            uniform fixed4 _Color;
           
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color * _Color;
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }
 
            fixed4 frag (v2f i) : COLOR
            {
                // Get the raw texture value
                fixed4 texColor = tex2D(_MainTex, i.texcoord);
                // Calculate the luminance of the blend color
                float luminance =  dot(texColor, fixed4(0.2126, 0.7152, 0.0722, 0));
                // Declare the output structure
                fixed4 output = 0;

                // Quick maths		
                output = (1 - ((1-luminance) / i.color));

                // The alpha can actually just be a simple blend of the two-
                // makes things nicely controllable in both texture and color
                output.a  = texColor.a * i.color.a;
                return output;
            }
            ENDCG
        }
    }  
 
    Fallback off
}

