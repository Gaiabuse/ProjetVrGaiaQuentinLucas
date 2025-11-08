using System;
using UnityEngine;
using UnityEngine.XR;
using System.Collections;

public class PlushManager : MonoBehaviour
{
    public float distanceBeforeBreak = 5f;

    [SerializeField] private float timeBeforeRespawn = 5f;
    [SerializeField] private GameObject[] members;
    [SerializeField] private GameObject[] cubesRight;
    [SerializeField] private GameObject[] cubesLeft;
    [SerializeField] private float stretchMultiplier = 1f;
    [Range(-1f, 1f)][SerializeField] private float angleForSwitchMembersY = 0.60f;
    [Range(-1f, 1f)][SerializeField] private float angleForSwitchMembersX = 0.60f;

    private bool _rightHandGrab;
    private bool _leftHandGrab;
    private Vector3 _vPointDirectionR;
    private Vector3 _vPointDirectionL;
    

    public void ChangeScale(int memberId, float distance, bool isRight)
    {
        if (members[memberId].activeSelf)
        {
            if (isRight)
            {
                _rightHandGrab = true;
            }
            else
            {
                _leftHandGrab = true;
            }
            memberId = Mathf.Clamp(memberId, 0, members.Length);
            members[memberId].transform.localScale = new Vector3( 1,1 + distance * stretchMultiplier ,1);
        }
    }

    public void Rotation(int memberId, Transform cubeTransform) // ça sert a rien a faire après
    {
        memberId = Mathf.Clamp(memberId, 0, members.Length);
    }

	public void DestroyMember(int memberId)
	{
        memberId = Mathf.Clamp(memberId, 0, members.Length);
        members[memberId].SetActive(false);
        StartCoroutine(RespawnMember(memberId));
    }

    IEnumerator RespawnMember(int memberId)
    {
        yield return new WaitForSeconds(timeBeforeRespawn);
        members[memberId].SetActive(true);
    }


    public void Reset(int memberId, bool isRight)
    {
        if (members[memberId].activeSelf)
        {
            memberId = Mathf.Clamp(memberId, 0, members.Length);
            members[memberId].transform.localScale = Vector3.one;
            if (isRight)
            {
                _rightHandGrab = false;
            }
            else
            {
                _leftHandGrab = false;
            }
        }
    }
    private void Update()
    {
        InputDevice handRDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        InputDevice handLDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        
        if (!_rightHandGrab)
        {
            handRDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotR);
            _vPointDirectionR = rotR * Vector3.forward;
            _vPointDirectionR = transform.TransformDirection(_vPointDirectionR);
       
            if (_vPointDirectionR.y < angleForSwitchMembersY && -_vPointDirectionR.x > angleForSwitchMembersX && !cubesRight[0].activeInHierarchy)
            {
                cubesRight[0].SetActive(true);
                cubesRight[1].SetActive(false);
            }
            else if(_vPointDirectionR.y > angleForSwitchMembersY &&-_vPointDirectionR.x < angleForSwitchMembersX && !cubesRight[1].activeInHierarchy)
            {
                cubesRight[0].SetActive(false);
                cubesRight[1].SetActive(true);
            }
        }

        if (!_leftHandGrab)
        {
            handLDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotL);
            _vPointDirectionL = rotL * Vector3.forward;
            _vPointDirectionL = transform.TransformDirection(_vPointDirectionL);
            if (_vPointDirectionL.y < angleForSwitchMembersY && _vPointDirectionL.x > angleForSwitchMembersX && !cubesLeft[0].activeInHierarchy)
            {
                cubesLeft[0].SetActive(true);
                cubesLeft[1].SetActive(false); 
            }
            else if(_vPointDirectionL.y > angleForSwitchMembersY && _vPointDirectionL.x < angleForSwitchMembersX && !cubesLeft[1].activeInHierarchy)
            {
                cubesLeft[0].SetActive(false);
                cubesLeft[1].SetActive(true);
            }
        }

       
    }
}
