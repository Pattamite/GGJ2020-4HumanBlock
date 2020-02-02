using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDetector : MonoBehaviour {
    public SfxPlayer sfxPlayer;
    public delegate void RepairSuccessAction ();
    public static event RepairSuccessAction OnRepairsuccess;

    [SerializeField] float errorThreshold = 10;
    [SerializeField] public float maxWaitTime = 1.5f;
    [SerializeField] public float maxStartEmissionDelay = 1.5f;
    [SerializeField] public Color startEmissionColor = new Color (0.047f, 0.365f, 1.0f);
    [SerializeField] public Color defaultEmissionColor = new Color (1.0f, 0.847f, 0.545f);
    [SerializeField] public Color correctEmissionColor = new Color (0.439f, 1.0f, 0.259f);
    [SerializeField] public float emisissionMultiplier = 2.0f;

    private Collider detectorCollider;
    private string expectedBlockName;
    private List<Block> blockInDetector;
    private Material mat;
    private bool isCorrectedFlag = false;
    private float waitTime = -1f;
    private float startItemEmissionDelay = -1.0f;
    private int maxSplit = 4;
    private Color targetEmissionColor;

    void Awake () {
        mat = GetComponent<Renderer> ().material;
        waitTime = maxWaitTime;
        startItemEmissionDelay = maxStartEmissionDelay;
        isCorrectedFlag = false;
        targetEmissionColor = startEmissionColor;
    }

    // Start is called before the first frame update
    void Start () {

        blockInDetector = new List<Block> ();

        detectorCollider = GetComponent<Collider> ();

    }

    // Update is called once per frame
    void Update () {
        if (isCorrectedFlag) {
            // Delay for something
            if (waitTime <= 0.0f) {
                isCorrectedFlag = false;
                waitTime = maxWaitTime;
                OnRepairsuccess ();
                this.RepairSuccess ();
            } else {
                waitTime -= Time.deltaTime;;
            }
        } else {
            // Debug.Log ("Start Emission Delay: " + startItemEmissionDelay);
            if (startItemEmissionDelay > 0.0f) {
                startItemEmissionDelay -= Time.deltaTime;
                if (targetEmissionColor != startEmissionColor) {
                    targetEmissionColor = startEmissionColor;
                    updateEmission (blockInDetector.Count);
                }
            } else {
                targetEmissionColor = defaultEmissionColor;
                updateEmission (blockInDetector.Count);
            }
            if (blockInDetector.Count >= maxSplit) {
                if (checkCombineBlock (expectedBlockName)) {
                    sfxPlayer.PlaySfxClip (SfxItem.Block_EnterCorrect);
                    if (OnRepairsuccess != null) {
                        isCorrectedFlag = true;
                        waitTime = maxWaitTime;
                        startItemEmissionDelay = maxStartEmissionDelay;
                        mat.SetColor ("_EmissionColor", correctEmissionColor * 2.0f);
                    }
                }
            }
        }
    }

    void updateEmission (int blockCount) {
        float emissionLvl = Mathf.Max (blockCount + 1, maxSplit) / (maxSplit + 1.0f) * emisissionMultiplier;
        mat.SetColor ("_EmissionColor", targetEmissionColor * emissionLvl);
    }

    public void SetExpectedBlockName (string blockName) {
        expectedBlockName = blockName;
    }

    void OnTriggerEnter (Collider other) {
        Block otherBlock = other.gameObject.GetComponent<Block> ();

        Debug.Log ("Count - " + blockInDetector.Count);

        if (otherBlock == null)
            return;

        blockInDetector.Add (otherBlock);

        if (blockInDetector.Count == 1) {
            sfxPlayer.PlaySfxClip (SfxItem.Block_Enter1);
        } else if (blockInDetector.Count == 2) {
            sfxPlayer.PlaySfxClip (SfxItem.Block_Enter2);
        } else if (blockInDetector.Count == 3) {
            sfxPlayer.PlaySfxClip (SfxItem.Block_Enter3);
        } else if (blockInDetector.Count >= maxSplit) {
            if (checkCombineBlock (expectedBlockName)) {
                sfxPlayer.PlaySfxClip (SfxItem.Block_EnterCorrect);
                if (OnRepairsuccess != null) {
                    OnRepairsuccess ();
                    this.RepairSuccess ();
                }
            } else {
                sfxPlayer.PlaySfxClip (SfxItem.Block_EnterIncorrect);
            }
        }

        updateEmission (blockInDetector.Count);
    }

    void OnTriggerExit (Collider other) {

        Block otherBlock = other.gameObject.GetComponent<Block> ();

        blockInDetector.Remove (otherBlock);

        updateEmission (blockInDetector.Count);

    }

    bool checkCombineBlock (string requiredBlockId) {

        foreach (Block block in blockInDetector) {
            print (block.getBlockName ());
            if (block.getBlockName () != requiredBlockId || (!block.isCorrectOrientation ()))
                return false;
        }

        return true;

    }

    void RepairSuccess () {
        foreach (Block block in blockInDetector) {
            Destroy (block.gameObject);
        }

        blockInDetector.Clear ();
        updateEmission (blockInDetector.Count);
    }

}