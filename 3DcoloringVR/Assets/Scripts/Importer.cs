// 외부에서 접근시 아래 함수만 사용할 것
// Importer.importer imp = new Importer.importer();
// imp.Import();
// model_mesh = imp.mesh;

//기능 : 메시 임포트, 필터/렌더러/콜라이더 추가, uv 언래핑, 텍스쳐 지정


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Globalization;

namespace Importer
{
    public class Importer : MonoBehaviour
    {
        FileStream file;
        string file_path;
        string file_name;
       
        public Mesh mesh;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public MeshCollider meshCollider;

        List<Vector3> vertList;
        List<int> triList;
        List<int> uvsList;
        List<Vector2> uvList;

        public Vector2[] uv;

        //이 함수만 쓰임
        public void Import(string path, string name)
        {
            /* 메쉬 오브젝트 생성 */
            meshFilter = GameObject.FindGameObjectWithTag("model").AddComponent<MeshFilter>();
            meshRenderer = GameObject.FindGameObjectWithTag("model").AddComponent<MeshRenderer>();
            meshCollider = GameObject.FindGameObjectWithTag("model").AddComponent<MeshCollider>();
            mesh = new Mesh();
            mesh.Clear();

            /* 메쉬 임포트 */
            file_path = path;
            file_name = name;
            readFile(file, mesh);

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            
            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
            
        }

        // Update is called once per frame
        void Update()
        {
        }
        
        /* file을 로드하여 mesh에 메쉬 생성 */
        void readFile(FileStream file, Mesh mesh)
        {
            vertList = new List<Vector3>();
            triList = new List<int>();
            uvsList = new List<int>();
            uvList = new List<Vector2>();

            vertList.Add(new Vector3(0,0,0));
            uvList.Add(new Vector2(0,0));

            file = new FileStream(file_path + file_name, FileMode.Open, FileAccess.Read);

            StreamReader sr = new StreamReader(file);
            string source; //한 줄 단위로 읽음

            Debug.Log("Reading " + file.Name + ".....");

            while (sr.EndOfStream == false)
            {
                source = sr.ReadLine();

                /* 정점 정보 입력 */
                if (source.StartsWith("v "))
                {
                    string[] v = new string[] { }; //한 점을 이루는 xyz 좌표
                    v = source.Split(' ');

                    vertList.Add(new Vector3(float.Parse(v[1]), float.Parse(v[2]), float.Parse(v[3])));
                }
                /* 면 정보 입력 */
                else if (source.StartsWith("f "))
                {
                    string[] f = new string[] { }; //한 면을 이루는 정점 인덱스 3~4개
                    f = source.Split(' ');

                    if (f.Length == 4)
                    {
                        triList.Add(System.Convert.ToInt32(f[1].Split('/')[0]));
                        uvsList.Add(System.Convert.ToInt32(f[1].Split('/')[1]));

                        triList.Add(System.Convert.ToInt32(f[2].Split('/')[0]));
                        uvsList.Add(System.Convert.ToInt32(f[2].Split('/')[1]));

                        triList.Add(System.Convert.ToInt32(f[3].Split('/')[0]));
                        uvsList.Add(System.Convert.ToInt32(f[3].Split('/')[1]));
                    }
                    else if (f.Length == 5)
                    {//123 134

                        triList.Add(System.Convert.ToInt32(f[1].Split('/')[0]));
                        uvsList.Add(System.Convert.ToInt32(f[1].Split('/')[1]));

                        triList.Add(System.Convert.ToInt32(f[2].Split('/')[0]));
                        uvsList.Add(System.Convert.ToInt32(f[2].Split('/')[1]));

                        triList.Add(System.Convert.ToInt32(f[3].Split('/')[0]));
                        uvsList.Add(System.Convert.ToInt32(f[3].Split('/')[1]));


                        triList.Add(System.Convert.ToInt32(f[1].Split('/')[0]));
                        uvsList.Add(System.Convert.ToInt32(f[1].Split('/')[1]));

                        triList.Add(System.Convert.ToInt32(f[3].Split('/')[0]));
                        uvsList.Add(System.Convert.ToInt32(f[3].Split('/')[1]));
                        
                        triList.Add(System.Convert.ToInt32(f[4].Split('/')[0]));
                        uvsList.Add(System.Convert.ToInt32(f[4].Split('/')[1]));
                    }
                }

                else if (source.StartsWith("vt "))
                {
                    string[] uv = new string[] { };
                    uv = source.Split(' ');

                    uvList.Add(new Vector2(float.Parse(uv[1]), float.Parse(uv[2])));
                }
            }

            Vector3[] a = vertList.ToArray();
            Vector2[] c = uvList.ToArray();
            int[] b = triList.ToArray();
            int[] d = uvsList.ToArray();

            Vector3[] temp_vert = new Vector3[b.Length];
            Vector2[] temp_uv = new Vector2[b.Length];

            for (int i = 0; i < temp_vert.Length; i++)
            {
                temp_vert[i] = a[b[i]];
            }
            mesh.vertices = temp_vert;


            for (int i = 0; i < temp_uv.Length; i++)
            {
                temp_uv[i] = c[d[i]];
            }
            uv = temp_uv;

            for (int i= 0; i<b.Length; i++)
            {
                b[i] = i;
            }
            mesh.triangles = b;
            
            Debug.Log(mesh.triangles.Length);
            Debug.Log(mesh.vertices.Length);
            Debug.Log(mesh.uv.Length);
        }
    }
}