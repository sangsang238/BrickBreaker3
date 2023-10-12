using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    [SerializeField] private GameObject sphere;
    private Renderer sphereRenderer;

    public Toggle toggleParticle;
    public ParticleSystem efFallingLeaves;
    private bool isParticleActive;

    private MongoDBManager manager = new MongoDBManager();
    public static ModelPlayer mdplayer = null;

    public AudioMixer audioMixer;

    private void Start()
    {
        sphereRenderer = sphere.GetComponent<Renderer>();
        sphereRenderer.material.SetColor("_Color", new Color32(172, 130, 218, 255));

        if (mdplayer != null)
        {
            Debug.Log(mdplayer.Username);
            if (manager.GetPlayerColor(mdplayer.Username) == 1)
                sphereRenderer.material.SetColor("_Color", new Color32(172, 130, 218, 255));
            else if (manager.GetPlayerColor(mdplayer.Username) == 2)
                sphereRenderer.material.SetColor("_Color", new Color32(135, 206, 235, 255));
            else
                sphereRenderer.material.SetColor("_Color", new Color32(204, 173, 184, 255));
        }

        isParticleActive = efFallingLeaves.isPlaying;
        toggleParticle.isOn = isParticleActive;
    }
    private void Update()
    {
        //if (mdplayer != null)
        //    manager.SavePlayerColor(mdplayer.Username, GameManager.colorChoosen);
    }

    public void ChangeSphereColor1()
    {
        sphereRenderer.material.SetColor("_Color", new Color32(172, 130, 218, 255));
        GameManager.colorChoosen = 1;
        if (mdplayer != null)
            manager.SavePlayerColor(mdplayer.Username, 1);
    }
    public void ChangeSphereColor2()
    {
        sphereRenderer.material.SetColor("_Color", new Color32(135, 206, 235, 255));
        GameManager.colorChoosen = 2;
        if (mdplayer != null)
            manager.SavePlayerColor(mdplayer.Username, 2);
    }
    public void ChangeSphereColor3()
    {
        sphereRenderer.material.SetColor("_Color", new Color32(204, 173, 184, 255));
        GameManager.colorChoosen = 3;
        if (mdplayer != null)
            manager.SavePlayerColor(mdplayer.Username, 3);
    }



    
    public void SetFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
    }

    public void OnToggleValueChanged()
    {
        if (toggleParticle.isOn)
        {
            efFallingLeaves.Play();
        }
        else
        {
            efFallingLeaves.Stop();
        }

        isParticleActive = toggleParticle.isOn;
    }
}
