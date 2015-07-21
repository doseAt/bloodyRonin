using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SamuraiMain : MonoBehaviour 
{
    public int lives = 3;
    public float jumpForce = 100f;

    public LayerMask whatIsGround;
    private Transform groundCheck;
    private float groundRadius = 0.5f;
    public bool grounded = false;

    public float attackTime = 0.5f;                     //koliko dugo traje 1 napad samuraja
    private static float startAttackTime;               //sluzi da se vrati attackTime na inicijalizovanu vrednost
    public bool attackMode = false;                     //bool koji kaze da li je attack u toku!
    public int attackNum;                               //trenutni broj mogucih napada zaredom    
    public int attackNumMax = 7;                        //broj maximalnih napada ZAREDOM   

    private int[] attackHistory = new int[3];           //lista poslednja 3 napada

    private float chargTime = 0.0f;                      //pomocna promenljiva koja kaze  koliko se vec vremena puni attack, resetuje se nakon sto se br napada poveca
    public float unitChargTime = 0.5f;                  //vreme potrebno da se napuni 1 attack samuraja

    private Animator anim;                          
    private SamuraiInput samuraiInput;

    //promenljive za korigovanje pozicije x (zbog trenja)
    private float startPositionX;                        //inicjalna pozicija po x osi                    
    public float toleranceLeftFromStart;                //odstupanje od start x nakon kod deluje sila na desno
    public float tolaranceRightFromStart;               //odstupanje od start x nakon kog deluje sila na levo
    public float toleranceForce;                        //jacina sile koja koriguje playera


    void Awake()
    {
        startAttackTime = attackTime;
        groundCheck = transform.Find("GroundCheck");       
        anim = GetComponent<Animator>();
        samuraiInput=GetComponent<SamuraiInput>();
    }

    void Start()
    {
        attackHistory[0] = 100;
        attackHistory[1] = 100;
        attackHistory[2] = 100;
        attackNum = attackNumMax;
        startPositionX = transform.position.x;
    }

    void Update()
    {
        //Zaustavlja attack mode
        AttackStop();

        //punjenje broja napada
        Charging();

        //Proverava da li je ziv i reaguje ako nije
        IsDead();	
    }

    void FixedUpdate()
    {
        //posto dolazi do pomeranja igraca levo desno zbog jebenog trenja, 
        //koriguje se pozicija x sa vremena na vreme malom silom
        if (transform.position.x < startPositionX - toleranceLeftFromStart)
            rigidbody2D.AddForce(new Vector2(toleranceForce, 0f));

        if (transform.position.x > startPositionX + tolaranceRightFromStart)
            rigidbody2D.AddForce(new Vector2(-toleranceForce, 0f));

        
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        anim.SetBool("Ground", grounded);
        anim.SetFloat("vSpeed", rigidbody2D.velocity.y);
    }

    private void Charging()
    {
        //ako je broj napada samuraja pun, nema potrebe puniti 
        if (attackNum == attackNumMax)
            return;

        //ako se usred punjenja napadne, punjenje se resetuje (anulira)
        else if (attackMode == true)
            chargTime = 0f;

        //punjenje (ako je attackMode off i ako je broj napada manji od maxa)
        else
        {
            chargTime = chargTime + Time.deltaTime;
            if (chargTime >= unitChargTime)
            {
                attackNum = attackNum + 1;
                chargTime = 0;
            }
        }
 
    }

    public void Jump()
    {
            anim.SetBool("Ground", false);
            rigidbody2D.AddForce(new Vector2(0f, jumpForce));
    }

    public void Attack()
    {
        if (attackNum > 0)
        {
            int attackOption;
            samuraiInput.attackEnable = false;

            attackOption = Random.Range(0, 5);
            while (Search(attackOption, attackHistory) == true)
                attackOption = Random.Range(0, 5);

            ShiftArrayAndPutIn(attackOption, attackHistory);
            anim.SetBool("Attack", true);
            anim.SetInteger("AttackOption", attackOption);
            attackMode = true;
            attackNum = attackNum - 1;
        }
    }

    private void AttackStop()
    {
        if (attackMode == true && attackTime >= 0)
        {
            attackTime = attackTime - Time.deltaTime;

            if (attackTime < 0)
            {
                attackMode = false;
                attackTime = startAttackTime;
                anim.SetBool("Attack", false);
                samuraiInput.attackEnable = true;
            }
        }
    }

    private void IsDead()
    {
        if (lives <= 0)
        {
            anim.SetBool("Alive", false);
            samuraiInput.inputEnable = false;
        }
    }

    private bool Search(int x, int[] array)
    {
        for (int i = 0; i < 3; i++)
        {
            if (x == array[i])
                return true;
        }
        return false;
    }

    private void ShiftArrayAndPutIn(int x, int[] array)
    {
        for (int i = 2; i > 0; i--)
        {
            array[i] = array[i - 1];
        }
        array[0] = x;
    }

}
