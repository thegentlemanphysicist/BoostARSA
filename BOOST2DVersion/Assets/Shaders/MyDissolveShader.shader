Shader "Custom/MyDissolveShader"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white"{}
		_DissolveMap("Dissolve Shape", 2D) = "white"{}
		
		_DissolveVal("Dissolve Value", Range(-0.2, 1.2)) = 1.2
		_LineWidth("Line Width", Range(0.0, 0.2)) = 0.1
		
		_LineColor("Line Color", Color) = (1.0, 1.0, 1.0, 1.0)
	
	//here I'm trying to add a second colour to the line
		_LineWidthFrac("Line Width Fraction", Range(0.0, 1.0)) = 0.5
		
		_LineColor2("Line Color 2", Color) = (1.0, 1.0, 1.0, 1.0)
		
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		//#pragma surface surf Standard fullforwardshadows
		#pragma surface surf Lambert keepalpha
		sampler2D _MainTex;
		sampler2D _DissolveMap;
		
		float4 _LineColor;
		float _DissolveVal;
		float _LineWidth;
		
		//second line
		float4 _LineColor2;
		float _LineWidthFrac;
		
		
		struct Input 
		{
     			half2 uv_MainTex;
     			half2 uv_DissolveMap;
    	};

		void surf (Input IN, inout SurfaceOutput o ) //Standard o) 
		{
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;

			half4 dissolve = tex2D(_DissolveMap, IN.uv_DissolveMap);

			half4 clear = half4(0.0,0.0,0.0,0.0);
			
			//I need an int that will choose between the two line colours.
			//0 for _LineColor, 1 for _LineColor2
			//the int function gives the lerp function a 0 or 1 variable
			int isColor1 = int(dissolve.r - (_DissolveVal + _LineWidth*_LineWidthFrac) + 0.99);
			half4 lineColorChoice= lerp(_LineColor, _LineColor2, isColor1) ;
			
			
			//isClear resolve to 0 if dissolve.r isn’t greater than _DissolvVal + _LineWidth
			int isClear = int(dissolve.r - (_DissolveVal + _LineWidth) + 0.99);
			
			//isAtLeastLine will be 0 if we should use the regular texture instead 
				//of using the line color or transparency
			int isAtLeastLine = int(dissolve.r - (_DissolveVal) + 0.99);
			
				
			half4 altCol = lerp(lineColorChoice, clear, isClear);

			o.Albedo = lerp(o.Albedo, altCol, isAtLeastLine);
			
			o.Alpha = lerp(1.0, 0.0, isClear);
			
		}
		ENDCG
	}
}