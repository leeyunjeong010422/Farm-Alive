using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] List<ParticleSystem[]> _particleSystem;

    [SerializeField] SectionManager _sectionManager;

    private ParticleSystem[] _particles;

    // TODO : AWAKE . ADDLISTENER -> SECTIONMOVER
    //        ONDESTROY . REMOVELISTENER

    // ONSECTIONCHANGED -> 현재 파티클 시스템에 할당

    // 물은 색상 STARTCOLOR 하얗게 변환 후 재생 코루틴
    // 비료는 초록색 ,
}
