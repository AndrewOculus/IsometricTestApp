using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	private const int GridSize = 1;
	private const int GridCount = 10;
	private const string CubeTag = "CUBE";
	private Camera _view;
	private bool[,] _field;
	private GameObject _activeGameObject = null;

	public GameObject box;

	void Start () {
		_field = new bool[GridCount, GridCount];
		_view = GameObject.FindObjectOfType<Camera> ();
		MakeGrid ();
		MakeCubes ();
	}
	

	void Update () {
		if (Input.GetKey (KeyCode.Mouse0))
			MakeRaycast ();
		else {
			DisactivateGameObject ();
		}
	}

	void MakeRaycast () {
		RaycastHit hit;
		Ray ray = _view.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit)) {
			if (!_activeGameObject) {
				ActivateGameObject (hit);
			} else {
				if (hit.collider.tag != CubeTag) {
					_activeGameObject.transform.position = GetGridCell(hit.point);
				}
			}
		}
	}

	Vector3 GetGridCell(Vector3 point){
		var x = (int)(point.x / GridSize);
		var z = (int)(point.z / GridSize);

		if (x <= GridCount && z <= GridCount) {
			if (!_field [x, z]) {
				point.Set ((int)x + GridSize/2.0f, 0.5f, (int)z + GridSize/2.0f);
				_field [x, z] = true;

				var oldX = (int)(_activeGameObject.transform.position.x / GridSize);
				var oldZ = (int)(_activeGameObject.transform.position.z / GridSize);
				_field [oldX, oldZ] = false;

				return point;
			} 
		}
		return _activeGameObject.transform.position;
	}

	void ActivateGameObject (RaycastHit hit) {
		if (hit.collider.tag == CubeTag) {
			_activeGameObject = hit.collider.gameObject;
			_activeGameObject.GetComponent<BoxCollider> ().enabled = false;
		}
	}

	void DisactivateGameObject () {
		if (_activeGameObject != null) {
			_activeGameObject.GetComponent<BoxCollider> ().enabled = true;
			_activeGameObject = null;
		}
	}
		
	void MakeCubes(){
		for (int i = 1; i < 5; i++) {
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = new Vector3(i - GridSize/2.0f, 0.5f, i - GridSize/2.0f);
			cube.tag = CubeTag;
			_field [i-1, i-1] = true;
		}
	}

	void MakeGrid(){
		for (int x = 0; x < GridCount+1; x+=GridSize) {
			MakePln (string.Format ("grid x= {0}", x), new Vector3 (0, 0.02f, x),new Vector2(GridCount*GridSize , 0.02f));
		}

		for (int y = 0; y < GridCount+1; y+=GridSize) {
			MakePln (string.Format ("grid y= {0}", y), new Vector3 (y, 0.02f, 0),new Vector2(0.02f, GridCount*GridSize));
		}
	}

	void MakePln(string name, Vector3 pos , Vector2 size){
		GameObject plane = new GameObject(name);
		MeshFilter meshFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));
		meshFilter.mesh = CreateMesh(size.x, size.y);
		MeshRenderer renderer = plane.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		renderer.material.shader = Shader.Find ("Legacy Shaders/Diffuse");
		Texture2D tex = new Texture2D(1, 1);
		tex.SetPixel(0, 0, Color.blue);
		tex.Apply();
		renderer.material.mainTexture = tex;
		renderer.material.color = Color.white;
		plane.transform.position = pos;
	}

	Mesh CreateMesh(float width, float height)
	{
		Mesh m = new Mesh();
		m.name = "ScriptedMesh";
		m.vertices = new Vector3[] {
			new Vector3(0, 0.01f ,0),
			new Vector3(width, 0.01f, 0),
			new Vector3(width, 0.01f, height),
			new Vector3(0,0.01f, height)
		};
		m.uv = new Vector2[] {
			new Vector2 (0, 0),
			new Vector2 (0, 1),
			new Vector2(1, 1),
			new Vector2 (1, 0)
		};
		m.triangles = new int[] { 3, 2, 0, 2, 1, 0}; //{ 0, 1, 2, 0, 2, 3};
		m.RecalculateNormals();

		return m;
	}
}
