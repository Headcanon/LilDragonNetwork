using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControl : NetworkBehaviour
{
    GameObject playerHud;
    Rigidbody rb;

    #region Movimento
    [SerializeField] float vel = 10f;
    [SerializeField] float rotacaoVel = 50f;
    #endregion

    #region Tiro
    [SerializeField] GameObject tiroPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float tiroVel = 10f;
    [SerializeField] float tiroDuracao = 2f;
    #endregion

    #region Vida
    [SerializeField] Slider sliderPrefab;
    Slider playerVida;
    [SyncVar] int vida = 100;

    Vector3 initialScale;
    [SerializeField] float objectScale = 1f;

    bool vivo = true;
    #endregion
    
    #region Cor
    [SerializeField] GameObject model;
    [SerializeField] Material red, green, magenta, blue, yellow;

    [SyncVar(hook = "TrocaCor")] public Color corPlayer;
    #endregion

    public string nome;

    [SerializeField] Text textPrefab;
    Text showKills;
    [SyncVar] int killcount = 0;
    GameObject inimigo;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (isLocalPlayer)
        {
            CameraFollow.player = transform;
        }

        playerHud = GameObject.Find("PlayerHud");
        playerVida = Instantiate(sliderPrefab, playerHud.transform);
        showKills = Instantiate(textPrefab, playerHud.transform);

        initialScale = transform.localScale;

        if (isServer)
        {
            CmdCTrocaCor(corPlayer);
        }
        else
        {
            TrocaCor(corPlayer);
        }
    }

    private void OnDestroy()
    {
        if (playerVida != null)
        {
            Destroy(playerVida.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer && vivo)
        {
            float y = Input.GetAxis("Horizontal") * Time.deltaTime * rotacaoVel;
            float z = Input.GetAxis("Vertical") * Time.deltaTime * vel;

            rb.transform.Rotate(0, y, 0);
            rb.transform.Translate(0, 0, z);

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                CmdAtirar();
            }
        }

        showKills.text = killcount.ToString("0");

        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        playerVida.transform.position = pos;
        showKills.transform.position = pos;

        Plane plane = new Plane(Camera.main.transform.forward, Camera.main.transform.position);
        float dist = plane.GetDistanceToPoint(transform.position);

        playerVida.transform.localScale = initialScale * objectScale / dist;
        //showKills.transform.localScale = initialScale * objectScale / dist;
    }

    #region VidaMétodos
    //[Command]
    void CmdAlteraVida(int v)
    {
        //vida = v;
        vida += v;           
        
        playerVida.value = vida;

        if (vida == 0)
        {
            StopCoroutine(this.RespawnCoroutine());
            StartCoroutine(this.RespawnCoroutine());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ICollidable obj = other.gameObject.GetComponent<ICollidable>();

            if (obj != null && vivo)
            {
                CmdAlteraVida(obj.Value());

                inimigo = other.gameObject;
            }
        
    }
    
    
    //void CmdAlteraVida(int v)
    //{
    //    RpcAlteraVida(v);
    //}
    
    //void RpcAlteraVida(int v)
    //{
    //    AlteraVida(v);
    //}

    IEnumerator RespawnCoroutine()
    {
        inimigo.transform.parent.gameObject.GetComponent<PlayerControl>().killcount++;
        Debug.Log("Killconunt: " + inimigo.transform.parent.gameObject.GetComponent<PlayerControl>().killcount);
        vivo = false;
        model.GetComponent<Renderer>().enabled = false;
        playerVida.gameObject.SetActive(false);

        yield return new WaitForSeconds(3f);

        vivo = true;
        model.GetComponent<Renderer>().enabled = true;
        playerVida.gameObject.SetActive(true);
        vida = 100;
        playerVida.value = vida;
    }
    #endregion

    #region TiroMétodos
    void Atirar()
    {
        GameObject bala = Instantiate(tiroPrefab, firePoint.position, firePoint.rotation);
        bala.transform.parent = this.transform;
        bala.GetComponent<Rigidbody>().velocity = firePoint.transform.forward * tiroVel;
        Destroy(bala, tiroDuracao);
    }

    [Command]
    void CmdAtirar()
    {
        RpcAtirar();
    }

    [ClientRpc]
    void RpcAtirar()
    {
        Atirar();
    }
    #endregion

    #region TrocaCorMétodos
    [Command]
    void CmdCTrocaCor(Color c)
    {
        if (c == Color.green)
        {
            model.GetComponent<Renderer>().material = green;
            name = "Verde";
        }
        else if (c == Color.magenta) { model.GetComponent<Renderer>().material = magenta; name = "Magenta"; }
        else if (c == Color.cyan) { model.GetComponent<Renderer>().material = blue; name = "Blue"; }
        else if (c == Color.yellow) { model.GetComponent<Renderer>().material = yellow; name = "Amarelo"; }
        else if (c == Color.red) { model.GetComponent<Renderer>().material = red; name = "Vermelho"; }
    }

    void TrocaCor(Color c)
    {
        if (c == Color.green)
        {
            model.GetComponent<Renderer>().material = green;
            name = "Verde";
        }
        else if (c == Color.magenta) { model.GetComponent<Renderer>().material = magenta; name = "Magenta"; }
        else if (c == Color.cyan) { model.GetComponent<Renderer>().material = blue; name = "Blue"; }
        else if (c == Color.yellow) { model.GetComponent<Renderer>().material = yellow; name = "Amarelo"; }
        else if (c == Color.red) { model.GetComponent<Renderer>().material = red; name = "Vermelho"; }
    }
    #endregion
}
