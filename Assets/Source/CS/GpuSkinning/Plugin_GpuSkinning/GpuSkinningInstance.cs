﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;



public class GpuSkinningInstance : MonoBehaviour {

    //public TextAsset textAsset;

    private Material _material;
	public Material AnimMaterial
	{
		get { return _material; }
		set { 
				if (_material != value)
				{
					_material = value; 
					refreshInstance();
				}
			}
	}

	public GpuSkinningAnimData anim_data;

	void Awake() 
	{
		_material = GetComponent<MeshRenderer>().material;
	}

	// Use this for initialization
	void Start () 
	{
		refreshInstance();
	}

	void refreshInstance()
    {
        if (anim_data == null)
        {
            return;
        }


        //Texture2D anim_tex = new Texture2D(anim_data.texWidth, anim_data.texHeight, TextureFormat.RGBA32, false, false);
        //anim_tex.filterMode = FilterMode.Point;
        //anim_tex.LoadRawTextureData(anim_data.texBytes);
        //anim_tex.Apply(false);

        _material.SetInt("_BoneNum", anim_data.totalBoneNum);
        //_material.SetTexture("_AnimationTex", anim_tex);
        _material.SetVector("_AnimationTexSize", new Vector4(anim_data.texWidth, anim_data.texHeight, 0, 0));
	}

    // Update is called once per frame
    int testFrame = 0;
    float testDuration = 0.03f;
    float testTimer = 0;
    float speed = 0.05f;
	void Update ()
	{
        if (_material != null)
        {
            testTimer += Time.deltaTime;
            if (testTimer >= testDuration/speed)
            {
                testFrame++;
                if(testFrame >= anim_data.totalFrame-1)
                {
                    testFrame = 0;
                }
                testTimer -= testDuration;

                _material.SetInt("_FrameIndex", testFrame);
            }
        }
    }
}