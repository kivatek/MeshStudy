using UnityEngine;
using System.Collections;

public class BgImage : MonoBehaviour {

	MeshFilter mf;
	
	int waveCount;
	int waveTableSize;
	float[]	waveTable;
	
	static int MAX_HTILE = 16;
	static int MAX_VTILE = 9;
	static int MAX_HINDEX = MAX_HTILE+1;
	static int MAX_VINDEX = MAX_VTILE+1;
	
	// Use this for initialization
	void Start () {
		int halfLength = (MAX_HTILE-2) * 2;
		waveTableSize = halfLength * 2;
		float step = Mathf.PI / halfLength;
		float[] table = new float[waveTableSize];
		float radian = 0;
		for (int count = 0; count < halfLength; count++) {
			table[count] = Mathf.Cos(radian);
			radian += step;
		}
		for (int count = 0; count < halfLength; count++) {
			table[count+halfLength] = Mathf.Cos(radian);
			radian -= step;
		}
		waveTable = table;
		
		mf = GetComponent<MeshFilter>();
		mf.mesh = new Mesh();
		Generate();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Wave();
		waveCount = (waveCount + 1) % waveTableSize;
	}
	
	void Generate() {
		Vector3[] vertices = new Vector3[MAX_HINDEX*MAX_VINDEX];
		for (int vi = 0; vi < MAX_VINDEX; vi++) {
			for (int hi = 0; hi < MAX_HINDEX; hi++) {
				float x = (float)hi - (float)(MAX_HINDEX-1)*0.5f;
				float y = (float)vi - (float)(MAX_VINDEX-1)*0.5f;
				vertices[vi*MAX_HINDEX+hi] = new Vector3(x, -y, 0);
			}
		}
		mf.mesh.vertices = vertices;

		int[] tri = new int[MAX_HTILE*MAX_VTILE*2*3];
		for (int index = 0; index < MAX_HTILE*MAX_VTILE; index++) {
			int upperleft =  MAX_HINDEX * (index/MAX_HTILE) + index % MAX_HTILE;
			tri[index * 6 + 0] = upperleft + 0;
			tri[index * 6 + 1] = upperleft + 1;
			tri[index * 6 + 2] = upperleft + MAX_HINDEX + 0;
			tri[index * 6 + 3] = upperleft + 1;
			tri[index * 6 + 4] = upperleft + MAX_HINDEX + 1;
			tri[index * 6 + 5] = upperleft + MAX_HINDEX + 0;
		}
		mf.mesh.triangles = tri;

		Vector3[] normals = new Vector3[MAX_HINDEX*MAX_VINDEX];
		for (int vi = 0; vi < MAX_VINDEX; vi++) {
			for (int hi = 0; hi < MAX_HINDEX; hi++) {
				normals[vi*MAX_HINDEX+hi] = -Vector3.forward;
			}
		}
		mf.mesh.normals = normals;
		
		// uvは左下原点で指定。
		Vector2[] uv = new Vector2[MAX_HINDEX*MAX_VINDEX];
		for (int vi = 0; vi < MAX_VINDEX; vi++) {
			for (int hi = 0; hi < MAX_HINDEX; hi++) {
				uv[vi*MAX_HINDEX+hi] = new Vector2((1.0f)/MAX_HTILE*hi, 1.0f-(1.0f)/MAX_VTILE*vi);
			}
		}
		mf.mesh.uv = uv;
	}
	
	/// <summary>
	/// 揺らぎ頂点生成.
	/// </summary>
	void Wave() {
		Vector3[] vertices = new Vector3[MAX_HINDEX*MAX_VINDEX];
		for (int vi = 0; vi < MAX_VINDEX; vi++) {
			int baseOffset = (waveCount + vi) % waveTableSize;
			for (int hi = 0; hi < MAX_HINDEX; hi++) {
				float x = (float)hi - (float)MAX_HTILE*0.5f;
				float y = (float)vi - (float)MAX_VTILE*0.5f;
				if ((vi == 0) || (vi == MAX_VINDEX-1) || (hi == 0) || (hi == MAX_HINDEX-1)) {
					// 最外周はゆらぎ処理の対象外.
				} else {
					float rate = 0.5f;
					int waveOffset = (baseOffset + hi) % waveTableSize;
					float wave = waveTable[waveOffset] * rate;
					x += wave;
				}
				vertices[vi*MAX_HINDEX+hi] = new Vector3(x, -y, 0);
			}
		}
		mf.mesh.vertices = vertices;
		
	}
}
