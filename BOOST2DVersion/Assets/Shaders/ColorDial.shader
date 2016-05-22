Shader "Custom/ColorDial" {
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "" {} //Visible Spectrum Texture ( RGB )
		_UVTex("UV",2D) = "" {} //UV texture
		_IRTex("IR",2D) = "" {} //IR texture
		//will need to declare some variables for softened gamma
		_softGamma("Softened Gamma", Range(1,100)) = 1.1
		_directionOfJenny("Direction", float ) = 1.0 
		//need a variable for the rgb value of the base colour
		_baseColor("Pulse Colour", Color) = (0.0,0.0,1.0,1.0)
	}
	 
	 SubShader {
        
       	Tags{ "Queue" = "Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
        
	
        
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

				// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members pos2,uv1,svc,vr,draw)
				#pragma exclude_renderers d3d11 xbox360
				// Upgrade NOTE: excluded shader from Xbox360; has structs without semantics (struct v2f members pos2,uv1,svc,vr)	
				// Not sure when this^ got added, seems like unity did it automatically some update?
				#pragma exclude_renderers xbox360
				#pragma glsl

				
				//Color shift variables, used to make guassians for XYZ curves
				#define xla 0.39952807612909519
				#define xlb 444.63156780935032
				#define xlc 20.095464678736523

				#define xha 1.1305579611401821
				#define xhb 593.23109262398259
				#define xhc 34.446036241271742

				#define ya 1.0098874822455657
				#define yb 556.03724875218927
				#define yc 46.184868454550838

				#define za 2.0648400466720593
				#define zb 448.45126344558236
				#define zc 22.357297606503543

				//Used to determine where to center UV/IR curves
				#define IR_RANGE 400
				#define IR_START 700
				#define UV_RANGE 380
				#define UV_START 0


			
			
				sampler2D _MainTex;
				sampler2D _IRTex;
				sampler2D _UVTex;
				float _softGamma;
				float _directionOfJenny;
				float4 _baseColor;

            struct vertexInput {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
            	float2 uv1 : TEXCOORD1;//Used to specify what part of the texture to grab in the fragment shader(not relativity specific, general shader variable)
   
            };

            struct fragmentInput{
                float4 position : SV_POSITION;
                float4 texcoord0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;//Used to specify what part of the texture to grab in the fragment shader(not relativity specific, general shader variable)
            };

            fragmentInput vert(vertexInput i){
                fragmentInput o;
                o.uv1.xy = i.texcoord0; //get the UV coordinate for the current vertex, will be passed to fragment shade
                o.position = mul (UNITY_MATRIX_MVP, i.vertex);
                o.texcoord0 = i.texcoord0;
                return o;
            }

            
            
            //I need the doppler shift formula from Jenny shoot
		        
		    float dopplerShift(float aa, float cosTheta)
		    {     
		        //aa is now the softened gamma factor.  Still has the range [1,infinity]
		        //aa = 1+dopplerSoftening*(gammaX-1);
		        //sign of aa is the direction jenny's facing 
		     
				float b = aa*_directionOfJenny * sqrt(1-pow(aa,-2)) ;
				
				return  (aa - b*cosTheta  );
            }
            
            //insert the colour manipulating functions here from the relativity shader
            
			//Color functions, there's no check for division by 0 which may cause issues on
			//some graphics cards.
			float3 RGBToXYZC(  float r,  float g,  float b)
			{
				float3 xyz;
				xyz.x = 0.13514*r + 0.120432*g + 0.057128*b;
				xyz.y = 0.0668999*r + 0.232706*g + 0.0293946*b;
				xyz.z = 0.0*r + 0.0000218959*g + 0.358278*b;
				return xyz;
			}
			float3 XYZToRGBC(  float x,  float y,  float z)
			{
				float3 rgb;
				rgb.x = 9.94845*x -5.1485*y - 1.16389*z;
				rgb.y = -2.86007*x + 5.77745*y - 0.0179627*z;
				rgb.z = 0.000174791*x - 0.000353084*y + 2.79113*z;

				return rgb;
			}
			
			
			float3 weightFromXYZCurves(float3 xyz)
			{
				float3 returnVal;
				returnVal.x = 0.0735806 * xyz.x -0.0380793 * xyz.y - 0.00860837 * xyz.z;
				returnVal.y = -0.0665378 * xyz.x +  0.134408 * xyz.y - 0.000417865 * xyz.z;
				returnVal.z = 0.00000299624 * xyz.x - 0.00000605249 * xyz.y + 0.0484424 * xyz.z;
				return returnVal;
			}
			
			
			float getXFromCurve(float3 param,  float shift)
			{
				 float top1 = param.x * xla * exp( (float)(-(pow((param.y*shift) - xlb,2)
					/(2*(pow(param.z*shift,2)+pow(xlc,2))))))*sqrt( (float)(float(2)*(float)3.14159265358979323));
				 float bottom1 = sqrt((float)(1/pow(param.z*shift,2))+(1/pow(xlc,2))); 

				 float top2 = param.x * xha * exp( float(-(pow((param.y*shift) - xhb,2)
					/(2*(pow(param.z*shift,2)+pow(xhc,2))))))*sqrt( (float)(float(2)*(float)3.14159265358979323));
				 float bottom2 = sqrt((float)(1/pow(param.z*shift,2))+(1/pow(xhc,2)));

				return (top1/bottom1) + (top2/bottom2);
			}
			
			
			float getYFromCurve(float3 param,  float shift)
			{
				 float top = param.x * ya * exp( float(-(pow((param.y*shift) - yb,2)
					/(2*(pow(param.z*shift,2)+pow(yc,2))))))*sqrt( float(float(2)*(float)3.14159265358979323));
				 float bottom = sqrt((float)(1/pow(param.z*shift,2))+(1/pow(yc,2))); 

				return top/bottom;
			}

			 float getZFromCurve(float3 param,  float shift)
			{
				 float top = param.x * za * exp( float(-(pow((param.y*shift) - zb,2)
					/(2*(pow(param.z*shift,2)+pow(zc,2))))))*sqrt( float(float(2)*(float)3.14159265358979323));
				 float bottom = sqrt((float)(1/pow(param.z*shift,2))+(1/pow(zc,2)));

				return top/bottom;
			}
			
			float3 constrainRGB( float r,  float g,  float b)
			{
				float w;
			    
				w = (0 < r) ? 0 : r;
				w = (w < g) ? w : g;
				w = (w < b) ? w : b;
				w = -w;
			    
				if (w > 0) {
					r += w;  g += w; b += w;
				}
				w = r;
				w = ( w < g) ? g : w;
				w = ( w < b) ? b : w;

				if ( w > 1 )
				{
					r /= w;
					g /= w;
					b /= w;
				}	
				float3 rgb;
				rgb.x = r;
				rgb.y = g;
				rgb.z = b;
				return rgb;

			};	
		                        
           
            
             
             
            //this updates each pixel of the shader                        
            fixed4 frag(fragmentInput i) : SV_Target {
                //this is how far from the middle of the texture (0.5,0.5) the pixel is
                float r = sqrt(pow(i.texcoord0.x-0.5,2) + pow (i.texcoord0.y-0.5,2) );
                
                
                
                //Get initial color 
				float4 data = tex2D (_MainTex, i.uv1).rgba;   
				float UV = tex2D( _UVTex, i.uv1).r;
				float IR = tex2D( _IRTex, i.uv1).r;
	
                
                
                
                
                
                //see if there is a shorter form for this
                //check if the red component is less than 0.5
                if (r>0.2 && r<0.5)//data.x<0.5 )//
                {
	 	            // return fixed4(i.texcoord0.x/r,0.0 ,0.0,1.0);
	 	        	
	 	        	//the doppler shift is dopplerShift(softgamma, costheta)
                	float shift = dopplerShift(_softGamma, (i.texcoord0.x-0.5)/r);    
	 	               
	 	            //this is a test color   
	                float3 xyz = RGBToXYZC(_baseColor.x,_baseColor.y,_baseColor.z);
	                
					float3 weights = weightFromXYZCurves(xyz);
					float3 rParam,gParam,bParam,UVParam,IRParam;
					rParam.x = weights.x; rParam.y = ( float) 615; rParam.z = ( float)8;
					gParam.x = weights.y; gParam.y = ( float) 550; gParam.z = ( float)4;
					bParam.x = weights.z; bParam.y = ( float) 463; bParam.z = ( float)5; 
					
					
					UVParam.x = 0.02; UVParam.y = UV_START + UV_RANGE*UV; UVParam.z = (float)5;
					IRParam.x = 0.02; IRParam.y = IR_START + IR_RANGE*IR; IRParam.z = (float)5;
					
					float xf = pow((1/shift),3)*getXFromCurve(rParam, shift) + getXFromCurve(gParam,shift) + getXFromCurve(bParam,shift) + getXFromCurve(IRParam,shift) + getXFromCurve(UVParam,shift);
					float yf = pow((1/shift),3)*getYFromCurve(rParam, shift) + getYFromCurve(gParam,shift) + getYFromCurve(bParam,shift) + getYFromCurve(IRParam,shift) + getYFromCurve(UVParam,shift);
					float zf = pow((1/shift),3)*getZFromCurve(rParam, shift) + getZFromCurve(gParam,shift) + getZFromCurve(bParam,shift) + getZFromCurve(IRParam,shift) + getZFromCurve(UVParam,shift);
					
					float3 rgbFinal = XYZToRGBC(xf,yf,zf);
					rgbFinal = constrainRGB(rgbFinal.x,rgbFinal.y, rgbFinal.z); //might not be needed

					//Test if unity_Scale is correct, unity occasionally does not give us the correct scale and you will see strange things in vertices,  this is just easy way to test
			  		//float4x4 temp  = mul(unity_Scale.w*_Object2World, _World2Object);
					//float4 temp2 = mul( temp,float4( (float)rgbFinal.x,(float)rgbFinal.y,(float)rgbFinal.z,data.a));
					//return temp2;	
					//float4 temp2 =float4( (float)rgbFinal.x,(float)rgbFinal.y,(float)rgbFinal.z,data.a );
					return float4((float)rgbFinal.x,(float)rgbFinal.y,(float)rgbFinal.z,1.0); //use me for any real build

				} else 
 	            {
 	            	return fixed4(1.0,1.0,1.0,0.0);
 	            }
 	            
		                  
                                    
             //return fixed4(i.texcoord0.xy,0.0,1.0);
            }
            ENDCG
        }
    }
	
} // shader