Shader "Custom/XRayBehindWalls"
{
    Properties
    {
        _XRayColor ("X-Ray Color", Color) = (0, 1, 1, 1)
        _Alpha ("Alpha", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "Queue" = "Overlay" "RenderType" = "Transparent" }

        Pass
        {
            // Configuración para solo mostrarse si el objeto está OCULTO
            ZTest Greater     // Comparación de profundidad: solo si hay algo delante
            ZWrite Off        // No escribe en el z-buffer
            Cull Back         // Opcional: dibuja solo caras visibles
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _XRayColor;
            float _Alpha;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return fixed4(_XRayColor.rgb, _Alpha);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
